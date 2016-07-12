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
        ChessPiece GetPiece(Coordinate coordinate);

        bool SubmitMove(Coordinate from, Coordinate to);

        MoveData GetCurrentMove();

        IList<MoveData> GetMoves(bool stopOnCurrentMove = true);

        void GoToMove(int moveIndex);
    }
}
