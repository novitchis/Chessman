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

        void LoadFromFen(string fenString);

        string GetFen();

        ChessPiece GetPiece(Coordinate coordinate);
    }
}
