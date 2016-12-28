using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChessEngineClient.ViewModel
{
    public class PracticeViewModel: BoardPageViewModel, INavigationAware
    {
        private IPracticeBoardService exerciseBoardService = null;

        public PracticeViewModel(INavigationService navigationService, IPracticeBoardService exerciseBoardService)
            : base(navigationService, exerciseBoardService)
        {
            this.exerciseBoardService = exerciseBoardService;
        }
    }
}
