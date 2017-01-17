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
        private int secondsLeft = -1;
        private bool isStarted = false;

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
            if (GetIsComputerTurn())
                return false;

            return base.SubmitMove(from, to);
        }

        public void SwitchUserColor()
        {
            bool wasStarted = isStarted;
            if (wasStarted)
                Stop();

            UserColor = UserColor == SideColor.Black ? SideColor.White : SideColor.Black;

            if (wasStarted)
                Start();
        }

        public bool GetIsComputerTurn()
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

            engine.Analyze(ChessBoard, secondsLeft);
        }

        public void Start()
        {
            analysisReceiver.AnalysisReceived += OnAnalysisReceived;
            isStarted = true;

            if (!GetIsComputerTurn())
                return;

            RequestComputerMove();
        }

        private void OnAnalysisReceived(object sender, AnalysisEventArgs e)
        {
            if (e.Data.IsBestMove)
            {
                base.SubmitMove(e.Data.Analysis[0].GetFrom(), e.Data.Analysis[0].GetTo());
                mainSynchronizationContext.Post(o =>
                {
                    // TODO: this should be done differently
                    Messenger.Default.Send(new MessageBase(this, this), NotificationMessages.MoveExecuted);
                }, null);
            }
        }

        public void Stop()
        {
            analysisReceiver.AnalysisReceived -= OnAnalysisReceived;
            engine.StopAnalyzing();
            isStarted = false;
        }

        public void SetEngineStrength(int strengthValue)
        {
            // reajust the engine skill level since the level 2 is already too strong
            // strength(1..10) - skill level(0..20)
            int skillLevel = 0;
            if (strengthValue <= 3)
                skillLevel = strengthValue - 1;
            else if (strengthValue < 6)
                skillLevel = (strengthValue - 1) * 2;
            else
                skillLevel = strengthValue * 2;

            engine.SetOptions(new EngineOptions() { SkillLevel = skillLevel });
            // this can be further adjusted
            secondsLeft = skillLevel;
        }
    }
}
