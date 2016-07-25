using ChessEngine;
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
        private int manipulation_threshold = 50;
        // Sample usage the Chess Engine //

        //void foo()
        //{
        //    // Sample engine usage //
        //    try
        //    {
        //        EngineNotifications notificationHandler = new EngineNotifications();
        //        ChessBoard board = new ChessBoard();
        //        board.Initialize();
        //        ChessEngine.Engine engine = new ChessEngine.Engine(notificationHandler);
        //        engine.Start();
        //        //engine.Analyze(board);
        //    }
        //    catch( Exception e)
        //    {

        //    }
        //}

        public MainPage()
        {
            this.InitializeComponent();
            SystemNavigationManager navigation = SystemNavigationManager.GetForCurrentView();
            navigation.BackRequested += OnBackExecuted;
            //foo();
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
            }

        }

        private bool IsSouthToNorth(ManipulationCompletedRoutedEventArgs e)
        {
            return IsVertical(e) &&
                e.Cumulative.Translation.Y < 0 &&
                e.Cumulative.Translation.Y < -manipulation_threshold;
        }

        private bool IsNorthToSouth(ManipulationCompletedRoutedEventArgs e)
        {
            return IsVertical(e) &&
                e.Cumulative.Translation.Y > 0 &&
                e.Cumulative.Translation.Y > manipulation_threshold;
        }

        private bool IsVertical(ManipulationCompletedRoutedEventArgs e)
        {
            return Math.Abs(e.Cumulative.Translation.X) < Math.Abs(e.Cumulative.Translation.Y);
        }

        private bool IsMultiTouchGesture(ManipulationCompletedRoutedEventArgs e)
        {
            return e.Cumulative.Rotation != 0 && e.Cumulative.Expansion != 0;
        }
    }
}
