using ChessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chessman
{
    public interface IBoardEditorService : IBasicBoardService
    {
        void SetPiece(Coordinate coordinate, ChessPiece piece);

        void SetSideToMove(bool white);

        bool AcceptEditedPosition();
    }
}
