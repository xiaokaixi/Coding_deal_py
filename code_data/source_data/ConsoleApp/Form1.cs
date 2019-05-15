using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;   //引用串口操作
using System.Data.OleDb;
using Renci.SshNet;
using System.Threading;
using System.Diagnostics;

namespace ConsoleApp1
{
    public partial class 盘点机扫描程序 : Form
    {
        public 盘点机扫描程序()
        {
            InitializeComponent();
        }
        class DataTag//自定义的标签读取结构体
        {

            public int Count = 0;

            public string Epc = "";
            //public string Tid = "";

            public string LatestTime = "";

        }
        public mysqlclass pandian1 = new mysqlclass();
        public DataSet mysql1 = new DataSet();
        public DataSet mysql2 = new DataSet();
        Thread sm;
        private Thread multi_get_thread;//循环处理数据的线程
        private volatile bool _shouldStop;
        //public DataTable mytable2 = new DataTable();
        //DataColumn shjclm1 = new DataColumn("id", Type.GetType("System.Int32"));
        //DataColumn shjclm2 = new DataColumn("TID", Type.GetType("System.String"));
        //DataColumn shjclm3 = new DataColumn("time", Type.GetType("System.String"));
        Vbarapi qrsm = new Vbarapi();
        private bool loop = true;

        public class mysqlclass
        {

            public int conn = 0;
            public static string mysqlconn = "Server=127.0.0.1;user=root;pwd=654321;";
            public MySqlConnection mysqlconn1 = new MySqlConnection(mysqlconn);
            //public MySqlDataAdapter mysqlada = new MySqlDataAdapter();     //数据库执行命令方法,适合大数据量

