﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
{
    public class EditChessBoardViewModel: ChessBoardViewModel
    {
        private IEditorBoardService editorBoardService = null;

        public EditChessBoardViewModel(IEditorBoardService editorBoardService)
            : base(editorBoardService)
        {
            this.editorBoardService = editorBoardService;
            base.InitBoard();
        }
    }
}
