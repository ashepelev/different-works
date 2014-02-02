using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graphics_Task4_5
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point(Point p)
        {
            X = p.X;
            Y = p.Y;
            Z = p.Z;
        }

        public void copy(Point from)
        {
            X = from.X;
            Y = from.Y;
            Z = from.Z;
        }
    }
}
