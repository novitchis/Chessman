using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient
{
    public class AppShellViewModel : ViewModelBase
    {
        private NavMenuItem currentNavItem = null;
        private INavigationService navigationService = null;

        public List<NavMenuItem> NavigationItems { get; set; }

        /// <summary>
        /// Setting this, does not actually navigate to that page
        /// </summary>
        public NavMenuItem CurrentNavItem
        {
            get { return currentNavItem; }
            set
            {
                if (currentNavItem != value)
                {
                    currentNavItem = value;
                    NotifyPropertyChanged();
                    OnCurrentNavItemChanged();
                }
            }
        }

        public AppShellViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;

            NavigationItems = new List<NavMenuItem>
            {
                new NavMenuItem("Analysis", "Home",ViewModelLocator.MainPageNavigationName),
                new NavMenuItem("Setup a position", "Edit", ViewModelLocator.EditPositionPageNavigationName),
            };

            CurrentNavItem = NavigationItems[0];

            navigationService.CurrentPageChanged += (o, e) => 
                CurrentNavItem = NavigationItems.FirstOrDefault(i => i.PageNavigationName == navigationService.CurrentPageKey);
        }

        private void OnCurrentNavItemChanged()
        {
            // if the change occurs from navigation changed event 
            // don't need to do the actual navigation
            if (navigationService.CurrentPageKey != CurrentNavItem.PageNavigationName)
                navigationService.NavigateTo(CurrentNavItem.PageNavigationName);
        }
    }
}
