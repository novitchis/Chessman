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
        bool IsWhiteTurn { get; }

        void ResetBoard();

        void LoadFromFen(string fenString);

        ChessPiece GetPiece(Coordinate coordinate);

        bool SubmitMove(Coordinate from, Coordinate to);

        MoveData GetCurrentMove();

        IList<MoveData> GetMoves(bool stopOnCurrentMove = true);

        IList<MoveData> GetVariationMoveData(IList<Move> moves);

        bool GoToMove(int moveIndex);

        bool IsValid(string fen);
    }
}
