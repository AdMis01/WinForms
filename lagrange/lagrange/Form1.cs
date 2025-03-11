using System;
using System.Windows.Forms;

namespace lagrange
{
    public partial class Form1 : Form
    {
        class Data
        {
            public int x, y;
            public Data(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        };
        static double interpolacja(Data[] f,int xi, int n)
        {
            double wynik = 0;

            for (int i = 0; i < n; i++)
            {
                double wsp = f[i].y;
                for (int j = 0; j < n; j++)
                {
                    if (j != i)
                        wsp = wsp * (xi - f[j].x)/(f[i].x - f[j].x);
                }
                wynik += wsp;
            }
            return wynik;
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int i = Int32.Parse(textBox1.Text);
            int x = Int32.Parse(textBox2.Text);
            Data[] f = new Data[i];
            f[0] = new Data(0, 2);
            f[1] = new Data(1, 3);
            f[2] = new Data(2, 12);
            f[3] = new Data(5, 147);
            textBox1.Text = "Wartosc f("+x+") jest: " + (int)interpolacja(f, x, 4);
        }
    }
}
