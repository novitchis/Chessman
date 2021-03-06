﻿using ChessEngine;
using Chessman.Services;
using Chessman.ViewModel;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace Chessman
{
    public class ViewModelLocator
    {
        public const string AboutPageNavigationName = "AboutPage";
        public const string SettingsPageNavigationName = "SettingsPage";

        public const string MainPageNavigationName = "MainPage";
        public const string TacticsPageNavigationName = "TacticsPage";
        public const string PracticePageNavigationName = "PracticePage";
        public const string EditPositionPageNavigationName = "EditPositionPage";

        public static UnityContainer IOCContainer = new UnityContainer();

        public static MainViewModel MainViewModel
        {
            get
            {
                MainViewModelCreated = true;
                return IOCContainer.Resolve<MainViewModel>();
            }
        }

        public static bool MainViewModelCreated { get; private set; }

        public static TacticsViewModel TacticsViewModel
        {
            get
            {
                return IOCContainer.Resolve<TacticsViewModel>();
            }
        }

        public static EditPositionViewModel EditPositionViewModel
        {
            get
            {
                return IOCContainer.Resolve<EditPositionViewModel>();
            }
        }

        public static PracticeViewModel PracticeViewModel
        {
            get
            {
                PracticeViewModelCreated = true;
                return IOCContainer.Resolve<PracticeViewModel>();
            }
        }

        public static bool PracticeViewModelCreated { get; private set; }


        public static SettingsViewModel SettingsViewModel
        {
            get
            {
                return IOCContainer.Resolve<SettingsViewModel>();
            }
        }

        static ViewModelLocator()
        {
            IOCContainer.RegisterType<MainViewModel, MainViewModel>(new ContainerControlledLifetimeManager());
            IOCContainer.RegisterType<TacticsViewModel, TacticsViewModel>(new ContainerControlledLifetimeManager());
            IOCContainer.RegisterType<EditPositionViewModel, EditPositionViewModel>(new ContainerControlledLifetimeManager());
            IOCContainer.RegisterType<PracticeViewModel, PracticeViewModel>(new ContainerControlledLifetimeManager());
            IOCContainer.RegisterType<SettingsViewModel, SettingsViewModel>(new ContainerControlledLifetimeManager());

            IOCContainer.RegisterType<IAppSettings, LocalAppDataSettings>();

            IOCContainer.RegisterType<IAnalysisReceiver, AnalysisReceiver>(new ContainerControlledLifetimeManager());
            IOCContainer.RegisterType<IEngineNotification, AnalysisReceiver>(new ContainerControlledLifetimeManager());
            IOCContainer.RegisterType<ITacticsService, TacticsService>(new ContainerControlledLifetimeManager());

            Engine engine = IOCContainer.Resolve<Engine>();
            engine.Start();
            IOCContainer.RegisterInstance<IEngine>(engine);

            IOCContainer.RegisterType<IAnalysisBoardService, AnalysisBoardService>(new ContainerControlledLifetimeManager());
            IOCContainer.RegisterType<IBoardEditorService, EditorService>(new ContainerControlledLifetimeManager());
            IOCContainer.RegisterType<IPracticeBoardService, PracticeBoardService>(new ContainerControlledLifetimeManager());
            IOCContainer.RegisterType<ITacticsBoardService, TacticsBoardService>(new ContainerControlledLifetimeManager());

            IOCContainer.RegisterType<ITextReaderService, TextReaderService>(new ContainerControlledLifetimeManager());
            IOCContainer.RegisterType<IMoveAudioFeedbackService, MoveAudioFeedbackService>(new ContainerControlledLifetimeManager());
            
        }
    }
}
