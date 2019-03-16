using ChessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chessman
{
    public interface IBoardService: IBasicBoardService
    {
        bool WasBlackFirstToMove();

        MoveData GetCurrentMove();

        bool ValidateMove(Coordinate from, Coordinate to);

        bool SubmitMove(Coordinate from, Coordinate to);

        bool SubmitMove(string pgnMove);

        bool SubmitPromotionMove(Coordinate from, Coordinate to, PieceType promotionPieceType);

        IList<MoveData> GetMoves(bool stopOnCurrentMove = true);

        IList<MoveData> GetVariationMoveData(IList<Move> moves);

        IList<Coordinate> GetAvailableMoves(Coordinate coordinate);

        bool GoToMove(int moveIndex);

        void ResetBoard();

        bool GetIsInCheck();

        bool GetIsStalemate();

        bool GetIsMate();

        bool CurrentIsLastMove();
    }
}
