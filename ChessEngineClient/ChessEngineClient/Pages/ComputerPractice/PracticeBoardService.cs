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

        public void SwitchUserColor()
        {
            Stop();
            UserColor = UserColor == SideColor.Black ? SideColor.White : SideColor.Black;
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
            // TODO: set in a different way the engine strength

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
        }

        public void SetEngineStrength(int strengthValue)
        {
            // skill level : 0..20
            engine.SetOptions(new EngineOptions() { SkillLevel = strengthValue * 2});
            // this can be further adjusted
            secondsLeft = strengthValue * 20;
        }
    }
}
