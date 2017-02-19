using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Shapes;

namespace ChessEngineClient
{
    public static class ArrowDrawUtil
    {
        public static List<Point> GetArrowGeometryPoints(double length, double baseWidth)
        {
            double halfWidth = baseWidth / 2.0f;
            double headLength = baseWidth * 2.5f;
            double baseLength = length - headLength;
            double halfWingsSpan = baseWidth * 1.8f;

            return new List<Point>()
            {
                new Point(0, -halfWidth),
                new Point(baseLength, -halfWidth),
                new Point(baseLength, -halfWingsSpan),
                new Point(length, 0),
                new Point(baseLength, halfWingsSpan),
                new Point(baseLength,halfWidth),
                new Point(0,halfWidth),
            };
        }

        public static double GetLineAngle(Point start, Point end)
        {
            double m = GetLineGradient(start, end);
            double radianAngle = Math.Atan(m);
            double degreeAngle = radianAngle * (180 / Math.PI);

            // adjust direction of the line
            if (start.X > end.X)
                degreeAngle += 180;

            return degreeAngle;
        }

        private static double GetLineGradient(Point p1, Point p2)
        {
            return (p2.Y - p1.Y) / (p2.X - p1.X);
        }

        public static double GetDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
    }
}
