using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ChessEngineClient.View
{
    public class NotationPanel : Panel
    {
        protected override Size ArrangeOverride(Size finalSize)
        {
            UIElementCollection children = Children;
            Rect rcChild = new Rect(new Point(), finalSize);
            double previousChildSize = 0.0;
            //
            // Arrange and Position Children.
            //
            for (int i = 0, count = Children.Count; i < count; ++i)
            {
                UIElement child = (UIElement)Children[i];

                if (child == null) { continue; }

                rcChild.X += previousChildSize;
                previousChildSize = child.DesiredSize.Width;
                rcChild.Width = previousChildSize;
                rcChild.Height = Math.Max(finalSize.Height, child.DesiredSize.Height);

                child.Arrange(rcChild);
            }

            return finalSize;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            double boardSize = Math.Min(constraint.Width, constraint.Height);
            Size childConstraint = new Size(Double.PositiveInfinity, Double.PositiveInfinity);

            foreach (UIElement child in Children)
            {
                if (child == null) { continue; }
                child.Measure(childConstraint);
            }

            return new Size(boardSize, boardSize);
        }
    }
}
