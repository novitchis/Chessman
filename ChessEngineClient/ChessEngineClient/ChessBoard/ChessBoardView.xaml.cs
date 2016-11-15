using ChessEngine;
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
        private const double MinimumDragDistance = 15;

        public ChessBoardViewModel ViewModel { get { return DataContext as ChessBoardViewModel; } }
        private ChessPieceView draggingPiece = null;
        private Point dragStartPoint = new Point();
        private SquareView dragStartSquare = null;

        public ChessBoardView()
        {
            this.InitializeComponent();
        }

        private void OnBoardPointerMoved(object sender, PointerRoutedEventArgs e)
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

        private void OnBoardPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            EndDragMove();
            e.Handled = true;
        }

        private void EndDragMove()
        {
            dragCanvas.Children.Clear();
            draggingPiece = null;
            board.ReleasePointerCaptures();
            dragStartSquare.IsPieceDragged = false;
        }

        private void OnSquarePointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (draggingPiece == null && e.Pointer.IsInContact)
            {
                SquareViewModel squareVM = (SquareViewModel)dragStartSquare.DataContext;
                if (squareVM.Piece != null)
                {
                    PointerPoint pointerPoint = e.GetCurrentPoint(board);
                    if (!HasPointerMovedEnaugh(pointerPoint.Position))
                        return;

                    StartDragMove(e.Pointer, pointerPoint, squareVM.Piece);
                    e.Handled = true;
                }
            }
        }

        private void StartDragMove(Pointer pointer, PointerPoint pointerPoint, ChessPiece piece)
        {
            double optimalPieceSize = pointer.PointerDeviceType == PointerDeviceType.Touch ?
                    Math.Max(dragStartSquare.ActualWidth * 1.5, MinPieceSize) : dragStartSquare.ActualWidth;

            draggingPiece = new ChessPieceView()
            {
                Width = optimalPieceSize,
                Height = optimalPieceSize,
                DataContext = piece,
                IsHitTestVisible = false
            };

            SetDraggedPiecePosition(pointerPoint);
            dragCanvas.Children.Add(draggingPiece);
            board.CapturePointer(pointer);

            dragStartSquare.IsPieceDragged = true;
        }

        private bool HasPointerMovedEnaugh(Point postion)
        {
            return Math.Abs(postion.X - dragStartPoint.X) > MinimumDragDistance ||
                Math.Abs(postion.Y - dragStartPoint.Y) > MinimumDragDistance;
        }

        private void OnSquarePointerPressed(object sender, PointerRoutedEventArgs e)
        {
            dragStartPoint = e.GetCurrentPoint(board).Position;
            dragStartSquare = (SquareView)sender;
        }
    }
}
