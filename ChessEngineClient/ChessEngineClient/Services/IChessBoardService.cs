using ChessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient
{
    public interface IChessBoardService
    {
        event EventHandler<ChessEventArgs> MoveExecuted;

        ChessPiece GetPiece(Coordinate coordinate);

        bool SubmitMove(Coordinate from, Coordinate to);

        IList<MoveData> GetMoves();
    }
}
