using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
{
    public class AnalysisLineViewModel : ViewModelBase
    {
        private SideColor gameStartedBy = SideColor.White;
        private bool isWhiteFirstMove = true;
        private AnalysisData analysisData = null;
        private IList<MoveData> moves = new List<MoveData>();

        #region "Properties"

        public int Depth
        {
            get
            {
                if (analysisData == null)
                    return 0;
                    
                return analysisData.Depth;
            }
        }

        public string Evaluation
        {
            get
            {
                if (analysisData == null)
                    return null;

                float whiteScoreEvaluation = isWhiteFirstMove ? analysisData.Score : analysisData.Score * -1;
                string evaluation = whiteScoreEvaluation > 0 ? $"+{whiteScoreEvaluation}" : whiteScoreEvaluation.ToString();
                if (moves.Last().PgnMove.TrimEnd().EndsWith("#"))
                {
                    string sign = "-";

                    if ((moves.Count % 2 != 0 && isWhiteFirstMove) ||
                        (moves.Count % 2 == 0 && !isWhiteFirstMove))
                    {
                        sign = "+";
                    }

                    return sign + "M" + Math.Ceiling(((float)moves.Count) / 2).ToString();
                }

                return evaluation;
            }
        }

        public string MovesString
        {
            get
            {
                if (analysisData == null)
                    return String.Empty;

                StringBuilder variationBuilder = new StringBuilder();

                bool isFirstMoveProcesssed = false;
                foreach (MoveData moveData in moves)
                {
                    int actualMoveindex = moveData.Index;
                    if (gameStartedBy == SideColor.Black)
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

        public bool IsLastItem { get; set; }

        public bool DisplayEvaluation { get; set; } = true;

        #endregion

        public AnalysisLineViewModel() { }

        public AnalysisLineViewModel(SideColor gameStartedBy, AnalysisData analysisData, IList<MoveData> moves)
        {
            this.gameStartedBy = gameStartedBy;
            this.analysisData = analysisData;

            this.isWhiteFirstMove = gameStartedBy == SideColor.White && moves.First().Index % 2 == 0 ||
                gameStartedBy == SideColor.Black && moves.First().Index % 2 != 0;

            this.moves = moves;
        }
    }
}
