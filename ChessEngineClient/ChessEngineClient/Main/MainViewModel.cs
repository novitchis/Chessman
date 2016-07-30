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
    public class MainViewModel : ViewModelBase
    {
        private IChessBoardService chessBoardService;

        public ChessBoardViewModel BoardViewModel { get; set; }

        public AnalysisViewModel AnalysisViewModel { get; set; }

        public NotationViewModel NotationViewModel { get; set; }

        public ICommand TogglePerspectiveCommand
        {
            get
            {
                return new RelayCommand(TogglePerspectiveExecuted);
            }
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

        public MainViewModel(IChessBoardService chessBoardService)
        {
            this.chessBoardService = chessBoardService;
            BoardViewModel = ViewModelLocator.IOCContainer.Resolve<ChessBoardViewModel>();
            AnalysisViewModel = new AnalysisViewModel();
            NotationViewModel = ViewModelLocator.IOCContainer.Resolve<NotationViewModel>();
            Messenger.Default.Register<MessageBase>(this, NotificationMessages.GoBack, OnGoBackCommand);
            Messenger.Default.Register<MessageBase>(this, NotificationMessages.GoForward, OnGoForwardCommand);
        }

        private void TogglePerspectiveExecuted(object obj)
        {
            BoardViewModel.TogglePerspective();
        }

        private void OnGoForwardCommand(object obj)
        {
            MoveData currentMove = chessBoardService.GetCurrentMove();
            int moveIndex = 0;
            if (currentMove != null)
                moveIndex = currentMove.Index + 1;

            if (chessBoardService.GoToMove(moveIndex))
                Messenger.Default.Send(new MessageBase(), NotificationMessages.MoveExecuted);

        }

        private void OnGoBackCommand(object obj)
        {
            MoveData currentMove = chessBoardService.GetCurrentMove();
            if (currentMove != null)
            {
                if (chessBoardService.GoToMove(currentMove.Index - 1))
                    Messenger.Default.Send(new MessageBase(), NotificationMessages.MoveExecuted);

            }
        }

        private void OnNewGameCommand(object obj)
        {
            chessBoardService.ResetBoard();
            Messenger.Default.Send(new MessageBase(), NotificationMessages.MoveExecuted);
        }
    }
}
