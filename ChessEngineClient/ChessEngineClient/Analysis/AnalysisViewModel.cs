﻿using ChessEngine;
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
        private IChessBoardService chessBoardService = null;
        private IAnalysisReceiver analysisReceiver = null;
        private string moves = String.Empty;
        SynchronizationContext uiSynchronizationContext = null;
        private string evaluation = "0.0";

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

        #endregion

        public AnalysisViewModel(IChessBoardService chessBoardService, IAnalysisReceiver analysisReceiver)
        {
            this.chessBoardService = chessBoardService;
            this.analysisReceiver = analysisReceiver;
            this.analysisReceiver.AnalysisReceived += OnAnalysisReceived;
            this.analysisReceiver.AnalysisStopped += OnAnalysisStopped;

            uiSynchronizationContext = SynchronizationContext.Current;
        }

        private void OnAnalysisStopped(object sender, AnalysisEventArgs e)
        {
            Evaluation = "-";
            Moves = string.Empty;
        }

        private void OnAnalysisReceived(object sender, AnalysisEventArgs e)
        {
            // make sure it is executed on the ui thread
            uiSynchronizationContext.Post(o =>
            {
                try
                {
                    Moves = GetEvaluationVariationString(e.Data);
                    Evaluation = e.Data.Score > 0 ? String.Format("+{0}", e.Data.Score) : e.Data.Score.ToString();
                    if (Moves.Trim().EndsWith("#"))
                        Evaluation = GetMateEvaluation();
                }
                catch
                {
                }
            }, null);
        }

        private string GetMateEvaluation()
        {
            int moveCount = Moves.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Where(p => !p.Contains('.')).Count();
            string sign = "+";

            if (moveCount % 2 == 0)
                sign = "-";

            //if (!chessBoardService.WasBlackFirstToMove() && moveCount % 2 == 0 ||
            //    chessBoardService.WasBlackFirstToMove() && moveCount % 2 != 0)
            //    sign = "-";

            return sign + "M" + moveCount.ToString();
        }

        private string GetEvaluationVariationString(AnalysisData data)
        {
            StringBuilder variationBuilder = new StringBuilder();

            var analysisVariation = chessBoardService.GetVariationMoveData(data.Analysis);
            MoveData firstMove = analysisVariation.First();

            bool isFirstMoveProcesssed = false;
            foreach (MoveData moveData in chessBoardService.GetVariationMoveData(data.Analysis))
            {
                int actualMoveindex = moveData.Index;
                if (chessBoardService.WasBlackFirstToMove())
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
