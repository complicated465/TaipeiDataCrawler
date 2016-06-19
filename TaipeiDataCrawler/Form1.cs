using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaipeiDataCrawler
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
            WebCrawer myWebCrawer = new WebCrawer(this);
            myWebCrawer.craw();
          
            
        }


        public void update(List<Data> dataList)
        {
            comboBox1.DataSource = dataList;
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "RID";
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WebCrawer myWebCrawer = new WebCrawer(this);
            myWebCrawer.craw(new string[] { comboBox1.Text, comboBox1.SelectedValue.ToString() });
        }
    }
}
