using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        protected override bool OnSelectionChanged(SquareViewModel oldSquare, SquareViewModel newSquare)
        {
            if (oldSquare == null || newSquare == null)
                return false;

            bool result = analysisBoardService.SubmitMove(oldSquare.Coordinate, newSquare.Coordinate);
            if (result)
                Messenger.Default.Send(new MessageBase(this, analysisBoardService), NotificationMessages.MoveExecuted);

            return result;
        }

        public override void RefreshSquares()
        {
            base.RefreshSquares();
            RefreshPositionStateMarkers();            
        }

        private void RefreshPositionStateMarkers()
        {
            MoveData lastMove = analysisBoardService.GetCurrentMove();
            if (lastMove != null)
            {
                Squares[GetSquareIndex(lastMove.Move.GetFrom())].IsLastMoveSquare = true;
                Squares[GetSquareIndex(lastMove.Move.GetTo())].IsLastMoveSquare = true;
            }

            bool shouldHighlightWhiteKing = analysisBoardService.GetIsStalemate() || (analysisBoardService.IsWhiteTurn && analysisBoardService.GetIsInCheck());
            bool shouldHighlightBlackKing = analysisBoardService.GetIsStalemate() || (!analysisBoardService.IsWhiteTurn && analysisBoardService.GetIsInCheck());

            foreach (SquareViewModel square in Squares)
            {
                if (square.PieceViewModel != null && square.PieceViewModel.Piece.Type == PieceType.King)
                {
                    if (square.PieceViewModel.Piece.Color == PieceColor.White)
                        square.PieceViewModel.IsHighlighted = shouldHighlightWhiteKing;
                    else
                        square.PieceViewModel.IsHighlighted = shouldHighlightBlackKing;
                }
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
