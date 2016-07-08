using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Data;

namespace ChessEngineClient.ViewModel
{
    public class NotationViewModel : ViewModelBase
    {
        private IChessBoardService chessBoardService = null;
        private IList<MoveDataGroup> groupedMoves = new List<MoveDataGroup>();

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

        public ICommand SeekBackCommand
        {
            get
            {
                return new RelayCommand((p) => { chessBoardService.GoToMove(2); });
            }
        }

        #endregion

        public NotationViewModel(IChessBoardService chessBoardService)
        {
            this.chessBoardService = chessBoardService;
            this.chessBoardService.MoveExecuted += OnMoveExecuted;
            this.chessBoardService.GoToExecuted += OnMoveExecuted;
        }

        private void OnMoveExecuted(object sender, ChessEventArgs e)
        {
            int moveIndex = 1;
            List<MoveDataGroup> newMoves = new List<MoveDataGroup>();

            using (var movesEnumerator = chessBoardService.GetMoves().GetEnumerator())
            {
                while (movesEnumerator.MoveNext())
                {
                    // white move
                    MoveDataGroup moveGroup = new MoveDataGroup(moveIndex);
                    moveGroup.WhiteMove = movesEnumerator.Current;

                    // black move
                    if (movesEnumerator.MoveNext())
                        moveGroup.BlackMove = movesEnumerator.Current;

                    newMoves.Add(moveGroup);

                    moveIndex++;
                }
            }

            GroupedMoves = newMoves;
        }
    }
}
