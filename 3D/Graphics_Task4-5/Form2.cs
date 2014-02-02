using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Graphics_Task4_5
{
    public partial class Form2 : Form
    {
        Point start;
        List<Point> points;
        Pen main = Pens.Black;

        internal List<SquareFace> list;
        public Form2()
        {
            InitializeComponent();
            points = new List<Point>();
            Bitmap btp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = btp;
            pictureBox1.BackColor = Color.White;

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X,e.Y,0);
            start = p;
            points.Add(p);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Graphics gr = Graphics.FromImage(pictureBox1.Image);
          //      gr.SmoothingMode = SmoothingMode.HighQuality;
                Point p = new Point(e.X, e.Y, 0);
                gr.DrawLine(main, (int)start.X, (int)start.Y, (int)p.X, (int)p.Y);
                points.Add(new Point(e.X,e.Y - 200,0));
                gr.Dispose();
                start = p;
                pictureBox1.Invalidate();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            List<SquareFace> sqFaceList = new List<SquareFace>();
            list = new List<SquareFace>();
            for (int i = 0; i < points.Count - 11; i++)
            {
                if (i + 10 > points.Count - 1)
                    break;
                Point p = new Point(points[i].X, points[i].Y, points[i].Z);
                Point p1 = new Point(points[i+10].X, points[i+10].Y, points[i+10].Z);
                for (int j = 0; j < 36; j++)
                {
                    Point prev = new Point(p.X,p.Y,p.Z);
                    Matrix vec = Matrix.GetVector(p);
                    Matrix m_transformMatrix = Matrix.YRotation(10);
                    Point temp = Matrix.PointFromVector(Matrix.MultMatrix(vec,m_transformMatrix));
                    p = new Point(temp);

                    Point prev1 = new Point(p1.X, p1.Y, p1.Z);
                    Matrix vec1 = Matrix.GetVector(p1);
                    Point temp1 = Matrix.PointFromVector(Matrix.MultMatrix(vec1,m_transformMatrix));
                    p1 = new Point(temp1);

                    sqFaceList.Add(new SquareFace(prev1,p1,p,prev));
                }
                i += 9;
            }
            list = sqFaceList;
            Hide();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            points = new List<Point>();
            Bitmap btp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = btp;
            pictureBox1.BackColor = Color.White;
        }
    }
}
