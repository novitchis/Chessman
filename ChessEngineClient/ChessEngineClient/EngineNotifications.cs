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
            throw new NotImplementedException();
        }

        public void OnEngineMoveFinished(Move move)
        {
            throw new NotImplementedException();
        }

        public void OnGameEnded(bool bWhiteWins)
        {
            throw new NotImplementedException();
        }
    }
}
