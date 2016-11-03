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

namespace ChessEngineClient.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditPositionPage : Page
    {
        public EditPositionPage()
        {
            this.InitializeComponent();
        }

        private void PageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // for now there is no other way to enforce a min width on the right column
            // for pc layout that can be honored by the parent pannel
            if (piecesPalette.MinWidth > 0)
                chessBoard.MaxWidth = e.NewSize.Width - piecesPalette.MinWidth - 50;
            else
                chessBoard.MaxWidth = Double.PositiveInfinity;

            if (piecesPalette.MinHeight > 0)
                chessBoard.MaxHeight = e.NewSize.Height - piecesPalette.MinHeight - 130;
            else
                chessBoard.MaxHeight = Double.PositiveInfinity;
        }
    }
}
