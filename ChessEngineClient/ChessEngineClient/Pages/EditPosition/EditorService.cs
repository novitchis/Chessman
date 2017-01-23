using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine;

namespace ChessEngineClient
{
    public class EditorService : IBoardEditorService
    {
        private ChessBoard chessBoard = null;

        public bool IsWhiteTurn
        {
            get { return chessBoard.IsWhiteTurn(); }
        }

        public EditorService()
        {
            chessBoard = new ChessBoard();
            chessBoard.Initialize();
            chessBoard.StorePGN();
        }

        public ChessPiece GetPiece(Coordinate coordinate)
        {
            return chessBoard.GetPiece(coordinate);
        }

        public void SetPiece(Coordinate coordinate, ChessPiece piece)
        {
            chessBoard.SetPiece(coordinate, piece);
        }

        public void SetSideToMove(bool white)
        {
            chessBoard.SetSideToMove(white);
        }

        public bool AcceptEditedPosition()
        {
            return chessBoard.AcceptEditedPosition();
        }

        public string Serialize(BoardSerializationType type)
        {
            return chessBoard.Serialize((int)type);
        }

        public bool LoadFrom(string serializedValue)
        {
            return chessBoard.LoadFrom(serializedValue);
        }
    }
}
