using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chessman.ViewModel
{
    public class TacticsChessBoardViewModel : AnalysisChessBoardViewModel
    {
        ITacticsBoardService tacticsBoardService = null;

        public TacticsChessBoardViewModel(ITacticsBoardService tacticsBoardService)
            : base(tacticsBoardService)
        {
            this.tacticsBoardService = tacticsBoardService;
        }

        protected override void OnNewMoveExecuted()
        {
        }

        protected override void OnSelectionChanged(SquareViewModel oldSquare, SquareViewModel newSquare)
        {
            if (tacticsBoardService.IsComputerTurn())
                return;

            base.OnSelectionChanged(oldSquare, newSquare);
        }

        public override void OnPieceDropped(SquareViewModel sourceSquare, SquareViewModel targetSquare)
        {
            if (tacticsBoardService.IsComputerTurn())
                return;

            base.OnPieceDropped(sourceSquare, targetSquare);
        }
    }
}
