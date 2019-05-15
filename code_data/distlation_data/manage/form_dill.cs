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
    public partial class form_dill : Form
    {
        #region  
        firtdoor f1;
        DataSet mysql = new DataSet();
        DataSet mysql_v = new DataSet();
        DataSet mysql_v_1 = new DataSet();
        DataSet mysql_v_2 = new DataSet();
        DataSet mysql_dill = new DataSet();
        public int count = 1, count1 = 0;
        public string perid;  //
        List<string> manyname = new List<string>();
        //
        public string dirpath = Application.StartupPath + "\\FreeVK.exe";  //
        public dynamic file;
        public Process p1;
        public int diitype = 3;
        #endregion

        #region 
        public form_dill(firtdoor f11)
        {
            InitializeComponent();
            f1 = f11;
        }

        private void form_dill_Load(object sender, EventArgs e)
        {
            //
            perinit();
            //
            timenow();
        }
        #endregion

        #region    
        //
        public System.Timers.Timer tnow = new System.Timers.Timer();
        public bool flgtnow = false;
        public void timenow()
        {
            tnow.Interval = 1;       //1s
            tnow.Elapsed += new System.Timers.ElapsedEventHandler(theoutime);
            tnow.AutoReset = true;  //true,false
            tnow.Enabled = true;
        }
        public void theoutime(object source, System.Timers.ElapsedEventArgs e)
        {
            if (flgtnow)
            { return; }
            flgtnow = true;
            SetData1();
            flgtnow = false;
        }

        //,
        private delegate void SetDataDelegate1();
        public string username;
        private void SetData1()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new SetDataDelegate1(SetData1));
                }
                else
                {
                    //
                    textBox12.Text = DateTime.Now.ToString();
                }
            }
            catch
            { }
        }
        #endregion

        #region  
        private void perinit()
        {
            //mysql
            mysql = f1.load_mysql("store", "tablename", "nowstore");
            load_mysql(mysql);  
            //tid
            mysql_v.Tables.Add();
            mysql_v.Tables[0].Columns.Add();
            mysql_v.Tables[0].Columns[0].ColumnName = "ID";
            mysql_v.Tables[0].Columns.Add();
            mysql_v.Tables[0].Columns[1].ColumnName = "姓名";
            mysql_v.Tables[0].Columns.Add();
            mysql_v.Tables[0].Columns[2].ColumnName = "单位名称";
            mysql_v.Tables[0].Columns.Add();
            mysql_v.Tables[0].Columns[3].ColumnName = "档案盒数";
            mysql_v.Tables[0].Columns.Add();
            mysql_v.Tables[0].Columns[4].ColumnName = "在库盒数";
            mysql_v.Tables[0].Columns.Add();
            mysql_v.Tables[0].Columns[5].ColumnName = "存放位置";
            dataGridView1.DataSource = mysql_v.Tables[0];
            dataGridView1.Columns[0].Width = 60;
            dataGridView1.Columns[1].Width = 120;
            dataGridView1.Columns[2].Width = 300;
            //
            mysql_v_1.Tables.Add();
            mysql_v_1.Tables[0].Columns.Add();
            mysql_v_1.Tables[0].Columns[0].ColumnName = "ID";
            mysql_v_1.Tables[0].Columns.Add();
            mysql_v_1.Tables[0].Columns[1].ColumnName = "name";
            mysql_v_1.Tables[0].Columns.Add();
            mysql_v_1.Tables[0].Columns[2].ColumnName = "unitname";
            mysql_v_1.Tables[0].Columns.Add();
            mysql_v_1.Tables[0].Columns[3].ColumnName = "idcard";
            mysql_v_1.Tables[0].Columns.Add();
            mysql_v_1.Tables[0].Columns[4].ColumnName = "transfertype";
            mysql_v_1.Tables[0].Columns.Add();
            mysql_v_1.Tables[0].Columns[5].ColumnName = "transferreason";
            mysql_v_1.Tables[0].Columns.Add();
            mysql_v_1.Tables[0].Columns[6].ColumnName = "tounitname";
            mysql_v_1.Tables[0].Columns.Add();
            mysql_v_1.Tables[0].Columns[7].ColumnName = "operate_time";
            mysql_v_1.Tables[0].Columns.Add();
            mysql_v_1.Tables[0].Columns[8].ColumnName = "operate_id";

            mysql_v_2.Tables.Add();
            mysql_v_2.Tables[0].Columns.Add();
            mysql_v_2.Tables[0].Columns[0].ColumnName = "ID";
            mysql_v_2.Tables[0].Columns.Add();
            mysql_v_2.Tables[0].Columns[1].ColumnName = "姓名";
            mysql_v_2.Tables[0].Columns.Add();
            mysql_v_2.Tables[0].Columns[2].ColumnName = "干部单位";
            mysql_v_2.Tables[0].Columns.Add();
            mysql_v_2.Tables[0].Columns[3].ColumnName = "身份证号";
            mysql_v_2.Tables[0].Columns.Add();
            mysql_v_2.Tables[0].Columns[4].ColumnName = "转递类型";
            mysql_v_2.Tables[0].Columns.Add();
            mysql_v_2.Tables[0].Columns[5].ColumnName = "转递原因";
            mysql_v_2.Tables[0].Columns.Add();
            mysql_v_2.Tables[0].Columns[6].ColumnName = "接收单位";

            //
            mysql_dill.Tables.Add();
            mysql_dill.Tables[0].Columns.Add();
            mysql_dill.Tables[0].Columns[0].ColumnName = "id";
            mysql_dill.Tables[0].Columns.Add();
            mysql_dill.Tables[0].Columns[1].ColumnName = "tid";
            mysql_dill.Tables[0].Columns.Add();
            mysql_dill.Tables[0].Columns[2].ColumnName = "status";
            mysql_dill.Tables[0].Columns.Add();
            mysql_dill.Tables[0].Columns[3].ColumnName = "name";
            mysql_dill.Tables[0].Columns.Add();
            mysql_dill.Tables[0].Columns[4].ColumnName = "perid";
            mysql_dill.Tables[0].Columns.Add();
            mysql_dill.Tables[0].Columns[5].ColumnName = "position";
            mysql_dill.Tables[0].Columns.Add();
            mysql_dill.Tables[0].Columns[6].ColumnName = "operate_time";
            mysql_dill.Tables[0].Columns.Add();
            mysql_dill.Tables[0].Columns[7].ColumnName = "unit_name";
            mysql_dill.Tables[0].Columns.Add();
            mysql_dill.Tables[0].Columns[8].ColumnName = "operate_id";

            //
            button17.Enabled = false;
            button7.Enabled = false;
            label1.Enabled = false;
            comboBox3.Enabled = false;
            label2.Enabled = false;
            comboBox4.Enabled = false;
        }
        #endregion

        #region  
        public bool selectflg = false;
        public List<string> temp=new List<string>();
        public List<string> positionlist = new List<string>();
        public int ro = 0;
        private void button1_Click_1(object sender, EventArgs e)
        {
            int boxnum=0;
            int currentnum = 0;
            string str = "";
            if (comboBox1.Text == "" || comboBox2.Text == "" || comboBox9.Text == "")
            {
                MessageBox.Show("请选择完善整理信息！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //01
            if (str != "" && diitype != comboBox9.SelectedIndex)
            {
                MessageBox.Show("整理类型与该批次操作不符，请检查！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            diitype = comboBox9.SelectedIndex;

            #region  
            //
            if (comboBox2.Text == "全部")
            {
                List<string> str1 = new List<string>();
                for (int i = 0; i < comboBox2.Items.Count; i++)
                {
                    if (comboBox2.Items[i].ToString() != "全部")
                    {
                        str1.Add(comboBox2.Items[i].ToString());
                    }
                }
                for (int k = 0; k < str1.Count; k++)
                {
                    boxnum = 0;
                    currentnum = 0;

                    //
                    for (int i = 0; i < mysql_v.Tables[0].Rows.Count; i++)
                    {
                        if (comboBox3.Enabled == false)
                        {
                            if (str1[k] == mysql_v.Tables[0].Rows[i][1].ToString() && comboBox1.Text == mysql_v.Tables[0].Rows[i][2].ToString())
                            {
                                MessageBox.Show("该干部信息已录入，请重新选择！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                        else
                        {
                            if (temp.Contains(comboBox3.Text) && str1[k] == mysql_v.Tables[0].Rows[i][1].ToString() && comboBox1.Text == mysql_v.Tables[0].Rows[i][2].ToString())
                            {
                                MessageBox.Show("该干部信息已录入，请重新选择！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    //
                    if (!manyname.Contains(str1[k]))
                    {
                        //
                        for (int i = 0; i < mysql.Tables[0].Rows.Count; i++)
                        {
                            if ((str1[k] == mysql.Tables[0].Rows[i][3].ToString() && comboBox1.Text == mysql.Tables[0].Rows[i][7].ToString()))
                            {
                                boxnum++;
                                if (mysql.Tables[0].Rows[i][2].ToString() == "1")
                                {
                                    currentnum++;
                                    mysql_dill.Tables[0].Rows.Add();
                                    mysql_dill.Tables[0].Rows[ro][0] = mysql.Tables[0].Rows[i][0];
                                    mysql_dill.Tables[0].Rows[ro][1] = mysql.Tables[0].Rows[i][1];
                                    mysql_dill.Tables[0].Rows[ro][2] = mysql.Tables[0].Rows[i][2];
                                    mysql_dill.Tables[0].Rows[ro][3] = mysql.Tables[0].Rows[i][3];
                                    mysql_dill.Tables[0].Rows[ro][4] = mysql.Tables[0].Rows[i][4];
                                    mysql_dill.Tables[0].Rows[ro][5] = mysql.Tables[0].Rows[i][5];
                                    mysql_dill.Tables[0].Rows[ro][6] = mysql.Tables[0].Rows[i][6];
                                    mysql_dill.Tables[0].Rows[ro][7] = mysql.Tables[0].Rows[i][7];
                                    mysql_dill.Tables[0].Rows[ro][8] = mysql.Tables[0].Rows[i][8];
                                    ro++;
                                    //
                                    int mm = f1.chenking_dilldata(mysql.Tables[0].Rows[i][1].ToString());
                                    if (mm == 0 && diitype == 1&& mysql.Tables[0].Rows[i][1].ToString()!="")
                                    {
                                        MessageBox.Show("档案没有整理出库记录，请重新操作", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        mysql_dill.Tables[0].Clear();
                                        mysql_v.Tables[0].Clear();
                                        ro = 0;
                                        return;
                                    }
                                    if (mm > 0 && diitype == 0)
                                    {
                                        MessageBox.Show("档案已出库整理，请重新操作", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        mysql_dill.Tables[0].Clear();
                                        mysql_v.Tables[0].Clear();
                                        ro = 0;
                                        return;
                                    }

                                }
                            }
                            if ((str1[k] == mysql.Tables[0].Rows[i][3].ToString() && comboBox1.Text == mysql.Tables[0].Rows[i][7].ToString() && str != mysql.Tables[0].Rows[i][5].ToString()))
                            {

                                mysql_v.Tables[0].Rows.Add();
                                mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][0] = count++;         //ID
                                mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][1] = str1[k];  //
                                mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][2] = comboBox1.Text;  //
                                mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][3] = boxnum;  //
                                mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][4] = currentnum;  //
                                mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][5] = mysql.Tables[0].Rows[i][5].ToString();  //
                                positionlist.Add(mysql.Tables[0].Rows[i][5].ToString());
                                temp.Add(mysql.Tables[0].Rows[i][4].ToString());
                                str = mysql.Tables[0].Rows[i][5].ToString();
                            }
                        }
                        mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][3] = boxnum;  //
                        mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][4] = currentnum;  //
                    }
                    //
                    else
                    {
                        for (int i = 0; i < mysql.Tables[0].Rows.Count; i++)
                        {
                            if (comboBox1.Text == mysql.Tables[0].Rows[i][7].ToString() && str1[k] == mysql.Tables[0].Rows[i][3].ToString())
                            {
                                if (comboBox3.Items.Contains(mysql.Tables[0].Rows[i][4].ToString()))
                                { continue; }
                                comboBox3.Items.Add(mysql.Tables[0].Rows[i][4].ToString());
                            }
                        }
                        for (int m = 0; m < comboBox3.Items.Count; m++)
                        {
                            boxnum = 0;
                            currentnum = 0;
                            string str2 = comboBox3.Items[m].ToString();
                            for (int i = 0; i < mysql.Tables[0].Rows.Count; i++)
                            {
                                if ((str2 == mysql.Tables[0].Rows[i][4].ToString() && str1[k] == mysql.Tables[0].Rows[i][3].ToString() && comboBox1.Text == mysql.Tables[0].Rows[i][7].ToString()))
                                {
                                    boxnum++;
                                    if (mysql.Tables[0].Rows[i][2].ToString() == "1")
                                    {
                                        currentnum++;
                                        mysql_dill.Tables[0].Rows.Add();
                                        mysql_dill.Tables[0].Rows[ro][0] = mysql.Tables[0].Rows[i][0];
                                        mysql_dill.Tables[0].Rows[ro][1] = mysql.Tables[0].Rows[i][1];
                                        mysql_dill.Tables[0].Rows[ro][2] = mysql.Tables[0].Rows[i][2];
                                        mysql_dill.Tables[0].Rows[ro][3] = mysql.Tables[0].Rows[i][3];
                                        mysql_dill.Tables[0].Rows[ro][4] = mysql.Tables[0].Rows[i][4];
                                        mysql_dill.Tables[0].Rows[ro][5] = mysql.Tables[0].Rows[i][5];
                                        mysql_dill.Tables[0].Rows[ro][6] = mysql.Tables[0].Rows[i][6];
                                        mysql_dill.Tables[0].Rows[ro][7] = mysql.Tables[0].Rows[i][7];
                                        mysql_dill.Tables[0].Rows[ro][8] = mysql.Tables[0].Rows[i][8];
                                        ro++;
                                        //
                                        int mm = f1.chenking_dilldata(mysql.Tables[0].Rows[i][1].ToString());
                                        if (mm == 0 && diitype == 1 && mysql.Tables[0].Rows[i][1].ToString() != "")
                                        {
                                            MessageBox.Show("档案没有整理出库记录，请重新操作", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            mysql_dill.Tables[0].Clear();
                                            mysql_v.Tables[0].Clear();
                                            ro = 0;
                                            return;
                                        }
                                        if (mm > 0 && diitype == 0)
                                        {
                                            MessageBox.Show("档案已出库整理，请重新操作", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            mysql_dill.Tables[0].Clear();
                                            mysql_v.Tables[0].Clear();
                                            ro = 0;
                                            return;
                                        }
                                    }
                                }
                                if ((str2 == mysql.Tables[0].Rows[i][4].ToString() && str1[k] == mysql.Tables[0].Rows[i][3].ToString() && comboBox1.Text == mysql.Tables[0].Rows[i][7].ToString() && str != mysql.Tables[0].Rows[i][5].ToString()))
                                {
                                    mysql_v.Tables[0].Rows.Add();
                                    mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][0] = count++;         //ID
                                    mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][1] = str1[k];  //
                                    mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][2] = comboBox1.Text;  //
                                    mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][3] = boxnum;  //
                                    mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][4] = currentnum;  //
                                    mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][5] = mysql.Tables[0].Rows[i][5].ToString();  //
                                    positionlist.Add(mysql.Tables[0].Rows[i][5].ToString());
                                    temp.Add(mysql.Tables[0].Rows[i][4].ToString());
                                    str = mysql.Tables[0].Rows[i][5].ToString();
                                }
                            }
                            mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][3] = boxnum;  //
                            mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][4] = currentnum;  //
                        }
                    }
                }
                comboBox3.Text = "";
                comboBox3.Enabled = false;
                selectflg = true;
                button17.Enabled = true;
                return;
            }
            #endregion

            #region  
            //
            if (selectflg == false)
            {
                //
                if (manyname.Contains(comboBox2.Text) &&comboBox3.Enabled==false)
                {
                    MessageBox.Show("库中存在多个" + comboBox2.Text + "请选择身份证信息！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    label1.Enabled = true;
                    comboBox3.Enabled = true;
                    for (int i = 0; i < mysql.Tables[0].Rows.Count; i++)
                    {
                        if (comboBox1.Text == mysql.Tables[0].Rows[i][7].ToString()&&comboBox2.Text == mysql.Tables[0].Rows[i][3].ToString())
                        {
                            if (comboBox3.Items.Contains(mysql.Tables[0].Rows[i][4].ToString()))
                            { continue; }
                            comboBox3.Items.Add(mysql.Tables[0].Rows[i][4].ToString());
                        }
                    }
                    return;
                }

                //
                for (int i = 0; i < mysql_v.Tables[0].Rows.Count; i++)
                {
                    if (comboBox3.Enabled == false)
                    {
                        if (comboBox2.Text == mysql_v.Tables[0].Rows[i][1].ToString() && comboBox1.Text == mysql_v.Tables[0].Rows[i][2].ToString())
                        {
                            MessageBox.Show("该干部信息已录入，请重新选择！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    else
                    {
                        if (temp.Contains(comboBox3.Text) && comboBox2.Text == mysql_v.Tables[0].Rows[i][1].ToString() && comboBox1.Text == mysql_v.Tables[0].Rows[i][2].ToString())
                        {
                            MessageBox.Show("该干部信息已录入，请重新选择！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }
                
                if (comboBox3.Enabled==false)
                {
                    for (int i = 0; i < mysql.Tables[0].Rows.Count; i++)
                    {
                        if ((comboBox2.Text == mysql.Tables[0].Rows[i][3].ToString() && comboBox1.Text == mysql.Tables[0].Rows[i][7].ToString()))
                        {
                            boxnum++;
                            if (mysql.Tables[0].Rows[i][2].ToString() == "1")
                            {
                                currentnum++;
                                mysql_dill.Tables[0].Rows.Add();
                                mysql_dill.Tables[0].Rows[ro][0] = mysql.Tables[0].Rows[i][0];
                                mysql_dill.Tables[0].Rows[ro][1] = mysql.Tables[0].Rows[i][1];
                                mysql_dill.Tables[0].Rows[ro][2] = mysql.Tables[0].Rows[i][2];
                                mysql_dill.Tables[0].Rows[ro][3] = mysql.Tables[0].Rows[i][3];
                                mysql_dill.Tables[0].Rows[ro][4] = mysql.Tables[0].Rows[i][4];
                                mysql_dill.Tables[0].Rows[ro][5] = mysql.Tables[0].Rows[i][5];
                                mysql_dill.Tables[0].Rows[ro][6] = mysql.Tables[0].Rows[i][6];
                                mysql_dill.Tables[0].Rows[ro][7] = mysql.Tables[0].Rows[i][7];
                                mysql_dill.Tables[0].Rows[ro][8] = mysql.Tables[0].Rows[i][8];
                                ro++;
                                //
                                int mm = f1.chenking_dilldata(mysql.Tables[0].Rows[i][1].ToString());
                                if (mm == 0 && diitype == 1 && mysql.Tables[0].Rows[i][1].ToString() != "")
                                {
                                    MessageBox.Show("档案没有整理出库记录，请重新操作", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    mysql_dill.Tables[0].Clear();
                                    mysql_v.Tables[0].Clear();
                                    ro = 0;
                                    return;
                                }
                                if (mm > 0 && diitype == 0)
                                {
                                    MessageBox.Show("档案已出库整理，请重新操作", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    mysql_dill.Tables[0].Clear();
                                    mysql_v.Tables[0].Clear();
                                    ro = 0;
                                    return;
                                }
                            }
                        }
                            if ((comboBox2.Text == mysql.Tables[0].Rows[i][3].ToString() && comboBox1.Text == mysql.Tables[0].Rows[i][7].ToString()&&str != mysql.Tables[0].Rows[i][5].ToString()))
                        {

                            mysql_v.Tables[0].Rows.Add();
                            mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][0] = count++;         //ID
                            mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][1] = comboBox2.Text;  //
                            mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][2] = comboBox1.Text;  //
                            mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][3] = boxnum;  //
                            mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][4] = currentnum;  //
                            mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][5] = mysql.Tables[0].Rows[i][5].ToString();  //
                            positionlist.Add(mysql.Tables[0].Rows[i][5].ToString());
                            temp.Add(mysql.Tables[0].Rows[i][4].ToString());
                            str = mysql.Tables[0].Rows[i][5].ToString();
                        }
                    }
                    mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][3] = boxnum;  //
                    mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][4] = currentnum;  //
                }
                else
                {
                    for (int i = 0; i < mysql.Tables[0].Rows.Count; i++)
                    {
                        if ((comboBox3.Text == mysql.Tables[0].Rows[i][4].ToString() && comboBox2.Text == mysql.Tables[0].Rows[i][3].ToString() && comboBox1.Text == mysql.Tables[0].Rows[i][7].ToString()))
                        {
                            boxnum++;
                            if (mysql.Tables[0].Rows[i][2].ToString() == "1")
                            {
                                currentnum++;
                                mysql_dill.Tables[0].Rows.Add();
                                mysql_dill.Tables[0].Rows[ro][0] = mysql.Tables[0].Rows[i][0];
                                mysql_dill.Tables[0].Rows[ro][1] = mysql.Tables[0].Rows[i][1];
                                mysql_dill.Tables[0].Rows[ro][2] = mysql.Tables[0].Rows[i][2];
                                mysql_dill.Tables[0].Rows[ro][3] = mysql.Tables[0].Rows[i][3];
                                mysql_dill.Tables[0].Rows[ro][4] = mysql.Tables[0].Rows[i][4];
                                mysql_dill.Tables[0].Rows[ro][5] = mysql.Tables[0].Rows[i][5];
                                mysql_dill.Tables[0].Rows[ro][6] = mysql.Tables[0].Rows[i][6];
                                mysql_dill.Tables[0].Rows[ro][7] = mysql.Tables[0].Rows[i][7];
                                mysql_dill.Tables[0].Rows[ro][8] = mysql.Tables[0].Rows[i][8];
                                ro++;
                                //
                                int mm = f1.chenking_dilldata(mysql.Tables[0].Rows[i][1].ToString());
                                if (mm == 0 && diitype == 1 && mysql.Tables[0].Rows[i][1].ToString() != "")
                                {
                                    MessageBox.Show("档案没有整理出库记录，请重新操作", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    mysql_dill.Tables[0].Clear();
                                    mysql_v.Tables[0].Clear();
                                    ro = 0;
                                    return;
                                }
                                if (mm > 0 && diitype == 0)
                                {
                                    MessageBox.Show("档案已出库整理，请重新操作", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    mysql_dill.Tables[0].Clear();
                                    mysql_v.Tables[0].Clear();
                                    ro = 0;
                                    return;
                                }
                            }
                        }
                        if ((comboBox3.Text == mysql.Tables[0].Rows[i][4].ToString() && comboBox2.Text == mysql.Tables[0].Rows[i][3].ToString() && comboBox1.Text == mysql.Tables[0].Rows[i][7].ToString() && str != mysql.Tables[0].Rows[i][5].ToString()))
                        {
                            mysql_v.Tables[0].Rows.Add();
                            mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][0] = count++;         //ID
                            mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][1] = comboBox2.Text;  //
                            mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][2] = comboBox1.Text;  //
                            mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][3] = boxnum;  //
                            mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][4] = currentnum;  //
                            mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][5] = mysql.Tables[0].Rows[i][5].ToString();  //
                            positionlist.Add(mysql.Tables[0].Rows[i][5].ToString());
                            temp.Add(mysql.Tables[0].Rows[i][4].ToString());
                            str = mysql.Tables[0].Rows[i][5].ToString();
                        }
                    }
                    mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][3] = boxnum;  //
                    mysql_v.Tables[0].Rows[mysql_v.Tables[0].Rows.Count - 1][4] = currentnum;  //
                }
                comboBox3.Text = "";
                comboBox3.Enabled = false;
                selectflg = true;
                button17.Enabled = true;
            }
            #endregion
            else
            {
                selectflg = false;
                comboBox1.Text = "";
                comboBox2.Text = "";
                MessageBox.Show("已选择，请重新选择整理信息！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

        }
        #endregion

        #region  mysql
        private void load_mysql(DataSet mysql)
        {
            for (int i = 0; i < mysql.Tables[0].Rows.Count; i++)
            {
                if (comboBox1.Items.Contains(mysql.Tables[0].Rows[i][7]))
                {
                    continue;
                }
                comboBox1.Items.Add(mysql.Tables[0].Rows[i][7]);
                comboBox6.Items.Add(mysql.Tables[0].Rows[i][7]);
            }
        }
        #endregion

        #region  combox
        List<string> boxid = new List<string>();
        List<int> boxid_count = new List<int>();
        private void namelist(object sender, EventArgs e)
        {
           
            if (tabControl1.SelectedIndex==0)
            {
                comboBox2.Text = "";
                comboBox2.Items.Clear();
                boxid.Clear();
                boxid_count.Clear();
                manyname.Clear();
                for (int i = 0; i < mysql.Tables[0].Rows.Count; i++)
                {
                    if (comboBox1.Text == mysql.Tables[0].Rows[i][7].ToString())
                    {
                        //
                        if (boxid.Contains(mysql.Tables[0].Rows[i][4].ToString()))
                        {
                            continue;
                        }
                        boxid.Add(mysql.Tables[0].Rows[i][4].ToString());
                        //
                        if (comboBox2.Items.Contains(mysql.Tables[0].Rows[i][3].ToString()))
                        {
                            manyname.Add(mysql.Tables[0].Rows[i][3].ToString());
                            boxid_count.Add(i);
                            continue;
                        }
                        else
                        {
                            comboBox2.Items.Add(mysql.Tables[0].Rows[i][3].ToString());
                        }

                    }
                }
                comboBox2.Items.Add("");
                selectflg = false;
                comboBox2.SelectedIndex = (comboBox2.Items.IndexOf(""));
            }
            else
            {
                comboBox5.Text = "";
                comboBox5.Items.Clear();
                boxid.Clear();
                boxid_count.Clear();
                manyname.Clear();
                for (int i = 0; i < mysql.Tables[0].Rows.Count; i++)
                {
                    if (comboBox6.Text == mysql.Tables[0].Rows[i][7].ToString())
                    {
                        //
                        if (boxid.Contains(mysql.Tables[0].Rows[i][4].ToString()))
                        {
                            continue;
                        }
                        boxid.Add(mysql.Tables[0].Rows[i][4].ToString());
                        //
                        if (comboBox5.Items.Contains(mysql.Tables[0].Rows[i][3].ToString()))
                        {
                            manyname.Add(mysql.Tables[0].Rows[i][3].ToString());
                            boxid_count.Add(i);
                            continue;
                        }
                        else
                        {
                            comboBox5.Items.Add(mysql.Tables[0].Rows[i][3].ToString());
                        }
                    }
                }
                selectflg = false;
            }
        }

        #endregion

        #region   

        private void button17_Click_1(object sender, EventArgs e)
        {
            //
            form_shizhurenpasswd szr = new form_shizhurenpasswd(f1, 0, positionlist);
            szr.Text = "出入库整理开锁权限认证";
            szr.ShowDialog();
            if (szr.rightorwrang == true)
            {
                button17.Enabled = false;
                //
                comboBox1.Text = "";
                comboBox2.Text = "";
                comboBox3.Text = "";
                comboBox9.Text = "";
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                comboBox9.Enabled = false;
                label1.Enabled = false;
                label3.Enabled = false;
                label16.Enabled = false;
                positionlist.Clear();
                //
                f1.savedate_dill(mysql_dill, ro, diitype);
            }
            else
            { return; }
        }
        #endregion

        #region  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click_1(object sender, EventArgs e)
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
        #endregion

        #region  
        private void button4_Click(object sender, EventArgs e)
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
        #endregion

        #region  
        //
        public int id_num = 0;
        string idcard = "";
        private void button6_Click(object sender, EventArgs e)
        {
            if (comboBox6.Text == "" || comboBox5.Text == "" || comboBox7.Text == "" || comboBox8.Text == "" || textBox1.Text == "")
            {
                MessageBox.Show("请完善干部信息！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //
            for (int i = 0; i < mysql_v_1.Tables[0].Rows.Count; i++)
            {
                if (comboBox4.Enabled == false)
                {
                    if (comboBox5.Text == mysql_v_1.Tables[0].Rows[i][1].ToString() && comboBox6.Text == mysql_v_1.Tables[0].Rows[i][2].ToString())
                    {
                        MessageBox.Show("该干部信息已录入，请重新选择！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    if (temp.Contains(comboBox4.Text) && comboBox5.Text == mysql_v_1.Tables[0].Rows[i][1].ToString() && comboBox6.Text == mysql_v_1.Tables[0].Rows[i][2].ToString())
                    {
                        MessageBox.Show("该干部信息已录入，请重新选择！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }
            //
            if (manyname.Contains(comboBox5.Text) && comboBox4.Enabled == false)
            {
                MessageBox.Show("库中存在多个" + comboBox5.Text + "请选择身份证信息！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                label2.Enabled = true;
                comboBox4.Enabled = true;
                for (int i = 0; i < mysql.Tables[0].Rows.Count; i++)
                {
                    if (comboBox6.Text == mysql.Tables[0].Rows[i][7].ToString() && comboBox5.Text == mysql.Tables[0].Rows[i][3].ToString())
                    {
                        if (comboBox4.Items.Contains(mysql.Tables[0].Rows[i][4].ToString()))
                        { continue; }
                        comboBox4.Items.Add(mysql.Tables[0].Rows[i][4].ToString());
                    }
                }
                return;
            }
            //
            string perid_str = "*";
            for (int i = 0; i < mysql.Tables[0].Rows.Count; i++)
            {
                if (comboBox4.Enabled == false)
                {
                    if (mysql.Tables[0].Rows[i][3].ToString() == comboBox5.Text)
                    {
                        idcard = mysql.Tables[0].Rows[i][4].ToString();
                        perid_str+=mysql.Tables[0].Rows[i][1].ToString()+"*";
                        if (positionlist.IndexOf(mysql.Tables[0].Rows[i][5].ToString()) < 0)
                        {
                            positionlist.Add(mysql.Tables[0].Rows[i][5].ToString());
                        }
                    }
                }
                else
                {
                    if (mysql.Tables[0].Rows[i][4].ToString() == comboBox4.Text)
                    {
                        idcard = mysql.Tables[0].Rows[i][4].ToString();
                        perid_str += mysql.Tables[0].Rows[i][1].ToString() + "*";
                        if (positionlist.IndexOf(mysql.Tables[0].Rows[i][5].ToString()) < 0)
                        {
                            positionlist.Add(mysql.Tables[0].Rows[i][5].ToString());
                        }
                    }
                }
            }
            peridlist1.Add(perid_str);

            mysql_v_1.Tables[0].Rows.Add();
            mysql_v_1.Tables[0].Rows[id_num][0] = 00;             //
            mysql_v_1.Tables[0].Rows[id_num][1] = comboBox5.Text;       //
            mysql_v_1.Tables[0].Rows[id_num][2] = comboBox6.Text;       //
            mysql_v_1.Tables[0].Rows[id_num][3] = idcard;               //
            mysql_v_1.Tables[0].Rows[id_num][4] = comboBox7.Text;       //
            mysql_v_1.Tables[0].Rows[id_num][5] = comboBox8.Text;       //
            mysql_v_1.Tables[0].Rows[id_num][6] = textBox1.Text;       //
            mysql_v_1.Tables[0].Rows[id_num][7] = DateTime.Now.ToString();       //
            mysql_v_1.Tables[0].Rows[id_num][8] = f1.username;       //

            mysql_v_2.Tables[0].Rows.Add();
            mysql_v_2.Tables[0].Rows[id_num][0] = id_num + 1;             //
            mysql_v_2.Tables[0].Rows[id_num][1] = comboBox5.Text;       //
            mysql_v_2.Tables[0].Rows[id_num][2] = comboBox6.Text;       //
            mysql_v_2.Tables[0].Rows[id_num][3] = idcard;       //
            mysql_v_2.Tables[0].Rows[id_num][4] = comboBox7.Text;       //
            mysql_v_2.Tables[0].Rows[id_num][5] = comboBox8.Text;       //
            mysql_v_2.Tables[0].Rows[id_num][6] = textBox1.Text;       //

            dataGridView1.DataSource = mysql_v_2.Tables[0];

            if (dataGridView1.RowCount == 2)
            {
                //
                DataGridViewButtonColumn colum_button = new DataGridViewButtonColumn();
                colum_button.UseColumnTextForButtonValue = false;
                colum_button.DefaultCellStyle.NullValue = "转  递";
                //colum_button.Text = "转  递";
                colum_button.Name = "转递状态";
                dataGridView1.Columns.Add(colum_button);
                dataGridView1.ClearSelection();
                button7.Enabled = true;
            }
            id_num++;
        }
        #endregion

        #region   
        public List<string> peridlist1 = new List<string>();
        public List<string> delete_idlist = new List<string>();
        public void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;  
                                                            
            if (dgv.Columns[e.ColumnIndex].Name == "转递状态"&& dgv.Rows[e.RowIndex].Cells[7].Value == null)
            {
                try
                {
                    //
                    form_transfer ftrans = new form_transfer(mysql_v_2.Tables[0].Rows[e.RowIndex][2].ToString(), mysql_v_1.Tables[0].Rows[e.RowIndex][1].ToString(), mysql_v_1.Tables[0].Rows[e.RowIndex][3].ToString(), mysql_v_2.Tables[0].Rows[e.RowIndex][4].ToString(), mysql_v_2.Tables[0].Rows[e.RowIndex][6].ToString(), mysql_v_2.Tables[0].Rows[e.RowIndex][5].ToString(), peridlist1[e.RowIndex]);
                    ftrans.ShowDialog();
                    delete_idlist = ftrans.perid_listcheck;

                    if (ftrans.ok_flg == true)
                    {
                        id_num--;
                        //
                        f1.mysql_clear_data("store", "tablename", delete_idlist);
                        //
                        f1.savedate_transfer(mysql_v_1, id_num - 1);
                        dgv.ClearSelection();
                        dgv.Rows[e.RowIndex].Cells[7].Value = "转递完成";
                        dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                        MessageBox.Show("转递操作成功！", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch
                { }
            }
        }
        #endregion

        #region    
        private void button7_Click(object sender, EventArgs e)
        {           
            //
            form_shizhurenpasswd szr = new form_shizhurenpasswd(f1, 0, positionlist);
            szr.Text = "出入库整理开锁权限认证";
            szr.ShowDialog();
            if (szr.rightorwrang == true)
            {
                button7.Enabled = false;
                //
                comboBox6.Text = "";
                comboBox5.Text = "";
                comboBox4.Text = "";
                comboBox7.Text = "";
                comboBox8.Text = "";
                textBox1.Text = "";
                positionlist.Clear();
            }
            else
            { return; }
        }
        #endregion

        #region   
        private void button5_Click(object sender, EventArgs e)
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
        #endregion

        #region   
        private void button2_Click_1(object sender, EventArgs e)
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
        #endregion
    }
}