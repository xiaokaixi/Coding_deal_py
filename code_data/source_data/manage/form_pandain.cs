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
    public partial class form_pandain : Form
    {
        public DataSet mysqlf10, mysqlf11,mysql,mysql2;
        public form_pandain(DataSet mysql3, DataSet mysql31)
        {
            InitializeComponent();
            mysqlf10 = mysql3;            
            mysql = mysql31;                       
        }

        private void Form10_Load(object sender, EventArgs e)
        {
            //数据表中文替换初始
            preinit();
            button1.Enabled = true;
            mysql2 = mysql;
            //盘点机数据位置查询,用于显示
            mysqlf10.Tables[0].Columns.Add();
            mysqlf10.Tables[0].Columns[0].ColumnName = "序号";
            mysqlf10.Tables[0].Columns[1].ColumnName = "RFID标签号";
            mysqlf10.Tables[0].Columns[2].ColumnName = "盘点时间";
            mysqlf10.Tables[0].Columns[3].ColumnName = "存放位置";
            //位置信息查询
            check_position(mysqlf10);
            //排序设置
            //mysqlf10.Tables[0].Columns[3].s;
            mysqlf11 = mysqlf10;
            dataGridView1.DataSource = mysqlf10.Tables[0];
            dataGridView1.Columns[0].Width = 100;
            dataGridView1.Columns[1].Width = 350;
        }

        #region   操作状态中文替换初始
        /// <summary>
        /// 数据表中文替换初始
        /// </summary>
        private void preinit()
        {

            for (int i = 0; i < mysql.Tables[0].Rows.Count; i++)
            {
                if (mysql.Tables[0].Rows[i][2].ToString()=="1")
                {
                    mysql.Tables[0].Rows[i][2] = "在 库";
                }
                if (mysql.Tables[0].Rows[i][2].ToString() == "0")
                {
                    mysql.Tables[0].Rows[i][2] = "借 出";
                }
            }
            mysql.Tables[0].Columns[0].ColumnName = "ID";
            mysql.Tables[0].Columns[1].ColumnName = "RFID编号";
            mysql.Tables[0].Columns[2].ColumnName = "在库状态";
            mysql.Tables[0].Columns[3].ColumnName = "姓名";
            mysql.Tables[0].Columns[4].ColumnName = "身份证号";
            mysql.Tables[0].Columns[5].ColumnName = "存放位置";
            mysql.Tables[0].Columns[6].ColumnName = "入库时间";
            mysql.Tables[0].Columns[7].ColumnName = "单位名称";
            mysql.Tables[0].Columns[8].ColumnName = "操作员编号";
        }
        #endregion

        #region  查询盘点数据位置
        private void check_position(DataSet mysql3)
        {
            for (int i = 0; i < mysql3.Tables[0].Rows.Count; i++)
            {
                for (int j = 0; j < mysql2.Tables[0].Rows.Count; j++)
                {
                    if (mysql3.Tables[0].Rows[i][1].ToString() == mysql2.Tables[0].Rows[j][1].ToString())
                    {
                        mysql3.Tables[0].Rows[i][3] = mysql2.Tables[0].Rows[j][5].ToString();  //找到位置信息
                        continue;
                    }
                }
            }
        }
        #endregion

        //清点
        private void button1_Click(object sender, EventArgs e)
        {
            int outnum = 0, innum = 0, min = 0 ;
            List<int> jihemysql2 = new List<int>();
            List<int> jihemysqlf11 = new List<int>();
            List<int> jihemysqlf12 = new List<int>();
            for (int i = 0; i < mysql.Tables[0].Rows.Count; i++)
            {
                if (mysql.Tables[0].Rows[i][2].ToString() == "借 出")
                {
                    jihemysql2.Add(i);
                    outnum++;
                    continue;
                }
                for (int j = 0; j < mysqlf10.Tables[0].Rows.Count; j++)
                {
                    if (mysqlf10.Tables[0].Rows[j][1].ToString() == mysql.Tables[0].Rows[i][1].ToString())
                    {
                        jihemysql2.Add(i);
                        jihemysqlf11.Add(j);
                        continue;
                    }
                }
                innum++;
            }

            for (int i = 0; i < jihemysql2.Count; i++)
            {
                mysql2.Tables[0].Rows.RemoveAt(jihemysql2[i]-i);
            }
            for (int i = 0; i < jihemysqlf11.Count-1; i++)
            {
                for (int j = 0; j < jihemysqlf11.Count-1-i; j++)
                {
                    if (jihemysqlf11[j] > jihemysqlf11[j+1])
                    {
                        min = jihemysqlf11[j+1];
                        jihemysqlf11[j + 1] = jihemysqlf11[j];
                        jihemysqlf11[j] = min;
                    }
                }
            }

            for (int i = 0; i < jihemysqlf11.Count; i++)
            {
                mysqlf11.Tables[0].Rows.RemoveAt(jihemysqlf11[i] - i);
            }
            dataGridView1.DataSource = mysqlf11.Tables[0];
            if (mysqlf11.Tables[0].Rows.Count != 0||(mysql2.Tables[0].Rows.Count)!=0)
            {
                textBox2.Text = "异  常";
            }
            else
            {
                textBox2.Text = "正  常";
            }
            textBox23.Text =innum.ToString();
            textBox1.Text =outnum.ToString();
            textBox3.Text = (mysqlf11.Tables[0].Rows.Count+ mysql2.Tables[0].Rows.Count).ToString();
            button1.Enabled = false;
            //button2.Enabled = false;
        }

        //
        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "数据库")
            {
                dataGridView1.DataSource = mysql.Tables[0];
            }
            else
            {
                dataGridView1.DataSource = mysqlf11.Tables[0];
            }
        }
    }
}
