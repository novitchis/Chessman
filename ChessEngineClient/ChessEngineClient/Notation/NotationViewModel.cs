using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ChessEngineClient.ViewModel
{
    public class NotationViewModel : ViewModelBase
    {
        private IChessBoardService chessBoardService = null;
        private IList<MoveDataGroup> groupedMoves = new List<MoveDataGroup>();
        private MoveData currentMove = null;

        #region Properties

        public IList<MoveDataGroup> GroupedMoves
        {
            get { return groupedMoves; }
            set
            {
                if (value != groupedMoves)
                {
                    groupedMoves = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public MoveData CurrentMove
        {
            get { return currentMove; }
            set
            {
                if (currentMove != value)
                {
                    currentMove = value;
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
            int moveIndex = 1;
            List<MoveDataGroup> newGroupedMoves = new List<MoveDataGroup>();

            var moves = chessBoardService.GetMoves();

            using (var movesEnumerator = moves.GetEnumerator())
            {
                while (movesEnumerator.MoveNext())
                {
                    // white move
                    MoveDataGroup moveGroup = new MoveDataGroup(moveIndex);
                    moveGroup.WhiteMove = movesEnumerator.Current;

                    // black move
                    if (movesEnumerator.MoveNext())
                        moveGroup.BlackMove = movesEnumerator.Current;

                    newGroupedMoves.Add(moveGroup);

                    moveIndex++;
                }
            }

            GroupedMoves = newGroupedMoves;
            CurrentMove = moves.Last();
        }
    }
}
