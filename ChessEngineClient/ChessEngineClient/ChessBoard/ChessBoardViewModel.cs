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
        private AnalysisPerspective perspective = AnalysisPerspective.White;
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

        #endregion

        public ChessBoardViewModel(IChessBoardService chessBoardService)
        {
            this.chessBoardService = chessBoardService;
            Messenger.Default.Register<GenericMessage<MoveData>>(this, NotificationMessages.CurrentMoveChanged, OnCurrentMoveChangedMessage);
            Messenger.Default.Register<MessageBase>(this, NotificationMessages.GoBack, OnGoBackMessage);
            Messenger.Default.Register<MessageBase>(this, NotificationMessages.GoForward, OnGoForwardMessage);

            InitBoard();
        }

        private void OnGoForwardMessage(object obj)
        {
            MoveData currentMove = chessBoardService.GetCurrentMove();
            int moveIndex = 0;
            if (currentMove != null)
                moveIndex = currentMove.Index + 1;

            if (chessBoardService.GoToMove(moveIndex))
            {
                Messenger.Default.Send(new MessageBase(), NotificationMessages.MoveExecuted);
                RefreshPieces();
            }
        }

        private void OnGoBackMessage(object obj)
        {
            MoveData currentMove = chessBoardService.GetCurrentMove();
            if (currentMove != null)
            {
                if (chessBoardService.GoToMove(currentMove.Index - 1))
                {
                    Messenger.Default.Send(new MessageBase(), NotificationMessages.MoveExecuted);
                    RefreshPieces();
                }
            }
        }

        private void InitBoard()
        {
            var newSquares = new List<SquareViewModel>();

            if (perspective == AnalysisPerspective.White)
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

            RefreshPieces();
        }

        private void OnCurrentMoveChangedMessage(GenericMessage<MoveData> moveMessage)
        {
            RefreshPieces();
        }

        public void TogglePerspective()
        {
            perspective = perspective == AnalysisPerspective.Black ?
                AnalysisPerspective.White : AnalysisPerspective.Black;

            InitBoard();
        }

        private void OnSelectionChanged(SquareViewModel oldSquare, SquareViewModel newSquare)
        {
            if (oldSquare != null)
                oldSquare.IsSelected = false;

            if (newSquare != null)
                newSquare.IsSelected = true;

            if (oldSquare == null || newSquare == null)
                return;

            if (chessBoardService.SubmitMove(oldSquare.Coordinate, newSquare.Coordinate))
            {
                RefreshPieces();
                Messenger.Default.Send(new MessageBase(), NotificationMessages.MoveExecuted);
            }
        }

        private void RefreshPieces()
        {
            foreach (SquareViewModel square in Squares)
                square.Piece = chessBoardService.GetPiece(square.Coordinate);
        }
    }
}
