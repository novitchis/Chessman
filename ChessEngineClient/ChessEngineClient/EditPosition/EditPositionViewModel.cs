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
    public class EditPositionViewModel : ViewModelBase
    {
        private INavigationService navigationService = null;

        public ChessBoardViewModel BoardViewModel { get; set; }

        public PiecesPaletteViewModel PiecesPaletteViewModel { get; set; }

        public ICommand ClearCommand
        {
            get { return new RelayCommand(ClearExecuted); }
        }

        public ICommand CancelCommand
        {
            get { return new RelayCommand(CancelExecuted); }
        }

        public EditPositionViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            BoardViewModel = ViewModelLocator.IOCContainer.Resolve<ChessBoardViewModel>();
            BoardViewModel.IsEdit = true;

            PiecesPaletteViewModel = new PiecesPaletteViewModel();

            Messenger.Default.Register<SquareViewModel>(this, NotificationMessages.SquarePressed, OnSquarePressed);
        }

        private void OnSquarePressed(SquareViewModel square)
        {
            if (square.Piece != null)
                square.Piece = null;
            else
                square.Piece = PiecesPaletteViewModel.SelectedPiece;
        }

        private void CancelExecuted(object obj)
        {
            navigationService.NavigateTo(ViewModelLocator.MainPageNavigationName);
        }

        private void ClearExecuted(object obj)
        {
            BoardViewModel.ClearPieces();
        }
    }
}
