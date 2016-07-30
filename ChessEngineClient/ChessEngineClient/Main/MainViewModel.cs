using Framework.MVVM;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChessEngineClient.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private INavigationService navigationService = null;

        public ChessBoardViewModel BoardViewModel { get; set; }

        public AnalysisViewModel AnalysisViewModel { get; set; }

        public NotationViewModel NotationViewModel { get; set; }

        public ICommand TogglePerspectiveCommand
        {
            get { return new RelayCommand(TogglePerspectiveExecuted); }
        }

        public ICommand EditPositionCommand
        {
            get { return new RelayCommand(EditPositionExecuted); }
        }

        public MainViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;

            BoardViewModel = ViewModelLocator.IOCContainer.Resolve<ChessBoardViewModel>();
            AnalysisViewModel = new AnalysisViewModel();
            NotationViewModel = ViewModelLocator.IOCContainer.Resolve<NotationViewModel>();
        }

        private void TogglePerspectiveExecuted(object obj)
        {
            BoardViewModel.TogglePerspective();
        }

        private void EditPositionExecuted(object obj)
        {
            navigationService.NavigateTo(ViewModelLocator.EditPositionPageNavigationName);
        }
    }
}
