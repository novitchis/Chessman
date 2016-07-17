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
                    if (currentMove != null)
                    {
                        chessBoardService.GoToMove(currentMove.Index);
                        Messenger.Default.Send(new GenericMessage<MoveData>(currentMove), NotificationMessages.CurrentMoveChanged);
                        NotifyPropertyChanged();
                    }
                }
            }
        }

        #endregion

        public NotationViewModel(IChessBoardService chessBoardService)
        {
            this.chessBoardService = chessBoardService;
            Messenger.Default.Register<MessageBase>(this, NotificationMessages.MoveExecuted, OnMoveExecutedMessage);
        }

        private void OnMoveExecutedMessage(MessageBase message)
        {
            int groupIndex = 1;
            List<MoveDataGroup> newGroupedMoves = new List<MoveDataGroup>();
            var moves = chessBoardService.GetMoves(false);

            using (var movesEnumerator = moves.GetEnumerator())
            {
                while (movesEnumerator.MoveNext())
                {
                    // white move
                    MoveDataGroup moveGroup = new MoveDataGroup(groupIndex);
                    moveGroup.WhiteMove = movesEnumerator.Current;

                    // black move
                    if (movesEnumerator.MoveNext())
                        moveGroup.BlackMove = movesEnumerator.Current;

                    newGroupedMoves.Add(moveGroup);
                    groupIndex++;
                }
            }

            GroupedMoves = newGroupedMoves;
            MoveData currentMove = chessBoardService.GetCurrentMove();
            // in order for the selected item to work CurrentMove needs to be an object from the moves list
            CurrentMove = moves[currentMove.Index];
        }
    }
}
