using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
     
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Osoba osoba = new Osoba();
            osoba.wzrost = float.Parse(textBox1.Text);
            osoba.waga = float.Parse(textBox2.Text);
            osoba.wiek = float.Parse(textBox3.Text);
            float bmi = (float)(osoba.waga / Math.Pow(osoba.wzrost / 100,2));

            listBox1.Items.Add(bmi);
            richTextBox1.Text += "to jest to " + bmi;


            DataTable dt = new DataTable(); 
            DataColumn col = new DataColumn();
            col.ColumnName = "Name";
            col.DataType = typeof(string);
            dt.Columns.Add(col);
            DataRow dr = dt.NewRow();
            dr[0] = bmi;
            dt.Rows.Add(dr);   
            BindingSource bs = new BindingSource();
            bs.DataSource = dt;
            dataGridView1.DataSource = bs;
        }
    }
}
