using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
{
    public class PracticeViewModel: ViewModelBase, INavigationAware
    {
        private IPracticeBoardService exerciseBoardService = null;

        public ChessBoardViewModel BoardViewModel
        {
            get;
            private set;
        }

        public NotationViewModel NotationViewModel
        {
            get;
            private set;
        }

        public PracticeViewModel(IPracticeBoardService exerciseBoardService)
        {
            this.exerciseBoardService = exerciseBoardService;
            BoardViewModel = new AnalysisChessBoardViewModel(exerciseBoardService);
            NotationViewModel = new NotationViewModel(exerciseBoardService);
        }

        public void OnNavigatedTo(object parameter)
        {
            PositionLoadOptions positionLoadOptions = parameter as PositionLoadOptions;
            if (positionLoadOptions != null)
            {
                exerciseBoardService.LoadFromFen(positionLoadOptions.Fen);
                ReloadBoard(positionLoadOptions.Perspective);
            }

            exerciseBoardService.Start();
        }

        public void OnNavigatingFrom()
        {
            exerciseBoardService.Stop();
        }

        public void ReloadBoard(SideColor changedPerspectiveColor)
        {
            BoardViewModel.RefreshBoard(changedPerspectiveColor);
            NotationViewModel.ReloadMoves();
        }
    }
}
