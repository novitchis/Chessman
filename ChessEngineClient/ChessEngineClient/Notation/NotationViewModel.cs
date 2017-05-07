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
        private ObservableCollection<MoveDataGroup> groupedMoves = new ObservableCollection<MoveDataGroup>();
        private MoveData currentMove = null;

        #region Properties

        public ObservableCollection<MoveDataGroup> GroupedMoves
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
                    if (currentMove != null && analysisBoardService.GetCurrentMove().Index != currentMove.Index)
                    {
                        analysisBoardService.GoToMove(currentMove.Index);
                        Messenger.Default.Send(new GenericMessage<MoveData>(currentMove), NotificationMessages.CurrentMoveChanged);
                    }

                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        public NotationViewModel(IBoardService analysisBoardService)
        {
            this.analysisBoardService = analysisBoardService;
            Messenger.Default.Register<GenericMessage<MoveData>>(this, NotificationMessages.MoveExecuted, OnMoveExecutedMessage);
            Messenger.Default.Register<GenericMessage<MoveData>>(this, NotificationMessages.GoBackExecuted, OnMoveExecutedMessage);
            Messenger.Default.Register<GenericMessage<MoveData>>(this, NotificationMessages.GoForwardExecuted, OnMoveExecutedMessage);
        }

        private void OnMoveExecutedMessage(GenericMessage<MoveData> message)
        {
            // check if message is intended for current board service
            if (message.Target == analysisBoardService)
                LoadExecutedMove();
        }

        private void LoadExecutedMove()
        {
            var currentMoveData = analysisBoardService.GetCurrentMove();
            if (currentMoveData == null)
            {
                CurrentMove = null;
                return;
            }

            // one null move is added at the begining if the black was first to move
            int displayMoveIndex = currentMoveData.Index;
            if (analysisBoardService.WasBlackFirstToMove())
                displayMoveIndex++;

            int groupIndex = displayMoveIndex / 2;
            bool isWhiteMove = displayMoveIndex % 2 == 0;

            if (GroupedMoves.Count == groupIndex)
            {
                if (groupIndex == 0 && analysisBoardService.WasBlackFirstToMove())
                {
                    GroupedMoves.Add(new MoveDataGroup(groupIndex + 1) { WhiteMove = MoveData.CreateEmptyMove(), BlackMove = currentMoveData });
                }
                else
                {
                    GroupedMoves.Add(new MoveDataGroup(groupIndex + 1) { WhiteMove = currentMoveData });
                }
            }
            else
            {
                bool isSameAsDisplayedMove = isWhiteMove ? 
                    GroupedMoves[groupIndex].WhiteMove.PgnMove == currentMoveData.PgnMove :
                    GroupedMoves[groupIndex].BlackMove?.PgnMove == currentMoveData.PgnMove;

                if (!isSameAsDisplayedMove)
                {
                    //remove the entire group since ethe group moves collection is not observable
                    var overridingGroup = GroupedMoves[groupIndex];

                    //white move can never be null in a group
                    if (isWhiteMove)
                    {
                        overridingGroup.WhiteMove = currentMoveData;
                        overridingGroup.BlackMove = null;
                    }
                    else
                    {
                        overridingGroup.BlackMove = currentMoveData;
                    }

                    GroupedMoves[groupIndex] = overridingGroup;

                    // remove all other moves
                    while (groupIndex + 1 < GroupedMoves.Count)
                        GroupedMoves.RemoveAt(GroupedMoves.Count - 1);
                }
            }

            CurrentMove = isWhiteMove ? GroupedMoves[groupIndex].WhiteMove : 
                GroupedMoves[groupIndex].BlackMove;
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

            GroupedMoves.Clear();
            newGroupedMoves.ForEach(GroupedMoves.Add);

            MoveData currentMove = moves.FirstOrDefault(m => m.IsCurrent);
            if (currentMove != null)
                CurrentMove = currentMove;
            // select the first ... item by default
            else if (startedAsBlack && GroupedMoves.Count > 0)
                CurrentMove = GroupedMoves[0].WhiteMove;
        }
    }
}