            //读取返回DataReader指令
            public List<string> mysqlreader(string sql)
            {
                List<string> list1 = new List<string>();
                MySqlDataReader tablename = null;   //数据库命令返回数据，适合小数据量
                MySqlCommand mysqlcom_showtable = new MySqlCommand(sql, mysqlconn1);

                try
                {
                    mysqlconn1.Open();
                    tablename = mysqlcom_showtable.ExecuteReader();
                    if (tablename.HasRows)   //判断返回表name是否为空
                    {
                        string t;
                        while (tablename.Read())
                        {
                            t = tablename[0].ToString();
                            list1.Add(t);

                            t = tablename[1].ToString();
                            list1.Add(t);

                            t = tablename["time"].ToString();
                            list1.Add(t);

                            list1.Add("%");
                        }
                        tablename.Close(); //关闭读数据库返回命令
                        MessageBox.Show("数据库查询成功", "提示");
                    }
                    else
                    { MessageBox.Show("数据库无任何表！", "error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    mysqlcom_showtable.Dispose();  //关闭数据库命令
                    tablename.Dispose();
                    mysqlconn1.Dispose();
                    return list1;
                }
                catch { return list1; }

            }


            //mysql adapter select命令
            public DataSet mysqladapter(string mysqlselect)
            {
                //MySqlCommand mysqlcom_select = new MySqlCommand(mysqlselect, mysqlconn1);  //sql-select命令
                DataSet mysql = new DataSet();
                MySqlDataAdapter ada = new MySqlDataAdapter(mysqlselect, mysqlconn1);
                try
                {
                    mysqlconn1.Open();
                    try
                    {

                        ada.Fill(mysql);

                    }
                    catch
                    {
                        MessageBox.Show("数据库链接中断！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    finally
                    {
                        ada.Dispose();

                        mysqlconn1.Dispose();

                    }
                    return mysql;
                }
                catch { return mysql; }
            }



            //mysql 保存更新数据命令
            public bool mysqlupdateSaveAll(string mysqlcom, DataSet table)
            {
                bool file = true;
                //List<string> comlist = new List<string>();
                StringBuilder com = new StringBuilder();
                MySqlCommand mysqlcom_save = new MySqlCommand();  //sql-show  


                mysqlconn1.Open();

                for (int j=0;j<table.Tables[0].Rows.Count;j++)
                {
                    com.Clear();
                    com.Append(mysqlcom);
                    for (int i = 0; i < 3; i++)
                        {
                        if (i == 0)
                            com.Append(table.Tables[0].Rows[j][i].ToString()  + ",");
                        else if (i != 2)
                            com.Append("\"" + table.Tables[0].Rows[j][i].ToString() + "\"" + ",");
                        else
                            com.Append("\"" + table.Tables[0].Rows[j][i].ToString() + "\"" + ")");
                        }
                    
                    mysqlcom_save = new MySqlCommand(com.ToString(), mysqlconn1);  //sql-save
                    mysqlcom_save.ExecuteNonQuery();
                    mysqlcom_save.Dispose();

                }
                //com.Append("where id=" + table.Tables[0].Rows[i][0]);
                //comlist.Add(com.ToString());


             
                    mysqlconn1.Dispose();
                
               
                return file;
            }


            public bool mysqlupdatecomarow(string mysqlcom, DataSet table)
            {
                bool file = true;
                List<string> comlist = new List<string>();
                StringBuilder com = new StringBuilder();
                MySqlCommand mysqlcom_save = new MySqlCommand();  //sql-show           


                com.Clear();
                com.Append(mysqlcom);
                for (int j = 0; j < table.Tables[0].Columns.Count; j++)
                {

                    if (j != table.Tables[0].Columns.Count - 1)
                    {
                        com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[table.Tables[0].Rows.Count - 2][j] + "\",");
                    }
                    else
                    {
                        com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[table.Tables[0].Rows.Count - 2][j] + "\"");
                    }
                }
                //com.Append("where id=" + table.Tables[0].Rows[table.Tables[0].Rows.Count - 2][0]);
                comlist.Add(com.ToString());

                try
                {
                    //int.Parse(i)//string转int
                    mysqlconn1.Open();
                    try
                    {
                        for (int i = 0; i < comlist.Count(); i++)
                        {
                            mysqlcom_save = new MySqlCommand(comlist[i], mysqlconn1);  //sql-save
                            mysqlcom_save.ExecuteNonQuery();
                        }

                    }
                    catch
                    {
                        file = false;
                    }
                    mysqlconn1.Dispose();
                }
                catch
                {
                    MessageBox.Show("保存数据失败！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return file;
            }

            public bool mysqlsavecom(string mysqlcom, DataSet table, string tablename)
            {
                bool file = true;
                List<string> comlist = new List<string>();
                StringBuilder com = new StringBuilder();
                MySqlCommand mysqlcom_save = new MySqlCommand();  //sql-show        
                com.Clear();
                com.Append(mysqlcom);
                try
                {
                    for (int i = 0; i < table.Tables[0].Columns.Count; i++)
                    {
                        if (i == table.Tables[0].Columns.Count - 1)
                        {
                            com.Append(table.Tables[0].Columns[i] + " char(100))");
                        }
                        else
                        {
                            com.Append(table.Tables[0].Columns[i] + " char(100),");
                        }
                    }

                    mysqlconn1.Open();
                    mysqlcom_save = new MySqlCommand(com.ToString(), mysqlconn1);  //sql-save
                    mysqlcom_save.ExecuteNonQuery();
                    mysqlconn1.Dispose();

                }
                catch
                {
                    MessageBox.Show("00001", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


                mysqlcom = "insert into " + tablename + " set ";
                for (int i = 0; i < table.Tables[0].Rows.Count; i++)
                {
                    com.Clear();
                    com.Append(mysqlcom);
                    for (int j = 0; j < table.Tables[0].Columns.Count; j++)
                    {
                        if (j != table.Tables[0].Columns.Count - 1)
                        {
                            com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[i][j] + "\",");
                        }
                        else
                        {
                            com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[i][j] + "\"");
                        }
                    }

                    comlist.Add(com.ToString());
                }

                try
                {
                    //int.Parse(i)//string转int
                    mysqlconn1.Open();
                    try
                    {
                        for (int i = 0; i < comlist.Count() - 1; i++)
                        {
                            mysqlcom_save = new MySqlCommand(comlist[i], mysqlconn1);  //sql-save
                            mysqlcom_save.ExecuteNonQuery();
                        }

                    }
                    catch
                    {
                        file = false;
                    }
                    mysqlconn1.Dispose();
                }
                catch
                {
                    MessageBox.Show("00002！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return file;
            }


            //不返回值的mysql命令
            public void mysqlnon(string sql)
            {


                MySqlCommand mysqlcom_showtable = new MySqlCommand(sql, mysqlconn1);  //sql-show
                try
                {
                    mysqlconn1.Open();
                    mysqlcom_showtable.ExecuteNonQuery();

                }
                catch
                {
                    MessageBox.Show("数据库链接中断！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    mysqlcom_showtable.Dispose();  //关闭数据库命令
                    mysqlconn1.Dispose();
                }
            }
        }
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    string mystr="";
        //    foreach (Control outc in groupBox1.Controls)
        //        if (outc is RadioButton)
        //            if (((RadioButton)outc).Checked)
        //                mystr = "申进毕业于"+outc.Text;
        //    foreach (Control outc in groupBox2.Controls)
        //        if (outc is RadioButton)
        //            if (((RadioButton)outc).Checked)
        //                mystr += "\n申进的年龄是" + outc.Text;
        //    Console.WriteLine(mystr);
        //    MessageBox.Show(mystr,"选择结果是");

        //}

        private void load_exdata(string str3)
        {
            string mysqlcom1 = "use debug";


            string mysqlselect2 = "select * from " + str3;
            List<string> list1 = new List<string>();
            //查询表
            pandian1.mysqlnon(mysqlcom1);                  //切换数据库

            //选中模板表1,用于存储

            mysql2 = pandian1.mysqladapter(mysqlselect2);
            mysql1 = pandian1.mysqladapter(mysqlselect2);

            //    if (mysql.Tables.Count != 0)
            //    {
            mysql2.Tables[0].Columns[0].ColumnName = "id";
            mysql2.Tables[0].Columns[1].ColumnName = "tid编号";
            mysql2.Tables[0].Columns[2].ColumnName = "操作时间";
            //dataGridView1.DataSource = mysql2.Tables[0];
            //    }
            //    else
            //    {
            //        MessageBox.Show("查询入库模板数据失败！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
        }
        public void sm_init()
        {
            if (qrsm.openDevice(1))
            {
                //MessageBox.Show("连接设备成功");
                //添加设备支持的码制
                if (qrsm.addCodeFormat((byte)1))
                {
                    //MessageBox.Show("添加二维码成功");
                    //引入模板
                    load_exdata("test");
                    //开灯
                    qrsm.backlight(true);
                    //蜂鸣2次
                    qrsm.beepControl(2);
                    //button4.BackgroundImage = Properties.Resources.timg;
                    dataGridView1.ReadOnly = true;
                }
            }
        }
        public string Decoder()
        {
            byte[] result;
            string sResult = null;
            int size;
            if (qrsm.getResultStr(out result, out size))
            {
                string msg = System.Text.Encoding.Default.GetString(result);
                byte[] buffer = Encoding.UTF8.GetBytes(msg);
                sResult = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            }
            else
            {
                sResult = null;
            }
            return sResult;
        }
        public void sm_thread()
        {

            string p1;

            do
            {
                p1 = Decoder();
                if (p1 != null)
                {
                    qrsm.beepControl(1);
                    listen_sm sm = new listen_sm(listen_thread);
                    this.Invoke(sm, new object[] { this.dataGridView1, p1 });

                    p1 = null;
                }
            }
            while (loop);

        }

        public delegate void listen_sm(DataGridView d1, string s1);

        public void listen_thread(DataGridView d1, string s1)
        {
            List<string> t1 = danalysis(s1, System.DateTime.Now.ToString());
            mysql1.Tables[0].Rows.Add();
            mysql1.Tables[0].Rows[mysql1.Tables[0].Rows.Count - 1][0] = mysql1.Tables[0].Rows.Count;
            mysql1.Tables[0].Rows[mysql1.Tables[0].Rows.Count - 1][1] = dataGridView1.Rows[mysql1.Tables[0].Rows.Count].Cells[1].Value; //t1[0].ToString();
            mysql1.Tables[0].Rows[mysql1.Tables[0].Rows.Count - 1][2] = dataGridView1.Rows[mysql1.Tables[0].Rows.Count].Cells[1].Value;//new MySql.Data.Types.MySqlDateTime(t1[1].ToString());

            mysql2.Tables[0].Rows.Add();
            mysql2.Tables[0].Rows[mysql2.Tables[0].Rows.Count - 1][0] = mysql2.Tables[0].Rows.Count;
            mysql2.Tables[0].Rows[mysql2.Tables[0].Rows.Count - 1][1] = t1[0].ToString();
            mysql2.Tables[0].Rows[mysql2.Tables[0].Rows.Count - 1][2] = new MySql.Data.Types.MySqlDateTime(t1[1].ToString());

            //dataGridView1.DataSource = mysql2.Tables[0];


            d1.ClearSelection();     //取消dataGridView选中焦点状态
            d1.Rows[mysql1.Tables[0].Rows.Count - 1].Selected = true;  //选中最新行
            d1.FirstDisplayedScrollingRowIndex = mysql1.Tables[0].Rows.Count - 1;
            d1.Columns[0].Width = 50;  //id列的宽度设置
            d1.Columns[1].Width = 200;
            if (pandian1.mysqlupdateSaveAll("insert into _pdj_data set ", mysql1))
            {
                d1.EndEdit();            //关闭dataGridView编辑状态
                                         //d1.ClearSelection();
            }
            t1.Clear();

        }
        public List<string> danalysis(string buf, string nowtime)
        {
            List<string> messagex1 = new List<string>();
            //messagex1
            //List<string> messagex2 = new List<string>();
            var timenyr1 = buf.Split(new char[1] { ' ' });
            var timenyr11 = timenyr1[0].Split(new char[1] { '%' });
            messagex1.Add(timenyr11[1].ToString());  //加入TID号-important
            var timl = timenyr1[1].ToString().Split(new char[1] { ':' });
            int time1 = Int32.Parse(timl[0].ToString()) * 100 + Int32.Parse(timl[1]);
            var timenyr2 = nowtime.Split(new char[1] { ' ' });
            timl = timenyr2[1].ToString().Split(new char[1] { ':' });
            int time2 = Int32.Parse(timl[0].ToString()) * 100 + Int32.Parse(timl[1]);
            messagex1.Add(nowtime);
            return messagex1;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //List<string> s1 = new List<string>();
            //string[] s2 = new string[8];
            //pandian1.mysqlsshconn();
            //pandian1.mysqlcom("use mysql");
            //mysql2.Tables.Add(mytable2);
            //mytable2.Columns.Add(shjclm1);
            //mytable2.Columns.Add(shjclm2);
            //mytable2.Columns.Add(shjclm3);

            //mysql2.Tables[0].Columns[0].ColumnName = "id";
            //mysql2.Tables[0].Columns[1].ColumnName = "tid编号";
            //mysql2.Tables[0].Columns[2].ColumnName = "时间";

            //s1=pandian1.mysqlreader("select * from _pdj_data ");
            //DataSet m1=pandian1.mysqladapter("select * from _pdj_data where id=2");
            ////s2=s1.ToString().Split(new char[]{'%'});
            //// object [] n1 = new object[3];

            ////foreach (string b1 in s2)
            ////    Console.Write(b1+"\t");
            ////    Console.WriteLine();
            //dataGridView1.DataSource = m1.Tables[0];
            //Console.ReadKey();
            //(new Form3()).Show();
            //dynamic file = Application.StartupPath + @"\FreeVK.exe";
            //if (!System.IO.File.Exists(file))
            //    MessageBox.Show("启动程序不存在","错误",MessageBoxButtons.OK,MessageBoxIcon.Error);
            //else
            ////Process.Start(file);
            //sm_init();
            //sm = new Thread(new ThreadStart(sm_thread));
            //sm.IsBackground = true;

            //sm.Start();
            string mysqlcom1 = "use debug";


            string mysqlselect2 = "select * from " + "test";
        
            //查询表
            pandian1.mysqlnon(mysqlcom1);                  //切换数据库

            //选中模板表1,用于存储

            mysql1 = pandian1.mysqladapter(mysqlselect2);
           
            query_time = 0;
            dataGridView1.Rows.Clear();
            mysql1.Clear();
            MultiReadTagData();
            this.button3.Enabled = true;
            this.button2.Enabled = false;
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //loop = false;
            //if(sm!=null)
            //sm.Abort();
            //qrsm.backlight(false);
            ////蜂鸣3次
            //qrsm.beepControl(2);
            ////关闭设备连接
            //qrsm.disConnected();
            DialogResult cc =
                MessageBox.Show("您确定结束本次扫描吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (cc == DialogResult.OK)
            //dataGridView1.ClearSelection();
            {
                StopReadTagData();
                this.button2.Enabled = false;
                this.button3.Enabled = false;
                int cot = int.Parse(lxLedMultiCount.Text);
                StreamWriter wr = new StreamWriter(Application.StartupPath+@"\历史盘点信息.txt",true);
                wr.Write(lxLedMultiCount.Text+'%'+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+'#');//以个数%时间#的格式存储盘点信息
                wr.Dispose();

                lxLedControl1.Text = lxLedMultiCount.Text;
                textBox1.Text =DateTime.Now.ToString("yyyy-MM-dd HH:mm:dd");

                TagsList.Clear();
                query_time = 0;
                dataGridView1.Rows.Clear();
                mysql1.Clear();
                lxLedMultiCount.Text = "0";
                lxLedMultiTime.Text = "0";   
            }
            else
                ;
            //恢复初始化
            button4.Enabled = true;
            button1.Enabled = true ;
            borrow_flg = false;
            label2.Text = "扫描枪连接状态:";
            tidlist.Clear();
            countlist.Clear();
            mysql_serverdata.Clear();
        }

        
        public void UpdataCkStatuText(string strlog)
        {
            //textBox1.Text = "";
            //Thread.Sleep(100);
            //textBox1.Text = strlog;
            //textBox1.Refresh();
        }
        private void RequestStart()
        {
            _shouldStop = false;
        }
        private void MultiReadTagData()
        {
            int ret = 0;
            string oper_result;
            //pandian1.mysqlnon("use debug");
            pandian1.mysqlnon("delete from _pdj_data");
            multi_get_thread = new Thread(new ThreadStart(this._multi_get));
            ret = ReaderApi.StartInventoryMultiple(0x00);

            if (ret == 0)
            {
                this.RequestStart();
                this.MultiRead_timer.Enabled = true;//datagridview更新定时器100ms更新一次

                
                multi_get_thread.Start();
                
                oper_result = "启动循环查询标签成功！";
            }
            else
            {
                oper_result = "启动循环查询寻标签失败！";//Start Multi query fail.
                
            }
            textBox2.Text = oper_result;
        }
        private void updata_tags_listview()
        {
            List<DataTag> tmp_tags_list = new List<DataTag>(8000);//用于暂存TagList


            tmp_tags_list = TagsList;
           

            for (int index = 0; index < tmp_tags_list.Count; index++)
            {
                UpdataLabel(tmp_tags_list[index]);//一条一条的处理
            }
        }
        List<string> tidlist = new List<string>();
        List<int> countlist = new List<int>();
        private void UpdataLabel(DataTag tag_item)
        {
            //转换成string


            string str_epc = tag_item.Epc;
           // string str_tid = tag_item.Tid;


            string str_read_cnt = tag_item.Count.ToString();

            string str_time = tag_item.LatestTime;


            string[] arr = new string[5];
            arr[0] = (dataGridView1.RowCount + 1).ToString();//目前行数加1表示序号

            arr[1] = str_epc;

            #region  检测tid编号位置
            //way1：
            //mysql_serverdata.Tables[0].Rows.Contains(str_epc);
            //主键查询字段值
            if (!borrow_flg)
            {
                if (tidlist.Contains(str_epc))
                {

                }
                else
                {
                    tidlist.Add(str_epc);
                    object[] objs = new object[] { str_epc };
                    DataRow dr = mysql_serverdata.Tables[0].Rows.Find(objs);
                    int count_row = mysql_serverdata.Tables[0].Rows.IndexOf(dr);
                    if (count_row >= 0)
                    {
                        arr[2] = mysql_serverdata.Tables[0].Rows[count_row][4].ToString();
                        //删除检索行
                        mysql_serverdata.Tables[0].Rows.RemoveAt(count_row);
                        countlist.Add(count_row);
                        //arr[2] = dr[5].ToString();  
                    }
                    else
                    {
                        arr[2] = "未入库";
                    }
                }
            }
            else
            {
                arr[2] = "-";
            }
            #endregion

            arr[3] = str_read_cnt;

            arr[4] = str_time;

            bool Exist = false;

            //为防止下一个时间间隔进入时，不能重复添加
            foreach (DataGridViewRow dt in this.dataGridView1.Rows)//遍历dataGridView的所有行
            {
                if (dt.Cells[1].Value.ToString() == str_epc)
                {
                    dt.Cells[3].Value = str_read_cnt;
                    dt.Cells[4].Value = str_time;
                    Exist = true;
                    break;
                }
            }

            if (!Exist)
            {
                dataGridView1.Rows.Insert(dataGridView1.RowCount, arr);
                dataGridView1.AutoResizeColumns();
                dataGridView1.Rows[dataGridView1.RowCount - 1].Selected = true;
                dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
            }
            
        }
        private void MultiRead_timer_Tick(object sender, EventArgs e)
        {
            //DataGridViewRow rows = new DataGridViewRow();

            double TmpQueryTime = 0.0;
            if (query_time < 999)
            {
                TmpQueryTime = Convert.ToDouble(query_time) / Convert.ToDouble(10);
            }
            else
            {
                TmpQueryTime = query_time / 10;
            }

            ++query_time;

            TmpQueryTime = Math.Round(TmpQueryTime, 1);
            //已盘点数
            lxLedMultiCount.Text = TagsList.Count.ToString();
            //待盘点数
            //lxLedMultiTime.Text = TmpQueryTime.ToString("f1");
            if (!borrow_flg)
            {
                lxLedMultiTime.Text = mysql_serverdata.Tables[0].Rows.Count.ToString();
            }
            
            updata_tags_listview();
            /*
            if((query_time % 2) == 0)
            {
                ReaderStatus.BackColor = Color.Yellow;
            }
            else
            {
                ReaderStatus.BackColor = System.Drawing.SystemColors.Control;
            }
             */
        }
        private void _multi_get()
        {
            while (!_shouldStop)
            {
                multi_query_epc_t multi_epc = new multi_query_epc_t();
                if (!multi_get_thread.IsAlive)
                {
                    break;
                }

                int ret = ReaderApi.InventoryMultipleData(ref multi_epc);
                //Console.Write("循环查询结果：");
                //Console.WriteLine(ret);
                Multi_analyze_data(multi_epc);
                Thread.Sleep(0);
            }
        }
    
        private int tags_chongfu(string epc)
        {
            for (int index = 0; index < TagsList.Count; index++)
            {
                if ((TagsList[index].Epc == epc))
                {
                    return index;
                }
            }

            return -1;
        }
        
        //private ushort pc_calculate(byte pc_msb, byte pc_lsb)
        //{
        //    ushort temp_pc = (ushort)((((ushort)pc_msb) << 8) + (ushort)pc_lsb);

        //    return temp_pc;
        //}
        private string epc_format(byte[] epc, byte epc_len)
        {
            string str_epc = "";
            for (int index = 0; index < epc_len; index++)
            {
                str_epc += epc[index].ToString("X2");//输出大写的两位16进制
                //if (index < epc_len - 1)
                //{
                //    str_epc += "-";
                //}
            }
            return str_epc;
        }
        private void Multi_analyze_data(multi_query_epc_t tags)
        {
            //if ((tags.in_count_msb != 0) || (tags.in_count_lsb != 0) || (tags.out_count_msb != 0) || (tags.out_count_lsb != 0))
            //{
            //    global.InStatistics = pc_calculate(tags.in_count_msb, tags.in_count_lsb);
            //    global.OutStatistics = pc_calculate(tags.out_count_msb, tags.out_count_lsb);
            //}

            for (int index = 0; index < tags.packet_num; index++)
            {
                if ((tags.tags_epc[index].epc.epc_len > 124) || (tags.tags_epc[index].epc.epc_len <= 0))//舍弃掉epc长度不正常的情况
                {
                    continue;
                }

                // SetAccumulateCount();

                string temp_epc = epc_format(tags.tags_epc[index].epc.epc, tags.tags_epc[index].epc.epc_len);
                //byte ant = tags.tags_epc[index].ant_id;
               // Console.WriteLine("调试信息1：epc解析后信息"+temp_epc);
               // Console.WriteLine("调试信息2：epc的第一个字节信息" + tags.tags_epc[index].epc.epc[0]);
               // Console.WriteLine("调试信息3：epc长度" + tags.tags_epc[index].epc.epc_len.ToString());
                //Console.WriteLine("调试信息3：包总长"+tags.packet_num);
                // 判断当前标签是会否存在列表中
                int sjbz1 = 0;
                if (-1 == (sjbz1 = tags_chongfu(temp_epc)))//第一次一定会进入这里
                {
                    DataTag tmp_tag = new DataTag();//新建的临时标签,存储新的epc数据
                    tmp_tag.Count = 1;


                    tmp_tag.Epc = temp_epc;
                    //tmp_tag.Tid = tid_format(tags.tags_epc[index].tid, tags.tags_epc[index].tid_len);

                    tmp_tag.LatestTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);//以2019-01-11 18:00:00格式输出时间

                    TagsList.Add(tmp_tag);
                    mysql1.Tables[0].Rows.Add();
                    mysql1.Tables[0].Rows[mysql1.Tables[0].Rows.Count - 1][0] = mysql1.Tables[0].Rows.Count;

                    mysql1.Tables[0].Rows[mysql1.Tables[0].Rows.Count - 1][1] = temp_epc;//dataGridView1.Rows[mysql1.Tables[0].Rows.Count-1].Cells[1].Value; //t1[0].ToString();
                    mysql1.Tables[0].Rows[mysql1.Tables[0].Rows.Count - 1][2] = new MySql.Data.Types.MySqlDateTime(tmp_tag.LatestTime);//dataGridView1.Rows[mysql1.Tables[0].Rows.Count-1].Cells[3].Value;//new MySql.Data.Types.MySqlDateTime(t1[1].ToString());



                }
                else        // 标签已存在，增加count 即可
                {
                    // 写保护
              
                        TagsList[sjbz1].Count += 1;
                                                                    
                        TagsList[sjbz1].LatestTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                    //Console.WriteLine(temp_epc);
                }
                
                // mysql1.Tables[0].Rows.Add();
                //mysql1.Tables[0].Rows[mysql1.Tables[0].Rows.Count - 1][0] = mysql1.Tables[0].Rows.Count;
                //mysql1.Tables[0].Rows[mysql1.Tables[0].Rows.Count - 1][1] = dataGridView1.Rows[mysql1.Tables[0].Rows.Count-1].Cells[1].Value; //t1[0].ToString();
                //mysql1.Tables[0].Rows[mysql1.Tables[0].Rows.Count - 1][2] = dataGridView1.Rows[mysql1.Tables[0].Rows.Count-1].Cells[4].Value;//new MySql.Data.Types.MySqlDateTime(t1[1].ToString());
                //if (pandian1.mysqlupdatecomall("insert into _pdj_data set ", mysql1))
                //{
                //  dataGridView1.EndEdit();            //关闭dataGridView编辑状态
                //d1.ClearSelection();
                //}

            }
        }
        private volatile List<DataTag> TagsList = new List<DataTag>(2000);//初始容量为2000的DataTag容器数组，用于存储用于显示的所有读取信息
        private int query_time;

        //private ushort pc_calculate(byte pc_msb, byte pc_lsb)
        //{
        //    ushort temp_pc = (ushort)((((ushort)pc_msb) << 8) + (ushort)pc_lsb);

        //    return temp_pc;
        //}
        private void StopReadTagData()
        {
            string oper_result;
            try
            {
                // 1 停止循环读标签
                int ret = ReaderApi.StopInventoryMultiple();

                if (ret==0)
                {
                    oper_result = "停止循环查询成功！";//Stop Multi query success.";
                }
                else
                {
                    oper_result = "停止循环查询失败！";//Stop Multi query fail.";
                }
                //global.MultiReadStatus = false;
                //EnableFormSet();
                textBox2.Text = oper_result;

                _shouldStop = true;
                this.MultiRead_timer.Enabled = false;
                pandian1.mysqlupdateSaveAll("insert into _pdj_data values (", mysql1);
                MessageBox.Show("盘点数据存储成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
               
               // this.button5.Enabled = true;
                // Use the Join method to block the current thread 
                // until the object's thread terminates.
                //multi_get_thread.Join();
            }
            catch (Exception ex)
            {
                oper_result = "Operation Error :" + ex.Message;
                textBox2.Text = oper_result;
            }
            //MultiRead_timer.Enabled = false;
           
        }
        public void get_version()
        {
            int ret = 0;
            byte[] firm_ware = new byte[global.PACKET_128];
            ret = ReaderApi.GetFirmwareVersion(firm_ware); // 获取固件版本号
            string firm_str;
            if (global.OPER_OK == ret)
            {
                string firm_ver;
                firm_ver = "版本：V" + System.Text.Encoding.Default.GetString(firm_ware);

                //this.UpdataFrimWareLaberText(firm_ver);

                firm_str = "盘点枪连接正常";//Get Firmware success.";
            }
            else
            {
                firm_str = "获取固件版本失败!";//Get Firmware fail.";
            }
            this.UpdataCkStatuText(firm_str);
        }
       

        private void clear_data_Click(object sender, EventArgs e)
        {

            TagsList.Clear();
            dataGridView1.Rows.Clear();
            //TagsList.Clear();
        }

        private void 盘点机扫描程序_Load(object sender, EventArgs e)
        {
            this.button2.Enabled = false;
            this.button3.Enabled = false;
            ReaderApi.basic_init(101);   //初始化为串口通信模式
            uart_open_t _open = new uart_open_t("COM1", 115200);//根据用户选择好的参数进行创建一个新的串口连接
            uart_close_t _close = new uart_close_t();
            int ret = transfer.transfer_open(ref _open);//串口连接，并返回操作成功与否值
            
            if (ret == 0)
            {
                this.UpdataCkStatuText("打开串口成功！");//Open Com successful");
                this.progressBar1.Value = 50;
            }
            else
            {
                MessageBox.Show("请重新连接扫描枪再启动本程序", "警告", MessageBoxButtons.OK, MessageBoxIcon.Information);
                transfer.transfer_close(ref _close);
                this.Close();
            }
            ret = ReaderApi.Set_Buzzer(1);
            if (ret == 0)
            {
                this.UpdataCkStatuText("设置蜂鸣器成功！");//Close Com successful");
            }
            else
            {
                this.UpdataCkStatuText("设置蜂鸣器失败！");//Close Com fail");
            }
            //this.dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(new FontFamily("黑体"),12,FontStyle.Bold);
            this.dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;
            StreamReader rd = new StreamReader(Application.StartupPath+@"\历史盘点信息.txt");
            string pd_info=rd.ReadToEnd();
            string [] pd_sz1=pd_info.Split('#');
            string[] pd_sz2 = pd_sz1[pd_sz1.Length - 2].Split('%');
            lxLedControl1.Text = pd_sz2[0];
            textBox1.Text = pd_sz2[1];
            rd.Dispose();
            //this.set_button();
            Thread.Sleep(200);
            this.progressBar1.Value = 100;
            //this.get_version();
        }
        public void set_button()
        {
            this.button2.Enabled = true;
            this.button3.Enabled = false;
        }
        public void set_button2()
        {
            this.button2.Enabled = false;
            this.button3.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox3.Text =DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        #region  获取数据库数据
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            label2.Text = "数据库获取状态:";
            groupBox3.Text = "已盘点标签列表";
            //获取数据
            if (mysqldata_get("192.168.1.222"))
            {
                progressBar1.Value = 100;
                MessageBox.Show("获取数据库数据成功！请进行盘点！", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                mysql_serverdata.Tables[0].Columns.RemoveAt(0);
                mysql_serverdata.Tables[0].Columns.RemoveAt(7);
                mysql_serverdata.Tables[0].Columns[0].ColumnName = "标签号";
                mysql_serverdata.Tables[0].Columns[1].ColumnName = "在库状态";
                mysql_serverdata.Tables[0].Columns[2].ColumnName = "姓名";
                mysql_serverdata.Tables[0].Columns[3].ColumnName = "身份证号";
                mysql_serverdata.Tables[0].Columns[4].ColumnName = "存放位置";
                mysql_serverdata.Tables[0].Columns[5].ColumnName = "入库时间";
                mysql_serverdata.Tables[0].Columns[6].ColumnName = "单位名称";

                for (int i = 0; i < mysql_serverdata.Tables[0].Rows.Count; i++)
                {
                    if (mysql_serverdata.Tables[0].Rows[i][1].ToString() == "1")
                    {
                        mysql_serverdata.Tables[0].Rows[i][1] = "在 库";
                    }
                    else
                    {
                        mysql_serverdata.Tables[0].Rows[i][1] = "借 出";
                    }
                }
                //设置主键Columns[1]
                mysql_serverdata.Tables[0].PrimaryKey = new DataColumn[]
                {
                        mysql_serverdata.Tables[0].Columns[0]
                };
                dataGridView2.DataSource = mysql_serverdata.Tables[0];
                set_button();
                borrow_flg = false;
            }
            else
            {
                MessageBox.Show("获取数据库数据失败！请检查网线！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                button1.Enabled = true;
            }
            
        }

        #region    数据导出
        public List<DataTable> list_savetable = new List<DataTable>();
        public List<string> savenamestr = new List<string>();
        public bool currentflg = false;

        private void button4_Click(object sender, EventArgs e)
        {
            

        }

        #endregion

        #region  远程数据获取
        public DataSet mysql_serverdata = new DataSet();  //在库档案数据
        mysqlclass_1 mysqlconn00 = new mysqlclass_1();  //盘点mysql
        private bool mysqldata_get(string ipstr)
        {
            bool flg = false;
            List<string> list0 = new List<string>();
            string mysqlshowdatabases = "show databases";
            string mysqlcom1 = "use store";
            string mysqlselect__tablename_n = "select*from tablename where status = \"1\"";  //在库数据查询
            progressBar1.Value = 10;
            try
            {
                mysqlconn00.mysqlconnn(ipstr);    //初始化连接数据库
                progressBar1.Value = 30;
                list0 = mysqlconn00.mysqlshow2(mysqlshowdatabases);   //获取得到数据库查询
                progressBar1.Value = 50;
                if (list0.Count() != 0)
                {
                    
                    //获取在库、不在库表tablename
                    mysqlconn00.mysqlcom2(mysqlcom1);
                    progressBar1.Value = 70;
                    mysql_serverdata = mysqlconn00.mysqlselectcom2(mysqlselect__tablename_n);
                    progressBar1.Value = 90;
                    flg = true;
                    return flg;
                }
                else
                {
                    MessageBox.Show("获取数据失败，请检查网络连接！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    mysqlconn00.mysqlconn2.Dispose();
                    progressBar1.Value = 0;
                    return flg;
                }
            }
            catch
            {
                MessageBox.Show("获取数据失败，请检查网络连接！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                mysqlconn00.mysqlconn2.Dispose();
                return flg;
            }
        }
        #endregion

        #endregion

        #region  借阅出库整理核对
        public bool borrow_flg = false;
        private void button4_Click_1(object sender, EventArgs e)
        {
            button2.Enabled = true;
            button4.Enabled = false;
            borrow_flg = true;            
            MessageBox.Show("请点击开始扫描！", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            groupBox3.Text = "已扫描标签列表";
        }
        #endregion

        #region   强制退出判断
        public void 盘点机扫描程序_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (button3.Enabled==true)
            {
                if (MessageBox.Show(
                        "请结束扫描！",
                        "提示",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }
        #endregion
    }
}
