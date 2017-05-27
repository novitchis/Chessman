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

namespace ChessEngineClient.ViewModel
{
    public class ChessBoardViewModel : ViewModelBase
    {
        public static int[] RankNumbersAsWhite = new[] { 8, 7, 6, 5, 4, 3, 2, 1 };
        public static char[] FieldLettersAsWhite = new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };

        private IBasicBoardService basicBoardService = null;
        private List<SquareViewModel> squares = null;
        private SquareViewModel selectedSquare = null;
        private ObservableCollection<ChessPieceViewModel> pieces = new ObservableCollection<ChessPieceViewModel>();
        private SideColor perspective = SideColor.White;
        private int[] rankNumbers = RankNumbersAsWhite;
        private char[] fieldLetters = FieldLettersAsWhite;
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

        public int[] RankNumbers
        {
            get { return rankNumbers; }
            set
            {
                rankNumbers = value;
                NotifyPropertyChanged();
            }
        }

        public char[] FieldLetters
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
            if (perspective == SideColor.White)
            {
                RankNumbers = RankNumbersAsWhite;
                FieldLetters = FieldLettersAsWhite;
            }
            else
            {
                RankNumbers = RankNumbersAsWhite.Reverse().ToArray();
                FieldLetters = FieldLettersAsWhite.Reverse().ToArray();
            }
        }

        public void RefreshBoard(SideColor changedPerspective)
        {
            Perspective = changedPerspective;
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

        public ChessPieceViewModel GetPieceViewModel(Coordinate coordinate)
        {
            return Pieces.FirstOrDefault(p => p.Coordinate.X == coordinate.X && p.Coordinate.Y == coordinate.Y);
        }

        public bool RemovePiece(ChessPieceViewModel pieceViewModel)
        {
            return Pieces.Remove(pieceViewModel);
        }

        public void AddPiece(ChessPieceViewModel pieceViewModel)
        {
            Pieces.Add(pieceViewModel);
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

        public virtual void OnPieceDropped(SquareViewModel targetSquare)
        {
            SelectedSquare = targetSquare;
        }
    }
}
