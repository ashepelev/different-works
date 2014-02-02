using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Network
{
    public partial class Form1 : Form
    {

        List<record> records = new List<record>(10175);
        List<cluster> clusters = new List<cluster>(2);
        Graphics gr;

  //      double[] newfeature1 = {-0.37603966426900,-0.3944624787965,-0.45921123001, -0.524735920021, -0.465586494};
    //    double[] newfeature2 = {-0.559834203844677, 0.5833311361, 0.4458342413392, -0.0979286591, -0.37141924664};
   //     double[] newfeature2 = { -0.733979933236678206, -0.275730722376455350, -0.0374541014297885208, 0.272247204779628771, 0.556529133217126048 };
  //      double[] newfeature1 = { -0.0610345525238969594, -0.149498859620990244, -0.169130289194843242, 0.797544584183579098, -0.556095725470481916 };
  //      double[] newfeature2 = { 0.522309879873698799e-1, -0.636982572864024376, 0.748564633844263972, -0.0697587821591673374, -0.162203042606061748 };
    
   //        double[] newfeature1 = { -0.214360485688030872, -0.531459426504111598, -0.523305039465369770, -0.443503516789798935, -0.448393718087955528 };
   //     double[] newfeature2 = { -0.717610579921709512, -0.0916614542603873816, 0.349338957679603766, -0.400784630987344404, 0.440417081411433042 };
    //    double[] newfeature1 = { -0.652729170446047724, 0.362146402165299197, -0.145917657658434340, 0.487357191883367624, -0.428935447533283608 };
        double[] newfeature1 = { 0.114047010229283805, 0.520096897085828069, 0.351170312155996756, -0.577287347435286758, -0.509814895281459512 };
        double[] newfeature2 = { -0.00474839621298868594, 0.554525253548667108, -0.677865456680905476, -0.268139135842024700, 0.401346511606704870 };

        double maxt = 524734.9375;
        int maxsp = 20436, maxdp = 6923, maxsb = 1851499, maxdb = 7259109;

        List<finfeature> finfeatures = new List<finfeature>(5023);

        // For main components method
   //     double max1 = 2.95604051221888, min1 = -5.47422300857865, max2 = 2.44879994027994, min2 = -3.26403443962363;
   //     double max1 = 1.4759090490954323, min1 = -42.0515893297281, max2 = 9.6498896153705651, min2 = -40.157764690848012;
        double max1, min1, max2, min2;
     //   double maxt = 0;
     //   int maxsp = 0, maxdp = 0, maxsb = 0, maxdb = 0;

        public class record
        {
            public int port;
            public double time;
            public int srcpkg;
            public int dstpkg;
            public int srcbyte;
            public int dstbyte;

            public record(int p, double t, int sp, int dp, int sb, int db)
            {
                this.port = p;
                this.time = t;
                this.srcpkg = sp;
                this.dstpkg = dp;
                this.srcbyte = sb;
                this.dstbyte = db;
            }
        }

        public class recordMainComp
        {
            public double port;
            public double time;
            public double srcpkg;
            public double dstpkg;
            public double srcbyte;
            public double dstbyte;

            public recordMainComp(double p, double t, double sp, double dp, double sb, double db)
            {
                this.port = p;
                this.time = t;
                this.srcpkg = sp;
                this.dstpkg = dp;
                this.srcbyte = sb;
                this.dstbyte = db;
            }
        }

        public struct finfeature
        {
            public double feat1;
            public double feat2;
            public int port;
        }

        public class cluster
        {
            public double X;
            public double Y;

            public double R1;

            public int N;

            public double Sx1 = 0;
            public double Sx2 = 0;

            public double Sy1 = 0;
            public double Sy2 = 0;

            public bool toDelete = false;

            public cluster(double XX, double YY, double RR1)
            {
                this.X = XX;
                this.Y = YY;
                this.R1 = RR1;
                this.N = 0;
            }

            public void add(double x, double y)
            {
                ++N;
                Sx1 += x;
                Sy1 += y;
                Sx2 += x * x;
                Sy2 += y * y;
                    if (N > 30)
                    {
                        double sqx = (Sx2 - (2.0/N)*Sx1*Sx1 + (1.0/N)*Sx1*Sx1) / N;
                        double sqy = (Sy2 - (2.0 / N) * Sy1 * Sy1 + (1.0/N) * Sy1 * Sy1) / N;
                        double sqs = sqx + sqy;
                        if (sqs < 0)
                            R1 = 1;
                        R1 = Math.Sqrt(sqs);
                    }
                X = (double)Sx1 / N;
                Y = (double)Sy1 / N;
            }

            public bool belongs(double x, double y)
            {
                if (Math.Sqrt((X - x) * (X - x) + (Y - y) * (Y - y)) < R1) // (!!!!!!!!!!!!!!!)
                    return true;
                return false;
            }

            public bool clusterBelong(cluster c)
            {
                double d = Math.Sqrt((X - c.X) * (X - c.X) + (Y - c.Y) * (Y - c.Y));
                if ( d < R1)
                    return true;
                return false;
            }

            public void Draw(Graphics g)
            {
                if (N > 30)
                g.DrawEllipse(new Pen(Brushes.Red), (float)(X - R1), (float)(Y - R1), 
                    (float) (2 * R1), (float)(2 * R1));
            }

            public void Union(cluster c)
            {
                N += c.N;
                Sx1 += c.Sx1;
                Sx2 += c.Sx2;
                Sy1 += c.Sy1;
                Sy2 += c.Sy2;

                X = (double)Sx1 / N;
                Y = (double)Sy1 / N;

                double dist = Math.Sqrt((X - c.X) * (X - c.X) + (Y - c.Y) * (Y - c.Y));
                if (dist + c.R1 > R1)
                    R1 = R1 + (R1 - dist);
            }
            
        }

        Predicate<cluster> pred = new Predicate<cluster>(toDelete);

        static bool toDelete(cluster obj)
        {
            return obj.toDelete;
        }


        public void getArgs(string xline, string yline, out double x, out double y, int i)
        {
            x = 0;
            y = 0;
            double k = pictureBox1.Image.Width - 250;
            switch (xline)
            {
                case "Время соединения":
                    x = records[i].time;
                    k /= maxt;
                    x *= k;
                    break;
                case "Пакетов принято":
                    x = records[i].srcpkg;
                    k /= maxsp;
                    x *= k;
                    break;
                case "Пакетов передано":
                    x = records[i].dstpkg;
                    k /= maxdp;
                    x *= k;
                    break;
                case "Байт принято":
                    x = records[i].srcbyte;
                    k /= maxsb;
                    x *= k;
                    break;
                case "Байт передано":
                    x = records[i].dstbyte;
                    k /= maxdb;
                    x *= k;
                    break;
                case "Соединения":
                    x = i;
                    k /= 10172;
                    x *= k;
                    break;
                default:
                    x = i;
                    k /= 10172;
                    x *= k;
                    break;
            }
            k = pictureBox1.Image.Height - 120;
            switch (yline)
            {
                case "Время соединения":
                    y = records[i].time;
                    k /= maxt;
                    y *= k;
                    break;
                case "Пакетов принято":
                    y = records[i].srcpkg;
                    k /= maxsp;
                    y *= k;
                    break;
                case "Пакетов передано":
                    y = records[i].dstpkg;
                    k /= maxdp;
                    y *= k;
                    break;
                case "Байт принято":
                    y = records[i].srcbyte;
                    k /= maxsb;
                    y *= k;
                    break;
                case "Байт передано":
                    y = records[i].dstbyte;
                    k /= maxdb;
                    y *= k;
                    break;
                case "Соединения":
                    y = i;
                    k /= 10172;
                    y *= k;
                    break;
                default:
                    y = i;
                    k /= 10172;
                    y *= k;
                    break;
            }
        }

        public void GraphicDrawer()
        {
            gr.ResetTransform();
            gr.TranslateTransform(0, pictureBox1.Image.Height);
            gr.ScaleTransform(1, -1);
            gr.TranslateTransform(50, 110);
            string xline = (string)comboBox1.SelectedItem;
            string yline = (string)comboBox2.SelectedItem;
            double maxx = 0, maxy = 0;
            makeMax(xline, out maxx);
            makeMax(yline, out maxy);
            double x = 0;
            double y = 0;
            int count = checkBox3.Checked ? finfeatures.Count : records.Count;
      //      double k = (double) (pictureBox1.Image.Width - 250) / (pictureBox1.Image.Height - 120);
            for (int i = 0; i < count; i++)
            {
                if (!checkBox3.Checked)
                    getArgs(xline, yline, out x, out y, i);
                else
                {
                    double k = pictureBox1.Image.Width - 250;
                    k /= (max1-min1);
                    x = finfeatures[i].feat1 - min1;
                    x *= k;

                    k = pictureBox1.Image.Height - 120;
                    k /= (max2 - min2);
                    y = finfeatures[i].feat2 - min2;
                    y *= k;
                }
                int xx = (int)Math.Round(x);
                int yy = (int)Math.Round(y);
                Brush br;
                if (checkBox1.Checked)
                {
                    switch (checkBox3.Checked ? finfeatures[i].port : records[i].port)
                    {
                        case 53: // DNS
                            br = Brushes.Red;
                            break;
                        case 80: // HTTP
                            br = Brushes.Blue;
                            break;
                        case 445: // MS-DS
                            br = Brushes.Green;
                            break;
                        case 139: // Netbios
                            br = Brushes.Purple;
                            break;
                        case 22: // SSH
                            br = Brushes.Gray;
                            break;
                        default:
                            br = Brushes.Black;
                            break;
                    }
                }
                else
                    br = Brushes.Black;
                if (checkBox2.Checked)
                {
                    bool added = false;
                    for (int j = 0; j < clusters.Count; j++)
                    {
                        if (clusters[j].belongs(x, y))
                        {
                            if (i == 5000)
                                i = 5000;
                            clusters[j].add(x, y);
                            added = true;
                            break;
                        }
                    }
                    if (!added)
                    {
                        clusters.Add(new cluster(x, y, 35));
                        clusters[clusters.Count - 1].add(x, y);
                    }
                    //            if (i % 100 == 0)
                    for (int j = 0; j < clusters.Count; j++)
                    {
                        if (clusters[j].toDelete)
                            continue;
                        for (int l = 0; l < clusters.Count; l++)
                        {
                            if (clusters[l].toDelete)
                                continue;
                            if (clusters[j].clusterBelong(clusters[l]))
                            {
                                if (j == l)
                                    continue;
                                clusters[j].Union(clusters[l]);
                                clusters[l].toDelete = true;
                            }
                        }
                    }
                    clusters.RemoveAll(pred);
                    for (int j = 0; j < clusters.Count; j++)
                        if (clusters[j].toDelete)
                        {
                            clusters.RemoveAt(j);
                            --j;
                        }
                }
                gr.DrawEllipse(new Pen(br), xx - 2, yy - 2, 4, 4);
         //       if (i == 4998)
         //       {
         //           i = 0;
         //           count = 2765;
         //       }

            }
            if (checkBox2.Checked)
            {
                for (int j = 0; j < clusters.Count; j++)
                    clusters[j].Draw(gr);
            }
                       
        }

        public void ResetPictureBox()
        {
            if (pictureBox1.Image != null)
                pictureBox1.Image.Dispose();
            Draw();
            GraphicDrawer();
        }

        public void Draw()
        {
            pictureBox1.Image = new Bitmap(Width, Height);
            pictureBox1.BackColor = Color.White;
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            g.TranslateTransform(0, pictureBox1.Image.Height);
            g.ScaleTransform(1, -1);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            DrawCoordinate(g);
            gr = g;
        }

        public void makeMax(string line, out double max)
        {
            switch (line)
            {
                case "Время соединения":
                    max = maxt;
                    break;
                case "Пакетов принято":
                    max = maxsp;
                    break;
                case "Пакетов передано":
                    max = maxdp;
                    break;
                case "Байт принято":
                    max = maxsb;
                    break;
                case "Байт передано":
                    max = maxdb;
                    break;
                case "Соединения":
                    max = 10172;
                    break;
                default:
                    max = 10172;
                    break;
            }
        }

        public void putTicks(Graphics g, double maxx, double maxy)
        {
            g.ResetTransform();
            g.TranslateTransform(0, pictureBox1.Image.Height);
            g.ScaleTransform(1, -1);
            g.TranslateTransform(50, 110);
            double k = (pictureBox1.Image.Width - 250) / maxx;
            double d = maxx / 10.0;
            double x = 0;
            for (int i = 0; i < 10; i++)
            {
                x += d;
                g.DrawLine(new Pen(Brushes.Black, 1.5f), (int)Math.Round(k * x), -5, (int)Math.Round(k * x), 5);
            }
            x = 0;
            g.ResetTransform();
            g.TranslateTransform(0, pictureBox1.Image.Height);
            g.TranslateTransform(50, -110);
            for (int i = 0; i < 10; i++)
            {
                x += d;
                string str;
                if (!checkBox3.Checked)
                    str = Math.Round(x).ToString();
                else
                    str = (x + min1).ToString().Remove(6);
                g.DrawString(str, new Font("Times New Roman", 9.0f), Brushes.Black, (int)Math.Round(k * x) - 20, 20);

            }
            g.ResetTransform();
            g.TranslateTransform(0, pictureBox1.Image.Height);
            g.ScaleTransform(1, -1);
            g.TranslateTransform(50, 110);
            k = (pictureBox1.Image.Height - 120) / maxy;
            d = maxy / 10.0;
            x = 0;
            for (int i = 0; i < 10; i++)
            {
                x += d;
                g.DrawLine(new Pen(Brushes.Black, 1.5f), -5, (int)Math.Round(k * x), 5, (int)Math.Round(k * x));
            }
            g.ResetTransform();
            g.TranslateTransform(0, pictureBox1.Image.Height);
            g.TranslateTransform(50, -110);
            x = 0;
            g.RotateTransform(-90.0f);
            for (int i = 0; i < 10; i++)
            {
                x += d;
                string str;
                if (!checkBox3.Checked)
                    str = Math.Round(x).ToString();
                else
                    str = (x+min2).ToString().Remove(6);
                g.DrawString(str, new Font("Times New Roman", 7.0f), Brushes.Black, (int)Math.Round(k * x) - 15, -20);
            }
        }

        public void DrawCoordinate(Graphics g)
        {
            g.TranslateTransform(50, 110);
            g.DrawLine(new Pen(Brushes.Black, 2.0f), 0, 0, pictureBox1.Image.Width - 250, 0);
            g.DrawLine(new Pen(Brushes.Black, 2.0f), 0, 0, 0, pictureBox1.Image.Height - 120);
            string xline, yline;
            if (!checkBox3.Checked)
            {
                xline = (string)comboBox1.SelectedItem;
                yline = (string)comboBox2.SelectedItem;
            }
            else
            {
                xline = "Компонента 1";
                yline = "Компонента 2";
            }
            g.ResetTransform();            
            g.DrawString(xline, new Font("Times New Roman", 12.0f), Brushes.Black, pictureBox1.Image.Width - 350, pictureBox1.Image.Height - 70);
            g.TranslateTransform(0, 150);
            g.RotateTransform(-90.0f);
            g.DrawString(yline, new Font("Times New Roman", 12.0f), Brushes.Black, 0, 0);

            double maxx = 0, maxy = 0;
            if (!checkBox3.Checked)
            {
                makeMax(xline, out maxx);
                makeMax(yline, out maxy);
            }
            else
            {
                maxx = (max1 - min1);
                maxy = (max2 - min2);
            }

            putTicks(g, maxx, maxy);

        }

        public void readStat()
        {
            System.IO.StreamReader file = new System.IO.StreamReader(@"..\..\statistics.txt");
            string line;
            int i = 0;
            while ((line = file.ReadLine()) != null)
            {
                string[] args = line.Split(' ');
                records.Add(new record(int.Parse(args[0]), double.Parse(args[1]), int.Parse(args[2]),
                    int.Parse(args[3]), int.Parse(args[4]), int.Parse(args[5])));
                             if (records[i].time > maxt)
                                 maxt = records[i].time;
                             if (records[i].srcpkg > maxsp)
                                 maxsp = records[i].srcpkg;
                             if (records[i].time > maxdp)
                                 maxdp = records[i].dstpkg;
                             if (records[i].srcbyte > maxsb)
                                 maxsb = records[i].srcbyte;
                             if (records[i].dstbyte > maxdb)
                                 maxdb = records[i].dstbyte;
                ++i;
            }
            file.Close();
        }

        public void init()
        {
            string[] items = { "Время соединения", "Пакетов принято", "Пакетов передано", "Байт принято", "Байт передано" };
            comboBox2.Items.AddRange(items);
            comboBox1.Items.AddRange(items);
            comboBox1.Items.Add("Соединения");
            comboBox1.SelectedItem = "Соединения";
            comboBox2.SelectedItem = "Время соединения";
            checkBox1.Checked = true;
            checkBox2.Checked = true;
        }

        public Form1()
        {
            InitializeComponent();
            init();
            makeIndependentFeatures();
            readStat();
            Draw();

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripContainer1_RightToolStripPanel_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null) 
                pictureBox1.Image.Dispose();
            Draw();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null) 
                pictureBox1.Image.Dispose();
            Draw();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
                pictureBox1.Image.Dispose();
            Draw();
            GraphicDrawer();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
            }
            else
            {
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
            }
        }

        public void mainComponentsDrawCoord()
        {
            if (pictureBox1.Image != null)
                pictureBox1.Image.Dispose();
            pictureBox1.Image = new Bitmap(Width, Height);
            pictureBox1.BackColor = Color.White;
            Graphics g = Graphics.FromImage(pictureBox1.Image);
        }

        public void makeIndependentFeatures()
        {
            System.IO.StreamReader file = new System.IO.StreamReader(@"..\..\centr_norm_http_ssh.txt");
            System.IO.StreamWriter file1 = new System.IO.StreamWriter(@"..\..\centr_norm_ind_feat.txt");
            string line;
            max1 = -1; min1 = 2; max2 = -1; min2 = 1;
            while ((line = file.ReadLine()) != null)
            {
                string[] args = line.Split(' ');
                double sum1 = 0, sum2 = 0;
                for (int i = 0; i < 5; i++)
                {
                    sum1 += double.Parse(args[i+1]) * newfeature1[i];
                    sum2 += double.Parse(args[i+1]) * newfeature2[i];
                }
                finfeature f;
                if (sum1 > max1)
                    max1 = sum1;
                if (sum1 < min1)
                    min1 = sum1;
                if (sum2 > max2)
                    max2 = sum2;
                if (sum2 < min2)
                    min2 = sum2;
        //        f.feat1 = double.Parse(args[1]);
        //        f.feat2 = double.Parse(args[2]);
        //        f.port = int.Parse(args[0]);
                f.feat1 = sum1;
                f.feat2 = sum2;
                f.port = int.Parse(args[0]);
                file1.Write("{0} {1} {2}\n", f.port, f.feat1, f.feat2);
                finfeatures.Add(f);
            }
            file.Close();
        //    file1.Close();
            
        }
    }
}
