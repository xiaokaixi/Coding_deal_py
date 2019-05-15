using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Timers;
using System.Runtime.InteropServices;
using System.Data;
using System.Text;
using ReaderB;  //RFID 
using System.Speech.Synthesis;  //语音提示


namespace manage
{
    public partial class form_newinput : Form
    {
        #region  全局变量
        public firtdoor f1;
        public string dirpath = Application.StartupPath + "\\FreeVK.exe";  //获取应用程序路径
        public dynamic file;
        public Process p1;
        public int selecttype = 0;  //自动或手动选择
        public int vertloction1 = 0;  //垂直滑动条位置
        public System.Timers.Timer vsbar_t = new System.Timers.Timer();
        public DataSet mysql_tid = new DataSet();
        public DataSet mysql_tid_1 = new DataSet();   //用于查询是否已经入库
        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        public static extern bool MoveWindow(System.IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        //扫码线程全局变量
        delegate void datagrid1dill(DataGridView datagrid1, TextBox textbox8, TextBox textbox6, TextBox textbox1, string strbuf, string strbuf_qr);
        public static bool bIsLoop = false;
        public static Thread DecodeThread = null;
        Vbarapi qrsm = new Vbarapi();
        public SpeechSynthesizer speech; //TTS
        #endregion

        #region  窗体初始化
        public form_newinput(firtdoor f11)
        {
            InitializeComponent();
            f1 = f11;
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            //滑动textbox实时显示事件
            moveandtext3();
            //功能初始化
            initfunction();
            //初始化北京时间
            timenow();
            //开启柜号分配定时器
            System.Timers.Timer t1 = new System.Timers.Timer(50);
            t1.Elapsed += new ElapsedEventHandler(choose_doorid);
            t1.AutoReset = true;
            t1.Enabled = true;
            //跨线程调用窗体控件的初始化设置
            Control.CheckForIllegalCrossThreadCalls = false;
        }
        #endregion

        #region    北京时间刷新
        //定时刷新
        public System.Timers.Timer tnow = new System.Timers.Timer();
        public bool flgtnow = false;
        public void timenow()
        {
            tnow.Interval = 1;       //每1s刷新一次
            tnow.Elapsed += new System.Timers.ElapsedEventHandler(theoutime);
            tnow.AutoReset = true;  //true一直执行,false执行一次
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

        //声明委托,跨线程
        private delegate void SetDataDelegate1();
        public string username;
        public int countt=0;
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
                    //实时时间
                    textBox9.Text = DateTime.Now.ToString();
                    if (countt == 3)
                    {
                        f1.load_exdata("ex", "exdata", "dispydata", "dispy_instatus");
                        f1.openport();
                    }
                    if (countt == 4)
                    {
                        countt = 3;
                    }
                    countt++;
                }
            }
            catch
            { }
        }
        #endregion

        #region   自动，手动柜号分配定时器函数
        //开启新线程
        private void choose_doorid(object sender, ElapsedEventArgs e)
        {
            dothis();
        }

