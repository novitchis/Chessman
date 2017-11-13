using ChessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chessman
{
    public interface IBasicBoardService
    {
        bool IsWhiteTurn { get; }

        ChessPiece GetPiece(Coordinate coordinate);

        bool LoadFrom(string value, int index = -1);

        string Serialize(BoardSerializationType type, bool stopOnCurrent);
    }

    public enum BoardSerializationType
    {
        FEN,
        PGN
    }
}
