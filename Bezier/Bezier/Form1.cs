using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Bezier
{
    public partial class Form1 : Form
    {
        List<Point> points;
        Bitmap btp;
        bool locking = true;
        public const int accuracy = 1000;
        public Form1()
        {
            InitializeComponent();
            init();
            points = new List<Point>();
        }

        public void init()
        {
            btp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = btp;
            pictureBox1.BackColor = Color.White;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Graphics g = Graphics.FromImage(btp);
            points.Add(new Point(e.X, e.Y));
            g.FillEllipse(Brushes.Black, e.X - 3, e.Y - 3, 6, 6);
            pictureBox1.Image = btp;
            g.Dispose();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {            
            int coef = points.Count / 3;
            List<Point> pnts= new List<Point>();
            
            if ((points.Count == 1) || (points.Count == 0))
                return;
            if (points.Count == 2)
            {
                pnts.AddRange(lineal(points[points.Count - 2], points[points.Count - 1]));
                drawPoints(pnts);
                return;
            }
            else if (points.Count == 3)
            {
                pnts.AddRange(square(points[points.Count - 3], points[points.Count - 2], points[points.Count - 1]));
                drawPoints(pnts);
                return;
            }
            else if (points.Count == 4)
            {
                pnts.AddRange(cube(points[points.Count - 4], points[points.Count - 3], points[points.Count - 2], points[points.Count - 1]));
                drawPoints(pnts);
                return;
            }
            findAndInsertPoints();
            pnts = buildCubeSplains();
            drawPoints(pnts);
            points = new List<Point>();
        }

        private void findAndInsertPoints()
        {
            int findInd = 0;
            int insertInd = 0;
            if (!locking)
            {
                while (insertInd + 3 < points.Count - 1)
                {
                    Point addedPoint = Point.findMidPoint(points[findInd + 2], points[findInd + 3]);
                    points.Insert(insertInd + 3, addedPoint);
                    findInd += 3;
                    insertInd += 3;
                }
                int d = points.Count - 1 - insertInd;
                if (d == 1)
                {
                    Point addedPoint1 = Point.findMidPoint(points[points.Count - 2], points[points.Count - 1]);
                    points.Insert(insertInd + 1, addedPoint1);
                    Point addedPoint2 = Point.findMidPoint(points[points.Count - 2], points[points.Count - 1]);
                    points.Insert(insertInd + 2, addedPoint2);
                }
                if (d == 2)
                {
                    Point addedPoint = Point.findMidPoint(points[points.Count - 2], points[points.Count - 1]);
                    points.Insert(insertInd + 1, addedPoint);
                }
            }
            else
            {
               
                Point first = points[0];
                Point last = points[points.Count - 1];

                Point addedPoint = Point.findMidPoint(points[0], points[1]);
                Point addedPoint1 = Point.findMidPoint(points[points.Count - 2], points[points.Count - 1]);
                points.Insert(1, addedPoint);
                points.Insert(points.Count - 2, addedPoint1);


                points.RemoveAt(0);
                points.RemoveAt(points.Count - 1);

                while (insertInd + 3 < points.Count - 2)
                {
                    addedPoint = Point.findMidPoint(points[findInd + 2], points[findInd + 3]);
                    points.Insert(insertInd + 3, addedPoint);
                    findInd += 3;
                    insertInd += 3;
                }
                int d = points.Count - 2 - insertInd;
                if (d == 1)
                {
                    Point addedPoint3 = Point.findMidPoint(points[points.Count - 2], points[points.Count - 1]);
                    points.Insert(insertInd + 1, addedPoint3);
                    Point addedPoint4 = Point.findMidPoint(points[points.Count - 2], points[points.Count - 1]);
                    points.Insert(insertInd + 2, addedPoint4);
                }
                if (d == 2)
                {
                    addedPoint = Point.findMidPoint(points[points.Count - 2], points[points.Count - 1]);
                    points.Insert(insertInd + 1, addedPoint);
                }
                points.Insert(points.Count - 1, last);
                points.Insert(points.Count - 1, first);
                points.Insert(points.Count - 1, points[0]);
                
            }
                
        }
        private List<Point> buildCubeSplains()
        {
            List<Point> pnts = new List<Point>();
            int curInd = 0;
            while (curInd+3 < points.Count)
            {
                List<Point> curFour = cube(points[curInd], points[curInd + 1], points[curInd + 2], points[curInd + 3]);
                pnts.AddRange(curFour);
                curInd += 3;
            }
            return pnts;
        }
        /*
        private List<Point> buildSplains(List<Point> pnts, int curInd)
        {
            int d = points.Count - curInd - 1;
            if (d == 1)
            {
                pnts.AddRange(lineal(points[points.Count - 2], points[points.Count - 1]));
            }
            else if (d == 2)
            {
                pnts.AddRange(square(points[points.Count - 3], points[points.Count - 2], points[points.Count - 1]));
            }
            else if (d == 3)
            {
                pnts = buildCubeSplains(1, ref curInd);
            }
            else if (d > 3)
            {
                pnts = buildCubeSplains(d / 3, ref curInd);
                buildSplains(pnts, curInd);
            }
            return pnts;
        }
        */
        private void drawPoints(List<Point> pnts)
        {
            for (int i = 0; i < pnts.Count; i++)
                btp.SetPixel(pnts[i].X, pnts[i].Y, Color.Blue);
            pictureBox1.Image = btp;
        }

        private List<Point> lineal(Point p0, Point p1)
        {
            List<Point> result = new List<Point>(accuracy+1);
            double t = 0;
            while (t <= 1)
            {
                Point p = p0.Multiply(1 - t).Add(p1.Multiply(t));
                result.Add(p);
                t += 1.0 / accuracy;
            }
            return result;
        }

        private List<Point> square(Point p0, Point p1, Point p2)
        {
            List<Point> result = new List<Point>(accuracy+1);
            double t = 0;
            while (t <= 1)
            {
                Point p = p2.Multiply(t * t);
                p = p.Add(p1.Multiply(2 * t * (1 - t)));
                p = p.Add(p0.Multiply((1-t)*(1-t)));
                result.Add(p);
                t += 1.0 / accuracy;
            }
            return result;
        }

        private List<Point> cube(Point p0, Point p1, Point p2, Point p3)
        {
            List<Point> result = new List<Point>(accuracy + 1);
            double t = 0;
            while (t <= 1)
            {
                Point p = p3.Multiply(t * t * t);
                p = p.Add(p2.Multiply((1 - t) * 3 * t * t));
                p = p.Add(p1.Multiply(3 * t * (1 - t) * (1 - t)));
                p = p.Add(p0.Multiply(Math.Pow(1 - t, 3)));
                result.Add(p);
                t += 1.0 / accuracy;
            }
            return result;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            init();
            points = new List<Point>();
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
        //    init();
        }

        private void вклToolStripMenuItem_Click(object sender, EventArgs e)
        {
            locking = true;
        }

        private void выклToolStripMenuItem_Click(object sender, EventArgs e)
        {
            locking = false;
        }
    }
}
