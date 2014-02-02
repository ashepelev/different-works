using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graphics_Task4_5
{
    public class SquareFace
    {
        public Point[] points { get; set; }

        public SquareFace(Point p0, Point p1, Point p2, Point p3)
        {
            points = new Point[4];
            points[0] = p0;
            points[1] = p1;
            points[2] = p2;
            points[3] = p3;
        }

        public bool visible()
        {
            double normal = points[0].X * (points[1].Y - points[2].Y) + 
                points[1].X * (points[2].Y - points[0].Y) + points[2].X * (points[0].Y - points[1].Y);
            return normal > 0;
        }
    }
}

