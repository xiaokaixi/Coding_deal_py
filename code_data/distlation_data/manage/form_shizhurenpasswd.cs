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
    public partial class form_shizhurenpasswd : Form
    {
        #region  
        public passwdtext check = new passwdtext();
        public string dirpath = Application.StartupPath + "\\FreeVK.exe";  //
        public dynamic file;
        public Process p1;
        public firtdoor f1;
        public int controlnum;
        public bool rightorwrang = false;
        List<string> positionlist = new List<string>();
        #endregion
        public form_shizhurenpasswd(firtdoor f, int controlnum1,List<string> positionlist1)
        {
            InitializeComponent();
            f1 = f;
            controlnum = controlnum1;
            positionlist = positionlist1;
        }

        private void form_shizhurenpasswd_Load(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            try
            {
                //
                //file = dirpath;
                //if (!System.IO.File.Exists(file))
                //    return;
                //p1 = Process.Start(file);
                //
                if (positionlist == null)
                {
                    list = check.logincheckset_contin("s");  //
                    for (int i = 0; i < list.Count; i++)
                    {
                        comboBox1.Items.Add(list[i]);
                    }
                    textBox1.PasswordChar = '*';
                }
                else
                {
                    label2.Text = "账户编号：";
                    label1.Text = "登录密码：";
                    list = check.logincheckset_contin("c");  //
                    for (int i = 0; i < list.Count; i++)
                    {
                        comboBox1.Items.Add(list[i]);
                    }
                    list = check.logincheckset_contin("s");  //
                    for (int i = 0; i < list.Count; i++)
                    {
                        comboBox1.Items.Add(list[i]);
                    }
                    list = check.logincheckset_contin("g");  //
                    for (int i = 0; i < list.Count; i++)
                    {
                        comboBox1.Items.Add(list[i]);
                    }
                    textBox1.PasswordChar = '*';
                }
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
        private void button17_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.ToString() == "")
            {
                MessageBox.Show("请输入密码！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    passwdtext check = new passwdtext();
                    if (check.logincheck(comboBox1.Text, textBox1.Text.ToString()))
                    {
                        if (controlnum != 0)
                        {
                            Hide();
                            //p1.CloseMainWindow();
                            this.Close();
                            form_guanlingdaopasswd gld = new form_guanlingdaopasswd(f1, controlnum, null);
                            gld.Text = "紧急开锁认证-馆领导权限";
                            gld.ShowDialog();
                            rightorwrang = gld.rightorwrang;
                        }
                        if(positionlist!=null)
                        {
                            Hide();
                            this.Close();
                            positionlist.Add(comboBox1.Text);
                            form_guanlingdaopasswd gld = new form_guanlingdaopasswd(f1, 0, positionlist);
                            gld.Text = "出入库整理开锁权限认证";
                            gld.ShowDialog();
                            rightorwrang = gld.rightorwrang;
                        }
                    }
                    else
                    {
                        MessageBox.Show("请检查密码！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        textBox1.Text = "";
                        textBox1.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = "";
                p1.CloseMainWindow();
                rightorwrang = false;
                this.Close();
            }
            catch
            {
                textBox1.Text = "";
                rightorwrang = false;
                this.Close();
            }
        }
    }
}