        //定义委托
        private delegate void doorid();
        private void dothis()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new doorid(dothis));
                }
                else
                {
                    //自动分配
                    if (radioButton1.Checked == true)
                    {
                        textBox4.Enabled = true;
                        textBox5.Enabled = false;
                        textBox7.Enabled = false;
                    }
                    //手动分配
                    else
                    {
                        textBox4.Enabled = false;
                        textBox5.Enabled = true;
                        textBox7.Enabled = true;
                    }
                }
            }
            catch
            { }
        }
        #endregion

        #region   功能初始化
        /// <summary>
        /// 功能初始化
        /// </summary>
        private void initfunction()
        {
            //tid显示数据集
            mysql_tid.Tables.Add();
            mysql_tid.Tables[0].Columns.Add();
            mysql_tid.Tables[0].Columns[0].ColumnName = "RFID编号";
            mysql_tid.Tables[0].Columns.Add();
            mysql_tid.Tables[0].Columns[1].ColumnName = "确认状态";
            dataGridView1.DataSource = mysql_tid.Tables[0];
            dataGridView1.Columns[1].Width = 80;
            mysql_tid_1.Tables.Add();
            mysql_tid_1.Tables[0].Columns.Add();
            mysql_tid_1.Tables[0].Columns[0].ColumnName = "test";

            //干部档案入库信息
            label17.Visible = false;
            label8.Visible = false;
            label10.Visible = false;
            label12.Visible = false;
            label14.Visible = false;
            label15.Visible = false;
            textBox4.Text = f1.num.ToString() + "-" + f1.box.ToString();
            //入库功能
            button1.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button17.Enabled = false;
            label9.Enabled = false;
            //开启扫描线程
            f1.init1();
            qrresult();
            run_qr = true;
            //语音设置初始化
            speech = new SpeechSynthesizer();
            speech.Volume = 100;
            speech.Rate = 0;
        }
        #endregion

        #region  扫码线程

        //开启线程
        private void qrresult()
        {
            bIsLoop = true;
            DecodeThread = new Thread(new ThreadStart(DecodeThreadMethod));
            DecodeThread.IsBackground = true;
            DecodeThread.Start();
        }

        //关闭线程
        private void StopDecodeThread()
        {
            bIsLoop = false;
            if (DecodeThread != null)
            {
                DecodeThread.Abort();
                while (DecodeThread.ThreadState != System.Threading.ThreadState.Aborted)
                {
                    Thread.Sleep(50);
                }
            }
        }

        public string decoderesult = null;
        public string decoderesult_qr = null;
        public bool run_qr = false;
        //委托引用 线程
        private void DecodeThreadMethod()
        {
            datagrid1dill datagrid1dillRef_Instance = new datagrid1dill(listenport_receive);
            do
            {
                try
                {
                    //读二维码扫码器
                    if (run_qr)
                    {
                        decoderesult_qr = f1.Decoder();  //有二维码扫码器时使用
                    }
                    //读RFID读卡器
                    f1.readrfid();
                    if ((f1.CardNum != 0 || decoderesult != null|| decoderesult_qr != null) && saomaflg == true)
                    {
                        if (decoderesult_qr != null)
                        {
                            f1.qrsm_1();
                        }
                        this.Invoke(datagrid1dillRef_Instance, new object[] { this.dataGridView1, this.textBox8,this.textBox6,this.textBox1, decoderesult, decoderesult_qr });
                        decoderesult = null;
                        decoderesult_qr = null;
                        f1.CardNum = 0;
                    }
                }
                catch
                { }
            }
            while (bIsLoop);
        }

        passwdtext check = new passwdtext();        //写密钥txt对象
        timestoend timestoend = new timestoend();   //时间有效性计算对象
        private void listenport_receive(DataGridView datagrid1, TextBox textbox8, TextBox textbox6, TextBox textbox1, string strbuf, string strbuf_qr)
        {
            string showstr1 = "", showstr2 = "";
            List<byte> buffer = new List<byte>();  //定义列表成员，存放buf数据
            List<string> messagex;   //数据解析结果
            List<string> chenstr1, chenstr2, chenstr3;   //数据解析结果
            int n = 0, i = 0;
            byte[] buf = new byte[2000];
            if (decoderesult != null)
            {
                n = strbuf.Length;
                //判断串口数据来源
                for (i = 0; i < strbuf.Length; i++)
                {
                    buf[i] = Convert.ToByte(strbuf[i]);
                }
            }
            if (f1.CardNum != 0)
            {
                f1.sEPC = "%" + f1.sEPC;
                n = f1.sEPC.Length;
                //判断串口数据来源
                for (i = 0; i < f1.sEPC.Length; i++)
                {
                    buf[i] = Convert.ToByte(f1.sEPC[i]);
                }
            }
            if (strbuf_qr != null)
            {
                n = 0;
                //判断串口数据来源
                for (i = 0; i < strbuf_qr.Length; i++)
                {
                    if (Convert.ToByte(strbuf_qr[i]).ToString() == "0" && Convert.ToByte(strbuf_qr[i + 1]).ToString() == "0" && Convert.ToByte(strbuf_qr[i + 2]).ToString() == "0")
                    {
                        break;
                    }
                    buf[n] = Convert.ToByte(strbuf_qr[i]);
                    n++;
                }      
            }
            //定义一个新的buf存储
            buffer.AddRange(buf);              //缓存数据到buffer，error          

            //RFID读数据源
            if (buffer[0] == '%')         //帧头判断，数据开始
            {
                //转ASCII byte[]为string
                string buffer1 = Encoding.ASCII.GetString(buf, 0, n);
                buffer.RemoveRange(0, n);
                //RFID读卡器数据解析函数
                messagex = f1.danalysis(buffer1, DateTime.Now.ToString());
                if (messagex != null)
                {
                    //查询在库
                    chenstr1 = f1.chenking(messagex[0]);
                    //查询是否已经入库
                    if (chenstr1.Count != 0)
                    {
                        MessageBox.Show("该档案已入库，请重新扫描！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    showstr2 = messagex[0].Replace("00000", "*");

                    //检查是否重复读取-当前
                    for (i = 0; i < mysql_tid.Tables[0].Rows.Count; i++)
                    {
                        if (mysql_tid.Tables[0].Rows[i][0].ToString() == messagex[0])
                        { return; }
                    }
                    //检查是否重复读取-上次
                    for (i = 0; i < f1.mysql.Tables[0].Rows.Count; i++)
                    {
                        if (f1.mysql.Tables[0].Rows[i][1].ToString() == messagex[0])
                        {
                            MessageBox.Show("该档案已经录入！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    //命令组包
                    f1.danalysis1(f1.num, f1.box);

                    //加入开锁命令列表，用于批量开锁
                    f1.openlist.Add(f1.comm3);
                    f1.closelist.Add(f1.num.ToString() + "-" + f1.box.ToString());

                    mysql_tid.Tables[0].Rows.Add();
                    mysql_tid_1.Tables[0].Rows.Add();
                    f1.mysql.Tables[0].Rows.Add();
                    f1.mysql2.Tables[0].Rows.Add();
                    f1.mysql4.Tables[0].Rows.Add();
                    f1.mysql5.Tables[0].Rows.Add();
                    //operate_id操作员编号

                    mysql_tid.Tables[0].Rows[mysql_tid.Tables[0].Rows.Count - 1][0] = messagex[0];
                    mysql_tid.Tables[0].Rows[mysql_tid.Tables[0].Rows.Count - 1][1] = "等待确认";
                    mysql_tid_1.Tables[0].Rows[mysql_tid_1.Tables[0].Rows.Count - 1][0] = messagex[0];
                    f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][0] = f1.count;  //记id
                    f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][0] = f1.count++;  //记id
                    f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][1] = messagex[0];  //TID号
                    f1.mysql2.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][0] = showstr2;  //TID号
                    f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][1] = messagex[0];  //TID号
                    f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][0] = messagex[0];  //TID号
                    f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][2] = "new";  //入库   
                    f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][2] = 1;  //入库状态，1：在库，0：出库
                    counnm++;
                    f1.num1 = f1.num;
                    f1.box1 = f1.box;
                    f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][5] = f1.num1.ToString() + "-" + f1.box1.ToString();  //确定柜号+门号
                    f1.mysql2.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][1] = f1.num1.ToString() + "-" + f1.box1.ToString();  //确定柜号+门号
                    f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][5] = f1.num1.ToString() + "-" + f1.box1.ToString();  //确定柜号+门号
                    f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][4] = f1.num1.ToString() + "-" + f1.box1.ToString();  //确定柜号+门号
                    f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());  //转换为mysqltime时间格式                                                                                                                                                   //}
                    f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());  //转换为mysqltime时间格式 
                    f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][2] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                    datagrid1.DataSource = mysql_tid.Tables[0];
                    datagrid1.Columns[0].Width = 150;  //id列的宽度设置
                    datagrid1.Columns[1].Width = 100;  //tid列的宽度设置

                    //操作功能开启
                    if (mysql_tid.Tables[0].Rows.Count == 1)
                    {
                        button1.Enabled = true;
                        button17.Enabled = true;
                        label9.Enabled = true;
                    }
                    return;
                }
            }

            //二维码信息处理
            else
            {
                //转ASCII byte[]为string
                string buffer1 = Encoding.ASCII.GetString(buf, 0, n);
                buffer.RemoveRange(0, n);
                //string passkey = "A0123456789";  //解码密钥，要读取本地密钥
                //读取二进制文件,并转换密钥
                string passkey1 = check.logincheckset_lingdao("guanlingdao_keywd");
                string passkey = jiema(passkey1);
                string codestr = f1.des_string(buffer1, passkey);

                if (codestr[0] == '%')         //帧头判断，数据开始
                {

                    var str_1 = codestr.Split(new char[1] { '%' });
                    var str_2 = str_1[3].Split(new char[1] { ',' });
                    //借阅类型判断
                    if (str_1[1] != "I1")
                    {
                        MessageBox.Show("无效二维码，请扫描入库二维码！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);  //此代码在归还入库时有效
                        return;
                    }
                    //时效性判断
                    if (timestoend.time_hours(str_1[2], DateTime.Now.ToString()) > 24)
                    {
                        MessageBox.Show("二维码已过期，请在申请二维码24小时内操作！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);   //二维码有效性判断
                        return;
                    }
                    //借阅姓名
                    textbox8.Text = f1.gb2312tochinese(str_2[0]);

                    //借阅单位编号
                    textbox6.Text = str_2[1];
                    //借阅身份证号
                    textbox1.Text = str_2[2];

                    //写最新密钥
                    string new_passkey = bianma(str_2[3]);
                    if (!check.texthandl_ldkeywd("guanlingdao_keywd", new_passkey))
                    {
                        MessageBox.Show("口令权限操作失败，请重新操作", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);   //操作口令操作未成功标志
                        return;
                    }

                }
                else
                {
                    MessageBox.Show("无效二维码！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);   //此代码在归还入库时有效
                    return;
                }               
            }
        }
        #endregion

        #region    最高权限口令编解码
        /// <summary>
        /// ASII转二进制
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string bianma(string s)
        {
            byte[] data = Encoding.Unicode.GetBytes(s);
            StringBuilder result = new StringBuilder(data.Length * 8);

            foreach (byte b in data)
            {
                result.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }
            return result.ToString();
        }

        /// <summary>
        /// 二进制转ASII转换
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string jiema(string s)
        {
            System.Text.RegularExpressions.CaptureCollection cs = System.Text.RegularExpressions.Regex.Match(s, @"([01]{8})+").Groups[1].Captures;
            byte[] data = new byte[cs.Count];
            for (int i = 0; i < cs.Count; i++)
            {
                data[i] = Convert.ToByte(cs[i].Value, 2);
            }
            return Encoding.Unicode.GetString(data, 0, data.Length);
        }

        #endregion

        #region  滑动实时显示事件
        /// <summary>
        /// 定时器刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private delegate void setrefresh1(); //定义委托
        public bool flag = false;
        public int count = 0;
        private void moveandtext3()
        {
            Thread moveandtext = new Thread(new ThreadStart(refreshtime1_0));
            moveandtext.IsBackground = true;  //线程转后台
            moveandtext.Start();
        }
        //线程函数
        private void refreshtime1_0()
        {
            try
            {
                vsbar_t.Interval = 50;  //每50ms刷新一次    
                vsbar_t.Elapsed += new System.Timers.ElapsedEventHandler(refreshtime1);
                vsbar_t.AutoReset = true;  //true一直执行,false执行一次
                vsbar_t.Enabled = true;
                //关闭textbox记录事件定时器
                vsbar_t.Stop();
            }
            catch { }
        }
        private void refreshtime1(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (f1.b1 == false) { StopDecodeThread(); this.Close(); return; }
                refresh2();
            }
            catch
            { }
        }
        int nucount = 0, nucount1 = 0;
        bool runflg = false;
        public string doorname = "";
        private void refresh2()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new setrefresh1(refresh2));
                }

                else
                {
                    if (runflg)
                    {
                        return;
                    }
                    runflg = true;

                    try
                    {
                        //开关门状态实时记录
                        //for (int i = 0; i < f1.mysql_openview.Tables[0].Rows.Count; i++)
                        //{
                        if (f1.doorlistopen.IndexOf(f1.closestr) >= 0)
                        {
                            if (textBox3.Text.Contains(f1.closestr))
                            {
                            }
                            else
                            {
                                textBox3.AppendText("\r\n"); //换行
                                textBox3.AppendText(DateTime.Now.ToString() + "  " + f1.closestr + "  已打开，正在入库！");
                                textBox3.AppendText("\r\n"); //换行
                                textBox3.AppendText(f1.mysql.Tables[0].Rows[0][7].ToString() + "  " + f1.mysql.Tables[0].Rows[0][3].ToString() + " 正在入库");
                                textBox3.AppendText("\r\n"); //换行
                                f1.label24text = "";
                                doorname = f1.closestr;
                                //语音朗提示
                                speech.SpeakAsync(f1.closestr + "已打开，请放入" + num[0].ToString() + "盒档案");
                            }
                        }
                        //}
                        if (f1.label24text == "入库成功")
                        {
                            if (f1.label24text == "")
                            {
                                return;
                            }
                            if (nucount1.ToString() == num[0].ToString())
                            {
                                //语音朗提示
                                speech.SpeakAsync(doorname + "入库完成");
                                textBox3.AppendText(DateTime.Now.ToString() + "  " + doorname + "  共" + num[0].ToString() + "盒档案入库成功！");
                                textBox3.AppendText("\r\n"); //换行
                                nucount = f1.closelist.Count;
                                f1.label24text = "";
                                num.RemoveAt(0);
                                nucount1 = 0;
                                if (f1.dflag == 1)
                                { vsbar_t.Stop(); } 
                            }
                            else
                            {
                                textBox3.AppendText(DateTime.Now.ToString() + "  " + doorname + "  已关闭，1盒入库完成！");
                                textBox3.AppendText("\r\n"); //换行
                                nucount1++;
                            }
                            
                        }

                        //检测故障处理
                        if (f1.label24text == "入库异常")
                        {
                            f1.label24text = "";
                            //语音朗提示
                            speech.SpeakAsync(f1.closestr + "入库异常，请重新入库！");
                            textBox3.AppendText(DateTime.Now.ToString() + "  " + f1.closestr + "  1盒档案入库异常，请重新入库！");
                            textBox3.AppendText("\r\n"); //换行
                            button2.Enabled = true;
                            vsbar_t.Stop();
                        }
                        //检测是否完成所有操作
                        if (num.Count == 0&&f1.b1 == true)
                        {
                            if (f1.dflag == 1)
                            {
                                //语音朗提示
                                speech.SpeakAsync("入库操作完成，请手动点击返回主界面退出！");
                                textBox3.AppendText("\r\n"); //换行
                                textBox3.AppendText(DateTime.Now.ToString() + "  " + doorname + "  入库操作完成，请手动退出！");
                                textBox3.AppendText("\r\n"); //换行
                                button2.Enabled = false;
                            }
                            else
                            {
                                //语音朗提示
                                speech.SpeakAsync("入库操作完成，正在退出");
                                textBox3.AppendText("\r\n"); //换行
                                textBox3.AppendText(DateTime.Now.ToString() + "  " + doorname + "  入库操作完成，正在退出！");
                                textBox3.AppendText("\r\n"); //换行 
                                //延时5秒退出新增入库界面
                                run_qr = false;
                                f1.b1 = false;
                                f1.dflag = 0;
                                mysql_tid.Clear();
                                mysql_tid_1.Clear();
                                Thread.Sleep(7000);
                                speech.Dispose();
                            }           
                        }
                    }
                    catch
                    {  }
                }
                runflg = false;
            }
            catch
            { }
        }

        #endregion

        #region  打开虚拟键盘
        private void button5_Click(object sender, EventArgs e)
        {
            //打开虚拟键盘
            if (button5.Text== "打开输入键盘")
            {
                button5.Text = "关闭输入键盘";
                try
                {
                    //file = dirpath;
                    //if (!System.IO.File.Exists(file))
                    //    return;
                    //p1 = Process.Start(file);
                    //new Thread(() =>
                    //{
                    //    Thread.Sleep(300);
                    //    if (p1 != null && p1.MainWindowHandle != IntPtr.Zero)
                    //    {
                    //        MoveWindow(p1.MainWindowHandle, 150, 320, 650, 360, true);
                    //    }
                    //}).Start();
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
            else
            {
                button5.Text = "打开输入键盘";
                p1.CloseMainWindow();
            }
        }
        #endregion

        #region  返回主界面
        /// <summary>
        /// 返回主界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                StopDecodeThread();
                run_qr = false;
                f1.b1 = false;
                f1.dflag = 0;
                speech.Dispose();
                vsbar_t.Stop();
                closeinit();
                p1.CloseMainWindow();
                this.Close();
            }
            catch
            {
                StopDecodeThread();
                run_qr = false;
                f1.b1 = false;
                f1.dflag = 0;
                speech.Dispose();
                vsbar_t.Stop();
                closeinit();
                this.Close();
            }
        }
        #endregion

        #region   结束初始
        private void closeinit()
        {
            //关闭桌面读卡器
            int fCmdRet1;
            fCmdRet1 = StaticClassReaderB.CloseSpecComPort(f1.port);
            if (fCmdRet1 == 0)
            {
                //MessageBox.Show("关闭成功！");
            }
            //关闭二维码扫码器
            f1.init1close();
            //关闭初始
            dataGridView1.DataSource = null;
            f1.openlist.Clear();
            f1.closelist.Clear();
        }
        #endregion

        #region  清空信息
        /// <summary>
        /// 清除已录信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //清理已录数据集
            int m = f1.mysql.Tables[0].Rows.Count;
            int m2 = f1.mysql2.Tables[0].Rows.Count;
            int m4 = f1.mysql4.Tables[0].Rows.Count;
            int m5 = f1.mysql5.Tables[0].Rows.Count;
            for (int i=0;i<mysql_tid_1.Tables[0].Rows.Count;i++)
            {
                f1.mysql.Tables[0].Rows.RemoveAt(m - 2);
                f1.mysql2.Tables[0].Rows.RemoveAt(m2 -1);
                f1.mysql4.Tables[0].Rows.RemoveAt(m4 -2);
                f1.mysql5.Tables[0].Rows.RemoveAt(m5 -1);
                f1.openlist.RemoveAt(f1.openlist.Count-1);
                f1.closelist.RemoveAt(f1.closelist.Count - 1);
                m--;
                m2--;
                m4--;
                m5--;
                f1.count--;
                counnm--;
            }
            //清理界面
            mysql_tid.Clear();
            mysql_tid_1.Clear();
            textBox1.Clear();
            textBox2.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            label17.Visible = false;
            label8.Visible = false;
            label10.Visible = false;
            label12.Visible = false;
            label14.Visible = false;
            label15.Visible = false;
            //禁用信息确认、清空功能
            button1.Enabled = false;
            button17.Enabled = false;
            label9.Text = "干部档案信息等待核对！";
            label9.Enabled = false;
            //开启扫码
            saomaflg = true;

        }

        private void mysqlclear(DataTable dt,int nu,string str)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][nu].ToString() == str)
                {
                    
                    dt.Rows.RemoveAt(i);
                    Thread.Sleep(10);
                    try
                    {
                        if (dt.Rows[i][nu].ToString() == "")
                            dt.Rows.RemoveAt(i);
                    }
                    catch
                    { }
                    return;
                }
            }
        }

        #endregion

        #region  干部档案信息确定
        /// <summary>
        /// 干部档案信息确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public List<int> num=new List<int>();
        public bool saomaflg = true;
        private void button17_Click(object sender, EventArgs e)
        {
            List<string> chenumlist = new List<string>();
            try
            {            
                int sum = 0;
                //停止扫码线程
                saomaflg = false;
                //信息输入不合法
                //姓名
                if (textBox8.Text == "")
                {
                    label17.Text = "× 输入为空！";
                    label17.ForeColor = System.Drawing.Color.Red;
                    label17.Visible = true;
                }
                //单位编号
                if (textBox6.Text == "")
                {
                    label8.Text = "× 输入为空！";
                    label8.ForeColor = System.Drawing.Color.Red;
                    label8.Visible = true;
                }
                else
                {
                    string str = check_unitname(textBox6.Text);
                    if (str!="")
                    {
                        label10.Text = str;
                        label10.Visible = true;
                        label8.Text = "√";
                        label8.ForeColor = System.Drawing.Color.Green;
                        label8.Visible = true;
                        sum++;
                    }
                    else
                    {
                        label8.Text = "× 编号错误！";
                        label8.ForeColor = System.Drawing.Color.Red;
                        label8.Visible = true;
                    }  
                }
                //身份证号
                if (textBox1.Text == "")
                {
                    label12.Text = "× 输入为空！";
                    label12.ForeColor = System.Drawing.Color.Red;
                    label12.Visible = true;
                }
                else if (textBox1.Text.Length != 18)
                {
                    label12.Text = "× 格式有误！";
                    label12.ForeColor = System.Drawing.Color.Red;
                    label12.Visible = true;
                }
                else
                {
                    chenumlist = f1.chenking(textBox1.Text.ToString());
                    //需要改
                    if (chenumlist.Count == 6)
                    {
                        label12.Text = "× 存放数量达上限！";
                        label12.Visible = true;
                        label12.ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        label12.Text = "√";
                        label12.ForeColor = System.Drawing.Color.Green;
                        label12.Visible = true;
                        sum++;
                    }
                }
                //档案盒数
                if (textBox2.Text == "")
                {
                    label14.Text = "× 盒数为空！";
                    label14.ForeColor = System.Drawing.Color.Red;
                    label14.Visible = true;
                }
                else if (int.Parse(textBox2.Text) != mysql_tid.Tables[0].Rows.Count)
                {
                    label14.Text = "× 盒数不符！";
                    label14.ForeColor = System.Drawing.Color.Red;
                    label14.Visible = true;
                }
                else if (chenumlist.Count == 3)
                {
                    //判断在库盒数，确保每个柜子都装3份档案,未满3盒
                    if (int.Parse(chenumlist[2]) + int.Parse(textBox2.Text) <= 3)
                    {
                        label14.Text = "√ 新增档案！";
                        label14.ForeColor = System.Drawing.Color.Green;
                        label14.Visible = true;
                        textBox4.Text = chenumlist[0] + "-" + chenumlist[1];
                        changedata2();
                        sum++;
                    }
                    //判断在库盒数，确保每个柜子都装3份档案，已满3盒，继续增加柜体
                    if (int.Parse(chenumlist[2]) + int.Parse(textBox2.Text) > 3&& int.Parse(chenumlist[2]) + int.Parse(textBox2.Text)<= (int.Parse(f1.persumh) + 1) * 3)
                    {
                        label14.Text = "√  新增档案!";
                        label14.ForeColor = System.Drawing.Color.Green;
                        label14.Visible = true;
                        sum++;
                    }
                }
                else
                {
                    label14.Text = "√";
                    label14.ForeColor = System.Drawing.Color.Green;
                    label14.Visible = true;
                    sum++;
                }
                //分配柜体
                if (radioButton2.Checked == true)
                {
                    for (int i = 0; i < f1.mysql.Tables[0].Rows.Count; i++)
                    {
                        if (f1.mysql.Tables[0].Rows[i][5].ToString() == textBox5.Text + "-" + textBox7.Text)
                        {
                            label15.Text = "× 柜体号已录！";
                            label15.ForeColor = System.Drawing.Color.Red;
                            label15.Visible = true;
                        }
                    }
                    if (textBox5.Text == "" || textBox7.Text == "")
                    {
                        label15.Text = "× 输入为空！";
                        label15.ForeColor = System.Drawing.Color.Red;
                        label15.Visible = true;
                    }
                    else if (int.Parse(textBox7.Text) > 30)
                    {
                        label15.Text = "× 柜体号非法！";
                        label15.ForeColor = System.Drawing.Color.Red;
                        label15.Visible = true;
                    }
                    //只有身份证号为已入库人员时判断
                    else if (f1.chenking_position(textBox5.Text.ToString() + "-" + textBox7.Text.ToString()) == 3)
                    {
                        try
                        {
                            if (textBox5.Text + "-" + textBox7.Text == chenumlist[0] + "-" + chenumlist[1])
                            { }
                        }
                        catch
                        {
                            label15.Text = "× 柜体已满！";
                            label15.ForeColor = System.Drawing.Color.Red;
                            label15.Visible = true;
                        }
                    }
                    else if (chenumlist.Count>0)
                    {
                        try
                        {
                            if ((int.Parse(chenumlist[2]) + int.Parse(textBox2.Text) > 3) && (textBox5.Text + "-" + textBox7.Text == chenumlist[0] + "-" + chenumlist[1]))
                            {
                                label15.Text = "× 只差" + (3 - int.Parse(chenumlist[2])) + "盒";
                                label15.ForeColor = System.Drawing.Color.Red;
                                label15.Visible = true;
                            }
                        }
                        catch
                        { }
                    }

                    else
                    {
                        //更正手动分配柜号
                        changedata();
                        label15.Text = "√";
                        label15.ForeColor = System.Drawing.Color.Green;
                        label15.Visible = true;
                        sum++;
                    }
                }
                else
                {
                    label15.Text = "√";
                    label15.ForeColor = System.Drawing.Color.Green;
                    label15.Visible = true;
                    sum++;
                }

                //数据库查询tid标签合法性，是否已经在库
                for (int i = 0; i < mysql_tid_1.Tables[0].Rows.Count; i++)
                {
                    chenumlist = f1.chenking(mysql_tid_1.Tables[0].Rows[i][0].ToString());
                    if(chenumlist.Count >= 1)
                    {
                        mysql_tid.Tables[0].Rows[i][1] = "×已入库！";
                        dataGridView1.Rows[i].Cells[0].Style.ForeColor = System.Drawing.Color.Red;
                        dataGridView1.Rows[i].Cells[1].Style.ForeColor = System.Drawing.Color.Red;
                        sum--;
                    }
                    else
                    {
                        mysql_tid.Tables[0].Rows[i][1] = "确认正常";
                        dataGridView1.Rows[i].Cells[1].Style.ForeColor = System.Drawing.Color.Green;
                    } 
                }

                //所有确认正常，开启其他功能
                if (sum == 4)
                {
                    //数据集信息完善确认操作
                    bool flg = changedata1();
                    if (!flg)
                    {
                        return;
                    }
                    label9.Text = "干部档案信息核对成功！";
                    button2.Enabled = true;
                    radioButton3.Enabled = true;
                    radioButton4.Enabled = true;
                    button3.Enabled = true;
                    button1.Enabled = false;
                    //禁用自动手动分配
                    radioButton1.Enabled = false;
                    radioButton2.Enabled = false;
                    //datagridview显示确认正常
                    for (int i=0;i< mysql_tid.Tables[0].Rows.Count;i++)
                    {
                        mysql_tid.Tables[0].Rows[i][1] = "确认正常";
                    }
                    
                    //当前入库档案盒数
                    num.Add(int.Parse(textBox2.Text.ToString()));
                    button17.Enabled = false;
                }
                else
                {
                    label9.Text = "干部档案信息核对有误！";
                }
            }
            catch
            {
                label9.Text = "干部档案信息核对有误！";
            }
        }

        //手动分配柜体号
        public int counnm = 0;
        private void changedata()
        {
            f1.num = int.Parse(textBox5.Text.ToString());
            f1.box = int.Parse(textBox7.Text.ToString());
            f1.num1 = f1.num;
            f1.box1 = f1.box;
            f1.danalysis1(f1.num, f1.box);
            int m = counnm;
            for (int i = counnm - mysql_tid.Tables[0].Rows.Count; i < m; i++)
            {
                f1.closelist[i] = f1.num.ToString() + "-" + f1.box.ToString();
                f1.mysql.Tables[0].Rows[i][5] = f1.num1.ToString() + "-" + f1.box1.ToString();  //确定柜号+门号
                f1.mysql2.Tables[0].Rows[i][1] = f1.num1.ToString() + "-" + f1.box1.ToString();  //确定柜号+门号
                f1.mysql4.Tables[0].Rows[i][5] = f1.num1.ToString() + "-" + f1.box1.ToString();  //确定柜号+门号
                f1.mysql5.Tables[0].Rows[i][4] = f1.num1.ToString() + "-" + f1.box1.ToString();  //确定柜号+门号
                f1.openlist[i]=f1.comm3;
                f1.closelist[i]=f1.num.ToString() + "-" + f1.box.ToString();
            }
        }

        //库中已存在，自动分配柜体号
        public int counnm2 = 0;
        private void changedata2()
        {
            var trs = textBox4.Text.Split(new char[1] { '-'});
            f1.num = int.Parse(trs[0]);
            f1.box = int.Parse(trs[1]);
            f1.num1 = f1.num;
            f1.box1 = f1.box;
            int m = counnm2 + mysql_tid.Tables[0].Rows.Count;
            for (int i = counnm2; i < m; i++, counnm2++)
            {
                f1.closelist[i] = f1.num.ToString() + "-" + f1.box.ToString();
                f1.mysql.Tables[0].Rows[i][5] = f1.num1.ToString() + "-" + f1.box1.ToString();  //确定柜号+门号
                f1.mysql2.Tables[0].Rows[i][1] = f1.num1.ToString() + "-" + f1.box1.ToString();  //确定柜号+门号
                f1.mysql4.Tables[0].Rows[i][5] = f1.num1.ToString() + "-" + f1.box1.ToString();  //确定柜号+门号
                f1.mysql5.Tables[0].Rows[i][4] = f1.num1.ToString() + "-" + f1.box1.ToString();  //确定柜号+门号
            }
        }

        public int counnm1 = 0;
        private bool changedata1()
        {
            bool flg = true;
            int m = counnm1 + mysql_tid.Tables[0].Rows.Count;
            //身份证号是否重复录入判断
            for (int i = 0; i < f1.mysql.Tables[0].Rows.Count; i++)
            {
                if (textBox1.Text == f1.mysql.Tables[0].Rows[i][4].ToString())
                {
                    label12.Text = "× 已录入！";
                    label12.ForeColor = System.Drawing.Color.Red;
                    label12.Visible = true;
                    flg = false;
                    return flg;
                }
            }
            for (int i = counnm1; i < m; i++,counnm1++)
            {
                f1.mysql.Tables[0].Rows[i][3] = textBox8.Text;  //name  姓名
                f1.mysql4.Tables[0].Rows[i][3] = textBox8.Text;  //name  姓名
                f1.mysql.Tables[0].Rows[i][4] = textBox1.Text;  //perid 工号
                f1.mysql.Tables[0].Rows[i][7] = label10.Text;  //unit_name 单位编号
                f1.mysql4.Tables[0].Rows[i][4] = textBox1.Text;  //perid 身份证号
                f1.mysql4.Tables[0].Rows[i][7] = label10.Text;  //unit_name 单位编号
                f1.mysql5.Tables[0].Rows[i][1] = textBox1.Text;  //perid 身份证号
                f1.mysql.Tables[0].Rows[i][8] = f1.username;   //入库操作员编码
                f1.mysql4.Tables[0].Rows[i][8] = f1.username;  //入库操作员编码
                f1.mysql5.Tables[0].Rows[i][3] = f1.username;  //入库操作员编码
            }
            return flg;
        }
        #endregion

        #region   
        private string check_unitname(string unitid)
        {
            string unitname = "";
            DataSet mysql_unitname = new DataSet();
            mysql_unitname = f1.pre_load_unitname("ex", "unittable");
            if (mysql_unitname.Tables[0].Rows.Count != 0)
            {
                for (int i = 0; i < mysql_unitname.Tables[0].Rows.Count; i++)
                {
                    if (mysql_unitname.Tables[0].Rows[i][0].ToString() == unitid)
                    {
                        unitname = mysql_unitname.Tables[0].Rows[i][1].ToString();
                    }
                }
            }
            else
            {
                return unitname;
            }
            return unitname;
        }
        #endregion

        #region  继续录入
        /// <summary>
        /// 继续录入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            //允许扫码线程
            saomaflg = true;
            //清空
            textBox1.Clear();
            textBox2.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            label17.Visible = false;
            label8.Visible = false;
            label10.Visible = false;
            label12.Visible = false;
            label14.Visible = false;
            label15.Visible = false;
            //禁用信息确认、清空功能
            button1.Enabled = false;
            button17.Enabled = false;
            label9.Text = "干部档案信息等待核对！";
            label9.Enabled = false;
            //关闭开锁功能
            button2.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            button3.Enabled = false;
            //清空临时存储数据集
            mysql_tid.Tables[0].Clear();
            mysql_tid_1.Tables[0].Clear();
            //恢复入库信息功能-自动手动分配
            radioButton1.Enabled = true;
            radioButton2.Enabled = true;

            //自动分配盒数检查-重复录入，库中已存在  
            while (true)
            {
                try
                {
                    //查已录入表
                    for (int i = 0; i < f1.mysql.Tables[0].Rows.Count; i++)
                    {
                        if (f1.mysql.Tables[0].Rows[i][5].ToString() == f1.num + "-" + f1.box)
                        {
                            if (f1.box == 30)
                            {
                                f1.num++;
                                f1.box = 1;
                            }
                            else
                            {
                                f1.box++;
                            }
                            i = 0;
                            continue;
                        }
                    }
                    //查数据库表
                    if (!f1.chenkcount_position(f1.num + "-" + f1.box))
                    {
                        break;
                    }
                    else
                    {
                        if (f1.box == 30)
                        {
                            f1.num++;
                            f1.box = 1;
                        }
                        else
                        {
                            f1.box++;
                        }
                    }
                }
                catch
                { break; }
            }
            radioButton1.Checked = true; 
            textBox4.Text= f1.num.ToString() + "-" + f1.box.ToString();
        }
        #endregion

        #region  入库开锁操作
        /// <summary>
        /// 入库开锁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void button2_Click(object sender, EventArgs e)
        {
            vsbar_t.Start();
            //清空信息区
            mysql_tid.Clear();
            mysql_tid_1.Clear();
            textBox1.Clear();
            textBox2.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            label17.Visible = false;
            label8.Visible = false;
            label10.Visible = false;
            label12.Visible = false;
            label14.Visible = false;
            label15.Visible = false;
            //相关功能禁用
            //柜门分配
            radioButton1.Enabled = false;
            radioButton2.Enabled = false;
            textBox4.Enabled = false;
            textBox5.Enabled = false;
            textBox7.Enabled = false;
            //入库功能区禁用
            button17.Enabled = false;
            label9.Enabled = false;
            button1.Enabled = false;
            button3.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            //自动开
            if (radioButton4.Checked==true)
            {
                //语音朗提示
                speech.SpeakAsync("自动入库开始，请根据语音提示依次有序进行操作！");
                Thread.Sleep(1000);
                button2.Enabled = false;
                f1.dflag = 2;
                //停止温湿度定时器
                f1.t.Stop();
                f1.newinstore();
            }
            //手动开
            else
            {
                //语音朗提示
                speech.SpeakAsync("手动入库开始！");
                f1.dflag = 1;
                //停止温湿度定时器
                f1.t.Stop();
                f1.newinstore();
            }
        }
        #endregion

        #region  测试删库（正式上线后取消此功能）
        private void button21_Click(object sender, EventArgs e)
        {
            f1.mysql_clear_alldata("gatenote","gatenote1");
            f1.mysql_clear_alldata("note19", "note1901");
            f1.mysql_clear_alldata("note19", "dilltable");
            f1.mysql_clear_alldata("note19", "transfernote");
            f1.mysql_clear_alldata("store", "intable");
            f1.mysql_clear_alldata("store", "outtable");
            f1.mysql_clear_alldata("store", "tablename");
            MessageBox.Show("测试删库成功！");
        }
        #endregion

    }
}
