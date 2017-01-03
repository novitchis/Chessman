using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Data;

namespace ChessEngineClient.ViewModel
{
    public class NotationViewModel : ViewModelBase
    {
        private IBoardService analysisBoardService = null;
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
                        analysisBoardService.GoToMove(currentMove.Index);
                        NotifyPropertyChanged();
                    }
                    Messenger.Default.Send(new GenericMessage<MoveData>(currentMove), NotificationMessages.CurrentMoveChanged);
                }
            }
        }

        #endregion

        public NotationViewModel(IBoardService analysisBoardService)
        {
            this.analysisBoardService = analysisBoardService;
            Messenger.Default.Register<MessageBase>(this, NotificationMessages.MoveExecuted, OnMoveExecutedMessage);
        }

        private void OnMoveExecutedMessage(MessageBase message)
        {
            // check if message is intended for current board service
            if (message.Target == analysisBoardService)
                ReloadMoves();
        }

        public void ReloadMoves()
        {
            int groupIndex = 1;
            List<MoveDataGroup> newGroupedMoves = new List<MoveDataGroup>();
            var moves = analysisBoardService.GetMoves(false);

            bool startedAsBlack = analysisBoardService.WasBlackFirstToMove();

            using (var movesEnumerator = moves.GetEnumerator())
            {
                while (movesEnumerator.MoveNext())
                {
                    MoveDataGroup moveGroup = new MoveDataGroup(groupIndex);
                    if (startedAsBlack && groupIndex == 1)
                        moveGroup.WhiteMove = MoveData.CreateEmptyMove();
                    else
                        moveGroup.WhiteMove = movesEnumerator.Current;

                    if (moveGroup.WhiteMove.Index == -1 || movesEnumerator.MoveNext())
                        moveGroup.BlackMove = movesEnumerator.Current;

                    newGroupedMoves.Add(moveGroup);
                    groupIndex++;
                }
            }

            GroupedMoves = newGroupedMoves;

            MoveData currentMove = moves.FirstOrDefault(m => m.IsCurrent);
            if (currentMove != null)
                CurrentMove = currentMove;
            // select the first ... item by default
            else if (startedAsBlack && GroupedMoves.Count > 0)
                CurrentMove = GroupedMoves[0].WhiteMove;
        }
    }
}
