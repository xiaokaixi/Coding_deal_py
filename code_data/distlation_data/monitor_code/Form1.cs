using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;  //access
using System.Drawing;    //chart
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;   //
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Reflection;  //wav
using System.Media;   //wav


namespace SerialPort_ViewSWUST1205
{

    public partial class Form1 : Form
    {
        #region  
        SerialPort workingport1 = new SerialPort();
        public bool click = false;
        public string tablename = "table";   //新建表名
        public int num = 0,nums=0;     //ID
        public string dirpath = System.Windows.Forms.Application.StartupPath + "\\Data\\Database.accdb";  //
        public OleDbConnection conn;    //
        public OleDbConnection conns;   //
        public string connstring;      //       
        public int o = 0;  //acess
        System.Timers.Timer mytimer;
        //
        public List<string> numlist1 = new List<string>();  //list
        public List<string> numlist2 = new List<string>();  //list
        public List<string> numlist3 = new List<string>();  //list
        public DataSet mysql = new DataSet();
        public DataSet mysql_statu = new DataSet();
        //
        public DataSet mysql_tablename_n = new DataSet();  //
        public DataSet mysql_tablename_b = new DataSet();  //
        public DataSet mysql_outtable = new DataSet();  //
        public DataSet mysql_intable = new DataSet();   //
        public DataSet mysql_new = new DataSet();        //
        public DataSet mysql_transfer = new DataSet();        //
        public DataSet mysql_gatenote1 = new DataSet();  //
        //
        public string namespaceName = Assembly.GetExecutingAssembly().GetName().Name.ToString();
        //
        public Assembly assembly = Assembly.GetExecutingAssembly();
        
        #endregion

