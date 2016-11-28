using ChessEngineClient.View;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Practices.Unity;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.UI.ViewManagement;

namespace ChessEngineClient
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);

            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += OnUnhandledException;

            BootstrapNavigationService();
        }

        private void BootstrapNavigationService()
        {
            NavigationService navigationService = new NavigationService();

            navigationService.Configure(ViewModelLocator.MainPageNavigationName, typeof(MainPage));
            navigationService.Configure(ViewModelLocator.EditPositionPageNavigationName, typeof(EditPositionPage));

            ViewModelLocator.IOCContainer.RegisterInstance<INavigationService>(navigationService);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(400, 500));

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);

                    if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                        HardwareButtons.BackPressed += HardwareButtons_BackPressed;

                    SystemNavigationManager navigation = SystemNavigationManager.GetForCurrentView();
                    navigation.BackRequested += OnBackExecuted;
                }
                // Ensure the current window is active
                Window.Current.Activate();  
            }

            HideMobileStatusBar();
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            ExecuteBackAction();
        }

        private async void HideMobileStatusBar()
        {
            // If we have a phone contract, hide the status bar
            if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1, 0))
            {
                var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                await statusBar.HideAsync();
            }
        }

        private void OnBackExecuted(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            ExecuteBackAction();
        }

        private void ExecuteBackAction()
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CurrentSourcePageType == typeof(MainPage))
                ConfirmAndExit();
            else if (rootFrame.CurrentSourcePageType == typeof(EditPositionPage))
                ViewModelLocator.EditPositionViewModel.ReturnToMainView();
            else
                throw new NotImplementedException("The back button is not implemented for this page");
        }

        private async void ConfirmAndExit()
        {
            var dialog = new MessageDialog("Are you sure you want to exit?");
            dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
            dialog.Commands.Add(new UICommand { Label = "Cancel", Id = 1 });

            var result = await dialog.ShowAsync();

            if ((int)result.Id == 0)
                Application.Current.Exit();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private async void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            try
            {
                var dialog = new MessageDialog("Ooops! We didn't anticipate this: " + e.Exception.Message);
                dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });

                var result = await dialog.ShowAsync();
            }
            catch
            {
                var dialog = new MessageDialog("An unhandled error occurred.");
                dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });

                var result = await dialog.ShowAsync();
            }
        }
    }
}
