using ChessEngine;
using ChessEngineClient.Util;
using ChessEngineClient.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
namespace ChessEngineClient.View
{
    public sealed partial class ChessBoardView : UserControl
    {
        private const double MinPieceSize = 75;
        private const double MinimumDragDistance = 15;

        public ChessBoardViewModel ViewModel { get { return DataContext as ChessBoardViewModel; } }
        private bool isDragStarted = false;
        private ChessPieceView draggingPieceView = null;
        private Point dragStartPoint = new Point();
        private SquareView pointerPressSquare = null;
        private Ellipse highlightEllipse = null;

        public ChessBoardView()
        {
            this.InitializeComponent();
        }

        private void OnBoardPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isDragStarted && e.Pointer.IsInContact)
            {
                MoveDraggedPiece(e.GetCurrentPoint(board));
                e.Handled = true;
            }
        }

        private void OnSquarePointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (isDragStarted)
                HighlightSquareForDrop((SquareView)sender, e.Pointer.PointerDeviceType);
        }

        private void OnSquarePointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (isDragStarted)
                RemoveDropHighlightFromSquare((SquareView)sender, e.Pointer.PointerDeviceType);
        }

        private void RemoveDropHighlightFromSquare(SquareView square, PointerDeviceType pointerType)
        {
            square.IsDropTarget = false;

            if (pointerType == PointerDeviceType.Touch)
            {
                //TODO: also check the size of the square

            }

        }

        private void HighlightSquareForDrop(SquareView square, PointerDeviceType pointerType)
        {
            square.IsDropTarget = true;

            if (pointerType == PointerDeviceType.Touch)
            {
                //TODO: also check the size of the square
                if (highlightEllipse == null)
                {
                    highlightEllipse = new Ellipse()
                    {
                        Width = square.ActualWidth * 2,
                        Height = square.ActualHeight * 2,
                        Fill = new SolidColorBrush(Colors.Black) { Opacity = 0.3 },
                    };
                }

                Canvas.SetLeft(highlightEllipse, 0); 
                Canvas.SetTop(highlightEllipse, 0);

                dragCanvas.Children.Add(highlightEllipse);
            }
        }

        private void MoveDraggedPiece(PointerPoint point)
        {
            Canvas.SetLeft(draggingPieceView, point.Position.X - draggingPieceView.Width / 2);

            double positionY = point.PointerDevice.PointerDeviceType == PointerDeviceType.Touch ?
                point.Position.Y - draggingPieceView.Height : point.Position.Y - draggingPieceView.Height / 2;
            Canvas.SetTop(draggingPieceView, positionY);
        }

        private void OnBoardPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            EndDragMove();
        }

        private void EndDragMove()
        {
            if (!isDragStarted)
                return;

            dragCanvas.Children.Clear();
            draggingPieceView = null;
            isDragStarted = false;
            pointerPressSquare.IsPieceDragged = false;
            pointerPressSquare = null;
        }

        private void OnSquarePointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!isDragStarted && pointerPressSquare != null && e.Pointer.IsInContact)
            {
                SquareViewModel squareVM = (SquareViewModel)pointerPressSquare.DataContext;
                if (squareVM.Piece != null)
                {
                    PointerPoint pointerPoint = e.GetCurrentPoint(board);
                    if (!HasPointerMovedEnaugh(pointerPoint.Position))
                        return;

                    StartDragMove(e.Pointer, pointerPoint, squareVM.Piece);
                }
            }
        }

        private void StartDragMove(Pointer pointer, PointerPoint pointerPoint, ChessPiece piece)
        {
            double optimalPieceSize = pointer.PointerDeviceType == PointerDeviceType.Touch ?
                    Math.Max(pointerPressSquare.ActualWidth * 1.5, MinPieceSize) : pointerPressSquare.ActualWidth;

            draggingPieceView = new ChessPieceView()
            {
                Width = optimalPieceSize,
                Height = optimalPieceSize,
                DataContext = piece,
                IsHitTestVisible = false
            };

            MoveDraggedPiece(pointerPoint);
            dragCanvas.Children.Add(draggingPieceView);

            board.SelectedItem = pointerPressSquare.DataContext;

            isDragStarted = true;
            pointerPressSquare.IsPieceDragged = true;
        }

        private bool HasPointerMovedEnaugh(Point postion)
        {
            return Math.Abs(postion.X - dragStartPoint.X) > MinimumDragDistance ||
                Math.Abs(postion.Y - dragStartPoint.Y) > MinimumDragDistance;
        }

        private void OnSquarePointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (ViewModel.IsEdit)
                return;

            dragStartPoint = e.GetCurrentPoint(board).Position;
            pointerPressSquare = (SquareView)sender;
        }

        private void OnSquarePointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (isDragStarted)
            {
                SquareView dropSquare = (SquareView)sender;
                dropSquare.IsDropTarget = false;
                board.SelectedItem = dropSquare.DataContext;
            }

            EndDragMove();
        }

        private void OnBoardPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (isDragStarted)
                board.CapturePointer(e.Pointer);
        }

        private void OnBoardPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (isDragStarted)
            {
                board.ReleasePointerCaptures();

                // if enterd fast enaugh this can cause the square 
                // not to receive mouse enter events
                SquareView squareView = VisualTreeHelperEx.FindParent<SquareView>(e.OriginalSource as DependencyObject);
                if (squareView != null)
                    squareView.IsDropTarget = true;
            }
        }
    }
}
