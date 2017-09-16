using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine;
using System.Threading;
using ChessEngineClient.Extensions;

namespace ChessEngineClient
{
    public class AnalysisBoardService : BoardService, IAnalysisBoardService
    {
        private IEngine engine = null;
        private IEngineNotification engineNotification = null;
        private bool isStarted = false;
        private Action debouncedAnalysisAction = null;
        private int linesCount = 2;

        public AnalysisBoardService(IEngineNotification engineNotification, IEngine engine)
        {
            this.engineNotification = engineNotification;
            this.engine = engine;

            debouncedAnalysisAction = DebounceExtension.Debounce(() =>
            {
                System.Diagnostics.Debug.WriteLine("Analysis");
                engine.Analyze(ChessBoard, -1, -1);
            });
        }

        public void SetAnalysisLines(int linesCount)
        {
            this.linesCount = linesCount;
        }

        public override void ResetBoard()
        {
            // the engine stops if is analyzing
            if (isStarted)
                engine.StopAnalyzing();

            base.ResetBoard();

            if (isStarted)
                AnalyseCurrentPosition();
        }

        public override bool SubmitMove(Coordinate from, Coordinate to)
        {
            bool result = base.SubmitMove(from, to);
            if (result)
                AnalyseCurrentPosition();

            return result;
        }

        public override bool SubmitPromotionMove(Coordinate from, Coordinate to, ChessPiece piece)
        {
            bool result = base.SubmitPromotionMove(from, to, piece);
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
            isStarted = true;

            // if is not maximum the analyze infinite command has some issues
            engine.SetOptions(new EngineOptions() { SkillLevel = 20, MultiPV = linesCount });

            AnalyseCurrentPosition();
        }

        private void AnalyseCurrentPosition()
        {
            if (!isStarted)
                return;

            if (!ChessBoard.IsStalemate() && !ChessBoard.IsCheckmate())
            {
                engineNotification.OnStateChanged(EngineState.Analyze);
                debouncedAnalysisAction();
            }
            else
            {
                engineNotification.OnStateChanged(EngineState.Stop);
            }
        }

        public void Stop()
        {
            isStarted = false;
            engine.StopAnalyzing();
        }

        public override bool LoadFrom(string serializedValue, int currentMoveIndex = -1)
        {
            // the engine stops if is analyzing
            if (isStarted)
                engine.StopAnalyzing();

            bool result = base.LoadFrom(serializedValue, currentMoveIndex);

            if (isStarted)
                AnalyseCurrentPosition();

            return result;
        }
    }
}
