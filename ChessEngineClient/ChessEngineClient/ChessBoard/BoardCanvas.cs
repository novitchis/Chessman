using ChessEngineClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace ChessEngineClient.View
{
    public sealed class BoardCanvas : Canvas
    {
        public BoardCanvas()
        {
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double squareSize = Math.Min(finalSize.Width, finalSize.Height) / 8;

            //TODO: normalize the square size for multiple screens (4x)

            foreach (FrameworkElement child in Children)
            {
                SquareViewModel squareViewModel = (SquareViewModel)child.DataContext;

                double x = squareViewModel.Coordinate.X * squareSize;
                double y = (7 - squareViewModel.Coordinate.Y) * squareSize;

                Rect childRect = new Rect(x, y, squareSize, squareSize);

                Canvas.SetLeft(child, x);
                Canvas.SetTop(child, y);

                child.Arrange(childRect);
            }

            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double boardSize = Math.Min(availableSize.Width, availableSize.Height);

            return new Size(boardSize, boardSize);
        }
    }
}
