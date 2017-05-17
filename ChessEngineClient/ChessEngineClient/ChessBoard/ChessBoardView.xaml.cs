using ChessEngine;
using ChessEngineClient.Controls;
using ChessEngineClient.Util;
using ChessEngineClient.ViewModel;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
namespace ChessEngineClient.View
{
    public sealed partial class ChessBoardView : UserControl
    {
        private const double MinPieceSize = 75;
        private const double MinimumDragDistance = 4;

        public static readonly DependencyProperty SuggestedMoveProperty =DependencyProperty.Register("SuggestedMove", typeof(Move), typeof(ChessBoardView), new PropertyMetadata(null, OnSuggestedMoveChangedThunk));
        public static readonly DependencyProperty HasDragAndDropProperty = DependencyProperty.Register("HasDragAndDrop", typeof(bool), typeof(ChessBoardView), new PropertyMetadata(false));

        private bool isDragStarted = false;
        private ContentPresenter draggingPieceItem = null;
        private Point dragStartPoint = new Point();
        private SquareView pointerPressSquare = null;
        private ChessPieceView draggingPieceView = null;
        private ChessPieceViewModel dragSourcePieceViewModel = null;

        public Move SuggestedMove
        {
            get { return (Move)GetValue(SuggestedMoveProperty); }
            set { SetValue(SuggestedMoveProperty, value); }
        }

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

            Binding suggestedMoveBinding = new Binding() { Path =  new PropertyPath("SuggestedMove") };
            SetBinding(SuggestedMoveProperty, suggestedMoveBinding);

            Messenger.Default.Register<GenericMessage<MoveAnimationTask>>(this, NotificationMessages.AnimateMoveTaskCreated, OnAnimateMoveTaskReceived);
            //Messenger.Default.Register<GenericMessage<MoveData>>(this, NotificationMessages.MoveExecuted, OnMoveExecuted);
            //Messenger.Default.Register<GenericMessage<MoveData>>(this, NotificationMessages.GoForwardExecuted, OnGoForwardExecuted);
        }

        private void OnAnimateMoveTaskReceived(GenericMessage<MoveAnimationTask> message)
        {
            if (message.Sender != ViewModel)
                return;

            MoveAnimationsFactory moveAnimationsFactory = new MoveAnimationsFactory();

            foreach (var pieceMove in message.Content.MovedPieceCoordinates)
            {
                Coordinate fromCoordinate = pieceMove.Item1;
                Coordinate toCoordinate = pieceMove.Item2;

                Point animationStartPoint = GetCoordinatePositionOnBoard(fromCoordinate);
                Point animationEndPoint = GetCoordinatePositionOnBoard(toCoordinate);

                ContentPresenter pieceItemView = (ContentPresenter)piecesItemsControl.ContainerFromItem(ViewModel.GetPieceViewModel(fromCoordinate));
                moveAnimationsFactory.AddMoveAnimation(pieceItemView, animationStartPoint, animationEndPoint);
            }

            if (message.Content.RemovedPieceCoordinate != null)
            {
                ContentPresenter pieceItemView = (ContentPresenter)piecesItemsControl.ContainerFromItem(ViewModel.GetPieceViewModel(message.Content.RemovedPieceCoordinate));
                moveAnimationsFactory.AddRemoveAnimation(pieceItemView);
            }

            moveAnimationsFactory.StoryBoard.Completed += (o, e) => message.Content.CompleteTask();
            moveAnimationsFactory.StoryBoard.Begin();
        }

        private Point GetCoordinatePositionOnBoard(Coordinate coordinate)
        {
            FrameworkElement squareItem = (FrameworkElement)board.ContainerFromIndex(ViewModel.GetSquareIndex(coordinate));
            return squareItem.TransformToVisual(board).TransformPoint(new Point(0, 0));
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

            isDragStarted = false;

            dragSourcePieceViewModel.IsDragSource = false;
            dragSourcePieceViewModel = null;
            pointerPressSquare = null;
        }

