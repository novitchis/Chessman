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

        public MoveData GetCurrentMove()
        {
            return chessBoard.GetCurrentMove();
        }

        public IList<MoveData> GetMoves(bool stopOnCurrentMove = true)
        {
            return chessBoard.GetMoves(stopOnCurrentMove);
        }

        public void GoToMove(int moveIndex)
        {
            bool result = chessBoard.GoToMove(moveIndex);
            if (result)
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
