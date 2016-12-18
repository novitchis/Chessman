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

        public void LoadFromFen(string fenString)
        {
            chessBoard.LoadFrom(fenString, AnalysisService.FenSerializationType);
        }

        public string GetFen()
        {
            return chessBoard.Serialize(AnalysisService.FenSerializationType);
        }
    }
}
