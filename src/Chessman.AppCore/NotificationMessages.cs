using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chessman
{
    public static class NotificationMessages
    {
        public const string AnalysisIsOnChanged = "AnalysisIsOnChanged";
        public const string CurrentMoveChanged = "CurrentMoveChanged";
        public const string AnalysisBestMoveReceived = "AnalysisBestMoveReceived";
        public const string MoveExecuted = "MoveExecuted";

        // tasks
        public const string AnimatePromotionMoveTask = "AnimatePromotionMoveTask";
        public const string AnimateMoveTaskCreated = "AnimateMoveTaskCreated";
        public const string AnimateUndoMoveTaskCreated = "AnimateUndoMoveTaskCreated";

        public const string GoBackExecuted = "GoBackExecuted";
        public const string GoForwardExecuted = "GoForwardExecuted";
        public const string SquarePressed = "SquarePressed";
    }
}
