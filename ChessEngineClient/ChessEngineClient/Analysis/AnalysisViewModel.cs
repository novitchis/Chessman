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
        private IAnalysisBoardService analysisBoardService = null;
        private IAnalysisReceiver analysisReceiver = null;
        private SynchronizationContext uiSynchronizationContext = null;
        private bool isEngineOn = false;
        private bool isActive = false;
        private List<AnalysisLineViewModel> analysisLines = null;

        #region "Properties"

        public bool IsEngineOn
        {
            get => isEngineOn;
            set
            {
                if (isEngineOn != value)
                {
                    isEngineOn = value;
                    ChangeEngineOnOff(isEngineOn);
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

        public int LinesCount { get; private set; } = 2;

        #endregion

        public AnalysisViewModel(IAnalysisBoardService analysisBoardService, IAnalysisReceiver analysisReceiver)
        {
            this.analysisBoardService = analysisBoardService;
            this.analysisReceiver = analysisReceiver;
            uiSynchronizationContext = SynchronizationContext.Current;
        }

        private void InitiateEmptyLines()
        {
            AnalysisLines = GetEmptyLinesVM().ToList();
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

        private void ChangeEngineOnOff(bool isEngineOn)
        {
            if (isEngineOn)
            {
                IsActive = true;
                InitiateEmptyLines();
                analysisBoardService.Start();
            }
            else
            {
                IsActive = false;
                AnalysisLines = null;
                analysisBoardService.Stop();
            }

            Messenger.Default.Send(new GenericMessage<bool>(isEngineOn), NotificationMessages.AnalysisIsOnChanged);
        }

        public void SetAnalysisLines(int linesCount)
        {
            LinesCount = linesCount;
            analysisBoardService.SetAnalysisLines(linesCount);
            if (IsEngineOn)
                InitiateEmptyLines();
        }

        private void OnAnalysisStateChanged(object sender, AnalysisStateEventArgs e)
        {
            switch (e.NewState)
            {
                case EngineState.Analyze:
                    IsActive = true;
                    break;
                case EngineState.Stop:
                    IsActive = false;
                    InitiateEmptyLines();
                    break;
                case EngineState.Start:
                default:
                    break;
            }
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
            // ignore best move since it does not contain all analysis moves
            if (analysis[0].IsBestMove)
                return;

            List<AnalysisLineViewModel> newAnalysiLines = GetAnalysisLineVMs(analysis);

            // analysis is received on a background thread
            uiSynchronizationContext.Post(o =>
            {
                // this check has to be on the UI thread to avoid race conditions
                if (!IsActive)
                    return;

                AnalysisLines = newAnalysiLines;
                Messenger.Default.Send(new GenericMessage<Move>(this, analysis[0].Analysis[0]), NotificationMessages.AnalysisBestMoveReceived);

            }, null);
        }

        private List<AnalysisLineViewModel> GetAnalysisLineVMs(AnalysisData[] analysis)
        {
            List<AnalysisLineViewModel> result = GetEmptyLinesVM().ToList();

            foreach (AnalysisData lineData in analysis)
            {
                SideColor gameStartedBy = analysisBoardService.WasBlackFirstToMove() ? SideColor.Black : SideColor.White;
                var moves = analysisBoardService.GetVariationMoveData(lineData.Analysis);
                result[lineData.MultiPV - 1] = new AnalysisLineViewModel(gameStartedBy, lineData, moves) { IsLastItem = lineData.MultiPV == LinesCount };
            }

            if (LinesCount == 1)
                result[0].DisplayEvaluation = false;

            return result;
        }

        private IEnumerable<AnalysisLineViewModel> GetEmptyLinesVM()
        {
            return Enumerable.Range(0, LinesCount).Select(i => new AnalysisLineViewModel() { IsLastItem = i == LinesCount - 1 });
        }
    }
}
