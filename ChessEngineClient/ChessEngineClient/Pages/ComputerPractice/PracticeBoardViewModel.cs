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
        }

        protected override bool OnSelectionChanged(SquareViewModel oldSquare, SquareViewModel newSquare)
        {
            bool result = base.OnSelectionChanged(oldSquare, newSquare);

            if (result && practiceBoardService.GetIsComputerTurn())
                practiceBoardService.RequestComputerMove();

            return result;
        }
    }
}
