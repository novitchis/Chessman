using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace ChessEngineClient.View
{
    public class MoveAnimationsFactory
    {
        public const double AnimationDurationSeconds = 0.15;

        public Storyboard StoryBoard { get; private set; }

        public MoveAnimationsFactory()
        {
            StoryBoard = new Storyboard();
        }

        public void AddMoveAnimation(ContentPresenter item, Point from, Point to)
        {
            DoubleAnimation xAnimation = new DoubleAnimation() { From = from.X, To = to.X, Duration = TimeSpan.FromSeconds(AnimationDurationSeconds) };
            StoryBoard.Children.Add(xAnimation);
            Storyboard.SetTarget(xAnimation, item);
            Storyboard.SetTargetProperty(xAnimation, "(Canvas.Left)");

            DoubleAnimation yAnimation = new DoubleAnimation() { From = from.Y, To = to.Y, Duration = TimeSpan.FromSeconds(AnimationDurationSeconds) };
            StoryBoard.Children.Add(yAnimation);
            Storyboard.SetTarget(yAnimation, item);
            Storyboard.SetTargetProperty(yAnimation, "(Canvas.Top)");

            ObjectAnimationUsingKeyFrames zIndexAnimation = new ObjectAnimationUsingKeyFrames();
            zIndexAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame() { Value = 1, KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)) });
            zIndexAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame() { Value = 0, KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(AnimationDurationSeconds)) });

            StoryBoard.Children.Add(zIndexAnimation);
            Storyboard.SetTarget(zIndexAnimation, item);
            Storyboard.SetTargetProperty(zIndexAnimation, "(Canvas.ZIndex)");
        }

        public void AddRemoveAnimation(ContentPresenter item)
        {
            // set the z index to a lower value so that the capturing piece will always be above it
            ObjectAnimationUsingKeyFrames zIndexAnimation = new ObjectAnimationUsingKeyFrames();
            zIndexAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame() { Value = -1, KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)) });

            StoryBoard.Children.Add(zIndexAnimation);
            Storyboard.SetTarget(zIndexAnimation, item);
            Storyboard.SetTargetProperty(zIndexAnimation, "(Canvas.ZIndex)");
        }
    }
}
