using ChessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace ChessEngineClient
{
    public class AnalysisReceiver : IAnalysisReceiver, IEngineNotification
    {
        public event AnalysisEventHandler AnalysisReceived;
        public event AnalysisEventHandler AnalysisStopped;

        public AnalysisReceiver()
        {
        }

        protected virtual void OnAnalysisReceived(AnalysisEventArgs e)
        {
            AnalysisReceived?.Invoke(this, e);
        }

        public void OnEngineError()
        {
            throw new NotImplementedException();
        }

        public void OnEngineMoveFinished(Move move, AnalysisData analysis)
        {
            OnAnalysisReceived(new AnalysisEventArgs() { Data = analysis });
        }

        public void OnGameEnded(bool bWhiteWins)
        {
            //throw new NotImplementedException();
        }

        public void OnEngineStop()
        {
            AnalysisStopped?.Invoke(this, new AnalysisEventArgs());
        }
    }
}
