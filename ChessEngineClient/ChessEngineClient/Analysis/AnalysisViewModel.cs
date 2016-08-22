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
        private string moves = String.Empty;
        private IAnalysisReceiver analysisReceiver = null;
        SynchronizationContext uiSynchronizationContext = null;
        private float evaluation = 0.0f;

        public float Evaluation
        {
            get { return evaluation; }
            set
            {
                if (evaluation != value)
                {
                    evaluation = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Moves
        {
            get { return moves; }
            set
            {
                if (moves != value)
                {
                    moves = value;
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
            // make sure it is executed on the ui thread
            uiSynchronizationContext.Post(o =>
            {
                Evaluation = e.Data.Score;
                Moves = String.Join(" ", e.Data.Analysis.Cast<object>());
            }, null);

            
        }
    }
}
