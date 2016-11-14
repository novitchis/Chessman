using ChessEngineClient.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
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
    public sealed partial class ChessBoardView : UserControl
    {
        public ChessBoardViewModel ViewModel { get { return DataContext as ChessBoardViewModel; } }
        private ChessPieceView draggingPiece = null;

        public ChessBoardView()
        {
            this.InitializeComponent();
        }

        private void board_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (draggingPiece != null && e.Pointer.IsInContact)
            {
                SetDraggedPiecePosition(e.GetCurrentPoint(board));
                e.Handled = true;
            }
        }

        private void SetDraggedPiecePosition(PointerPoint point)
        {
            Canvas.SetLeft(draggingPiece, point.Position.X - draggingPiece.Width / 2);

            double positionY = point.PointerDevice.PointerDeviceType == PointerDeviceType.Touch ?
                point.Position.Y - draggingPiece.Height : point.Position.Y - draggingPiece.Height / 2;
            Canvas.SetTop(draggingPiece, positionY);
        }

        private void board_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            dragCanvas.Children.Clear();
            draggingPiece = null;
            e.Handled = true;
        }

        private void square_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (draggingPiece == null && e.Pointer.IsInContact)
            {
                SquareView square = (SquareView)sender;
                SquareViewModel squareVM = (SquareViewModel)square.DataContext;
                if (squareVM.Piece != null)
                {
                    double optimalPieceSize = e.Pointer.PointerDeviceType == PointerDeviceType.Touch ?
                        Math.Max(square.ActualWidth * 1.5, 75) : square.ActualWidth;

                    draggingPiece = new ChessPieceView()
                    {
                        Width = optimalPieceSize,
                        Height = optimalPieceSize,
                        DataContext = squareVM.Piece,
                        IsHitTestVisible = false
                    };
                    SetDraggedPiecePosition(e.GetCurrentPoint(board));
                    dragCanvas.Children.Add(draggingPiece);
                    e.Handled = true;
                }
            }
        }
    }
}
