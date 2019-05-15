using System;
using System.Data;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Timers;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using System.Speech.Synthesis;  //语音提示

namespace manage
{
    public partial class form_borrowinput : Form
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
        public DataSet mysql_tid_1 = new DataSet();
        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        public static extern bool MoveWindow(System.IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        //扫码线程全局变量
        delegate void datagrid1dill(DataGridView datagrid1, TextBox textbox5, string strbuf, string strbuf_qr);
        public static bool bIsLoop = false;
        public static Thread DecodeThread = null;
        Vbarapi qrsm = new Vbarapi();
        public bool goon = false;
        public string perid = "";
        public SpeechSynthesizer speech; //TTS
        public bool run_qr=false;
        #endregion

        #region  窗体引导
        public form_borrowinput(firtdoor f11)
        {
            InitializeComponent();
            f1 = f11;
        }

        private void form_borrowinput_Load(object sender, EventArgs e)
        {
            //置顶
            //this.TopMost = true;           
            //滑动实时显示事件
            moveandtext3();
            //功能初始化
            initfunction();
            //初始化北京时间
            timenow();
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
                    textBox12.Text = DateTime.Now.ToString();
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
            dataGridView2.DataSource = mysql_tid.Tables[0];
            dataGridView2.Columns[1].Width = 80;
            mysql_tid_1.Tables.Add();
            mysql_tid_1.Tables[0].Columns.Add();
            mysql_tid_1.Tables[0].Columns[0].ColumnName = "test";
           
            //干部档案信息显示禁用
            textBox4.Enabled = false;
            textBox6.Enabled = false;
            textBox7.Enabled = false;
            textBox8.Enabled = false;
            textBox11.Enabled = false;
            //入库功能区选择性禁用
            button9.Enabled = false;
            label5.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;
            button2.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            label4.Visible = false;
            textBox2.Visible = false;
            //初始变量
            f1.openlist.Clear();
            f1.closelist.Clear();
            //扫码线程
            run_qr = true;
            qrresult();
            //语音设置初始化
            speech = new SpeechSynthesizer();
            speech.Volume = 100;
            speech.Rate = 0;
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
                //关闭滑动条事件定时器
                vsbar_t.Stop();
            }
            catch { }
        }
        private void refreshtime1(object sender, ElapsedEventArgs e)
        {
            refresh2();
        }
        int nucount = 0, nucount1 = 0;
        bool runflg = false;
        public bool textflg = false;
        private void refresh2()
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
                        { }
                        else
                        {
                            textBox3.AppendText("\r\n"); //换行
                            textBox3.AppendText(DateTime.Now.ToString() + "  " + f1.closestr + "  已打开，正在出库！");
                            textBox3.AppendText("\r\n"); //换行
                            textBox3.AppendText(f1.mysql.Tables[0].Rows[0][7].ToString()+"  "+ f1.mysql.Tables[0].Rows[0][3].ToString()+" 正在出库");
                            textBox3.AppendText("\r\n"); //换行
                            //语音朗提示
                            //speech.SpeakAsync(f1.closestr + "已打开，请取出" + num[0].ToString() + "盒档案");
                            speech.SpeakAsync(f1.closestr + "已打开，请取出档案");
                        }
                    }
                    //}
                    if (f1.label52text == "出库成功")
                    {
                        speech.SpeakAsync(f1.closestr + "出库完成");
                        textBox3.AppendText(DateTime.Now.ToString() + "  " + f1.closestr + "  已关闭，档案出库完成！");
                        textBox3.AppendText("\r\n"); //换行
                        f1.label52text = "";
                    }

                    if (f1.openlist.Count == 0)
                    {
                        num.Clear();
                    }

                    //检测故障处理
                    if (f1.label52text == "出库异常")
                    {
                        f1.label52text = "";
                        //语音朗提示
                        speech.SpeakAsync(f1.closestr + "出库异常，请重新出库！");
                        textBox3.AppendText(DateTime.Now.ToString() + "  " + f1.closestr + "  出库异常，请重试！");
                        textBox3.AppendText("\r\n"); //换行
                        button2.Enabled = true;
                        vsbar_t.Stop();
                    }
                    //检测是否完成所有操作
                    if (f1.openlist.Count == 0&& textflg == true&& num.Count==0)
                    {
                        //语音朗提示
                        speech.SpeakAsync("借阅取件完成，请进行扫描核对！");
                        textBox3.AppendText("\r\n"); //换行
                        textBox3.AppendText(DateTime.Now.ToString() + "  借阅取件完成，请扫描核对！");
                        textBox3.AppendText("\r\n"); //换行
                        Thread.Sleep(2000);
                        StopDecodeThread();
                        textflg = false;
                        run_qr = false;
                        f1.dflag = 0;
                        //出库取件完毕
                        button5.Enabled = true;
                        button2.Enabled = false;
                        vsbar_t.Stop();
                        if (f1.dflag == 1)
                        { vsbar_t.Stop(); }
                    }
                }
                catch
                { }
            }
            runflg = false;
        }

        #endregion

        #region 返回主界面
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                StopDecodeThread();
                run_qr = false;
                f1.b2 = false;
                f1.dflag = 0;
                f1.borrow_save = false;
                f1.label52text = "";
                time_gatecheck.Stop();
                speech.Dispose();
                vsbar_t.Stop();
                p1.CloseMainWindow();
                f1.borrow_checkout = false;
                closeinit();
                this.Close();
            }
            catch
            {
                StopDecodeThread();
                run_qr = false;
                f1.b2 = false;
                f1.dflag = 0;
                f1.borrow_save = false;
                f1.label52text = "";
                time_gatecheck.Stop();
                speech.Dispose();
                f1.borrow_checkout = false;
                closeinit();
                this.Close();
            }
        }

        #endregion

        #region   结束初始
        private void closeinit()
        {
            //关闭初始
            f1.openlist.Clear();
            f1.closelist.Clear();
            f1.init1close();
        }
        #endregion

        #region   打开输入键盘
        private void button6_Click(object sender, EventArgs e)
        {
            //打开虚拟键盘
            if (button6.Text == "打开输入键盘")
            {
                button6.Text = "关闭输入键盘";
                //try
                //{
                //    file = dirpath;
                //    if (!System.IO.File.Exists(file))
                //        return;
                //    p1 = Process.Start(file);
                //    new Thread(() =>
                //    {
                //        Thread.Sleep(300);
                //        if (p1 != null && p1.MainWindowHandle != IntPtr.Zero)
                //        {
                //            MoveWindow(p1.MainWindowHandle, 150, 300, 650, 360, true);
                //        }
                //    }).Start();
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
            else
            {
                button6.Text = "打开输入键盘";
                p1.CloseMainWindow();
            }
        }
        #endregion

        #region  档案信息1次确认
        private void button8_Click(object sender, EventArgs e)
        {
            List<string> chenstr1, chenstr2, chenstr3;   //数据解析结果
            textBox2.Text = "0";
            if (textBox2.Text == "" || textBox5.Text == "")
            {
                MessageBox.Show("请输入借阅信息！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                //身份证信息查询，查看是否在库
                chenstr1 = f1.chenking(textBox5.Text);
                if (chenstr1[0]=="1")
                {
                    MessageBox.Show("该身份信息不在库，请检查！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    return;
                }
                //检查档案盒数正确性
                if (chenstr1[0] == "0" && chenstr1.Count < int.Parse(textBox2.Text))
                {
                    MessageBox.Show("请检查档案盒数，库中档案小于请求盒数！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    return;
                }
                //检查档案是否被借出
                if (chenstr1.Contains("mm"))
                {
                    MessageBox.Show("该干部档案已全部被借出！请检查！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    return;
                }
                //输入格式检查
                if (textBox5.Text.Length !=18 )
                {
                    MessageBox.Show("请检查身份证号码位数（18位）！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    return;
                }
                //档案详细信息显示解禁
                textBox4.Enabled = true;
                textBox6.Enabled = true;
                textBox7.Enabled = true;
                textBox8.Enabled = true;
                textBox11.Enabled = true;
                //datagridview2显示
                mysql_tid.Tables[0].Clear();
                for (int i = 0; i < f1.mysqll.Tables[0].Rows.Count; i++)
                {
                    mysql_tid.Tables[0].Rows.Add();
                    mysql_tid.Tables[0].Rows[i][0] = f1.mysqll.Tables[0].Rows[i][1];  //干部身份证编号
                    mysql_tid.Tables[0].Rows[i][1] = "等待确认";
                }
                //档案详细信息显示
                textBox11.Text= f1.mysqll.Tables[0].Rows[0][3].ToString();   //name姓名字段
                textBox8.Text = f1.mysqll.Tables[0].Rows[0][7].ToString();   //unit_name字段 单位名称
                textBox6.Text = f1.mysqll.Tables[0].Rows.Count.ToString();   //档案盒数
                textBox7.Text = f1.mysqll.Tables[0].Rows[0][4].ToString();   //perid身份证号字段
                textBox4.Text = f1.mysqll.Tables[0].Rows[0][5].ToString();   //确定柜号+门号 

                button9.Enabled = true;
                label5.Enabled = true;
            }
        }
        #endregion

        #region  档案信息2次确认
        public List<int> num = new List<int>();
        private void button9_Click(object sender, EventArgs e)
        {
            if (int.Parse(textBox2.Text) <= int.Parse(textBox6.Text))
            {
                //检查是否有重复输入
                if (checkdata())
                {
                    MessageBox.Show("借阅信息已存在，请重新录入！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    return;
                }
                inputid_data();                
                label5.Text = "借阅信息确认成功！";
                button3.Enabled = true;
                button2.Enabled = true;
                radioButton3.Enabled = true;
                radioButton4.Enabled = true;
                //单次借阅数量记录
                //num.Add(int.Parse(textBox2.Text.ToString()));
                num.Add(0);
                textflg = true;
            }
            else
            {
                label5.Text = "请检查借阅信息！";
                return;
            }
        }

        private bool checkdata()
        {
            for (int i=0;i<f1.mysql.Tables[0].Rows.Count-2;i++)
            {
                if (textBox5.Text == f1.mysql.Tables[0].Rows[i][4].ToString())
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region  出库开锁操作
        private void button2_Click(object sender, EventArgs e)
        {
            vsbar_t.Start();
            //清空信息
            mysql_tid.Tables[0].Rows.Clear();
            textBox2.Clear();
            textBox5.Clear();
            //干部档案信息显示禁用
            textBox4.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            textBox11.Clear();
            textBox4.Enabled = false;
            textBox6.Enabled = false;
            textBox7.Enabled = false;
            textBox8.Enabled = false;
            textBox11.Enabled = false;
            button8.Enabled = false;
            //入库功能区禁用
            button9.Enabled = false;
            label5.Enabled = false;
            button3.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            //自动开
            if (radioButton4.Checked == true)
            {
                //语音朗提示
                speech.SpeakAsync("自动出库开始，请根据语音提示依次有序进行操作！");
                Thread.Sleep(1000);
                button2.Enabled = false;
                f1.dflag = 2;
                f1.t.Stop();
                f1.newinstore();
            }
            //手动开
            else
            {
                //一个一个开
                //语音朗提示
                speech.SpeakAsync("手动出库开始！");
                f1.dflag = 1;
                f1.t.Stop();
                f1.newinstore();
            }
        }
        #endregion

        #region  继续借阅
        private void button3_Click(object sender, EventArgs e)
        {            
            //清空信息
            mysql_tid.Tables[0].Rows.Clear();
            textBox2.Clear();
            textBox5.Clear();
            //干部档案信息显示禁用
            textBox4.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            textBox11.Clear();
            textBox4.Enabled = false;
            textBox6.Enabled = false;
            textBox7.Enabled = false;
            textBox8.Enabled = false;
            textBox11.Enabled = false;
            //入库功能区禁用            
            button9.Enabled = false;
            label5.Enabled = false;
            button3.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            button2.Enabled = false;
        }

        //批量借阅干部编号生成数据集
        public void inputid_data()
        {
            string str11 = "";
            string strperid = "%O1%2%" + DateTime.Now.ToString() + "%";
            int start = 0;

            str11 = f1.chenking_perid(textBox5.Text);
            if (str11 == "")
            {
                MessageBox.Show("该编号不存在，请检查！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (start != 0)
                {
                    if (strperid.IndexOf(textBox7.Text + ":") < 0)
                    {
                        mysql_tid_1.Tables[0].Rows.Add();
                        mysql_tid_1.Tables[0].Rows[mysql_tid_1.Tables[0].Rows.Count - 1][0] = textBox7.Text;
                        strperid += ";" + textBox7.Text + ":";
                        strperid += str11;
                    }
                }
                else
                {
                    if (strperid.IndexOf(textBox7.Text + ":") < 0)
                    {
                        mysql_tid_1.Tables[0].Rows.Add();
                        mysql_tid_1.Tables[0].Rows[mysql_tid_1.Tables[0].Rows.Count - 1][0] = textBox7.Text;
                        strperid += textBox7.Text + ":";
                        strperid += str11;
                        start = 1;
                    }
                }
            }
            strperid += "%" + "c001" + "%";
            decoderesult = strperid;
        }
        #endregion

        #region  扫码线程

        //开启扫二维码线程
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
        //委托引用 线程
        private void DecodeThreadMethod()
        {
            datagrid1dill datagrid1dillRef_Instance = new datagrid1dill(listenport_receive);
            do
            {
                try
                {
                    if (run_qr)
                    {
                        decoderesult_qr = f1.Decoder();  //有二维码扫码器时使用
                    }
                    if (decoderesult != null|| decoderesult_qr!=null)
                    {
                        if (decoderesult_qr != null)
                        {
                            f1.qrsm_1();
                        }  
                        this.Invoke(datagrid1dillRef_Instance, new object[] { this.dataGridView1, this.textBox5, decoderesult , decoderesult_qr });
                        decoderesult = null;
                        decoderesult_qr = null;
                    }
                }
                catch
                { }
            }
            while (bIsLoop);
        }

        passwdtext check = new passwdtext();        //写密钥txt对象
        timestoend timestoend = new timestoend();   //时间有效性计算对象
        private void listenport_receive(DataGridView datagrid1, TextBox textbox5, string strbuf, string strbuf_qr)
        {
            string showstr1 = "", showstr2 = "";
            List<byte> buffer = new List<byte>();  //定义列表成员，存放buf数据
            List<string> messagex;   //数据解析结果
            List<string> chenstr1, chenstr2, chenstr3;   //数据解析结果
            int n = 0, i = 0;
            byte[] buf = new byte[2000];
            if (strbuf != null)
            {
                n = strbuf.Length;
                //判断串口数据来源
                for (i = 0; i < strbuf.Length; i++)
                {
                    buf[i] = Convert.ToByte(strbuf[i]);
                }
                //定义一个新的buf存储
                buffer.AddRange(buf);              //缓存数据到buffer，error        
            }
            if (strbuf_qr != null)
            {
                n = 0;
                //判断串口数据来源
                for (i = 0; i < strbuf_qr.Length; i++)
                {
                    if (Convert.ToByte(strbuf_qr[i]).ToString() == "0"&& Convert.ToByte(strbuf_qr[i+1]).ToString() == "0"&& Convert.ToByte(strbuf_qr[i+2]).ToString() == "0")
                    {
                        break;
                    }
                    buf[n] = Convert.ToByte(strbuf_qr[i]);
                    n++;
                }
                //定义一个新的buf存储
                buffer.AddRange(buf);              //缓存数据到buffer，error        
            }
              
            //RFID读数据源
            if (buffer[0] == '%')         //帧头判断，数据开始
            {
                //转ASCII byte[]为string
                string buffer1 = Encoding.ASCII.GetString(buf, 0, n);
                buffer.RemoveRange(0, n);

                //二维码数据解析函数
                messagex = f1.danalysis(buffer1, DateTime.Now.ToString());

                //批量出库
                var data2 = messagex[2].Split(new char[1] { ';' });
                for (i = 0; i < data2.Count(); i++)
                {
                    var data3 = data2[i].Split(new char[2] { ':', ',' });
                    for (int j = 1; j < data3.Count() - 1; j++)
                    {
                        //查询在库 判断档案状态
                        chenstr1 = f1.chenking(data3[j]);
                        if (chenstr1.Count == 0)
                        { }
                        else if (chenstr1[0] == "0")
                        {
                            //killtxt = "ERROR";
                            //startkill();
                            MessageBox.Show(data3[j] + "该档案已经借出！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);   //档案已经借出判断                                                                                                                           //return;
                            continue;
                        }
                        else if (chenstr1[0] == "1")
                        {
                            //killtxt = "ERROR";
                            //startkill();
                            MessageBox.Show(data3[j] + "该档案不存在！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);   //档案不存在（或未入库）判断                                                                                                                        //return;
                            continue;
                        }

                        f1.mysql.Tables[0].Rows.Add();
                        f1.mysql2.Tables[0].Rows.Add();
                        f1.mysql4.Tables[0].Rows.Add();
                        f1.mysql5.Tables[0].Rows.Add();

                        f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][0] = f1.mysqll.Tables[0].Rows[0][0];    //id号 
                        f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][0] = f1.mysqll.Tables[0].Rows[0][0];    //id号
                        f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][1] = f1.mysqll.Tables[0].Rows[0][1];    //tid号
                        f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][1] = f1.mysqll.Tables[0].Rows[0][1];    //tid号
                        f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][0] = f1.mysqll.Tables[0].Rows[0][1];  //TID号 
                        f1.mysql2.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][0] = f1.mysqll.Tables[0].Rows[0][1].ToString().Replace("00000", "*");  //TID号 
                        f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][2] = "borrow";  //出库    
                        f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][2] = f1.mysqll.Tables[0].Rows[0][2];  //status在库代码编号1
                        f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][3] = f1.mysqll.Tables[0].Rows[0][3];  //姓名
                        f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][3] = f1.mysqll.Tables[0].Rows[0][3];  //姓名                                                                                                                                                 
                        f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][4] = f1.mysqll.Tables[0].Rows[0][4];  //perid 工号
                        f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][4] = f1.mysqll.Tables[0].Rows[0][4];  //perid 工号
                        f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][1] = f1.mysqll.Tables[0].Rows[0][4];  //perid 工号
                        f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][5] = f1.mysqll.Tables[0].Rows[0][5];  //位置编号,position
                        f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][5] = f1.mysqll.Tables[0].Rows[0][5];  //位置编号,position
                        f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][4] = f1.mysqll.Tables[0].Rows[0][5];  //确定柜号+门号
                        f1.mysql2.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][1] = f1.mysqll.Tables[0].Rows[0][5];  //确定柜号+门号 
                        f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][7] = f1.mysqll.Tables[0].Rows[0][7];  //单位名称
                        f1.openlist.Add(f1.comm3);
                        f1.closelist.Add(f1.mysqll.Tables[0].Rows[0][5].ToString());
                        f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());  //时间代码编号
                        f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                        f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][2] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                        f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][8] = f1.username;        //出库操作员编码
                        f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][8] = f1.username;
                        f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][3] = f1.username;        //出库操作员编码
                    }
                }
                var str4 = f1.mysqll.Tables[0].Rows[0][5].ToString().Split(new char[1] { '-' });
                f1.num = int.Parse(str4[0]);
                f1.box = int.Parse(str4[1]);
                f1.danalysis1(f1.num, f1.box);
                f1.num1 = f1.num;
                f1.box1 = f1.box;
            }

            //二维码信息处理
            else
            {
                //转ASCII byte[]为string
                string buffer1 = Encoding.ASCII.GetString(buf, 0, n);
                buffer.RemoveRange(0, n);
                //string passkey = "A0123456789";  //解码密钥，要读取本地密钥
                //读取二进制文件,并转换
                string passkey1 = check.logincheckset_lingdao("guanlingdao_keywd");
                string passkey=jiema(passkey1);
                string codestr = f1.des_string(buffer1, passkey);

                if (codestr[0] == '%')         //帧头判断，数据开始
                {

                    var str_1 = codestr.Split(new char[1] { '%' });
                    var str_2 = str_1[3].Split(new char[1] { ',' });
                    string idcard=str_2[0];
                    //借阅类型判断
                    if (str_1[1] != "O1")
                    {
                        MessageBox.Show("无效二维码，请扫描借阅二维码！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);  //此代码在归还入库时有效
                        return;
                    }
                    //时效性判断
                    if (timestoend.time_hours(str_1[2], DateTime.Now.ToString())>24)
                    {
                        MessageBox.Show("二维码已过期，请在申请二维码24小时内操作！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);   //二维码有效性判断
                        return;
                    }
                    //借阅身份证号
                    textbox5.Text = idcard;

                    //写最新密钥
                    string new_passkey = bianma(str_2[1]);
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

        #region  盘点扫描、通道门确认

        public System.Timers.Timer time_gatecheck = new System.Timers.Timer();
        //盘点
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                //盘点机1次盘点确认
                if (f1.pdjsytatu == false)
                {
                    MessageBox.Show("请检查移动盘点机网线连接！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    return;
                }
                //获取的mysql是数据库的，mysql3是盘点机的
                if (!f1.check_pand()) {
                    return;
                } 
                //数据集更新
                data_refresh();
                textBox9.AppendText(DateTime.Now.ToString() + "  本次共借阅"+ (f1.mysql.Tables[0].Rows.Count).ToString()+"盒，扫描完成！");
                textBox9.AppendText("\r\n"); //换行
                //语音朗提示
                speech.SpeakAsync("本次共借阅" + (f1.mysql.Tables[0].Rows.Count).ToString() + "盒档案，出库扫描核对完成，请出库");

                //开启通道门2次检测确认
                time_gatecheck.Interval = 5;
                time_gatecheck.Elapsed += new ElapsedEventHandler(gate_check);
                time_gatecheck.AutoReset = true;
                time_gatecheck.Enabled = true;
                f1.borrow_checkout = true;
                f1.borrow_checkout_r = true;
                button5.Enabled = false;
                spcoun = 0;
            }
            catch
            { }
        }

        //数据库更新
        private void data_refresh()
        {
            f1.openlist.Clear();
            f1.closelist.Clear();
            f1.mysql.Tables[0].Clear();
            f1.mysql2.Tables[0].Clear();
            f1.mysql4.Tables[0].Clear();
            f1.mysql5.Tables[0].Clear();
            for (int i = 0; i < f1.mysql3.Tables[0].Rows.Count; i++)
            {
                f1.chenking(f1.mysql3.Tables[0].Rows[i][1].ToString());
                f1.mysql.Tables[0].Rows.Add();
                f1.mysql2.Tables[0].Rows.Add();
                f1.mysql4.Tables[0].Rows.Add();
                f1.mysql5.Tables[0].Rows.Add();
                f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][0] = f1.mysqll.Tables[0].Rows[0][0];   //id号 
                f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][0] = f1.mysqll.Tables[0].Rows[0][0];  //id号                                                                                                                     
                f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][1] = f1.mysqll.Tables[0].Rows[0][1];   //tid号
                f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][1] = f1.mysqll.Tables[0].Rows[0][1];  //tid号
                f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][0] = f1.mysqll.Tables[0].Rows[0][1];  //TID号 
                textBox9.AppendText(DateTime.Now.ToString() + "  RFID编号：  "+f1.mysqll.Tables[0].Rows[0][1]);   //盘点显示借阅RFID编号
                textBox9.AppendText("\r\n"); //换行
                f1.openlist.Add(f1.comm3);
                f1.closelist.Add(f1.mysqll.Tables[0].Rows[0][5].ToString());
                f1.mysql2.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][0] = f1.mysqll.Tables[0].Rows[0][1].ToString().Replace("00000", "*");  //TID号 
                f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][2] = "borrow";  //出库    
                f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][2] = f1.mysqll.Tables[0].Rows[0][2];  //status在库代码编号1
                f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][3] = f1.mysqll.Tables[0].Rows[0][3];  //姓名 
                f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][3] = f1.mysqll.Tables[0].Rows[0][3];  //姓名                                                                                                                                                
                f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][4] = f1.mysqll.Tables[0].Rows[0][4];  //perid 工号
                f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][4] = f1.mysqll.Tables[0].Rows[0][4];  //perid 工号
                f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][1] = f1.mysqll.Tables[0].Rows[0][4];  //perid 工号
                f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][5] = f1.mysqll.Tables[0].Rows[0][5];  //位置编号,position
                f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][5] = f1.mysqll.Tables[0].Rows[0][5];  //位置编号,position
                f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][4] = f1.mysqll.Tables[0].Rows[0][5];  //确定柜号+门号
                f1.mysql2.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][1] = f1.mysqll.Tables[0].Rows[0][5];  //确定柜号+门号 
                f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());  //时间代码编号
                f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][2] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][7] = f1.mysqll.Tables[0].Rows[0][7];        //单位名称
                f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][7] = f1.mysqll.Tables[0].Rows[0][7];       //单位名称
                f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][8] = f1.username;        //出库操作员编码
                f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][8] = f1.username;
                f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][3] = f1.username;        //出库操作员编码
            }
        }

        //开启新线程
        public bool flag1 = false;
        private void gate_check(object sender, ElapsedEventArgs e)
        {
            //if (f1.b2 == false) { StopDecodeThread(); this.Close(); return; }
            if (flag1)
            { return; }
            flag1 = true;
            dothis();
            flag1 = false;
        }

        //定义委托
        private delegate void doorid();
        public List<string> cardlist1 = new List<string>();  //实时标签
        public int spcoun=0;
        bool gate_runflg = false;
        int countt = 0;
       
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
                    if (gate_runflg)
                    { return; }
                    gate_runflg = true;
                    if (f1.borrow_checkout_r == false)
                    {
                        cardlist1 = f1.checkout;
                        f1.borrow_checkout_r = true;
                        List<int> ranf = new List<int>();
                        if (f1.checkout.Count == f1.mysql3.Tables[0].Rows.Count)
                        {
                            //保存数据
                            f1.borrow_save = true;
                            int count = f1.mysql.Tables[0].Rows.Count;
                            for (int i = 0; i < count; i++)
                            {
                                f1.savedate();
                                textBox10.AppendText(DateTime.Now.ToString() + "  RFID编号：  " + f1.mysql.Tables[0].Rows[0][1] + "出库正常！");
                                textBox10.AppendText("\r\n"); //换行
                                f1.mysql.Tables[0].Rows.RemoveAt(0); //status出库成功代码编号0 
                                f1.mysql4.Tables[0].Rows.RemoveAt(0);
                            }
                            //借阅出库成功二维码打印
                            f1.produceqr();
                            //关闭扫码器
                            closeinit();
                            //通道数据清理
                            f1.checkout = null;
                            f1.borrow_checkout = false;
                            f1.borrow_save = false;
                            //清空通道门寄存器
                            f1.gate_cleardata();
                            //显示完成进度
                            //语音朗提示
                            //speech.SpeakAsync("出库操作完成，请手动点击返回主界面退出！");
                            //textBox3.AppendText("\r\n"); //换行
                            //textBox10.AppendText(DateTime.Now.ToString() + "  借阅出库完成，请点击返回主界面！");
                            //textBox10.AppendText("\r\n"); //换行
                            speech.SpeakAsync("借阅出库操作完成，正在退出");
                            textBox10.AppendText("\r\n"); //换行
                            textBox10.AppendText(DateTime.Now.ToString() + "出库操作完成，正在退出！");
                            textBox10.AppendText("\r\n"); //换行
                            //延时7秒退出新增入库界面
                            Thread.Sleep(7000);
                            f1.dflag = 0;
                            f1.label52text = "";
                            time_gatecheck.Stop();
                            f1.b2 = false;
                            button4.Enabled = false;
                        }

                        else
                        {
                            ranf.Clear();
                            if (cardlist1.Count > f1.mysql3.Tables[0].Rows.Count)
                            {
                                for (int i = 0; i < cardlist1.Count; i++)
                                {
                                    for (int j = 0; j < f1.mysql3.Tables[0].Rows.Count; j++)
                                    {
                                        if (cardlist1[i] == f1.mysql3.Tables[0].Rows[j][1].ToString())
                                        {
                                            ranf.Add(i);
                                            continue;
                                        }
                                    }
                                }
                                for (int i = 0; i < cardlist1.Count; i++)
                                {
                                    if (ranf.Contains(i))
                                    { continue; }
                                    if (spcoun == 0)
                                    {
                                        speech.SpeakAsync(cardlist1.Count - ranf.Count + "盒档案核对异常，请重新扫描核对！");
                                        spcoun++;
                                        f1.Buzzer((byte)1, (byte)1, (byte)10, (byte)1, (byte)1, (byte)10);
                                    }
                                    textBox10.AppendText(DateTime.Now.ToString() + "  RFID编号：  " + cardlist1[i] + "出库未核对，请重新扫描！");
                                    textBox10.AppendText("\r\n"); //换行                                    
                                    MessageBox.Show(cardlist1[i] + "出库核对异常，请重新扫描核对！！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                                    f1.CardNum = 0;
                                    f1.borrow_checkout = false;
                                    button5.Enabled = true;
                                    time_gatecheck.Stop();
                                }
                            }
                            else
                            {
                                ranf.Clear();
                                if (cardlist1.Count < f1.mysql3.Tables[0].Rows.Count)
                                {
                                    for (int i = 0; i < f1.mysql3.Tables[0].Rows.Count; i++)
                                    {
                                        for (int j = 0; j < cardlist1.Count; j++)
                                        {
                                            if (f1.mysql3.Tables[0].Rows[i][1].ToString() == cardlist1[j])
                                            {
                                                ranf.Add(i);
                                                continue;
                                            }
                                        }
                                    }
                                }
                                for (int i = 0; i < f1.mysql3.Tables[0].Rows.Count; i++)
                                {
                                    if (ranf.Contains(i))
                                    {
                                        continue;
                                    }
                                    if (spcoun == 0)
                                    {
                                        speech.SpeakAsync(f1.mysql3.Tables[0].Rows.Count - cardlist1.Count + "盒档案出库检测异常，请重过通道门");
                                        spcoun++;
                                        f1.Buzzer((byte)1, (byte)1, (byte)10, (byte)1, (byte)1, (byte)10);
                                    }
                                    textBox10.AppendText(DateTime.Now.ToString() + "  RFID编号：  " + f1.mysql3.Tables[0].Rows[i][1] + "出库检测异常，请重过通道门");
                                    textBox10.AppendText("\r\n"); //换行
                                    MessageBox.Show(f1.mysql3.Tables[0].Rows[i][1] + "出库检测异常，请重过通道门！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                                }
                                spcoun = 0;
                            }
                        }
                    }
                }
            }
            catch
            { }
            gate_runflg = false;
        }

        #endregion
    }
}
