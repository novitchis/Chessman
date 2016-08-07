using Framework.MVVM;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace ChessEngineClient.ViewModel
{
    public class EditPositionViewModel : ViewModelBase
    {
        private INavigationService navigationService = null;
        private IChessBoardService chessBoardService = null;
        private SideColor toMoveSide = SideColor.White;

        #region "Properties"

        public ChessBoardViewModel BoardViewModel { get; set; }

        public PiecesPaletteViewModel PiecesPaletteViewModel { get; set; }

        public SideColor ToMoveSide
        {
            get { return toMoveSide; }
            set
            {
                if (toMoveSide != value)
                {
                    toMoveSide = value;
                    NotifyPropertyChanged();
                } 
            }
        }

        public ICommand ClearCommand
        {
            get { return new RelayCommand(ClearExecuted); }
        }

        public ICommand CancelCommand
        {
            get { return new RelayCommand(CancelExecuted); }
        }

        public ICommand SaveCommand
        {
            get { return new RelayCommand(SaveExecuted);  }
        }

        #endregion

        public EditPositionViewModel(INavigationService navigationService, IChessBoardService chessBoardService)
        {
            this.navigationService = navigationService;
            this.chessBoardService = chessBoardService;

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
            ReturnToMainView();
        }

        private void ReturnToMainView()
        {
            Messenger.Default.Unregister<SquareViewModel>(this);
            navigationService.NavigateTo(ViewModelLocator.MainPageNavigationName);
        }

        private void ClearExecuted(object obj)
        {
            BoardViewModel.ClearPieces();
        }

        private void SaveExecuted(object obj)
        {
            chessBoardService.LoadFromFen(GetFen());
            ViewModelLocator.MainViewModel.ReloadPosition();
            ReturnToMainView();
        }

        private string GetFen()
        {
            StringBuilder fenBuilder = new StringBuilder();

            int index = 0;
            foreach(var square in BoardViewModel.Squares)
            {
                if (index != 0 && index % 8 == 0)
                    fenBuilder.Append('/');

                char squareContent = '1';
                if (square.Piece != null)
                    squareContent = square.Piece.Color == ChessEngine.PieceColor.Black ? square.Piece.NotationChar : Char.ToUpper(square.Piece.NotationChar);

                fenBuilder.Append(squareContent);
                index++;
            }

            //only who's on move is implemented
            fenBuilder.AppendFormat(" {0} KQkq - 0 1", toMoveSide == SideColor.White ? 'w' : 'b');

            return fenBuilder.ToString();
        }
    }
}
