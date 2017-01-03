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

        #region "Properties"

        public AnalysisChessBoardViewModel BoardViewModel { get; set; }

        public NotationViewModel NotationViewModel { get; set; }

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

        public virtual void OnNavigatedTo(object parameter)
        {
            PositionLoadOptions positionLoadOptions = parameter as PositionLoadOptions;
            if (positionLoadOptions != null)
            {
                boardService.LoadFromFen(positionLoadOptions.Fen);
                ReloadBoard(positionLoadOptions.Perspective);
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
            boardService.ResetBoard();
            ReloadBoard(BoardViewModel.Perspective);
        }

        private void EditPositionExecuted(object obj)
        {
            PositionLoadOptions positionLoadOptions = new PositionLoadOptions()
            {
                Fen = boardService.GetFen(),
                Perspective = BoardViewModel.Perspective,
            };

            navigationService.NavigateTo(ViewModelLocator.EditPositionPageNavigationName, positionLoadOptions);
        }
    }
}
