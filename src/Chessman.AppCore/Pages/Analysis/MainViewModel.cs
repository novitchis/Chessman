using Chessman.Services;
using Framework.MVVM;
using Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;


namespace Chessman.ViewModel
{
    public class MainViewModel : BoardPageViewModel, INavigationAware
    {
        private IAppSettings appSettings = null;
        private ITextReaderService textService = null;

        public AnalysisViewModel AnalysisViewModel { get; set; }

        public ICommand PracticePositionCommand
        {
            get { return new RelayCommand(PracticePositionExecuted); }
        }

        public MainViewModel(
            INavigationService navigationService, 
            IAnalysisBoardService analysisBoardService, 
            ITextReaderService textService, 
            IAppSettings appSettings)
            : base(navigationService, analysisBoardService)
        {
            this.appSettings = appSettings;
            this.textService = textService;
            AnalysisViewModel = ViewModelLocator.IOCContainer.Resolve<AnalysisViewModel>();
            BoardViewModel = new AnalysisChessBoardViewModel(analysisBoardService);

            Messenger.Default.Register<GenericMessage<bool>>(this, NotificationMessages.AnalysisIsOnChanged, OnAnalysisIsOnChangedMessage);
        }

        private void OnAnalysisIsOnChangedMessage(GenericMessage<bool> message)
        {
            if (!message.Content)
                BoardViewModel.SuggestedMove = null;
        }

        public override void OnNavigatedTo(object parameter)
        {
            InitSettings();

            AnalysisViewModel.SubscribeToAnalysis();
            if (!firstNavigatedToOcurred && parameter == null)
                parameter = AppPersistenceManager.GetSavedPosition(appSettings, AppPersistenceManager.SavedAnalysisPositionPgnKey);

            base.OnNavigatedTo(parameter);

            if (AnalysisViewModel.IsEngineOn)
                boardService.Start();
        }

        private void InitSettings()
        {
            BoardViewModel.ShowSuggestedMoveArrow = (bool)appSettings.Values[AppPersistenceManager.ShowBestMoveArrowKey];
            BoardViewModel.ShowLegalMoves = (bool)appSettings.Values[AppPersistenceManager.ShowLegalMovesKey];
            BoardViewModel.PlaySounds = (bool)appSettings.Values[AppPersistenceManager.EnableMoveSoundsKey];
            AnalysisViewModel.SetAnalysisLines((int)appSettings.Values[AppPersistenceManager.MultipleLinesKey]);

            bool useFigurineNotation = (int)appSettings.Values[AppPersistenceManager.NotationTypeKey] == (int)NotationType.Figurines;
            AnalysisViewModel.UseFigurineNotation = useFigurineNotation;
            NotationViewModel.UseFigurineNotation = useFigurineNotation;
        }

        public override void OnNavigatingFrom()
        {
            if (AnalysisViewModel.IsEngineOn)
                boardService.Stop();

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
                string text = await textService.ReadText(file);
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

                await FileIO.WriteTextAsync(file, boardService.Serialize(serializationType, false));
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
