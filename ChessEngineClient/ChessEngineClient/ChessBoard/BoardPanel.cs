using Chessman.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Chessman.Controls
{
    public class BoardPanel: Canvas
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
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            //this will throw if the panel is inside an infinite sized parent which should not happen
            double minSize = Math.Min(availableSize.Width, availableSize.Height);

            double rectSize = GetAjustedSquareSize(minSize / 8);
            foreach (var child in Children.OfType<FrameworkElement>())
                child.Measure(new Size(rectSize, rectSize));

            return new Size(rectSize * 8, rectSize * 8);
        }

        private double GetAjustedSquareSize(double size)
        {
            // the rendering is fine for mobile
            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                return size;

            // on desktop rendering is more crisp on a grid of 4 px
            return (int)size / 4 * 4;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double rectSize = Math.Min(finalSize.Width, finalSize.Height) / 8;
            if (Perspective == SideColor.White)
            {
                foreach (FrameworkElement child in Children.OfType<FrameworkElement>())
                {
                    ICoordinatedItem coordinateItem = child.DataContext as ICoordinatedItem;
                    if (coordinateItem == null)
                        continue;

                    double x = Math.Round(coordinateItem.Coordinate.X * rectSize);
                    double y = Math.Round((7 - coordinateItem.Coordinate.Y) * rectSize);

                    Canvas.SetLeft(child, x);
                    Canvas.SetTop(child, y);
                    child.Arrange(new Rect(new Point(x, y), child.DesiredSize));
                }
            }
            else
            {
                foreach (FrameworkElement child in Children.OfType<FrameworkElement>())
                {
                    ICoordinatedItem coordinateItem = child.DataContext as ICoordinatedItem;
                    if (coordinateItem == null)
                        continue;

                    double x = (7 - coordinateItem.Coordinate.X) * rectSize;
                    double y = coordinateItem.Coordinate.Y * rectSize;

                    Canvas.SetLeft(child, x);
                    Canvas.SetTop(child, y);
                    child.Arrange(new Rect(new Point(x, y), child.DesiredSize));
                }
            }

            return finalSize;
        }

        private static void OnPerspectiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BoardPanel)d).InvalidateArrange();
        }
    }
}
