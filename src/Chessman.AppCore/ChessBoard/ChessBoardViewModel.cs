using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chessman.ViewModel
{
    public class ChessBoardViewModel : ViewModelBase
    {
        private IBasicBoardService basicBoardService = null;
        private List<SquareViewModel> squares = null;
        private SquareViewModel selectedSquare = null;
        private ObservableCollection<ChessPieceViewModel> pieces = new ObservableCollection<ChessPieceViewModel>();
        private SideColor perspective = SideColor.White;
        private RankOrFieldViewModel[] rankNumbers = RankOrFieldViewModel.GetRanks(SideColor.White);
        private RankOrFieldViewModel[] fieldLetters = RankOrFieldViewModel.GetFields(SideColor.White);
        private Move suggestedMove = null;
        
        #region "Properties"

        public List<SquareViewModel> Squares
        {
            get { return squares; }
            set
            {
                if (squares != value)
                {
                    squares = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public SquareViewModel SelectedSquare
        {
            get { return selectedSquare; }
            set
            {
                if (selectedSquare != value)
                {
                    OnSelectionChanged(selectedSquare, value);
                    selectedSquare = value;

                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<ChessPieceViewModel> Pieces
        {
            get { return pieces; }
            set
            {
                if (pieces != value)
                {
                    pieces = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public RankOrFieldViewModel[] RankNumbers
        {
            get { return rankNumbers; }
            set
            {
                rankNumbers = value;
                NotifyPropertyChanged();
            }
        }

        public RankOrFieldViewModel[] FieldLetters
        {
            get { return fieldLetters; }
            set
            {
                fieldLetters = value;
                NotifyPropertyChanged();
            }
        }

        public SideColor Perspective
        {
            get { return perspective; }
            set
            {
                if (perspective != value)
                {
                    perspective = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Move SuggestedMove
        {
            get { return suggestedMove; }
            set
            {
                if (suggestedMove != value)
                {
                    suggestedMove = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand TogglePerspectiveCommand
        {
            get { return new RelayCommand(TogglePerspectiveExecuted); }
        }

        #endregion

        public ChessBoardViewModel(IBasicBoardService basicBoardService)
        {
            this.basicBoardService = basicBoardService;
        }

        protected void InitBoard()
        {
            var newSquares = new List<SquareViewModel>();
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                    newSquares.Add(new SquareViewModel(new Coordinate(x, 7 - y)));
            }

            Squares = newSquares;
            UpdateRankAndFields();

            RefreshSquares();
        }

        private void UpdateRankAndFields()
        {
            RankNumbers = RankOrFieldViewModel.GetRanks(perspective);
            FieldLetters = RankOrFieldViewModel.GetFields(perspective);
        }

        public void RefreshBoard(SideColor changedPerspective)
        {
            Perspective = changedPerspective;
            UpdateRankAndFields();
            RefreshSquares();
        }

        public virtual void RefreshSquares()
        {
            ClearCurrentMoveData();
            Pieces.Clear();

            foreach (SquareViewModel square in Squares)
            {
                ChessPiece piece = basicBoardService.GetPiece(square.Coordinate);
                if (piece != null)
                    Pieces.Add(new ChessPieceViewModel(piece, square.Coordinate));
            }
        }

        protected void ClearCurrentMoveData()
        {
            SuggestedMove = null;
            SelectedSquare = null;
        }

        public int GetSquareIndex(Coordinate coordinate)
        {
            return coordinate.X + (8 * (7 - coordinate.Y));
        }

        public SquareViewModel GetSquare(Coordinate coordinate)
        {
            return Squares[GetSquareIndex(coordinate)];
        }

        public ChessPieceViewModel GetPiece(Coordinate coordinate)
        {
            return Pieces.FirstOrDefault(p => !p.RemovePending && p.Coordinate.X == coordinate.X && p.Coordinate.Y == coordinate.Y);
        }

        public ChessPieceViewModel GetRemovedPiece(Coordinate coordinate)
        {
            return Pieces.FirstOrDefault(p => p.RemovePending && p.Coordinate.X == coordinate.X && p.Coordinate.Y == coordinate.Y);
        }

        public bool RemovePiece(ChessPieceViewModel pieceViewModel)
        {
            return Pieces.Remove(pieceViewModel);
        }

        public void AddPiece(ChessPieceViewModel pieceViewModel)
        {
            Pieces.Add(pieceViewModel);
        }

        public void ReplacePiece(ChessPieceViewModel oldPieceViewModel, ChessPieceViewModel newPieceViewModel)
        {
            RemovePiece(oldPieceViewModel);
            AddPiece(newPieceViewModel);
        }

        protected virtual void OnSelectionChanged(SquareViewModel oldSquare, SquareViewModel newSquare)
        {
        }

        private void TogglePerspectiveExecuted(object obj)
        {
            Perspective = Perspective == SideColor.Black ? SideColor.White : SideColor.Black;
            UpdateRankAndFields();
            SuggestedMove = null;
        }

        public virtual void OnPieceDropped(SquareViewModel sourceSquare, SquareViewModel targetSquare)
        {
            selectedSquare = targetSquare;
            NotifyPropertyChanged("SelectedSquare");
        }
    }
}
