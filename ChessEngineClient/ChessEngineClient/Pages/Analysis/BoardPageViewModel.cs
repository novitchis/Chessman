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
        protected IEngineBoardService boardService = null;
        protected bool firstNavigatedToOcurred = false;

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

        public virtual void OnNavigatedTo(object parameter)
        {
            firstNavigatedToOcurred = true;

            PositionLoadOptions positionLoadOptions = parameter as PositionLoadOptions;
            if (positionLoadOptions != null)
                LoadPosition(positionLoadOptions);
        }

        public virtual void LoadPosition(PositionLoadOptions positionLoadOptions)
        {
            if (String.IsNullOrEmpty(positionLoadOptions.SerializedBoard))
                boardService.ResetBoard();
            else
                boardService.LoadFrom(positionLoadOptions.SerializedBoard, positionLoadOptions.CurrentMoveIndex);

            ReloadBoard(positionLoadOptions.Perspective);
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
                Messenger.Default.Send(new GenericMessage<MoveData>(this, boardService, boardService.GetCurrentMove()), NotificationMessages.GoForwardExecuted);
        }

        private void OnGoBackCommand(object obj)
        {
            MoveData undoedMove = boardService.GetCurrentMove();
            if (undoedMove != null)
            {
                if (boardService.GoToMove(undoedMove.Index - 1))
                    Messenger.Default.Send(new GenericMessage<MoveData>(this, boardService, undoedMove), NotificationMessages.GoBackExecuted);
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

        public PositionLoadOptions GetPositionLoadOptions(BoardSerializationType serializationType, bool stopOnCurrent = true)
        {
            PositionLoadOptions result = new PositionLoadOptions()
            {
                SerializedBoard = boardService.Serialize(serializationType, stopOnCurrent),
                SerializationType = serializationType,
                Perspective = BoardViewModel.Perspective,
            };

            MoveData currentMove = boardService.GetCurrentMove();
            if (currentMove != null)
                result.CurrentMoveIndex = currentMove.Index;

            return result;
        }
    }
}
