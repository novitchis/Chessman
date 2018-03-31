using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chessman.ViewModel
{
    public class PracticeBoardViewModel : AnalysisChessBoardViewModel
    {
        IPracticeBoardService practiceBoardService = null;

        public PracticeBoardViewModel(IPracticeBoardService practiceBoardService)
            : base(practiceBoardService)
        {
            this.practiceBoardService = practiceBoardService;
            this.practiceBoardService.AnalysisReceived += OnAnalysisReceived;
        }

        protected override void OnNewMoveExecuted()
        {
            if (practiceBoardService.IsComputerTurn())
                practiceBoardService.RequestComputerMove();
        }

        protected override void OnSelectionChanged(SquareViewModel oldSquare, SquareViewModel newSquare)
        {
            if (practiceBoardService.IsComputerTurn())
                return;

            base.OnSelectionChanged(oldSquare, newSquare);
        }

        public override void OnPieceDropped(SquareViewModel targetSquare)
        {
            if (practiceBoardService.IsComputerTurn())
                return;

            base.OnPieceDropped(targetSquare);
        }

        private async void OnAnalysisReceived(object sender, AnalysisEventArgs e)
        {
            // just delay for one second the move, to not be so fast
            await Task.Delay(1000);
            ExecuteCurrentMoveOnBoard(false);
            //TODO: select a square?
        }
    }
}
