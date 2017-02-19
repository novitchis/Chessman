using Framework.MVVM;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;

namespace ChessEngineClient.ViewModel
{
    public class MainViewModel : BoardPageViewModel, INavigationAware
    {
        private IAppSettings appSettings = null;

        public AnalysisViewModel AnalysisViewModel { get; set; }

        public ICommand PracticePositionCommand
        {
            get { return new RelayCommand(PracticePositionExecuted); }
        }

        public MainViewModel(INavigationService navigationService, IEngineBoardService analysisBoardService, IAppSettings appSettings)
            : base(navigationService, analysisBoardService)
        {
            this.appSettings = appSettings;
            AnalysisViewModel = ViewModelLocator.IOCContainer.Resolve<AnalysisViewModel>();
            BoardViewModel = new AnalysisChessBoardViewModel(analysisBoardService);

            // TODO: just until we fix the crash on start
            useInitializationDelay = true;
        }

        public override void OnNavigatedTo(object parameter)
        {
            InitSettings();

            AnalysisViewModel.SubscribeToAnalysis();
            base.OnNavigatedTo(parameter);
        }

        private void InitSettings()
        {
            BoardViewModel.ShowSuggestedMoveArrow = (bool)appSettings.Values[AppSettingsKeys.ShowBestMoveArrowKey];
        }

        public override void OnNavigatingFrom()
        {
            base.OnNavigatingFrom();
            AnalysisViewModel.UnsubscribeToAnalysis();
        }

        private void PracticePositionExecuted(object obj)
        {
            NavigationService.NavigateTo(ViewModelLocator.PracticePageNavigationName, GetPositionLoadOptions(BoardSerializationType.PGN));
        }

        public ICommand LoadPGNCommand
        {
            get { return new RelayCommand(LoadPGNExecuted); }
        }

        public ICommand SavePGNCommand
        {
            get { return new RelayCommand(SavePGNExecuted); }
        }

        public ICommand LoadFromClipboardCommand
        {
            get { return new RelayCommand(LoadFromClipboardExecuted); }
        }

        private async void LoadPGNExecuted(object obj)
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.Downloads
            };
            picker.FileTypeFilter.Add(".pgn");
            picker.FileTypeFilter.Add(".fen");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                string text = await ReadFile(file);
                string fileType = file.FileType;
                if (!fileType.ToLower().Equals(".pgn") && !fileType.ToLower().Equals(".fen"))
                {
                    var dialog = new MessageDialog($"Unsupported file format {fileType}.\nPlease provide a .pgn or .fen file!");
                    dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });

                    await dialog.ShowAsync();
                    return;
                }
                LoadFrom(text);
            }
        }

        private static async Task<string> ReadFile(StorageFile file)
        {
            using (var stream = await file.OpenAsync(FileAccessMode.Read))
            using (var inputStream = stream.GetInputStreamAt(0))
            using (var dataReader = new DataReader(inputStream))
            {
                uint numBytesLoaded = await dataReader.LoadAsync((uint)stream.Size);
                byte[] fileBytes = new byte[numBytesLoaded];
                dataReader.ReadBytes(fileBytes);

                Encoding fileEncoding = DetectEncoding(fileBytes);
                int textOffset = GetTextOffset(fileBytes, fileEncoding);

                return fileEncoding.GetString(fileBytes, textOffset, fileBytes.Length - textOffset);
            }
        }

        private static Encoding DetectEncoding(byte[] fileBytes)
        {
            Encoding result = Encoding.UTF8;

            Ude.CharsetDetector detector = new Ude.CharsetDetector();
            detector.Feed(fileBytes, 0, fileBytes.Length);
            detector.DataEnd();
            if (detector.Charset != null)
                result = Encoding.GetEncoding(detector.Charset);

            return result;
        }

        private static int GetTextOffset(byte[] fileBytes, Encoding fileEncoding)
        {
            int result = 0;
            foreach (var item in fileEncoding.GetPreamble())
            {
                if (fileBytes[result] == item)
                    result++;
                else
                    break;
            }
            return result;
        }

        private async void SavePGNExecuted(object obj)
        {
            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "My Game"
            };
            savePicker.FileTypeChoices.Add("PGN File", new List<string>() { ".pgn" });
            savePicker.FileTypeChoices.Add("FEN File", new List<string>() { ".fen" });

            var file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                BoardSerializationType serializationType = BoardSerializationType.PGN;

                string fileType = file.FileType;
                if (fileType.ToLower().Equals(".pgn"))
                    serializationType = BoardSerializationType.PGN;
                else if (fileType.ToLower().Equals(".fen"))
                    serializationType = BoardSerializationType.FEN;
                else
                {
                    var dialog = new MessageDialog(String.Format("Unsupported file format {0}.\nPlease provide a .pgn or .fen file!" + fileType));
                    dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });

                    await dialog.ShowAsync();
                    return;
                }

                await FileIO.WriteTextAsync(file, boardService.Serialize(serializationType));
            }
        }

        private async void LoadFromClipboardExecuted(object obj)
        {
            var clipboardData = Clipboard.GetContent();
            string clipboardText = "Invalid PGN";

            if (clipboardData.Contains("Text"))
                clipboardText = await clipboardData.GetTextAsync();

            LoadFrom(clipboardText);
        }

        private async void LoadFrom(string data)
        {
            if (!boardService.LoadFrom(data))
            {
                var dialog = new MessageDialog("Could not load PGN/FEN. The content is invalid or not supported.");
                dialog.Commands.Add(new UICommand { Label = "OK", Id = 0 });
                await dialog.ShowAsync();
                return;
            }

            BoardViewModel.RefreshSquares();
            NotationViewModel.ReloadMoves();
        }
    }
}
