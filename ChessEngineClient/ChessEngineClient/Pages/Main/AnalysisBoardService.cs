using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine;
using System.Threading;

namespace ChessEngineClient
{
    public class AnalysisBoardService : BoardService, IEngineBoardService
    {
        private IEngine engine = null;
        private IEngineNotification engineNotification = null;

        public AnalysisBoardService(IEngineNotification engineNotification, IEngine engine)
        {
            this.engineNotification = engineNotification;
            this.engine = engine;
        }

        public override void ResetBoard()
        {
            base.ResetBoard();
            AnalyseCurrentPosition();
        }

        public override void LoadFromFen(string fenString)
        {
            base.LoadFromFen(fenString);
            AnalyseCurrentPosition();
        }

        public override bool SubmitMove(Coordinate from, Coordinate to)
        {
            bool result = base.SubmitMove(from, to);
            if (result)
                AnalyseCurrentPosition();

            return result;
        }

        public override bool GoToMove(int moveIndex)
        {
            bool result = base.GoToMove(moveIndex);
            if (result)
                AnalyseCurrentPosition();

            return result;
        }

        public void Start()
        {
            AnalyseCurrentPosition();
        }

        private void AnalyseCurrentPosition()
        {
            if (!ChessBoard.IsStalemate() && !ChessBoard.IsCheckmate())
            {
                engineNotification.OnStateChanged(EngineState.Analyze);
                engine.Analyze(ChessBoard, -1);
            }
            else
            {
                engineNotification.OnStateChanged(EngineState.Stop);
            }
        }

        public void Stop()
        {
            engine.StopAnalyzing();
        }
    }
}
