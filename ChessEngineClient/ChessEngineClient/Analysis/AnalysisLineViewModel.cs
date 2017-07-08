using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
{
    public class AnalysisLineViewModel : ViewModelBase
    {
        private SideColor gameStartedBy = SideColor.White;
        private bool isWhiteFirstMove = true;
        private float score = .0f;

        public IList<MoveData> Moves { get; set; }

        public string Evaluation
        {
            get
            {
                float whiteScoreEvaluation = isWhiteFirstMove ? score : score * -1;
                string evaluation = whiteScoreEvaluation > 0 ? $"+{whiteScoreEvaluation}" : whiteScoreEvaluation.ToString();
                if (Moves.Last().PgnMove.TrimEnd().EndsWith("#"))
                {
                    string sign = "-";

                    if ((Moves.Count % 2 != 0 && isWhiteFirstMove) ||
                        (Moves.Count % 2 == 0 && !isWhiteFirstMove))
                    {
                        sign = "+";
                    }

                    return sign + "M" + Math.Ceiling(((float)Moves.Count) / 2).ToString();
                }

                return evaluation;
            }
        }

        public string MovesString
        {
            get
            {
                StringBuilder variationBuilder = new StringBuilder();

                bool isFirstMoveProcesssed = false;
                foreach (MoveData moveData in Moves)
                {
                    int actualMoveindex = moveData.Index;
                    if (gameStartedBy == SideColor.White)
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

        public AnalysisLineViewModel(SideColor gameStartedBy, float score, IList<MoveData> moves)
        {
            this.gameStartedBy = gameStartedBy;
            this.score = score;

            this.isWhiteFirstMove = gameStartedBy == SideColor.White && moves.First().Index % 2 == 0 ||
                gameStartedBy == SideColor.Black && moves.First().Index % 2 != 0;

            Moves = moves;
        }
    }
}
