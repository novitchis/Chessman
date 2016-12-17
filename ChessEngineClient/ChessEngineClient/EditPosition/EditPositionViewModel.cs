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
        private IEditorBoardService editorBoardService = null;
        private bool isWhiteToMove = true;
        private bool isBoardValid = true;

        #region "Properties"

        public EditChessBoardViewModel BoardViewModel { get; set; }

        public PiecesPaletteViewModel PiecesPaletteViewModel { get; set; }

        public bool IsWhiteToMove
        {
            get { return isWhiteToMove; }
            set
            {
                if (isWhiteToMove != value)
                {
                    isWhiteToMove = value;
                    NotifyPropertyChanged();
                    editorBoardService.SetSideToMove(isWhiteToMove);
                    IsBoardValid = editorBoardService.AcceptEditedPosition();
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

        public ICommand SaveCommand
        {
            get { return new RelayCommand(SaveExecuted); }
        }

        #endregion

        public EditPositionViewModel(INavigationService navigationService, IEditorBoardService editorBoardService)
        {
            this.navigationService = navigationService;
            this.editorBoardService = editorBoardService;

            BoardViewModel = new EditChessBoardViewModel(editorBoardService);
            PiecesPaletteViewModel = new PiecesPaletteViewModel();

            Messenger.Default.Register<SquareViewModel>(this, NotificationMessages.SquarePressed, OnSquarePressed);
        }

        public void OnPageNavigatedTo(PositionLoadOptions loadOptions)
        {
            editorBoardService.LoadFromFen(loadOptions.Fen);
            BoardViewModel.RefreshBoard(loadOptions.Perspective);
            IsWhiteToMove = editorBoardService.IsWhiteTurn;
            IsBoardValid = editorBoardService.AcceptEditedPosition();
        }

        private void OnSquarePressed(SquareViewModel squareVM)
        {
            // the square does not belong to our bord
            if (BoardViewModel.Squares.IndexOf(squareVM) == -1)
                return;

            ChessPiece newPiece = squareVM.Piece != null ? null : PiecesPaletteViewModel.SelectedPiece;
            squareVM.Piece = newPiece;
            editorBoardService.SetPiece(squareVM.Coordinate, newPiece);
            IsBoardValid = editorBoardService.AcceptEditedPosition();
        }

        private void ClearExecuted(object obj)
        {
            BoardViewModel.Squares.ForEach(s =>
            {
                s.Piece = null;
                editorBoardService.SetPiece(s.Coordinate, null);
            });

            IsBoardValid = false;
        }

        private void SaveExecuted(object obj)
        {
            PositionLoadOptions positionLoadOptions = new PositionLoadOptions()
            {
                Fen = editorBoardService.GetFen(),
                Perspective = BoardViewModel.Perspective,
            };

            navigationService.NavigateTo(ViewModelLocator.MainPageNavigationName, positionLoadOptions);
        }
    }
}
