﻿using System;
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
    public partial class form_managepassschange : Form
    {

        public string dirpath = Application.StartupPath + "\\FreeVK.exe";  //获取应用程序路径
        public dynamic file;
        public Process p1;
        public form_managepassschange()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 密码修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public passwdtext check = new passwdtext();
        private void button5_Click_1(object sender, EventArgs e)
        {
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
            if (check.logincheck("manager", textBox3.Text.ToString()))
            {
                if (check.texthandle("manager", textBox4.ToString()))
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
            this.Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
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
    }
}