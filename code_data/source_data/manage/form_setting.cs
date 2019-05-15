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
    public partial class form_setting : Form
    {
        #region  全局变量
        public string dirpath = Application.StartupPath + "\\FreeVK.exe";  //获取应用程序路径
        public dynamic file;
        public Process p1;
        #endregion
        public form_setting()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体引导
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form5_Load(object sender, EventArgs e)
        {
            //读取参数
            checkset();
            //管理员管理密码
            textBox5.PasswordChar = '*';
        }

        /// <summary>
        /// 温度H+
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = (float.Parse(textBox1.Text.ToString()) + 1).ToString("#0.0");  //保留1位小数
            if (float.Parse(textBox1.Text.ToString()) >50)
            {
                MessageBox.Show("数据非法！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                textBox1.Text = "25.0";
            }
        }
        /// <summary>
        /// 温度H-
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = (float.Parse(textBox1.Text.ToString()) - 1).ToString("#0.0");  //保留1位小数
            if (float.Parse(textBox1.Text.ToString()) < float.Parse(textBox3.Text.ToString()))
            {
                MessageBox.Show("数据非法！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                textBox1.Text = "25.0";
            }
        }

        /// <summary>
        /// 温度L+
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            textBox3.Text = (float.Parse(textBox3.Text.ToString()) + 1).ToString("#0.0");  //保留1位小数
            if (float.Parse(textBox3.Text.ToString()) > float.Parse(textBox1.Text.ToString()))
            {
                MessageBox.Show("数据非法！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                textBox3.Text = "15.0";
            }
        }

        /// <summary>
        /// 温度L-
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            textBox3.Text = (float.Parse(textBox3.Text.ToString()) - 1).ToString("#0.0");  //保留1位小数
            if (float.Parse(textBox3.Text.ToString()) < 0.1|| float.Parse(textBox3.Text.ToString()) > float.Parse(textBox1.Text.ToString()))
            {
                MessageBox.Show("数据非法！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                textBox3.Text = "15.0";
            }
        }

        /// <summary>
        /// 湿度H+
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.Text = (float.Parse(textBox2.Text.ToString()) + 1).ToString("#0.0");
            if (float.Parse(textBox2.Text.ToString()) > 70 )
            {
                MessageBox.Show("数据非法！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                textBox2.Text = "27.0";
            }
        }
        /// <summary>
        /// 湿度H-
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            textBox2.Text = (float.Parse(textBox2.Text.ToString()) - 1).ToString("#0.0");
            if (float.Parse(textBox2.Text.ToString()) < float.Parse(textBox4.Text.ToString()))
            {
                MessageBox.Show("数据非法！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                textBox2.Text = "27.0";
            }
        }

        /// <summary>
        /// 湿度L+
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button14_Click(object sender, EventArgs e)
        {
            textBox4.Text = (float.Parse(textBox4.Text.ToString()) + 1).ToString("#0.0");
            if (float.Parse(textBox4.Text.ToString()) > float.Parse(textBox2.Text.ToString()))
            {
                MessageBox.Show("数据非法！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                textBox4.Text = "10.0";
            }
        }

        /// <summary>
        /// 湿度L-
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button15_Click(object sender, EventArgs e)
        {
            textBox4.Text = (float.Parse(textBox4.Text.ToString()) - 1).ToString("#0.0");  //保留1位小数
            if (float.Parse(textBox4.Text.ToString()) < 0.1 || float.Parse(textBox4.Text.ToString()) > float.Parse(textBox2.Text.ToString()))
            {
                MessageBox.Show("数据非法！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                textBox4.Text = "10.0";
            }
        }

        /// <summary>
        /// 取消设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
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
        /// <summary>
        /// 保存设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                check.texthandl_seting("temph", textBox1.ToString());
                check.texthandl_seting("templ", textBox3.ToString());
                check.texthandl_seting("weth", textBox2.ToString());
                check.texthandl_seting("wetl", textBox4.ToString());
                check.texthandl_seting("time1",comboBox1.SelectedIndex.ToString());
                check.texthandl_seting("time2",comboBox2.SelectedIndex.ToString());
                check.texthandl_seting("controlsum", comboBox6.SelectedIndex.ToString());
                check.texthandl_seting("persumh", comboBox4.SelectedIndex.ToString());
                check.texthandl_seting("borrowday", comboBox3.SelectedIndex.ToString());
                check.texthandl_seting("warningday", comboBox5.SelectedIndex.ToString());
                checkset();
                MessageBox.Show("设置成功！", "Success", MessageBoxButtons.OKCancel, MessageBoxIcon.None);
                p1.CloseMainWindow();
                this.Close();                
            }
            catch
            {
                this.Close();
            }
        }

        /// <summary>
        /// 打开虚拟键盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
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

        #region  操作员+

        private void button16_Click(object sender, EventArgs e)
        {
            form_adduser adduser = new form_adduser("c");
            adduser.ShowDialog();
            checkedListBox1.Items.Clear();
            //查操作员标号列表
            list = check.logincheckset_contin("c");  //操作员
            for (int i = 0; i < list.Count; i++)
            {
                checkedListBox1.Items.Add(list[i]);
            }
        }
        #endregion

        #region  操作员 -
        private void button20_Click(object sender, EventArgs e)
        {
            string username = "";
            try
            {
                username = checkedListBox1.SelectedItem.ToString();
            }
            catch
            {
                MessageBox.Show("请选择删除的用户名！", "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
            if (check.texthandle_cut(username))
            {
                MessageBox.Show("删除操作员成功！", "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
                checkedListBox1.Items.Clear();
                //查操作员标号列表
                list = check.logincheckset_contin("c");  //操作员
                for (int i = 0; i < list.Count; i++)
                {
                    checkedListBox1.Items.Add(list[i]);
                }
            }
            else
            {
                MessageBox.Show("删除操作员失败！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region 室主任 +
        private void button17_Click(object sender, EventArgs e)
        {
            form_adduser adduser = new form_adduser("s");
            adduser.ShowDialog();
            checkedListBox2.Items.Clear();
            //查操作员标号列表
            list = check.logincheckset_contin("s");  //操作员
            for (int i = 0; i < list.Count; i++)
            {
                checkedListBox2.Items.Add(list[i]);
            }
        }
        #endregion

        #region 室主任 -
        private void button21_Click(object sender, EventArgs e)
        {
            string username = "";
            try
            {
                username = checkedListBox2.SelectedItem.ToString();
            }
            catch
            {
                MessageBox.Show("请选择删除的用户名！", "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
            if (check.texthandle_cut(username))
            {
                MessageBox.Show("删除室主任用户成功！", "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
                checkedListBox2.Items.Clear();
                //查操作员标号列表
                list = check.logincheckset_contin("s");  //操作员
                for (int i = 0; i < list.Count; i++)
                {
                    checkedListBox2.Items.Add(list[i]);
                }
            }
            else
            {
                MessageBox.Show("删除室主任用户失败！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        #region  馆领导 +
        private void button18_Click(object sender, EventArgs e)
        {
            form_adduser adduser = new form_adduser("g");
            adduser.ShowDialog();
            checkedListBox3.Items.Clear();
            //查操作员标号列表
            list = check.logincheckset_contin("g");  //操作员
            for (int i = 0; i < list.Count; i++)
            {
                checkedListBox3.Items.Add(list[i]);
            }
        }
        #endregion

        #region  馆领导 -
        private void button22_Click(object sender, EventArgs e)
        {
            string username = "";
            try
            {
                username = checkedListBox3.SelectedItem.ToString();
            }
            catch
            {
                MessageBox.Show("请选择删除的用户名！", "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
            if (check.texthandle_cut(username))
            {
                MessageBox.Show("删除馆领导用户成功！", "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
                checkedListBox3.Items.Clear();
                //查操作员标号列表
                list = check.logincheckset_contin("g");  //操作员
                for (int i = 0; i < list.Count; i++)
                {
                    checkedListBox3.Items.Add(list[i]);
                }
            }
            else
            {
                MessageBox.Show("删除室主任用户失败！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion


        /// <summary>
        /// 查询数据设置
        /// </summary>
        public List<string> list = new List<string>();
        public passwdtext check = new passwdtext();
        public string temph,weth, templ, wetl, time1,time2, controlsum, persumh, borrowday, warningday;

        #region  管理员密码登录
        private void button19_Click(object sender, EventArgs e)
        {
            
            if (textBox5.Text.ToString() == "")
            {
                MessageBox.Show("请输入密码！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    passwdtext check = new passwdtext();
                    if (check.logincheck("manager", textBox5.Text.ToString()))
                    {
                        panel1.Visible = false;
                        //查操作员标号列表
                        list = check.logincheckset_contin("c");  //操作员
                        for (int i = 0; i < list.Count; i++)
                        {
                            checkedListBox1.Items.Add(list[i]);
                            //checkedListBox1.SelectedIndex = i;
                        }
                        //查室主任标号列表
                        list = check.logincheckset_contin("s");  //室主任
                        for (int i = 0; i < list.Count; i++)
                        {
                            checkedListBox2.Items.Add(list[i]);
                            //checkedListBox2.SelectedIndex = i;
                        }
                        //查管领导标号列表
                        list = check.logincheckset_contin("g");  //管领导
                        for (int i = 0; i < list.Count; i++)
                        {
                            checkedListBox3.Items.Add(list[i]);
                            //checkedListBox3.SelectedIndex = i;
                        }
                    }
                    else
                    {
                        MessageBox.Show("请检查密码！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        textBox5.Text = "";
                        textBox5.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        #endregion

        private void checkset()
        {
            try
            {               
                textBox1.Text = check.logincheckset("temph");  //温度
                temph = textBox1.Text;
                textBox3.Text = check.logincheckset("templ");  //温度
                templ = textBox3.Text;
                textBox2.Text = check.logincheckset("weth");   //湿度
                weth = textBox2.Text;
                textBox4.Text = check.logincheckset("wetl");   //湿度
                wetl = textBox4.Text;
                comboBox1.SelectedIndex = int.Parse(check.logincheckset("time1"));  //出入库历史时限
                time1 = comboBox1.SelectedIndex.ToString();
                comboBox2.SelectedIndex = int.Parse(check.logincheckset("time2"));  //档案盒状态时限
                time2 = comboBox2.SelectedIndex.ToString();
                comboBox6.SelectedIndex = int.Parse(check.logincheckset("controlsum"));  //在用柜体数
                controlsum = comboBox6.SelectedIndex.ToString();
                comboBox4.SelectedIndex = int.Parse(check.logincheckset("persumh"));  //单人档案上限数
                persumh = comboBox4.SelectedIndex.ToString();
                comboBox3.SelectedIndex = int.Parse(check.logincheckset("borrowday"));  //档案借出时限
                borrowday = comboBox3.SelectedIndex.ToString();
                comboBox5.SelectedIndex = int.Parse(check.logincheckset("warningday"));  //档案借出时限
                warningday = comboBox5.SelectedIndex.ToString();

            }
            catch
            {
                MessageBox.Show("查询失败！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 操作员密码修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            form_caozuoyuanpasswdchange czy = new form_caozuoyuanpasswdchange();
            czy.ShowDialog();
        }

        /// <summary>
        /// 室主任密码修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            form_shizhurenpasswdchenge szr = new form_shizhurenpasswdchenge();
            szr.ShowDialog();
        }

        /// <summary>
        /// 馆领导密码修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            form_guanlingdaopasswdchange gld = new form_guanlingdaopasswdchange();
            gld.ShowDialog();
        }

        /// <summary>
        /// 管理员密码修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            form_managepassschange mag = new form_managepassschange();
            mag.ShowDialog();
        }

    }
}
