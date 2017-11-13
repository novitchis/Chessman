using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chessman.ViewModel
{
    public class EditChessBoardViewModel: ChessBoardViewModel
    {
        private IBoardEditorService editorBoardService = null;

        public EditChessBoardViewModel(IBoardEditorService editorBoardService)
            : base(editorBoardService)
        {
            this.editorBoardService = editorBoardService;
            base.InitBoard();
        }
    }
}
