﻿using ChessEngineClient.ViewModel;
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

        public ViewModelLocator()
        {
            IOCContainer.RegisterType<MainViewModel, MainViewModel>(new ContainerControlledLifetimeManager());
            IOCContainer.RegisterType<IChessBoardService, ChessBoardService>(new ContainerControlledLifetimeManager());
        }
    }
}
