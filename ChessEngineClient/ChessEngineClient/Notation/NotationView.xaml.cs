using ChessEngineClient.Util;
using ChessEngineClient.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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
    public sealed partial class NotationView : UserControl
    {
        public NotationViewModel ViewModel
        {
            get { return DataContext as NotationViewModel; }
        }

        public NotationView()
        {
            this.InitializeComponent();
        }

        private async void OnSelectedMoveChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox list = (ListBox)sender;
            if (list.SelectedItem == null)
                return;

            await Dispatcher.RunIdleAsync((o) => 
            {
                var scrollViewer = VisualTreeHelperEx.FindChild<ScrollViewer>(list);
                ListBoxItem item = list.ContainerFromItem(list.SelectedItem) as ListBoxItem;
                if (item != null)
                {
                    scrollViewer.ScrollToElement(item);
                }
            });
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var list = (ListBox)sender;
            if (list != null && ViewModel != null && ViewModel.CurrentMove != null)
            {
                list.SelectedItem = ViewModel.CurrentMove;
            }
        }
    }
}
