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
        private List<string> moves;

        #region Properties

        public List<string> Moves
        {
            get
            {
                return moves;
            }
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
            this.chessBoardService.ChessmanMoved += ChessBoardService_ChessmanMoved;
            Moves = new List<string>();
        }

        private void ChessBoardService_ChessmanMoved(object sender, ChessEventArgs e)
        {
            Moves = GetTableMoves(e.Move.ToString());
        }

        private List<string> GetTableMoves(string newMove)
        {
            List<string> result = new List<string>(Moves);
            result = result.Concat(new List<string> { newMove }).ToList();
            return result;
        }
    }
}
