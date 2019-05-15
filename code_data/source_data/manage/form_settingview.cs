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
        #region  全局变量
        public passwdtext check = new passwdtext();
        #endregion
        public form_settingview()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体引导
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form8_Load(object sender, EventArgs e)
        {
            test = 100;
            checkset();
        }

        /// <summary>
        /// 查询数据设置
        /// </summary>
        private void checkset()
        {
            try
            {
                label5.Text = check.logincheckset("temph");  //温度H
                label17.Text = check.logincheckset("templ");  //温度L
                label6.Text = check.logincheckset("weth");   //湿度H
                label19.Text = check.logincheckset("wetl");   //湿度L
                if (int.Parse(check.logincheckset("time1"))<11)
                {
                    label7.Text = (int.Parse(check.logincheckset("time1"))+1).ToString() + "个月";  //出入库历史时限
                }
                if (int.Parse(check.logincheckset("time1")) >= 11)
                {
                    label7.Text = (int.Parse(check.logincheckset("time1"))-10).ToString() + " 年";  //出入库历史时限
                }
                if (int.Parse(check.logincheckset("time2")) < 11)
                {
                    label8.Text = (int.Parse(check.logincheckset("time2"))+1).ToString() + "个月";  //档案盒状态时限
                }
                if (int.Parse(check.logincheckset("time2")) >= 11)
                {
                    label8.Text = (int.Parse(check.logincheckset("time2")) - 10).ToString() + " 年";  //档案盒状态时限
                }
                label21.Text = ((int.Parse(check.logincheckset("controlsum")))+1).ToString() + "个";        //在用柜体数 
                label23.Text= ((int.Parse(check.logincheckset("persumh")) + 1) * 3).ToString()+"盒";        //单人档案上限数
                label15.Text = ((int.Parse(check.logincheckset("borrowday")))+1).ToString() + "天";         //档案借出时限
                label16.Text= ((int.Parse(check.logincheckset("warningday")))+1).ToString() + "天";           //发卡器射频
            }
            catch
            {
                MessageBox.Show("查询失败！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

    }
}
