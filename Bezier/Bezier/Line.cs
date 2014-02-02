using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bezier
{
    class Line
    {
        public double a { get; set; }
        public double b { get; set; }
        public double c { get; set; }

        public const double EPS = 0.00001;

        public Line(Point p, Point q)
        {
            a = p.Y - q.Y;
            b = q.X - p.X;
            c = -a * p.X - b * p.Y;
            norm();
        }

        private void norm()
        {
            double z = Math.Sqrt(a * a + b * b);
            if (Math.Abs(z) > EPS)
            {
                a /= z; b /= z; c /= z;
            }
        }

        double dist(Point p)
        {
            return a * p.X + b * p.Y + c;
        }
    }
}
