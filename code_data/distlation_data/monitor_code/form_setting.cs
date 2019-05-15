using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SerialPort_ViewSWUST1205
{
    public partial class form_setting : Form
    {
        public string th, tl, wh, wl;

        #region  
        public form_setting(string th1,string tl1, string wh1, string wl1)
        {
            InitializeComponent();
            th = th1;
            tl = tl1;
            wh = wh1;
            wl = wl1;
        }
        private void form_setting_Load(object sender, EventArgs e)
        {
            textBox1.Text = th;
            textBox1.Select(0,0);
            textBox3.Text = tl;
            textBox2.Text = wh;
            textBox4.Text = wl;

        }
        #endregion

        #region    + -
        /// <summary>
        /// H+
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click_1(object sender, EventArgs e)
        {
            textBox1.Text = (float.Parse(textBox1.Text.ToString()) + 0.1).ToString("#0.0");  //1
            if (float.Parse(textBox1.Text.ToString()) > 50)
            {
                MessageBox.Show("数据非法！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                textBox1.Text = "25.0";
            }
        }
        /// <summary>
        /// H-
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click_1(object sender, EventArgs e)
        {
            textBox1.Text = (float.Parse(textBox1.Text.ToString()) - 0.1).ToString("#0.0");  //1
            if (float.Parse(textBox1.Text.ToString()) < float.Parse(textBox3.Text.ToString()))
            {
                MessageBox.Show("数据非法！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                textBox1.Text = "25.0";
            }
        }

        /// <summary>
        /// L+
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click_1(object sender, EventArgs e)
        {
            textBox3.Text = (float.Parse(textBox3.Text.ToString()) + 0.1).ToString("#0.0");  //1
            if (float.Parse(textBox3.Text.ToString()) > float.Parse(textBox1.Text.ToString()))
            {
                MessageBox.Show("数据非法！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                textBox3.Text = "15.0";
            }
        }

        /// <summary>
        /// L-
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click_1(object sender, EventArgs e)
        {
            textBox3.Text = (float.Parse(textBox3.Text.ToString()) - 0.1).ToString("#0.0");  //1
            if (float.Parse(textBox3.Text.ToString()) < 0.1 || float.Parse(textBox3.Text.ToString()) > float.Parse(textBox1.Text.ToString()))
            {
                MessageBox.Show("数据非法！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                textBox3.Text = "15.0";
            }
        }

        /// <summary>
        /// H+
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click_1(object sender, EventArgs e)
        {
            textBox2.Text = (float.Parse(textBox2.Text.ToString()) + 0.1).ToString("#0.0");
            if (float.Parse(textBox2.Text.ToString()) > 70)
            {
                MessageBox.Show("数据非法！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                textBox2.Text = "27.0";
            }
        }

        /// <summary>
        /// H-
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click_1(object sender, EventArgs e)
        {
            textBox2.Text = (float.Parse(textBox2.Text.ToString()) - 0.1).ToString("#0.0");
            if (float.Parse(textBox2.Text.ToString()) < float.Parse(textBox4.Text.ToString()))
            {
                MessageBox.Show("数据非法！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                textBox2.Text = "27.0";
            }
        }

        /// <summary>
        /// L+
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button14_Click_1(object sender, EventArgs e)
        {
            textBox4.Text = (float.Parse(textBox4.Text.ToString()) + 0.1).ToString("#0.0");
            if (float.Parse(textBox4.Text.ToString()) > float.Parse(textBox2.Text.ToString()))
            {
                MessageBox.Show("数据非法！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                textBox4.Text = "10.0";
            }
        }

        /// <summary>
        /// L-
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button15_Click_1(object sender, EventArgs e)
        {
            textBox4.Text = (float.Parse(textBox4.Text.ToString()) - 0.1).ToString("#0.0");  //1
            if (float.Parse(textBox4.Text.ToString()) < 0.1 || float.Parse(textBox4.Text.ToString()) > float.Parse(textBox2.Text.ToString()))
            {
                MessageBox.Show("数据非法！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                textBox4.Text = "10.0";
            }
        }
        #endregion

        #region  
        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length<4|| textBox3.Text.Length<4|| textBox2.Text.Length < 4 || textBox4.Text.Length < 4)
            {
                MessageBox.Show("请检查输入格式，正确输入格式：12.1，30.0（保留一位小数）！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            th = textBox1.Text;
            tl = textBox3.Text;
            wh = textBox2.Text;
            wl = textBox4.Text;
            string t = tl + "-" + th;
            string h = wl + "%-" + wh+"%";
            try
            {
                texthandl_seting("t", t);
                texthandl_seting("h", h);                
                this.Close();
            }
            catch
            {
                this.Close();
            }
        }
        #endregion

        #region  
        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region  txt
        string logFile = System.AppDomain.CurrentDomain.BaseDirectory + @"seting.txt";
        private StreamWriter writer;
        private FileStream fileStream = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="passwd"></param>
        /// <returns></returns>
        public bool texthandl_seting(string name, string passwd)
        {
            bool flagxiu = false;           
            StreamReader xiao_str1 = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + @"seting.txt");
            string text = "";
            string[] ary = null;
            int i = 0;
            var newpasswd = passwd.Split(new char[2] { ':', ' ' });
            try
            {
                while (text != null)
                {
                    text = xiao_str1.ReadLine();
                    i++;
                    if (name == text)
                    {
                        if (newpasswd.Count() > 2)
                        {
                            ary = File.ReadAllLines(System.AppDomain.CurrentDomain.BaseDirectory + @"seting.txt", Encoding.UTF8);   //txt
                            ary[i] = newpasswd[3];
                        }
                        if (newpasswd.Count() == 1)
                        {
                            ary = File.ReadAllLines(System.AppDomain.CurrentDomain.BaseDirectory + @"seting.txt", Encoding.UTF8);   //txt
                            ary[i] = passwd;
                        }
                    }
                }
                xiao_str1.Close();
                int j = 0;
                cleartext("seting.txt");
                while (j < i - 1)
                {
                    log(ary[j]);
                    j++;
                }
                flagxiu = true;
                return flagxiu;
            }
            catch
            {
                return flagxiu;
            }
        }

        /// <summary>
        /// text
        /// </summary>
        /// <param name="textname"></param>
        public void cleartext(string textname)
        {
            string appdir = System.AppDomain.CurrentDomain.BaseDirectory + @textname;
            FileStream stream = File.Open(appdir, FileMode.OpenOrCreate, FileAccess.Write);
            stream.Seek(0, SeekOrigin.Begin);
            stream.SetLength(0);
            stream.Close();       //
        }

        /// <summary>
        /// txt
        /// </summary>
        /// <param name="info"></param>
        public void log(string info)
        {

            try
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(logFile);
                if (!fileInfo.Exists)      //passwdtxt
                {
                    fileStream = fileInfo.Create();
                    writer = new StreamWriter(fileStream);
                }
                else
                {
                    fileStream = fileInfo.Open(FileMode.Append, FileAccess.Write);   // 
                    writer = new StreamWriter(fileStream);
                }
                writer.WriteLine(info);    //
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
        }

        #endregion


    }
}
