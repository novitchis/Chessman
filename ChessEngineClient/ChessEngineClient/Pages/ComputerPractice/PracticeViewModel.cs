using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChessEngineClient.ViewModel
{
    public class PracticeViewModel : BoardPageViewModel, INavigationAware
    {
        private IPracticeBoardService practiceBoardService = null;

        public ICommand SwitchColor
        {
            get { return new RelayCommand(SwitchColorExecuted); }
        }

        public PracticeViewModel(INavigationService navigationService, IPracticeBoardService practiceBoardService)
            : base(navigationService, practiceBoardService)
        {
            this.practiceBoardService = practiceBoardService;
            BoardViewModel = new PracticeBoardViewModel(practiceBoardService);
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
    }
}
