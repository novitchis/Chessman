using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Animation;

namespace ChessEngineClient.View
{
    public static class ChessBoardAnimationsFactory
    {
        public static Storyboard GetMoveAnimationStoryBoard(ChessPieceView pieceView, Point from, Point to)
        {
            Storyboard storyBoard = new Storyboard();

            DoubleAnimation xAnimation = new DoubleAnimation() { From = from.X, To = to.X, Duration = TimeSpan.FromSeconds(0.2) };
            storyBoard.Children.Add(xAnimation);
            Storyboard.SetTarget(xAnimation, pieceView);
            Storyboard.SetTargetProperty(xAnimation, "(Canvas.Left)");

            DoubleAnimation yAnimation = new DoubleAnimation() { From = from.Y, To = to.Y, Duration = TimeSpan.FromSeconds(0.2) };
            storyBoard.Children.Add(yAnimation);
            Storyboard.SetTarget(yAnimation, pieceView);
            Storyboard.SetTargetProperty(yAnimation, "(Canvas.Top)");

            return storyBoard;
        }
    }
}
