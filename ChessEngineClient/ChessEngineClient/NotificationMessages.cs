using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient
{
    public static class NotificationMessages
    {
        public const string CurrentMoveChanged = "CurrentMoveChanged";
        public const string AnalysisBestMoveReceived = "AnalysisBestMoveReceived";
        public const string MoveExecuted = "MoveExecuted";
        public const string AnimateMoveTaskCreated = "MoveExecuted";
        public const string AnimateUndoMoveTaskCreated = "UndoMoveExecuted";
        public const string GoBackExecuted = "GoBackExecuted";
        public const string GoForwardExecuted = "GoForwardExecuted";
        public const string SquarePressed = "SquarePressed";
    }
}
