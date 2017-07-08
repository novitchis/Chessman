using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChessEngineClient
{
    public class PracticeBoardService : BoardService, IPracticeBoardService
    {
        private SynchronizationContext mainSynchronizationContext = null;
        private IEngine engine = null;
        private IAnalysisReceiver analysisReceiver = null;
        private int depth = -1;
        private int moveTime = -1;
        private bool isStarted = false;

        public event AnalysisEventHandler AnalysisReceived;

        public SideColor UserColor
        {
            get; private set;
        }

        public PracticeBoardService(IEngine engine, IAnalysisReceiver analysisReceiver)
        {
            UserColor = SideColor.White;
            this.engine = engine;
            this.analysisReceiver = analysisReceiver;

            mainSynchronizationContext = SynchronizationContext.Current;
        }

        public override bool SubmitMove(Coordinate from, Coordinate to)
        {
            return base.SubmitMove(from, to);
        }

        //TODO: this should not know anything about user color
        // move this to view model
        public void SwitchUserColor()
        {
            bool wasStarted = isStarted;
            if (wasStarted)
                Stop();

            UserColor = UserColor == SideColor.Black ? SideColor.White : SideColor.Black;

            if (wasStarted)
                Start();
        }

        public bool IsComputerTurn()
        {
            SideColor sideToMove = IsWhiteTurn ? SideColor.White : SideColor.Black;
            if (sideToMove == UserColor)
                return false;

            return true;
        }

        public void RequestComputerMove()
        {
            if (ChessBoard.IsStalemate() || ChessBoard.IsCheckmate())
                return;

            engine.Analyze(ChessBoard, depth, moveTime);
        }

        public void Start()
        {
            analysisReceiver.AnalysisReceived += OnAnalysisReceived;
            isStarted = true;

            if (IsComputerTurn())
                RequestComputerMove();
        }

        private void OnAnalysisReceived(object sender, AnalysisEventArgs e)
        {
            if (e.AnalysisLines[0].IsBestMove)
                mainSynchronizationContext.Post(o => AnalysisReceived?.Invoke(this, e), null);
        }

        public void Stop()
        {
            analysisReceiver.AnalysisReceived -= OnAnalysisReceived;
            engine.StopAnalyzing();
            isStarted = false;
        }

        public void SetEngineStrength(int strengthValue)
        {
            int skillLevel = 0;

            switch (strengthValue)
            {
                case 1:
                    skillLevel = 0; depth = 1; moveTime = 50; break;
                case 2:
                    skillLevel = 2; depth = 2; moveTime = 100; break;
                case 3:
                    skillLevel = 5; depth = 3; moveTime = 150; break;
                case 4:
                    skillLevel = 7; depth = 4; moveTime = 200; break;
                case 5:
                    skillLevel = 10; depth = 5; moveTime = 250; break;
                case 6:
                    skillLevel = 12; depth = 7; moveTime = 300; break;
                case 7:
                    skillLevel = 14; depth = 10; moveTime = 350; break;
                case 8:
                    skillLevel = 16; depth = 12; moveTime = 500; break;
                case 9:
                    skillLevel = 20; depth = 14; moveTime = 1000; break;
                case 10:
                    skillLevel = 20; depth = 16; moveTime = 2000; break;
                default:
                    break;
            }

            engine.SetOptions(new EngineOptions() { SkillLevel = skillLevel });
        }
    }
}
