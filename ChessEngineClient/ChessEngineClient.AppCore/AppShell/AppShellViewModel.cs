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
        private NavMenuItem currentSecondaryNavItem = null;
        private INavigationService navigationService = null;

        public List<NavMenuItem> NavigationItems { get; set; }

        public List<NavMenuItem> SecondaryNavigationItems { get; set; }

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
                    if (value != null)
                    {
                        OnCurrentNavItemChanged(value);
                        CurrentSecondaryNavItem = null;
                    }
                }
            }
        }

        public NavMenuItem CurrentSecondaryNavItem
        {
            get { return currentSecondaryNavItem; }
            set
            {
                if (currentSecondaryNavItem != value)
                {
                    currentSecondaryNavItem = value;
                    NotifyPropertyChanged();
                    if (value != null)
                    {
                        OnCurrentNavItemChanged(value);
                        CurrentNavItem = null;
                    }
                }
            }
        }

        public AppShellViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;

            NavigationItems = new List<NavMenuItem>
            {
                new NavMenuItem("Analysis", "Find",ViewModelLocator.MainPageNavigationName),
                new NavMenuItem("Board editor", "Edit", ViewModelLocator.EditPositionPageNavigationName),
                new NavMenuItem("Computer practice", "ReportHacked", ViewModelLocator.PracticePageNavigationName),
            };

            SecondaryNavigationItems = new List<NavMenuItem>
            {
                new NavMenuItem("Settings", "Setting", ViewModelLocator.SettingsPageNavigationName),
            };

            CurrentNavItem = NavigationItems[0];

            navigationService.CurrentPageChanged += (o, e) =>
            {
                CurrentNavItem = NavigationItems.FirstOrDefault(i => i.PageNavigationName == navigationService.CurrentPageKey);
                CurrentSecondaryNavItem = SecondaryNavigationItems.FirstOrDefault(i => i.PageNavigationName == navigationService.CurrentPageKey);
            };
        }

        private void OnCurrentNavItemChanged(NavMenuItem navItem)
        {
            // if the change occurs from navigation changed event 
            // don't need to do the actual navigation
            if (navigationService.CurrentPageKey != navItem.PageNavigationName)
                navigationService.NavigateTo(navItem.PageNavigationName);
        }
    }
}
