using Chessman.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Chessman.View
{
    public sealed partial class TacticsPage : Page
    {
        public TacticsPage()
        {
            this.InitializeComponent();
            this.Loaded += OnPageLoaded;
            this.SizeChanged += PageSizeChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Window.Current.CoreWindow.KeyDown += OnCoreWindowKeyDown;
            chessBoard.RegisterAnimationHandlers();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Window.Current.CoreWindow.KeyDown -= OnCoreWindowKeyDown;
            chessBoard.UnRegisterAnimationHandlers();
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            // When changing the current page the PageSizeChanged event
            // is raised before layout is performed
            // UpdateColumnsRestraints needs AdaptiveTrigger for windows size to be applied
            UpdateColumnsRestraints(RenderSize);
        }

        private void PageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateColumnsRestraints(e.NewSize);
        }

        private void UpdateColumnsRestraints(Size newSize)
        {
            // for now there is no other way to enforce a min width on the right column
            // for pc layout that can be honored by the parent pannel
            if (tacticDetailsView.MinWidth > 0)
                chessBoard.MaxWidth = newSize.Width - tacticDetailsView.MinWidth - 50 - adBanner.ActualWidth;
            else
                chessBoard.MaxWidth = Double.PositiveInfinity;

            if (notationView.MinHeight > 0)
                chessBoard.MaxHeight = newSize.Height - notationView.MinHeight - 130;
            else
                chessBoard.MaxHeight = Double.PositiveInfinity;
        }

        private void OnCoreWindowKeyDown(CoreWindow sender, KeyEventArgs e)
        {
            if (e.Handled)
                return;

            var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);

            if (e.VirtualKey == VirtualKey.Left)
            {
                (this.DataContext as TacticsViewModel).GoBackCommand.Execute(null);
            }
            else if (e.VirtualKey == VirtualKey.Right)
            {
                (this.DataContext as TacticsViewModel).GoForwardCommand.Execute(null);
            }
        }

        private void OnCommandBarClosed(object sender, object e)
        {
            // get rid of the command bar being keyboard focused
            this.Focus(FocusState.Programmatic);
        }
    }
}
