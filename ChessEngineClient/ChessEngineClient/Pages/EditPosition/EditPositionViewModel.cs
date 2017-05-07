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
        private IBoardEditorService editorBoardService = null;
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

        public ICommand AnalyseCommand
        {
            get { return new RelayCommand(AnalyseExecuted); }
        }

        public ICommand PracticeCommand
        {
            get { return new RelayCommand(PracticeExecuted); }
        }

        #endregion

        public EditPositionViewModel(INavigationService navigationService, IBoardEditorService editorBoardService)
        {
            this.navigationService = navigationService;
            this.editorBoardService = editorBoardService;

            BoardViewModel = new EditChessBoardViewModel(editorBoardService);
            PiecesPaletteViewModel = new PiecesPaletteViewModel();

            Messenger.Default.Register<SquareViewModel>(this, NotificationMessages.SquarePressed, OnSquarePressed);
        }

        public void OnPageNavigatedTo(PositionLoadOptions loadOptions)
        {
            if (loadOptions.SerializationType != BoardSerializationType.FEN)
                throw new ArgumentException("Use FEN serialization for edit board loading.");

            editorBoardService.LoadFrom(loadOptions.SerializedBoard);
            BoardViewModel.RefreshBoard(loadOptions.Perspective);
            IsWhiteToMove = editorBoardService.IsWhiteTurn;
            IsBoardValid = editorBoardService.AcceptEditedPosition();
        }

        private void OnSquarePressed(SquareViewModel squareVM)
        {
            // the square does not belong to our bord
            if (BoardViewModel.Squares.IndexOf(squareVM) == -1)
                return;

            ChessPieceViewModel pieceViewModel = BoardViewModel.GetPieceViewModel(squareVM.Coordinate);

            if (pieceViewModel != null || PiecesPaletteViewModel.SelectedPiece == null)
            {
                BoardViewModel.RemovePiece(pieceViewModel);
                editorBoardService.SetPiece(squareVM.Coordinate, null);
            }
            else
            {
                BoardViewModel.AddPiece(new ChessPieceViewModel(PiecesPaletteViewModel.SelectedPiece.Piece, squareVM.Coordinate));
                editorBoardService.SetPiece(squareVM.Coordinate, PiecesPaletteViewModel.SelectedPiece.Piece);
            }

            IsBoardValid = editorBoardService.AcceptEditedPosition();
        }

        private void ClearExecuted(object obj)
        {
            foreach (ChessPieceViewModel piece in BoardViewModel.Pieces)
                editorBoardService.SetPiece(piece.Coordinate, null);

            BoardViewModel.Pieces.Clear();

            IsBoardValid = false;
        }

        private void AnalyseExecuted(object obj)
        {
            navigationService.NavigateTo(ViewModelLocator.MainPageNavigationName, GetPositionLoadOptions());
        }

        private void PracticeExecuted(object obj)
        {
            navigationService.NavigateTo(ViewModelLocator.PracticePageNavigationName, GetPositionLoadOptions());
        }

        private PositionLoadOptions GetPositionLoadOptions()
        {
            return new PositionLoadOptions()
            {
                SerializedBoard = editorBoardService.Serialize(BoardSerializationType.FEN),
                Perspective = BoardViewModel.Perspective,
            };
        }
    }
}
