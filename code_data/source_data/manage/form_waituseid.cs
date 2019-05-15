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
    public partial class form_waituseid : Form
    {
        public int controlsum;
        public DataSet mysql;
        public DataSet mysql2=new DataSet();
        public form_waituseid(int controlsum1,DataSet mysql1)
        {
            InitializeComponent();
            controlsum = controlsum1;
            mysql = mysql1;
        }

        /// <summary>
        /// 窗体引导程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void form_waituseid_Load(object sender, EventArgs e)
        {
            this.comboBox3.SelectedIndexChanged += new System.EventHandler(comboBox3_selectchanged);
            for (int i = 1; i < controlsum + 2; i++)
            {
                comboBox3.Items.Add(i.ToString());
            }
            mysql2.Tables.Add();
            mysql2.Tables[0].Columns.Add();
            mysql2.Tables[0].Columns[0].ColumnName = "柜门编号";
            comboBox3.SelectedIndex = 0;
        }

        /// <summary>
        /// comboBox3选择改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public int m;
        private void comboBox3_selectchanged(object sender, EventArgs e)
        {
            mysql2.Tables[0].Clear();
            m = 0;
            bool flag = false;
            int k = comboBox3.SelectedIndex + 1;
            for (int i = 1; i < 31; i++)
            {
                for (int j = 0; j < mysql.Tables[0].Rows.Count; j++)
                {
                    if (mysql.Tables[0].Rows[j][5].ToString() == k.ToString() + "-" + i.ToString())
                    {
                        //mysql.Tables[0].Rows.RemoveAt(j);
                        flag = true;
                        break;
                    }
                    flag = false;
                }
                if (flag == false)
                {
                    mysql2.Tables[0].Rows.Add();
                    mysql2.Tables[0].Rows[m][0] = k.ToString() + "-" + i.ToString();
                    m++;
                }
            }
            dataGridView1.DataSource = mysql2.Tables[0];
        }
    }
}
