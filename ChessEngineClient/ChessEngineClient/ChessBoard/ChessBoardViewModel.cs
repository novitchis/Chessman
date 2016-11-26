using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
{
    public class ChessBoardViewModel : ViewModelBase
    {
        public static int[] RankNumbersAsWhite = new[] { 8, 7, 6, 5, 4, 3, 2, 1 };
        public static char[] FieldLettersAsWhite = new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };

        private IChessBoardService chessBoardService = null;
        private List<SquareViewModel> squares = null;
        private SquareViewModel selectedSquare = null;
        private SideColor perspective = SideColor.White;
        private int[] rankNumbers = RankNumbersAsWhite;
        private char[] fieldLetters = FieldLettersAsWhite;

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

        public bool IsEdit { get; private set; }

        #endregion

        public ChessBoardViewModel(IChessBoardService chessBoardService, bool isEdit = false)
        {
            this.chessBoardService = chessBoardService;
            this.IsEdit = isEdit;

            if (!IsEdit)
                Messenger.Default.Register<GenericMessage<MoveData>>(this, NotificationMessages.CurrentMoveChanged, OnCurrentMoveChangedMessage);

            InitBoard();
        }

        private void InitBoard()
        {
            var newSquares = new List<SquareViewModel>();

            if (perspective == SideColor.White)
            {
                RankNumbers = RankNumbersAsWhite;
                FieldLetters = FieldLettersAsWhite;

                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                        newSquares.Add(new SquareViewModel(new Coordinate(x, 7 - y)));
                }
            }
            else
            {
                RankNumbers = RankNumbersAsWhite.Reverse().ToArray();
                FieldLetters = FieldLettersAsWhite.Reverse().ToArray();

                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                        newSquares.Add(new SquareViewModel(new Coordinate(7 - x, y)));
                }
            }

            Squares = newSquares;

            RefreshSquares();
        }

        private void OnCurrentMoveChangedMessage(GenericMessage<MoveData> moveMessage)
        {
            RefreshSquares();
        }

        public void TogglePerspective()
        {
            perspective = perspective == SideColor.Black ?
                SideColor.White : SideColor.Black;

            InitBoard();
        }

        private void OnSelectionChanged(SquareViewModel oldSquare, SquareViewModel newSquare)
        {
            if (oldSquare == null || newSquare == null)
                return;

            if (!IsEdit && chessBoardService.SubmitMove(oldSquare.Coordinate, newSquare.Coordinate))
                Messenger.Default.Send(new MessageBase(), NotificationMessages.MoveExecuted);
        }

        public void RefreshSquares()
        {
            foreach (SquareViewModel square in Squares)
            {
                square.Piece = chessBoardService.GetPiece(square.Coordinate);
                square.IsLastMoveSquare = false;
            }

            SelectedSquare = null;

            if (IsEdit)
                return;

            MoveData lastMove = chessBoardService.GetCurrentMove();
            if (lastMove != null)
            {
                Squares[GetSquareIndex(lastMove.Move.GetFrom())].IsLastMoveSquare = true;
                Squares[GetSquareIndex(lastMove.Move.GetTo())].IsLastMoveSquare = true;
            }
        }

        private int GetSquareIndex(Coordinate coordinate)
        {
            int index = -1;
            if (perspective == SideColor.White)
                index = (7 - coordinate.Y) * 8 + coordinate.X; 
            else
                index = coordinate.Y * 8 + 7 - coordinate.X;

            return index;
        }

        public void ClearPieces()
        {
            Squares.ForEach(s => s.Piece = null);
        }
    }
}
