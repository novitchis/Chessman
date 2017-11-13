using ChessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace Chessman
{
    public class AnalysisReceiver : IAnalysisReceiver, IEngineNotification
    {
        public event AnalysisEventHandler AnalysisReceived;
        public event AnalysisStateEventHandler AnalysisStateChanged;

        public AnalysisReceiver()
        {
        }

        protected virtual void OnAnalysisReceived(AnalysisEventArgs e)
        {
            AnalysisReceived?.Invoke(this, e);
        }

        public void OnEngineMoveFinished(AnalysisData[] analysis)
        {
            OnAnalysisReceived(new AnalysisEventArgs() { AnalysisLines = analysis });
        }

        public void OnEngineError()
        {
            throw new NotImplementedException();
        }

        public void OnGameEnded(bool bWhiteWins)
        {
            //throw new Exception("Game ended: " + (bWhiteWins ? "white wins" : "black wins"));
        }

        public void OnStateChanged(EngineState state)
        {
            AnalysisStateChanged?.Invoke(this, new AnalysisStateEventArgs(state));
        }        
    }
}
