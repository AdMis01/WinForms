using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektMetodyNumeryczne
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public struct Point
        {
            public double x;
            public double y;
            public Point(double xIn, double yIn)
            {
                x = xIn;
                y = yIn;
            }
        }

        const int border = 1000000;
        const double low = 1E-5;

        static double interpolacja(Point[] f, int xi, int n)
        {
            double wynik = 0;

            for (int i = 0; i < n; i++)
            {
                double wsp = f[i].y;
                for (int j = 0; j < n; j++)
                {
                    if (j != i)
                        wsp = wsp * (xi - f[j].x) / (f[i].x - f[j].x);
                }
                wynik += wsp;
            }
            return wynik;
        }
        private List<Point> funkcjaLagranga(List<Point> point)
        {
            List<Point> lagrange = new List<Point>();

            double minX = border;
            double maxX = -border;

            for (int i = 0; i < point.Count; i++)
            {
                if (point[i].x > maxX) maxX = point[i].x;
                if (point[i].x < minX) minX = point[i].x;
            }

            for (int X = Convert.ToInt32(minX); X <= Convert.ToInt32(maxX); X++)
            {
                double Y = 0.0;
                for (int i = 0; i < point.Count; i++)
                {
                    double basis = 1.0;
                    for (int j = 0; j < point.Count; j++)
                    {
                        if (j != i)
                        {
                            basis *= (X - point[j].x) / (point[i].x - point[j].x);
                        }
                    }
                        
                    Y += basis * point[i].y;
                }

                Point punkt = new Point(X, Y);
                lagrange.Add(punkt);
            }
            return lagrange;
        }

        private List<Point> DgvNaList(DataGridView dgv)
        {
            List<double> X = new List<double>();
            for (int k = 0; k < dgv.Rows.Count - 1; k++)
            {
                double current = Convert.ToDouble(dgv[0, k].Value.ToString());
                if (X.Contains(current)) { MessageBox.Show("Poprawny numer linii nie mogą się powtarzać " + (k + 1)); return new List<Point>(); }
                X.Add(current);
            }

            int pointsNumber = dgv.Rows.Count - 1;
            List<Point> point = new List<Point>();

            int i = 0;
            try
            {
                for (i = 0; i < pointsNumber; i++)
                {
                    double x = Convert.ToDouble(dgv[0, i].Value.ToString());
                    double y = Convert.ToDouble(dgv[1, i].Value.ToString());
                    Point temp = new Point(x, y);
                    point.Add(temp);
                }
            }
            catch { MessageBox.Show("W numerze lini " + (i + 1) + " dane wpisane błędnie"); }

            return point;
        }

        private double countResizeKoeff(List<Point> point)
        {
            double minX, minY, maxX, maxY;
            FindMaxMin(point, out minX, out minY, out maxX, out maxY);
            double w = funcPrinterBox.Size.Width, h = funcPrinterBox.Size.Height;
            double Sx = w / (maxX - minX), Sy = h / (maxY - minY);

            double S = Sx;
            if (Sy < S)
            {
                S = Sy;
            }
            if (S > 99999999)
            {
                S = 0;
            }
            S *= 0.55;
            return S;
        }

        private double countResizeKoeffWithZero(List<Point> point)
        {
            double minX, minY, maxX, maxY;
            FindMaxMin(point, out minX, out minY, out maxX, out maxY);
            double w = funcPrinterBox.Size.Width, h = funcPrinterBox.Size.Height;

            if (minY > 0)
            {
                minY = 0;
            }
            if (minX > 0)
            {
                minX = 0;
            }

            double Sx = w / (maxX - minX), Sy = h / (maxY - minY);

            double S = Sx;
            if (Sy < S)
            {
                S = Sy;
            }
            if (S > 99999999)
            {
                S = 0;
            } 

            S *= 0.88;
            return S;
        }

        private void FindMaxMin(List<Point> point, out double minX, out double minY, out double maxX, out double maxY)
        {
            minX = border;
            maxX = -border;
            minY = border;
            maxY = -border;

            for (int i = 0; i < point.Count; i++)
            {
                if (point[i].x + low < minX) minX = point[i].x;
                if (point[i].y + low < minY) minY = point[i].y;
                if (point[i].x > maxX + low) maxX = point[i].x;
                if (point[i].y > maxY + low) maxY = point[i].y;
            }
        }

        private void ReverseY(ref List<Point> point)
        {
            for (int i = 0; i < point.Count; i++)
            {
                Point tmp;
                tmp.x = point[i].x;
                tmp.y = -point[i].y;
                point[i] = tmp;
            }
        }

        private void PlusXY(List<Point> point, double X, double Y)
        {
            for (int i = 0; i < point.Count; i++)
            {
                Point tmp;
                tmp.x = point[i].x + X;
                tmp.y = point[i].y + Y;
                point[i] = tmp;
            }
        }

        private void MoveToBegin(List<Point> point, List<Point> point2) //Przesuń współrzędne punktów tak, aby pasowały do ​​ramki graficznej
        {
            double minX, minY, maxX, maxY;
            FindMaxMin(point2, out minX, out minY, out maxX, out maxY);
            for (int i = 0; i < point.Count; i++)
            {
                Point tmp;
                tmp.x = point[i].x - minX;
                tmp.y = point[i].y - minY;
                point[i] = tmp;
            }
        }

        private new void Resize(ref List<Point> a, double koeff)
        {
            for (int i = 0; i < a.Count; i++)
            {
                Point tmp;
                tmp.x = a[i].x * koeff;
                tmp.y = a[i].y * koeff;
                a[i] = tmp;
            }
        }

        private void Draw_Points(List<Point> point, List<Point> address)
        {
            Graphics g = Graphics.FromHwnd(funcPrinterBox.Handle);
            for (int i = 0; i < point.Count; i++)
            {
                String drawString = "[" + Convert.ToString(address[i].x) + ";" + Convert.ToString(address[i].y) + "]";
                g.FillEllipse(Brushes.Black, Convert.ToInt32(point[i].x - 3), Convert.ToInt32(point[i].y - 3), 5, 5);

                int size = 10;
                Font drawFont = new Font("Arial", size);
                SolidBrush drawBrush = new SolidBrush(Color.Brown);

                PointF drawPoint = new PointF(Convert.ToInt32(point[i].x), Convert.ToInt32(point[i].y + 5));
                StringFormat drawFormat = new StringFormat();
                g.DrawString(drawString, drawFont, drawBrush, drawPoint, drawFormat);
            }
        }

        private void Draw_Poly(List<Point> lagrange)
        {
            Pen myPen = new Pen(Color.Black, 1);
            Graphics g = Graphics.FromHwnd(funcPrinterBox.Handle);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            System.Drawing.Point
                from = new System.Drawing.Point(0, 0),
                to = new System.Drawing.Point(0, 0);

            for (int i = 1; i < lagrange.Count; i++)
            {
                from.X = (int)lagrange[i - 1].x;
                from.Y = (int)lagrange[i - 1].y;
                to.X = (int)lagrange[i].x;
                to.Y = (int)lagrange[i].y;
                /*
                from.X = (int)lagrange[i - 1].x;
                from.Y = (int)lagrange[i - 1].y;
                to.X = (int)lagrange[i].x;
                to.Y = (int)lagrange[i].y;
                 */


                g.DrawLine(myPen, from, to);
            }
        }

        private void DrawXY()
        {
            //int w = funcPrinterBox.Size.Width *1.2, h = funcPrinterBox.Size.Height*1.2;
            int w = funcPrinterBox.Size.Width, h = funcPrinterBox.Size.Height;

            Pen myPen = new Pen(Color.Black, 1);
            Graphics g = Graphics.FromHwnd(funcPrinterBox.Handle);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            System.Drawing.Point
            from = new System.Drawing.Point(0, h / 2),
            to = new System.Drawing.Point(w, h / 2);
            g.DrawLine(myPen, from, to);

            g.DrawLine(myPen, to, new System.Drawing.Point(w - 20, h / 2 + 5));
            g.DrawLine(myPen, to, new System.Drawing.Point(w - 20, h / 2 - 5));

            from = new System.Drawing.Point(w / 2, h);
            to = new System.Drawing.Point(w / 2, 0);
            g.DrawLine(myPen, from, to);

            g.DrawLine(myPen, to, new System.Drawing.Point(w / 2 - 5, 20));
            g.DrawLine(myPen, to, new System.Drawing.Point(w / 2 + 5, 20));
        }

        private void ClearBox()
        {
            Graphics g = Graphics.FromHwnd(funcPrinterBox.Handle);
            g.Clear(funcPrinterBox.BackColor);
            //g.Clear(funcPrinterBox);
        }

        private void dgvCoords_CellEndEdit(object sender, DataGridViewCellEventArgs e) //Sprawdzanie wprowadzonej wartości w komórce
        {
            try
            {
                dgvCoords[e.ColumnIndex, e.RowIndex].Value = dgvCoords[e.ColumnIndex, e.RowIndex].Value.ToString().Replace('.', ',');
                Convert.ToDouble(dgvCoords[e.ColumnIndex, e.RowIndex].Value);
            }
            catch
            {
                MessageBox.Show("Podaj poprawnie współrzędne!");
                dgvCoords.CurrentCell = dgvCoords[e.ColumnIndex, e.RowIndex];
                dgvCoords[e.ColumnIndex, e.RowIndex].Value = "";
            }
        }
        /*
        private void dgvCoords_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            for (int i = 0; i < dgvCoords.Rows.Count - 1; i++)
                if (dgvCoords.Rows[i].Cells[2] == dgvCoords.CurrentCell)
                {
                    dgvCoords.Rows.RemoveAt(i);
                    dgvCoords.CurrentCell = null;
                    return;
                }
        }
        */
        private void button1_Click(object sender, EventArgs e)
        {
            int i = 0;
            int x = 0;
            int n = 0;

            if(textBox1.Text == "" || textBox1.Text == null)
            {
                MessageBox.Show("Podaj ilość punktów!");
            }
            else
            {
                i = Int32.Parse(textBox1.Text);
            }
            if (textBox2.Text == "" || textBox2.Text == null)
            {
                MessageBox.Show("Podaj wartosc x!");
            }
            else
            {
                x = Int32.Parse(textBox2.Text);
                //int i = Int32.Parse(textBox1.Text);
                //int x = Int32.Parse(textBox2.Text);
                Point[] f = new Point[i];

                for (int j = 0; j < i; j++)
                {
                    int kol1 = Int32.Parse(dgvCoords.Rows[j].Cells[0].Value.ToString());
                    int kol2 = Int32.Parse(dgvCoords.Rows[j].Cells[1].Value.ToString());
                    f[j] = new Point(kol1, kol2);
                }
                //f[0] = new Point(0, 2);
                //f[1] = new Point(1, 3);
                //f[2] = new Point(2, 12);
                //f[3] = new Point(5, 147);

                label1.Text = "Wynik: Wartosc f(" + x + ") jest: " + (int)interpolacja(f, x, i);
            }
            
        }

        private void buttonFuncInCoord_Click_1(object sender, EventArgs e)
        {
            ClearBox();
            List<Point> punkty = DgvNaList(dgvCoords);

            List<Point> adresy = new List<Point>();
            for (int i = 0; i < punkty.Count; i++)
            {
                adresy.Add(punkty[i]);
            }

            double skalowanie = countResizeKoeffWithZero(punkty);
            skalowanie *= 0.5;
            Resize(ref punkty, skalowanie);
            ReverseY(ref punkty);
            double w = funcPrinterBox.Size.Width, h = funcPrinterBox.Size.Height;
            PlusXY(punkty, w / 2, h / 2);

            List<Point> lagrange = funkcjaLagranga(punkty);
            DrawXY();
            Draw_Points(punkty, adresy);
            Draw_Poly(lagrange);
        }
    }
}
