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
        private IAnalysisBoardService analysisBoardService = null;
        private IAnalysisReceiver analysisReceiver = null;
        private string moves = String.Empty;
        private SynchronizationContext uiSynchronizationContext = null;
        private string evaluation = "0.0";
        private bool isActive = true;

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

        public AnalysisViewModel(IAnalysisBoardService analysisBoardService, IAnalysisReceiver analysisReceiver)
        {
            this.analysisBoardService = analysisBoardService;
            this.analysisReceiver = analysisReceiver;
            this.analysisReceiver.AnalysisReceived += OnAnalysisReceived;
            this.analysisReceiver.AnalysisStopped += OnAnalysisStopped;

            uiSynchronizationContext = SynchronizationContext.Current;
        }

        private void OnAnalysisStopped(object sender, AnalysisEventArgs e)
        {
            IsActive = false;
            Evaluation = "-";
            Moves = string.Empty;
        }

        private void OnAnalysisReceived(object sender, AnalysisEventArgs e)
        {
            try
            {
                string newEvalutation = e.Data.Score > 0 ? $"+{e.Data.Score}" : e.Data.Score.ToString();
                string newMoves = GetEvaluationVariationString(e.Data);
                if (newMoves.IndexOf('#') > -1)
                    newEvalutation = GetMateEvaluation();

                // make sure it is executed on the ui thread
                uiSynchronizationContext.Post(o =>
                {
                    IsActive = true;
                    Evaluation = newEvalutation;
                    Moves = newMoves;
                }, null);
            }
            catch
            {
            }
        }

        private string GetMateEvaluation()
        {
            int moveCount = Moves.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Where(p => !p.Contains('.')).Count();
            string sign = "+";

            if (moveCount % 2 == 0)
                sign = "-";

            return sign + "M" + Math.Ceiling(((float)moveCount) / 2).ToString();
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
