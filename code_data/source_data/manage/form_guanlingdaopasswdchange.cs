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
    public partial class form_guanlingdaopasswdchange : Form
    {
        public string dirpath = Application.StartupPath + "\\FreeVK.exe";  //获取应用程序路径
        public dynamic file;
        public Process p1;
        public form_guanlingdaopasswdchange()
        {
            InitializeComponent();
        }

        private void guanlidaopasswd_Load(object sender, EventArgs e)
        {
            textBox1.Enabled = false;
            List<string> list = new List<string>();
            try
            {
                list = check.logincheckset_contin("g");  
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
        /// 密码修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public passwdtext check = new passwdtext();
        private void button5_Click(object sender, EventArgs e)
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
            if (check.logincheck(comboBox1.Text.ToString(), textBox3.Text.ToString()) || check.logincheck("guanleader2", textBox3.Text.ToString()) || check.logincheck("guanleader3", textBox3.Text.ToString()))
            {
                if (check.texthandle(comboBox1.Text.ToString(), textBox4.ToString()) || check.texthandle("guanleader2", textBox4.ToString()) || check.texthandle("guanleader3", textBox4.ToString()))
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
        /// 打开虚拟键盘
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
        /// 退出密码修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                p1.CloseMainWindow();
                this.Close();
            }
            catch
            {
                this.Close();
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            //权限认证
            if (button3.Text == "获  取")
            {
                form_guanlingdaopasswd gldpwd = new form_guanlingdaopasswd(null, 0, null);
                gldpwd.Text = "馆领导权限认证";
                gldpwd.ShowDialog();
                if (gldpwd.rightorwrang == false) { return; }
            }
            string str;
            if (button3.Text == "获  取") 
            {
                button3.Text = "写  入";
                textBox1.Enabled = true;
                //读取二进制文件,并转换
                str= check.logincheckset_lingdao("guanlingdao_keywd");
                textBox1.Text = jiema(str);
                return;
            }
            if(button3.Text == "写  入")
            {
                button3.Text = "获  取";
                str = bianma(textBox1.Text);
                if (check.texthandl_ldkeywd("guanlingdao_keywd", str))
                {
                    MessageBox.Show("口令写入成功！", "Success", MessageBoxButtons.OKCancel, MessageBoxIcon.None);
                    textBox1.Text = "";
                    textBox1.Enabled = false;
                    try
                    {
                        p1.CloseMainWindow();
                    }
                    catch
                    {  }
                    return;
                }
                else {  }
            }

        }

        /// <summary>
        /// 二进制转ASII转换
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string jiema(string s)
        {
            System.Text.RegularExpressions.CaptureCollection cs =System.Text.RegularExpressions.Regex.Match(s, @"([01]{8})+").Groups[1].Captures;
            byte[] data = new byte[cs.Count];
            for (int i = 0; i < cs.Count; i++)
            {
                data[i] = Convert.ToByte(cs[i].Value, 2);
            }
            return Encoding.Unicode.GetString(data, 0, data.Length);
        }

        /// <summary>
        /// ASII转二进制
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string bianma(string s)
        {
            byte[] data = Encoding.Unicode.GetBytes(s);
            StringBuilder result = new StringBuilder(data.Length * 8);

            foreach (byte b in data)
            {
                result.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }
            return result.ToString();
        }
    }
}
