using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine;

namespace ChessEngineClient
{
    public class ChessBoardService : IChessBoardService
    {
        private ChessBoard chessBoard = null;

        public event EventHandler<ChessEventArgs> MoveExecuted;
        public event EventHandler<ChessEventArgs> GoToExecuted;

        public ChessBoardService()
        {
            chessBoard = new ChessBoard();
            chessBoard.Initialize();
            chessBoard.StorePGN();
        }

        public ChessPiece GetPiece(Coordinate coordinate)
        {
            return chessBoard.GetPiece(coordinate);
        }

        public bool SubmitMove(Coordinate from, Coordinate to)
        {
            Move move = new Move(from, to);
            bool result = chessBoard.SubmitMove(from, to);

            if (result)
                OnMovedExecuted(new ChessEventArgs());

            return result;
        }

        public IList<MoveData> GetMoves()
        {
            return chessBoard.GetMoves();
        }

        public void GoToMove(int moveIndex)
        {
            IList<MoveData> moves = GetMoves();
            int itemsToRemove = moves.Count - moveIndex - 1;
            bool executed = false;
            while (itemsToRemove > 0)
            {
                executed = true;
                bool isWhiteMove = moves.Count % 2 == 1;
                moves.RemoveAt(moves.Count - 1);
                chessBoard.UndoMove(isWhiteMove);
                itemsToRemove--;
            }

            if (executed)
                OnGoToExecuted(new ChessEventArgs());
        }

        protected virtual void OnMovedExecuted(ChessEventArgs e)
        {
            MoveExecuted?.Invoke(this, e);
        }

        protected virtual void OnGoToExecuted(ChessEventArgs e)
        {
            GoToExecuted?.Invoke(this, e);
        }
    }

    public class ChessEventArgs: EventArgs
    {

    }
}
