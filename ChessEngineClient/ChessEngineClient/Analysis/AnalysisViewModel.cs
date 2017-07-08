using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
{
    public class AnalysisViewModel : ViewModelBase
    {
        private IEngineBoardService analysisBoardService = null;
        private IAnalysisReceiver analysisReceiver = null;
        private SynchronizationContext uiSynchronizationContext = null;
        private string evaluation = "-";
        private string depth = "";
        private bool isActive = false;
        private List<AnalysisLineViewModel> analysisLines = null;

        #region "Properties"

        public string Evaluation
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

        public string Depth
        {
            get { return depth; }
            set
            {
                if (depth != value)
                {
                    depth = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsActive
        {
            get { return isActive; }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public List<AnalysisLineViewModel> AnalysisLines
        {
            get { return analysisLines; }
            set
            {
                if (analysisLines != value)
                {
                    analysisLines = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        public AnalysisViewModel(IEngineBoardService analysisBoardService, IAnalysisReceiver analysisReceiver)
        {
            this.analysisBoardService = analysisBoardService;
            this.analysisReceiver = analysisReceiver;
            uiSynchronizationContext = SynchronizationContext.Current;
        }

        public void SubscribeToAnalysis()
        {
            analysisReceiver.AnalysisReceived += OnAnalysisReceived;
            analysisReceiver.AnalysisStateChanged += OnAnalysisStateChanged;
        }

        public void UnsubscribeToAnalysis()
        {
            analysisReceiver.AnalysisReceived -= OnAnalysisReceived;
            analysisReceiver.AnalysisStateChanged -= OnAnalysisStateChanged;
        }

        private void OnAnalysisStateChanged(object sender, AnalysisStateEventArgs e)
        {
            switch (e.NewState)
            {
                case EngineState.Analyze:
                    IsActive = true;
                    break;
                case EngineState.Stop:
                    OnAnalysisStopped();
                    break;
                case EngineState.Start:
                default:
                    break;
            }
        }

        private void OnAnalysisStopped()
        {
            IsActive = false;
            Evaluation = "-";
            AnalysisLines = null;
        }

        private void OnAnalysisReceived(object sender, AnalysisEventArgs e)
        {
            // since the engine may not be stopped in time and the board can be already changed, 
            // some variations may throw exceptions when getting the pgn from the current board
            try
            {
                UpdateAnalysisLines(e.AnalysisLines);
            }
            catch
            {
            }
        }

        private void UpdateAnalysisLines(AnalysisData[] analysis)
        {
            // we can get duplicated varitaions for the same line
            Dictionary<int, AnalysisData> multiPvToLineMap = analysis.ToDictionary(l => l.MultiPV);

            List<AnalysisLineViewModel> newAnalysisLinesVM = new List<AnalysisLineViewModel>();

            foreach (int multiPv in multiPvToLineMap.Keys.OrderBy(key => key))
            {
                // ignore best move since it does not have the entire line moves
                //if (AnalysisLines.IsBestMove)
                //    return;

                AnalysisData analysisLine = multiPvToLineMap[multiPv];

                SideColor gameStartedBy = analysisBoardService.WasBlackFirstToMove() ? SideColor.Black : SideColor.White;
                AnalysisLineViewModel lineVm = new AnalysisLineViewModel(gameStartedBy, analysisLine.Score, analysisBoardService.GetVariationMoveData(analysisLine.Analysis));

                newAnalysisLinesVM.Add(lineVm);
            }

            // make sure it is executed on the ui thread
            uiSynchronizationContext.Post(o =>
            {
                if (!IsActive)
                    return;

                // notify best move received for arrow rendering
                Messenger.Default.Send(new GenericMessage<Move>(this, analysis[0].Analysis[0]), NotificationMessages.AnalysisBestMoveReceived);

                AnalysisLines = newAnalysisLinesVM;

                //TODO: can get rid of this?
                Evaluation = AnalysisLines[0].Evaluation;
                Depth = $"Depth {analysis[0].Depth}";

            }, null);

        }
    }
}
