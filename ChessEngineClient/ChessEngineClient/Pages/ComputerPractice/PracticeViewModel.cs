using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation.Collections;

namespace ChessEngineClient.ViewModel
{
    public class PracticeViewModel : BoardPageViewModel, INavigationAware
    {
        private IPracticeBoardService practiceBoardService = null;
        private IAppSettings appSettings = null;

        public ICommand SwitchColor
        {
            get { return new RelayCommand(SwitchColorExecuted); }
        }

        public ICommand AnalyseGameCommand
        {
            get { return new RelayCommand(AnalyseGameExecuted); }
        }

        public SideColor UserColor
        {
            get { return practiceBoardService.UserColor; }
            set
            {
                if (UserColor != value)
                {
                    practiceBoardService.SwitchUserColor();
                    NotifyPropertyChanged();
                }
            }
        }

        public PracticeViewModel(
            INavigationService navigationService, 
            IPracticeBoardService practiceBoardService,
            IAppSettings appSettings)
            : base(navigationService, practiceBoardService)
        {
            this.practiceBoardService = practiceBoardService;
            this.appSettings = appSettings;
            BoardViewModel = new PracticeBoardViewModel(practiceBoardService);

            AppPersistenceManager.RestoreBoardPosition(appSettings, this);
        }

        public override void OnNavigatedTo(object parameter)
        {
            InitiSettings();
            base.OnNavigatedTo(parameter);
            boardService.Start();
        }

        private void InitiSettings()
        {
            int engineStrength = (int)appSettings.Values[AppPersistenceManager.ComputerStrengthKey];
            practiceBoardService.SetEngineStrength(engineStrength);

            BoardViewModel.PlaySounds = (bool)appSettings.Values[AppPersistenceManager.EnableMoveSoundsKey];
            BoardViewModel.ShowLegalMoves = (bool)appSettings.Values[AppPersistenceManager.ShowLegalMovesKey];
            NotationViewModel.UseFigurineNotation = (int)appSettings.Values[AppPersistenceManager.NotationTypeKey] == (int)NotationType.Figurines;
        }

        public override void LoadPosition(PositionLoadOptions positionLoadOptions)
        {
            base.LoadPosition(positionLoadOptions);
            UserColor = positionLoadOptions.Perspective;
        }

        private void SwitchColorExecuted(object obj)
        {
            UserColor = UserColor == SideColor.Black ? SideColor.White : SideColor.Black;
            if (practiceBoardService.UserColor != BoardViewModel.Perspective)
                BoardViewModel.TogglePerspectiveCommand.Execute(null);
        }

        protected override void NewGame()
        {
            practiceBoardService.Stop();
            base.NewGame();
            practiceBoardService.Start();
        }

        private void AnalyseGameExecuted(object obj)
        {
            NavigationService.NavigateTo(ViewModelLocator.MainPageNavigationName, GetPositionLoadOptions(BoardSerializationType.PGN, false));
        }
    }
}