        private void OnSquarePointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!isDragStarted && pointerPressSquare != null && e.Pointer.IsInContact)
            {
                SquareViewModel squareVM = (SquareViewModel)pointerPressSquare.DataContext;
                dragSourcePieceViewModel = ViewModel.GetPieceViewModel(squareVM.Coordinate);

                if (dragSourcePieceViewModel != null)
                {
                    PointerPoint pointerPoint = e.GetCurrentPoint(board);
                    if (!HasPointerMovedEnaugh(pointerPoint.Position))
                        return;

                    StartDragMove(e.Pointer, pointerPoint);
                }
            }
        }

        private void StartDragMove(Pointer pointer, PointerPoint pointerPoint)
        {
            double optimalPieceSize = pointerPressSquare.ActualWidth;
            if (pointer.PointerDeviceType == PointerDeviceType.Touch && optimalPieceSize < MinPieceSize)
                optimalPieceSize = Math.Max(pointerPressSquare.ActualWidth * 1.5, MinPieceSize);

            draggingPieceView = new ChessPieceView()
            {
                Width = optimalPieceSize,
                Height = optimalPieceSize,
                DataContext = new ChessPieceViewModel(dragSourcePieceViewModel.Piece, null),
                IsHitTestVisible = false
            };

            MoveDraggedPiece(pointerPoint);
            dragCanvas.Children.Add(draggingPieceView);

            board.SelectedItem = pointerPressSquare.DataContext;

            isDragStarted = true;
            dragSourcePieceViewModel.IsDragSource = true;
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
                ViewModel.OnPieceDropped((SquareViewModel)dropSquare.DataContext);
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
                board.ReleasePointerCaptures();
        }

        private static void OnSuggestedMoveChangedThunk(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChessBoardView)d).RefreshArrows();
        }

        private void RefreshArrows()
        {
            if (SuggestedMove != null)
                DrawSuggestedMoveArrow();
            else
                ClearAllArrows();
        }

        private void DrawSuggestedMoveArrow()
        {
            if (board.ActualWidth == 0)
                return;

            double squareWidth = board.ActualWidth / 8;

            Point start = GetSquareMiddlePoint(SuggestedMove.GetFrom(), squareWidth);
            Point end = GetSquareMiddlePoint(SuggestedMove.GetTo(), squareWidth);

            DrawArrow(start, end, squareWidth / 4);
        }

        private Point GetSquareMiddlePoint(Coordinate coordinate, double squareWidth)
        {
            double halfSquareWidth = squareWidth / 2;

            // from black perspective the board is rotated
            double actualX = ViewModel.Perspective == SideColor.White ? coordinate.X : 7 - coordinate.X;
            double actualY = ViewModel.Perspective == SideColor.White ? 7 - coordinate.Y : coordinate.Y;

            Point middlePoint = new Point()
            {
                X = actualX * squareWidth + halfSquareWidth,
                Y = actualY * squareWidth + halfSquareWidth
            };

            return middlePoint;
        }

        private void DrawArrow(Point start, Point end, double baseWidth)
        {
            ClearAllArrows();

            double distance = ArrowDrawUtil.GetDistance(start, end);
            var points = ArrowDrawUtil.GetArrowGeometryPoints(distance, baseWidth);
            double angle = ArrowDrawUtil.GetLineAngle(start, end);

            var pathFigure = new PathFigure() { StartPoint = points[0] };
            points.Skip(1).ToList().ForEach(p => pathFigure.Segments.Add(new LineSegment() { Point = p }));

            Path bestMoveArrowPath = new Path() { Fill = (Brush)App.Current.Resources["BoardArrowBrush"] };

            Canvas.SetLeft(bestMoveArrowPath, start.X);
            Canvas.SetTop(bestMoveArrowPath, start.Y);
            bestMoveArrowPath.Data = new PathGeometry() { Figures = new PathFigureCollection() { pathFigure } };
            bestMoveArrowPath.RenderTransform = new RotateTransform() { Angle = angle };

            arrowsCanvas.Children.Add(bestMoveArrowPath);
        }

        private void ClearAllArrows()
        {
            arrowsCanvas.Children.Clear();
        }

        private void OnBoardSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshArrows();
        }
    }
}
