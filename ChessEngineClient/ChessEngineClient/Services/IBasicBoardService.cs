using ChessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient
{
    public interface IBasicBoardService
    {
        bool IsWhiteTurn { get; }

        ChessPiece GetPiece(Coordinate coordinate);

        bool LoadFrom(string value);

        string Serialize(BoardSerializationType type);
    }

    public enum BoardSerializationType
    {
        FEN,
        PGN
    }
}
