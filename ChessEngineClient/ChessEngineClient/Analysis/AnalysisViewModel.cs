using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
{
    public class AnalysisViewModel : ViewModelBase
    {
        private string analysisString = String.Empty;
        private IAnalysisReceiver analysisReceiver = null;
        SynchronizationContext uiSynchronizationContext = null;

        public string AnalysisString
        {
            get { return analysisString; }
            set
            {
                if (analysisString != value)
                {
                    analysisString = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public AnalysisViewModel(IAnalysisReceiver analysisReceiver)
        {
            this.analysisReceiver = analysisReceiver;
            this.analysisReceiver.AnalysisReceived += OnAnalysisReceived;

            uiSynchronizationContext = SynchronizationContext.Current;
        }

        private void OnAnalysisReceived(object sender, AnalysisEventArgs e)
        {
            string value = String.Format("{0} ", e.Data.Score);
            value += String.Join(" ", e.Data.Analysis.Cast<object>());

            // make sure it is executed on the ui thread
            uiSynchronizationContext.Post(o =>
            {
                AnalysisString = value;
            }, null);

            
        }
    }
}
