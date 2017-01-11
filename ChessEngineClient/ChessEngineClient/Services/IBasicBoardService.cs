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

        void LoadFrom(string value, BoardSerializationType type);

        string Serialize(BoardSerializationType type);
    }

    public enum BoardSerializationType
    {
        FEN,
        PGN
    }
}
