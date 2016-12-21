using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
{
    public class ExerciseViewModel: ViewModelBase
    {
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

        //TODO: use playing service
        public ExerciseViewModel(IExerciseBoardService exerciseBoardService)
        {
            BoardViewModel = new AnalysisChessBoardViewModel(exerciseBoardService);
            NotationViewModel = new NotationViewModel(exerciseBoardService);
        }
    }
}
