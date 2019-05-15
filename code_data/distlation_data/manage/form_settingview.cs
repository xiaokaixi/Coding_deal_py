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
    public partial class form_settingview : Form
    {
        public int test;
        #region  
        public passwdtext check = new passwdtext();
        #endregion
        public form_settingview()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form8_Load(object sender, EventArgs e)
        {
            test = 100;
            checkset();
        }

        /// <summary>
        /// 
        /// </summary>
        private void checkset()
        {
            try
            {
                label5.Text = check.logincheckset("temph");  //H
                label17.Text = check.logincheckset("templ");  //L
                label6.Text = check.logincheckset("weth");   //H
                label19.Text = check.logincheckset("wetl");   //L
                if (int.Parse(check.logincheckset("time1"))<11)
                {
                    label7.Text = (int.Parse(check.logincheckset("time1"))+1).ToString() + "";  //
                }
                if (int.Parse(check.logincheckset("time1")) >= 11)
                {
                    label7.Text = (int.Parse(check.logincheckset("time1"))-10).ToString() + " ";  //
                }
                if (int.Parse(check.logincheckset("time2")) < 11)
                {
                    label8.Text = (int.Parse(check.logincheckset("time2"))+1).ToString() + "";  //
                }
                if (int.Parse(check.logincheckset("time2")) >= 11)
                {
                    label8.Text = (int.Parse(check.logincheckset("time2")) - 10).ToString() + " ";  //
                }
                label21.Text = ((int.Parse(check.logincheckset("controlsum")))+1).ToString() + "";        // 
                label23.Text= ((int.Parse(check.logincheckset("persumh")) + 1) * 3).ToString()+"";        //
                label15.Text = ((int.Parse(check.logincheckset("borrowday")))+1).ToString() + "";         //
                label16.Text= ((int.Parse(check.logincheckset("warningday")))+1).ToString() + "";           //
            }
            catch
            {
                MessageBox.Show("查询失败！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

    }
}
