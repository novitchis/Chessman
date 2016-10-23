using ChessEngine;
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
        private bool isBoardValid = false;

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
                    IsBoardValid = chessBoardService.IsValid(GetFen());
                }
            }
        }

        public bool IsBoardValid
        {
            get { return isBoardValid; }
            set
            {
                if (isBoardValid != value)
                {
                    isBoardValid = value;
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
            get { return new RelayCommand(SaveExecuted); }
        }

        #endregion

        public EditPositionViewModel(INavigationService navigationService, IChessBoardService chessBoardService)
        {
            this.navigationService = navigationService;
            this.chessBoardService = chessBoardService;

            BoardViewModel = ViewModelLocator.IOCContainer.Resolve<ChessBoardViewModel>();
            BoardViewModel.IsEdit = true;
            IsBoardValid = chessBoardService.IsValid(GetFen());

            PiecesPaletteViewModel = new PiecesPaletteViewModel();

            Messenger.Default.Register<SquareViewModel>(this, NotificationMessages.SquarePressed, OnSquarePressed);
        }

        private void OnSquarePressed(SquareViewModel square)
        {
            if (square.Piece != null)
                square.Piece = null;
            else
                square.Piece = PiecesPaletteViewModel.SelectedPiece;

            IsBoardValid = chessBoardService.IsValid(GetFen());
        }

        private void CancelExecuted(object obj)
        {
            ReturnToMainView();
        }

        public void ReturnToMainView()
        {
            Messenger.Default.Unregister<SquareViewModel>(this);
            navigationService.NavigateTo(ViewModelLocator.MainPageNavigationName);
        }

        private void ClearExecuted(object obj)
        {
            BoardViewModel.ClearPieces();
            IsBoardValid = false;
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
                    squareContent = square.Piece.Color == PieceColor.Black ? square.Piece.NotationChar : Char.ToUpper(square.Piece.NotationChar);

                fenBuilder.Append(squareContent);
                index++;
            }

            //only who's on move is implemented
            string castling = GetCastlingFen();
            fenBuilder.AppendFormat(" {0} {1} - 0 1", toMoveSide == SideColor.White ? 'w' : 'b', castling);

            return fenBuilder.ToString();
        }

        private string GetCastlingFen()
        {
            return GetWhiteCastlingFen() + GetBlackCastlingFen();
        }

        private string GetBlackCastlingFen()
        {
            string result = "";
            ChessPiece blackKing = BoardViewModel.Squares[4].Piece;
            ChessPiece a8Rook = BoardViewModel.Squares[0].Piece;
            ChessPiece h8Rook = BoardViewModel.Squares[7].Piece;

            if (blackKing == null || a8Rook == null || h8Rook == null ||
                blackKing.Type != PieceType.King || blackKing.Color != PieceColor.Black)
                return result;

            if (h8Rook.Type == PieceType.Rook && h8Rook.Color == PieceColor.Black)
                result += "k";

            if (a8Rook.Type == PieceType.Rook && a8Rook.Color == PieceColor.Black)
                result += "q";

            return result;
        }

        private string GetWhiteCastlingFen()
        {
            string result = "";
            ChessPiece whiteKing = BoardViewModel.Squares[60].Piece;
            ChessPiece a1Rook = BoardViewModel.Squares[56].Piece;
            ChessPiece h1Rook = BoardViewModel.Squares[63].Piece;
            if (whiteKing == null || a1Rook == null || h1Rook == null || 
                whiteKing.Type != PieceType.King || whiteKing.Color != PieceColor.White)
                return result;

            if (h1Rook.Type == PieceType.Rook && h1Rook.Color == PieceColor.White)
                result += "K";

            if (a1Rook.Type == PieceType.Rook && a1Rook.Color == PieceColor.White)
                result += "Q";

            return result;

        }
    }
}
