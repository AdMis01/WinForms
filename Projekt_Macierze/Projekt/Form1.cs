using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projekt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            dataGridView1.AllowUserToAddRows = false;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView3.AllowUserToAddRows = false;
        }
        int xMacierzA = 0;
        int yMacierzA = 0;
        int xMacierzB = 0;
        int yMacierzB = 0;

        

        private void button1_Click(object sender, EventArgs e)
        {
            xMacierzB = int.Parse(textBox1.Text);
            yMacierzB = int.Parse(textBox2.Text);
            for (int i = 0; i < yMacierzB; i++)
            {
                dataGridView2.Columns.Add("", "");
                DataGridViewColumn column = dataGridView2.Columns[i];
            }
            dataGridView2.Rows.Add(xMacierzB);


        }

        private void button2_Click(object sender, EventArgs e)
        {
            xMacierzA = int.Parse(textBox1.Text);
            yMacierzA = int.Parse(textBox2.Text);

            for (int i = 0; i < yMacierzA; i++)
            {
                dataGridView1.Columns.Add("", "");
                DataGridViewColumn column = dataGridView1.Columns[i];
            }
            dataGridView1.Rows.Add(xMacierzA);
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (yMacierzA != xMacierzB)
            {
                MessageBox.Show("Nie moża policzyć tych macierzy");
            }
            else
            {
                for (int i = 0; i < yMacierzB; i++)
                {
                    dataGridView3.Columns.Add("", "");
                    DataGridViewColumn column = dataGridView3.Columns[i];
                }
                dataGridView3.Rows.Add(xMacierzA);

                double[,] macierzA;
                macierzA = new double[xMacierzA, yMacierzA];
                double[,] macierzB;
                macierzB = new double[xMacierzB, yMacierzB];

                for (int i = 0; i < xMacierzA; i++)
                {
                    for (int j = 0; j < yMacierzA; j++)
                    {
                        macierzA[i, j] = double.Parse(dataGridView1[j, i].Value.ToString());
                    }
                }
                for (int i = 0; i < xMacierzB; i++)
                {
                    for (int j = 0; j < yMacierzB; j++)
                    {
                        macierzB[i, j] = double.Parse(dataGridView2[j, i].Value.ToString());
                    }
                }
                double[,] macierzWynikowa = new double[xMacierzA, yMacierzB];

                for (int i = 0; i < xMacierzA; i++)
                {
                    for (int j = 0; j < yMacierzB; j++)
                    {
                        macierzWynikowa[i, j] = 0;
                        for (int k = 0; k < xMacierzB; k++)
                        {
                            macierzWynikowa[i, j] += macierzA[i, k] * macierzB[k, j];
                        }
                    }
                }

                for (int i = 0; i < xMacierzA; i++)
                {
                    for (int k = 0; k < yMacierzB; k++)
                    {
                        dataGridView3[k, i].Value = macierzWynikowa[i, k];
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();
            dataGridView2.Rows.Clear();
            dataGridView2.Columns.Clear();
            dataGridView2.Refresh();
            dataGridView3.Rows.Clear();
            dataGridView3.Columns.Clear();
            dataGridView3.Refresh();

        }
    }
}
