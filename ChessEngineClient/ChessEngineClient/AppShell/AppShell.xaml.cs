﻿using ChessEngineClient.View;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ChessEngineClient
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
                titleBar.ButtonBackgroundColor = new Color() { R = 85, G = 85, B = 85 };
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.BackgroundColor = new Color() { R = 51, G = 51, B = 51};
                titleBar.ForegroundColor = Colors.White;
            }

            Loaded += (o, e) =>
            {
                if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                    HardwareButtons.BackPressed += HardwareButtons_BackPressed;

                SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManager_BackRequested;
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            };
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            ExecuteBackAction();
        }

        private void ExecuteBackAction()
        {
            //TODO: this should be revisited
            if (AppFrame.CurrentSourcePageType == typeof(MainPage))
                ConfirmAndExit();
            else if (AppFrame.CurrentSourcePageType == typeof(EditPositionPage))
                ViewModelLocator.EditPositionViewModel.ReturnToMainView(); //TODO: change this
            else
                throw new NotImplementedException("The back button is not implemented for this page");
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
            exitConfirmationDialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
            exitConfirmationDialog.Commands.Add(new UICommand { Label = "Cancel", Id = 1 });

            var result = await exitConfirmationDialog.ShowAsync();
            exitConfirmationDialog = null;
            if (result != null && (int)result.Id == 0)
                Application.Current.Exit();
        }

        private void TogglePaneButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void TogglePaneButton_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
