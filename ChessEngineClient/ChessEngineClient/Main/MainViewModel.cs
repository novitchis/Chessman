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
        private INavigationService navigationService = null;
        private IAnalysisBoardService analysisBoardService = null;

        public AnalysisChessBoardViewModel BoardViewModel { get; set; }

        public AnalysisViewModel AnalysisViewModel { get; set; }

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

        public MainViewModel(INavigationService navigationService, IAnalysisBoardService analysisBoardService)
        {
            this.navigationService = navigationService;
            this.analysisBoardService = analysisBoardService;
            BoardViewModel = new AnalysisChessBoardViewModel(analysisBoardService);
            AnalysisViewModel = ViewModelLocator.IOCContainer.Resolve<AnalysisViewModel>();
            NotationViewModel = ViewModelLocator.IOCContainer.Resolve<NotationViewModel>();
        }

        public void OnPageNavigatedTo(PositionLoadOptions positionLoadOptions)
        {
            analysisBoardService.LoadFromFen(positionLoadOptions.Fen);
            ReloadBoard(positionLoadOptions.Perspective);
        }

        public void ReloadBoard(SideColor changedPerspectiveColor)
        {
            BoardViewModel.RefreshBoard(changedPerspectiveColor);
            NotationViewModel.ReloadMoves();
        }

        private void OnGoForwardCommand(object obj)
        {
            MoveData currentMove = analysisBoardService.GetCurrentMove();
            int moveIndex = 0;
            if (currentMove != null)
                moveIndex = currentMove.Index + 1;

            if (analysisBoardService.GoToMove(moveIndex))
                Messenger.Default.Send(new MessageBase(), NotificationMessages.MoveExecuted);
        }

        private void OnGoBackCommand(object obj)
        {
            MoveData currentMove = analysisBoardService.GetCurrentMove();
            if (currentMove != null)
            {
                if (analysisBoardService.GoToMove(currentMove.Index - 1))
                    Messenger.Default.Send(new MessageBase(), NotificationMessages.MoveExecuted);
            }
        }

        private void OnNewGameCommand(object obj)
        {
            analysisBoardService.ResetBoard();
            ReloadBoard(BoardViewModel.Perspective);
        }

        private void EditPositionExecuted(object obj)
        {
            PositionLoadOptions positionLoadOptions = new PositionLoadOptions()
            {
                Fen = analysisBoardService.GetFen(),
                Perspective = BoardViewModel.Perspective,
            };

            navigationService.NavigateTo(ViewModelLocator.EditPositionPageNavigationName, positionLoadOptions);
        }
    }
}
