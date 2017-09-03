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

namespace ChessEngineClient.View
{
    public sealed partial class PromotionView : UserControl
    {
        public static readonly DependencyProperty PieceSizeProperty = DependencyProperty.Register("PieceSize", typeof(double), typeof(PromotionView), new PropertyMetadata(0));

        public double PieceSize
        {
            get { return (double)GetValue(PieceSizeProperty); }
            set { SetValue(PieceSizeProperty, value); }
        }

        public PromotionView()
        {
            this.InitializeComponent();
        }
    }
}
