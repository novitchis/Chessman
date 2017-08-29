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

        bool ValidateMove(Coordinate from, Coordinate to);

        bool SubmitMove(Coordinate from, Coordinate to);

        //bool IsPromotionMove(Coordinate from, Coordinate to);

        IList<MoveData> GetMoves(bool stopOnCurrentMove = true);

        IList<MoveData> GetVariationMoveData(IList<Move> moves);

        bool GoToMove(int moveIndex);

        void ResetBoard();

        bool GetIsInCheck();

        bool GetIsStalemate();

        bool GetIsMate();
    }
}
