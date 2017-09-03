using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine;
using System.Threading;

namespace ChessEngineClient
{
    public class BoardService : IBoardService
    {
        public const int FenSerializationType = 0;

        private ChessBoard chessBoard = null;

        public bool IsWhiteTurn
        {
            get { return chessBoard.IsWhiteTurn(); }
        }

        protected ChessBoard ChessBoard { get { return chessBoard; } }

        public BoardService()
        {
            chessBoard = new ChessBoard();
            chessBoard.Initialize();
            chessBoard.StorePGN();
        }

        public virtual void ResetBoard()
        {
            chessBoard.Initialize();
        }

        public ChessPiece GetPiece(Coordinate coordinate)
        {
            return chessBoard.GetPiece(coordinate);
        }

        public bool ValidateMove(Coordinate from, Coordinate to)
        {
            return chessBoard.ValidateMove(from, to);
        }

        public virtual bool SubmitMove(Coordinate from, Coordinate to)
        {
            return chessBoard.SubmitMove(from, to);
        }

        public bool SubmitPromotionMove(Coordinate from, Coordinate to, ChessPiece piece)
        {
            return chessBoard.SubmitPromotionMove(from, to, piece);
        }

        public MoveData GetCurrentMove()
        {
            return chessBoard.GetCurrentMove();
        }

        public bool WasBlackFirstToMove()
        {
            MoveData currentMove = GetCurrentMove();
            if (currentMove == null || currentMove.Index % 2 != 0)
                return !IsWhiteTurn;

            return IsWhiteTurn;
        }

        public IList<MoveData> GetMoves(bool stopOnCurrentMove = true)
        {
            return chessBoard.GetMoves(stopOnCurrentMove);
        }

        public virtual bool GoToMove(int moveIndex)
        {
            return chessBoard.GoToMove(moveIndex);
        }

        public IList<MoveData> GetVariationMoveData(IList<Move> moves)
        {
            return chessBoard.GetVariationMoveData(moves);
        }

        public bool GetIsInCheck()
        {
            return chessBoard.IsCheck();
        }

        public bool GetIsStalemate()
        {
            return chessBoard.IsStalemate();
        }

        public bool GetIsMate()
        {
            return chessBoard.IsCheckmate();
        }

        public string Serialize(BoardSerializationType type, bool stopOnCurrent = true)
        {
            return chessBoard.Serialize((int)type, stopOnCurrent);
        }

        public virtual bool LoadFrom(string serializedValue, int currentMoveIndex = -1)
        {
            bool result = chessBoard.LoadFrom(serializedValue);
            if (result)
                chessBoard.GoToMove(currentMoveIndex);

            return result;
        }
    }
}
