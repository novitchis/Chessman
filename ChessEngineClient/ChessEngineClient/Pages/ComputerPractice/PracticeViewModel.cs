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
        private IPropertySet propertiesSet = null;

        public ICommand SwitchColor
        {
            get { return new RelayCommand(SwitchColorExecuted); }
        }

        public ICommand AnalyseGameCommand
        {
            get { return new RelayCommand(AnalyseGameExecuted); }
        }

        public PracticeViewModel(
            INavigationService navigationService, 
            IPracticeBoardService practiceBoardService,
            IPropertySet propertiesSet)
            : base(navigationService, practiceBoardService)
        {
            this.practiceBoardService = practiceBoardService;
            this.propertiesSet = propertiesSet;
            BoardViewModel = new PracticeBoardViewModel(practiceBoardService);
        }

        public override void OnNavigatedTo(object parameter)
        {
            int engineStrength = (int)propertiesSet[AppSettingsKeys.ComputerStrengthKey];
            practiceBoardService.SetEngineStrength(engineStrength);

            PositionLoadOptions positionLoadOptions = parameter as PositionLoadOptions;
            if (positionLoadOptions != null)
            {
                if (practiceBoardService.UserColor != positionLoadOptions.Perspective)
                    practiceBoardService.SwitchUserColor();
            }

            base.OnNavigatedTo(parameter);
        }

        private void SwitchColorExecuted(object obj)
        {
            practiceBoardService.SwitchUserColor();
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
            NavigationService.NavigateTo(ViewModelLocator.MainPageNavigationName, GetPositionLoadOptions(BoardSerializationType.PGN));
        }
    }
}
