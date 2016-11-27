using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine;

namespace ChessEngineClient
{
    public class ChessBoardService : IAnalysisBoardService
    {
        public const int FenSerializationType = 0;
        private ChessBoard chessBoard = null;
        private Engine engine = null;
        private IEngineNotification engineNotification = null;

        public bool IsWhiteTurn
        {
            get { return chessBoard.IsWhiteTurn(); }
        }

        public ChessBoardService(IEngineNotification engineNotification)
        {
            chessBoard = new ChessBoard();
            chessBoard.Initialize();
            chessBoard.StorePGN();

            this.engineNotification = engineNotification;
            engine = new Engine(engineNotification);
            engine.Start();
            RefreshAnalysis();
        }

        public void ResetBoard()
        {
            chessBoard.Initialize();
            RefreshAnalysis();
        }

        public void LoadFromFen(string fenString)
        {
            chessBoard.LoadFrom(fenString, FenSerializationType);
            RefreshAnalysis();
        }

        public string GetFen()
        {
            return chessBoard.Serialize(FenSerializationType);
        }

        public ChessPiece GetPiece(Coordinate coordinate)
        {
            return chessBoard.GetPiece(coordinate);
        }

        public bool SubmitMove(Coordinate from, Coordinate to)
        {
            bool result = chessBoard.SubmitMove(from, to);
            if (result)
                RefreshAnalysis();

            return result;
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

        public IList<MoveData> GetVariationMoveData(IList<Move> moves)
        {
            return chessBoard.GetVariationMoveData(moves);
        }

        public bool GoToMove(int moveIndex)
        {
            bool result = chessBoard.GoToMove(moveIndex);
            if (result)
                RefreshAnalysis();

            return result;
        }

        private void RefreshAnalysis()
        {
            if (!chessBoard.IsStalemate() && !chessBoard.IsCheckmate())
                engine.Analyze(chessBoard);
            else
                engineNotification.OnEngineStop();
        }
    }
}
