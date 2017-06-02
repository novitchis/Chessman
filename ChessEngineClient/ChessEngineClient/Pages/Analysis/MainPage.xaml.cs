using ChessEngine;
using ChessEngineClient;
using ChessEngineClient.ViewModel;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
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
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += OnMainPageLoaded;
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
            var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
            if (ctrl.HasFlag(CoreVirtualKeyStates.Down) && e.VirtualKey == VirtualKey.V)
            {
                (this.DataContext as MainViewModel).LoadFromClipboardCommand.Execute(null);
            }
            else if (e.VirtualKey == VirtualKey.Left)
            {
                (this.DataContext as MainViewModel).GoBackCommand.Execute(null);
            }
            else if (e.VirtualKey == VirtualKey.Right)
            {
                (this.DataContext as MainViewModel).GoForwardCommand.Execute(null);
            }
        }

        private void OnMainPageLoaded(object sender, RoutedEventArgs e)
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
            if (analysisView.MinWidth > 0)
                chessBoard.MaxWidth = newSize.Width - analysisView.MinWidth - 50;
            else
                chessBoard.MaxWidth = Double.PositiveInfinity;

            if (notationView.MinHeight > 0)
                chessBoard.MaxHeight = newSize.Height - notationView.MinHeight - 130;
            else
                chessBoard.MaxHeight = Double.PositiveInfinity;
        }
    }
}
