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
    public partial class form_neworreturn : Form
    {
        public firtdoor f1;
        public int handletype;
        public form_neworreturn(firtdoor f, int handletype1)
        {
            InitializeComponent();
            f1 = f;
            handletype=handletype1;
    }
        
        /// <summary>
        /// 新增入库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button17_Click(object sender, EventArgs e)
        {
            Hide();
            form_caozuoyuanpasswd fczy = new form_caozuoyuanpasswd(f1, 0, handletype);
            fczy.Text = "新增入库操作员权限认证";
            this.Close();
            fczy.ShowDialog();
            if (handletype != 0)
            {
                handletype = 3;  //新增入库
            }
            else
            {
                handletype = fczy.handletype;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            handletype = 0;
            this.Close();
        }

        /// <summary>
        /// 归还
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
            form_caozuoyuanpasswd fczy = new form_caozuoyuanpasswd(f1, 0, handletype);
            fczy.Text = "归还入库操作员权限认证";
            this.Close();
            fczy.ShowDialog();
            if (handletype != 0)
            {
                handletype = 4;  //归还入库
            }
            else
            {
                handletype = fczy.handletype;
            }
        }
    }
}
