using Chessman.View;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Unity;
using Windows.Storage;
using Windows.System.Profile;
using Windows.Graphics.Display;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Chessman
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppShell : Page
    {
        private MessageDialog exitConfirmationDialog = null;

        public Frame AppFrame { get { return this.frame; } }

        public AppShellViewModel VM
        {
            get { return DataContext as AppShellViewModel; }
        }

        public AppShell()
        {
            InitializeComponent();

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            if (titleBar != null)
            {
                titleBar.ButtonBackgroundColor = (Color)App.Current.Resources["Accent800"];
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.BackgroundColor = (Color)App.Current.Resources["Accent800"];
                titleBar.ForegroundColor = Colors.White;
            }

            if (Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.IsSupported())
            {
                this.feedbackButton.Visibility = Visibility.Visible;
            }

            Loaded += (o, e) =>
            {
                if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                    HardwareButtons.BackPressed += HardwareButtons_BackPressed;

                SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManager_BackRequested;
            };

            // the name is not reliable
            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            ExecuteBackAction();
        }

        private void ExecuteBackAction()
        {
            INavigationService navigationService = ViewModelLocator.IOCContainer.Resolve<INavigationService>();
            if (AppFrame.CanGoBack)
                navigationService.GoBack();
            else
                ConfirmAndExit();
        }

        private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            ExecuteBackAction();
        }

        private async void ConfirmAndExit()
        {
            // dialog is open
            if (exitConfirmationDialog != null)
                return;

            exitConfirmationDialog = new MessageDialog("Are you sure you want to exit?");
            exitConfirmationDialog.Commands.Add(new UICommand { Label = "OK", Id = 0 });
            exitConfirmationDialog.Commands.Add(new UICommand { Label = "Cancel", Id = 1 });

            var result = await exitConfirmationDialog.ShowAsync();
            exitConfirmationDialog = null;
            if (result != null && (int)result.Id == 0)
            {
                AppPersistenceManager.SaveApplicationState(ApplicationData.Current.LocalSettings);
                Application.Current.Exit();
            }
        }

        private void OnCurrentPageChanged(object sender, SelectionChangedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppFrame.CanGoBack ?
                    AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;

            if (rootSplitView.DisplayMode != SplitViewDisplayMode.Inline &&
                rootSplitView.DisplayMode != SplitViewDisplayMode.CompactInline)
            {
                rootSplitView.IsPaneOpen = false;                
            }
        }

        private async void OnFeedbackButtonClick(object sender, RoutedEventArgs e)
        {
            var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        }
    }
}
