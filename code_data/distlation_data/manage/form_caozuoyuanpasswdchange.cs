using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace manage
{
    public partial class form_caozuoyuanpasswdchange : Form
    {
        public string dirpath = Application.StartupPath + "\\FreeVK.exe";  //
        public dynamic file;
        public Process p1;
        public form_caozuoyuanpasswdchange()
        {
            InitializeComponent();
        }

        private void caozuyuanpasswd_Load(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            try
            {
                list = check.logincheckset_contin("c");  
                for (int i = 0; i < list.Count; i++)
                {
                    comboBox1.Items.Add(list[i]);
                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public passwdtext check = new passwdtext();
        private void button5_Click_1(object sender, EventArgs e)
        {
            if (comboBox1.Text == "")
            {
                MessageBox.Show("请输入用户名！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (textBox3.Text == "")
            {
                MessageBox.Show("请输入原密码！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (textBox4.Text == "")
            {
                MessageBox.Show("请输入新密码！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (check.logincheck(comboBox1.Text.ToString(), textBox3.Text.ToString()))
            {
                if (check.texthandle(comboBox1.Text.ToString(), textBox4.ToString()))
                {
                    MessageBox.Show("修改密码成功！", "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
                    textBox3.Text = "";
                    textBox4.Text = "";
                    this.Close();
                }
                else
                {
                    MessageBox.Show("修改密码失败！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox3.Text = "";
                    textBox4.Text = "";
                }
            }
            else
            {
                MessageBox.Show("原密码错误！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox3.Text = "";
                textBox4.Text = "";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                file = dirpath;
                if (!System.IO.File.Exists(file))
                    return;
                p1 = Process.Start(file);
            }
            catch (Exception)
            {
                int i = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
