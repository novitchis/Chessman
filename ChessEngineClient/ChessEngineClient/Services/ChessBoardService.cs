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
                OnMovedExecuted(new ChessEventArgs(move));

            return result;
        }

        protected virtual void OnMovedExecuted(ChessEventArgs e)
        {
            MoveExecuted?.Invoke(this, e);
        }

        public IList<MoveData> GetMoves()
        {
            return chessBoard.GetMoves();
        }
    }


    public class ChessEventArgs: EventArgs
    {
        public Move Move { get; private set; }

        public ChessEventArgs(Move move)
        {
            Move = move;
        }
    }
}
