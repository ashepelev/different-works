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
    public partial class Form1 : Form
    {
        Bitmap btp;
        Pen figureColor = Pens.White;
        int figure;
        Dictionary<Point, List<Point>> Axis;
        Dictionary<Point, List<TriangleFace>> tet;
        Dictionary<Point, List<SquareFace>> cub;
        Dictionary<Point, List<TriangleFace>> oct;
        List<SquareFace> randomlist;
        bool b_RandomAxisCreated = false;
        private Form2 form2 = new Form2();

        Point optional1 = null;
        Point optional2 = null;

        Point start;
        public Form1()
        {
            InitializeComponent();
            init();
            AddOwnedForm(form2);
        }

        public void init()
        {
            btp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = btp;
            pictureBox1.BackColor = Color.Black;
            Axis = Make_Axis();
            DrawCurrentAxis();
            button2.Enabled = false;
            Activate();
            pictureBox1.Focus();
        }

        private Dictionary<Point, List<Point>> Make_Axis()
        {
            Axis = new Dictionary<Point, List<Point>>();
            Point p0 = new Point(0, 0, 0);
            Point p1 = new Point(400, 0, 0);
            Point p2 = new Point(0, 400, 0);
            Point p3 = new Point(0, 0, 400);
            List<Point> pointList0 = new List<Point>();
            pointList0.Add(p0); pointList0.Add(p1);
            pointList0.Add(p2); pointList0.Add(p3);
            Dictionary<Point, List<Point>> result = new Dictionary<Point, List<Point>>();
            result.Add(p0, pointList0);
            return result;
        }

        private void DrawCurrentAxis()
        {
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            g.TranslateTransform(pictureBox1.Width / 2, pictureBox1.Height / 2);
            Pen XAxis = new Pen(Color.Blue, 1);
            Pen YAxis = new Pen(Color.Red, 1);
            Pen ZAxis = new Pen(Color.Green, 1);
            Pen RandomAxis = new Pen(Color.Yellow, 1);
            g.DrawLine(XAxis, (int)Axis.ElementAt(0).Key.X, (int)Axis.ElementAt(0).Key.Y, (int)Axis.ElementAt(0).Value.ElementAt(1).X, (int)Axis.ElementAt(0).Value.ElementAt(1).Y);
            g.DrawLine(YAxis, (int)Axis.ElementAt(0).Key.X, (int)Axis.ElementAt(0).Key.Y, (int)Axis.ElementAt(0).Value.ElementAt(2).X, (int)Axis.ElementAt(0).Value.ElementAt(2).Y);
            g.DrawLine(ZAxis, (int)Axis.ElementAt(0).Key.X, (int)Axis.ElementAt(0).Key.Y, (int)Axis.ElementAt(0).Value.ElementAt(3).X, (int)Axis.ElementAt(0).Value.ElementAt(3).Y);
            if (optional1 != null && optional2 != null)
                g.DrawLine(RandomAxis, (int)optional1.X, (int)optional1.Y, (int)optional2.X, (int)optional2.Y);
            g.Dispose();
        }

        public static Dictionary<Point, List<TriangleFace>> tetrahedron()
        {
            Point center = new Point(0, 0, 0);
            Point p0 = new Point(0, 0, 100);
            Point p1 = new Point((2*Math.Sqrt(2)/3.0)*100, 0, (-1/3.0)*100);
            Point p2 = new Point((-Math.Sqrt(2)/3.0)*100, (Math.Sqrt(6)/3.0)*100, (-1/3.0)*100);
            Point p3 = new Point((-Math.Sqrt(2)/3.0)*100, (-Math.Sqrt(6)/3.0)*100, (-1/3.0)*100);
            TriangleFace triangleFace0 = new TriangleFace(p0, p1, p2);
            TriangleFace triangleFace1 = new TriangleFace(p0, p2, p3);
            TriangleFace triangleFace2 = new TriangleFace(p0, p3, p1);
            TriangleFace triangleFace3 = new TriangleFace(p1, p3, p2);
            TriangleFace[] triangleFaceArray0 = { triangleFace0, triangleFace1, triangleFace2 };
            TriangleFace[] triangleFaceArray1 = { triangleFace0, triangleFace2, triangleFace3 };
            TriangleFace[] triangleFaceArray2 = { triangleFace0, triangleFace1, triangleFace3 };
            TriangleFace[] triangleFaceArray3 = { triangleFace1, triangleFace2, triangleFace3 };
            List<TriangleFace> triangleFaceList0 = new List<TriangleFace>(triangleFaceArray0);
            List<TriangleFace> triangleFaceList1 = new List<TriangleFace>(triangleFaceArray1);
            List<TriangleFace> triangleFaceList2 = new List<TriangleFace>(triangleFaceArray2);
            List<TriangleFace> triangleFaceList3 = new List<TriangleFace>(triangleFaceArray3);
            Dictionary<Point, List<TriangleFace>> result = new Dictionary<Point, List<TriangleFace>>();
            result.Add(p0, triangleFaceList0);
            result.Add(p1, triangleFaceList1);
            result.Add(p2, triangleFaceList2);
            result.Add(p3, triangleFaceList3);

            return result;
        }

        public Dictionary<Point, List<SquareFace>> Cube()
        {
            Point p0 = new Point(-100, -100, -100);
            Point p1 = new Point(100, -100, -100);
            Point p2 = new Point(100, 100, -100);
            Point p3 = new Point(-100, 100, -100);
            Point p4 = new Point(-100, -100, 100);
            Point p5 = new Point(100, -100, 100);
            Point p6 = new Point(100, 100, 100);
            Point p7 = new Point(-100, 100, 100);
            SquareFace squareFace0 = new SquareFace(p0, p3, p2, p1);
            SquareFace squareFace1 = new SquareFace(p0, p1, p5, p4);
            SquareFace squareFace2 = new SquareFace(p0, p4, p7, p3);
            SquareFace squareFace3 = new SquareFace(p6, p5, p1, p2);
            SquareFace squareFace4 = new SquareFace(p6, p2, p3, p7);
            SquareFace squareFace5 = new SquareFace(p6, p7, p4, p5);
            SquareFace[] squareFaceArray0 = { squareFace0, squareFace1, squareFace2 };
            SquareFace[] squareFaceArray1 = { squareFace0, squareFace1, squareFace3 };
            SquareFace[] squareFaceArray2 = { squareFace0, squareFace3, squareFace4 };
            SquareFace[] squareFaceArray3 = { squareFace0, squareFace2, squareFace4 };
            SquareFace[] squareFaceArray4 = { squareFace1, squareFace2, squareFace5 };
            SquareFace[] squareFaceArray5 = { squareFace1, squareFace3, squareFace5 };
            SquareFace[] squareFaceArray6 = { squareFace3, squareFace4, squareFace5 };
            SquareFace[] squareFaceArray7 = { squareFace2, squareFace4, squareFace5 };
            List<SquareFace> squareFaceList0 = new List<SquareFace>(squareFaceArray0);
            List<SquareFace> squareFaceList1 = new List<SquareFace>(squareFaceArray1);
            List<SquareFace> squareFaceList2 = new List<SquareFace>(squareFaceArray2);
            List<SquareFace> squareFaceList3 = new List<SquareFace>(squareFaceArray3);
            List<SquareFace> squareFaceList4 = new List<SquareFace>(squareFaceArray4);
            List<SquareFace> squareFaceList5 = new List<SquareFace>(squareFaceArray5);
            List<SquareFace> squareFaceList6 = new List<SquareFace>(squareFaceArray6);
            List<SquareFace> squareFaceList7 = new List<SquareFace>(squareFaceArray7);
            Dictionary<Point, List<SquareFace>> result = new Dictionary<Point, List<SquareFace>>();
            result.Add(p0, squareFaceList0);
            result.Add(p1, squareFaceList1);
            result.Add(p2, squareFaceList2);
            result.Add(p3, squareFaceList3);
            result.Add(p4, squareFaceList4);
            result.Add(p5, squareFaceList5);
            result.Add(p6, squareFaceList6);
            result.Add(p7, squareFaceList7);
            return result;
        }

        public Dictionary<Point, List<TriangleFace>> octahedron()
        {
            double sqrt2 = Math.Sqrt(2);
            Point p0 = new Point(100, 0, 0);
            Point p1 = new Point(-100, 0, 0);
            Point p2 = new Point(0, 100, 0);
            Point p3 = new Point(0, -100, 0);
            Point p4 = new Point(0, 0, 100);
            Point p5 = new Point(0, 0, -100);
            TriangleFace triangleFace0 = new TriangleFace(p4, p0, p2);
            TriangleFace triangleFace1 = new TriangleFace(p4, p2, p1);
            TriangleFace triangleFace2 = new TriangleFace(p4, p1, p3);
            TriangleFace triangleFace3 = new TriangleFace(p4, p3, p0);
            TriangleFace triangleFace4 = new TriangleFace(p5, p2, p0);
            TriangleFace triangleFace5 = new TriangleFace(p5, p1, p2);
            TriangleFace triangleFace6 = new TriangleFace(p5, p3, p1);
            TriangleFace triangleFace7 = new TriangleFace(p5, p0, p3);
            TriangleFace[] triangleFaceArray0 = { triangleFace0, triangleFace3, triangleFace4, triangleFace7 };
            TriangleFace[] triangleFaceArray1 = { triangleFace1, triangleFace2, triangleFace5, triangleFace6 };
            TriangleFace[] triangleFaceArray2 = { triangleFace0, triangleFace1, triangleFace4, triangleFace5 };
            TriangleFace[] triangleFaceArray3 = { triangleFace2, triangleFace3, triangleFace6, triangleFace7 };
            TriangleFace[] triangleFaceArray4 = { triangleFace0, triangleFace1, triangleFace2, triangleFace3 };
            TriangleFace[] triangleFaceArray5 = { triangleFace4, triangleFace5, triangleFace6, triangleFace7 };
            List<TriangleFace> triangleFaceList0 = new List<TriangleFace>(triangleFaceArray0);
            List<TriangleFace> triangleFaceList1 = new List<TriangleFace>(triangleFaceArray1);
            List<TriangleFace> triangleFaceList2 = new List<TriangleFace>(triangleFaceArray2);
            List<TriangleFace> triangleFaceList3 = new List<TriangleFace>(triangleFaceArray3);
            List<TriangleFace> triangleFaceList4 = new List<TriangleFace>(triangleFaceArray4);
            List<TriangleFace> triangleFaceList5 = new List<TriangleFace>(triangleFaceArray5);
            Dictionary<Point, List<TriangleFace>> result = new Dictionary<Point, List<TriangleFace>>();
            result.Add(p0, triangleFaceList0);
            result.Add(p1, triangleFaceList1);
            result.Add(p2, triangleFaceList2);
            result.Add(p3, triangleFaceList3);
            result.Add(p4, triangleFaceList4);
            result.Add(p5, triangleFaceList5);
            return result;
        }

        private void DrawEdge(Pen pen, Graphics g, Point p0, Point p1)
        {
            g.DrawLine(pen, (int)p0.X, (int)p0.Y, (int)p1.X, (int)p1.Y);
        }

        private void DrawTriangleFace(TriangleFace tf)
        {
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            g.TranslateTransform(pictureBox1.Width / 2, pictureBox1.Height / 2);
            Pen pen = figureColor;
            g.DrawLine(pen, (int)tf.points[0].X, (int)tf.points[0].Y, (int)tf.points[1].X, (int)tf.points[1].Y);
            g.DrawLine(pen, (int)tf.points[1].X, (int)tf.points[1].Y, (int)tf.points[2].X, (int)tf.points[2].Y);
            g.DrawLine(pen, (int)tf.points[2].X, (int)tf.points[2].Y, (int)tf.points[0].X, (int)tf.points[0].Y);
            g.Dispose();
        }

        private void DrawSquareFace(SquareFace sf)
        {
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            g.TranslateTransform(pictureBox1.Width / 2, pictureBox1.Height / 2);
            Pen pen = figureColor;
            g.DrawLine(pen, (int)sf.points[0].X, (int)sf.points[0].Y, (int)sf.points[1].X, (int)sf.points[1].Y);
            g.DrawLine(pen, (int)sf.points[1].X, (int)sf.points[1].Y, (int)sf.points[2].X, (int)sf.points[2].Y);
            g.DrawLine(pen, (int)sf.points[2].X, (int)sf.points[2].Y, (int)sf.points[3].X, (int)sf.points[3].Y);
            g.DrawLine(pen, (int)sf.points[3].X, (int)sf.points[3].Y, (int)sf.points[0].X, (int)sf.points[0].Y);
            g.Dispose();
        }

        private bool readRandomAxis()
        {
            double x1, y1, z1;
            bool bx1 = Double.TryParse(textBox1.Text, out x1);
            bool by1 = Double.TryParse(textBox2.Text, out y1);
            bool bz1 = Double.TryParse(textBox3.Text, out z1);
            double x2, y2, z2;
            bool bx2 = Double.TryParse(textBox4.Text, out x2);
            bool by2 = Double.TryParse(textBox5.Text, out y2);
            bool bz2 = Double.TryParse(textBox6.Text, out z2);
            if (bx1 && by1 && bz1 && bx2 && by2 && bz2)
            {
                optional1 = new Point(x1, y1, z1);
                optional2 = new Point(x2, y2, z2);
                return true;
            }
            return false;
        }

        public Matrix transformPoint(KeyEventArgs e)
        {
            double angle, shift, scale;
            if (!Double.TryParse(textBox7.Text, out angle))
                angle = 10;
            if (!Double.TryParse(textBox8.Text, out shift))
                shift = 10;
            if (!Double.TryParse(textBox9.Text, out scale))
                scale = 2;
            b_RandomAxisCreated = readRandomAxis();
            Matrix m3 = Matrix.OneMatrix();
            switch (e.KeyCode)
            {
                case Keys.Q:
                    m3 = Matrix.XRotation(angle);
                    break;
                case Keys.A:
                    m3 = Matrix.XRotation(-angle);
                    break;
                case Keys.W:
                    m3 = Matrix.YRotation(angle);
                    break;
                case Keys.S:
                    m3 = Matrix.YRotation(-angle);
                    break;
                case Keys.E:
                    m3 = Matrix.ZRotation(angle);
                    break;
                case Keys.D:
                    m3 = Matrix.ZRotation(-angle);
                    break;
                case Keys.R:
                    if (b_RandomAxisCreated)
                        m3 = Matrix.RandomAxisTurn(optional1, optional2, angle);
                    break;
                case Keys.F:
                    if (b_RandomAxisCreated)
                        m3 = Matrix.RandomAxisTurn(optional1, optional2, -angle);
                    break;
                case Keys.C:
                    m3 = Matrix.Compression(scale, scale, scale);
                    break;
                case Keys.X:
                    m3 = Matrix.Compression(1.0 / scale, 1.0 / scale, 1.0 / scale);
                    break;
                case Keys.H:
                    m3 = Matrix.Shifting(-shift, 0, 0);
                    break;
                case Keys.K:
                    m3 = Matrix.Shifting(shift, 0, 0);
                    break;
                case Keys.Y:
                    m3 = Matrix.Shifting(0, shift, 0);
                    break;
                case Keys.N:
                    m3 = Matrix.Shifting(0, -shift, 0);
                    break;
                case Keys.B:
                    m3 = Matrix.Shifting(0, 0, -shift);
                    break;
                case Keys.I:
                    m3 = Matrix.Shifting(0, 0, shift);
                    break;
                case Keys.D1:
                    m3 = Matrix.ReflectionXY();
                    break;
                case Keys.D2:
                    m3 = Matrix.ReflectionYZ();
                    break;
                case Keys.D3:
                    m3 = Matrix.ReflectionZX();
                    break;
                default:
                    break;
            }
            return m3;
        }

        private void TransformAxis(Matrix m_transform)
        {
            Point p = Axis.ElementAt(0).Key;
            Matrix vec = Matrix.GetVector(p);
            Point p1 = Matrix.PointFromVector(Matrix.MultMatrix(vec, m_transform));
            p.copy(p1);
            foreach (Point k in Axis[p])
            {
                vec = Matrix.GetVector(k);
                Point k1 = Matrix.PointFromVector(Matrix.MultMatrix(vec, m_transform));
                k.copy(k1);
            }/*
            if (b_RandomAxisCreated)
            {
                Matrix vec1 = Matrix.GetVector(optional1);
                Matrix vec2 = Matrix.GetVector(optional2);
                Point k1 = Matrix.PointFromVector(Matrix.MultMatrix(vec1, m_transform));
                Point k2 = Matrix.PointFromVector(Matrix.MultMatrix(vec2, m_transform));
                optional1.copy(k1);
                optional2.copy(k2);
            }*/
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            btp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = btp;
            pictureBox1.BackColor = Color.Black;
            g.TranslateTransform(pictureBox1.Width / 2, pictureBox1.Height / 2);
            Matrix m_transform = transformPoint(e);
            if (checkBox1.Checked)
            {
                TransformAxis(m_transform);
            }
            DrawCurrentAxis();
            switch (figure)
            {
                case 1:
                    Dictionary<Point, List<TriangleFace>>.KeyCollection tet_keys = tet.Keys;
                    foreach (Point p in tet_keys)
                    {
                        Matrix vec = Matrix.GetVector(p);
                        Point p1 = Matrix.PointFromVector(Matrix.MultMatrix(vec, m_transform));
                        p.copy(p1);
                    }
                    foreach (Point p in tet_keys)
                        foreach (TriangleFace tf in tet[p])
                        {
                            if (checkBox2.Checked) {
                                if (tf.visible())
                                    DrawTriangleFace(tf);
                            }
                            else
                                DrawTriangleFace(tf);
                        }
                    break;
                case 3:
                    Dictionary<Point, List<TriangleFace>>.KeyCollection oct_keys = oct.Keys;
                    foreach (Point p in oct_keys)
                    {
                        Matrix vec = Matrix.GetVector(p);
                        Point p1 = Matrix.PointFromVector(Matrix.MultMatrix(vec, m_transform));
                        p.copy(p1);
                    }
                    foreach (Point p in oct_keys)
                        foreach (TriangleFace tf in oct[p])
                        {
                            if (checkBox2.Checked)
                            {
                                if (tf.visible())
                                    DrawTriangleFace(tf);
                            }
                            else
                                DrawTriangleFace(tf);
                        }
                    break;
                case 2:
                    Dictionary<Point, List<SquareFace>>.KeyCollection cub_keys = cub.Keys;
                    foreach (Point p in cub_keys)
                    {
                        Matrix vec = Matrix.GetVector(p);
                        Point p1 = Matrix.PointFromVector(Matrix.MultMatrix(vec, m_transform));
                        p.copy(p1);
                    }
                    foreach (Point p in cub_keys)
                        foreach (SquareFace sf in cub[p])
                        {
                            if (checkBox2.Checked)
                            {
                                if (sf.visible())
                                    DrawSquareFace(sf);
                            }
                            else
                                DrawSquareFace(sf);
                        }
                    break;
                case 4:
                    foreach (SquareFace sf in randomlist)
                    {
                        for (int i = 0; i < sf.points.Count(); i++)
                        {
                            Matrix vec = Matrix.GetVector(sf.points[i]);
                            Point p1 = Matrix.PointFromVector(Matrix.MultMatrix(vec,m_transform));
                            sf.points[i].copy(p1);
                        }
                    }
                    DrawFigure(randomlist);
                    break;
                default:
                    break;
            }
            g.Dispose();     
        }

        private void тетраэдрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tet = tetrahedron();
            figure = 1;
        }

        private void кубToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cub = Cube();
            figure = 2;
        }

        private void октаэдрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            oct = octahedron();
            figure = 3;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pictureBox1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            form2.Show();
            button2.Enabled = true;
        }

        private void DrawFigure(List<SquareFace> list)
        {
            Bitmap btp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = btp;
            pictureBox1.BackColor = Color.Black;
            DrawCurrentAxis();
            foreach (SquareFace sf in list)
                if (checkBox2.Checked)
                {
                    if (sf.visible())
                        DrawSquareFace(sf);
                }
                else
                    DrawSquareFace(sf);
            pictureBox1.Invalidate();
            Application.DoEvents();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            randomlist = new List<SquareFace>();
            randomlist = form2.list;
            figure = 4;
            DrawFigure(randomlist);
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            init();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            start = new Point(e.X, e.Y, 0);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Graphics g = Graphics.FromImage(pictureBox1.Image);
                btp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                pictureBox1.Image = btp;
                pictureBox1.BackColor = Color.Black;
                g.TranslateTransform(pictureBox1.Width / 2, pictureBox1.Height / 2);
                
                DrawCurrentAxis();
                Point now = new Point(e.X, e.Y, 0);
                switch (figure)
                {
                    case 1:
                        Dictionary<Point, List<TriangleFace>>.KeyCollection tet_keys = tet.Keys;
                        foreach (Point p in tet_keys)
                        {
                            Point p1 = new Point(p.X + (now.X - start.X), p.Y + (now.Y - start.Y), p.Z);
                            p.copy(p1);
                        }
                        foreach (Point p in tet_keys)
                            foreach (TriangleFace tf in tet[p])
                            {
                                if (checkBox2.Checked)
                                {
                                    if (tf.visible())
                                        DrawTriangleFace(tf);
                                }
                                else
                                    DrawTriangleFace(tf);
                            }
                        break;
                    case 3:
                        Dictionary<Point, List<TriangleFace>>.KeyCollection oct_keys = oct.Keys;
                        foreach (Point p in oct_keys)
                        {
                            Point p1 = new Point(p.X + (now.X - start.X), p.Y + (now.Y - start.Y), p.Z);
                            p.copy(p1);
                        }
                        foreach (Point p in oct_keys)
                            foreach (TriangleFace tf in oct[p])
                            {
                                if (checkBox2.Checked)
                                {
                                    if (tf.visible())
                                        DrawTriangleFace(tf);
                                }
                                else
                                    DrawTriangleFace(tf);
                            }
                        break;
                    case 2:
                        Dictionary<Point, List<SquareFace>>.KeyCollection cub_keys = cub.Keys;
                        foreach (Point p in cub_keys)
                        {
                            Point p1 = new Point(p.X + (now.X - start.X), p.Y + (now.Y - start.Y), p.Z);
                            p.copy(p1);
                        }
                        foreach (Point p in cub_keys)
                            foreach (SquareFace sf in cub[p])
                            {
                                if (checkBox2.Checked)
                                {
                                    if (sf.visible())
                                        DrawSquareFace(sf);
                                }
                                else
                                    DrawSquareFace(sf);
                            }
                        break;
                    case 4:
                        foreach (SquareFace sf in randomlist)
                        {
                            for (int i = 0; i < sf.points.Count(); i++)
                            {
                                Point p1 = new Point(sf.points[i].X + (now.X - start.X), sf.points[i].Y + (now.Y - start.Y), sf.points[i].Z);
                                sf.points[i].copy(p1);
                            }
                        }
                        DrawFigure(randomlist);
                        break;
                    default:
                        break;
                }
                g.Dispose();
                start = now;
                pictureBox1.Invalidate();
            }
        }
    }
}
