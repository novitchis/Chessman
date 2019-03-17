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
        private TacticDetailsViewModel tacticDetailsViewModel = new TacticDetailsViewModel(null);
        private ITacticsBoardService boardService = null;
        private TacticState tacticState = TacticState.NotStarted;
        private IAppSettings appSettings = null;

        public TacticDetailsViewModel TacticDetailsViewModel
        {
            get { return tacticDetailsViewModel; }
            set
            {
                if (tacticDetailsViewModel != value)
                {
                    tacticDetailsViewModel = value;
                    NotifyPropertyChanged();
                } 
            }
        }

        public TacticState TacticState
        {
            get { return tacticState; }
            set
            {
                if (tacticState != value)
                {
                    tacticState = value;
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
        public ICommand AnalyseGameCommand
        {
            get { return new RelayCommand(AnalyseGameExecuted); }
        }

        public ICommand PracticePositionCommand
        {
            get { return new RelayCommand(PracticePositionExecuted); }
        }

        public TacticsViewModel(INavigationService navigationService, ITacticsBoardService boardService, IAppSettings appSettings)
            :base(navigationService, boardService)
        {
            this.appSettings = appSettings;
            this.boardService = boardService;
            boardService.StateChanged += (e, o) =>
            {
                TacticState = this.boardService.GetState();
                TacticDetailsViewModel.State = TacticState;
            };

            BoardViewModel = new TacticsChessBoardViewModel(boardService);
            Messenger.Default.Register<GenericMessage<MoveData>>(this, NotificationMessages.MoveExecuted, OnMoveExecuted);
        }

        private void InitiSettings()
        {
            BoardViewModel.PlaySounds = (bool)appSettings.Values[AppPersistenceManager.EnableMoveSoundsKey];
            BoardViewModel.ShowLegalMoves = (bool)appSettings.Values[AppPersistenceManager.ShowLegalMovesKey];
            NotationViewModel.UseFigurineNotation = (int)appSettings.Values[AppPersistenceManager.NotationTypeKey] == (int)NotationType.Figurines;
        }

        public override async void OnNavigatedTo(object parameter)
        {
            InitiSettings();
            base.OnNavigatedTo(parameter);
            if (boardService.GetState() == TacticState.NotStarted)
                await StartTacticAsync();
        }

        public void OnNavigatingFrom()
        {
            //throw new NotImplementedException();
        }

        private void OnMoveExecuted(GenericMessage<MoveData> obj)
        {
            if (boardService.CurrentIsLastMove() && boardService.IsComputerTurn() && boardService.GetState() == TacticState.InProgress)
                OnExecuteNextMoveAsync(null);
        }

        private async Task StartTacticAsync()
        {
            Tactic newTactic = await boardService.LoadTacticAsync();
            TacticDetailsViewModel = new TacticDetailsViewModel(newTactic);

            ReloadBoard(boardService.WasBlackFirstToMove() ? SideColor.White : SideColor.Black);
            OnExecuteNextMoveAsync(null);
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

        private void AnalyseGameExecuted(object obj)
        {
            NavigationService.NavigateTo(ViewModelLocator.MainPageNavigationName, GetPositionLoadOptions(BoardSerializationType.PGN, false));
        }

        private void PracticePositionExecuted(object obj)
        {
            NavigationService.NavigateTo(ViewModelLocator.PracticePageNavigationName, GetPositionLoadOptions(BoardSerializationType.PGN));
        }
    }
}
