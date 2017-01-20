using ChessEngine;
using Framework.MVVM;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChessEngineClient.ViewModel
{
    public class BoardPageViewModel : ViewModelBase, INavigationAware
    {
        private INavigationService navigationService = null;
        private IEngineBoardService boardService = null;
        protected bool useInitializationDelay = false;

        #region "Properties"

        public AnalysisChessBoardViewModel BoardViewModel { get; set; }

        public NotationViewModel NotationViewModel { get; set; }

        protected INavigationService NavigationService
        {
            get { return navigationService; }
        }

        public ICommand EditPositionCommand
        {
            get { return new RelayCommand(EditPositionExecuted); }
        }

        public ICommand GoBackCommand
        {
            get
            {
                return new RelayCommand(OnGoBackCommand);
            }
        }

        public ICommand GoForwardCommand
        {
            get
            {
                return new RelayCommand(OnGoForwardCommand);
            }
        }

        public ICommand NewGameCommand
        {
            get
            {
                return new RelayCommand(OnNewGameCommand);
            }
        }

        #endregion

        public BoardPageViewModel(INavigationService navigationService, IEngineBoardService boardService)
        {
            this.navigationService = navigationService;
            this.boardService = boardService;

            NotationViewModel = new NotationViewModel(boardService);
        }

        public virtual async void OnNavigatedTo(object parameter)
        {
            PositionLoadOptions positionLoadOptions = parameter as PositionLoadOptions;
            if (positionLoadOptions != null)
            {
                boardService.LoadFrom(positionLoadOptions.SerializedBoard, positionLoadOptions.SerializationType);
                ReloadBoard(positionLoadOptions.Perspective);
            }

            if (useInitializationDelay)
            {
                await Task.Delay(1500);
                useInitializationDelay = false;
            }

            boardService.Start();
        }

        public virtual void OnNavigatingFrom()
        {
            boardService.Stop();
        }

        public void ReloadBoard(SideColor changedPerspectiveColor)
        {
            BoardViewModel.RefreshBoard(changedPerspectiveColor);
            NotationViewModel.ReloadMoves();
        }

        private void OnGoForwardCommand(object obj)
        {
            MoveData currentMove = boardService.GetCurrentMove();
            int moveIndex = 0;
            if (currentMove != null)
                moveIndex = currentMove.Index + 1;

            if (boardService.GoToMove(moveIndex))
                Messenger.Default.Send(new MessageBase(this, boardService), NotificationMessages.MoveExecuted);
        }

        private void OnGoBackCommand(object obj)
        {
            MoveData currentMove = boardService.GetCurrentMove();
            if (currentMove != null)
            {
                if (boardService.GoToMove(currentMove.Index - 1))
                    Messenger.Default.Send(new MessageBase(this, boardService), NotificationMessages.MoveExecuted);
            }
        }

        private void OnNewGameCommand(object obj)
        {
            NewGame();
        }

        protected virtual void NewGame()
        {
            boardService.ResetBoard();
            ReloadBoard(BoardViewModel.Perspective);
        }

        private void EditPositionExecuted(object obj)
        {
            navigationService.NavigateTo(ViewModelLocator.EditPositionPageNavigationName, GetPositionLoadOptions(BoardSerializationType.FEN));
        }

        protected PositionLoadOptions GetPositionLoadOptions(BoardSerializationType serializationType)
        {
            return new PositionLoadOptions()
            {
                SerializedBoard = boardService.Serialize(serializationType),
                SerializationType = serializationType,
                Perspective = BoardViewModel.Perspective,
            };
        }
    }
}
