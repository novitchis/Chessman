using ChessEngineClient.ViewModel;
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
        public static UnityContainer IOCContainer = new UnityContainer();

        public MainViewModel MainViewModel
        {
            get
            {
                return IOCContainer.Resolve<MainViewModel>();
            }
        }

        public ViewModelLocator()
        {
            IOCContainer.RegisterType<IChessBoardService, ChessBoardService>(new ContainerControlledLifetimeManager());
            IOCContainer.RegisterInstance<MainViewModel>(new MainViewModel());
        }
    }
}
