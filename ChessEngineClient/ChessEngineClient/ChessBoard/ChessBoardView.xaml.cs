using ChessEngineClient.ViewModel;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
namespace ChessEngineClient.View
{
    public sealed partial class ChessBoardView : UserControl
    {
        private Point startingPoint;
        private int xThreshold = 50;

        public ChessBoardViewModel ViewModel { get { return DataContext as ChessBoardViewModel; } }

        public ChessBoardView()
        {
            this.InitializeComponent();
        }

        private void Grid_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            startingPoint = e.Position;
        }

        private void Grid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.IsInertial)
            {
                Point currentPoint = e.Position;
                if (startingPoint.X - currentPoint.X > xThreshold)
                {
                    Messenger.Default.Send(new MessageBase(), NotificationMessages.GoBack);
                    e.Complete();
                }
                else if (currentPoint.X - startingPoint.X > xThreshold)
                {
                    Messenger.Default.Send(new MessageBase(), NotificationMessages.GoForward);
                    e.Complete();
                }
            }
        }
    }
}
