using ChessEngine;
using Framework.MVVM;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Popups;

namespace ChessEngineClient.ViewModel
{
    public class MainViewModel : BoardPageViewModel, INavigationAware
    {
        public AnalysisViewModel AnalysisViewModel { get; set; }

        public ICommand PracticePositionCommand
        {
            get { return new RelayCommand(PracticePositionExecuted); }
        }

        public MainViewModel(INavigationService navigationService, IEngineBoardService analysisBoardService)
            : base(navigationService, analysisBoardService)
        {
            AnalysisViewModel = ViewModelLocator.IOCContainer.Resolve<AnalysisViewModel>();
            BoardViewModel = new AnalysisChessBoardViewModel(analysisBoardService);

            // TODO: just until we fix the crash on start
            useInitializationDelay = true;
        }

        public override void OnNavigatedTo(object parameter)
        {
            AnalysisViewModel.SubscribeToAnalysis();
            base.OnNavigatedTo(parameter);
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
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".pgn");
            picker.FileTypeFilter.Add(".fen");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // read to end //
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                string data;
                using (var inputStream = stream.GetInputStreamAt(0))
                {
                    using (var dataReader = new Windows.Storage.Streams.DataReader(inputStream))
                    {
                        uint numBytesLoaded = await dataReader.LoadAsync((uint)stream.Size);
                        data = dataReader.ReadString(numBytesLoaded);
                    }
                }
                stream.Dispose(); // Or use the stream variable (see previous code snippet) with a using statement as well.
                string fileType = file.FileType;
                int serializationType = BoardSerialization.BS_PGN;
                if (!fileType.ToLower().Equals(".pgn") && !fileType.ToLower().Equals(".fen"))
                {
                    var dialog = new MessageDialog(String.Format("Unsupported file format {0}.\nPlease provide a .pgn or .fen file!" + fileType));
                    dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });

                    await dialog.ShowAsync();
                    return;
                }
                LoadFrom(data);
            }
        }

        private async void SavePGNExecuted(object obj)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("PGN File", new List<string>() { ".pgn" });
            savePicker.FileTypeChoices.Add("FEN File", new List<string>() { ".fen" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "My Game";
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

                await Windows.Storage.FileIO.WriteTextAsync(file, boardService.Serialize(serializationType));
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
