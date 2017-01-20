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
        private const double MinPieceSize = 75;
        private const double MinimumDragDistance = 4;

        public static readonly DependencyProperty HasDragAndDropProperty = DependencyProperty.Register("HasDragAndDrop", typeof(bool), typeof(ChessBoardView), new PropertyMetadata(false));

        private bool isDragStarted = false;
        private ChessPieceView draggingPieceView = null;
        private Point dragStartPoint = new Point();
        private SquareView pointerPressSquare = null;

        public bool HasDragAndDrop
        {
            get { return (bool)GetValue(HasDragAndDropProperty); }
            set { SetValue(HasDragAndDropProperty, value); }
        }

        public ChessBoardViewModel ViewModel
        {
            get { return DataContext as ChessBoardViewModel; }
        }

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
                ((SquareView)sender).IsDropTarget = true;
        }

        private void OnSquarePointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (isDragStarted)
                ((SquareView)sender).IsDropTarget = false;
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
                if (squareVM.PieceViewModel != null)
                {
                    PointerPoint pointerPoint = e.GetCurrentPoint(board);
                    if (!HasPointerMovedEnaugh(pointerPoint.Position))
                        return;

                    StartDragMove(e.Pointer, pointerPoint, squareVM.PieceViewModel);
                }
            }
        }

        private void StartDragMove(Pointer pointer, PointerPoint pointerPoint, ChessPieceViewModel pieceViewModel)
        {
            double optimalPieceSize = pointer.PointerDeviceType == PointerDeviceType.Touch ?
                    Math.Max(pointerPressSquare.ActualWidth * 1.5, MinPieceSize) : pointerPressSquare.ActualWidth;

            draggingPieceView = new ChessPieceView()
            {
                Width = optimalPieceSize,
                Height = optimalPieceSize,
                DataContext = pieceViewModel,
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
            if (!HasDragAndDrop)
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
                //SquareView squareView = VisualTreeHelperEx.FindParent<SquareView>(e.OriginalSource as DependencyObject);
                //if (squareView != null)
                //    squareView.IsDropTarget = true;
            }
        }
    }
}
