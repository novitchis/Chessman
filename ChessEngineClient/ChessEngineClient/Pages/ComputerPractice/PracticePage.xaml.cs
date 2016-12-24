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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ChessEngineClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PracticePage : Page
    {
        public PracticePage()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
            SizeChanged += PageSizeChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
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
            if (notationView.MinWidth > 0)
                chessBoard.MaxWidth = newSize.Width - notationView.MinWidth - 50;
            else
                chessBoard.MaxWidth = Double.PositiveInfinity;

            if (notationView.MinHeight > 0)
                chessBoard.MaxHeight = newSize.Height - notationView.MinHeight - 130;
            else
                chessBoard.MaxHeight = Double.PositiveInfinity;
        }
    }
}
