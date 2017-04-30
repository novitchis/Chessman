using ChessEngineClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ChessEngineClient.Controls
{
    public class BoardPanel: Panel
    {
        public static readonly DependencyProperty PerspectiveProperty =
            DependencyProperty.Register("Perspective", typeof(SideColor), typeof(BoardPanel), new PropertyMetadata(SideColor.White, OnPerspectiveChanged));

        public SideColor Perspective
        {
            get { return (SideColor)GetValue(PerspectiveProperty); }
            set { SetValue(PerspectiveProperty, value); }
        }

        public BoardPanel()
        {
            UseLayoutRounding = true;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            //this will throw if the panel is inside an infinite sized parent which should not happen
            double rectSize = Math.Min(availableSize.Width, availableSize.Height) / 8;
            foreach (var child in Children.OfType<FrameworkElement>())
                child.Measure(new Size(rectSize, rectSize));

            double boardSize = rectSize * 8;
            return new Size(boardSize, boardSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double rectSize = Math.Min(finalSize.Width, finalSize.Height) / 8;
            foreach (FrameworkElement child in Children.OfType<FrameworkElement>())
            {
                SquareViewModel squareViewModel = (SquareViewModel)child.DataContext;
                if (Perspective == SideColor.White)
                {
                    child.Arrange(new Rect(squareViewModel.Coordinate.X * rectSize, 
                        (7 - squareViewModel.Coordinate.Y) * rectSize, rectSize, rectSize));
                }
                else
                {
                    child.Arrange(new Rect((7 - squareViewModel.Coordinate.X) * rectSize,
                        squareViewModel.Coordinate.Y * rectSize, rectSize, rectSize));
                }
            }

            double boardSize = rectSize * 8;
            return new Size(boardSize, boardSize);
        }

        private static void OnPerspectiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BoardPanel)d).InvalidateArrange();
        }
    }
}
