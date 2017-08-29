using ChessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
{
    public class PromotionMoveTask
    {
        //public Action OnTransitionCompleted
        //{
        //    get; set;
        //}

        public PieceType PromotionPieceType { get; set; }

        public Move Move { get; private set; }

        public PromotionMoveTask(Move move)
        {
            Move = move;
        }
    }
}
