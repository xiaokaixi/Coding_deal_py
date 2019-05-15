using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace manage
{
    public partial class form_adduser : Form
    {

        public passwdtext check = new passwdtext();
        public string newuser,str;
        public form_adduser(string str1)
        {
            InitializeComponent();
            str=str1;
        }
       
        //确定
        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox3.Text.ToString() == "" || textBox4.Text.ToString() == "")
            {
                MessageBox.Show("请检查编号和密码！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if ((str == "c" && textBox3.Text.ToString().ToList()[0].ToString() == "c"))
            {
                if (check.texthandle_add(textBox3.ToString(), textBox4.ToString()))
                {
                    MessageBox.Show("添加操作员成功！", "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
                    newuser = textBox3.Text.ToString();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("添加操作员失败！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else if (str == "s" && textBox3.Text.ToString().ToList()[0].ToString() == "s")
            {
                if (check.texthandle_add(textBox3.ToString(), textBox4.ToString()))
                {
                    MessageBox.Show("添加室主任成功！", "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
                    newuser = textBox3.Text.ToString();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("添加室主任失败！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else if (str == "g" && textBox3.Text.ToString().ToList()[0].ToString() == "g")
            {
                if (check.texthandle_add(textBox3.ToString(), textBox4.ToString()))
                {
                    MessageBox.Show("添加馆领导成功！", "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
                    newuser = textBox3.Text.ToString();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("添加馆领导失败！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("用户名格式错误！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Text = "";
                textBox4.Text = "";
                return;
            } 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
