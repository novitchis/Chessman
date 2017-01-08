using ChessEngine;
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
        private IEngineBoardService analysisBoardService = null;
        private IAnalysisReceiver analysisReceiver = null;
        private string moves = String.Empty;
        private SynchronizationContext uiSynchronizationContext = null;
        private string evaluation = "-";
        private bool isActive = false;

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
            Moves = string.Empty;
        }

        private void OnAnalysisReceived(object sender, AnalysisEventArgs e)
        {
            try
            {
                // ignore best move since it does not have the entire line moves
                if (e.Data.IsBestMove)
                    return;

                float whiteScoreEvaluation = analysisBoardService.IsWhiteTurn ? e.Data.Score : e.Data.Score * -1;
                string newEvalutation = whiteScoreEvaluation > 0 ? $"+{whiteScoreEvaluation}" : whiteScoreEvaluation.ToString();
                string newMoves = GetEvaluationVariationString(e.Data);
                if (newMoves.TrimEnd().EndsWith("#"))
                    newEvalutation = GetMateEvaluation(e.Data);

                // make sure it is executed on the ui thread
                uiSynchronizationContext.Post(o =>
                {
                    if (!IsActive)
                        return;

                    Evaluation = newEvalutation;
                    Moves = newMoves;
                }, null);
            }
            catch
            {
            }
        }

        private string GetMateEvaluation(AnalysisData data)
        {
            string sign = "-";

            if ((data.Analysis.Length % 2 != 0 && analysisBoardService.IsWhiteTurn) ||
                (data.Analysis.Length % 2 == 0 && !analysisBoardService.IsWhiteTurn))
            {
                sign = "+";
            }

            return sign + "M" + Math.Ceiling(((float)data.Analysis.Length) / 2).ToString();
        }

        private string GetEvaluationVariationString(AnalysisData data)
        {
            StringBuilder variationBuilder = new StringBuilder();

            bool isFirstMoveProcesssed = false;
            foreach (MoveData moveData in analysisBoardService.GetVariationMoveData(data.Analysis))
            {
                int actualMoveindex = moveData.Index;
                if (analysisBoardService.WasBlackFirstToMove())
                    actualMoveindex++;

                bool isWhiteMove = actualMoveindex % 2 == 0;
                int moveGroupNumber = actualMoveindex / 2 + 1;

                if (isWhiteMove)
                    variationBuilder.AppendFormat("{0}. {1}", moveGroupNumber, moveData.PgnMove);
                else if (!isFirstMoveProcesssed)
                    variationBuilder.AppendFormat("{0}. ... {1} ", moveGroupNumber, moveData.PgnMove);
                else
                    variationBuilder.AppendFormat(" {0} ", moveData.PgnMove);

                isFirstMoveProcesssed = true;
            }

            return variationBuilder.ToString();
        }
    }
}
