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
        public List<Control> controlList = new List<Control>();
        public Form1()
        {
            InitializeComponent();
            WebCrawer myWebCrawer = new WebCrawer(this);
            myWebCrawer.craw();
            myWebCrawer.weatherCraw();
            myWebCrawer.govCraw();
            comboBox3.TextChanged += ComboBox3_TextChanged;
           
        }

       

        public void update(List<Data> dataList)
        {
            comboBox1.DataSource = dataList;
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "RID";
        }
        public void updateWeather(List<Data> dataList)
        {
            comboBox2.DataSource = dataList;
            comboBox2.DisplayMember = "weatherContent";
            comboBox2.ValueMember = "weatherKey";
        }
        public void updateGov(List<govData> dataList)
        {
            comboBox3.DataSource = dataList;
            comboBox3.DisplayMember = "tite";
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WebCrawer myWebCrawer = new WebCrawer(this);
            if (comboBox1.SelectedValue == null)
            {
                MessageBox.Show("無RID!!");
            }
            else
            {
                myWebCrawer.craw(new string[] { comboBox1.Text, comboBox1.SelectedValue.ToString() });
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WebCrawer myWebCrawer = new WebCrawer(this);
            if (comboBox2.SelectedValue == null)
            {
                MessageBox.Show("無Key!!");
            }
            else
            {
                myWebCrawer.weatherCraw(new string[] { comboBox2.Text, comboBox2.SelectedValue.ToString() });
            }
        }

        private void ComboBox3_TextChanged(object sender, EventArgs e)
        {
            if (controlList != null)
            {
                foreach (Control item in controlList)
                {
                    item.MouseEnter -= DownloadItem_MouseEnter;
                    item.Click -= DownloadItem_Click;
                    this.Controls.Remove(item);
                }
            }
           
            
            govData temp = comboBox3.SelectedItem as govData;
            int dis = 0;
           
            foreach (resourceData res in temp.distrbution)
            {
                downloadLabel downloadItem = new downloadLabel();
                downloadItem.AutoSize = true;
                downloadItem.Location = new System.Drawing.Point(21, 263+dis);
                downloadItem.Text = res.format;
                downloadItem.url = res.downloadURL;
                downloadItem.MouseEnter += DownloadItem_MouseEnter;
                downloadItem.MouseLeave += DownloadItem_MouseLeave; 
                downloadItem.Click += DownloadItem_Click;
                //downloadItem.Click += new EventHandler(a);
                controlList.Add(downloadItem);
                this.Controls.Add(downloadItem);
                dis += 20;
            }
           
            
        }

        private void DownloadItem_MouseLeave(object sender, EventArgs e)
        {
            downloadLabel temp = sender as downloadLabel;
            temp.ForeColor = Color.Black;
        }

        private void DownloadItem_MouseEnter(object sender, EventArgs e)
        {
            downloadLabel temp = sender as downloadLabel;
            temp.ForeColor = Color.Blue;
        }

        private void DownloadItem_Click(object sender, EventArgs e)
        {
            WebCrawer myWebCrawer = new WebCrawer(this);
            downloadLabel temp = sender as downloadLabel;
            if (temp.url == null)
            {
                MessageBox.Show("無Key!!");
            }
            else
            {
                myWebCrawer.govCraw(new string[] { comboBox3.Text,temp.url });
            }
        }
    }

    class downloadLabel : Label
    {
        public string url { set; get; }
    }
}
