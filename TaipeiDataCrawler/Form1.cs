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
            comboBox1.Items.Add("ddd");

        }

        public void update(string message)
        {
            comboBox1.Items.Add(message);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

    }
}
