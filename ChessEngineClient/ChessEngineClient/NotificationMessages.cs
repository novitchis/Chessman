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
        public const string GoBack = "GoBack";
        public const string GoForward = "GoForward";
        public const string SquarePressed = "SquarePressed";
    }
}
