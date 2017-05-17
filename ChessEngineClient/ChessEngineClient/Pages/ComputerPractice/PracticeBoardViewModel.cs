using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
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

        protected override bool TryExecuteMove(Coordinate fromCoordinate, Coordinate toCoordinate, bool useAnimations)
        {
            bool result = base.TryExecuteMove(fromCoordinate, toCoordinate, useAnimations);
            if (result && practiceBoardService.IsComputerTurn())
                practiceBoardService.RequestComputerMove();

            return result;
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
            TryExecuteMove(e.Data.Analysis[0].GetFrom(), e.Data.Analysis[0].GetTo(), true);
            //TODO: select a square?
        }
    }
}
