using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ChessEngineClient.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int manipulationThreshold = 50;

        public MainPage()
        {
            this.InitializeComponent();
            SystemNavigationManager navigation = SystemNavigationManager.GetForCurrentView();
            navigation.BackRequested += OnBackExecuted;
        }

        private async void OnBackExecuted(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;

            var dialog = new MessageDialog("Are you sure you want to exit?");
            dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
            dialog.Commands.Add(new UICommand { Label = "Cancel", Id = 1 });

            var result = await dialog.ShowAsync();

            if ((int)result.Id == 0)
                Application.Current.Exit();
        }

        private void GridManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (IsMultiTouchGesture(e))
            {
                if (IsNorthToSouth(e))
                    CommandBar.ClosedDisplayMode = AppBarClosedDisplayMode.Hidden;
                else if (IsSouthToNorth(e))
                    CommandBar.ClosedDisplayMode = AppBarClosedDisplayMode.Compact;
                else if (IsWestToEast(e))
                    Messenger.Default.Send(new MessageBase(), NotificationMessages.GoBack);
                else if (IsEastToWest(e))
                    Messenger.Default.Send(new MessageBase(), NotificationMessages.GoForward);
            }
        }

        private bool IsSouthToNorth(ManipulationCompletedRoutedEventArgs e)
        {
            return IsVertical(e) &&
                e.Cumulative.Translation.Y < 0 &&
                e.Cumulative.Translation.Y < -manipulationThreshold;
        }

        private bool IsNorthToSouth(ManipulationCompletedRoutedEventArgs e)
        {
            return IsVertical(e) &&
                e.Cumulative.Translation.Y > 0 &&
                e.Cumulative.Translation.Y > manipulationThreshold;
        }

        private bool IsEastToWest(ManipulationCompletedRoutedEventArgs e)
        {
            return IsHorizontal(e) &&
                e.Cumulative.Translation.X > 0 &&
                e.Cumulative.Translation.X > manipulationThreshold;
        }

        private bool IsWestToEast(ManipulationCompletedRoutedEventArgs e)
        {
            return IsHorizontal(e) &&
                e.Cumulative.Translation.X < 0 &&
                e.Cumulative.Translation.X < -manipulationThreshold;
        }

        private bool IsVertical(ManipulationCompletedRoutedEventArgs e)
        {
            return Math.Abs(e.Cumulative.Translation.X) < Math.Abs(e.Cumulative.Translation.Y);
        }

        private bool IsHorizontal(ManipulationCompletedRoutedEventArgs e)
        {
            return Math.Abs(e.Cumulative.Translation.Y) < Math.Abs(e.Cumulative.Translation.X);
        }

        private bool IsMultiTouchGesture(ManipulationCompletedRoutedEventArgs e)
        {
            return e.Cumulative.Rotation != 0 && e.Cumulative.Expansion != 0;
        }
    }
}
