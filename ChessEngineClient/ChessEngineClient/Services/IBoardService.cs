using ChessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient
{
    public interface IBoardService: IBasicBoardService
    {
        bool WasBlackFirstToMove();

        MoveData GetCurrentMove();

        bool SubmitMove(Coordinate from, Coordinate to);

        IList<MoveData> GetMoves(bool stopOnCurrentMove = true);

        IList<MoveData> GetVariationMoveData(IList<Move> moves);

        bool GoToMove(int moveIndex);

        void ResetBoard();
    }
}
