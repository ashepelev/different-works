using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bezier
{
    class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x; Y = y;
        }

        public Point Add(Point p)
        {
            return new Point(X + p.X, Y + p.Y);
        }

        public Point Multiply(double d)
        {
            return new Point((int)(X * d), (int)(Y * d));
        }

        public static bool intersect(Point a, Point b, Point c, Point d, ref Point res)
        {
            Line m = new Line(a, b);
            Line n = new Line(c, d);
            double zn = det(m.a, m.b, n.a, n.b);
            if (Math.Abs(zn) < Line.EPS)
                return false;
            res.X = (int)-det(m.c, m.b, n.c, n.b);
            res.Y = (int)-det(m.a, m.c, n.a, n.c);
            return true;
        }

        public static double det(double a, double b, double c, double d)
        {
            return a * d - b * c;
        }

        public static Point findMidPoint(Point a, Point b)
        {
            return new Point(a.X + (b.X - a.X) / 2, a.Y + (b.Y - a.Y) / 2);
        }

    }
}
