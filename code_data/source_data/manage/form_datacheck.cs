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
    public partial class form_datacheck : Form
    {
        public DataSet mysqlf9;
        public DataSet mysqlselt = new DataSet();
        bool size;
        public string type;
        public int days;
        public firtdoor f1;
        public form_datacheck(firtdoor f11,DataSet mysql5,bool size1,string typestr,int days1)
        {
            InitializeComponent();
            mysqlf9 = mysql5;
            size = size1;
            type = typestr;
            days = days1;
            f1 = f11;
        }

        private void Form9_Load(object sender, EventArgs e)
        {
            mysqlselt.Tables.Add();
            for (int i = 0; i < mysqlf9.Tables[0].Columns.Count;i++)
            {
                mysqlselt.Tables[0].Columns.Add();
                mysqlselt.Tables[0].Columns[i].ColumnName = mysqlf9.Tables[0].Columns[i].ColumnName;
            }
            
            try
            {
                dataGridView1.DataSource = mysqlf9.Tables[0];
                if (type != "gate_in" && type != "gate_out" && type != "gate_warning")
                {
                    if (size == true)
                    {
                        dataGridView1.Columns[0].Width = 180;
                        dataGridView1.Columns[1].Width = 100;
                        dataGridView1.Columns[2].Width = 100;
                        dataGridView1.Columns[4].Width = 80;
                    }
                    if (size == false)
                    {
                        dataGridView1.Columns[0].Width = 50;
                        dataGridView1.Columns[1].Width = 200;
                    }
                    for (int i = 0; i < mysqlf9.Tables[0].Rows.Count; i++)
                    {
                        if (mysqlf9.Tables[0].Rows[i][2].ToString() == "1")
                        {
                            mysqlf9.Tables[0].Rows[i][2] = "在 库";
                        }
                        if (mysqlf9.Tables[0].Rows[i][2].ToString() == "0")
                        {
                            mysqlf9.Tables[0].Rows[i][2] = "借 出";
                        }
                        if (mysqlf9.Tables[0].Rows[i][2].ToString() == "new")
                        {
                            mysqlf9.Tables[0].Rows[i][2] = "新 增";
                        }
                        if (mysqlf9.Tables[0].Rows[i][2].ToString() == "borrow")
                        {
                            mysqlf9.Tables[0].Rows[i][2] = "借 阅";
                        }
                        if (mysqlf9.Tables[0].Rows[i][2].ToString() == "return")
                        {
                            mysqlf9.Tables[0].Rows[i][2] = "归 还";
                        }
                        if (type!="transfer")
                        {
                            var mm = mysqlf9.Tables[0].Rows[i][6].ToString().Split(new char[1] { ' ' });
                            if ((comboBox1.Items.IndexOf(mm[0])) >= 0)
                            { continue; }
                            comboBox1.Items.Add(mm[0]);
                            if ((comboBox2.Items.IndexOf(mm[0])) >= 0)
                            { continue; }
                            comboBox2.Items.Add(mm[0]);
                        }
                        else
                        {
                            var mm = mysqlf9.Tables[0].Rows[i][7].ToString().Split(new char[1] { ' ' });
                            if ((comboBox1.Items.IndexOf(mm[0])) >= 0)
                            { continue; }
                            comboBox1.Items.Add(mm[0]);
                            if ((comboBox2.Items.IndexOf(mm[0])) >= 0)
                            { continue; }
                            comboBox2.Items.Add(mm[0]);
                        }
                    }
                    for (int i = 0; i < mysqlf9.Tables[0].Rows.Count; i++)
                    {
                        if (mysqlf9.Tables[0].Rows[i][2].ToString() == "新 增")
                        {
                            mysqlf9.Tables[0].Rows.RemoveAt(i);
                            i--;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < mysqlf9.Tables[0].Rows.Count; i++)
                    {
                        if (mysqlf9.Tables[0].Rows[i][1].ToString() == "1")
                        {
                            mysqlf9.Tables[0].Rows[i][1] = "在 库";
                        }
                        else
                        {
                            mysqlf9.Tables[0].Rows[i][1] = "不在库";
                        }
                        if (mysqlf9.Tables[0].Rows[i][2].ToString() == "goout")
                        {
                            mysqlf9.Tables[0].Rows[i][2] = "出 门";
                        }
                        else
                        {
                            mysqlf9.Tables[0].Rows[i][2] = "进 入";
                        }
                        var mm = mysqlf9.Tables[0].Rows[i][3].ToString().Split(new char[1] { ' ' });
                        if ((comboBox1.Items.IndexOf(mm[0])) >= 0)
                        { continue; }
                        comboBox1.Items.Add(mm[0]);
                        if ((comboBox2.Items.IndexOf(mm[0])) >= 0)
                        { continue; }
                        comboBox2.Items.Add(mm[0]);
                    }
                }
                
                switch (type)
                {
                    case "handlestatus":
                        label1.Enabled = false;
                        comboBox1.Enabled = false;
                        label2.Enabled = false;
                        comboBox2.Enabled = false;
                        comboBox3.Items.Clear();
                        label3.Enabled = false;
                        comboBox3.Enabled = false;
                        comboBox4.Items.Clear();
                        label4.Enabled = false;
                        comboBox4.Enabled = false;
                        comboBox1.SelectedIndex = 0;
                        comboBox2.SelectedIndex = comboBox2.Items.Count - 1;
                        comboBox3.SelectedIndex = 3;
                        comboBox4.SelectedIndex = 2;
                        break;
                    case "note19":
                        comboBox3.Items.Clear();
                        comboBox3.Items.Add("借阅");
                        comboBox3.Items.Add("归还");
                        comboBox3.Items.Add("全部");
                        comboBox1.SelectedIndex = 0;
                        comboBox2.SelectedIndex = comboBox2.Items.Count - 1;
                        comboBox3.SelectedIndex = 2;
                        comboBox4.SelectedIndex = 2;
                        break;
                    case "nowstore":
                        comboBox3.Items.Clear();
                        comboBox3.Items.Add("在库");
                        comboBox3.Items.Add("借出");
                        comboBox3.Items.Add("全部");
                        comboBox3.SelectedIndex = 2;
                        label4.Enabled = false;
                        comboBox4.Items.Clear();
                        comboBox4.Enabled = false;
                        comboBox1.SelectedIndex = 0;
                        comboBox2.SelectedIndex = comboBox2.Items.Count - 1;
                        comboBox3.SelectedIndex = 2;
                        comboBox4.SelectedIndex = 2;
                        break;
                    case "borrow":
                        comboBox3.Items.Clear();
                        label3.Enabled = false;
                        comboBox3.Enabled = false;
                        comboBox4.Items.Clear();
                        label4.Enabled = false;
                        comboBox4.Enabled = false;
                        dataGridView1.DataSource = null;
                        comboBox1.SelectedIndex = 0;
                        comboBox2.SelectedIndex = comboBox2.Items.Count - 1;
                        comboBox3.SelectedIndex = 3;
                        comboBox4.SelectedIndex = 2;
                        break;
                    case "should_return":
                        comboBox1.Items.Clear();
                        comboBox1.Enabled = false;
                        label1.Enabled = false;
                        comboBox2.Items.Clear();
                        comboBox2.Enabled = false;
                        label2.Enabled = false;
                        comboBox3.Items.Clear();
                        label3.Enabled = false;
                        comboBox3.Enabled = false;
                        comboBox4.Items.Clear();
                        label4.Enabled = false;
                        comboBox4.Enabled = false;
                        groupBox1.Text = "查 询";
                        dataGridView1.DataSource = null;
                        comboBox1.SelectedIndex = 0;
                        comboBox2.SelectedIndex = comboBox2.Items.Count - 1;
                        comboBox3.SelectedIndex = 3;
                        comboBox4.SelectedIndex = 2;
                        break;
                    case "new":
                        comboBox3.Items.Clear();
                        comboBox3.Items.Add("在库");
                        comboBox3.Items.Add("借出");
                        comboBox3.Items.Add("全部");
                        comboBox4.Items.Clear();
                        label4.Enabled = false;
                        comboBox4.Enabled = false;
                        comboBox1.SelectedIndex = 0;
                        comboBox2.SelectedIndex = comboBox2.Items.Count - 1;
                        comboBox3.SelectedIndex = 2;
                        comboBox4.SelectedIndex = 2;
                        break;
                    case "gate_in":
                        comboBox3.Items.Clear();
                        comboBox3.Items.Add("进入库房");
                        comboBox3.Enabled=false;
                        comboBox4.Items.Clear();
                        comboBox4.Items.Add("在库");
                        comboBox4.Items.Add("不在库");
                        comboBox4.Items.Add("全部");
                        dataGridView1.DataSource = null;
                        comboBox1.SelectedIndex = 0;
                        comboBox2.SelectedIndex = comboBox2.Items.Count - 1;
                        comboBox3.SelectedIndex = 0;
                        comboBox4.SelectedIndex = 2;                        
                        break;
                    case "gate_out":
                        comboBox3.Items.Clear();
                        comboBox3.Items.Add("出库房");
                        comboBox3.Enabled = false;
                        comboBox4.Items.Clear();
                        comboBox4.Items.Add("在库");
                        comboBox4.Items.Add("不在库");
                        comboBox4.Items.Add("全部");
                        dataGridView1.DataSource = null;
                        comboBox1.SelectedIndex = 0;
                        comboBox2.SelectedIndex = comboBox2.Items.Count - 1;
                        comboBox3.SelectedIndex = 0;
                        comboBox4.SelectedIndex = 2;
                        break;
                    case "gate_warning":
                        comboBox3.Items.Clear();
                        comboBox3.Items.Add("入库异常");
                        comboBox3.Items.Add("出库异常");
                        comboBox3.Items.Add("全部");
                        comboBox4.Items.Clear();
                        comboBox4.Items.Add("在库");
                        comboBox4.Enabled = false;
                        dataGridView1.DataSource = null;
                        comboBox1.SelectedIndex = 0;
                        comboBox2.SelectedIndex = comboBox2.Items.Count - 1;
                        comboBox3.SelectedIndex = 2;
                        comboBox4.SelectedIndex = 0;
                        break;
                    case "transfer":
                        comboBox3.Enabled=false;
                        comboBox4.Enabled = false;
                        dataGridView1.DataSource = null;
                        comboBox1.SelectedIndex = 0;
                        comboBox2.SelectedIndex = comboBox2.Items.Count - 1;
                        break;
                }

                
            }
            catch
            {

            }
        }

        /// <summary>
        /// 按条件查询函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        public bool startime = false;
        private void button1_Click(object sender, EventArgs e)
        {
            int k = 0;
            mysqlselt.Tables[0].Rows.Clear();
            switch (type)
            {
                case "handlestatus":
                    #region  出入库操作状态查询
                    for (int i = 0; i < mysqlf9.Tables[0].Rows.Count; i++)
                    {
                        //终止时间判断
                        if ((mysqlf9.Tables[0].Rows[i][6].ToString().IndexOf(comboBox2.Text.ToString())) >= 0)
                        {
                            startime = false;
                            if (mysqlf9.Tables[0].Rows[i][2].ToString() == "在 库")
                            {
                                continue;
                            }
                            mysqlselt.Tables[0].Rows.Add();
                            for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                            {
                                mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                            }
                            k++;
                            if (i + 1 == mysqlf9.Tables[0].Rows.Count)
                            {
                                continue;
                            }
                            if ((mysqlf9.Tables[0].Rows[i + 1][6].ToString().IndexOf(comboBox2.Text.ToString())) < 0)
                            {
                                i = mysqlf9.Tables[0].Rows.Count;
                            }
                            continue;
                        }

                        //起始时间判断
                        if ((mysqlf9.Tables[0].Rows[i][6].ToString().IndexOf(comboBox1.Text.ToString())) >= 0 || startime == true)
                        {
                            startime = true;
                            if (mysqlf9.Tables[0].Rows[i][2].ToString() == "1")
                            {
                                continue;
                            }
                            mysqlselt.Tables[0].Rows.Add();
                            for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                            {
                                mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                            }
                            k++;
                        }
                    }
                    #endregion
                    break;
                case "note19":
                    #region  历史记录查询
                    for (int i = 0; i < mysqlf9.Tables[0].Rows.Count; i++)
                    {
                        //if (mysqlf9.Tables[0].Rows[i][2].ToString() == "新 增")
                        //{
                        //    continue;
                        //}
                        //终止时间判断
                        if ((mysqlf9.Tables[0].Rows[i][6].ToString().IndexOf(comboBox2.Text.ToString())) >= 0)
                        {
                            startime = false;
                            //全部
                            if (comboBox3.SelectedIndex == 2)
                            {
                                //全部
                                if (comboBox4.SelectedIndex == 2)
                                {
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                                //成功
                                if (comboBox4.SelectedIndex == 0)
                                {
                                    if (mysqlf9.Tables[0].Rows[i][2].ToString() == "0")
                                    {
                                        continue;
                                    }
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                                //失败
                                if (comboBox4.SelectedIndex == 1)
                                {
                                    if (mysqlf9.Tables[0].Rows[i][2].ToString() != "0")
                                    {
                                        continue;
                                    }
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                            }
                            //新增入库
                            if (comboBox3.SelectedIndex == 3)
                            {
                                if (mysqlf9.Tables[0].Rows[i][2].ToString() == "归 还" || mysqlf9.Tables[0].Rows[i][2].ToString() == "借 阅")
                                {
                                    continue;
                                }
                                //全部
                                if (comboBox4.SelectedIndex == 2)
                                {
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                                //成功
                                if (comboBox4.SelectedIndex == 0)
                                {
                                    if (mysqlf9.Tables[0].Rows[i][2].ToString() == "0")
                                    {
                                        continue;
                                    }
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                                //失败
                                if (comboBox4.SelectedIndex == 1)
                                {
                                    if (mysqlf9.Tables[0].Rows[i][2].ToString() != "0")
                                    {
                                        continue;
                                    }
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                            }
                            //借阅出库
                            if (comboBox3.SelectedIndex == 0)
                            {
                                if (mysqlf9.Tables[0].Rows[i][2].ToString() == "归 还" || mysqlf9.Tables[0].Rows[i][2].ToString() == "新 增")
                                {
                                    continue;
                                }
                                //全部
                                if (comboBox4.SelectedIndex == 2)
                                {
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                                //成功
                                if (comboBox4.SelectedIndex == 0)
                                {
                                    if (mysqlf9.Tables[0].Rows[i][2].ToString() == "0")
                                    {
                                        continue;
                                    }
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                                //失败
                                if (comboBox4.SelectedIndex == 1)
                                {
                                    if (mysqlf9.Tables[0].Rows[i][2].ToString() != "0")
                                    {
                                        continue;
                                    }
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                            }
                            //归还入库
                            if (comboBox3.SelectedIndex == 1)
                            {
                                if (mysqlf9.Tables[0].Rows[i][2].ToString() == "新 增" || mysqlf9.Tables[0].Rows[i][2].ToString() == "借 阅")
                                {
                                    continue;
                                }
                                //全部
                                if (comboBox4.SelectedIndex == 2)
                                {
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                                //成功
                                if (comboBox4.SelectedIndex == 0)
                                {
                                    if (mysqlf9.Tables[0].Rows[i][2].ToString() == "0")
                                    {
                                        continue;
                                    }
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                                //失败
                                if (comboBox4.SelectedIndex == 1)
                                {
                                    if (mysqlf9.Tables[0].Rows[i][2].ToString() != "0")
                                    {
                                        continue;
                                    }
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                            }
                            if (i + 1 == mysqlf9.Tables[0].Rows.Count)
                            {
                                continue;
                            }
                            if ((mysqlf9.Tables[0].Rows[i + 1][6].ToString().IndexOf(comboBox2.Text.ToString())) < 0)
                            {
                                i = mysqlf9.Tables[0].Rows.Count;
                            }
                            continue;
                        }

                        //起始时间判断
                        if ((mysqlf9.Tables[0].Rows[i][6].ToString().IndexOf(comboBox1.Text.ToString())) >= 0 || startime == true)
                        {
                            startime = true;
                            //全部
                            if (comboBox3.SelectedIndex == 2)
                            {
                                //全部
                                if (comboBox4.SelectedIndex == 2)
                                {
                                   
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                                //成功
                                if (comboBox4.SelectedIndex == 0)
                                {
                                    if (mysqlf9.Tables[0].Rows[i][2].ToString() == "0")
                                    {
                                        continue;
                                    }
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                                //失败
                                if (comboBox4.SelectedIndex == 1)
                                {
                                    if (mysqlf9.Tables[0].Rows[i][2].ToString() != "0")
                                    {
                                        continue;
                                    }
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                            }
                            //新增入库
                            if (comboBox3.SelectedIndex == 3)
                            {
                                if (mysqlf9.Tables[0].Rows[i][2].ToString() == "归 还" || mysqlf9.Tables[0].Rows[i][2].ToString() == "借 阅")
                                {
                                    continue;
                                }
                                //全部
                                if (comboBox4.SelectedIndex == 2)
                                {
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                                //成功
                                if (comboBox4.SelectedIndex == 0)
                                {
                                    if (mysqlf9.Tables[0].Rows[i][2].ToString() == "0")
                                    {
                                        continue;
                                    }
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                                //失败
                                if (comboBox4.SelectedIndex == 1)
                                {
                                    if (mysqlf9.Tables[0].Rows[i][2].ToString() != "0")
                                    {
                                        continue;
                                    }
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                            }
                            //借阅出库
                            if (comboBox3.SelectedIndex == 0)
                            {
                                if (mysqlf9.Tables[0].Rows[i][2].ToString() == "归 还" || mysqlf9.Tables[0].Rows[i][2].ToString() == "新 增")
                                {
                                    continue;
                                }
                                //全部
                                if (comboBox4.SelectedIndex == 2)
                                {
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                                //成功
                                if (comboBox4.SelectedIndex == 0)
                                {
                                    if (mysqlf9.Tables[0].Rows[i][2].ToString() == "0")
                                    {
                                        continue;
                                    }
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                                //失败
                                if (comboBox4.SelectedIndex == 1)
                                {
                                    if (mysqlf9.Tables[0].Rows[i][2].ToString() != "0")
                                    {
                                        continue;
                                    }
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                            }
                            //归还入库
                            if (comboBox3.SelectedIndex == 1)
                            {
                                if (mysqlf9.Tables[0].Rows[i][2].ToString() == "新 增" || mysqlf9.Tables[0].Rows[i][2].ToString() == "借 阅")
                                {
                                    continue;
                                }
                                //全部
                                if (comboBox4.SelectedIndex == 2)
                                {
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                                //成功
                                if (comboBox4.SelectedIndex == 0)
                                {
                                    if (mysqlf9.Tables[0].Rows[i][2].ToString() == "0")
                                    {
                                        continue;
                                    }
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                                //失败
                                if (comboBox4.SelectedIndex == 1)
                                {
                                    if (mysqlf9.Tables[0].Rows[i][2].ToString() != "0")
                                    {
                                        continue;
                                    }
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                            }
                        }
                    }
                    #endregion
                    break;
                case "nowstore":
                    #region  现存档案总数查询
                    for (int i = 0; i < mysqlf9.Tables[0].Rows.Count; i++)
                    {
                        if (mysqlf9.Tables[0].Rows[i][2].ToString() == "在 库")
                        {
                            //终止时间判断
                            if ((mysqlf9.Tables[0].Rows[i][6].ToString().IndexOf(comboBox2.Text.ToString())) >= 0)
                            {
                                startime = false;
                                //全部
                                if (comboBox3.SelectedIndex == 2)
                                {
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                                //在库
                                if (comboBox3.SelectedIndex == 0)
                                {
                                    if (mysqlf9.Tables[0].Rows[i][2].ToString() == "借 出")
                                    {
                                        continue;
                                    }
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }

                                //借出
                                if (comboBox3.SelectedIndex == 1)
                                {
                                    if (mysqlf9.Tables[0].Rows[i][2].ToString() == "在 库")
                                    {
                                        continue;
                                    }
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                                if (i + 1 == mysqlf9.Tables[0].Rows.Count)
                                {
                                    continue;
                                }
                                if ((mysqlf9.Tables[0].Rows[i + 1][6].ToString().IndexOf(comboBox2.Text.ToString())) < 0)
                                {
                                    i = mysqlf9.Tables[0].Rows.Count;
                                }
                                continue;
                            }

                            //起始时间判断
                            if ((mysqlf9.Tables[0].Rows[i][6].ToString().IndexOf(comboBox1.Text.ToString())) >= 0 || startime == true)
                            {
                                startime = true;
                                //全部
                                if (comboBox3.SelectedIndex == 2)
                                {
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }

                                //在库
                                if (comboBox3.SelectedIndex == 0)
                                {
                                    if (mysqlf9.Tables[0].Rows[i][2].ToString() == "借 阅")
                                    {
                                        continue;
                                    }
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }

                                //借出
                                if (comboBox3.SelectedIndex == 1)
                                {
                                    if (mysqlf9.Tables[0].Rows[i][2].ToString() == "在 库")
                                    {
                                        continue;
                                    }
                                    mysqlselt.Tables[0].Rows.Add();
                                    for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                    {
                                        mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                    }
                                    k++;
                                }
                            }
                        }
                    }
                    #endregion
                    break;
                case "borrow":
                    #region  取走档案档案盒数量查询
                    for (int i = 0; i < mysqlf9.Tables[0].Rows.Count; i++)
                    {
                        //终止时间判断
                        if ((mysqlf9.Tables[0].Rows[i][6].ToString().IndexOf(comboBox2.Text.ToString())) >= 0)
                        {
                            startime = false;
                            if (mysqlf9.Tables[0].Rows[i][2].ToString() == "在 库")
                            {
                                continue;
                            }
                            mysqlselt.Tables[0].Rows.Add();
                            for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                            {
                                mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                            }
                            k++;
                            if (i + 1 == mysqlf9.Tables[0].Rows.Count)
                            {
                                continue;
                            }
                            if ((mysqlf9.Tables[0].Rows[i + 1][6].ToString().IndexOf(comboBox2.Text.ToString())) < 0)
                            {
                                i = mysqlf9.Tables[0].Rows.Count;
                            }
                            continue;
                        }

                        //起始时间判断
                        if ((mysqlf9.Tables[0].Rows[i][6].ToString().IndexOf(comboBox1.Text.ToString())) >= 0 || startime == true)
                        {
                            startime = true;
                            if (mysqlf9.Tables[0].Rows[i][2].ToString() == "在 库")
                            {
                                continue;
                            }
                            mysqlselt.Tables[0].Rows.Add();
                            for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                            {
                                mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                            }
                            k++;
                        }                       
                    }
                    #endregion
                    break; 
                case "should_return":
                    #region  应还档案档案盒数量查询
                    timestoend timestoend = new timestoend();                   
                    for (int i = 0; i < mysqlf9.Tables[0].Rows.Count; i++)
                    {
                        if (mysqlf9.Tables[0].Rows[i][2].ToString() == "在 库")
                        {
                            continue;
                        }
                        if (f1.borrowwaring.Contains(mysqlf9.Tables[0].Rows[i][1].ToString()))
                        {
                            mysqlselt.Tables[0].Rows.Add();
                            for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                            {
                                mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                            }
                            mysqlselt.Tables[0].Rows[k][6] = f1.borrowwaring_time[f1.borrowwaring.IndexOf(mysqlf9.Tables[0].Rows[i][1].ToString())];
                            k++;
                        }
                    }
                    #endregion
                    break;
                case "new":
                    #region  现存档案总数查询
                    for (int i = 0; i < mysqlf9.Tables[0].Rows.Count; i++)
                    {
                        //终止时间判断
                        if ((mysqlf9.Tables[0].Rows[i][6].ToString().IndexOf(comboBox2.Text.ToString())) >= 0)
                        {
                            startime = false;
                            //全部
                            if (comboBox3.SelectedIndex == 2)
                            {
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }
                            //在库
                            if (comboBox3.SelectedIndex == 0)
                            {
                                if (mysqlf9.Tables[0].Rows[i][2].ToString() == "借 出")
                                {
                                    continue;
                                }
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }

                            //借出
                            if (comboBox3.SelectedIndex == 1)
                            {
                                if (mysqlf9.Tables[0].Rows[i][2].ToString() == "在 库")
                                {
                                    continue;
                                }
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }
                            if (i + 1 == mysqlf9.Tables[0].Rows.Count)
                            {
                                continue;
                            }
                            if ((mysqlf9.Tables[0].Rows[i + 1][6].ToString().IndexOf(comboBox2.Text.ToString())) < 0)
                            {
                                i = mysqlf9.Tables[0].Rows.Count;
                            }
                            continue;
                        }

                        //起始时间判断
                        if ((mysqlf9.Tables[0].Rows[i][6].ToString().IndexOf(comboBox1.Text.ToString())) >= 0 || startime == true)
                        {
                            startime = true;
                            //全部
                            if (comboBox3.SelectedIndex == 2)
                            {
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }

                            //在库
                            if (comboBox3.SelectedIndex == 0)
                            {
                                if (mysqlf9.Tables[0].Rows[i][2].ToString() == "借 出")
                                {
                                    continue;
                                }
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }

                            //借出
                            if (comboBox3.SelectedIndex == 1)
                            {
                                if (mysqlf9.Tables[0].Rows[i][2].ToString() == "在 库")
                                {
                                    continue;
                                }
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }
                        }
                    }
                    #endregion
                    break;
                case "gate_in":
                    #region  进入通道门数量查询
                    for (int i = 0; i < mysqlf9.Tables[0].Rows.Count; i++)
                    {
                        //终止时间判断
                        if ((mysqlf9.Tables[0].Rows[i][3].ToString().IndexOf(comboBox2.Text.ToString())) >= 0)
                        {
                            startime = false;
                            if (mysqlf9.Tables[0].Rows[i][2].ToString() == "出 门")
                            {
                                continue;
                            }
                            //全部
                            if (comboBox4.SelectedIndex == 2)
                            {
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }
                            //在库
                            if (comboBox4.SelectedIndex == 0)
                            {
                                if (mysqlf9.Tables[0].Rows[i][1].ToString() == "不在库")
                                {
                                    continue;
                                }
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }
                            //不在库
                            if (comboBox4.SelectedIndex == 1)
                            {
                                if (mysqlf9.Tables[0].Rows[i][1].ToString() == "在 库")
                                {
                                    continue;
                                }
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }
                            if (i + 1 == mysqlf9.Tables[0].Rows.Count)
                            {
                                continue;
                            }
                            if ((mysqlf9.Tables[0].Rows[i + 1][3].ToString().IndexOf(comboBox2.Text.ToString())) < 0)
                            {
                                i = mysqlf9.Tables[0].Rows.Count;
                            }
                            continue;
                        }

                        //起始时间判断
                        if ((mysqlf9.Tables[0].Rows[i][3].ToString().IndexOf(comboBox1.Text.ToString())) >= 0 || startime == true)
                        {
                            startime = true;
                            if (mysqlf9.Tables[0].Rows[i][2].ToString() == "出 门")
                            {
                                continue;
                            }
                            //全部
                            if (comboBox4.SelectedIndex == 2)
                            {
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }
                            //在库
                            if (comboBox4.SelectedIndex == 0)
                            {
                                if (mysqlf9.Tables[0].Rows[i][1].ToString() == "不在库")
                                {
                                    continue;
                                }
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }
                            //不在库
                            if (comboBox4.SelectedIndex == 1)
                            {
                                if (mysqlf9.Tables[0].Rows[i][1].ToString() == "在 库")
                                {
                                    continue;
                                }
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }
                        }
                    }
                    #endregion
                    break;
                case "gate_out":
                    #region  出通道门数量查询
                    for (int i = 0; i < mysqlf9.Tables[0].Rows.Count; i++)
                    {
                        //终止时间判断
                        if ((mysqlf9.Tables[0].Rows[i][3].ToString().IndexOf(comboBox2.Text.ToString())) >= 0)
                        {
                            startime = false;
                            if (mysqlf9.Tables[0].Rows[i][2].ToString() == "进 入")
                            {
                                continue;
                            }
                            //全部
                            if (comboBox4.SelectedIndex == 2)
                            {
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }
                            //在库
                            if (comboBox4.SelectedIndex == 0)
                            {
                                if (mysqlf9.Tables[0].Rows[i][1].ToString() == "不在库")
                                {
                                    continue;
                                }
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }
                            //不在库
                            if (comboBox4.SelectedIndex == 1)
                            {
                                if (mysqlf9.Tables[0].Rows[i][1].ToString() == "在 库")
                                {
                                    continue;
                                }
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }
                            if (i + 1 == mysqlf9.Tables[0].Rows.Count)
                            {
                                continue;
                            }
                            if ((mysqlf9.Tables[0].Rows[i + 1][3].ToString().IndexOf(comboBox2.Text.ToString())) < 0)
                            {
                                i = mysqlf9.Tables[0].Rows.Count;
                            }
                            continue;
                        }

                        //起始时间判断
                        if ((mysqlf9.Tables[0].Rows[i][3].ToString().IndexOf(comboBox1.Text.ToString())) >= 0 || startime == true)
                        {
                            startime = true;
                            if (mysqlf9.Tables[0].Rows[i][2].ToString() == "进 入")
                            {
                                continue;
                            }
                            //全部
                            if (comboBox4.SelectedIndex == 2)
                            {
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }
                            //在库
                            if (comboBox4.SelectedIndex == 0)
                            {
                                if (mysqlf9.Tables[0].Rows[i][1].ToString() == "不在库")
                                {
                                    continue;
                                }
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }
                            //不在库
                            if (comboBox4.SelectedIndex == 1)
                            {
                                if (mysqlf9.Tables[0].Rows[i][1].ToString() == "在 库")
                                {
                                    continue;
                                }
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }
                        }
                    }
                    #endregion
                    break;
                case "gate_warning":
                    #region  出入通道门异常数量查询
                    for (int i = 0; i < mysqlf9.Tables[0].Rows.Count; i++)
                    {
                        //终止时间判断
                        if ((mysqlf9.Tables[0].Rows[i][3].ToString().IndexOf(comboBox2.Text.ToString())) >= 0)
                        {
                            startime = false;
                            if (mysqlf9.Tables[0].Rows[i][1].ToString() == "不在库")
                            {
                                continue;
                            }
                            //全部
                            if (comboBox3.SelectedIndex == 2)
                            {
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }
                            //入库异常
                            if (comboBox3.SelectedIndex == 0)
                            {
                                if (mysqlf9.Tables[0].Rows[i][2].ToString() == "出 门")
                                {
                                    continue;
                                }
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }
                            //出库异常
                            if (comboBox3.SelectedIndex == 1)
                            {
                                if (mysqlf9.Tables[0].Rows[i][2].ToString() == "进 入")
                                {
                                    continue;
                                }
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }
                            if (i + 1 == mysqlf9.Tables[0].Rows.Count)
                            {
                                continue;
                            }
                            if ((mysqlf9.Tables[0].Rows[i + 1][3].ToString().IndexOf(comboBox2.Text.ToString())) < 0)
                            {
                                i = mysqlf9.Tables[0].Rows.Count;
                            }
                            continue;
                        }

                        //起始时间判断
                        if ((mysqlf9.Tables[0].Rows[i][3].ToString().IndexOf(comboBox1.Text.ToString())) >= 0 || startime == true)
                        {
                            startime = true;
                            if (mysqlf9.Tables[0].Rows[i][1].ToString() == "不在库")
                            {
                                continue;
                            }
                            //全部
                            if (comboBox3.SelectedIndex == 2)
                            {
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }
                            //入库异常
                            if (comboBox3.SelectedIndex == 0)
                            {
                                if (mysqlf9.Tables[0].Rows[i][2].ToString() == "出 门")
                                {
                                    continue;
                                }
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }
                            //出库异常
                            if (comboBox3.SelectedIndex == 1)
                            {
                                if (mysqlf9.Tables[0].Rows[i][2].ToString() == "进 入")
                                {
                                    continue;
                                }
                                mysqlselt.Tables[0].Rows.Add();
                                for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                                {
                                    mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                                }
                                k++;
                            }
                        }
                    }
                    #endregion
                    break;
                case "transfer":
                    #region  取走档案档案盒数量查询
                    for (int i = 0; i < mysqlf9.Tables[0].Rows.Count; i++)
                    {
                        //终止时间判断
                        if ((mysqlf9.Tables[0].Rows[i][7].ToString().IndexOf(comboBox2.Text.ToString())) >= 0)
                        {
                            startime = false;
                            mysqlselt.Tables[0].Rows.Add();
                            for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                            {
                                mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                            }
                            k++;
                            if (i + 1 == mysqlf9.Tables[0].Rows.Count)
                            {
                                continue;
                            }
                            if ((mysqlf9.Tables[0].Rows[i + 1][7].ToString().IndexOf(comboBox2.Text.ToString())) < 0)
                            {
                                i = mysqlf9.Tables[0].Rows.Count;
                            }
                            continue;
                        }

                        //起始时间判断
                        if ((mysqlf9.Tables[0].Rows[i][7].ToString().IndexOf(comboBox1.Text.ToString())) >= 0 || startime == true)
                        {
                            startime = true;
                            mysqlselt.Tables[0].Rows.Add();
                            for (int j = 0; j < mysqlf9.Tables[0].Columns.Count; j++)
                            {
                                mysqlselt.Tables[0].Rows[k][j] = mysqlf9.Tables[0].Rows[i][j];
                            }
                            k++;
                        }
                    }
                    #endregion
                    break;

            }
            dataGridView1.DataSource = mysqlselt.Tables[0];
        }
    }
}
