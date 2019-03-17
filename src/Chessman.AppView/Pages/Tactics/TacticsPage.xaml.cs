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
    }
}
