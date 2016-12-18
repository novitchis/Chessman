using ChessEngine;
using ChessEngineClient.ViewModel;
using Framework.MVVM;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient
{
    public class ViewModelLocator
    {
        public const string MainPageNavigationName = "MainPage";
        public const string EditPositionPageNavigationName = "EditPositionPage";
        public const string ExercisePageNavigationName = "ExercisePage";

        public static UnityContainer IOCContainer = new UnityContainer();

        public static MainViewModel MainViewModel
        {
            get
            {
                return IOCContainer.Resolve<MainViewModel>();
            }
        }

        public static EditPositionViewModel EditPositionViewModel
        {
            get
            {
                return IOCContainer.Resolve<EditPositionViewModel>();
            }
        }

        public static ExerciseViewModel ExerciseViewModel
        {
            get
            {
                return IOCContainer.Resolve<ExerciseViewModel>();
            }
        }

        public ViewModelLocator()
        {
            IOCContainer.RegisterType<MainViewModel, MainViewModel>(new ContainerControlledLifetimeManager());
            IOCContainer.RegisterType<EditPositionViewModel, EditPositionViewModel>(new ContainerControlledLifetimeManager());

            AnalysisReceiver analysisReceiver = new AnalysisReceiver();
            IOCContainer.RegisterInstance<IAnalysisReceiver>(analysisReceiver);
            IOCContainer.RegisterInstance<IEngineNotification>(analysisReceiver);

            IOCContainer.RegisterType<IBoardService, AnalysisBoardService>(new ContainerControlledLifetimeManager());
            IOCContainer.RegisterType<IBoardEditorService, EditorService>(new ContainerControlledLifetimeManager());
            IOCContainer.RegisterType<IExerciseBoardService, ExerciseBoardService>(new ContainerControlledLifetimeManager());
            
        }
    }
}
