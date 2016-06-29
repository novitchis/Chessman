using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
{
    public class NotationViewModel : ViewModelBase
    {
        private IChessBoardService chessBoardService = null;
        private IList<string> moves = null;

        #region Properties

        public IList<string> Moves
        {
            get { return moves; }
            set
            {
                if (value != moves)
                {
                    moves = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        public NotationViewModel(IChessBoardService chessBoardService)
        {
            this.chessBoardService = chessBoardService;
            this.chessBoardService.MoveExecuted += OnMoveExecuted;
        }

        private void OnMoveExecuted(object sender, ChessEventArgs e)
        {
            Moves = chessBoardService.GetMoves().Select(m => m.ToString()).ToList();
        }
    }
}