        #region  
        public Form1()
        {
            InitializeComponent();
            //
            proseting();
            //tcp
            tcpconnect();
            //
            mytimer = new System.Timers.Timer(2000);//300ms
            mytimer.Elapsed += mytimer_elapsed; //300ms
            mytimer.AutoReset = true;
            Control.CheckForIllegalCrossThreadCalls = false;   //
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        { 
            for (int j = 0; j < 9; j++)
            {
                numlist1.Add("00.0");
                numlist2.Add("00.0");
                numlist3.Add("0");
            }
            //
            timenow();
            //mysql
            mysql.Tables.Add();
            mysql.Tables[0].Columns.Add();
            mysql.Tables[0].Columns[0].ColumnName = "时间";
            mysql.Tables[0].Columns.Add();
            mysql.Tables[0].Columns[1].ColumnName = "温度(℃)";
            mysql.Tables[0].Columns.Add();
            mysql.Tables[0].Columns[2].ColumnName = "湿度(%)";
            dataGridView1.DataSource = mysql.Tables[0];
            dataGridView1.ClearSelection();
            //
            mysql_statu.Tables.Add();
            mysql_statu.Tables[0].Columns.Add();
            mysql_statu.Tables[0].Columns[0].ColumnName = "温度(℃)";
            mysql_statu.Tables[0].Columns.Add();
            mysql_statu.Tables[0].Columns[1].ColumnName = "湿度(%)";
            mysql_statu.Tables[0].Columns.Add();
            mysql_statu.Tables[0].Columns[2].ColumnName = "浸水状态";
            mysql_statu.Tables[0].Rows.Add();
            mysql_statu.Tables[0].Rows.Add();
            mysql_statu.Tables[0].Rows.Add();
            dataGridView2.DataSource = mysql_statu.Tables[0];
            dataGridView2.ClearSelection();
            //
            readsetting();
            //tcp
            datadetil();
            

        }
        #endregion

        #region    
        //

        public void timenow()
        {
            System.Timers.Timer t = new System.Timers.Timer(1);       //300ms
            t.Elapsed += new System.Timers.ElapsedEventHandler(theoutime);
            t.AutoReset = true;  //true,false
            t.Enabled = true;
        }
        public void theoutime(object source, System.Timers.ElapsedEventArgs e)
        {
            SetData1();
        }

        //,
        private delegate void SetDataDelegate1();
        public bool hnote_flg = false;
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
                    textBox1.Text = DateTime.Now.ToString();
                    if (textBox1.Text.Contains(":00:00"))
                    {
                        hnote_flg = true;
                    }   
                }
            }
            catch
            { }
        }
        #endregion

        #region  
        public string tempf,wetf,waterf;
        public void readsetting()
        {
            tempf = logincheckset("t");   //
            tl = tempf.Substring(0, 4);
            th = tempf.Substring(5,4);
            tempf=tempf.Replace("-", "-") +"";
            mysql_statu.Tables[0].Rows[2][0] = tempf;
            wetf = logincheckset("h");     //
            wl=wetf.Substring(0,4);
            wh = wetf.Substring(6,4);
            mysql_statu.Tables[0].Rows[2][1] = wetf;
            waterf = logincheckset("w");     //
            mysql_statu.Tables[0].Rows[2][2] = waterf;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string logincheckset(string name)
        {
            StreamReader xiao_str1 = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + @"seting.txt");
            string text = "";
            string text1 = "";
            while (text != null)
            {
                text = xiao_str1.ReadLine();
                if (name == text)
                {
                    name = "right";
                    continue;
                }
                if (name == "right")
                {
                    text1 = text;
                    xiao_str1.Close();
                    return text1;
                }
            }
            xiao_str1.Close();
            return text1;
        }
        #endregion

        #region  TCP
        //
        Socket socketSend= new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        byte[] buffersend = Encoding.UTF8.GetBytes("%read%192.168.1.23%");  //
        bool goflag;
        private void tcpconnect()
        {
            try
            {

                //Socketip  192.168.1.23:5000
                IPAddress ip = IPAddress.Parse("192.168.1.222");
                IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32("5000"));
                socketSend.Connect(point);
                MessageBox.Show("连接成功！", "CONNECT", MessageBoxButtons.OK, MessageBoxIcon.None);
                //
                goflag = true;
                Thread c_thread = new Thread(Received);
                c_thread.IsBackground = true;
                c_thread.Start();
                Thread.Sleep(50);
                socketSend.Send(buffersend);

            }
            catch (Exception)
            {
                MessageBox.Show("连接失败，请检查网络连接以及远端管理软件的开启！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region  tcp,
        private void Received()
        {
            string msg = "%readok%192.168.1.23%";                 //请求成功返回数据
            byte[] buffer1 = Encoding.UTF8.GetBytes(msg);
            socketSend.ReceiveTimeout = 1000;  //1s,
            //
            SoundPlayer sp = new SoundPlayer(assembly.GetManifestResourceStream("manage.Resources.warning.wav"));
            do
            {
                if (data_get)
                { continue; }
                try
                {
                    byte[] buffer = new byte[1024 * 1024 * 3];
                    //
                    int len = socketSend.Receive(buffer);
                    if (len == 0)
                    {
                        //break;
                    }
                    if (len != 0)
                    {
                        string str = Encoding.UTF8.GetString(buffer, 0, len);
                        var str2 = str.Split(new char[1] { '%' });    //
                        //numlist1[0] = str2[1];  //
                        numlist1[0]=(float.Parse(str2[1])).ToString("#0.0"); 
                        //numlist1[1] = str2[2];  // 
                        numlist1[1] = (float.Parse(str2[2])).ToString("#0.0");
                        numlist1[2] = str2[3];  //
                        cominnum = int.Parse(str2[4]);      //
                        gooutnum = int.Parse(str2[5]);      //
                        warningnum = int.Parse(str2[6]);    //
                        borrownum = int.Parse(str2[7]);     //
                        borandretnum = int.Parse(str2[8]);  //
                        should_borrownum = int.Parse(str2[9]);  //
                        count = int.Parse(str2[10]);            //
                        controlsum = int.Parse(str2[11]);       //
                        pandiansum = int.Parse(str2[12]);       //
                        pandinawarning = int.Parse(str2[13]);   //
                        warning= int.Parse(str2[14]);           //
                        transfernum = int.Parse(str2[15]);      //
                        serverok = str2[16];
                        if (warning == 1)
                        {    
                            sp.Play();
                        }
                        else
                        {
                            sp.Stop();
                        }
                        Thread.Sleep(50);
                        socketSend.Send(buffer1);
                    }
                    Thread.Sleep(200);
                    socketSend.Send(buffersend);
                }
                catch
                {                   
                    try
                    {
                        Thread.Sleep(100);
                        socketSend.Send(buffersend);
                    }
                    catch
                    {
                        if (button4.Text == "停止监测")
                        {
                            serverok = "notok";
                            MessageBox.Show("主控计算机已断开，请重新启动监测软件！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                            this.Close();
                        }

                    }
                    
                }
            }
            while (goflag);
        }
        #endregion

        #region  
        private void datadetil()
        {
            try
            {
                ThreadStart timetoshowstart = new ThreadStart(timetoshow1);
                Thread timetoshowthread = new Thread(timetoshowstart);
                timetoshowstart();
            }
            catch
            {

            }
        }
        //
        private void timetoshow1()
        {
            System.Timers.Timer t1 = new System.Timers.Timer(500);       //300ms
            t1.Elapsed += new System.Timers.ElapsedEventHandler(workingport1_receive);
            t1.AutoReset = true;  //true,false
            t1.Enabled = true;
        }
        #endregion

        #region   
        public void proseting()
        {
            List<string> list = new List<string>();
            string[] workingport = SerialPort.GetPortNames();
            string dirpath = System.Windows.Forms.Application.StartupPath + "\\Data\\Database.accdb"; 
            AccessDbclass mybd = new AccessDbclass(dirpath,null);
            tablename = mybd.buildtable(tablename);//
            dirpath = System.Windows.Forms.Application.StartupPath + "\\Data\\Database_24h.accdb";            
            AccessDbclass mybd1 = new AccessDbclass(dirpath,null);
            tablename = mybd1.buildtables(tablename);//            
            string dirpath1 = System.Windows.Forms.Application.StartupPath + "\\Data\\Database.accdb";
            AccessDbclass mydb = new AccessDbclass(dirpath1,null);    //
            System.Data.DataTable dt = new System.Data.DataTable();
            System.Data.DataTable dt1 = new System.Data.DataTable();
            System.Data.DataTable dts = new System.Data.DataTable();
            string sql = "select*from chushitable";
            string sql1 = "select*from "+tablename;           
            dt = mydb.selettodatatable(sql);
            dt1 = mydb.selettodatatable(sql1);
            dts = mydb.selettodatatable(sql1);
            //id
            num = dt1.Rows.Count;
            chart1.DataSource = dt;
            chart2.DataSource = dt;
            //
            chart1.Series.Add("serial2");
            Drawclass.chart_drawset1(dt,chart1);
            //
            chart2.Series.Add("serial3");
            Drawclass.chart_drawset2(dt, chart2);
            //combox
            list = mydb.gettablenames();
            
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ToString() == "table0") { continue; }
                if (comboBox2.Items.Contains(list[i].ToString().Substring(11, 2)))
                { continue; }
                comboBox2.Items.Add(list[i].ToString().Substring(11, 2));
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ToString() == "table0") {  continue; }
                if (comboBox1.Items.Contains(list[i].ToString().Substring(5, 6)))
                { continue; }
                comboBox1.Items.Add(list[i].ToString().Substring(5, 6));                
            }
            //
            connstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+ System.Windows.Forms.Application.StartupPath + "\\Data\\Database.accdb;Persist Security Info=False;";
            conn = new OleDbConnection(connstring);
            conn.Open(); o = 1;
            //
            connstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + System.Windows.Forms.Application.StartupPath + "\\Data\\Database_24h.accdb;Persist Security Info=False;";
            conns = new OleDbConnection(connstring);
            conns.Open(); o = 1;
            
        }

        private void proseting1()
        {
            List<string> list = new List<string>();
            string dirpath = System.Windows.Forms.Application.StartupPath + "\\Data\\Database.accdb";
            AccessDbclass mybd = new AccessDbclass(dirpath, null);
            string name = mybd.gettablename();
            tablename = mybd.buildtable(tablename);//
            dirpath = System.Windows.Forms.Application.StartupPath + "\\Data\\Database_24h.accdb";
            AccessDbclass mybd1 = new AccessDbclass(dirpath, null);
            tablename = mybd1.buildtables(tablename);//
            string dirpath1 = System.Windows.Forms.Application.StartupPath + "\\Data\\Database.accdb";
            AccessDbclass mydb = new AccessDbclass(dirpath1, null);    //
            System.Data.DataTable dt = new System.Data.DataTable();
            System.Data.DataTable dt1 = new System.Data.DataTable();
            System.Data.DataTable dts = new System.Data.DataTable();
            string sql = "select*from chushitable";
            string sql1 = "select*from " + tablename;
            int tablenum = 0;
            string dropname = "";
            dt = mydb.selettodatatable(sql);
            dt1 = mydb.selettodatatable(sql1);
            dts = mydb.selettodatatable(sql1);
            //id
            num = dt1.Rows.Count;
            //
            chart1.DataSource = dt;
            Drawclass.chart_drawset1(dt, chart1);
            //
            chart2.DataSource = dt;
            Drawclass.chart_drawset2(dt, chart2);
            list = mydb.gettablenames();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ToString() == "table0") { dropname = list[i + 1].ToString(); continue; }
                if (comboBox2.Items.Contains(list[i].ToString().Substring(11, 2)))
                { tablenum++; continue; }
                comboBox2.Items.Add(list[i].ToString().Substring(11, 2));
                tablenum++;
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ToString() == "table0") { ; continue; }
                if (comboBox1.Items.Contains(list[i].ToString().Substring(5, 6)))
                { continue; }
                comboBox1.Items.Add(list[i].ToString().Substring(5, 6));               
            }
            
            conn.Close();
            conns.Close();
            //
            connstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + System.Windows.Forms.Application.StartupPath + "\\Data\\Database.accdb;Persist Security Info=False;";
            conn = new OleDbConnection(connstring);
            conn.Open();
            //
            connstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + System.Windows.Forms.Application.StartupPath + "\\Data\\Database_24h.accdb;Persist Security Info=False;";
            conns = new OleDbConnection(connstring);
            conns.Open();
            //2()
            if (tablenum == 730)
            {
                string sql2 = "drop table " + dropname;
                mydb.selecttodatasets(sql2);
                comboBox2.Items.Remove(dropname.Substring(11, 2));
            }
            string sql3 = "drop table " + name;
            mydb.selecttodataset(sql3);

        }

        #endregion

        #region   tcp
        private List<byte> buffer = new List<byte>(4096);  //buf
        //16
        public bool flag = false, fg = false, insertfg = false;
        public int viewcount = 0;
        public string time,htime = "123 233:0";
        public int okcount=0;
        float x = (float)0.1;

        private void workingport1_receive(object source, System.Timers.ElapsedEventArgs e)
        {
            if (flag||float.Parse(numlist1[0]) > 90|| float.Parse(numlist1[1]) > 90|| DateTime.Now.ToString("G")==time|| float.Parse(numlist1[0]) <=0|| float.Parse(numlist1[1]) <= 0|| serverok=="notok")
            { return; }
            flag = true;
            //buffer.Clear();
            ////codes            
            //if (x < 10)
            //{
            //    if (Math.Sin(x) * 30 + 30 < 10)
            //    {
            //        numlist1[0] = "0" + (Math.Sin(x) * 30+30).ToString("#0.0");
            //    }
            //    else
            //    {
            //        numlist1[0] = (Math.Sin(x) * 30+30).ToString("#0.0");
            //    }
            //    if (Math.Sin(Math.Sin(x) * 30 + 30) +30< 10)
            //    {
            //        numlist1[1] = "0" + (Math.Sin(Math.Sin(x) * 30 + 30)+30).ToString("#0.0");
            //    }
            //    else
            //    {
            //        numlist1[1] = (Math.Sin(Math.Sin(x) * 30 + 30)+30).ToString("#0.0");
            //    }
            //    numlist1[2] = 0.ToString();
            //    x = x + (float)2;
            //}
            //else
            //{ x = 0; }

            System.Diagnostics.Stopwatch otime = new System.Diagnostics.Stopwatch();
            otime.Start();

            string str = "#" + numlist1[0];
            int nn = str.Length;     //,
            int n = str.Length;  //
            byte[] buf = new byte[n];          //buf
            byte jyw = 0;                      //
           

            for (int i = 0; i < str.Length; i++)
            {
                buf[i]= Convert.ToByte(str[i]);
            }
            buffer.AddRange(buf);              //buffererror

            if (buffer[0] == '#')         //
            {
                string buffer1 = Encoding.ASCII.GetString(buf, 0, nn);
                if (buffer.Count == 2)
                { buffer[3] = Convert.ToByte("."); buffer[4] = Convert.ToByte("0"); }
                //float x1 = Convert.ToSingle((buffer[1] - 48) * 10 + (buffer[2] - 48) + (buffer[4] - 48) * 0.1);
                float x1 = float.Parse(numlist1[0]);   //
                buffer.RemoveRange(0, nn);
                string y = "0";
                string z = "0";
                float y1 = float.Parse(numlist1[1]);  //
                float z1 = float.Parse(numlist1[2]);  //
                time = System.DateTime.Now.ToString("G");
                var strfen = time.Split(new char[1] { ' ' });
                string time_show_s = strfen[1];
                buffer1 = "";

                OleDbDataAdapter adapter = new OleDbDataAdapter();
                try
                {
                    //
                    insertfg = true;
                    string sql = "insert into " + tablename + "([ID],[Time],[Time_show],[temperature],[humidity])values('" + num + "','" + time + "','" + time_show_s + "','" + x1 + "','" + y1 + "')";
                    OleDbCommand command = new OleDbCommand(sql, conn);
                    command.ExecuteNonQuery();   //SQL
                    command.Dispose();
                    insertfg = false;
                    num++;
                }
                catch
                {
                    int m = 0;
                }
                var mm = time_show_s.Split(new char[1] { ':' });

                if (mm[1]=="00"&& mm[2] == "00"||hnote_flg==true)
                {
                    //
                    insertfg = true;
                    string hf = time.Split(new char[2] { ' ', ':' })[1];
                    if (hf == htime.Split(new char[2] {' ', ':' })[1])
                    { flag = false; hnote_flg = false; return; }
                    hnote_flg = false;
                    time = DateTime.Now.ToString().Split(new char[1] { ' '})[0]+" "+ hf + ":00:00";
                    string sqls = "insert into " + tablename + "([ID],[Time],[Time_show],[temperature],[humidity])values('" + nums + "','" + time + "','" + time_show_s + "','" + x1 + "','" + y1 + "')";
                    OleDbCommand commands = new OleDbCommand(sqls, conns);
                    commands.ExecuteNonQuery();   //SQL
                    commands.Dispose();
                    htime = time;
                    insertfg = false;
                    nums++;
                }
                if (htime == time&& time.Split(new char[2] { ' ', ':' })[1] == "0")
                {
                    mytimer.Stop();
                    //0
                    proseting1();
                    mytimer.Start();
                    nums = 0;
                    //
                    insertfg = true;
                    string sqls = "insert into " + tablename + "([ID],[Time],[Time_show],[temperature],[humidity])values('" + nums + "','" + time + "','" + time_show_s + "','" + x1 + "','" + y1 + "')";
                    OleDbCommand commands = new OleDbCommand(sqls, conns);
                    commands.ExecuteNonQuery();   //SQL
                    commands.Dispose();
                    insertfg = false;
                    nums++;
                }
                ////
                //SoundPlayer sp = new SoundPlayer(assembly.GetManifestResourceStream("manage.Resources.warning.wav"));
                //               
                if (viewcount == 4)
                {
                    mysql_statu.Tables[0].Rows[0][0] = x1.ToString();  //
                    mysql_statu.Tables[0].Rows[0][1] = y1.ToString();  //
                    mysql_statu.Tables[0].Rows[0][2] = z1.ToString();  //
                    if (x1 > float.Parse(th) || x1 < float.Parse(tl))
                    {
                        mysql_statu.Tables[0].Rows[1][0] = "异常";
                        dataGridView2.Rows[1].Cells[0].Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
                    }
                    else
                    {
                        mysql_statu.Tables[0].Rows[1][0] = "正常";
                        dataGridView2.Rows[1].Cells[0].Style.BackColor = Color.DimGray;
                    }
                    if (y1 > float.Parse(wh) || y1 < float.Parse(wl))
                    {
                        mysql_statu.Tables[0].Rows[1][1] = "异常";
                        dataGridView2.Rows[1].Cells[1].Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
                    }
                    else
                    {
                        mysql_statu.Tables[0].Rows[1][1] = "正常";
                        dataGridView2.Rows[1].Cells[1].Style.BackColor = Color.DimGray;
                    }
                    if (z1 == 0)
                    {
                        mysql_statu.Tables[0].Rows[1][2] = "正常";
                        dataGridView2.Rows[1].Cells[2].Style.BackColor = Color.DimGray;
                    }
                    else
                    {
                        mysql_statu.Tables[0].Rows[1][2] = "异常";
                        dataGridView2.Rows[1].Cells[2].Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));

                    }
                    viewcount = 0;
                }
                viewcount++;
            }
            else
            {
                buffer.RemoveAt(0);        //
            }
            otime.Stop();
            //string s=otime.Elapsed.Seconds+""+otime.Elapsed.Milliseconds+"";
            //MessageBox.Show(s);
            flag = false;
        }
        #endregion

        #region   
        public string tablenamestr = "";  //查询表名公共变量
        public System.Data.DataTable viewdata = new System.Data.DataTable();
        private void button3_Click(object sender, EventArgs e)
        {
            DataSet mysql_show = new DataSet();
            string dirpath1 = System.Windows.Forms.Application.StartupPath + "\\Data\\Database_24h.accdb";
            AccessDbclass mydb = new AccessDbclass(dirpath1,this);//;//"C:/data.mdb");    //           
            //SQLdatatable,dataGridView
            if (comboBox1.Text == "" || comboBox2.Text == "")
            {
                MessageBox.Show("请输入查询时间！", "Warrning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            tablenamestr = "table" + comboBox1.Text + comboBox2.Text;
            string sql = "select*from " + tablenamestr;
            viewdata = mydb.selettodatatables(sql);
            if (viewdata == null)
            {
                return;
            }
            viewdata.Columns.RemoveAt(0);
            viewdata.Columns.RemoveAt(1);
            viewdata.Columns[0].ColumnName= "时间";
            viewdata.Columns[1].ColumnName= "温度(℃)";
            viewdata.Columns[2].ColumnName = "湿度(%)";           
            dataGridView1.DataSource = viewdata;
            dataGridView1.Columns[0].Width = 130;
            dataGridView1.Columns[1].Width = 75;
            dataGridView1.Columns[2].Width = 75;
            dataGridView1.ClearSelection();

            //
            mydb.close();
        }
        #endregion

        #region   
        //
        public bool viewflag=false;   //


        void mytimer_elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (insertfg == true)
            {
                return;
            }
            string dirpath1 = System.Windows.Forms.Application.StartupPath + "\\Data\\Database.accdb";
            AccessDbclass mydb = new AccessDbclass(dirpath1,null);    //
            //string sql = "select*from table2 order by Time,y";
            string sql = "";
            if (num > 62)
            {
                sql = "select*from " + tablename + " where ID >" + (num - 62).ToString();
            }
            else
            {
                sql = "select*from " + tablename;
            }
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = mydb.selettodatatable(sql);
            if (dt.Rows.Count> 0)
            {
                chart1.DataSource = dt;
                Drawclass.chart_drawset1(dt, chart1);
                label8.Text = "一分钟温湿度数据实时监测图（时间：" + DateTime.Now.ToString("d") + "）";
            }
            if (viewflag == false)
            {
                sql = "select*from " + tablename;
                dt = mydb.selettodatatables(sql);
                chart2.DataSource = dt;
                nums = dt.Rows.Count;
                if (nums > 0)
                {
                    Drawclass.chart_drawset2(dt, chart2);
                }
                else
                {
                    sql = "select*from " + "chushitable";
                    dt = mydb.selettodatatable(sql);
                    chart2.DataSource = dt;
                    Drawclass.chart_drawset2(dt, chart2);
                }
                label9.Text = "24小时温湿度数据实时监测图（时间：" + DateTime.Now.ToString("d") + "）";
            }
            dt.Dispose();  //
            mydb.close();            
        }
        #endregion

        #region   
        //access
        private void button4_Click(object sender, EventArgs e)
        {
            if (!mytimer.Enabled)
            {
                mytimer.Enabled = true;//
                button4.Text = "停止监测";
            }
            else 
            {
                mytimer.Enabled = false;
                button4.Text = "开始监测";
            }
        }
        #endregion

        #region   
        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "数据预览")
            {
                string dirpath1 = System.Windows.Forms.Application.StartupPath + "\\Data\\Database.accdb";
                AccessDbclass mydb = new AccessDbclass(dirpath1,this);    //
                if (comboBox1.Text == "" || comboBox2.Text == "")
                {
                    MessageBox.Show("请输入查询时间！", "Warrning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                viewflag = true;
                tablenamestr = "table" + comboBox1.Text + comboBox2.Text;
                string sql = "select*from " + tablenamestr;
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = mydb.selettodatatables(sql);
                if (dt == null)
                {
                    return;
                }
                
                if (dt.Rows.Count > 0)
                {
                    chart2.DataSource = dt;
                    Drawclass.chart_drawset2(dt, chart2);
                    label9.Text = "24小时温湿度数据实时监测图（时间：" + dt.Rows[0][1].ToString().Split(new char[] { ' ' })[0].ToString() + "）";
                }               
                dt.Dispose();  //
                mydb.close();
                button2.Text = "退出预览";
            }
            else
            {
                viewflag = false;
                comboBox1.Text = "";
                comboBox2.Text = "";
                button2.Text = "数据预览";
            }
        }
        #endregion

        #region  
        public string th, tl, wh, wl;
        private void button1_Click(object sender, EventArgs e)
        {           
            form_setting fsetting = new form_setting(th,tl,wh,wl);
            fsetting.ShowDialog();
            readsetting();
        }
        #endregion

        #region    
        public List<DataTable> list_savetable = new List<DataTable>();
        public List<string> savenamestr = new List<string>();
        public bool currentflg=false;
        private void button5_Click(object sender, EventArgs e)
        {
            string foldPath = "";
            string sql = "";
            string dirpath1 = System.Windows.Forms.Application.StartupPath + "\\Data\\Database_24h.accdb";
            AccessDbclass mydb = new AccessDbclass(dirpath1, this);  // 
            //datatable
            list_savetable.Clear();
            savenamestr.Clear();
            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                for (int j = 0; j < comboBox2.Items.Count; j++)
                {
                    tablenamestr = "table" + comboBox1.Items[i].ToString() + comboBox2.Items[j].ToString();
                    sql = "select*from " + tablenamestr;
                    viewdata = mydb.save_checking(sql);
                    if (viewdata != null)
                    {
                        viewdata.Columns.RemoveAt(0);
                        viewdata.Columns.RemoveAt(1);
                        viewdata.Columns[0].ColumnName = "时间";
                        viewdata.Columns[1].ColumnName = "温度(℃)";
                        viewdata.Columns[2].ColumnName = "湿度(%)";
                        list_savetable.Add(viewdata);
                        savenamestr.Add(comboBox1.Items[i].ToString() + comboBox2.Items[j].ToString());
                    }
                }
            }

            form_seletsave selectf = new form_seletsave(this);
            selectf.ShowDialog();

            if (selectf.select_rw == true)
            {

                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "请选择保存文件路径";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    foldPath = dialog.SelectedPath;
                }
                else
                {
                    return;
                }
                form_save fsave = new form_save(this, foldPath,"-"+DateTime.Now.ToString().Split(new char[1] { ' '})[0]);
                fsave.ShowDialog();
            }
            else
            {
                return;
            }
        }

        #endregion

        #region   
        //cominnum:, gooutnum:, warningnum:, borrownum:, borandretnum:, should_borrownum:,count:controlsum,pandiansumpandinawarning;
        public int cominnum = 0, gooutnum = 0, warningnum = 0, borrownum = 0, borandretnum = 0, should_borrownum = 0, count = 0, controlsum = 0, pandiansum = 0, pandinawarning = 0, warning = 0, transfernum = 0;
        public string serverok;
        public bool data_get=false;
        private void button6_Click(object sender, EventArgs e)
        {
            data_get = true;
            form_remotedata fredata = new form_remotedata(this,cominnum,gooutnum , warningnum , borrownum , borandretnum , should_borrownum, count,  controlsum, pandiansum, pandinawarning, transfernum);
            fredata.ShowDialog();
        }
        #endregion

    }

    #region  ACCESS
    public class AccessDbclass
    {
        public OleDbConnection conn;    //
        public OleDbConnection conns;   //
        public string connstring;      //
        public Form1 f1;

        //
        public AccessDbclass(string dbpath,Form1 f11)
        {
            f1 = f11;
            try
            {
                if (conn.State.ToString() == "Open" && conns.State.ToString() == "Open")
                {
                    conn.Close();
                    conns.Close();
                }
                connstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + dbpath + ";Persist Security Info=False;";   //存在版本问题，Provider=Microsoft.Jet.OLEDB.4.0安装问题在成程序错误
                conn = new OleDbConnection(connstring);
                conn.Open();
                string dbpaths = System.Windows.Forms.Application.StartupPath + "\\Data\\Database_24h.accdb";
                string connstring1 = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + dbpaths + ";Persist Security Info=False;";   //存在版本问题，Provider=Microsoft.Jet.OLEDB.4.0安装问题在成程序错误
                conns = new OleDbConnection(connstring1);
                conns.Open();
            }
            catch
            {
                connstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + dbpath + ";Persist Security Info=False;";   //存在版本问题，Provider=Microsoft.Jet.OLEDB.4.0安装问题在成程序错误
                conn = new OleDbConnection(connstring);
                conn.Open();
                string dbpaths = System.Windows.Forms.Application.StartupPath + "\\Data\\Database_24h.accdb";
                string connstring1 = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + dbpaths + ";Persist Security Info=False;";   //存在版本问题，Provider=Microsoft.Jet.OLEDB.4.0安装问题在成程序错误
                conns = new OleDbConnection(connstring1);
                conns.Open();
            }
        }

        //
        public OleDbConnection dbconn()
        {
            conn.Open();
            return conn;
        }

        //
        public void close()
        {
            conn.Close();    //conn
            conn.Dispose();  //connnew
        }

        //dataGridView-
        public System.Data.DataTable selettodatatable(string sql)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            OleDbDataAdapter adapter = new OleDbDataAdapter();  //dataset Fill  DataSet  Update  DataSet 
            try
            {
                OleDbCommand command = new OleDbCommand(sql, conn);  //SQL
                adapter.SelectCommand = command;
                adapter.Fill(dt);
                adapter.Dispose();
                return dt;
            }
            catch
            {
                adapter.Dispose();
                string d = dt.Rows[0][0].ToString();
                return dt;
            }
        }

        //dataGridView-
        public System.Data.DataTable selettodatatables(string sql)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            OleDbDataAdapter adapter = new OleDbDataAdapter();  //dataset Fill  DataSet  Update  DataSet 
            try
            {
                OleDbCommand command = new OleDbCommand(sql, conns);  //SQL
                adapter.SelectCommand = command;
                adapter.Fill(dt);
                adapter.Dispose();
                return dt;
            }
            catch
            {
                adapter.Dispose();
                if (f1.comboBox1.Text != "" || f1.comboBox2.Text != "")
                {
                    MessageBox.Show("查询失败，未查询到该日期数据，请重新选择查询日期！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }
                return dt;
            }
        }

        //
        public System.Data.DataTable save_checking(string sql)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            OleDbDataAdapter adapter = new OleDbDataAdapter();  //dataset Fill  DataSet  Update  DataSet 
            try
            {
                OleDbCommand command = new OleDbCommand(sql, conns);  //SQL
                adapter.SelectCommand = command;
                adapter.Fill(dt);
                adapter.Dispose();
                return dt;
            }
            catch
            {
                adapter.Dispose();
                return null;
            }
        }

        //
        public void selecttodataset(string sql)
        {
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            OleDbCommand command = new OleDbCommand(sql, conn);
            command.ExecuteNonQuery();   //SQL 
            command.Dispose();  
        }
        //
        public void selecttodatasets(string sql)
        {
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            OleDbCommand command = new OleDbCommand(sql, conns);
            command.ExecuteNonQuery();   //SQL 
            command.Dispose();
        }

        //
        public void writetable(string sql)
        {
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            OleDbCommand command = new OleDbCommand(sql, conn);
            command.ExecuteNonQuery();   //SQL
            command.Dispose();
        }

        //
        public string buildtable(string tablename)
        {
            tablename = "table";
            string tableid = DateTime.Now.ToString("u").Replace("-", "").Split(new char[1] { ' ' })[0];
            tablename = tablename + tableid;
            bool b = false, a = true;
            while (a)
            {
                try
                {
                    string sql = "select * from " + tablename;
                    selettodatatable(sql);
                }
                catch     //
                {
                    b = true;
                }
                if (b == true) { a = false; }
                else { b = false; break; }
            }
            if (b == false) { return tablename; }

            //
            string sql1 = "create table " + tablename + " ([ID] int primary key,[Time] char(20),[Time_show] char(15),[temperature] float,[humidity] float)";
            selecttodataset(sql1);
            close();
            return tablename;
        }

        //
        public string buildtables(string tablename)
        {           
            bool b = false, a = true;
            while (a)
            {
                try
                {
                    string sql = "select * from " + tablename;
                    selettodatatables(sql);
                }
                catch     //
                {
                    b = true;
                }
                if (b == true) { a = false; }
                else { b = false; break; }
            }
            if (b == false) { return tablename; }            
            //
            string sql1 = "create table " + tablename + " ([ID] int primary key,[Time] char(20),[Time_show] char(15),[temperature] float,[humidity] float)";
            selecttodataset(sql1);
            close();
            return tablename;
        }

        //-
        public string gettablename()
        {
            string tablname1=null;
            List<string> list = new List<string>();
            System.Data.DataTable shemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            foreach (DataRow dr in shemaTable.Rows)
            {
                list.Add(dr["TABLE_NAME"].ToString());
            }
            foreach (string str in list)
            {
                if (str.Contains("TMP") || str.Contains("chushi"))
                {
                    continue;
                }
                else
                {
                    tablname1 = str;
                }
            }
            return tablname1;
        }

        //-
        public List<string> gettablenames()
        {
            bool flag = false;
            List<string> list = new List<string>();
            System.Data.DataTable shemaTable = conns.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            foreach (DataRow dr in shemaTable.Rows)
            {
                if (dr["TABLE_NAME"].ToString() != "table0"&& flag==false)
                {  continue; }
                flag = true;
                list.Add(dr["TABLE_NAME"].ToString());
            }
            return list;
        }
    }
    #endregion

    #region    chart
    public class Drawclass
    {
        //
        /// <summary>
        /// chart
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="chart"></param>
        public static void chart_drawset1(System.Data.DataTable dt, System.Windows.Forms.DataVisualization.Charting.Chart chart)
        {
            try
            {               
                //
                chart.Series[0].XValueMember = dt.Columns[2].ColumnName;
                chart.Series[0].YValueMembers = dt.Columns[3].ColumnName;
                //chart.Series[0].Points.DataBindXY(listx, listy);  //listx,listy
                //chart.Series[0].Points.DataBindY(listy);
                //
                chart.Series[0].MarkerColor = Color.Gold;
                //
                //chart.Series[0].ChartType = SeriesChartType.Spline;  //
                chart.Series[0].ChartType = SeriesChartType.Line;
                chart.Series[0].LegendText = "温 度(℃)";
                //
                chart.Series[0].MarkerStyle = MarkerStyle.Circle;
                chart.Series[0].MarkerSize = 5;
                //
                chart.Series[0].Color=Color.Gold;
                //
                chart.Series[0].BorderWidth = 4;
                //chart.Series[0].CustomProperties = "PointWidth=4";
                //
                chart.Series[0].IsValueShownAsLabel = false;

                //chart
                //chart.ChartAreas[0].Position.Width = 90;
                //chart.ChartAreas[0].Position.Height = 100;
                //
                chart.ChartAreas[0].CursorX.IsUserEnabled = true;
                chart.ChartAreas[0].CursorX.AutoScroll = true;
                chart.ChartAreas[0].CursorX.IsUserSelectionEnabled  = true;
                chart.ChartAreas[0].CursorY.IsUserEnabled  = true;
                chart.ChartAreas[0].CursorY.LineColor = Color.DodgerBlue;
                chart.ChartAreas[0].CursorY.LineWidth = 2;
                //x
                //chart.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
                //
                chart.ChartAreas[0].AxisX.ScrollBar.Enabled = false;
                //
                //chart.ChartAreas[0].AxisX.ScaleView.SmallScrollSize = double.NaN;
                //chart.ChartAreas[0].AxisX.ScaleView.SmallScrollMinSize = 0;              
                //
                chart.ChartAreas[0].AxisX.Interval = 1;
                //
                //chart.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
                chart.ChartAreas[0].AxisX.MajorGrid.LineColor=System.Drawing.Color.Silver;
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Silver;
                //chart.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Green;
                
                //chart.ChartAreas[0].AxisX.LabelStyle.Format = "";
                //chart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
                //chart.ChartAreas[0].AxisX.IsLabelAutoFit = true;
                //x
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = 90;
                chart.ChartAreas[0].AxisY.LabelStyle.Angle = 4;
                //x,y
                //chart.ChartAreas[0].AxisX.Title = "时间(s)";
                //chart.ChartAreas[0].AxisY.Title = "温湿度(℃/%)";
                //chart.ChartAreas[0].AxisX.IsStartedFromZero =false;  //x
                chart.ChartAreas[0].AxisX.ScaleView.Size = 60;      //60
                chart.ChartAreas[0].AxisX.ScaleView.Scroll(ScrollType.Last);
                double max =80;
                double min =0;

                chart.ChartAreas[0].AxisY.Maximum = max;
                chart.ChartAreas[0].AxisY.Minimum = min;
                chart.ChartAreas[0].AxisY.Interval = (max - min) / 10;
                


                //
                chart.Series[1].XValueMember = dt.Columns[2].ColumnName;
                chart.Series[1].YValueMembers = dt.Columns[4].ColumnName;
                //chart.Series[0].Points.DataBindXY(listx, listy);  //listx,listy
                //chart.Series[0].Points.DataBindY(listy);
                //
                chart.Series[1].MarkerColor = Color.Lime;
                //
                //chart.Series[0].ChartType = SeriesChartType.Spline;  //
                chart.Series[1].ChartType = SeriesChartType.Line;
                chart.Series[1].LegendText = "湿 度(%)";

                //
                chart.Series[1].MarkerStyle = MarkerStyle.Circle;
                chart.Series[1].MarkerSize = 5;
                //
                chart.Series[1].Color = Color.Lime;
                //
                chart.Series[1].BorderWidth = 4;
                //chart.Series[0].CustomProperties = "PointWidth=4";
                //
                chart.Series[1].IsValueShownAsLabel = false;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public static void chart_drawset2(System.Data.DataTable dt, System.Windows.Forms.DataVisualization.Charting.Chart chart)
        {
            try
            {
                //
                chart.Series[0].XValueMember = dt.Columns[2].ColumnName;
                chart.Series[0].YValueMembers = dt.Columns[3].ColumnName;
                //chart.Series[0].Points.DataBindXY(listx, listy);  //listx,listy
                //chart.Series[0].Points.DataBindY(listy);
                //
                chart.Series[0].MarkerColor = Color.Gold;
                //
                //chart.Series[0].ChartType = SeriesChartType.Spline;  //
                chart.Series[0].ChartType = SeriesChartType.Line;
                chart.Series[0].LegendText = "温 度(℃)";
                //
                chart.Series[0].MarkerStyle = MarkerStyle.Circle;
                chart.Series[0].MarkerSize = 5;
                //
                chart.Series[0].Color = Color.Gold;
                //
                chart.Series[0].BorderWidth = 4;
                //
                chart.Series[0].LabelForeColor = Color.Silver;
                //chart.Series[0].CustomProperties = "PointWidth=4";
                //
                chart.Series[0].IsValueShownAsLabel = true;

                //chart
                //chart.ChartAreas[0].Position.Width = 90;
                //chart.ChartAreas[0].Position.Height = 100;
                //
                chart.ChartAreas[0].CursorX.IsUserEnabled = true;
                chart.ChartAreas[0].CursorX.AutoScroll = true;
                chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                chart.ChartAreas[0].CursorY.IsUserEnabled = true;
                chart.ChartAreas[0].CursorY.LineColor = Color.DodgerBlue;
                chart.ChartAreas[0].CursorY.LineWidth = 2;
                //x
                //chart.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
                //
                chart.ChartAreas[0].AxisX.ScrollBar.Enabled = false;
                //
                //chart.ChartAreas[0].AxisX.ScaleView.SmallScrollSize = double.NaN;
                //chart.ChartAreas[0].AxisX.ScaleView.SmallScrollMinSize = 0;              
                //
                chart.ChartAreas[0].AxisX.Interval = 1;
                //
                //chart.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
                chart.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Silver;
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Silver;
                //chart.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Green;

                //chart.ChartAreas[0].AxisX.LabelStyle.Format = "";
                //chart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
                //chart.ChartAreas[0].AxisX.IsLabelAutoFit = true;
                //x
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = 90;
                chart.ChartAreas[0].AxisY.LabelStyle.Angle = 4;
                //x,y
                //chart.ChartAreas[0].AxisX.Title = "时间(s)";
                //chart.ChartAreas[0].AxisY.Title = "温湿度(℃/%)";
                //chart.ChartAreas[0].AxisX.IsStartedFromZero =false;  //x
                chart.ChartAreas[0].AxisX.ScaleView.Size = 24;      //24
                chart.ChartAreas[0].AxisX.ScaleView.Scroll(ScrollType.Last);
                double max = 80;
                double min = 0;

                chart.ChartAreas[0].AxisY.Maximum = max;
                chart.ChartAreas[0].AxisY.Minimum = min;
                chart.ChartAreas[0].AxisY.Interval = (max - min) / 10;



                //
                chart.Series[1].XValueMember = dt.Columns[2].ColumnName;
                chart.Series[1].YValueMembers = dt.Columns[4].ColumnName;
                //chart.Series[0].Points.DataBindXY(listx, listy);  //listx,listy
                //chart.Series[0].Points.DataBindY(listy);
                //
                chart.Series[1].MarkerColor = Color.Lime;
                //
                //chart.Series[0].ChartType = SeriesChartType.Spline;  //
                chart.Series[1].ChartType = SeriesChartType.Line;
                chart.Series[1].LegendText = "湿 度(%)";

                //
                chart.Series[1].MarkerStyle = MarkerStyle.Circle;
                chart.Series[1].MarkerSize = 5;
                //
                chart.Series[1].Color = Color.Lime;
                //
                chart.Series[1].BorderWidth = 4;
                //
                chart.Series[1].LabelForeColor = Color.Silver;
                //chart.Series[0].CustomProperties = "PointWidth=4";
                //
                chart.Series[1].IsValueShownAsLabel = true;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        //
        public static void showcurbyclick(int ringnum, System.Windows.Forms.DataVisualization.Charting.Chart chart)
        {
            //
            chart.ChartAreas[0].CursorX.Position = ringnum;
            //
            chart.ChartAreas[0].AxisX.ScaleView.Zoom(ringnum-1,ringnum+2);
            //
            chart.Series[0].BorderWidth = 1;
            //x
            //chart.ChartAreas[0].AxisX.Interval = 0.1;
        }
    }
    #endregion

}
