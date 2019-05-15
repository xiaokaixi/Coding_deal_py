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
    public partial class form_settingpasswd : Form
    {

        #region  
        public string dirpath = Application.StartupPath + "\\FreeVK.exe";  //
        public dynamic file;
        public Process p1;
        #endregion
        public form_settingpasswd()
        {
            InitializeComponent();
        }

        public passwdtext check = new passwdtext();
        private void Form4_Load(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            try
            {
                //
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
                comboBox1.Items.Add("manager");
                textBox1.PasswordChar = '*';
            }
            catch (Exception)
            {

            }
        }

        #region  
        public string temph, weth, templ, wetl, time1, time2, gate, rfid, controlsum, persumh, borrowday, warningday;
        private void button17_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.ToString() == ""|| comboBox1.Text.ToString()=="")
            {
                MessageBox.Show("请检查编号和密码！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    passwdtext check = new passwdtext();
                    if (check.logincheck(comboBox1.Text, textBox1.Text.ToString()))
                    {
                        //MessageBox.Show("欢迎登录！");
                        Hide();
                        this.Close();
                        //form5
                        form_setting f5 = new form_setting();
                        f5.ShowDialog();
                        temph = f5.temph;
                        templ = f5.templ;
                        weth = f5.weth;
                        wetl = f5.wetl;
                        time1 =f5.time1;
                        time2 = f5.time2;
                        controlsum = f5.controlsum;
                        persumh = f5.persumh;
                        borrowday = f5.borrowday;
                        warningday = f5.warningday;
                    }
                    else
                    {
                        MessageBox.Show("请检查密码！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        textBox1.Text = "";
                        textBox1.Focus();
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        #endregion

        #region  
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            this.Close();
        }
        #endregion
    }
}
