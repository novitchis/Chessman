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
        private const int LinesCount = 2;

        private IEngineBoardService analysisBoardService = null;
        private IAnalysisReceiver analysisReceiver = null;
        private SynchronizationContext uiSynchronizationContext = null;
        private bool isActive = false;
        private List<AnalysisLineViewModel> analysisLines = null;
        private bool isGameOver = false;

        #region "Properties"

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

        public bool IsGameOver
        {
            get { return isGameOver; }
            set
            {
                if (isGameOver != value)
                {
                    isGameOver = value;
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

            analysisLines = new List<AnalysisLineViewModel>(GetEmptyLinesVM(LinesCount));
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

            IsGameOver = analysisBoardService.GetIsMate() || analysisBoardService.GetIsStalemate();
        }

        private void OnAnalysisStopped()
        {
            IsActive = false;
            AnalysisLines = GetEmptyLinesVM(LinesCount).ToList();
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
            if (!IsActive)
                return;

            List<AnalysisLineViewModel> newAnalysiLines = GetAnalysisLineVMs(analysis);

            // analysis is received on a background thread
            uiSynchronizationContext.Post(o =>
            {
                AnalysisLines = newAnalysiLines;
                Messenger.Default.Send(new GenericMessage<Move>(this, analysis[0].Analysis[0]), NotificationMessages.AnalysisBestMoveReceived);

            }, null);
        }

        private List<AnalysisLineViewModel> GetAnalysisLineVMs(AnalysisData[] analysis)
        {
            List<AnalysisLineViewModel> result = GetEmptyLinesVM(LinesCount).ToList();

            foreach (AnalysisData lineData in analysis)
            {
                SideColor gameStartedBy = analysisBoardService.WasBlackFirstToMove() ? SideColor.Black : SideColor.White;
                var moves = analysisBoardService.GetVariationMoveData(lineData.Analysis);
                result[lineData.MultiPV - 1] = new AnalysisLineViewModel(gameStartedBy, lineData, moves);
            }

            return result;

        }

        private IEnumerable<AnalysisLineViewModel> GetEmptyLinesVM(int count)
        {
            return Enumerable.Range(0, count).Select(i => new AnalysisLineViewModel() { IsLastItem = i == count });
        }
    }
}
