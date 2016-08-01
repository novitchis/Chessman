using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine;

namespace ChessEngineClient
{
    public class EngineNotifications : ChessEngine.IEngineNotification
    {
        public void OnEngineError()
        {
            
        }

        public void OnEngineMoveFinished(Move move, AnalysisData analysis)
        {
        }

        public void OnGameEnded(bool bWhiteWins)
        {
        }
    }
}
