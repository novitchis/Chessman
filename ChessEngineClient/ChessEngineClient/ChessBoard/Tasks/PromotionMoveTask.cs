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
        public Action<PieceType> OnPieceSelected { get; private set; }

        public Move Move { get; private set; }

        public PromotionMoveTask(Move move, Action<PieceType> onPieceSelected)
        {
            OnPieceSelected = onPieceSelected;
            Move = move;
        }
    }
}
