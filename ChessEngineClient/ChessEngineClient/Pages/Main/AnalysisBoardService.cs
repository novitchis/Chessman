using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine;
using System.Threading;

namespace ChessEngineClient
{
    public class AnalysisBoardService : BoardService, IAnalysisBoardService
    {
        private IEngine engine = null;
        private IEngineNotification engineNotification = null;

        public AnalysisBoardService(IEngineNotification engineNotification, IEngine engine)
        {
            this.engineNotification = engineNotification;
            this.engine = engine;

            //SynchronizationContext mainSynchronizationContext = SynchronizationContext.Current;
            // this is a quick test to identify whether our crashes occur on initial engine start 
            //Task.Run(async () =>
            //{
            //    await Task.Delay(1500).ConfigureAwait(true);
            //    mainSynchronizationContext.Post(o => { Analyse(); }, null);
            //});
        }

        public override void ResetBoard()
        {
            base.ResetBoard();
            StartAnalysis();
        }

        public override void LoadFromFen(string fenString)
        {
            base.LoadFromFen(fenString);
            StartAnalysis();
        }

        public override bool SubmitMove(Coordinate from, Coordinate to)
        {
            bool result = base.SubmitMove(from, to);
            if (result)
                StartAnalysis();

            return result;
        }

        public override bool GoToMove(int moveIndex)
        {
            bool result = base.GoToMove(moveIndex);
            if (result)
                StartAnalysis();

            return result;
        }

        public void StartAnalysis()
        {
            if (!ChessBoard.IsStalemate() && !ChessBoard.IsCheckmate())
            {
                engineNotification.OnStateChanged(EngineState.Analyze);
                engine.Analyze(ChessBoard);
            }
            else
            {
                engineNotification.OnStateChanged(EngineState.Stop);
            }
        }

        public void StopAnalysis()
        {
            engine.StopAnalyzing();
        }
    }
}
