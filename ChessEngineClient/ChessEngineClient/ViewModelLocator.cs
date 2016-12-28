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

        public static PracticeViewModel ExerciseViewModel
        {
            get
            {
                return IOCContainer.Resolve<PracticeViewModel>();
            }
        }

        public ViewModelLocator()
        {
            IOCContainer.RegisterType<MainViewModel, MainViewModel>(new ContainerControlledLifetimeManager());
            IOCContainer.RegisterType<EditPositionViewModel, EditPositionViewModel>(new ContainerControlledLifetimeManager());
            IOCContainer.RegisterType<PracticeViewModel, PracticeViewModel>(new ContainerControlledLifetimeManager());

            IOCContainer.RegisterType<IAnalysisReceiver, AnalysisReceiver>(new ContainerControlledLifetimeManager());
            IOCContainer.RegisterType<IEngineNotification, AnalysisReceiver>(new ContainerControlledLifetimeManager());

            Engine engine = IOCContainer.Resolve<Engine>();
            engine.Start();
            IOCContainer.RegisterInstance<IEngine>(engine);

            IOCContainer.RegisterType<IEngineBoardService, AnalysisBoardService>(new ContainerControlledLifetimeManager());
            IOCContainer.RegisterType<IBoardEditorService, EditorService>(new ContainerControlledLifetimeManager());
            IOCContainer.RegisterType<IPracticeBoardService, PracticeBoardService>(new ContainerControlledLifetimeManager());
        }
    }
}
