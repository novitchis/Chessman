using ChessEngine;
using Framework.MVVM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chessman.ViewModel
{
    public class TacticsViewModel : BoardPageViewModel, INavigationAware
    {
        private Tactic tactic = null;
        private string currentTactic = "";
        private ITacticsBoardService boardService = null;

        public string CurrentTactic
        {
            get { return currentTactic; }
            set
            {
                if (currentTactic != value)
                {
                    currentTactic = value;
                    NotifyPropertyChanged();
                } 
            }
        }

        public ICommand SkipCommand
        {
            get
            {
                return new RelayCommand(OnSkip);
            }
        }

        public ICommand ExecuteNextMoveCommand
        {
            get
            {
                return new RelayCommand(OnExecuteNextMoveAsync);
            }
        }

        public ICommand RestartCommand
        {
            get
            {
                return new RelayCommand(OnRestart);
            }
        }

        public TacticsViewModel(INavigationService navigationService, ITacticsBoardService boardService)
            :base(navigationService, boardService)
        {
            this.boardService = boardService;
            BoardViewModel = new TacticsChessBoardViewModel(boardService);
            Messenger.Default.Register<GenericMessage<MoveData>>(this, NotificationMessages.MoveExecuted, OnMoveExecuted);
        }

        private void OnMoveExecuted(GenericMessage<MoveData> obj)
        {
            CurrentTactic = boardService.GetState().ToString();
            if (boardService.IsComputerTurn() && boardService.GetState() == TacticState.InProgress)
                OnExecuteNextMoveAsync(null);
        }

        public override async void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            if (boardService.GetState() == TacticState.NotStarted)
                await StartTacticAsync();
        }

        private async Task StartTacticAsync()
        {
            await boardService.LoadTacticAsync();
            ReloadBoard(boardService.WasBlackFirstToMove() ? SideColor.White : SideColor.Black);
            OnExecuteNextMoveAsync(null);
        }

        public void OnNavigatingFrom()
        {
            //throw new NotImplementedException();
        }

        private async void OnSkip(object obj)
        {
            await boardService.SkipAsync();
            await StartTacticAsync();
        }

        private async void OnExecuteNextMoveAsync(object obj)
        {
            await boardService.ExecuteNextMoveAsync();
            BoardViewModel.ExecuteCurrentMoveOnBoard(false);
        }

        private void OnRestart(object obj)
        {
            boardService.Restart();
            ReloadBoard(boardService.WasBlackFirstToMove() ? SideColor.White : SideColor.Black);
            OnExecuteNextMoveAsync(null);
        }
    }
}
