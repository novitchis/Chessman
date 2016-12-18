using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
{
    public class AnalysisChessBoardViewModel : ChessBoardViewModel
    {
        IBoardService analysisBoardService = null;

        public AnalysisChessBoardViewModel(IBoardService analysisBoardService)
            : base(analysisBoardService)
        {
            this.analysisBoardService = analysisBoardService;
            InitBoard();

            Messenger.Default.Register<GenericMessage<MoveData>>(this, NotificationMessages.CurrentMoveChanged, OnCurrentMoveChangedMessage);
        }

        private void OnCurrentMoveChangedMessage(GenericMessage<MoveData> moveMessage)
        {
            RefreshSquares();
        }

        protected override void OnSelectionChanged(SquareViewModel oldSquare, SquareViewModel newSquare)
        {
            base.OnSelectionChanged(oldSquare, newSquare);

            if (oldSquare == null || newSquare == null)
                return;

            if (analysisBoardService.SubmitMove(oldSquare.Coordinate, newSquare.Coordinate))
                Messenger.Default.Send(new MessageBase(), NotificationMessages.MoveExecuted);
        }

        public override void RefreshSquares()
        {
            base.RefreshSquares();

            MoveData lastMove = analysisBoardService.GetCurrentMove();
            if (lastMove != null)
            {
                Squares[GetSquareIndex(lastMove.Move.GetFrom())].IsLastMoveSquare = true;
                Squares[GetSquareIndex(lastMove.Move.GetTo())].IsLastMoveSquare = true;
            }
        }

        private int GetSquareIndex(Coordinate coordinate)
        {
            int index = -1;
            if (Perspective == SideColor.White)
                index = (7 - coordinate.Y) * 8 + coordinate.X;
            else
                index = coordinate.Y * 8 + 7 - coordinate.X;

            return index;
        }
    }
}
