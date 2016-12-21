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
    //TODO: rename to practice
    public class ExerciseBoardService : BoardService, IExerciseBoardService
    {
        private SynchronizationContext mainSynchronizationContext = null;
        private IEngine engine = null;

        public SideColor UserPerspective
        {
            get; set;
        }

        public ExerciseBoardService(IEngine engine)
        {
            UserPerspective = SideColor.White;
            this.engine = engine;

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

        private async void RequestComputerMoveAsync()
        {
            engine.Analyze(ChessBoard);
            await Task.Delay(2000);
            engine.StopAnalyzing();
        }

        //public void OnEngineMoveFinished(Move move, AnalysisData analysis)
        //{
        //    if (analysis.IsBestMove)
        //    {
        //        base.SubmitMove(move.GetFrom(), move.GetTo());
        //        mainSynchronizationContext.Post(o => 
        //        {
        //           Messenger.Default.Send<MessageBase>(new MessageBase(), NotificationMessages.MoveExecuted);
        //        }, null);
        //    }
        //}
    }
}
