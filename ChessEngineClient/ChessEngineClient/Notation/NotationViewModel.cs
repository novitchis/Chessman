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
                        NotifyPropertyChanged();
                    }
                    Messenger.Default.Send(new GenericMessage<MoveData>(currentMove), NotificationMessages.CurrentMoveChanged);
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
            ReloadMoves();
        }

        public void ReloadMoves()
        {
            int groupIndex = 1;
            List<MoveDataGroup> newGroupedMoves = new List<MoveDataGroup>();
            var moves = chessBoardService.GetMoves(false);
            bool startedAsBlack = StartedAsBlack();

            using (var movesEnumerator = moves.GetEnumerator())
            {
                while (movesEnumerator.MoveNext())
                {
                    MoveDataGroup moveGroup = new MoveDataGroup(groupIndex);
                    if (startedAsBlack && groupIndex == 1)
                        moveGroup.WhiteMove = null;
                    else
                        moveGroup.WhiteMove = movesEnumerator.Current;

                    if (moveGroup.WhiteMove == null || movesEnumerator.MoveNext())
                        moveGroup.BlackMove = movesEnumerator.Current;

                    newGroupedMoves.Add(moveGroup);
                    groupIndex++;
                }
            }

            GroupedMoves = newGroupedMoves;
            MoveData currentMove = chessBoardService.GetCurrentMove();
            // in order for the selected item to work CurrentMove needs to be an object from the 'moves' list
            if (currentMove != null)
                CurrentMove = moves[currentMove.Index];
        }

        private bool StartedAsBlack()
        {
            MoveData currentMove = chessBoardService.GetCurrentMove();
            if (currentMove == null || currentMove.Index % 2 != 0)
                return !chessBoardService.IsWhiteTurn;

            return chessBoardService.IsWhiteTurn;
        }
    }
}
