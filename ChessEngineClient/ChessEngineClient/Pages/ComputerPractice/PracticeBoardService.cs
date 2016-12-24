using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
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

        public SideColor UserPerspective
        {
            get; set;
        }

        public PracticeBoardService(IEngine engine, IAnalysisReceiver analysisReceiver)
        {
            UserPerspective = SideColor.White;
            this.engine = engine;
            this.analysisReceiver = analysisReceiver;

            mainSynchronizationContext = SynchronizationContext.Current;
        }

        public override bool SubmitMove(Coordinate from, Coordinate to)
        {
            if (GetIsComputerTurn())
                return false;

            if (base.SubmitMove(from, to))
            {
                RequestComputerMoveAsync();
                return true;
            }

            return false;
        }

        private bool GetIsComputerTurn()
        {
            SideColor sideToMove = IsWhiteTurn ? SideColor.White : SideColor.Black;
            if (sideToMove == UserPerspective)
                return false;

            return true;
        }

        private void RequestComputerMoveAsync()
        {
            if (ChessBoard.IsStalemate() || ChessBoard.IsCheckmate())
                return;

            engine.Analyze(ChessBoard);
        }

        public void Start()
        {
            analysisReceiver.AnalysisReceived += OnAnalysisReceived;
            engine.SetAnalysisDepth(5);

            if (!GetIsComputerTurn())
                return;

            RequestComputerMoveAsync();
        }

        private void OnAnalysisReceived(object sender, AnalysisEventArgs e)
        {
            if (e.Data.IsBestMove)
            {
                base.SubmitMove(e.Data.Analysis[0].GetFrom(), e.Data.Analysis[0].GetTo());
                mainSynchronizationContext.Post(o =>
                {
                    // TODO: this should be done differently
                    Messenger.Default.Send<MessageBase>(new MessageBase(), NotificationMessages.MoveExecuted);
                }, null);
            }
        }

        public void Stop()
        {
            analysisReceiver.AnalysisReceived -= OnAnalysisReceived;
            engine.StopAnalyzing();
        }
    }
}
