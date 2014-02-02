using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace rgb_hsl
{
    public partial class Form1 : Form
    {
        int prev1 = 5, prev2 = 5, prev3 = 5;
        Bitmap im = null;
        double[,] collisionMatrixH;
        double[,] collisionMatrixS;
        double[,] collisionMatrixL;
        double[,] prevH;

        
        // Вспомогательные ф-ии для HSL2RGB
        private static double helpFunc(double c)
        {
            if (c < 0)
                return c + 1;
            else if (c > 1)
                return c - 1;
            else
                return c;
        }

        private static double helpFunc2(double c, double p, double q)
        {
            if (c < (1 / 6.0))
                return p + ((q - p) * 6.0 * c);
            else if ((c >= (1 / 6.0)) && (c < (1 / 2.0)))
                return q;
            else if ((c >= (1 / 2.0)) && (c < (2 / 3.0)))
                return p + ((q - p) * ((2 / 3.0) - c) * 6.0);
            else
                return p;
        }

        // Функции перевода HSL<->RGB

        public static Color HSL2RGB(double H, double S, double L)
        {
            double q;
            if (L < 0.5)
                q = L * (1.0 + S);
            else
                q = L + S - L * S;
            double p = (2.0) * L - q;
            double Hk = H / 360.0;
            double Tr = Hk + (1 / 3.0);
            double Tg = Hk;
            double Tb = Hk - (1 / 3.0);
            Tr = helpFunc(Tr);
            Tg = helpFunc(Tg);
            Tb = helpFunc(Tb);
            double R = helpFunc2(Tr, p, q);
            double G = helpFunc2(Tg, p, q);
            double B = helpFunc2(Tb, p, q);
            if (B < 0)
                B = 0;
            return Color.FromArgb((int)(R * 255), (int)(G * 255), (int)(B * 255));
        }
        
        public static void RGB2HSL(double R, double G, double B, out double H, out double S, out double L)
        {
            R /= 255.0; G /= 255.0; B /= 255.0; // Т.к. 0 и 360 одинаковые
            double max = Math.Max(R, Math.Max(G, B));
            double min = Math.Min(R, Math.Min(G, B));
            //H
            if (max == min)
                H = double.NaN;
            else if ((max == R) && (G >= B))
                H = 60 * (G - B) / (max - min);
            else if ((max == R) && (G < B))
                H = 60 * (G - B) * (max - min) + 360;
            else if (max == G)
                H = 60 * (B - R) / (max - min) + 120;
            else //if (max == B)
                H = 60 * (R - G) / (max - min) + 240;
            //L
            L = (1 / 2.0) * (max + min);
            //S
            if ((L == 0) || (max == min))
                S = 0;
            else if ((L > 0) && (L <= (1 / 2.0)))
                S = ((max - min) / (max + min));
            else if ((L > (1 / 2.0)) && (L < 1))
                S = ((max - min) / (2 - (max + min)));
            else
                S = 1;
        }


        public Form1()
        {
            InitializeComponent();
            init();
        }

        public void init()
        {
            trackBar1.Maximum = 10;
            trackBar1.Minimum = 0;
            trackBar2.Maximum = 10;
            trackBar2.Minimum = 0;
            trackBar3.Maximum = 10;
            trackBar3.Minimum = 0;
            trackBar1.Value = 5;
            trackBar2.Value = 5;
            trackBar3.Value = 5;
            progressBar1.Value = 0;
            progressBar1.Maximum = 101;
            button1.Enabled = false;

        }

        // Функция открытия файла
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = null;
                string s = openFileDialog1.FileName;
                im = new Bitmap(s);
                pictureBox1.Image = im;
                collisionMatrixS = new double[im.Width, im.Height];
                collisionMatrixL = new double[im.Width, im.Height];
                prevH = new double[im.Width, im.Height];
                button1.Enabled = true;
            }

        }

        // Функция просчета компоненты Н
        private double calcH(double h, int change, int i, int j)
        {
            if (change != 0)
                return (h + change*36) % 360;
            else
                return h;
        }

        // Функция просчета компоненты S
        private double calcS(double s, int change, int i, int j)
        {
            if (change > 0)
            {
                if (s == 1)
                    collisionMatrixS[i, j] += change * 0.1;
                if (s == 0)
                {
                    if (collisionMatrixS[i, j] != 0)
                    {
                        collisionMatrixS[i, j] -= change * 0.1;
                        if (collisionMatrixS[i, j] < 0)
                        {
                            s = -collisionMatrixS[i, j];
                            collisionMatrixS[i, j] = 0;
                        }
                    }
                    else
                        s += change * 0.1;
                }
                else
                {
                    s += change * 0.1;
                    if (s > 1)
                    {
                        collisionMatrixS[i, j] = (s - 1);
                        s = 1;
                    }
                }
            }
            else if (change < 0)
            {
                if (s == 0)
                    collisionMatrixS[i, j] -= change * 0.1;
                if (s == 1)
                {
                    if (collisionMatrixS[i, j] != 0)
                    {
                        collisionMatrixS[i, j] += change * 0.1;
                        if (collisionMatrixS[i, j] < 0)
                        {
                            s = 1 + collisionMatrixS[i, j];
                            collisionMatrixS[i, j] = 0;
                        }
                    }
                    else
                        s += change * 0.1;
                }
                else
                {
                    s += change * 0.1;
                    if (s < 0)
                    {
                        collisionMatrixS[i, j] = -s;
                        s = 0;
                    }
                }
            }
            return s;
        }


        // Функция просчета компоненты L
        private double calcL(double l, int change, int i, int j)
        {
            if (change > 0)
            {
                if (l == 1)
                    collisionMatrixL[i, j] += change * 0.1;
                if (l == 0)
                {
                    if (collisionMatrixL[i, j] != 0)
                    {
                        collisionMatrixL[i, j] -= change * 0.1;
                        if (collisionMatrixL[i, j] < 0)
                        {
                            l = -collisionMatrixL[i, j];
                            collisionMatrixL[i, j] = 0;
                        }
                    }
                    else
                        l += change * 0.1;
                }
                else
                {
                    l += change * 0.1;
                    if (l > 1)
                    {
                        collisionMatrixL[i, j] = (l -1);
                        l = 1;
                    }
                }
            }
            else if (change < 0)
            {
                if (l == 0)
                    collisionMatrixL[i, j] -= change * 0.1;
                if (l == 1)
                {
                    if (collisionMatrixL[i, j] != 0)
                    {
                        collisionMatrixL[i, j] += change * 0.1;
                        if (collisionMatrixL[i, j] < 0)
                        {
                            l = 1 + collisionMatrixL[i, j];
                            collisionMatrixL[i, j] = 0;
                        }
                    }
                    else
                        l += change * 0.1;
                }
                else
                {
                    l += change * 0.1;
                    if (l < 0)
                    {
                        collisionMatrixL[i, j] = -l;
                        l = 0;
                    }
                }
            }
            return l;
        }

        // Применение изменений по изменению компонент
        private void button1_Click(object sender, EventArgs e)
        {      
            int change1 = trackBar1.Value - prev1;
            int change2 = trackBar2.Value - prev2;
            int change3 = trackBar3.Value - prev3;
            prev1 = trackBar1.Value;
            prev2 = trackBar2.Value;
            prev3 = trackBar3.Value;
            int k = (im.Width * im.Height) / 100;
            int count = 0;
            progressBar1.Value = 0;
            if (im != null)
            {
                for (int i = 0; i < im.Width; ++i)
                    for (int j = 0; j < im.Height; ++j)
                    {
                        double h, s, l;
                        Color color = im.GetPixel(i, j);
                        RGB2HSL((int)color.R, (int)color.G, (int)color.B, out h, out s, out l);
                        h = calcH(h, change1, i, j);
                        s = calcS(s, change2, i, j);
                        l = calcL(l, change3, i, j);
                        Color newCol = HSL2RGB(h, s, l);
                        //Color newCol = hsl2rgb(h, s, l);
                        im.SetPixel(i, j, newCol);
                        pictureBox1.Invalidate();
                        ++count;
                        if (k == count)
                        {
                            count = 0;
                            progressBar1.Value = progressBar1.Value + 1;
                        }
                    }
            }

        }
    }
}

