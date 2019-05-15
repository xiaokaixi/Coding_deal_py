﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;   //引用串口操作
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using Renci.SshNet;
using ThoughtWorks.QRCode.Codec;
using System.Drawing.Printing;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Timers;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using ReaderB; 
using Gate;     
using System.Speech.Synthesis;  
using System.Security.Cryptography; 



namespace manage
{

    #region  窗体类
    public partial class firtdoor : Form
    {

        #region  全局变量初始化
        int i = 0;
        public SerialPort port2 = new SerialPort();   //中间串口号
        public SerialPort port32 = new SerialPort();  //下位机串口号
        public SerialPort port33 = new SerialPort();  //扫描枪串口号
        public SerialPort listenport = new SerialPort();
        public List<string> portlistt;
        public List<string> portlist;
        public int count = 1;       //入库id
        public int jieyuecunt = 0;  //借阅id
        public int buttonflag = 0;
        public string killtxt;
        public float wnum1;
        public bool b1 = false, b2 = false, b3 = false, b4 = false;
        public int dflag = 0;
        public string name, perid;
        public DataSet mysql = new DataSet();
        public DataSet mysqll = new DataSet();
        public DataSet mysql2 = new DataSet();  //出入库显示用数据集合类型
        public DataSet mysql3 = new DataSet();  //盘点查询数据集合类型
        public DataSet mysql4 = new DataSet();  //note记录操作记录
        public DataSet mysql5 = new DataSet();  //出入库状态记录
        public DataSet mysql6 = new DataSet();  //开门记录
        public DataSet mysql7 = new DataSet();  //异常关门记录
        public DataSet mysql8 = new DataSet();  //历史操作数据记录
        public DataSet mysql9 = new DataSet();  //借走数据集
        public DataSet mysql_status = new DataSet(); //档案盒状态数据查询中间数据库
        public DataSet mysql_borrow = new DataSet(); //借出档案数据集
        public DataSet mysql_return = new DataSet(); //归还档案数据集
        public DataSet mysql_transfer = new DataSet(); //转递档案数据集
        public DataSet mysql_gate = new DataSet(); //归还档案数据集
        public DataSet mysql_openview = new DataSet(); //开门状态数据集
        public List<byte[]> openlist = new List<byte[]>();   //开门comd list
        public List<string> closelist = new List<string>();  //关门comd list
        public int closeflag = 0;
        public int vertloction1 = 0;  //垂直滑动条位置1
        public int vertloction2 = 0;  //垂直滑动条位置2
        public int vertloction3 = 0;  //垂直滑动条位置3
        //扫描线程
        public static Thread DecodeThread = null;
        public static Thread DecodeThread1 = null;
        public static Thread loadingdispThread = null;
        delegate void datagrid1dill(DataGridView datagrid1, DataGridView datagrid2, DataGridView datagrid3, TextBox txtbox, TextBox txtbox5, Button button31, Button button32, TextBox textbox4, TextBox textbox12, TextBox textbox27, TextBox textbox28, TextBox textbox29, TextBox textbox30, TextBox textbox31, TextBox textbox32, TextBox textbox33, TextBox textbox34, TextBox textbox35, TextBox textbox36, string strbuf);
        delegate void datagrid1dill2(DataGridView datagrid7, DataSet mysql22);
        delegate void devicerefreh(TableLayoutPanel tlp5, TableLayoutPanel tlp7, TableLayoutPanel tlp8, Button button20, Button button27, Button button46);
        //循环是否启动标记
        public static bool bIsLoop = false;
        public static bool bIsLoop1 = false;
        public bool checkmysqlflag = true;   //开门数据集操作标志
        public System.Timers.Timer timers_handles; //出入库操作定时器
        Vbarapi qrsm = new Vbarapi();
        //定时关闭窗体
        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        public const int WM_CLOSE = 0x10;
        public socketxiao socknet = new socketxiao();
        public passwdtext check = new passwdtext();  //txt文件操作
        //utf8编码
        UTF8Encoding utf8 = new UTF8Encoding();
        //自动锁屏中判断鼠标键盘操作的空闲时间API
        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
        private struct LASTINPUTINFO
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwTime;
        }
        #endregion

        #region  窗体初始化
        /// <summary>
        /// 窗体初始化
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="mysqlconn2"></param>
        public firtdoor(int i1, MySqlConnection mysqlconn2)
        {
            //loading界面
            if (i1 == 4)
            {
                loadingdisply();
                Thread.Sleep(4000);
            }
            InitializeComponent();
            i = i1;
            if (i == 1)
            {
                mysqlconn0.mysqlconn1 = mysqlconn2;
                mysqlconn0.conn = 1;
            }
        }

        /// <summary>
        /// 窗体引导
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public SpeechSynthesizer speech;
        public System.Timers.Timer t1;
        //写开机日志
        writelog log = new writelog();
        
        private void firstdoor_Load(object sender, EventArgs e)
        {
            if (i == 4 && mysqlconn0 != null)
            {
                #region  test
                //DEScode test
                //新增入库test
                //新增入库test
                //string name_str = gb_2312("田官权");
                //string codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",01,511725200104090817,A0123456789%";
                //string passkey = "A0123456789";
                //string descode;
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("路芸竹");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",01,140421200011098068,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("肖勇");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",02,513030200009300817,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("师一帆");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",02,410725200101263626,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("杜秋阳");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",03,511002199902127217,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("刘通");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",03,51140219990508519X,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("许志勇");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",05,513901200207162934,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("万家鑫");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",05,513424199808301938,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("肖佳发");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",06,513425199908252615,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("郑瑞东");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",06,51392120010828531X,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("王一帆");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",07,142401199912186218,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("冯金烨");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",07,450821199911153025,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("冯志伟");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",09,440281200006170412,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("冯俊霖");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",09,511124200009035714,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("雷小华");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",10,513029199812226272,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("杨宇");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",10,51132320000318211X,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("肖鹏龙");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",11,142723200008210216,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("蒋平");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",11,510921200009282217,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("王子嘉");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",12,511721200011027098,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("李秋宇");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",12,513825199911021617,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("许成年");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",13,342622200101017547,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("徐鹏");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",13,513922199908286573,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("罗宇杰");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",13,511181199908074436,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("范策飞");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",13,51300119990622081X,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("詹永生");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",17,342425200011064030,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("车孟伦");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",17,511126199506140918,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("徐智雯");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",18,331002199605224914,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("任柳清");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",18,511304199502285426,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("钦盼琛");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",22,513021199506166071,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //name_str = gb_2312("唐宗辉");
                //codestr = "%I1%" + DateTime.Now.ToString() + "%" + name_str + ",22,342622199704111393,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();

                ////借阅出库test
                //codestr = "%O1%" + DateTime.Now.ToString() + "%511725200104090817,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%140421200011098068,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%513030200009300817,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%410725200101263626,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%511002199902127217,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%51140219990508519X,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%513901200207162934,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%513424199808301938,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%513425199908252615,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%51392120010828531X,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%142401199912186218,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%450821199911153025,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%440281200006170412,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%511124200009035714,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%513029199812226272,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%51132320000318211X,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%142723200008210216,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%510921200009282217,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%511721200011027098,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%513825199911021617,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%342622200101017547,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%342622199704111393,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%513922199908286573,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%511181199908074436,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%51300119990622081X,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%342425200011064030,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%511126199506140918,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%331002199605224914,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%511304199502285426,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                //codestr = "%O1%" + DateTime.Now.ToString() + "%513021199506166071,A0123456789%";
                //descode = des_passkey(codestr, passkey);
                //txt = descode;
                //produceqr();
                ////string codestr = "o2fD4sL1yJTVI7R6vlTGgYFQWtzeBncBEc0Y5jVF9edumFsQiCKaffYw5p8Zim7+TQns5k6foN4=";
                ////codestr = des_string(codestr, passkey);
                //var str_1 = codestr.Split(new char[1] { '%' });
                //var str_2 = str_1[3].Split(new char[1] { ',' });
                //name_str = gb2312tochinese(str_2[0]);

                //语音朗读
                //speech = new SpeechSynthesizer();
                //speech.Volume = 100;
                //speech.Rate = 1;
                ////VoiceInfo sd = speech.Voice;
                ////speech.SelectVoice("Microsoft Lili");
                ////speech.SetOutputToWaveFile("test1.mp3");
                //int str = 100;
                //speech.SpeakAsync("柜门" + str.ToString() + "未关，请检查！");
                //输出语音文件
                //string namespaceName = Assembly.GetExecutingAssembly().GetName().Name.ToString();
                //Assembly assembly = Assembly.GetExecutingAssembly();
                //SoundPlayer sp = new SoundPlayer(assembly.GetManifestResourceStream("manage.Resources.warning.wav"));
                //sp.Play();
                //MessageBox.Show("shihiacnoa");
                ////speech.SpeakAsyncCancelAll();
                ////speech.Dispose();
                //MessageBox.Show("成功！");
                //GB2312 & utf8 test 
                //转utf8
                //string unit_name = "一所";
                //Byte[] temp = utf8.GetBytes(unit_name);
                //String result1 = utf8.GetString(temp);
                ////utf8转gb2312
                //string result2 = utf8togb2312(result1);
                ////string result2=gb_2312(unit_name);
                ////gb2312转汉字
                //string result3 = gb2312tostr(result2);              
                #endregion

                //初始化北京时间
                timenow();
                //初始化入库（归还、新增界面）
                instoreinit();
                //初始化出库（借阅界面）
                outstoreinit();
                //报警led画图初始
                this.Paint += new PaintEventHandler(firstdoor_Paint); // 手动添加处理事件
                //滑动条事件
                this.vScrollBar3.Scroll += new ScrollEventHandler(vscorllbar1move3);
                //默认禁用盘点功能图标
                button42.Enabled = false;
                button45.Enabled = false;               
                //this.Opacity = 100;
                //设置参数初始
                readsetting();
                //tcp通信初始化，远程监控计算机连接              
                socknet.socketinit("192.168.1.222",5000,255);
                //socknet.socketinit("192.168.1.23", 5000, 255);
                //通道门初始化
                gatedoorinit();
                //数据库链接初始化
                mysqlinit("192.168.1.21");
                //数据库count查询
                chenkcount();
                //档案状态查询
                refresh_checkdata();
                //查通道门数据
                gate_chenking();
                //门状态记录datagridview显示
                pre_load_exdata("ex", "dispydata");
                //串口设备链接初始化
                devlinit();
                Thread.Sleep(100);
                //温湿度读取,每500ms读一次
                dispaynum();

                //定时刷新采集数据
                timetoshow();
                
                //出入库操作定时器初始化
                timers_handles = new System.Timers.Timer(10);       //每10ms一次
                timers_handles.Elapsed += new System.Timers.ElapsedEventHandler(timer_handlesfun);
                timers_handles.AutoReset = true;  //true一直执行,false执行一次
                timers_handles.Enabled = true;
                timers_handles.Stop();

                //温湿度变量赋初值
                for (int j = 0; j < 9; j++)
                {
                    numlist1.Add("0.00");
                    numlist1.Add("0.00");
                    numlist2.Add("0.00");
                    numlist2.Add("0.00");
                    numlist3.Add("0");
                    numlist3.Add("0");
                }
                //设备在线检测
                devicecheck();
                //滑动条刷新事件
                systemrefresh();

                //打开登录界面
                form_login login = new form_login();
                login.ShowDialog();
                username = login.username;
                //开机日志
                log.Log(AppDomain.CurrentDomain.BaseDirectory + @"/log/Log.txt");
                log.log("Service start!  \n"); 
            }
        }

        private void firstdoor_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 1; i < 5; i++)
            {
                draw(i, System.Drawing.Color.Lime);
            }
        }
        #endregion

        #region  系统刷新事件
        private delegate void setrefresh(); //定义委托
        private void systemrefresh()
        {
            Thread comlisten = new Thread(new ThreadStart(systemrefresh1));
            comlisten.IsBackground = true;  //线程转后台
            comlisten.Start();
        }

        //开启线程
        private void systemrefresh1()
        {
            try
            {
                System.Timers.Timer t = new System.Timers.Timer(50);       //每10ms刷新一次
                t.Elapsed += new System.Timers.ElapsedEventHandler(refreshtime);
                t.AutoReset = true;  //true一直执行,false执行一次
                t.Enabled = true;
            }
            catch { }
        }
        private void refreshtime(object sender, ElapsedEventArgs e)
        {
            refresh1();
        }
        private void refresh1()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new setrefresh(refresh1));
            }
            else
            {
                try
                {
                    if (mysql_openview.Tables[0].Rows.Count > 1)
                    {
                        dataGridView3.FirstDisplayedScrollingRowIndex = vertloction3;
                    }
                }
                catch
                { }
            }
        }

        #endregion

        #region  滑动条触发事件

        /// <summary>
        /// 滚动条触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void vscorllbar1move3(object sender, ScrollEventArgs e)
        {
            try
            {
                if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
                {
                    vertloction3 = e.NewValue;
                }
            }
            catch { }
        }
        #endregion

        #region   系统强制推出函数
        public void firstdoor_FormClosed(object sender, FormClosedEventArgs e)
        {
            //关机日志
            log.Log(AppDomain.CurrentDomain.BaseDirectory + @"/log/Log.txt");
            log.log("Service stop!  \n");
            this.Dispose();
            this.Close();
        }
        #endregion

        #region  DES加/解密
        public string des_passkey(string codestr,string passkey)
        {
            string passcode="";
            try
            {
                byte[] bykey = null;
                byte[] iv = {0x12,0x34,0x56,0x78,0x90,0xAB,0xCD,0xEE };
                bykey = Encoding.UTF8.GetBytes(passkey.Substring(0,8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputbyteArray = Encoding.UTF8.GetBytes(codestr);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(bykey,iv), CryptoStreamMode.Write);
                cs.Write(inputbyteArray,0,inputbyteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch
            {
                return "";
            }
        }

        public string des_string(string passcode, string passkey)
        {
            try
            {
                byte[] bykey = null;
                byte[] iv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                byte[] inputbyteArray = new Byte[passcode.Length];
                bykey = Encoding.UTF8.GetBytes(passkey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputbyteArray = Convert.FromBase64String(passcode);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(bykey, iv), CryptoStreamMode.Write);
                cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                cs.FlushFinalBlock();
                Encoding coding = new UTF8Encoding();
                return coding.GetString(ms.ToArray());
            }
            catch
            {
                return "123";
            }
        }
        #endregion

        #region  转码
        #region UTF8转gb2312
        private string utf8togb2312(string utf8r)
        {
            ////声明字符集
            //System.Text.Encoding utf8_x, gb2312_x;
            ////utf8
            //utf8_x = System.Text.Encoding.GetEncoding("utf-8");
            ////gb2312
            //gb2312_x = System.Text.Encoding.GetEncoding("gb2312");
            //byte[] utf_str;
            //utf_str = utf8_x.GetBytes(utf8r);
            //utf_str = System.Text.Encoding.Convert(utf8_x,gb2312_x,utf_str);
            ////返回转换后的字符
            //return gb2312_x.GetString(utf_str);
            byte[] bs = Encoding.GetEncoding("UTF-8").GetBytes(utf8r);
            bs = Encoding.Convert(Encoding.GetEncoding("UTF-8"), Encoding.GetEncoding("GB2312"), bs);
            return Encoding.GetEncoding("GB2312").GetString(bs);
        }
        #endregion

        #region  GB2312转汉字
        public string gb2312tochinese(string buffer)
        {
            string chinese="";
            byte[] data = new byte[buffer.Length / 2];
            int i;
            try
            {                
                //buffer = buffer.Replace("0x", string.Empty);
                //buffer = buffer.Replace(" ", string.Empty);
                for (i = 0; i < buffer.Length / 2; i++)
                {
                    data[i] = Convert.ToByte(buffer.Substring(i * 2, 2), 16);
                }
                chinese = BytesToString(data);
            }
            catch
            {
            }
            return chinese;
        }
        private string BytesToString(byte[] Bytes)
        {
            string Mystring;
            Encoding FromEcoding = Encoding.GetEncoding("GB2312");
            Encoding ToEcoding = Encoding.GetEncoding("UTF-8");
            byte[] ToBytes = Encoding.Convert(FromEcoding, ToEcoding, Bytes);
            Mystring = ToEcoding.GetString(ToBytes);
            return Mystring;
        }

        #endregion

        #region  转gb312
        public string gb_2312(string codes)
        {
            Encoding gb2312 = Encoding.GetEncoding(936);
            byte[] gb_2312code = gb2312.GetBytes(codes);
            var gb_2312hex = BitConverter.ToString(gb_2312code, 0).Replace("-", string.Empty).ToLower();
            return gb_2312hex;
        }
        #endregion
        #endregion

        #region 刷新数据统计显示线程

        private void timetoshow()
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

        //启动定时器检测
        private void timetoshow1()
        {
            t1 = new System.Timers.Timer(300);       //每300ms刷新一次
            t1.Elapsed += new System.Timers.ElapsedEventHandler(theout1);
            t1.AutoReset = true;  //true一直执行,false执行一次
            t1.Enabled = true;
        }
        #endregion

        #region   导入Loading界面线程
        /// <summary>
        /// 开启线程
        /// </summary>
        /// 
        private void loadingdisply()
        {
            loadingdispThread = new Thread(new ThreadStart(loading_de));
            loadingdispThread.IsBackground = true;
            loadingdispThread.Start();
        }

        /// <summary>
        /// 开启线程定时器
        /// </summary>
        public int formf=0;
        private void loading_de()
        {
            try
            {
                System.Timers.Timer t5 = new System.Timers.Timer(10);       
                t5.Elapsed += new System.Timers.ElapsedEventHandler(loadingdisply1);
                t5.AutoReset = false;  //true一直执行,false执行一次
                t5.Enabled = true;
            }
            catch { }
        }
        private delegate void loadingdisplying(); //定义委托
        private void loadingdisply1(object source, System.Timers.ElapsedEventArgs e)
        {
            loadingdisply2();
        }

        /// <summary>
        /// loading窗体引导
        /// </summary>
        private void loadingdisply2()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new loadingdisplying(loadingdisply2));
            }
            else
            {
                form_Loading fl_loading = new form_Loading(this);
                fl_loading.TopMost = true;
                fl_loading.ShowDialog();
            }
        }

        #endregion

        #region  设置参数读取
        public void readsetting()
        {
            temph = check.logincheckset("temph");   //温度
            templ = check.logincheckset("templ");   //温度
            weth = check.logincheckset("weth");     //湿度
            wetl = check.logincheckset("wetl");     //湿度
            time1 = check.logincheckset("time1");   //历史时限 
            time2 = check.logincheckset("time2");   //档案盒状态时限
            controlsum = check.logincheckset("controlsum");   //在用柜体数
            persumh = check.logincheckset("persumh");        //单人档案上限数
            borrowday = check.logincheckset("borrowday");    //档案借出时限
            warningday = check.logincheckset("warningday");  //归还提醒时限
        }
        #endregion

        #region  设备在线查询刷新
        public bool dataserver=false,monitorpc=false,gatedoorstatus = false,controlboard=false,RFIDread=false,pdjsytatu=false,qrdevicestatu=false,printstatu=false;
        public bool n3,n4,n5,n6;
        public int n1=0, n2=0;
        /// <summary>
        /// 开启线程
        /// </summary>
        private void devicecheck()
        {
            bIsLoop1 = true;
            DecodeThread1 = new Thread(new ThreadStart(devicecheck1));
            DecodeThread1.IsBackground = true;
            DecodeThread1.Start();
        }

        /// <summary>
        /// 开启线程定时器
        /// </summary>
        public System.Timers.Timer t4 = new System.Timers.Timer(3000);       //每900ms刷新一次
        private void devicecheck1()
        {
            try
            {
                t4.Elapsed += new System.Timers.ElapsedEventHandler(devicecheck2);
                t4.AutoReset = true;  //true一直执行,false执行一次
                t4.Enabled = true;
            }
            catch { }
        }
        private delegate void devicechecking(); //定义委托
        private void devicecheck2(object source, System.Timers.ElapsedEventArgs e)
        {
            devicecheck3();
        }
        private void devicecheck3()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new devicechecking(devicecheck3));
            }
            else
            {
                //数据库服务器
                //dataserver = mysqldatacheck();
                //dataserver = pingip("192.168.1.21");
                dataserver = true;
                //断开连接功能禁用
                if (dataserver == false && n2 == 0)
                {
                    tableLayoutPanel7.Enabled = false;
                    tableLayoutPanel8.Enabled = false;
                    button27.Enabled = false;
                    n1 = 0;
                    t4.Stop();
                    while (true)
                    {
                        //关闭通道门查询
                        timer2.Enabled = false;
                        btDisConnectTcp();

                        MessageBox.Show("请检查数据库服务器网线！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (pingip("192.168.1.21") == true)
                        {
                            t4.Start();
                            //通道门初始化
                            gatedoorinit();
                            break;
                        }
                    }
                    
                }
                if (dataserver == true && n1 == 0)
                { 
                    tableLayoutPanel7.Enabled = true;
                    tableLayoutPanel8.Enabled = true;
                    button27.Enabled = true;
                    n2 = 0;
                }

                //温湿度监控计算机
                monitorpc = pingip("192.168.1.23"); 
                //控制通信线、桌面读卡器
                RegistryKey keyCom = Registry.LocalMachine.OpenSubKey("Hardware\\DeviceMap\\SerialComm");
                if (keyCom != null)
                {
                    string[] comtype = keyCom.GetValueNames();
                    int j = 0, k = 0;
                    foreach (string type in comtype)
                    {
                        k = type.IndexOf("Sila");
                        if (k > 0)
                        {
                            RFIDread = true;
                            break;
                        }
                    }
                    if (k <= 0)
                    {
                        RFIDread = false;
                    }
                }

                //控制通信线断开功能禁用
                if (controlboard == false && n4 == true)
                {
                    button20.Enabled = false;
                    button46.Enabled = false;
                    n3 = false;
                }
                if (controlboard == true && n3 == false)
                {
                    button20.Enabled = true;
                    button46.Enabled = true;
                    n4 = true;
                }
                //二维码扫码器
                if (qrsm.openDevice(1))
                {
                    qrdevicestatu = true;
                }
                else
                {
                    qrdevicestatu = false;
                }
                //打印机
                printstatu = true;
                //通道门
                if (IsGetting) return;
                IsGetting = true;
                fCmdRet = Device.ModeSwitch(ref ControllerAdr, ref fModel, ref IRStatus, PortHandle);//通道门设备信息查询
                IsGetting = false;
                if (fCmdRet == 0)
                {
                    gatedoorstatus = true;
                }
                else
                {
                    gatedoorstatus = false;
                }
                //盘点机
                //mysqlinitnossh("192.168.1.26");
                pdjsytatu = pingip("192.168.1.26");
                //盘点机网线断开功能禁用
                if (pdjsytatu == false && n6 == true)
                {
                    button45.Enabled = false;
                    button42.Enabled = false;
                    n5 = false;
                }
                if (pdjsytatu == true && n5 == false)
                {
                    button45.Enabled = true;
                    button42.Enabled = true;
                    n6 = true;
                }
                //控制板
                //门锁状态
            }
        }

        /// <summary>
        /// ping IP
        /// </summary>
        /// <param name="ipstr"></param>
        /// <returns></returns>
        private bool pingip(string ipstr)
        {
            bool ipstatus = false;
            try
            {
                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                System.Net.NetworkInformation.PingReply pingre = ping.Send(ipstr, 10);
                if (pingre.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    ipstatus = true;
                }
                return ipstatus;
            }
            catch
            { return ipstatus; }
        }

        #endregion

        #region  数据库档案状态查询
        public int days; //档案借阅时限
        DataSet mysql_borrowwaring = new DataSet();
        public List<string> borrowwaring = new List<string>();
        public List<string> borrowwaring_time = new List<string>();
        public void refresh_checkdata()
        {
            should_borrownum = 0;
            if (count != 1 && b1 != true && b2 != true)
            {
                try
                {
                    //数据出入库档案盒数量查询
                    mysql_borrow = load_mysql("store", "outtable", "borrow");
                    mysql_return = load_mysql("store", "intable", "return");
                    borandretnum = mysql_borrow.Tables[0].Rows.Count + mysql_return.Tables[0].Rows.Count;
                    //查借走数量查询
                    borrownum = borrow_chenking("0");
                    //提醒天数
                    days = int.Parse(borrowday) - int.Parse(warningday);
                    //应还档案数量
                    timestoend timestoend = new timestoend();
                    mysql_status = load_mysql("store", "tablename", "borrow");
                    borrowwaring.Clear();
                    for (int i = 0; i < mysql_status.Tables[0].Rows.Count; i++)
                    {
                        if (mysql_status.Tables[0].Rows[i][2].ToString() == "1")
                        {
                            continue;
                        }
                        mysql_borrowwaring = load_borrowwaring("store", "outtable", mysql_status.Tables[0].Rows[i][1].ToString());
                        //for (int n = 0; n < mysql_borrowwaring.Tables[0].Rows.Count; n++)
                        //{
                        //只获取最后一次出库时间，即为最近借出的时间
                        int n = mysql_borrowwaring.Tables[0].Rows.Count - 1;
                        if (timestoend.time_day(mysql_borrowwaring.Tables[0].Rows[n][6].ToString(), DateTime.Now.ToString()) >= days && (timestoend.time_day(mysql_borrowwaring.Tables[0].Rows[n][6].ToString(), DateTime.Now.ToString())) <= int.Parse(borrowday))
                        {
                            should_borrownum++;
                            borrowwaring.Add(mysql_status.Tables[0].Rows[i][1].ToString());
                            borrowwaring_time.Add(mysql_borrowwaring.Tables[0].Rows[n][6].ToString());
                        }
                        //}                        
                    }
                    //转递档案数量查询
                    mysql_transfer = load_mysql("note19", "transfernote", "transfer");
                    transfernum = mysql_transfer.Tables[0].Rows.Count;

                    #region   数据表按照设置时间定期清理
                    //数据表清理，保证所有数据在设置时限内
                    int day1 = 0;  //历史记录查询时限
                    int day2 =0;    //档案盒显示时限
                    int day_n = 0;
                    day1 = (int.Parse(time1) + 1) * 30;
                    day2 = (int.Parse(time2) + 1) * 30;

                    //outtable 借出表  极限时间 time2
                    day_n=timestoend.time_day(mysql_borrow.Tables[0].Rows[0][6].ToString(), DateTime.Now.ToString());
                    if (day_n > day2)
                    {
                        mysql_clear("store", "outtable", Convert.ToDateTime(mysql_borrow.Tables[0].Rows[0][6]), day_n- day2);
                    }
                    //intable  归还表  极限时间 time2
                    day_n = timestoend.time_day(mysql_return.Tables[0].Rows[0][6].ToString(), DateTime.Now.ToString());
                    if (day_n > day2)
                    {
                        mysql_clear("store", "intable", Convert.ToDateTime(mysql_return.Tables[0].Rows[0][6]), day_n - day2);
                    }
                    //通道门数据表  极限时间 time2
                    mysql_status = load_mysql("gatenote", "gatenote1", "gate_in");
                    day_n = timestoend.time_day(mysql_status.Tables[0].Rows[0][3].ToString(), DateTime.Now.ToString());
                    if (day_n > day2)
                    {
                        mysql_clear("gatenote", "gatenote1", Convert.ToDateTime(mysql_status.Tables[0].Rows[0][3]), day_n - day2);
                    }
                    //历史记录表   极限时间 time1
                    hisd_load_exdata("note19", "note1901");
                    day_n= timestoend.time_day(mysql8.Tables[0].Rows[0][6].ToString(), DateTime.Now.ToString());
                    if (day_n > day1)
                    {
                        mysql_clear("note19", "note1901", Convert.ToDateTime(mysql8.Tables[0].Rows[0][6]), day_n - day2);
                    }
                    #endregion

                }
                catch
                { }
            }
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
                    textBox9.Text = DateTime.Now.ToString();
                    //判断空闲时间5分钟
                    if (GetIdleTick() / 1000 >= 5 * 60)
                    {
                        form_login login = new form_login();
                        tnow.Stop();
                        login.ShowDialog();
                        username = login.username;
                        tnow.Start();
                        //数据库更新查询
                        chenkcount();
                        refresh_checkdata(); 
                    }
                }
            }
            catch
            { }
        }
        #endregion

        #region  自动锁屏
        /// <summary>
        /// 获取鼠标键盘空闲时间
        /// </summary>
        /// <returns></returns>
        public  long GetIdleTick()
        {
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            if (!b1&&!b2)
            {  
                lastInputInfo.cbSize = Marshal.SizeOf(lastInputInfo);
            }
            if (!GetLastInputInfo(ref lastInputInfo)||b1||b2) return 0;
            return Environment.TickCount - (long)lastInputInfo.dwTime;
        }
        #endregion

        #region    库房环境详情查看
        //库房环境详情查看
        private void button23_Click(object sender, EventArgs e)
        {
            form_everonmentview enevf = new form_everonmentview(this);
            enevf.ShowDialog();
        }

        #endregion

        #region 入库（归还、新增）界面
        private void instoreinit()
        {
            button28.Enabled = false;
            //button29.Text = "入库操作";            
            button31.Enabled = false;
            button32.Enabled = false;
            label24.Enabled = false;
            label25.Enabled = false;
            label30.Enabled = false;
            label32.Enabled = false;
            label33.Enabled = false;
            label26.Enabled = false;
            label28.Enabled = false;
            label29.Enabled = false;
            label24.Text = "等待入库";
            textBox4.Enabled = false;
            textBox12.Enabled = false;
            textBox27.Enabled = false;
            textBox28.Enabled = false;
            textBox29.Enabled = false;
            textBox30.Enabled = false;
            vScrollBar1.Enabled = false;
            textBox4.Text = "";
            textBox12.Text = "";
            textBox27.Text = "";
            textBox28.Text = "";
            textBox29.Text = "";
            textBox30.Text = "";
        }
        //清理界面
        private void instoreinit1()
        {

            //批入
            if (b2==false&&button31.Enabled == false&&openlist.Count==0)
            {
                instoreinit();
                button29.Enabled = true;
                //关闭扫码器
                init1close();
                //关闭桌面读卡器
                int fCmdRet1;
                fCmdRet1 = StaticClassReaderB.CloseSpecComPort(port);
                if (fCmdRet1 == 0)
                {
                    //MessageBox.Show("关闭成功！");
                }
                //b1 = false;
                //档案状态查询
                refresh_checkdata(); 
            }

        }

        //新增入库开始
        private void button29_Click(object sender, EventArgs e)
        {
            try
            {
                b1 = true;
                operate_type = "I1";
                form_newinput newin = new form_newinput(this);
                newin.ShowDialog();
                //数据库更新查询
                chenkcount();
                refresh_checkdata();
            }
            catch
            {  }
        }

        //归还入库开始
        /// <summary>
        /// 归还入库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button48_Click(object sender, EventArgs e)
        {
            b1 = true;
            operate_type = "D1";
            form_returninput returnin = new form_returninput(this);
            returnin.ShowDialog();
            //数据库更新查询
            chenkcount();
            refresh_checkdata();
            //清空通道门寄存器
            gate_cleardata();
        }

        //批量借阅输入干部编号
        private void inputid()
        {
            DataSet mysql_id = new DataSet();
            string str11="";
            string strperid = "%C1%2%" + DateTime.Now.ToString() + "%";
            int start = 0;
            mysql_id.Tables.Add();
            mysql_id.Tables[0].Columns.Add();
            mysql_id.Tables[0].Columns[0].ColumnName = "身份编号";

            while (true)
            {                
                form_borrowinput binput = new form_borrowinput(this);
                binput.ShowDialog();
                if (binput.goon == false)
                {
                    if (binput.perid !=""|| binput.dataGridView1.RowCount>1)
                    {
                        if (binput.perid != "")
                        {
                            str11 = chenking_perid(binput.perid);
                            strperid += binput.perid + ":";
                            strperid += str11;
                            mysql_id.Tables[0].Rows.Add();
                            mysql_id.Tables[0].Rows[mysql_id.Tables[0].Rows.Count - 1][0] = binput.perid;
                        }
                        else
                        {
                            break;
                        }
                        
                    }
                    else if (str11 == "")
                    {
                        strperid = null;
                        //清理借阅界面
                        b2 = false;
                        outstoreinit();
                        dataGridView2.DataSource = null;
                        init1close();
                        openlist.Clear();
                        closelist.Clear();
                        //档案状态查询
                        chenkcount();
                        refresh_checkdata();
                    }                    
                    break;
                }
                else
                {
                    str11 = chenking_perid(binput.perid);
                    if (str11 == "")
                    {
                        MessageBox.Show("该编号不存在，请检查！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        if (start != 0)
                        {
                            if (strperid.IndexOf(binput.perid + ":") < 0)
                            {
                                mysql_id.Tables[0].Rows.Add();
                                mysql_id.Tables[0].Rows[mysql_id.Tables[0].Rows.Count - 1][0] = binput.perid;
                                strperid += ";" + binput.perid + ":";
                                strperid += str11;
                            }
                        }
                        else
                        {
                            if (strperid.IndexOf(binput.perid + ":") < 0)
                            {
                                mysql_id.Tables[0].Rows.Add();
                                mysql_id.Tables[0].Rows[mysql_id.Tables[0].Rows.Count - 1][0] = binput.perid;
                                strperid += binput.perid + ":";
                                strperid += str11;
                                start = 1;
                            }
                        }
                    }     
                }
            }
            strperid += "%" + textBox32.Text + "%";
            decoderesult = strperid;
        }

        //单盒出入库异常显示
        private void warr()
        {
            //入库
            if (b1==true&&button31.Enabled == false)
            {
                for (int i = 0; i < mysql.Tables[0].Rows.Count - 1; i++)
                {
                    label24.Text = "入库异常";
                    mysql4.Tables[0].Rows[i][2] = 0;  //入库状态，1：成功，0：失败  
                    mysql5.Tables[0].Rows[i][5] = "入库失败";  //入库状态记录                                   
                }
                string mysqlcom1 = "use " + "note19";
                mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                string tablename = "note1901";
                string mysqlselect = "insert into " + tablename + " set ";
                if (mysqlconn0.mysqlupdatecomarow(mysqlselect, mysql4))
                {

                }
                else
                {
                    //创建表   
                    mysqlselect = "create table " + tablename + "(";
                    if (mysqlconn0.mysqlsavecom(mysqlselect, mysql4, tablename))
                    {

                    }
                    else
                    {
                        MessageBox.Show("操作记录保存失败，请重试！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            //出库，单出
            if (b2==true&&operate_type == "O1")
            {
                for (int i = 0; i < mysql.Tables[0].Rows.Count - 1; i++)
                {
                    label52.Text = "出库异常";
                    mysql4.Tables[0].Rows[i][2] = 0;  //入库状态，1：成功，0：失败  
                    mysql5.Tables[0].Rows[i][5] = "出库失败";  //入库状态记录                                   
                }
                string mysqlcom1 = "use " + "note19";
                mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                string tablename = "note1901";
                string mysqlselect = "insert into " + tablename + " set ";
                if (mysqlconn0.mysqlupdatecomarow(mysqlselect, mysql4))
                {

                }
                else
                {
                    //创建表   
                    mysqlselect = "create table " + tablename + "(";
                    if (mysqlconn0.mysqlsavecom(mysqlselect, mysql4, tablename))
                    {

                    }
                    else
                    {
                        MessageBox.Show("操作记录保存失败，请重试！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public string type;        //操作类型
        public string storeclass;  //单、批量操作
        public string inperid;     //操作人员编号

        //test测试button
        private void button30_Click(object sender, EventArgs e)
        {
            //mysql_clear("note19","note1901");
        }
        //批量入库（开锁）操作
        public int m=1;
        private void button31_Click(object sender, EventArgs e)
        {
            //    if (operate_type != "D1")
            //    {
            //        if (textBox29.Text == "")
            //        {
            //            MessageBox.Show("请扫描申请入库二维码！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            //            return;
            //        }
            //        //控制开门，带开门检测，带关门检测  
            //        try
            //        {
            //            m = 1;
            //            timers_handles.Start();
            //        }
            //        catch
            //        { }

            //    }
            //    //批量归还
            //    if (operate_type == "D1")
            //    {
            //        //控制开门，带开门检测，带关门检测
            //        try
            //        {
            //            m = 1;
            //            timers_handles.Start();
            //        }
            //        catch
            //        { }
            //    }
        }

        //批量入库函数
        public void newinstore()
        {
            //批量新增入库
            if (operate_type != "D1")
            {
                try
                {
                    m = 1;
                    timers_handles.Start();
                }
                catch
                { }

            }
            //批量归还
            if (operate_type == "D1")
            {
                //控制开门，带开门检测，带关门检测
                try
                {
                    m = 1;
                    timers_handles.Start();
                }
                catch
                { }
            }
        }

        //批量盒入库结束
        private void button32_Click(object sender, EventArgs e)
        {
            //关闭桌面读卡器
            int fCmdRet1;
            fCmdRet1 = StaticClassReaderB.CloseSpecComPort(port);
            if (fCmdRet1 == 0)
            {
                MessageBox.Show("关闭成功！");
            }
            //关闭初始
            instoreinit();
            button29.Enabled = true;
            dataGridView1.DataSource = null;
            openlist.Clear();
            closelist.Clear();
            init1close();
            instoreinit1();
            b1 = false;
            if (count != 1)
            {
                //查询存储id号
                chenkcount();
            }
        }

        //查看入库状态按钮
        private void button28_Click(object sender, EventArgs e)
        {
            //this.Close();
            //填写对应信息
            form_datacheck f9 = new form_datacheck(this,mysql5,true,"handlestatus",0);
            f9.ShowDialog();

        }

        //查看出库状态按钮
        private void button37_Click(object sender, EventArgs e)
        {
            form_datacheck f9 = new form_datacheck(this, mysql5,true, "handlestatus",0);
            f9.Text = "出库状态查询";
            f9.ShowDialog();
        }

        #endregion

        #region 出库（借阅）界面
        //界面初始化
        private void outstoreinit()
        {
            button34.Enabled = false;
            //button36.Text = "出库操作";
            button37.Enabled = false;
            label52text = "等待入库";
            label47.Enabled = false;
            label48.Enabled = false;
            label49.Enabled = false;
            label50.Enabled = false;
            label51.Enabled = false;
            label52.Enabled = false;
            label53.Enabled = false;
            label54.Enabled = false;
            textBox31.Enabled = false;
            textBox32.Enabled = false;
            textBox33.Enabled = false;
            textBox34.Enabled = false;
            textBox35.Enabled = false;
            textBox36.Enabled = false;
            vScrollBar2.Enabled = false;
            textBox31.Text = "";
            textBox32.Text = "";
            textBox33.Text = "";
            textBox34.Text = "";
            textBox35.Text = "";
            textBox36.Text = "";
        }

        //借阅出库操作
        public bool rightorwrang = false;
        private void button36_Click(object sender, EventArgs e)
        {            
            b2 = true;
            if (button36.Text == "借阅出库")
            {
                init2();
                operate_type = "O1";  
                form_borrowinput borrowin = new form_borrowinput(this);
                borrowin.ShowDialog();
                //数据库更新查询
                chenkcount();
                refresh_checkdata();
                return;
            }
            if (button36.Text == "出库（借阅）")
            {
                //if (textBox32.Text == "")
                //{
                //    MessageBox.Show("请扫描申请出库二维码！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                //    return;
                //}
                //单出
                if (dataGridView2.RowCount < 2)
                {
                    if (textBox35.Text == "")
                    {
                        MessageBox.Show("请扫描出库二维码！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        
                            //控制开门，带开门检测，带关门检测
                            t.Stop();
                            contorldoor();
                            t.Start();
                            //在关门检测中进行数据存储、报警、出二维码(另开线程)
                            closecheck();
                       
                    }
                }              
            }
        }

        //批量借阅开锁
        public void borrowout()
        {
            m = 1;
            timers_handles.Start();
        }

        //退出出库（借阅）操作
        private void button34_Click(object sender, EventArgs e)
        {
            b2 = false;
            outstoreinit();
            dataGridView2.DataSource = null;
            init1close();
            openlist.Clear();
            closelist.Clear();
            if (count != 1)
            {
                //查询存储id号
                chenkcount();
            }
            //档案状态查询
            refresh_checkdata();
        }

        #endregion

        #region 出入库操作定时器
        public bool timer_handlesfunflag = false;
        public int savetype = 0;
        public int position1 = 0;
        public int position2 = 0;
        public List<string> openlist1;

        /// <summary>
        /// loading窗体引导
        /// </summary>
        public string closestr="";
        public string label24text="";
        public string label52text = "";
        private void timer_handlesfun(object source, System.Timers.ElapsedEventArgs e)
        {
            if (timer_handlesfunflag)
            { return; }
            timer_handlesfunflag = true;          
            int p = 0;
            bool flagthis = true;

            //最新单入、批入(包括新增、归还批量操作)
            if (b1 == true && (dflag == 2||dflag==1))
            {
                try
                {  
                    if (m == 1)
                    {
                        var fenl = closelist[0].Split(new char[1] { '-' });
                        num = int.Parse(fenl[0]);
                        box = int.Parse(fenl[1]);
                        closestr = closelist[0];
                        danalysis1(num, box);
                        Thread.Sleep(3000);
                        contorldoor();
                        //在关门检测中进行数据存储、报警、出二维码(另开线程)                   
                        if (closeflag != 2)
                        {
                            closecheck();
                        }
                        //label24text = closelist[0] + "正在入库";
                        m = 0;
                    }

                    //关闭成功
                    if (closeflag == 1)
                    {
                        openlist.RemoveAt(0);
                        closelist.RemoveAt(0);     
                        mysql.Tables[0].Rows.RemoveAt(0);
                        flagthis = true;
                        for (; closelist.Contains(closestr);)
                        {

                            if (flagthis)
                            {
                                closeflag = 0;
                                savetype = 1;
                                Thread.Sleep(300);
                                closecheck();
                                flagthis = false;
                            }
                            if (closeflag == 1)
                            {
                                openlist.RemoveAt(0);
                                closelist.RemoveAt(0);
                                mysql.Tables[0].Rows.RemoveAt(position1);
                                flagthis = true;
                            }
                        }
                        label24text = "入库成功";
                        if (dflag == 1)
                        {
                            timers_handles.Stop();
                            t.Start();
                            closestr = "";
                        }
                        if (openlist.Count == 0)
                        {
                            timers_handles.Stop();
                            t.Start();
                            closestr = "";
                        }
                        m = 1;
                    }
                    //关闭失败，入库终止
                    if (closeflag == 2)
                    {
                        //MessageBox.Show("操作失败，入库终止，请重新入库剩余档案！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        label24text = "入库异常";
                        mysql4.Tables[0].Rows[mysql4.Tables[0].Rows.Count - 2 - openlist.Count + 1][2] = 0;  //入库状态，1：成功，0：失败
                        mysql5.Tables[0].Rows[mysql4.Tables[0].Rows.Count - 2 - openlist.Count + 1][5] = "入库失败";  //入库状态，1：成功，0：失败
                        string mysqlcom1 = "use " + "note19";
                        mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                        string tablename = "note1901";
                        string mysqlselect = "insert into " + tablename + " set ";
                        if (mysqlconn0.mysqlupdatecomarow1(mysqlselect, mysql4, openlist.Count - 1))
                        {

                        }
                        else
                        {
                            //创建表   
                            mysqlselect = "create table " + tablename + "(";
                            if (mysqlconn0.mysqlsavecom1(mysqlselect, mysql4, tablename, openlist.Count - 1))
                            {

                            }
                            else
                            {
                                MessageBox.Show("操作记录保存失败，请重试！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        timers_handles.Stop();
                        t.Start();
                        closestr = "";
                    }
                    closeflag = 0;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    if (dataGridView1.Rows.Count <= 2)
                    {
                        //MessageBox.Show("无可入档案，请重新扫描入库档案！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        timers_handles.Stop();
                        t.Start();
                        closestr = "";
                    }
                }
            }


            //批量借阅
            if (b2 == true&& dflag==2)
            {
                try
                {
                    if (m == 1)
                    {
                        label52text = "";
                        var fenl = closelist[0].Split(new char[1] { '-' });
                        num = int.Parse(fenl[0]);
                        box = int.Parse(fenl[1]);
                        closestr = closelist[0];
                        danalysis1(num, box);
                        Thread.Sleep(3000);
                        contorldoor();
                        //在关门检测中进行数据存储、报警、出二维码(另开线程)    
                        if (closeflag != 2)
                        {
                            closecheck();
                        }
                        //label52text = closelist[0] + "正在出库";
                        m = 0;
                    }
                    //关闭成功
                    if (closeflag == 1)
                    {
                        openlist.RemoveAt(0);
                        closelist.RemoveAt(0);
                        mysql.Tables[0].Rows.RemoveAt(0);
                        flagthis = true;
                        Thread.Sleep(200);
                        for (; closelist.Contains(closestr);)
                        {
                            if (flagthis)
                            {
                                closeflag = 0;
                                savetype = 1;
                                Thread.Sleep(100);
                                closecheck();
                                flagthis = false;
                            }
                            if (closeflag == 1)
                            {
                                openlist.RemoveAt(0);
                                closelist.RemoveAt(0);
                                mysql.Tables[0].Rows.RemoveAt(0);
                                flagthis = true;
                                label52text = "";
                            }
                        }
                        if (openlist.Count == 0)
                        {
                            timers_handles.Stop();
                            t.Start();
                        }
                        m = 1;
                    }
                    //关闭失败，出库终止
                    if (closeflag == 2)
                    {
                        //MessageBox.Show("操作失败，出库终止，请重新入库剩余档案！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        label52text = "出库异常";
                        mysql4.Tables[0].Rows[mysql4.Tables[0].Rows.Count - 2 - openlist.Count + 1][2] = 0;  //入库状态，1：成功，0：失败
                        mysql5.Tables[0].Rows[mysql4.Tables[0].Rows.Count - 2 - openlist.Count + 1][5] = "出库失败";  //入库状态，1：成功，0：失败
                        string mysqlcom1 = "use " + "note19";
                        mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                        string tablename = "note1901";
                        string mysqlselect = "insert into " + tablename + " set ";
                        if (mysqlconn0.mysqlupdatecomarow1(mysqlselect, mysql4, openlist.Count - 1))
                        {

                        }
                        else
                        {
                            //创建表   
                            mysqlselect = "create table " + tablename + "(";
                            if (mysqlconn0.mysqlsavecom1(mysqlselect, mysql4, tablename, openlist.Count - 1))
                            {

                            }
                            else
                            {
                                MessageBox.Show("操作记录保存失败，请重试！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        timers_handles.Stop();
                        t.Start();
                    }
                    closeflag = 0;
                }
                catch
                { }
            }

            //单量借阅
            if (b2 == true && dflag == 1)
            {
                try
                {
                    if (m == 1)
                    {
                        label52text = "";
                        var fenl = closelist[0].Split(new char[1] { '-' });
                        num = int.Parse(fenl[0]);
                        box = int.Parse(fenl[1]);
                        closestr = closelist[0];
                        danalysis1(num, box);
                        Thread.Sleep(3000);
                        contorldoor();
                        //在关门检测中进行数据存储、报警、出二维码(另开线程)    
                        if (closeflag != 2)
                        {
                            closecheck();
                        }
                        //label52text = closelist[0] + "正在出库";
                        m = 0;
                    }
                    //关闭成功
                    if (closeflag == 1)
                    {
                        openlist.RemoveAt(0);
                        closelist.RemoveAt(0);
                        mysql.Tables[0].Rows.RemoveAt(0);
                        flagthis = true;
                        Thread.Sleep(200);
                        for (; closelist.Contains(closestr);)
                        {
                            if (flagthis)
                            {
                                closeflag = 0;
                                savetype = 1;
                                Thread.Sleep(100);
                                closecheck();
                                flagthis = false;
                            }
                            if (closeflag == 1)
                            {
                                openlist.RemoveAt(0);
                                closelist.RemoveAt(0);
                                mysql.Tables[0].Rows.RemoveAt(0);
                                flagthis = true;
                                label52text = "";
                            }
                        }
                        timers_handles.Stop();
                        t.Start();
                        m = 1;
                    }
                    //关闭失败，出库终止
                    if (closeflag == 2)
                    {
                        //MessageBox.Show("操作失败，出库终止，请重新入库剩余档案！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        label52text = "出库异常";
                        mysql4.Tables[0].Rows[mysql4.Tables[0].Rows.Count - 2 - openlist.Count + 1][2] = 0;  //入库状态，1：成功，0：失败
                        mysql5.Tables[0].Rows[mysql4.Tables[0].Rows.Count - 2 - openlist.Count + 1][5] = "出库失败";  //入库状态，1：成功，0：失败
                        string mysqlcom1 = "use " + "note19";
                        mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                        string tablename = "note1901";
                        string mysqlselect = "insert into " + tablename + " set ";
                        if (mysqlconn0.mysqlupdatecomarow1(mysqlselect, mysql4, openlist.Count - 1))
                        {

                        }
                        else
                        {
                            //创建表   
                            mysqlselect = "create table " + tablename + "(";
                            if (mysqlconn0.mysqlsavecom1(mysqlselect, mysql4, tablename, openlist.Count - 1))
                            {

                            }
                            else
                            {
                                MessageBox.Show("操作记录保存失败，请重试！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        timers_handles.Stop();
                        t.Start();
                    }
                    closeflag = 0;
                }
                catch
                {
                    if (dataGridView2.Rows.Count <= 1)
                    {
                        MessageBox.Show("无可出档案，请重新检查！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        timers_handles.Stop();
                    }
                }
            }
            timer_handlesfunflag = false;
        }
        #endregion

        #region  警告led画图填充
        public void draw(int i, Color color)
        {
            Pen pen = new Pen(Color.Black);//画笔颜色
            Brush bush = new SolidBrush(color);//填充的颜色  
            if (i == 1)
            {
                Graphics gra1 = this.pictureBox1.CreateGraphics();
                gra1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gra1.DrawEllipse(pen, 1, 1, 15, 15);//画椭圆的方法，x坐标、y坐标、宽、高，如果是100，则半径为50
                Graphics gra11 = this.pictureBox1.CreateGraphics();
                gra11.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gra11.FillEllipse(bush, 1, 1, 15, 15);//画填充椭圆的方法，x坐标、y坐标、宽、高，如果是100，则半径为50画圆圈：

            }
            if (i == 2)
            {
                Graphics gra2 = this.pictureBox2.CreateGraphics();
                gra2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gra2.DrawEllipse(pen, 1, 1, 15, 15);//画椭圆的方法，x坐标、y坐标、宽、高，如果是100，则半径为50
                Graphics gra21 = this.pictureBox2.CreateGraphics();
                gra21.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gra21.FillEllipse(bush, 1, 1, 15, 15);//画填充椭圆的方法，x坐标、y坐标、宽、高，如果是100，则半径为50画圆圈：
            }
            if (i == 3)
            {
                Graphics gra3 = this.pictureBox3.CreateGraphics();
                gra3.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gra3.DrawEllipse(pen, 1, 1, 15, 15);//画椭圆的方法，x坐标、y坐标、宽、高，如果是100，则半径为50
                Graphics gra31 = this.pictureBox3.CreateGraphics();
                gra31.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gra31.FillEllipse(bush, 1, 1, 15, 15);//画填充椭圆的方法，x坐标、y坐标、宽、高，如果是100，则半径为50画圆圈：
            }
            if (i == 4)
            {
                Graphics gra4 = this.pictureBox4.CreateGraphics();
                gra4.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gra4.DrawEllipse(pen, 1, 1, 15, 15);//画椭圆的方法，x坐标、y坐标、宽、高，如果是100，则半径为50
                Graphics gra41 = this.pictureBox4.CreateGraphics();
                gra41.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gra41.FillEllipse(bush, 1, 1, 15, 15);//画填充椭圆的方法，x坐标、y坐标、宽、高，如果是100，则半径为50画圆圈：
            }
        }
        #endregion

        #region 数据库链接初始化

        public mysqlclass mysqlconn0 = new mysqlclass();
        public List<string> list0 = new List<string>();
        private void mysqlinit(string ipstr)
        {

            string mysqlshowdatabases = "show databases";

            if (this.progressBar1.Value == 0)
            {
                mysqlconn0.mysqlsshconn(ipstr);
                list0 = mysqlconn0.mysqlshow(mysqlshowdatabases);   //获取得到数据库列表查询
                if (list0.Count() == 0)
                { return; }
                button5.BackgroundImage = Properties.Resources.sql1;
            }
            else
            {

            }

        }
        //盘点机连接检测
        public mysqlclass mysqlconn00 = new mysqlclass();  //盘点mysql
        private void mysqlinitnossh(string ipstr)
        {
            try
            {
                string mysqlshowdatabases = "show databases";
                mysqlconn00.mysqlconnn(ipstr);
                list0 = mysqlconn00.mysqlshow2(mysqlshowdatabases);   //获取得到数据库列表查询
                if (list0.Count() == 0)
                {
                    pdjsytatu = false;
                    return;
                }
                pdjsytatu = true;
            }
            catch
            {
                MessageBox.Show("盘点失败，请检查网线连接！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            }
        }

        //数据库连接检测
        private bool mysqldatacheck()
        {
            bool statusd = false;
            if (b1 == false && b2 == false && b3 == false && b4 == false)
            {
                string mysqlshowdatabases = "show databases";
                list0 = mysqlconn0.mysqlshow(mysqlshowdatabases);   //获取得到数据库列表查询
                if (list0.Count() == 0)
                { statusd = false; }
                else
                { statusd = true; }
                return statusd;
            }
            return statusd;
        }
        #endregion

        #region  温湿度,进水传感器数据读取、数值刷新等操作
        /// <summary>
        /// 定时刷新
        /// </summary>
        public List<string> numlist1 = new List<string>();  //温度数据list
        public List<string> numlist2 = new List<string>();  //湿度数据list
        public List<string> numlist3 = new List<string>();  //进水数据list
        public int start = 0;  //计时启停
        bool readflg = false;

        public void theout1(object source, System.Timers.ElapsedEventArgs e)
        {
            if (b1 == true || b2 == true||readflg) { return; }
            readflg = true;
            SetData();
            readflg = false;
        }

        //声明委托,跨线程
        private delegate void SetDataDelegate();
        private void SetData()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new SetDataDelegate(SetData));
                }
                else
                {
                    //温湿度取平均值
                    //温度
                    textBox1.Text = (((float.Parse(numlist1[0])-2.5)+ (float.Parse(numlist1[1])-3.6))/2).ToString("#0.0");
                    //湿度
                    textBox2.Text = (((float.Parse(numlist2[0])+1.9) + (float.Parse(numlist2[1])+7)) / 2).ToString("#0.0");

                    //刷新及数据各类查询刷新显示
                    refresh();

                }

                int sum = 0;
                //遍历控件，进行颜色设定
                foreach (Control contorl in this.Controls)
                {
                    if (contorl.Name == "tableLayoutPanel1")
                    {
                        //遍历容器中的控件
                        for (int i = 0; i < contorl.Controls.Count; i++)
                        {
                            if (contorl.Controls[i].Name == "panel1")
                            {
                                for (int j = 0; j < contorl.Controls[i].Controls.Count; j++)
                                {
                                    if (contorl.Controls[i].Controls[j].Name == "textBox1")  //温度
                                    {
                                        if (float.Parse(contorl.Controls[i].Controls[j].Text.ToString()) > float.Parse(temph)|| float.Parse(contorl.Controls[i].Controls[j].Text.ToString()) < float.Parse(templ))
                                        {
                                            //超阈报警 (255,128,128
                                            draw(1, System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128))))));
                                            //label21.Text = "异 常";
                                        }
                                        if (float.Parse(contorl.Controls[i].Controls[j].Text.ToString()) <= float.Parse(temph) && float.Parse(contorl.Controls[i].Controls[j].Text.ToString()) >= float.Parse(templ))
                                        {
                                            //数值正常 web.Lime
                                            draw(1, System.Drawing.Color.Lime);
                                            sum++;
                                        }
                                    }

                                    if (contorl.Controls[i].Controls[j].Name == "textBox2")  //湿度
                                    {
                                        if (float.Parse(contorl.Controls[i].Controls[j].Text.ToString()) > float.Parse(weth)|| float.Parse(contorl.Controls[i].Controls[j].Text.ToString()) < float.Parse(wetl))
                                        {
                                            //超阈报警 (255,128,128
                                            draw(2, System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128))))));
                                            //label21.Text = "异 常";
                                        }
                                        if (float.Parse(contorl.Controls[i].Controls[j].Text.ToString()) <= float.Parse(weth) && float.Parse(contorl.Controls[i].Controls[j].Text.ToString()) >= float.Parse(wetl))
                                        {
                                            //数值正常 web.Lime
                                            draw(2, System.Drawing.Color.Lime);
                                            sum++;
                                        }
                                    }

                                    if (contorl.Controls[i].Controls[j].Name == "textBox3")
                                    {
                                        if (textBox3.TabIndex == 1)
                                        {
                                            //超阈报警 1进水 0正常
                                            contorl.Controls[i].Controls[j].Text = "浸 水";
                                            draw(3, System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128))))));

                                        }
                                        if (textBox3.TabIndex == 0)
                                        {
                                            //数值正常 web.Lime
                                            contorl.Controls[i].Controls[j].Text = "正 常";
                                            draw(3, System.Drawing.Color.Lime);
                                            sum++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        
                    }
                    else
                    {
                        continue;
                    }
                }
                if (sum == 3)
                {
                    label21.Text = "正 常";
                }
                else
                {
                    label21.Text = "异 常";
                }
            } 
            catch
            { }
        }


        //开启线程
        public List<Thread> thlist = new List<Thread>();
        public Thread comlisten;
        public void dispaynum()
        {
            comlisten = new Thread(new ThreadStart(readnum));
            comlisten.IsBackground = true;  //线程转后台
            comlisten.Start();
        }

        private void readnum()
        {
            try
            {
                ThreadStart threadstart = new ThreadStart(readnum1);
                Thread thread = new Thread(threadstart);
                threadstart();
            }
            catch
            {

            }
        }

        //启动定时器检测
        public System.Timers.Timer t;
        private void readnum1()
        {
            t = new System.Timers.Timer(50);  //每50ms检测一次
            //t.Interval=600;
            t.Elapsed += new System.Timers.ElapsedEventHandler(theout);
            t.AutoReset = true;  //true一直执行,false执行一次
            t.Enabled = true;   //是否执行Elapsed事件
        }

        public List<SerialPort> portlist1 = new List<SerialPort>();  //存储实际串口类型
        public int count1 = 0;
        public bool theoutflag = false;  //用于保证dataGridview3显示正确的标志位
        public void theout(object source, System.Timers.ElapsedEventArgs e)
        {
            if (theoutflag)
            { return; }
            theoutflag = true;
            count1++;
            serialport1 serial = new serialport1();
            List<byte[]> read1_comm32 = new List<byte[]> { };  //温度
            List<byte[]> read2_comm32 = new List<byte[]> { };  //湿度
            List<byte[]> read3_comm32 = new List<byte[]> { };  //进水
            List<byte> databack1 = new List<byte>(5);
            List<byte> databack2 = new List<byte>(5);
            List<byte> databack3 = new List<byte>(7);
            List<int> databack = new List<int>(10);
            
            for (int j = 0; j < 9; j++)
            {
                databack1.Add(0x0a);
                databack2.Add(0x0a);
            }
            byte sum = 0;

            //站101温湿度模块地址分配
            byte[] readnum1_comm32 = { 0x65, 0x03, 0x00, 0x01, 0x00, 0x01, 0xdd, 0xee };  //查101读温度
            readnum1_comm32 = crce(readnum1_comm32);
            byte[] readnum2_comm32 = { 0x65, 0x03, 0x00, 0x02, 0x00, 0x01, 0x2d, 0xee };  //查101读湿度
            byte[] readnum3_comm32 = { 0x14, 0x04, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00 };  //查1地址进水状态（浸水传感器布置在10号柜）
            readnum3_comm32 = crce(readnum3_comm32);
            read1_comm32.Add(readnum1_comm32);
            read2_comm32.Add(readnum2_comm32);
            read3_comm32.Add(readnum3_comm32);

            //站116温湿度模块地址分配
            byte[] readnum1_comm33 = { 0x66, 0x03, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00 };  //查102读温度  //实地验证完整命令{ 0x66, 0x03, 0x00, 0x01, 0x00, 0x01, 0xde, 0xaf }
            readnum1_comm33 = crce(readnum1_comm33);
            byte[] readnum2_comm33 = { 0x66, 0x03, 0x00, 0x02, 0x00, 0x01, 0x00, 0x00 };  //查102读湿度  //实地验证完整命令{0x66, 0x03, 0x00, 0x02, 0x00, 0x01, 0x2e, 0xaf}
            readnum2_comm33 = crce(readnum2_comm33);
            byte[] readnum3_comm33 = { 0x14, 0x04, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00 };  //查1地址进水状态（浸水传感器布置在10号柜）
            readnum3_comm33 = crce(readnum3_comm33);
            read1_comm32.Add(readnum1_comm33);
            read2_comm32.Add(readnum2_comm33);
            read3_comm32.Add(readnum3_comm33);

            //命令串口初始,串口查询
            sum = 0;
            byte[] str1 = { 0x65, 0x03, 0x00, 0x01, 0x00, 0x01, 0xDD, 0xEE };   //查询101站温度
            SerialPort port2 = new SerialPort();
            try
            {
                for (i = 0; i < comlist.Count(); i++)
                {
                    try
                    {
                        if (portlist1.Count == 0&& comlist[i]=="COM4")
                        {
                            port2.PortName = comlist[i];
                            port2.BaudRate = 9600;           //波特率
                            port2.Parity = Parity.None;      //奇偶校验
                            port2.DataBits = 8;              //数据位
                            port2.StopBits = StopBits.One;   //停止位
                            port2.ReadTimeout = 500;         //读超时
                            port2.Open();

                            portlist1.Clear();
                            portlist1.Add(port2);
                            port32 = port2;
                            break;
                                                        
                            port2.Write(str1, 0, str1.Length);      //质询下位机端口,寻找连接端口,查101温度
                            Thread.Sleep(50);                       //等待数据接收完成，100ms
                            if (port2.BytesToRead != 0)
                            {
                                portlist1.Clear();
                                portlist1.Add(port2);
                                port32 = port2;
                                //break;
                            }
                            else
                            {
                                port2.Close();
                            }
                        }
                        else
                        {

                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            catch
            {

            }

            #region  tcp-test
            //numlist1[0] = "25.5";
            //numlist2[0] = "44.2";
            //numlist3[0] = "0.0";
            //socknet.refresh(this,numlist1, numlist2, numlist3);
            #endregion

            //命令下发，数据回传
            for (int i = 0; i < read1_comm32.Count(); i++)
            {
                try
                {
                    databack1 = serial.senddata(read1_comm32[i], portlist1, port32);
                    Thread.Sleep(20);
                    string numstr1 = ((float)(Convert.ToDouble((Convert.ToInt32(databack1[3] << 8) + databack1[4])) / 100)).ToString();
                    numlist1[i] = numstr1; //温度换算后数据值                       

                    databack2 = serial.senddata(read2_comm32[i], portlist1, port32);
                    Thread.Sleep(20);
                    string numstr2 = ((float)(Convert.ToDouble((Convert.ToInt32(databack2[3] << 8) + databack2[4])) / 100)).ToString();
                    numlist2[i] = numstr2; //湿度换算后数据值

                    //查进水传感器
                    Thread.Sleep(30);
                    databack3 = serial.senddata3(read3_comm32[i], portlist1, port32);
                    if (databack3 != null)
                    {
                        textBox3.TabIndex = (int)(Convert.ToDouble(Convert.ToInt32(databack3[4])));
                        numlist3[i] = textBox3.TabIndex.ToString();
                    }   
                    //查485通信线
                    if (databack3.Count != 0)
                    {
                        controlboard = true;
                    }
                    else
                    {
                        controlboard = false;
                    }
                }
                catch
                {

                }
            }
            //开关门状态查询
            check_doorstatus();
            //远程监控计算机查温湿度
            socknet.refresh(this, numlist1, numlist2, numlist3);
            theoutflag = false;
        }
        #endregion

        #region   柜门检测命令集生成
        public List<string> doorlistopen = new List<string>();
        public List<string> doorlistclose = new List<string>();
        public void check_doorstatus()
        {
            int boxx = 1,mm=0, door_opennum1=0;
            List<byte> databack4;
            List<int> doorstatus_11;
            List<int> doorstatus_22;
            
            byte[] close_comm33 = { 0x00, 0x04, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00 };  //读门的状态
            try
            {
                for (int i = 1; i < (int.Parse(controlsum) + 2); i++)
                {
                    try
                    {
                        close_comm33[0] = Convert.ToByte(i);   //控制板地址
                        //通道1: 1-16 
                        close_comm33[3] = 0x03;   //通道1
                        //crc校验
                        close_comm33 = crce(close_comm33);
                        //关门检测
                        Thread.Sleep(20);
                        databack4 = serial.senddata(close_comm33, portlist1, port32);
                        Thread.Sleep(20);
                        doorstatus_11 = check_door(databack4[3], databack4[4], 1);
                        //通道2:17-32
                        close_comm33[3] = 0x04;   //通道2
                        //crc校验
                        close_comm33 = crce(close_comm33);
                        //关门检测
                        Thread.Sleep(20);
                        databack4 = serial.senddata(close_comm33, portlist1, port32);
                        Thread.Sleep(20);
                        doorstatus_22 = check_door(databack4[3], databack4[4], 2);
                        door_opennum1 += doorstatus_11.Count + doorstatus_22.Count;
                        //柜体状态列表统计
                        //for (i = 0; i < doorstatus_11.Count; i++)
                        //{
                        //    doorlistopen.Add(i.ToString() + "-" + doorstatus_11[i].ToString());
                        //}
                        //for (i = 0; i < doorstatus_22.Count; i++)
                        //{
                        //    doorlistopen.Add(i.ToString() + "-" + doorstatus_22[i].ToString());
                        //}

                        for (boxx = 1; boxx < 31; boxx++)
                        {
                            if (doorstatus_11.IndexOf(boxx) < 0 || doorstatus_22.IndexOf(boxx) < 0)
                            {
                                //if ((doorlistclose.IndexOf(i.ToString() + "-" + boxx.ToString())) < 0)
                                //{
                                //    doorlistclose.Add(i.ToString() + "-" + boxx.ToString());
                                //}
                                if ((doorlistopen.IndexOf(i.ToString() + "-" + boxx.ToString())) >= 0)
                                {
                                    doorlistopen.Remove(i.ToString() + "-" + boxx.ToString());
                                }
                            }
                            if (doorstatus_11.IndexOf(boxx) >= 0 || doorstatus_22.IndexOf(boxx) >= 0)
                            {
                                if ((doorlistopen.IndexOf(i.ToString() + "-" + boxx.ToString())) < 0)
                                {
                                    doorlistopen.Add(i.ToString() + "-" + boxx.ToString());
                                }
                                //if ((doorlistclose.IndexOf(i.ToString() + "-" + boxx.ToString())) >= 0)
                                //{
                                //    doorlistclose.Remove(i.ToString() + "-" + boxx.ToString());
                                //}
                            }
                        }
                    }
                    catch
                    { }       
                }
                //数量统计
                if (door_opennum1 != door_opennum|| door_opennum1==0)
                {
                    door_opennum = door_opennum1;
                    door_closenum = (int.Parse(controlsum) + 1) * 30 - door_opennum1;
                    if (door_opennum1 == 0)
                    {
                        mysql_openview.Clear();
                    }
                }
                //door_opennum = doorlistopen.Count;
                //door_closenum = (int.Parse(controlsum) + 1) * 30 - door_opennum;
                //开门数据集填充
                if (door_opennum != mm)
                {
                    mm = door_opennum;
                    checkmysqlflag = false;
                    if (mysql_openview.Tables[0].Rows.Count >= 0)
                    {
                        mysql_openview.Clear();
                    }                    
                    vertloction3 = 0;
                    for (int i = 0; i < doorlistopen.Count; i++)
                    {
                        mysql_openview.Tables[0].Rows.Add();
                        mysql_openview.Tables[0].Rows[i][0] = doorlistopen[i];
                        mysql_openview.Tables[0].Rows[i][1] = "-";
                    }
                    checkmysqlflag = true;
                }                                              
            }
            catch
            {

            }
        }

        public void check_doorstatus_1(int ii)
        {
            int boxx = 1, mm = 0, door_opennum1 = 0;
            List<byte> databack4;
            List<int> doorstatus_11;
            List<int> doorstatus_22;

            byte[] close_comm33 = { 0x00, 0x04, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00 };  //读门的状态
            try
            {
                try
                {
                    close_comm33[0] = Convert.ToByte(ii);   //控制板地址
                    //通道1: 1-16 
                    close_comm33[3] = 0x03;   //通道1
                                              //crc校验
                    close_comm33 = crce(close_comm33);
                    //关门检测
                    Thread.Sleep(20);
                    databack4 = serial.senddata(close_comm33, portlist1, port32);
                    Thread.Sleep(20);
                    doorstatus_11 = check_door(databack4[3], databack4[4], 1);
                    //通道2:17-32
                    close_comm33[3] = 0x04;   //通道2
                                              //crc校验
                    close_comm33 = crce(close_comm33);
                    //关门检测
                    Thread.Sleep(20);
                    databack4 = serial.senddata(close_comm33, portlist1, port32);
                    Thread.Sleep(20);
                    doorstatus_22 = check_door(databack4[3], databack4[4], 2);
                    door_opennum1 += doorstatus_11.Count + doorstatus_22.Count;
                    
                    //柜体状态列表统计
                    for (boxx = 1; boxx < 31; boxx++)
                    {
                        if (doorstatus_11.IndexOf(boxx) < 0 || doorstatus_22.IndexOf(boxx) < 0)
                        {

                            if ((doorlistopen.IndexOf(ii.ToString() + "-" + boxx.ToString())) >= 0)
                            {
                                doorlistopen.Remove(ii.ToString() + "-" + boxx.ToString());
                            }
                        }
                        if (doorstatus_11.IndexOf(boxx) >= 0 || doorstatus_22.IndexOf(boxx) >= 0)
                        {
                            if ((doorlistopen.IndexOf(ii.ToString() + "-" + boxx.ToString())) < 0)
                            {
                                doorlistopen.Add(ii.ToString() + "-" + boxx.ToString());
                            }

                        }
                    }
                }
                catch
                { }
                //数量统计
                if (door_opennum1 != door_opennum || door_opennum1 == 0)
                {
                    door_opennum = door_opennum1;
                    door_closenum = (int.Parse(controlsum) + 1) * 30 - door_opennum1;
                    if (door_opennum1 == 0)
                    {
                        mysql_openview.Clear();
                    }
                }

                //开门数据集填充
                if (door_opennum != mm)
                {
                    mm = door_opennum;
                    checkmysqlflag = false;
                    if (mysql_openview.Tables[0].Rows.Count >= 0)
                    {
                        mysql_openview.Clear();
                    }
                    vertloction3 = 0;
                    for (int i = 0; i < doorlistopen.Count; i++)
                    {
                        mysql_openview.Tables[0].Rows.Add();
                        mysql_openview.Tables[0].Rows[i][0] = doorlistopen[i];
                        mysql_openview.Tables[0].Rows[i][1] = "-";
                    }
                    checkmysqlflag = true;
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 串口设备链接初始化
        //public bool t = false;
        private void devlinit()
        {
            Thread comlisten = new Thread(new ThreadStart(comlistrn_fun_1));
            comlisten.IsBackground = true;  //线程转后台
            comlisten.Start();
        }

        //开启线程
        private void comlistrn_fun_1()
        {
            while (true)
            {
                try
                {
                    ThreadStart threadstart = new ThreadStart(comlistrn_fun);
                    Thread thread = new Thread(threadstart);
                    threadstart();
                }
                catch
                {
                    //t = false;
                    break;
                }
            }
        }

        public List<string> comlist = null;
        private void comlistrn_fun()
        {
            firtdoor f1 = new firtdoor(0, null);
            serialport com = new serialport();
            comlist = com.serialportcheck();
            Thread.Sleep(100);

        }
        #endregion

        #region 打印机（USB）设备链接初始化

        private string printlinit()
        {
            //读取默认打印机

            try
            {
                PrintDocument printd = new PrintDocument();
                string portname = printd.PrinterSettings.PrinterName; //获取默认打印机名
                return portname;
            }
            catch
            {
                MessageBox.Show("未找到默认打印机！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return "";
            }
        }

        #endregion

        #region resize 等比例调整控件大小 （未用）

        private void setTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ";" + con.Height + ";" + con.Left + ";" + con.Top + ";" + con.Font.Size;
                if (con.Controls.Count > 0)
                {
                    setTag(con);
                }
            }
        }

        #endregion

        #region  整理出入库

        private void button49_Click(object sender, EventArgs e)
        {
            form_dill dill = new form_dill(this);
            dill.ShowDialog();
            refresh_checkdata();
        }
        #endregion

        #region 新增、借阅、归还、盘点功能函数

        //新增入库b1
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //t.Stop();
                this.tabPage1.Parent = tabControl1;
                this.tabPage5.Parent = null;
                //tabControl1.SelectedTab = this.tabPage1;  //选中当前page
                b1 = true;
                init1();
                return;
            }
            catch
            {
                MessageBox.Show("请检查控制板通信线连接！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            }

        }

        //入库退出
        private void button16_Click(object sender, EventArgs e)
        {
            //t.Start();
            this.tabPage1.Parent = null;
            this.tabPage5.Parent = tabControl1;
            button8.BackgroundImage = Properties.Resources.devlink2;       //扫码枪断开连接图标
            progressBar4.Value = 0;
            init1close();
            b1 = false;
            return;
        }

        //借阅出库b2
        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                t.Stop();
                this.tabPage2.Parent = tabControl1;
                this.tabPage5.Parent = null;
                b2 = true;
                init2();
                return;
            }
            catch
            {
                MessageBox.Show("请检查扫描器连接！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            }
        }

        //出库退出
        private void button17_Click(object sender, EventArgs e)
        {
            t.Start();
            this.tabPage2.Parent = null;
            this.tabPage5.Parent = tabControl1;
            button8.BackgroundImage = Properties.Resources.devlink2;       //扫码枪断开连接图标
            progressBar4.Value = 0;
            init1close();
            b2 = false;
            textBox6.Clear();
            return;
        }

        //档案归还b3
        private void button3_Click_2(object sender, EventArgs e)
        {
            try
            {
                t.Stop();
                this.tabPage3.Parent = tabControl1;
                this.tabPage5.Parent = null;
                b3 = true;
                init3();
                return;
            }
            catch
            {
                MessageBox.Show("请检查控制板通信线连接！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            }
        }

        //归还结束
        private void button18_Click(object sender, EventArgs e)
        {
            t.Start();
            this.tabPage3.Parent = null;
            this.tabPage5.Parent = tabControl1;
            button8.BackgroundImage = Properties.Resources.devlink2;       //扫码枪断开连接图标
            progressBar4.Value = 0;
            init1close();
            b3 = false;
            textBox5.Clear();
            return;
        }

        #endregion

        #region   新增、借阅、归还、盘点功能界面初始化

        //新增入库
        public void init1()
        {
            if (operate_type == "I1")
            {
                //    //扫码连接设备
                //load_exdata("ex", "exdata", "dispydata", "dispy_instatus");
                if (qrsm.openDevice(1))
                {
                    //MessageBox.Show("连接设备成功");
                    //添加设备支持的码制
                    if (qrsm.addCodeFormat((byte)1))
                    {
                        //MessageBox.Show("添加二维码成功");
                        //引入模板
                        //load_exdata("ex", "exdata", "dispydata", "dispy_instatus");
                        //开灯
                        qrsm.backlight(true);
                        //蜂鸣2次
                        qrsm.beepControl(2);
                        //计数id
                        jieyuecunt = 0;
                        if (count != 1)
                        {
                            //查询存储id号
                            chenkcount();
                        }
                        Control.CheckForIllegalCrossThreadCalls = false;   //跨线程调用窗体控件的初始化设置
                    }
                }
                else
                {
                    //MessageBox.Show("连接设备失败");
                    t.Start();
                }
                //查位置编号及count
                chenkcount();
                //桌面读卡器初始化
                //openport();
            }
            else
            {
                //导入数据库
                //load_exdata("ex", "exdata", "dispydata", "dispy_instatus");
                //桌面读卡器初始化
                //openport();
            }
        }

        //借阅 扫码设备初始化
        private void init2()
        {
            //无二维码情况
            load_exdata("ex", "exdata", "dispydata", "dispy_instatus");
            //连接设备
            if (qrsm.openDevice(1))
            {
                //MessageBox.Show("连接设备成功");
                //添加设备支持的码制
                if (qrsm.addCodeFormat((byte)1))
                {
                    //MessageBox.Show("添加二维码成功");
                    //引入模板
                    load_exdata("ex", "exdata", "dispydata", "dispy_instatus");
                    //开灯
                    qrsm.backlight(true);
                    //蜂鸣2次
                    qrsm.beepControl(2);
                    //计数id
                    jieyuecunt = 0;
                    if (count != 1)
                    {
                        //查询存储id号
                        chenkcount();
                    }
                    Control.CheckForIllegalCrossThreadCalls = false;   //跨线程调用窗体控件的初始化设置
                }
            }
            else
            {
                //MessageBox.Show("连接设备失败");
                t.Start();
            }
        }

        //归还
        private void init3()
        {

            //连接设备
            if (qrsm.openDevice(1))
            {
                //MessageBox.Show("连接设备成功");
                //添加设备支持的码制
                if (qrsm.addCodeFormat((byte)1))
                {
                    //MessageBox.Show("添加二维码成功");
                    //引入模板
                    load_exdata("ex", "exdata", "dispydata", "dispy_instatus");
                    //dataGridView3.ReadOnly = true;    //dataGridV只读状态

                    button8.BackgroundImage = Properties.Resources.devlink1;       //扫码枪已经正常连接图标
                    progressBar4.Value = 100;
                    //开灯
                    qrsm.backlight(true);
                    //蜂鸣2次
                    qrsm.beepControl(2);
                    //计数id
                    jieyuecunt = 0;

                    Control.CheckForIllegalCrossThreadCalls = false;   //跨线程调用窗体控件的初始化设置
                }

            }
            else
            {
                //MessageBox.Show("连接设备失败");
                t.Start();
                this.tabPage3.Parent = null;
                this.tabPage5.Parent = tabControl1;
            }
            qrresult();
        }
        #endregion

        #region   扫码功能关闭

        public void init1close()
        {
            //t.Start();
            //停止解码
            StopDecodeThread();
            //关灯
            qrsm.backlight(false);
            //蜂鸣2次
            qrsm.beepControl(2);
            //关闭设备连接
            qrsm.disConnected();
        }

        public void qrsm_1()
        {
            qrsm.beepControl(1);
        }

        #endregion 

        #region 扫码线程

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
        //委托引用 线程
        private void DecodeThreadMethod()
        {
            datagrid1dill datagrid1dillRef_Instance = new datagrid1dill(listenport_receive);
            do
            {
                //decoderesult = Decoder();  有扫码枪时使用
                readrfid();
                if (CardNum != 0 || decoderesult != null)
                {
                    if (decoderesult != null)
                    {
                        qrsm.beepControl(1);
                    }
                    this.Invoke(datagrid1dillRef_Instance, new object[] { this.dataGridView1, this.dataGridView2, this.dataGridView3, this.textBox6, this.textBox5, this.button31, this.button32, this.textBox4, this.textBox12, this.textBox27, this.textBox28, this.textBox29, this.textBox30,this.textBox31,this.textBox32, this.textBox33, this.textBox34, this.textBox35, this.textBox36, decoderesult });
                    decoderesult = null;
                    CardNum = 0;
                }
            }
            while (bIsLoop);
        }

        //具体处理，数据接收处理
        //扫码枪
        //private List<byte> buffer = new List<byte>();  //定义列表成员，存放buf数据  
        private void listenport_receive(DataGridView datagrid1, DataGridView datagrid2, DataGridView datagrid3, TextBox txtbox, TextBox txtbox5, Button button31, Button button32, TextBox textbox4, TextBox textbox12, TextBox textbox27, TextBox textbox28, TextBox textbox29, TextBox textbox30, TextBox textbox31, TextBox textbox32, TextBox textbox33, TextBox textbox34, TextBox textbox35, TextBox textbox36, string strbuf)
        {
            string showstr1="", showstr2="";
            List<byte> buffer = new List<byte>();  //定义列表成员，存放buf数据
            List<string> messagex;   //数据解析结果
            List<string> chenstr1, chenstr2, chenstr3;   //数据解析结果
            int n = 0;
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
            if (CardNum != 0)
            {
                sEPC = "%" + sEPC;
                n = sEPC.Length;
                //判断串口数据来源
                for (i = 0; i < sEPC.Length; i++)
                {
                    buf[i] = Convert.ToByte(sEPC[i]);
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

                //二维码数据解析函数
                messagex = danalysis(buffer1, DateTime.Now.ToString());

                //RFID读卡器数据解析
                //messagex = rfidread(buffer1, DateTime.Now.ToString());

                if (messagex != null)
                {
                    //入库
                    if (b1 == true&&operate_type=="I1")
                    {
                        //单盒入库
                        if (messagex[0] == "1")
                        {
                            button31.Enabled = false;
                            button32.Enabled = false;
                            textbox29.Text = messagex[2];
                            return;
                        }
                        //批盒入库
                        if (messagex[0] == "2")
                        {
                            button29.Enabled = false;
                            button30.Enabled = false;
                            textbox29.Text = messagex[2];
                            return;
                        }
                        //判断是否扫描入库申请二维码
                        if (button29.Enabled == true && button30.Enabled == true && button31.Enabled == true && button32.Enabled == true)
                        {
                            MessageBox.Show("请扫描入库申请二维码！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);   //档案已经入库判断
                            return;
                        }
                        //判断是否已经入库
                        chenstr1 = chenking(messagex[0]);
                        if (chenstr1.Count == 0)
                        { }

                        else if (chenstr1[0] == "1")
                        {
                            killtxt = "ERROR";
                            startkill();
                            MessageBox.Show("该档案已入库！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);   //档案已经入库判断
                            return;
                        }

                        if (textbox4.Text == messagex[0] || textbox12.Text == messagex[0] || textbox27.Text == messagex[0])
                        {
                            killtxt = "Warning";
                            startkill();
                            MessageBox.Show("已扫描，请检查！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        //填写对应信息
                        form_newinput f2 = new form_newinput(this);                       
                        f2.ShowDialog();
                        
                        if (f2.textBox6.Text == "" || f2.textBox1.Text == "")
                        { return; }

                        //单量入库操作
                        if (button31.Enabled == false)
                        {

                            //查询在库
                            chenstr1 = chenking(f2.textBox1.Text);

                            if (chenstr1.Count != 0)
                            {
                                if (chenstr1[0] == "0" && chenstr1.Count == 1)
                                {
                                    killtxt = "Warning";
                                    startkill();
                                    MessageBox.Show("单人档案数已达上限（3份）！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);   //档案已经超限判断
                                    //mysql2.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = "入库失败";  //入库编码
                                    return;
                                }
                                if (int.Parse(chenstr1[2]) == 3)
                                {
                                    if (box < 30)
                                    {
                                        num = int.Parse(chenstr1[0]);
                                        box = int.Parse(chenstr1[1]) + 1;
                                    }
                                    else
                                    {
                                        num = int.Parse(chenstr1[0])+1;
                                        box = 1;
                                    }
                                }
                                else
                                {
                                    num = int.Parse(chenstr1[0]);
                                    box = int.Parse(chenstr1[1]);
                                }
                                ////手动输入柜号判断
                                //if (f2.comboBox1.SelectedIndex == 1)
                                //{
                                //    num = f2.comboBox2.SelectedIndex + 1;
                                //    box = f2.comboBox3.SelectedIndex + 1;
                                //    label32.Text = "档案柜位置编号（手动）：";
                                //    //暂没有加柜体是否满的判断
                                //}
                                danalysis1(num, box);
                            }

                            if (textbox12.Text == "" && f2.textBox1.Text != textBox28.Text && textbox28.Text != "")
                            {
                                killtxt = "Warning";
                                startkill();
                                MessageBox.Show("该档案不属于同一人，请检查！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if (textbox12.Text == "" && chenstr1.Count != 0 && chenstr1[2] == "2" && textbox4.Text != "")
                            {
                                killtxt = "Warning";
                                startkill();
                                MessageBox.Show("单人档案数已达上限（3份）！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);   //档案已经超限判断
                                return;
                            }
                            if (textbox27.Text == "" && f2.textBox1.Text != textBox28.Text && textbox28.Text != "")
                            {
                                killtxt = "Warning";
                                startkill();
                                MessageBox.Show("该档案不属于同一人，请检查！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if (textbox27.Text == "" && chenstr1.Count != 0 && chenstr1[2] == "1" && textbox12.Text != "")
                            {
                                killtxt = "Warning";
                                startkill();
                                MessageBox.Show("单人档案数已达上限（3份）！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);   //档案已经超限判断
                                return;
                            }

                            num1 = num;
                            box1 = box;

                            textbox30.Text = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号

                            mysql.Tables[0].Rows.Add();
                            mysql4.Tables[0].Rows.Add();
                            mysql5.Tables[0].Rows.Add();
                            //operate_id操作员编号
                            mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = textbox29.Text;
                            //查note数据库记录条数

                            //读取tid编号
                            if (textbox28.Text == "")
                            {
                                textbox28.Text = f2.textBox1.Text;  //工号
                                textbox4.Text = messagex[0];        //tid
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = count;  //记id
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = count++;  //记id
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = messagex[0];  //TID号
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = messagex[0];  //TID号    
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = messagex[0];  //TID号 
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] ="new";  //入库                     
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][2] = 1;  //入库状态，1：在库，0：出库
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = f2.textBox6.Text;  //name  姓名                           
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = f2.textBox1.Text;  //perid 工号
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = f2.textBox1.Text; //perid 工号
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = f2.textBox1.Text; //perid 工号
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][7] = textbox29.Text;  //入库操作员编码
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][7] = textbox29.Text;  //入库操作员编码
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][5] = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号 
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][5] = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号 
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号                       
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][2] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                                return;
                            }
                            else if (textbox12.Text == "")
                            {

                                textbox28.Text = f2.textBox1.Text;  //工号
                                textbox12.Text = messagex[0];        //tid
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = count;  //记id
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = count++;  //记id
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = messagex[0];  //TID号  
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = messagex[0];  //TID号 
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = messagex[0];  //TID号  
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = "new";  //入库                         
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][2] = 1;  //入库状态，1：在库，0：出库
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = f2.textBox6.Text;  //name  姓名                           
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = f2.textBox1.Text;  //perid 工号
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = f2.textBox1.Text;  //perid 工号
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = f2.textBox1.Text; //perid 工号
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][7] = textbox29.Text;  //入库操作员编码
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][7] = textbox29.Text;  //入库操作员编码
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][5] = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号 
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][5] = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号                         
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][2] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                                return;
                            }
                            else if (textbox27.Text == "")
                            {

                                textbox28.Text = f2.textBox1.Text;  //工号
                                textbox27.Text = messagex[0];       //tid
                                count = count + 1;
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = count;  //记id
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = count++;  //记id
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = messagex[0];  //TID号
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = messagex[0];  //TID号
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = messagex[0];  //TID号
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = "new";  //入库                             
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][2] = 1;  //入库状态，1：在库，0：出库
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = f2.textBox6.Text;  //name  姓名                           
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = f2.textBox1.Text;  //perid 工号
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = f2.textBox1.Text;  //perid 工号
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = f2.textBox1.Text; //perid 工号
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][7] = textbox29.Text;  //入库操作员编码
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][7] = textbox29.Text;  //入库操作员编码
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][5] = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号 
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][5] = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号                        
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][2] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                                return;
                            }

                        }

                        //批量入库操作
                        if (button31.Enabled == true)
                        {
                            //查询在库
                            chenstr1 = chenking(f2.textBox1.Text);
                            if (chenstr1.Count != 0)
                            {
                                if (chenstr1[0] == "0" && chenstr1.Count == 1)
                                {
                                    killtxt = "Warning";
                                    startkill();
                                    MessageBox.Show("单人档案数已达上限（3份）！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);   //档案已经超限判断                               
                                    return;
                                }
                                num = int.Parse(chenstr1[0]);
                                box = int.Parse(chenstr1[1]);
                                danalysis1(num, box);
                            }
                            if (textbox27.Text != "" && textbox28.Text == f2.textBox1.Text)
                            {
                                killtxt = "Warning";
                                startkill();
                                MessageBox.Show("单人档案数已达上限（3份）！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);   //档案已经超限判断
                                return;
                            }
                            if (textbox12.Text != "" && chenstr1.Count != 0 && chenstr1[2] == "1" && textbox28.Text == f2.textBox1.Text)
                            {
                                killtxt = "Warning";
                                startkill();
                                MessageBox.Show("单人档案数已达上限（3份）！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);   //档案已经超限判断
                                return;
                            }
                            if (textbox4.Text != "" && chenstr1.Count != 0 && chenstr1[2] == "2" && textbox28.Text == f2.textBox1.Text)
                            {
                                killtxt = "Warning";
                                startkill();
                                MessageBox.Show("单人档案数已达上限（3份）！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);   //档案已经超限判断
                                return;
                            }

                            //库存是否存在该档案
                            if (chenstr1.Count == 0 && textbox4.Text != "" && textbox28.Text != "*****" + f2.textBox1.Text.Substring(f2.textBox1.Text.Length - 4, 4))
                            {
                                //自加
                                box++;
                                if (box == 31)
                                {
                                    box = 1;
                                    num++;
                                }
                            }
                            //批量使判断是否为同一人
                            if (textbox28.Text != f2.textBox1.Text)
                            {
                                textBox4.Text = "";
                                textBox12.Text = "";
                                textBox27.Text = "";
                            }
                            //手动输入柜号判断
                            //if (f2.comboBox1.SelectedIndex == 1)
                            //{
                            //    num = f2.comboBox2.SelectedIndex + 1;
                            //    box = f2.comboBox3.SelectedIndex + 1;
                            //    label32.Text = "档案柜位置编号（手动）：";
                            //    //暂没有加柜体是否满的判断
                            //}
                            //命令组包
                            danalysis1(num, box);

                            mysql.Tables[0].Rows.Add();
                            mysql2.Tables[0].Rows.Add();
                            mysql4.Tables[0].Rows.Add();
                            mysql5.Tables[0].Rows.Add();
                            //operate_id操作员编号
                            mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = textbox29.Text;

                            //左提示显示
                            if (textBox4.Text == "")
                            {
                                //加入开锁命令列表，用于批量开锁
                                openlist.Add(comm3);
                                closelist.Add(num.ToString() + "-" + box.ToString());
                                //对f2.textBox1.Text进行简洁显示(只截取后4位)
                                showstr1= "*****" + f2.textBox1.Text.Substring(f2.textBox1.Text.Length - 4, 4);
                                showstr2 = messagex[0].Replace("00000","*");

                                textbox28.Text = showstr1;  //工号
                                textbox4.Text = showstr2;        //tid

                                datagrid1.DataSource = mysql2.Tables[0];  //导入模板
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = count;  //记id
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = count++;  //记id
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = messagex[0];  //TID号
                                mysql2.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = showstr2;  //TID号
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = messagex[0];  //TID号
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = messagex[0];  //TID号
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = "new";  //入库   
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][2] = 1;  //入库状态，1：在库，0：出库
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = f2.textBox6.Text;  //name  姓名
                                //mysql2.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = f2.textBox1.Text;  //perid 工号
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = f2.textBox1.Text;  //perid 工号
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = f2.textBox1.Text;  //perid 工号
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = f2.textBox1.Text;  //perid 工号
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][7] = textbox29.Text;   //入库操作员编码
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][7] = textbox29.Text;  //入库操作员编码
                                num1 = num;
                                box1 = box;
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][5] = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号
                                mysql2.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][5] = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号
                                textbox30.Text = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());  //转换为mysqltime时间格式                                                                                                                                                   //}
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());  //转换为mysqltime时间格式 
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][2] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                                datagrid1.DataSource = mysql2.Tables[0];                               
                                //datagrid1.ClearSelection();     //取消dataGridView选中焦点状态
                                //datagrid1.Rows[0].Selected = true;  //选中最新行
                                //datagrid1.FirstDisplayedScrollingRowIndex = mysql.Tables[0].Rows.Count - 2;
                                datagrid1.Columns[0].Width = 200;  //id列的宽度设置
                                datagrid1.Columns[1].Width = 80;  //tid列的宽度设置
                                return;
                            }
                            else if (textbox28.Text == f2.textBox1.Text && textBox12.Text == "")
                            {

                                //加入开锁命令列表，用于批量开锁
                                openlist.Add(comm3);
                                closelist.Add(num.ToString() + "-" + box.ToString());
                                //对f2.textBox1.Text进行简洁显示(只截取后4位)
                                showstr1 = "*****" + f2.textBox1.Text.Substring(f2.textBox1.Text.Length - 4, 4);
                                showstr2 = messagex[0].Replace("00000", "*");

                                textbox28.Text = showstr1;  //工号
                                textbox12.Text = showstr2;        //tid

                                datagrid1.DataSource = mysql2.Tables[0];  //导入模板
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = count;  //记id
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = count++;  //记id
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = messagex[0];  //TID号
                                mysql2.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = showstr2;  //TID号
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = messagex[0];  //TID号
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = messagex[0];  //TID号
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = "new";  //入库   
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][2] = 1;  //入库状态，1：在库，0：出库
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = f2.textBox6.Text;  //name  姓名
                                //mysql2.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = f2.textBox1.Text;  //name  姓名
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = f2.textBox1.Text;  //perid 工号
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = f2.textBox1.Text;  //perid 工号
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = f2.textBox1.Text;  //perid 工号
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][7] = textbox29.Text;   //入库操作员编码
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][7] = textbox29.Text;  //入库操作员编码
                                num1 = num;
                                box1 = box;
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][5] = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号
                                mysql2.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][5] = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号
                                textbox30.Text = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());  //转换为mysqltime时间格式 
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());  //转换为mysqltime时间格式
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][2] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                                datagrid1.DataSource = mysql2.Tables[0];
                                //datagrid1.ClearSelection();     //取消dataGridView选中焦点状态
                                //datagrid1.Rows[0].Selected = true;  //选中最新行
                                //datagrid1.FirstDisplayedScrollingRowIndex = mysql.Tables[0].Rows.Count - 2;
                                datagrid1.Columns[0].Width = 200;  //id列的宽度设置
                                datagrid1.Columns[1].Width = 80;  //tid列的宽度设置                                           
                                return;
                            }
                            else if (textbox28.Text == f2.textBox1.Text && textbox27.Text == "")
                            {
                                //加入开锁命令列表，用于批量开锁
                                openlist.Add(comm3);
                                closelist.Add(num.ToString() + "-" + box.ToString());
                                //对f2.textBox1.Text进行简洁显示(只截取后4位)
                                showstr1 = "*****" + f2.textBox1.Text.Substring(f2.textBox1.Text.Length - 4, 4);
                                showstr2 = messagex[0].Replace("00000", "*");

                                textbox28.Text = showstr1;  //工号
                                textbox27.Text = showstr2;        //tid

                                datagrid1.DataSource = mysql2.Tables[0];  //导入模板
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = count;  //记id
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = count++;  //记id
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = messagex[0];  //TID号
                                mysql2.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = showstr2;  //TID号
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = messagex[0];  //TID号
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = messagex[0];  //TID号
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = "new";  //入库   
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][2] = 1;  //入库状态，1：在库，0：出库
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = f2.textBox6.Text;  //name  姓名
                                //mysql2.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = f2.textBox1.Text;  //name  姓名
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = f2.textBox1.Text;  //perid 工号
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = f2.textBox1.Text;  //perid 工号
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = f2.textBox1.Text;  //perid 工号
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][7] = textbox29.Text;  //入库操作员编码
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][7] = textbox29.Text;  //入库操作员编码
                                num1 = num;
                                box1 = box;
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][5] = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号
                                mysql2.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][5] = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号
                                textbox30.Text = num1.ToString() + "-" + box1.ToString();  //确定柜号+门号   
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());  //转换为mysqltime时间格式                                                                                                                                             
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());  //转换为mysqltime时间格式                                                                                                                                               
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][2] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                                datagrid1.DataSource = mysql2.Tables[0];
                                //datagrid1.ClearSelection();     //取消dataGridView选中焦点状态
                                //datagrid1.Rows[0].Selected = true;  //选中最新行
                                //datagrid1.FirstDisplayedScrollingRowIndex = mysql.Tables[0].Rows.Count - 2;
                                datagrid1.Columns[0].Width = 200;  //id列的宽度设置
                                datagrid1.Columns[1].Width = 80;  //tid列的宽度设置                            
                                return;
                            }

                        }
                    }

                    //归还
                    if (b1 == true && operate_type == "D1")
                    {
                        //单盒入库
                        if (messagex[0] == "2")
                        {
                            button29.Enabled = false;
                            button30.Enabled = false;
                            textbox29.Text = messagex[2];
                            return;
                        }
                        //批量入库操作
                        if (button31.Enabled == true)
                        {
                            //查询在库
                            chenstr1 = chenking(messagex[0]);
                            if (chenstr1.Count == 0)
                            {
                                killtxt = "ERROR";
                                startkill();
                                MessageBox.Show("该档案不存在，请检查！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);   //档案不存在（或未入库）判断
                                return;
                            }
                            else if (chenstr1[0] == "1")
                            {
                                //非正常借出
                                if (mysqll.Tables[0].Rows[0][2].ToString() == "1")
                                {
                                    killtxt = "ERROR";
                                    startkill();
                                    MessageBox.Show("该档案未借出！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);   //档案不存在（或未入库）判断
                                    return;
                                }
                                //正常借出
                                else
                                {
                                    //重复录入
                                    try
                                    {
                                        for (int i = 0; i < mysql.Tables[0].Rows.Count; i++)
                                        {
                                            if (mysql.Tables[0].Rows[i][1].ToString() == messagex[0])
                                            {
                                                return;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                    }
                                    mysql.Tables[0].Rows.Add();
                                    mysql2.Tables[0].Rows.Add();
                                    mysql4.Tables[0].Rows.Add();
                                    mysql5.Tables[0].Rows.Add();
                                    datagrid1.DataSource = mysql2.Tables[0];
                                    mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = mysqll.Tables[0].Rows[0][0];    //id号 
                                    mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = mysqll.Tables[0].Rows[0][0];    //id号                                                                                                                     
                                    mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = mysqll.Tables[0].Rows[0][1];    //tid号
                                    mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = mysqll.Tables[0].Rows[0][1];    //tid号
                                    mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = mysqll.Tables[0].Rows[0][1];  //TID号 
                                    mysql2.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = mysqll.Tables[0].Rows[0][1].ToString().Replace("00000", "*");  //TID号 
                                    mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = "return";  //出库(借阅)   
                                    mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][2] = mysqll.Tables[0].Rows[0][2];  //status在库代码编号1
                                    mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = mysqll.Tables[0].Rows[0][3];  //姓名                                                                                                                                                 
                                    mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = mysqll.Tables[0].Rows[0][4];  //perid 工号
                                    mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = mysqll.Tables[0].Rows[0][4];  //perid 工号
                                    mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = mysqll.Tables[0].Rows[0][4];  //perid 工号
                                    mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][5] = mysqll.Tables[0].Rows[0][5];  //位置编号,position
                                    mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][5] = mysqll.Tables[0].Rows[0][5];  //位置编号,position
                                    mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = mysqll.Tables[0].Rows[0][5];  //确定柜号+门号
                                    mysql2.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = mysqll.Tables[0].Rows[0][5];  //确定柜号+门号 
                                    openlist.Add(comm3);
                                    closelist.Add(mysqll.Tables[0].Rows[0][5].ToString());
                                    mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());  //时间代码编号
                                    mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                                    mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][2] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                                    mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][7] = textbox29.Text;        //出库操作员编码
                                    mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][7] = textbox29.Text;
                                    mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = textbox29.Text;        //出库操作员编码
                                    //datagrid1.ClearSelection();     //取消dataGridView选中焦点状态
                                    //datagrid1.Rows[0].Selected = true;  //选中最新行
                                    //datagrid1.FirstDisplayedScrollingRowIndex = mysql.Tables[0].Rows.Count - 2;
                                    datagrid1.Columns[0].Width = 200;  //id列的宽度设置
                                    datagrid1.Columns[1].Width = 80;  //tid列的宽度设置 
                                }
                            }
                        }
                    }

                    //借阅
                    if (b2 == true && (operate_type == "O1" || operate_type == "C2"))
                    {
                        //二维码数据解析函数
                        List<string> data = new List<string>();
                        data = danalysis2(buffer1);

                        //单盒出库
                        if (messagex[0] == "1")
                        {
                            textbox32.Text = data[1];
                            var data1 = data[0].Split(new char[2] { ':', ',' });
                            int m = data1.Count() - 2;
                            textbox31.Text = data1[0];
                            if (m == 1)
                            {
                                //查询在库 判断档案状态
                                chenstr1 = chenking(data1[1]);
                                if (chenstr1.Count == 0)
                                { }
                                else if (chenstr1[0] == "0")
                                {
                                    killtxt = "ERROR";
                                    startkill();
                                    MessageBox.Show(data1[1] + "该档案已经借出！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);   //档案已经借出判断
                                    return;
                                }
                                else if (chenstr1[0] == "1")
                                {
                                    killtxt = "ERROR";
                                    startkill();
                                    MessageBox.Show(data1[1] + "该档案不存在！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);   //档案不存在（或未入库）判断
                                    return;
                                }
                                textbox35.Text = data1[1];
                            }
                            if (m == 2)
                            {
                                chenstr1 = chenking(data1[1]);
                                chenstr2 = chenking(data1[2]);
                                if (chenstr1.Count == 0 || chenstr2.Count == 0)
                                { }
                                else if (chenstr1[0] == "0" || chenstr2[0] == "0")
                                {
                                    killtxt = "ERROR";
                                    startkill();
                                    if (chenstr1[0] == "0")
                                    { MessageBox.Show(data1[1] + "该档案已经借出！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); } //档案已经借出判断
                                    if (chenstr2[0] == "0")
                                    { MessageBox.Show(data1[2] + "该档案已经借出！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                                    return;
                                }
                                else if (chenstr1[0] == "1" || chenstr2[0] == "1")
                                {
                                    killtxt = "ERROR";
                                    startkill();
                                    if (chenstr1[0] == "1")
                                    { MessageBox.Show(data1[1] + "该档案不存在！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); } //档案不存在（或未入库）判断
                                    if (chenstr2[0] == "1")
                                    { MessageBox.Show(data1[2] + "该档案不存在！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                                    return;
                                }
                                textbox35.Text = data1[1];
                                textbox34.Text = data1[2];
                            }
                            if (m == 3)
                            {
                                chenstr1 = chenking(data1[1]);
                                chenstr2 = chenking(data1[2]);
                                chenstr3 = chenking(data1[2]);
                                if (chenstr1.Count == 0 || chenstr2.Count == 0 || chenstr3.Count == 0)
                                { }
                                else if (chenstr1[0] == "0" || chenstr2[0] == "0" || chenstr3[0] == "0")
                                {
                                    killtxt = "ERROR";
                                    startkill();
                                    if (chenstr1[0] == "0")
                                    { MessageBox.Show(data1[1] + "该档案已经借出！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); } //档案已经借出判断
                                    if (chenstr2[0] == "0")
                                    { MessageBox.Show(data1[2] + "该档案已经借出！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                                    if (chenstr3[0] == "0")
                                    { MessageBox.Show(data1[3] + "该档案已经借出！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                                    return;
                                }
                                else if (chenstr1[0] == "1" || chenstr2[0] == "1" || chenstr3[0] == "1")
                                {
                                    killtxt = "ERROR";
                                    startkill();
                                    if (chenstr1[0] == "1")
                                    { MessageBox.Show(data1[1] + "该档案不存在！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); } //档案不存在（或未入库）判断
                                    if (chenstr2[0] == "1")
                                    { MessageBox.Show(data1[2] + "该档案不存在！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                                    if (chenstr3[0] == "1")
                                    { MessageBox.Show(data1[3] + "该档案不存在！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                                    return;
                                }
                                textbox34.Text = data1[2];
                                textbox35.Text = data1[1];
                                textbox33.Text = data1[3];
                            }

                            for (int i = 1; i < 1 + m; i++)
                            {
                                chenstr1 = chenking(data1[i]);
                                mysql.Tables[0].Rows.Add();
                                mysql4.Tables[0].Rows.Add();
                                mysql5.Tables[0].Rows.Add();
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = mysqll.Tables[0].Rows[0][0];    //id号 
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = mysqll.Tables[0].Rows[0][0];    //id号                                                                                                                     
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = mysqll.Tables[0].Rows[0][1];    //tid号
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = mysqll.Tables[0].Rows[0][1];    //tid号
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = mysqll.Tables[0].Rows[0][1];  //TID号 
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = "borrow";  //出库    
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][2] = mysqll.Tables[0].Rows[0][2];  //status在库代码编号1
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = mysqll.Tables[0].Rows[0][3];  //姓名                                                                                                                                                 
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = mysqll.Tables[0].Rows[0][4];  //perid 工号
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = mysqll.Tables[0].Rows[0][4];  //perid 工号
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = mysqll.Tables[0].Rows[0][4];  //perid 工号
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][5] = mysqll.Tables[0].Rows[0][5];  //位置编号,position
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][5] = mysqll.Tables[0].Rows[0][5];  //位置编号,position
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = mysqll.Tables[0].Rows[0][5];  //确定柜号+门号
                                textbox36.Text = mysqll.Tables[0].Rows[0][5].ToString();    //显示存放位置
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());  //时间代码编号
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][2] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                                mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][7] = textbox32.Text;        //出库操作员编码
                                mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][7] = textbox32.Text;
                                mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = textbox32.Text;        //出库操作员编码
                            }

                        }

                        //批量出库
                        if (messagex[0] == "2")
                        {
                            textbox32.Text = data[1];
                            var data2 = messagex[2].Split(new char[1] { ';' });
                            for (int i = 0; i < data2.Count(); i++)
                            {
                                var data3 = data2[i].Split(new char[2] { ':', ',' });
                                for (int j = 1; j < data3.Count() - 1; j++)
                                {
                                    //查询在库 判断档案状态
                                    chenstr1 = chenking(data3[j]);
                                    if (chenstr1.Count == 0)
                                    { }
                                    else if (chenstr1[0] == "0")
                                    {
                                        //killtxt = "ERROR";
                                        //startkill();
                                        MessageBox.Show(data3[j] + "该档案已经借出！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);   //档案已经借出判断
                                        //return;
                                        continue;
                                    }
                                    else if (chenstr1[0] == "1")
                                    {
                                        //killtxt = "ERROR";
                                        //startkill();
                                        MessageBox.Show(data3[j] + "该档案不存在！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);   //档案不存在（或未入库）判断
                                        //return;
                                        continue;
                                    }
                                    datagrid2.DataSource = mysql2.Tables[0];
                                    mysql.Tables[0].Rows.Add();
                                    mysql2.Tables[0].Rows.Add();
                                    mysql4.Tables[0].Rows.Add();
                                    mysql5.Tables[0].Rows.Add();
                                    mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = mysqll.Tables[0].Rows[0][0];    //id号 
                                    mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = mysqll.Tables[0].Rows[0][0];    //id号                                                                                                                     
                                    mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = mysqll.Tables[0].Rows[0][1];    //tid号
                                    mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = mysqll.Tables[0].Rows[0][1];    //tid号
                                    mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = mysqll.Tables[0].Rows[0][1];  //TID号 
                                    mysql2.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][0] = mysqll.Tables[0].Rows[0][1].ToString().Replace("00000", "*");  //TID号 
                                    mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = "borrow";  //出库    
                                    mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][2] = mysqll.Tables[0].Rows[0][2];  //status在库代码编号1
                                    mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = mysqll.Tables[0].Rows[0][3];  //姓名                                                                                                                                                 
                                    mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = mysqll.Tables[0].Rows[0][4];  //perid 工号
                                    mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = mysqll.Tables[0].Rows[0][4];  //perid 工号
                                    mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = mysqll.Tables[0].Rows[0][4];  //perid 工号
                                    mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][5] = mysqll.Tables[0].Rows[0][5];  //位置编号,position
                                    mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][5] = mysqll.Tables[0].Rows[0][5];  //位置编号,position
                                    mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = mysqll.Tables[0].Rows[0][5];  //确定柜号+门号
                                    mysql2.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][1] = mysqll.Tables[0].Rows[0][5];  //确定柜号+门号 
                                    openlist.Add(comm3);
                                    closelist.Add(mysqll.Tables[0].Rows[0][5].ToString());
                                    mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());  //时间代码编号
                                    mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                                    mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][2] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                                    mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][7] = textbox32.Text;        //出库操作员编码
                                    mysql4.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][7] = textbox32.Text;
                                    mysql5.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][3] = textbox32.Text;        //出库操作员编码
                                    //datagrid2.DataSource = mysql2.Tables[0];
                                    //datagrid2.ClearSelection();     //取消dataGridView选中焦点状态                                                                  
                                    datagrid2.Columns[0].Width = 180;  //id列的宽度设置
                                    datagrid2.Columns[1].Width = 100;  //tid列的宽度设置 
                                }
                            }
                        }
                        var str4 = mysqll.Tables[0].Rows[0][5].ToString().Split(new char[1] { '-' });
                        num = int.Parse(str4[0]);
                        box = int.Parse(str4[1]);
                        danalysis1(num, box);
                        num1 = num;
                        box1 = box;
                    }
                }
                else
                {
                    MessageBox.Show("二维码已过期，请在得到二维码4小时内操作！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);   //此代码在归还入库时有效
                }
            }

            //下位机32读数据源
            //if (buffer[0] == '#')         //帧头判断，数据开始
            //{

            //    if (buffer[0] == '#')
            //    {

            //    }
            //    else
            //    {
            //        MessageBox.Show("二维码已过期，请在得到二维码4小时内操作！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);   //此代码在归还入库时有效
            //    }
            //}

        }

        //返回扫描内容
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
        #endregion    

        #region  数据解析函数

        //扫二维码时数据解析
        public int num = 0;
        public int box = 0;
        public string operate_type;
        public List<string> danalysis(string buf, string nowtime)
        {
            List<string> messagex1 = new List<string>();
            var timenyr1 = buf.Split(new char[1] { ' ' });
            var timenyr11 = timenyr1[0].Split(new char[1] { '%' });
            if (timenyr11.Count() == 4)
            {
                operate_type = timenyr11[1];
            }
            
            if (timenyr1.Count() == 1)
            {
                messagex1.Add(timenyr11[1].ToString());
            }
            else
            {
                var timenyr12 = timenyr1[1].Split(new char[2] { '%', '\0' });
                messagex1.Add(timenyr11[1].ToString());  //加入TID号-important
                var timl = timenyr1[1].ToString().Split(new char[1] { ':' });
                int timestart = Int32.Parse(timl[0].ToString()) * 100 + Int32.Parse(timl[1]);
                var timenyr2 = nowtime.Split(new char[1] { ' ' });
                timl = timenyr2[1].ToString().Split(new char[1] { ':' });
                int timeend = Int32.Parse(timl[0].ToString()) * 100 + Int32.Parse(timl[1]);
                messagex1.Add(nowtime);

                //判断单盒操作
                if (timenyr11[2] == "1")
                {
                    messagex1[0] = "1";
                }
                //判断批量盒操作
                if (timenyr11[2] == "2")
                {
                    messagex1[0] = "2";
                }
                messagex1.Add(timenyr12[1]);  //操作员编号
            }

            ////////////////////////////////////////////测试用

            //num = count / 30 + 1;
            //if (count % 30 == 0) { num = count / 30; }
            //box = count % 30;
            //if (count % 30 == 0) { box = 30; }

            //下位机通信消息体
            //16进制组包协议

            byte[] comm31 = { 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            comm31[0] = Convert.ToByte(num);   //控制板地址
            //ch1-ch16
            if (box < 17)
            {
                comm31[3] = 0x01;   //柜号,CH1-CH16
                if (box < 9)
                {
                    comm31[4] = 0x00;
                    comm31[5] = (byte)(0x01 << (box - 1));
                }
                else
                {
                    comm31[5] = 0x00;
                    comm31[4] = (byte)(0x01 << (box - 9));
                }
            }
            //ch17-ch30
            else
            {
                comm31[3] = 0x02;   //柜号,CH17-CH30
                if (box < 25)
                {
                    comm31[4] = 0x00;
                    comm31[5] = (byte)(0x01 << (box - 17));
                }
                else
                {
                    comm31[5] = 0x00;
                    comm31[4] = (byte)(0x01 << (box - 25));
                }
            }

            //奇校验算法
            comm31 = crce(comm31);
            //赋值全局变量
            comm3 = comm31;
            tid = messagex1[0];

            return messagex1;
            /////////////////////////////////////////////

            //判断二维码时效性为4小时
            //if (timeend - timestart <= 400 && timenyr11[2].ToString() == timenyr2[0].ToString())
            //{
            //    return messagex1;
            //}
            //else
            //{
            //    return null;
            //}
        }

        //借阅档案信息数据解析
        public List<string> danalysis2(string buffer1)
        {
            List<string> messagex2=new List<string>();
            var timenyr12 = buffer1.Split(new char[1] { '%' });
            messagex2.Add(timenyr12[4]);
            messagex2.Add(timenyr12[5]);
            return messagex2;
        }

        //已有档案存在重组包-danalysis1
        public void danalysis1(int num3, int box3)
        {
            List<string> messagex1 = new List<string>();
            num = num3;
            box = box3;
            
            byte[] comm31 = { 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }; 
            comm31[0] = Convert.ToByte(num);   //控制板地址
            //ch1-ch16
            if (box < 17)
            {
                comm31[3] = 0x01;   //柜号,CH1-CH16
                if (box < 9)
                {
                    comm31[4] = 0x00;
                    comm31[5] = (byte)(0x01 << (box - 1));
                }
                else
                {
                    comm31[5] = 0x00;
                    comm31[4] = (byte)(0x01 << (box - 9));
                }
            }
            //ch17-ch30
            else
            {
                comm31[3] = 0x02;   //柜号,CH17-CH30
                if (box < 25)
                {
                    comm31[4] = 0x00;
                    comm31[5] = (byte)(0x01 << (box - 17));
                }
                else
                {
                    comm31[5] = 0x00;
                    comm31[4] = (byte)(0x01 << (box - 25));
                }
            }
            //奇校验算法
            comm31 = crce(comm31);
            //赋值全局变量
            comm3 = comm31;
        }


        //CRC校验算法
        public byte[] crce(byte[] data)
        {
            int rc1 = 0xffff;
            int i = 0;
            while (6 - i > 0)
            {
                rc1 = rc1 ^ data[i];
                for (int j = 0; j < 8; j++)
                {
                    //if (Convert.ToBoolean(rc1) & Convert.ToBoolean(0x01))
                    if ((rc1 & 0x01) == 0x01)
                    {
                        rc1 = (rc1 >> 1) ^ 0xA001;
                    }
                    else
                    {
                        rc1 = rc1 >> 1;
                    }
                }
                i++;
            }
            //data[7] = Convert.ToByte(Convert.ToBoolean(rc1) & Convert.ToBoolean(0x01));
            data[6] = (byte)(rc1 & 0x00ff);
            data[7] = Convert.ToByte(rc1 >> 8);
            return data;
        }

        //RFID读卡器数据解析
        public string tid;
        public string time;
        public string nums;
        public byte[] comm3;

        public List<string> rfidread(string buf, string nowtime)
        {
            //整体数据消息体，包括：1.TID号；2.实时时间；3.确定柜体编号及柜门；4.与下位机通信消息体，用于开锁；5.消息体“%1%2%3%4%”
            List<string> messagex2 = new List<string>();

            //获取TID号
            tid = buf;             //可能含角色信息，可能会解包
            messagex2.Add(tid);
            //实时时间
            time = nowtime;

            //mysql查询柜体空闲，包括2种情况，已有档案在库、第一次入库，通过档案角色（职务、姓名代码）识别
            //测试阶段尽量考虑简单情况，一人一份档案
            nums = mysqlcheck("databasename");
            nums = "";

            //下位机通信消息体
            //16进制组包协议
            byte[] comm32 = { 0x55, 0xCD, 0x01, 0x01, 0x00 };
            //byte[] str = { 0xAB, 0x12,0x00};
            byte sum = 0;
            //和校验
            foreach (byte arryement in comm32)
            {
                sum += arryement;
            }
            int i = comm32.Length - 1;
            comm32[i] = sum;

            return messagex2;

        }
        #endregion

        #region  关门检测，数据存储、报警、出二维码(另开线程)
        //开启线程
        public void closecheck()
        {
            Thread comlisten = new Thread(new ThreadStart(chenck));
            comlisten.IsBackground = true;  //线程转后台
            comlisten.Start();
        }

        private void chenck()
        {
            try
            {
                ThreadStart threadstart1 = new ThreadStart(check1);
                Thread thread1 = new Thread(threadstart1);
                threadstart1();
            }
            catch
            {

            }
        }

        //启动定时器2min

        private void check1()
        {
            System.Timers.Timer t3 = new System.Timers.Timer(400);  //每500ms检测一次，共检测240次（即2min）
            t3.Elapsed += new System.Timers.ElapsedEventHandler(theout2);
            t3.AutoReset = false;  //true一直执行,false执行一次
            t3.Enabled = true;   //是否执行Elapsed事件
        }

        //读锁扣状态
        public int road = 0;
        public int num1;
        public int box1;
        public bool borrow_save = false;
        public void theout2(object source, System.Timers.ElapsedEventArgs e)
        {

            //int count3 = 300, count4=400;
            //int box2 = 0;
            //serialport1 serial = new serialport1();
            //List<byte> databack_1=new List<byte>();
            //List<int> doorstatus_2;

            if (savetype != 1)
            {
                int count3 = 300;
                int box2 = 0;
                serialport1 serial = new serialport1();
                List<byte> databack_1 = new List<byte>();
                List<int> doorstatus_2;
                byte[] close_comm32 = { 0x00, 0x04, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00 };  //读门的状态
                close_comm32[0] = Convert.ToByte(num);   //控制板地址
                if (b1 == true && button31.Enabled == false)
                {
                    //门地址
                    box2 = box;
                }
                if (b1 == true && button31.Enabled == true)
                {
                    //门地址
                    box2 = box;
                    num1 = num;
                    box1 = box;
                }
                if (b2 == true)
                {
                    //门地址
                    box1 = box;
                    num1 = num;
                    box2 = box;
                }
                if (b3 == true)
                {
                    //门地址
                    box2 = box;
                }

                //判断通道1：1-16,通道2:17-32
                int m = box - 16;
                if (m > 0)
                {
                    close_comm32[3] = 0x04;   //通道2
                }
                else
                {
                    close_comm32[3] = 0x03;   //通道1
                }

                //crc校验
                close_comm32 = crce(close_comm32);

                //开关门状态查询
                //t.Stop();
                //check_doorstatus();
                //Thread.Sleep(30);
                //t.Start();

                //不断查询柜门状态
                while ((count3--) > 0)
                {
                    //t.Stop();
                    check_doorstatus_1(num);
                    Thread.Sleep(100);
                    databack_1 = serial.senddata(close_comm32, portlist1, port32);
                    //t.Start();
                    try
                    {
                        //关门检测
                        doorstatus_2 = doorstatus(m, databack_1[3], databack_1[4], box2);
                        if (doorstatus_2[0] == doorstatus_2[1])
                        {
                            //再次确认
                            databack_1 = serial.senddata(close_comm32, portlist1, port32);
                            //关门检测
                            doorstatus_2 = doorstatus(m, databack_1[3], databack_1[4], box2);
                            if (doorstatus_2[0] != doorstatus_2[1])
                            { continue; }
                            killtxt = "opendoor1";
                            startkill();
                            MessageBox.Show(num.ToString() + "-" + box.ToString() + "操作完成!", "opendoor1", MessageBoxButtons.OK, MessageBoxIcon.None);
                            Thread.Sleep(50);
                            //关门成功显示
                            closedoor_dis(true);
                            //关门成功数据存储变量
                            savedate();
                            //关门成功二维码小票打印,分单入和批入
                            produceqr();

                            //清理界面
                            instoreinit1();
                            ////自加
                            //box++;
                            //if (box == 31)
                            //{
                            //    box = 1;
                            //    num++;
                            //}
                            break;
                            //return;
                        }
                        if (count3 == 0)
                        {
                            //未关门成功，报警
                            killtxt = "opendoor1";
                            startkill();
                            MessageBox.Show("请注意，" + num1.ToString() + "-" + box1.ToString() + " 未关门！", "opendoor1", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            closeflag = 2;
                            Thread.Sleep(50);
                            //关门异常显示
                            closedoor_dis(false);
                            //关门操作错误处理
                            warr();
                            if (b1 == true)
                            {
                                //mysql2.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = "入库失败";  //入库成功编码
                            }
                            if (b2 == true)
                            {
                                //mysql2.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = "借阅失败";  //入库成功编码
                            }
                            if (b3 == true)
                            {
                                //mysql2.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = "归还失败";  //入库成功编码
                            }
                            return;
                            //报警，并提示，若消警则必须关门
                        }
                    }
                    catch
                    { }
                }
            }
            else
            {
                savetype = 0;
                savedate();
                //关门成功二维码小票打印,分单入和批入
                produceqr();
                //清理界面
                instoreinit1();
            }
        }

        #endregion

        #region  开门控制、检测
        public serialport1 serial = new serialport1();
        public void contorldoor()
        {
            List<int> doorstatus1=new List<int>();
            //step1：开门，提示开门成功或门闪烁
            //killtxt = "opendoor";
            //startkill();
            //MessageBox.Show("正在开锁，请稍后。。。", "opendoor", MessageBoxButtons.OK, MessageBoxIcon.None);   //此代码在归还入库时有效
            List<byte> openback = serial.senddata1(comm3, portlist1, port32);
            //如返回 openback={0x55,0x01,0x01,0x00,0x66} 就是1号板（1号柜体），如果选择通道1就是：8号门关闭；如果选择通道2就是：25号门
            //openback = serial.senddata1(comm3, portlist1, port32);  //再开一次保证开锁成功
            byte[] openback1 = { 0x55, 0x01, 0x01, 0x00, 0x66 };
            //int.Parse(openback[1]);
            //2个通道返回的数据检验
            int box2;
            box2 = box;

            //开门后检测
            int m = box - 16;

            try
            {
                doorstatus1 = doorstatus(m, openback[3], openback[4], box2);
            }
            catch
            {
                doorstatus1.Add(0);
                doorstatus1.Add(0);
            }
            //
            if (doorstatus1[0] != doorstatus1[1])
            {
                //开门成功效果显示
                //killtxt = "opendoor1";
                //startkill();
                //MessageBox.Show("已正常开锁！", "opendoor1", MessageBoxButtons.OK, MessageBoxIcon.None);
                closeflag = 0;
            }
            else
            {
                int c = 10;
                for (; c > 0; c--)
                {
                    try
                    {
                        Thread.Sleep(100);
                        if (c > 5)
                        {
                            openback = serial.senddata1(comm3, portlist1, port32);
                            doorstatus1 = doorstatus(m, openback[3], openback[4], box2);
                            if (doorstatus1[0] != doorstatus1[1])
                            {
                                //开门成功效果显示
                                //killtxt = "opendoor1";
                                //startkill();
                                //MessageBox.Show("已正常开锁！", "opendoor1", MessageBoxButtons.OK, MessageBoxIcon.None);
                                closeflag = 0;
                                break;
                            }
                        }
                        else if (c>1&&c<=5)
                        {
                            //2919.4.2添加
                            check_doorstatus_1(num);
                            if (doorlistopen.IndexOf(closestr) >= 0)
                            {
                                //killtxt = "opendoor1";
                                //startkill();
                                //MessageBox.Show("已正常开锁！", "opendoor1", MessageBoxButtons.OK, MessageBoxIcon.None);
                                closeflag = 0;
                                break;
                            }
                        }
                        else
                        {
                            //killtxt = "opendoor1";
                            //startkill();
                            //MessageBox.Show("已正常开锁！", "opendoor1", MessageBoxButtons.OK, MessageBoxIcon.None);
                            closeflag = 0;
                            break;
                        }
                        Thread.Sleep(20);
                    }
                    catch
                    {
                        continue;
                    }
                }                 
            }

        }
        #endregion

        #region  门状态回传数据解析
        public List<int> doorstatus(int m, byte openback2, byte openback3, int box2)
        {
            int openback11 = 0;
            int openback22 = 0;
            List<int> doorstatus = new List<int>();
            //判断通道1：1-16,通道2:17-32
            //ch1-ch16
            if (m <= 0)
            {
                if (box2 <= 8)
                {
                    openback11 = int.Parse((Convert.ToInt32((0x00 << 8) + 0x01 << (box2 - 1))).ToString());
                    openback22 = ((((openback3 >> (box2 - 1))) << 15) & 0xffff) >> (16 - box2);
                }
                else
                {
                    openback11 = int.Parse((Convert.ToInt32((0x01 << (box2 - 9)) << 8)).ToString());
                    openback22 = (((((openback2 << 8) + 0x00) >> (box2 - 1)) << 15) & 0xffff) >> (16 - box2);
                }

            }
            //ch17-ch30
            if (m > 0)
            {
                if (box2 <= 24)
                {
                    openback11 = int.Parse((Convert.ToInt32((0x00 << 8) + 0x01 << (box2 - 17))).ToString());
                    openback22 = ((((openback3 >> (box2 - 17))) << 15) & 0xffff) >> (32 - box2);
                }
                else
                {
                    openback11 = int.Parse((Convert.ToInt32((0x01 << (box2 - 25)) << 8)).ToString());
                    openback22 = (((((openback2 << 8) + 0x00) >> (box2 - 17)) << 15) & 0xffff) >> (32 - box2);
                }
            }
            doorstatus.Add(openback11);
            doorstatus.Add(openback22);
            return doorstatus;
        }

        public List<int> check_door(byte openback3, byte openback4,int m)
        {
            //只检测已开门的编号
            List<int> doorstatus = new List<int>();
            if (m==1)
            {
                for (int i = 0; i < 8; i++)
                {
                    if ((openback4 & (0x0001 << i)) == 0)
                    {
                        doorstatus.Add(i + 1);
                    }
                }
                for (int i = 0; i < 8; i++)
                {
                    if ((openback3 & (0x0001 << i)) == 0)
                    {
                        doorstatus.Add(i + 9);
                    }
                }
            }
            if (m==2)
            {
                for (int i = 0; i < 8; i++)
                {
                    if ((openback4 & (0x0001 << i)) == 0)
                    {
                        doorstatus.Add(i + 17);
                    }
                }
                for (int i = 0; i < 6; i++)
                {
                    if ((openback3 & (0x0001 << i)) == 0)
                    {
                        doorstatus.Add(i + 25);
                    }
                }
            }
            return doorstatus;
        }
        #endregion

        #region  入库数据保存
        public void savedate()
        {

            //MySqlCommandBuilder build = new MySqlCommandBuilder(mysqlconn0.mysqldata);
            //mysqlconn0.mysqldata.Update(mysql.Tables[0]);
            string mysqlselect = "insert into tablename" + " set ";
            string mysqlcom1 = "use " + "store";
            string tablename = "tablename";

            try
            {
                if (b1 == true)
                {
                    //批入
                    if (button31.Enabled == false && operate_type != "D1" && (dflag == 2|| dflag == 1))
                    {
                        mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                        //mysql2.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2][4] = "入库成功";  //入库成功编码
                        if (mysqlconn0.mysqlupdatecomarow1(mysqlselect, mysql, openlist.Count - 1))
                        {
                            closeflag = 1;
                            Thread.Sleep(100);
                            //mysql4.Tables[0].Rows[mysql4.Tables[0].Rows.Count - 2 - openlist.Count][2] = 1;  //入库状态，1：在库，0：出库
                            mysql5.Tables[0].Rows[mysql4.Tables[0].Rows.Count - 2 - openlist.Count][5] = "入库成功";  //入库状态，1：在库，0：出库
                            mysqlcom1 = "use " + "note19";
                            mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                            tablename = "note1901";
                            mysqlselect = "insert into " + tablename + " set ";
                            if (mysqlconn0.mysqlupdatecomarow1(mysqlselect, mysql4, openlist.Count))
                            {

                            }
                            else
                            {
                                //创建表   
                                mysqlselect = "create table " + tablename + "(";
                                if (mysqlconn0.mysqlsavecom1(mysqlselect, mysql4, tablename, openlist.Count))
                                {

                                }
                                else
                                {
                                    MessageBox.Show("操作记录保存失败，请重试！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        else
                        {
                            //创建表
                            mysqlcom1 = "use " + "store";
                            mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库

                            tablename = "tablename";
                            mysqlselect = "create table " + tablename + "(";

                            //创建表并存储表
                            if (mysqlconn0.mysqlsavecom1(mysqlselect, mysql, tablename, openlist.Count - 1))
                            {
                                closeflag = 1;
                                Thread.Sleep(100);
                                //mysql4.Tables[0].Rows[mysql4.Tables[0].Rows.Count - 2 - openlist.Count][2] = 1;  //入库状态，1：在库，0：出库
                                mysql5.Tables[0].Rows[mysql4.Tables[0].Rows.Count - 2 - openlist.Count][5] = "入库成功";  //入库状态，1：在库，0：出库
                                mysqlcom1 = "use " + "note19";
                                mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                                tablename = "note1901";
                                mysqlselect = "insert into " + tablename + " set ";
                                if (mysqlconn0.mysqlupdatecomarow1(mysqlselect, mysql4, openlist.Count))
                                {

                                }
                                else
                                {
                                    //创建表   
                                    mysqlselect = "create table " + tablename + "(";
                                    if (mysqlconn0.mysqlsavecom1(mysqlselect, mysql4, tablename, openlist.Count))
                                    {

                                    }
                                    else
                                    {
                                        MessageBox.Show("操作记录保存失败，请重试！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("保存失败，请重试！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    //单入
                    if (button31.Enabled == false && operate_type != "D1" && dflag == 4)
                    {
                        mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                        if (mysqlconn0.mysqlupdatecomarow(mysqlselect, mysql))
                        {
                            for (int i = 0; i < mysql.Tables[0].Rows.Count - 1; i++)
                            {
                                mysql4.Tables[0].Rows[i][2] = 1;  //入库状态，1：成功，0：失败
                                mysql5.Tables[0].Rows[i][5] = "入库成功";  //入库状态成功
                            }

                            mysqlcom1 = "use " + "note19";
                            mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                            tablename = "note1901";
                            mysqlselect = "insert into " + tablename + " set ";
                            if (mysqlconn0.mysqlupdatecomarow(mysqlselect, mysql4))
                            {

                            }
                            else
                            {
                                //创建表  
                                mysqlselect = "create table " + tablename + "(";
                                if (mysqlconn0.mysqlsavecom(mysqlselect, mysql4, tablename))
                                {

                                }
                                else
                                {
                                    MessageBox.Show("操作记录保存失败，请重试！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        else
                        {
                            //创建表
                            mysqlcom1 = "use " + "store";
                            mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                            tablename = "tablename";
                            mysqlselect = "create table " + tablename + "(";

                            //创建表并存储表
                            if (mysqlconn0.mysqlsavecom(mysqlselect, mysql, tablename))
                            {
                                label24text = "入库成功";
                                for (int i = 0; i < mysql.Tables[0].Rows.Count - 1; i++)
                                {
                                    mysql4.Tables[0].Rows[i][2] = 1;  //入库状态，1：成功，0：失败
                                    mysql5.Tables[0].Rows[i][4] = "入库成功";  //入库状态成功
                                }
                                mysqlcom1 = "use " + "note19";
                                mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                                tablename = "note1901";
                                mysqlselect = "insert into " + tablename + " set ";
                                if (mysqlconn0.mysqlupdatecomarow(mysqlselect, mysql4))
                                {

                                }
                                else
                                {
                                    //创建表     
                                    mysqlselect = "create table " + tablename + "(";
                                    if (mysqlconn0.mysqlsavecom(mysqlselect, mysql4, tablename))
                                    {

                                    }
                                    else
                                    {
                                        MessageBox.Show("操作记录保存失败，请重试！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("保存失败，请重试！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }

                //归还操作
                if (b1 == true && operate_type == "D1"&&(dflag==1||dflag==2))
                {
                    mysqlcom1 = "use " + "store";
                    mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库

                    //mysql.Tables[0].Rows[mysql.Tables[0].Rows.Count - 2 - openlist.Count + 1][2] = 1; //status入库成功代码编号1 
                    //mysql4.Tables[0].Rows[mysql4.Tables[0].Rows.Count - 2 - openlist.Count + 1][2] = 1; //status入库成功代码编号1 
                    //mysql5.Tables[0].Rows[mysql4.Tables[0].Rows.Count - 2 - openlist.Count + 1][5] = "归还成功";  //入库状态，1：在库，0：出库
                    mysql.Tables[0].Rows[position1][2] = 1; //status入库成功代码编号1 
                    //mysql4.Tables[0].Rows[position2][2] = 1; //status入库成功代码编号1 
                    mysql5.Tables[0].Rows[position2][5] = "归还成功";  //入库状态，1：在库，0：出库

                    mysqlselect = "update tablename set status=" + "\"1\"" + " where tid=" + "\"" + mysql.Tables[0].Rows[position1][1] + "\"";
                    mysqlconn0.mysqlcom(mysqlselect);  //执行mysql命令

                    tablename = "intable";
                    mysqlselect = "insert into " + tablename + " set ";
                    mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                    if (mysqlconn0.mysqlupdatecomarow1(mysqlselect, mysql, mysql.Tables[0].Rows.Count - 2 - position1))
                    {
                        closeflag = 1;
                        Thread.Sleep(100);
                        mysqlcom1 = "use " + "note19";
                        mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                        tablename = "note1901";
                        mysqlselect = "insert into " + tablename + " set ";
                        if (mysqlconn0.mysqlupdatecomarow1(mysqlselect, mysql4, mysql4.Tables[0].Rows.Count - 2 - position2))
                        {
                            mysql4.Tables[0].Rows.RemoveAt(0);
                        }
                        else
                        {
                            //创建表  
                            mysqlselect = "create table " + tablename + "(";
                            if (mysqlconn0.mysqlsavecom1(mysqlselect, mysql4, tablename, mysql4.Tables[0].Rows.Count - 2 - position2))
                            {
                                mysql4.Tables[0].Rows.RemoveAt(0);
                            }
                            else
                            {
                                MessageBox.Show("操作记录保存失败，请重试！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        //创建表
                        mysqlcom1 = "use " + "store";
                        mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                        mysqlselect = "create table " + tablename + "(";
                        //创建表并存储表
                        if (mysqlconn0.mysqlsavecom1(mysqlselect, mysql, tablename, mysql.Tables[0].Rows.Count - 2 - position1))
                        {
                            closeflag = 1;
                            Thread.Sleep(100);
                            mysqlcom1 = "use " + "note19";
                            mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                            tablename = "note1901";
                            mysqlselect = "insert into " + tablename + " set ";
                            if (mysqlconn0.mysqlupdatecomarow1(mysqlselect, mysql4, mysql4.Tables[0].Rows.Count - 2 - position2))
                            {
                                mysql4.Tables[0].Rows.RemoveAt(0);
                            }
                            else
                            {
                                //创建表  
                                mysqlselect = "create table " + tablename + "(";
                                if (mysqlconn0.mysqlsavecom1(mysqlselect, mysql4, tablename, mysql4.Tables[0].Rows.Count - 2 - position2))
                                {
                                    mysql4.Tables[0].Rows.RemoveAt(0);
                                }
                                else
                                {
                                    MessageBox.Show("操作记录保存失败，请重试！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("保存失败，请重试！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    return;
                }

                //if (b2 == true&& borrow_save==true)
                if (b2 == true)
                {
                    //单出
                    if (operate_type == "C0")
                    {
                        mysqlcom1 = "use " + "store"; ;
                        mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                        for (int i = 0; i < mysql.Tables[0].Rows.Count - 1; i++)
                        {
                            mysql.Tables[0].Rows[i][2] = 0;  //status出库代码编号0                            
                            mysqlselect = "update tablename set status=" + "\"0\"" + " where tid=" + "\"" + mysql.Tables[0].Rows[i][1] + "\"";
                            mysqlconn0.mysqlcom(mysqlselect);  //执行mysql命令
                        }
                        tablename = "outtable";
                        mysqlselect = "insert into " + tablename + " set ";
                        mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                        if (mysqlconn0.mysqlupdatecomarow(mysqlselect, mysql))
                        {
                            label52text = "出库成功";
                            for (int i = 0; i < mysql.Tables[0].Rows.Count - 1; i++)
                            {
                                mysql4.Tables[0].Rows[i][2] = 1;  //出库状态，1：成功，0：失败
                                mysql5.Tables[0].Rows[i][5] = "出库成功";  //出库状态成功
                            }

                            mysqlcom1 = "use " + "note19";
                            mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                            tablename = "note1901";
                            mysqlselect = "insert into " + tablename + " set ";
                            if (mysqlconn0.mysqlupdatecomarow(mysqlselect, mysql4))
                            {

                            }
                            else
                            {
                                //创建表  
                                mysqlselect = "create table " + tablename + "(";
                                if (mysqlconn0.mysqlsavecom(mysqlselect, mysql4, tablename))
                                {

                                }
                                else
                                {
                                    MessageBox.Show("操作记录保存失败，请重试！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        else
                        {
                            //创建表
                            mysqlcom1 = "use " + "store";
                            mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                            mysqlselect = "create table " + tablename + "(";
                            //创建表并存储表
                            if (mysqlconn0.mysqlsavecom(mysqlselect, mysql, tablename))
                            {
                                label52text = "出库成功";
                                for (int i = 0; i < mysql.Tables[0].Rows.Count - 1; i++)
                                {
                                    mysql4.Tables[0].Rows[i][2] = 1;  //出库状态，1：成功，0：失败
                                    mysql5.Tables[0].Rows[i][5] = "出库成功";  //出库状态成功
                                }
                                mysqlcom1 = "use " + "note19";
                                mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                                tablename = "note1901";
                                mysqlselect = "insert into " + tablename + " set ";
                                if (mysqlconn0.mysqlupdatecomarow(mysqlselect, mysql4))
                                {

                                }
                                else
                                {
                                    //创建表  
                                    mysqlselect = "create table " + tablename + "(";
                                    if (mysqlconn0.mysqlsavecom(mysqlselect, mysql4, tablename))
                                    {

                                    }
                                    else
                                    {
                                        MessageBox.Show("操作记录保存失败，请重试！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("保存失败，请重试！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        return;
                    }

                    //最新单量、批量出
                    else
                    {
                        if (dflag == 2||dflag==1)
                        {
                            closeflag = 1;
                            label52text = "出库成功";
                        }
                        else
                        {
                            mysqlcom1 = "use " + "store";
                            mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库

                            mysql.Tables[0].Rows[0][2] = 0; //status出库成功代码编号0 
                            //mysql4.Tables[0].Rows[0][2] = 1; //status出库成功代码编号1 
                            mysql5.Tables[0].Rows[0][5] = "出库成功";  //入库状态，1：在库，0：出库

                            mysqlselect = "update tablename set status=" + "\"0\"" + " where tid=" + "\"" + mysql.Tables[0].Rows[0][1] + "\"";
                            mysqlconn0.mysqlcom(mysqlselect);  //执行mysql命令

                            tablename = "outtable";
                            mysqlselect = "insert into " + tablename + " set ";
                            mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                            if (mysqlconn0.mysqlupdatecomarow1(mysqlselect, mysql, openlist.Count-2))
                            {
                                closeflag = 1;
                                openlist.RemoveAt(0);
                                Thread.Sleep(100);
                                label52text = "出库成功";
                                mysqlcom1 = "use " + "note19";
                                mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                                tablename = "note1901";
                                mysqlselect = "insert into " + tablename + " set ";
                                if (mysqlconn0.mysqlupdatecomarow1(mysqlselect, mysql4, openlist.Count-1))
                                {

                                }
                                else
                                {
                                    //创建表  
                                    mysqlselect = "create table " + tablename + "(";
                                    if (mysqlconn0.mysqlsavecom1(mysqlselect, mysql4, tablename, openlist.Count-1))
                                    {

                                    }
                                    else
                                    {
                                        MessageBox.Show("操作记录保存失败，请重试！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            else
                            {
                                //创建表
                                mysqlcom1 = "use " + "store";
                                mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                                mysqlselect = "create table " + tablename + "(";
                                //创建表并存储表
                                if (mysqlconn0.mysqlsavecom1(mysqlselect, mysql, tablename, openlist.Count-2))
                                {
                                    closeflag = 1;
                                    openlist.RemoveAt(0);
                                    Thread.Sleep(100);
                                    label52text = "出库成功";
                                    mysqlcom1 = "use " + "note19";
                                    mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                                    tablename = "note1901";
                                    mysqlselect = "insert into " + tablename + " set ";
                                    if (mysqlconn0.mysqlupdatecomarow1(mysqlselect, mysql4, openlist.Count-1))
                                    {

                                    }
                                    else
                                    {
                                        //创建表  
                                        mysqlselect = "create table " + tablename + "(";
                                        if (mysqlconn0.mysqlsavecom1(mysqlselect, mysql4, tablename, openlist.Count-1))
                                        {

                                        }
                                        else
                                        {
                                            MessageBox.Show("操作记录保存失败，请重试！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("保存失败，请重试！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            
                            return;
                        }
                    }
                }
                if (goout == true || comin == true)
                {
                    mysqlselect = "insert into gatenote1" + " set ";
                    mysqlcom1 = "use " + "gatenote";
                    tablename = "gatenote1";
                    mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                    if (cardlist.Count == 0)
                    {
                        return;
                    }
                    mysql_gate.Tables[0].Clear();
                    for (int i = 0; i < cardlist.Count; i++)
                    {
                        mysql_gate.Tables[0].Rows.Add();
                        mysql_gate.Tables[0].Rows[i][0] = cardlist[i];
                        mysql_gate.Tables[0].Rows[i][3] = DateTime.Now.ToString();
                        if (comin == true)
                        {
                            mysql_gate.Tables[0].Rows[i][2] = "comin";
                            if (warning == true && checkout.Contains(cardlist[i]))
                            {
                                mysql_gate.Tables[0].Rows[i][1] = "1";
                            }
                            else
                            {
                                mysql_gate.Tables[0].Rows[i][1] = "0";
                            }
                        }
                        else if (goout == true)
                        {
                            mysql_gate.Tables[0].Rows[i][2] = "goout";
                            if (warning == true && checkout.Contains(cardlist[i]))
                            {
                                mysql_gate.Tables[0].Rows[i][1] = "1";
                            }
                            else
                            {
                                mysql_gate.Tables[0].Rows[i][1] = "0";
                            }
                        }
                    }
                    if (mysqlconn0.mysqlupdatecomarow2(mysqlselect, mysql_gate))
                    {
                        return;
                    }
                    else
                    {
                        //创建表
                        mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                        tablename = "gatenote1";
                        mysqlselect = "create table " + tablename + "(";

                        //创建表并存储表
                        if (mysqlconn0.mysqlsavecom2(mysqlselect, mysql_gate, tablename))
                        {
                            return;
                        }
                        else
                        {
                            MessageBox.Show("保存失败，请重试！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("保存失败，请重试！", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Error);
                mysqlconn0.mysqlconn1.Dispose();
            }
            //dataGridView1.DataSource = mysql.Tables[0];
        }
        #endregion

        #region   转递数据存储
        public void savedate_transfer(DataSet mysql_trans,int count_num)
        {
            string tablename = "transfernote";
            string mysqlselect = "insert into transfernote" + " set ";
            string mysqlcom1 = "use " + "note19";
            try
            {
                mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                                                                 
                if (mysqlconn0.mysqlupdatecomarow1(mysqlselect, mysql_trans, count_num))
                {
                    mysqlselect = "alter table " + tablename + " drop ID";
                    mysqlconn0.mysqlselectcom(mysqlselect);   //执行sql命令
                    mysqlselect = "alter table " + tablename + " add id int(3) not null first";
                    mysqlconn0.mysqlselectcom(mysqlselect);   //执行sql命令
                    mysqlselect = "alter table " + tablename + " modify column ID int(3) not null auto_increment, add primary key(ID)";
                    mysqlconn0.mysqlselectcom(mysqlselect);   //执行sql命令
                    mysqlselect = "select* from " + tablename + " order by ID";
                    mysqlconn0.mysqlselectcom(mysqlselect);   //执行sql命令
                }
            }
            catch
            {
                MessageBox.Show("保存失败，请重试！", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Error);
                mysqlconn0.mysqlconn1.Dispose();
            }
        }
        #endregion

        #region   整理数据存储
        public void savedate_dill(DataSet mysql_dill, int count_num,int type)
        {
            string tablename = "dilltable";
            string mysqlselect = "insert into dilltable" + " set ";
            string mysqlcom1 = "use " + "note19";
            try
            {
                mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                if (type == 0)
                {
                    for (; count_num > 0; count_num--)
                    {
                        if (mysqlconn0.mysqlupdatecomarow1(mysqlselect, mysql_dill, count_num-2))
                        {
                            
                        }
                    }
                    MessageBox.Show("整理出库完成！", "success", MessageBoxButtons.OK, MessageBoxIcon.None);
                }
                else
                {
                    for (i = 0; i < mysql_dill.Tables[0].Rows.Count; i++)
                    {
                        //删除数据
                        mysqlselect = "delete from " + tablename + " where tid=" + "\"" + mysql_dill.Tables[0].Rows[i][1].ToString() + "\"";
                        mysqlconn0.mysqlselectcom(mysqlselect);   //执行sql命令                       
                    }
                    MessageBox.Show("整理入库完成！", "success", MessageBoxButtons.OK, MessageBoxIcon.None);
                }
            }
            catch
            {
                MessageBox.Show("保存失败，请重试！", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Error);
                mysqlconn0.mysqlconn1.Dispose();
            }
        }
        #endregion

        #region  关门状态显示
        public void closedoor_dis(bool flag1)
        {
            if (flag1 == true)
            {
                draw(4, System.Drawing.Color.Lime);
                //if (mysql6.Tables[0].Rows.Count > 0)
                //{
                //    mysql6.Tables[0].Rows.RemoveAt(0);
                //    dataGridView3.DataSource = mysql6.Tables[0];
                //}
                if (mysql7.Tables[0].Rows.Count > 0)
                {
                    mysql7.Tables[0].Rows.RemoveAt(0);
                    dataGridView5.DataSource = mysql7.Tables[0];
                }
            }
            if (flag1 == false)
            {
                draw(4, System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128))))));
                mysql7.Tables[0].Rows.Add();
                mysql7.Tables[0].Rows[0][0] = num.ToString() + "-" + box.ToString();
                if (operate_type == "I1")
                {
                    mysql7.Tables[0].Rows[0][1] = "入库";
                }
                if (operate_type == "O1")  //单借
                {
                    mysql7.Tables[0].Rows[0][1] = "借阅";
                }
                //if (operate_type == "C2")  //批借
                //{
                //    mysql7.Tables[0].Rows[0][1] = "借阅";
                //}
                if (operate_type == "D1")
                {
                    mysql7.Tables[0].Rows[0][1] = "归还";
                }
                dataGridView5.DataSource = mysql7.Tables[0];
            }
        }
        #endregion

        #region  Mysql应用导入库模板

        private void pre_load_exdata(string str1, string str2)
        {
            try
            {
                string mysqlcom1 = "use " + str1;
                string mysqlselect = "select*from " + str2;               
                List<string> list1 = new List<string>();
                //查询表
                mysqlconn0.mysqlcom(mysqlcom1); //切换数据库
                //选中模板表1,用于存储
                mysql6 = mysqlconn0.mysqlselectcom(mysqlselect);
                mysql7 = mysqlconn0.mysqlselectcom(mysqlselect);
                mysql_openview= mysqlconn0.mysqlselectcom(mysqlselect);
                //引入实例数据集
                string mysqlselect2 = "select*from " + "gatenote";
                mysql_gate = mysqlconn0.mysqlselectcom(mysqlselect2);

                if (mysql6.Tables.Count != 0)
                {
                    //mysql6 开门记录
                    mysql6.Tables[0].Columns[0].ColumnName = "位置编号";
                    mysql6.Tables[0].Columns[1].ColumnName = "操作";
                    dataGridView3.DataSource = mysql6.Tables[0];
                    dataGridView3.ClearSelection();
                    //mysql_openview 开门记录
                    mysql_openview.Tables[0].Columns[0].ColumnName = "位置编号"; 
                    mysql_openview.Tables[0].Columns[1].ColumnName = "操作";
                    dataGridView3.DataSource = mysql_openview.Tables[0];
                    dataGridView3.ClearSelection();
                    //mysql7 异常关门记录
                    mysql7.Tables[0].Columns[0].ColumnName = "位置编号";
                    mysql7.Tables[0].Columns[1].ColumnName = "操作";
                    dataGridView5.DataSource = mysql7.Tables[0];
                    dataGridView5.ClearSelection();
                }
                else
                {
                    MessageBox.Show("查询入库模板数据失败！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        //查询单位编号
        public DataSet pre_load_unitname(string str1, string str2)
        {
            DataSet mysql_unitname=new DataSet();
            try
            {
                string mysqlcom1 = "use " + str1;
                string mysqlselect = "select*from " + str2;
                List<string> list1 = new List<string>();
                //查询表
                mysqlconn0.mysqlcom(mysqlcom1); //切换数据库
                //选中模板表1,用于存储
                mysql_unitname = mysqlconn0.mysqlselectcom(mysqlselect);
                return mysql_unitname;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return mysql_unitname;
        }

        private void hisd_load_exdata(string str1, string str2)
        {
            try
            {
                string mysqlcom1 = "use " + str1;
                string mysqlselect = "select*from " + str2;
                List<string> list1 = new List<string>();
                //查询表
                mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                mysql8 = mysqlconn0.mysqlselectcom(mysqlselect);

                if (mysql8.Tables.Count != 0)
                {
                    mysql8.Tables[0].Columns[0].ColumnName = "ID";
                    mysql8.Tables[0].Columns[1].ColumnName = "RFID号";
                    mysql8.Tables[0].Columns[2].ColumnName = "在库状态";
                    mysql8.Tables[0].Columns[3].ColumnName = "类别";
                    mysql8.Tables[0].Columns[4].ColumnName = "身份证号";
                    mysql8.Tables[0].Columns[5].ColumnName = "存放位置";
                    mysql8.Tables[0].Columns[6].ColumnName = "操作时间";
                    mysql8.Tables[0].Columns[7].ColumnName = "单位名称";
                    mysql8.Tables[0].Columns[8].ColumnName = "操作员编号";
                }
                else
                {
                    MessageBox.Show("查询入库模板数据失败！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        public void load_exdata(string str1, string str2, string str3, string str4)
        {
            string mysqlcom1 = "use " + str1;
            string mysqlshowltable = "show tables";
            string mysqlselect = "select*from " + str2;
            string mysqlselect2 = "select*from " + str3;
            string mysqlselect3 = "select*from " + str4;
            List<string> list1 = new List<string>();
            //查询表
            mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
            list1 = mysqlconn0.mysqlshow(mysqlshowltable);   //数据表列表查询
            //选中模板表1,用于存储
            mysql = mysqlconn0.mysqlselectcom(mysqlselect);
            mysql4 = mysqlconn0.mysqlselectcom(mysqlselect);
            mysql5 = mysqlconn0.mysqlselectcom(mysqlselect3);

            mysql2 = mysqlconn0.mysqlselectcom(mysqlselect2);

            //新增、归还入库
            if (b1 == true)
            {
                if (mysql.Tables.Count != 0)
                {
                    //mysql2
                    mysql2.Tables[0].Columns[0].ColumnName = "RFID号";
                    mysql2.Tables[0].Columns[1].ColumnName = "存放位置";
                    dataGridView1.DataSource = mysql2.Tables[0];
                    //dataGridView1.ClearSelection();
                    //mysql5
                    mysql5.Tables[0].Columns[0].ColumnName = "RFID号";
                    mysql5.Tables[0].Columns[1].ColumnName = "工号";
                    mysql5.Tables[0].Columns[2].ColumnName = "入库时间";
                    mysql5.Tables[0].Columns[3].ColumnName = "操作员编号";
                    mysql5.Tables[0].Columns[4].ColumnName = "存放位置";
                    mysql5.Tables[0].Columns[5].ColumnName = "入库状态";
                }
                else
                {
                    MessageBox.Show("查询入库模板数据失败！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            //借阅出库
            if (b2 == true)
            {
                if (mysql.Tables.Count != 0)
                {
                    //mysql2
                    mysql2.Tables[0].Columns[0].ColumnName = "RFID号";
                    mysql2.Tables[0].Columns[1].ColumnName = "存放位置";
                    dataGridView2.DataSource = mysql2.Tables[0];
                    //mysql5
                    mysql5.Tables[0].Columns[0].ColumnName = "RFID号";
                    mysql5.Tables[0].Columns[1].ColumnName = "工号";
                    mysql5.Tables[0].Columns[2].ColumnName = "出库时间";
                    mysql5.Tables[0].Columns[3].ColumnName = "操作员编号";
                    mysql5.Tables[0].Columns[4].ColumnName = "存放位置";
                    mysql5.Tables[0].Columns[5].ColumnName = "出库状态";
                }
                else
                {
                    MessageBox.Show("查询入库模板数据失败！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (b3 == true)
            {
                if (mysql.Tables.Count != 0)
                {
                    mysql2.Tables[0].Columns[0].ColumnName = "id";
                    mysql2.Tables[0].Columns[1].ColumnName = "RFID号";
                    mysql2.Tables[0].Columns[2].ColumnName = "姓名";
                    mysql2.Tables[0].Columns[3].ColumnName = "存放位置";
                    mysql2.Tables[0].Columns[4].ColumnName = "操作状态";
                    dataGridView3.DataSource = mysql2.Tables[0];
                }
                else
                {
                    MessageBox.Show("查询入库模板数据失败！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public DataSet load_mysql(string databases,string table,string typestr)
        {
            DataSet mysql_j = new DataSet();
            string mysqlcom1 = "use " + databases;
            string mysqlselect = "select*from " + table;
            //切换数据库
            mysqlconn0.mysqlcom(mysqlcom1);
            //导出表
            mysql_j = mysqlconn0.mysqlselectcom(mysqlselect);
            switch (typestr)
            {
                case "nowstore":
                    mysql_j.Tables[0].Columns[0].ColumnName = "ID";
                    mysql_j.Tables[0].Columns[1].ColumnName = "RFID号";
                    mysql_j.Tables[0].Columns[2].ColumnName = "在库状态";
                    mysql_j.Tables[0].Columns[3].ColumnName = "姓名";
                    mysql_j.Tables[0].Columns[4].ColumnName = "身份证号";
                    mysql_j.Tables[0].Columns[5].ColumnName = "存放位置";
                    mysql_j.Tables[0].Columns[6].ColumnName = "入库时间";
                    mysql_j.Tables[0].Columns[7].ColumnName = "单位名称";
                    mysql_j.Tables[0].Columns[8].ColumnName = "操作员编号";
                    break;
                case "borrow":
                    mysql_j.Tables[0].Columns[0].ColumnName = "ID";
                    mysql_j.Tables[0].Columns[1].ColumnName = "RFID号";
                    mysql_j.Tables[0].Columns[2].ColumnName = "操作状态";
                    mysql_j.Tables[0].Columns[3].ColumnName = "姓名";
                    mysql_j.Tables[0].Columns[4].ColumnName = "身份证号";
                    mysql_j.Tables[0].Columns[5].ColumnName = "存放位置";
                    mysql_j.Tables[0].Columns[6].ColumnName = "操作时间";
                    mysql_j.Tables[0].Columns[7].ColumnName = "单位名称";
                    mysql_j.Tables[0].Columns[8].ColumnName = "操作员编号";
                    break;
                case "return":
                    mysql_j.Tables[0].Columns[0].ColumnName = "ID";
                    mysql_j.Tables[0].Columns[1].ColumnName = "RFID号";
                    mysql_j.Tables[0].Columns[2].ColumnName = "操作状态";
                    mysql_j.Tables[0].Columns[3].ColumnName = "姓名";
                    mysql_j.Tables[0].Columns[4].ColumnName = "身份证号";
                    mysql_j.Tables[0].Columns[5].ColumnName = "存放位置";
                    mysql_j.Tables[0].Columns[6].ColumnName = "操作时间";
                    mysql_j.Tables[0].Columns[7].ColumnName = "单位名称";
                    mysql_j.Tables[0].Columns[8].ColumnName = "操作员编号";
                    break;
                case "new":
                    mysql_j.Tables[0].Columns[0].ColumnName = "ID";
                    mysql_j.Tables[0].Columns[1].ColumnName = "RFID号";
                    mysql_j.Tables[0].Columns[2].ColumnName = "在库状态";
                    mysql_j.Tables[0].Columns[3].ColumnName = "姓名";
                    mysql_j.Tables[0].Columns[4].ColumnName = "身份证号";
                    mysql_j.Tables[0].Columns[5].ColumnName = "存放位置";
                    mysql_j.Tables[0].Columns[6].ColumnName = "入库时间";
                    mysql_j.Tables[0].Columns[7].ColumnName = "单位名称";
                    mysql_j.Tables[0].Columns[8].ColumnName = "操作员编号";
                    break; 
                case "gate_in":
                    mysql_j.Tables[0].Columns[0].ColumnName = "RFID号";
                    mysql_j.Tables[0].Columns[1].ColumnName = "在库状态";
                    mysql_j.Tables[0].Columns[2].ColumnName = "进出方向";
                    mysql_j.Tables[0].Columns[3].ColumnName = "时  间";
                    break; 
                case "gate_out":
                    mysql_j.Tables[0].Columns[0].ColumnName = "RFID号";
                    mysql_j.Tables[0].Columns[1].ColumnName = "在库状态";
                    mysql_j.Tables[0].Columns[2].ColumnName = "进出方向";
                    mysql_j.Tables[0].Columns[3].ColumnName = "时  间"; 
                    break;
                case "gate_warning":
                    mysql_j.Tables[0].Columns[0].ColumnName = "RFID号";
                    mysql_j.Tables[0].Columns[1].ColumnName = "在库状态";
                    mysql_j.Tables[0].Columns[2].ColumnName = "进出方向";
                    mysql_j.Tables[0].Columns[3].ColumnName = "时  间"; 
                    break;
                case "transfer":
                    mysql_j.Tables[0].Columns[0].ColumnName = "ID";
                    mysql_j.Tables[0].Columns[1].ColumnName = "干部姓名";
                    mysql_j.Tables[0].Columns[2].ColumnName = "原单位";
                    mysql_j.Tables[0].Columns[3].ColumnName = "身份证号";
                    mysql_j.Tables[0].Columns[4].ColumnName = "转递类型";
                    mysql_j.Tables[0].Columns[5].ColumnName = "转递原因";
                    mysql_j.Tables[0].Columns[6].ColumnName = "接收单位";
                    mysql_j.Tables[0].Columns[7].ColumnName = "转递时间";
                    mysql_j.Tables[0].Columns[8].ColumnName = "操作员";
                    break;
                default: break;
            }
            
            return mysql_j;
        }
        #endregion

        #region   清理数据表SQL操作
        private void mysql_clear(string database,string tablename,DateTime starttime,int day_num)
        {
            try
            {
                DateTime endtime=starttime.AddDays(day_num);
                string mysqlcom1 = "use " + database;
                //string mysqlselect = "delete from " + tablename;
                string mysqlselect = "delete from "+ tablename+" where operate_time between '"+ starttime.ToString()+"' and '"+ endtime.ToString()+ "'";  //删除tb表时间段的数据
                mysqlconn0.mysqlcom(mysqlcom1);           //切换数据库
                mysqlconn0.mysqlselectcom(mysqlselect);   //执行sql命令
            }
            catch(Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
        }

        //删除转递档案
        public void mysql_clear_data(string database, string tablename,List<string> peridstr)
        {
            try
            {
                string mysqlcom1 = "use " + database;
                mysqlconn0.mysqlcom(mysqlcom1);           //切换数据库
                string mysqlselect = "";
                for (int i = 0; i < peridstr.Count; i++)
                {
                    mysqlselect = "delete from " + tablename + " where tid=" + "\"" + peridstr[i] + "\"";  //删除tb表时间段的数据
                    mysqlconn0.mysqlselectcom(mysqlselect);   //执行sql命令
                }
                mysqlselect ="alter table "+ tablename + " drop id";
                mysqlconn0.mysqlselectcom(mysqlselect);   //执行sql命令
                mysqlselect = "alter table " + tablename + " add id int(3) not null first";
                mysqlconn0.mysqlselectcom(mysqlselect);   //执行sql命令
                mysqlselect = "alter table " + tablename + " modify column id int(3) not null auto_increment, add primary key(id)";
                mysqlconn0.mysqlselectcom(mysqlselect);   //执行sql命令
                mysqlselect = "select* from " + tablename + " order by id";
                mysqlconn0.mysqlselectcom(mysqlselect);   //执行sql命令
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
        }

        //删除所有表数据
        public void mysql_clear_alldata(string database, string tablename)
        {
            try
            {
                string mysqlcom1 = "use " + database;
                mysqlconn0.mysqlcom(mysqlcom1);           //切换数据库
                string mysqlselect = "";
                mysqlselect = "delete from " + tablename;  //删除数据
                mysqlconn0.mysqlselectcom(mysqlselect);   //执行sql命令
            }
            catch
            {
                MessageBox.Show("删除数据异常，请重新操作！");
            }
        }

        #endregion

        #region  应还档案盒提醒sql查询
        public DataSet load_borrowwaring(string databases, string table, string borrowid)
        {
            DataSet mysql_bw = new DataSet();
            string mysqlcom1 = "use " + databases;
            string mysqlselect = "select*from " + table + " where tid=" + "\"" + borrowid + "\"";
            try
            {
                mysqlconn0.mysqlcom(mysqlcom1);    //切换数据库
                mysql_bw = mysqlconn0.mysqlselectcom(mysqlselect);   //查询字段
                return mysql_bw;
            }
            catch
            {
                return mysql_bw;
            }
        }
        #endregion

        #region  Mysql应用盘点查询

        //单独盘点功能
        private void load_pandian(string str1, string str2)
        {
            string mysqlcom1 = "use " + str1;
            string mysqlshowltable = "show tables";
            string mysqlselect = "select*from " + str2;
            string mysqlcom2 = "use " + "store";
            string mysqlselect1 = "select*from " + "tablename";
            List<string> list1 = new List<string>();
            //查询表
            mysqlconn00.mysqlcom2(mysqlcom1);                  //切换数据库
            list1 = mysqlconn00.mysqlshow2(mysqlshowltable);   //数据表列表查询
            //选中模板表1,用于存储
            mysql3 = mysqlconn00.mysqlselectcom2(mysqlselect);
            mysqlconn0.mysqlcom(mysqlcom2);
            mysql = mysqlconn0.mysqlselectcom(mysqlselect1);

            if (list1.Count == 0)
            {
                MessageBox.Show("盘点失败！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //借阅出库盘点功能
        private void load_pandian_borrow(string str1, string str2)
        {
            string mysqlcom1 = "use " + str1;
            string mysqlshowltable = "show tables";
            string mysqlselect = "select*from " + str2;
            string mysqlcom2 = "use " + "store";
            string mysqlselect1 = "select*from " + "tablename";
            List<string> list1 = new List<string>();
            //查询表
            mysqlconn00.mysqlcom2(mysqlcom1);                  //切换数据库
            list1 = mysqlconn00.mysqlshow2(mysqlshowltable);   //数据表列表查询
            //选中模板表1,用于存储
            mysql3 = mysqlconn00.mysqlselectcom2(mysqlselect);
            mysqlconn0.mysqlcom(mysqlcom2);

            //mysql = mysqlconn0.mysqlselectcom(mysqlselect1);
            if (list1.Count == 0)
            {
                MessageBox.Show("盘点失败！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region  test二维码生成加密

        //public void button2_Click(object sender, EventArgs e)
        //{

        //    this.Enabled = false;
        //    //Form3 f3 = new Form3(image, txt);
        //    //f3.Show();
        //    //f3.ShowDialog();
        //    this.Enabled = true;
        //}

        #endregion

        #region  翻页按钮
        public void button3_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dat1 = mysql.Tables[0].Clone();  //克隆mysql.tables[0]结构框架
                for (int i = 0; i < 4; i++)
                {
                    dat1.Rows.Add(mysql.Tables[0].Rows[i].ItemArray);
                }
                dataGridView1.DataSource = dat1;
            }
            catch
            {
                return;
            }
        }
        #endregion

        #region  二维码生成

        //二维码生成
        public Image image;
        public string txt, txt1;
        public string txt_type;

        public void produceqr()
        {
            qrimage_code qr_code = new qrimage_code();
            //image = qr_code.qrimage(txt, 3, 11);
            //image = qr_code.qrimage(txt, 4, 7);
            //PrintPage();
            //获取最高权限口令
            string passkey1 = check.logincheckset_lingdao("guanlingdao_keywd");
            string passkey = jiema(passkey1);
            List<string> perid_qr=new List<string>();
            //入库
            if (b1 == true)
            {
                //批入
                if (button31.Enabled == false && openlist.Count == 0&&operate_type!="D1")
                {
                    perid_qr.Clear();
                    //出二维码
                    txt_type = "新增成功二维码";
                    txt1 = System.DateTime.Now.ToString();  //时效性加密
                    txt = "%I2%" + txt1 + "%";
                    for (int i = 0; i < mysql5.Tables[0].Rows.Count; i++)
                    {
                        if (perid_qr.IndexOf(mysql5.Tables[0].Rows[i][1].ToString()) >= 0)
                        { continue; }
                        perid_qr.Add(mysql5.Tables[0].Rows[i][1].ToString());
                        txt = "%I2%" + txt1 + "%";
                        txt += mysql5.Tables[0].Rows[i][1] + "," + passkey + "%";
                        string descode = des_passkey(txt, passkey);

                        image = qr_code.qrimage(descode, 4, 7);
                        PrintPage();
                    }
                }

                //归还入库
                if (button31.Enabled == false && openlist.Count == 0 && operate_type == "D1")
                {
                    perid_qr.Clear();
                    //出二维码
                    txt_type = "归还成功二维码";
                    txt1 = System.DateTime.Now.ToString();  //时效性加密
                    txt = "%R2%" + txt1 + "%";
                    for (int i = 0; i < mysql5.Tables[0].Rows.Count; i++)
                    {
                        if (perid_qr.IndexOf(mysql5.Tables[0].Rows[i][1].ToString()) >= 0)
                        { continue; }
                        perid_qr.Add(mysql5.Tables[0].Rows[i][1].ToString());
                        txt = "%R2%" + txt1 + "%";
                        txt += mysql5.Tables[0].Rows[i][1] + "," + passkey + "%";
                        string descode = des_passkey(txt, passkey);

                        image = qr_code.qrimage(descode, 4, 7);
                        PrintPage();
                    }
                }     
            }

            //借阅
            if (b2 == true&& borrow_save==true)
            {
                perid_qr.Clear();
                //批借
                if (operate_type == "O1")  //修改C1为批量借
                {
                    //出二维码
                    txt_type = "借阅成功二维码";                    
                    txt1 = System.DateTime.Now.ToString();  //时效性加密                    
                    for (int i = 0; i < mysql5.Tables[0].Rows.Count; i++)
                    {
                        if (perid_qr.IndexOf(mysql5.Tables[0].Rows[i][1].ToString())>=0)
                        { continue; }
                        perid_qr.Add(mysql5.Tables[0].Rows[i][1].ToString());
                        txt = "%O2%" + txt1 + "%";
                        txt += mysql5.Tables[0].Rows[i][1] +","+ passkey+"%";
                        string descode = des_passkey(txt, passkey);

                        image = qr_code.qrimage(descode, 4, 7);
                        PrintPage();
                    }
                }
            }
        }
        #endregion

        #region   最高权限口令解码
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

        #region 二维码小票打印
        public void PrintPage()
        {
            PrintPreviewDialog printPreviewDialog2 = new PrintPreviewDialog();//新建打印预览窗体   
            PrintDocument PrintDocument2 = new PrintDocument();//新建打印对象                     
            PageSetupDialog PageSetupDialog2 = new PageSetupDialog();//新建打印设置 
            PrintDocument2.PrintPage += new PrintPageEventHandler(printDocument2_PrintPage);//新建打印输出                         
            PrintDocument2.DefaultPageSettings.Landscape = false; //False 横打
            PaperSize pkCustomSize1 = new PaperSize("6cun", 580, 300);	//新建一个页面尺寸（6寸照片4*6英寸）
            PrintDocument2.DefaultPageSettings.PaperSize = pkCustomSize1;
            printPreviewDialog2.Document = PrintDocument2;//获取打印预览
            PrintController printController = new StandardPrintController();
            PrintDocument2.PrintController = printController;
            PrintDocument2.Print();

        }
        public void printDocument2_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Image obj = image;
            e.Graphics.DrawString(txt_type, new Font("黑体", 9, FontStyle.Regular), new SolidBrush(Color.Red), 60, 0);//写string
            e.Graphics.DrawImage(obj, 0, 15);//绘制二维码 (15,15) 
            tid = "TID:" + tid;
            //e.Graphics.DrawString(tid, new Font("黑体", 9, FontStyle.Regular), new SolidBrush(Color.Red), 15, 190);//写string
            e.Graphics.DrawString(System.DateTime.Now.ToString(), new Font("黑体", 9, FontStyle.Regular), new SolidBrush(Color.Red), 25, 205);//写string 
            PaperSize pkCustomSize1 = new PaperSize("6cun", 500, 300);
            e.PageSettings.PaperSize = pkCustomSize1;
        }
        #endregion

        #region  Mysql查询字段
        public string mysqlcheck(string databasename)
        {
            DataSet mysql0 = new DataSet();
            string backlocal = "";
            string mysqlcom1 = "use " + databasename;
            //string mysqlshowltable = "show tables";
            string mysqlselect = "select*from " + "tablename " + "where " + "id=1";
            List<string> list1 = new List<string>();
            //查询表
            mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
            //list1 = mysqlconn0.mysqlshow(mysqlshowltable);   //数据表列表查询
            //选中模板表
            mysql0 = mysqlconn0.mysqlselectcom(mysqlselect);

            if (mysql0.Tables.Count != 0)
            {
                //dataGridView1.DataSource = mysql.Tables[0];
                return backlocal;
            }
            else
            {
                MessageBox.Show("查询入库模板数据失败！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return backlocal;
        }
        #endregion

        #region  查询数据表id号
        private void chenkcount()
        {
            //首先查询id数据条数
            DataSet mysql1 = new DataSet();
            int count2 = 0,count1=0;
            bool syscount = false;
            string mysqlcom1 = "use store";
            string mysqlselect = "select*from tablename";
            string mysqlselect1 = "select*from tablename where position=";
            List<string> list1 = new List<string>();
            //查询表
            mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
            //list1 = mysqlconn0.mysqlshow(mysqlshowltable);   //数据表列表查询
            //选中模板表
            try
            {
                mysql1 = mysqlconn0.mysqlselectcom(mysqlselect);
                count = mysql1.Tables[0].Rows.Count + 1;
                //查询柜门编号
                for (int i = 1; i < (int.Parse(controlsum)+2); i++)
                {
                    for (int j = 1; j < 31; j++)
                    {
                        mysql1 = null;
                        mysqlselect1 = "select*from tablename where position=" + "\"" + i.ToString() + "-" + j.ToString() + "\"";
                        mysql1 = mysqlconn0.mysqlselectcom(mysqlselect1);   //查询字段
                        count2 = mysql1.Tables[0].Rows.Count;
                        if (count2 == 0&&syscount==false)
                        {
                            num = i;
                            box = j;
                            syscount = true;
                        }
                        if (count2!=0)
                        {
                            count1++;
                        }
                    }
                }
                usenownum = count1;
            }
            catch
            {
                mysqlconn0.mysqlconn1.Dispose();
                num = 1;
                box = 1;
                count = 1;

            }
        }
        #endregion

        #region  查柜体编号的合法性
        public bool chenkcount_position(string position)
        {
            //首先查询id数据条数
            bool current_flg = false;
            DataSet mysql1 = new DataSet();
            int count2 = 0;
            string mysqlcom1 = "use store";
            string mysqlselect1 = "select*from tablename where position="+ position;
            List<string> list1 = new List<string>();
            //查询表
            mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
            //选中模板表
            try
            {
                //查询柜门编号
                mysql1 = null;
                mysqlselect1 = "select*from tablename where position=" + "\"" + position + "\"";
                mysql1 = mysqlconn0.mysqlselectcom(mysqlselect1);   //查询字段
                count2 = mysql1.Tables[0].Rows.Count;
                if (count2 == 0)
                {
                    return current_flg;
                }
                else
                {
                    return current_flg = true;
                }
            }
            catch
            {
                return current_flg = true;
            }
        }
        #endregion

        #region  查询在库数据，通过关键字name或perid查询,已有库存
        /// <summary>
        /// 通用checking
        /// </summary>
        /// <param name="strr"></param>
        /// <returns></returns>
        public bool check_b1perid = false;
        public List<string> chenking(string strr)
        {
            //首先查询id数据条数
            int chenum = 0;
            string chenumstr;
            List<string> chenumlist = new List<string>();
            DataSet mysql1 = new DataSet();
            string mysqlcom1 = "use store";
            string mysqlshowltable = "show tables";
            string mysqlselect = "select*from tablename where perid=" + "\""+strr + "\"";           //查perid
            string mysqlselecttid = "select*from tablename where tid=" + "\"" + strr + "\"";        //查tid
            List<string> list1 = new List<string>();
            
            //查询表
            mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
            //list1 = mysqlconn0.mysqlshow(mysqlshowltable);   //数据表列表查询
            //选中模板表
            try
            {
                //归还时查询应还盒数
                if (b1 == true && check_b1perid == true)
                {
                    mysql1 = mysqlconn0.mysqlselectcom(mysqlselect);
                    for (int i = 0; i < mysql1.Tables[0].Rows.Count; i++)
                    {
                        if (mysql1.Tables[0].Rows[i][2].ToString() == "0")
                        {
                            chenumlist.Add("0");
                        }
                    }
                    return chenumlist;
                }
                //新增
                if (b1 == true)
                {
                    mysql1 = mysqlconn0.mysqlselectcom(mysqlselect);
                    chenum = mysql1.Tables[0].Rows.Count;
                    if (chenum <= (int.Parse(persumh)+1)*3 && chenum > 0)
                    {
                        for (; mysql1.Tables[0].Rows.Count != 1;)
                        {
                            mysql1.Tables[0].Rows.RemoveAt(0);
                        }
                        chenumstr = mysql1.Tables[0].Rows[0][5].ToString();
                        var nn = chenumstr.Split(new char[1] { '-' });
                        chenumlist.Add(nn[0]);
                        chenumlist.Add(nn[1]);
                        chenumlist.Add(chenum.ToString());  //返回在库盒数
                        if (chenum == (int.Parse(persumh) + 1) * 3)
                        {
                            chenumlist.Add("0");   //单人档案数达到最大限（3份）
                            return chenumlist;
                        }
                        return chenumlist;
                    }
                    
                    //判断是否在库，防止重复入库
                    mysql1 = mysqlconn0.mysqlselectcom(mysqlselecttid);
                    mysqll = mysql1;
                    chenum = mysql1.Tables[0].Rows.Count;
                    if (chenum != 0)
                    {
                        chenumlist.Add("1");   //已经存在该tid档案
                    }
                    return chenumlist;
                }
               
                //借阅
                if (b2 == true)
                {
                    mysql1 = mysqlconn0.mysqlselectcom(mysqlselect);
                    chenum = mysql1.Tables[0].Rows.Count;
                    if (chenum != 0)
                    {
                        mysqll = mysql1;
                        //返回盒数
                        for (int i = 0; i < mysql1.Tables[0].Rows.Count; i++)
                        {
                            if (mysql1.Tables[0].Rows[i][2].ToString() == "1")
                            {
                                chenumlist.Add("0");
                            }
                            if (mysql1.Tables[0].Rows[i][2].ToString() == "0")
                            {
                                chenumlist.Add("00");
                                if (chenumlist.Count == mysql1.Tables[0].Rows.Count)
                                {
                                    chenumlist.Add("mm");
                                }
                            }
                        }
                    }
                    else
                    {
                        mysql1 = mysqlconn0.mysqlselectcom(mysqlselecttid);
                        chenum = mysql1.Tables[0].Rows.Count;
                        mysqll = mysql1;
                        if (chenum == 1)
                        {
                            if (mysql1.Tables[0].Rows[0][2].ToString() == "0")
                            {
                                chenumlist.Add("0");
                            }
                            if (mysql1.Tables[0].Rows[0][2].ToString() == "1"&& borrow_checkout == true)
                            {
                                chenumlist.Add("0");
                            }

                        }
                        if (chenum == 0)
                        {
                            chenumlist.Add("1");
                        }
                    }

                }

                if (goout == true||comin==true)
                {
                    //判断是否在库，防止重复入库
                    mysql1 = mysqlconn0.mysqlselectcom(mysqlselecttid);
                    chenum = mysql1.Tables[0].Rows.Count;
                    if (chenum != 0)
                    {
                        if (mysql1.Tables[0].Rows[0][2].ToString() == "1")
                        {
                            chenumlist.Add("0");   //tid档案已经入库
                        }
                        else { return chenumlist; }
                    }
                }
                return chenumlist;
            }
            catch
            {
                mysqlconn0.mysqlconn1.Dispose();
                return chenumlist;
            }
        }


        public int chenking_position(string position)
        {
            //首先查询id数据条数
            int chenum = 0;
            List<string> chenumlist = new List<string>();
            DataSet mysql1 = new DataSet();
            string mysqlcom1 = "use store";
            string mysqlselect = "select*from tablename where position =" + "\"" + position + "\"";           //查position
            List<string> list1 = new List<string>();

            //查询表
            mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
            //list1 = mysqlconn0.mysqlshow(mysqlshowltable);   //数据表列表查询
            //选中模板表
            try
            {
                mysql1 = mysqlconn0.mysqlselectcom(mysqlselect);
                chenum = mysql1.Tables[0].Rows.Count;
                return chenum;
            }
            catch
            { }
            return chenum;
        }


        /// <summary>
        /// 查perid
        /// </summary>
        /// <param name="strr"></param>
        /// <returns></returns>
        public string chenking_perid(string strr)
        {
            string rfidstr="";
            DataSet mysql_perid = new DataSet();
            string mysqlcom1 = "use store";
            string mysqlselect = "select*from tablename where perid=" + "\"" + strr + "\"" ;
            try
            {
                mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                mysql_perid = mysqlconn0.mysqlselectcom(mysqlselect);
                for (int i = 0; i < mysql_perid.Tables[0].Rows.Count; i++)
                {
                    if (i== mysql_perid.Tables[0].Rows.Count-1)
                    {
                        rfidstr += mysql_perid.Tables[0].Rows[i][1].ToString() + ",";
                        rfidstr += mysql_perid.Tables[0].Rows[i][5].ToString();
                        break;
                    }
                    rfidstr += mysql_perid.Tables[0].Rows[i][1].ToString() + ",";
                }
                return rfidstr;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return rfidstr;
            }
        }

        /// <summary>
        /// 借出档案数据集查询
        /// </summary>
        /// <param name="strr1"></param>
        /// <returns></returns>
        private int borrow_chenking(string strr1)
        {
            int chenum = 0;
            try
            {
                //首先查询id数据条数                
                string mysqlcom1 = "use store";
                string mysqlselect = "select*from tablename where status=" + "\"" + strr1 + "\"";  //查status

                //查询表
                mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                mysql9 = mysqlconn0.mysqlselectcom(mysqlselect);
                chenum = mysql9.Tables[0].Rows.Count;
                return chenum;
            }
            catch
            { return chenum; }
        }
        /// <summary>
        /// 通道门数据查询
        /// </summary>
        /// <param name="strr"></param>
        /// <returns></returns>
        private void gate_chenking()
        {
            DataSet mysql_gate = new DataSet();
            try
            {
                gooutnum = 0;
                cominnum = 0;
                warningnum = 0;
                //首先查询id数据条数                
                string mysqlcom1 = "use gatenote";
                string mysqlselect1 = "select*from gatenote1 where type=" + "\"" + "comin" + "\"";  //查入门
                string mysqlselect2 = "select*from gatenote1 where type=" + "\"" + "goout" + "\"";  //查出门
                string mysqlselect3 = "select*from gatenote1 where status=" + "\"" + "1" + "\"";    //查异常
                                                                                                    //查询表
                mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
                mysql_gate = mysqlconn0.mysqlselectcom(mysqlselect1);
                cominnum = mysql_gate.Tables[0].Rows.Count;
                mysql_gate = mysqlconn0.mysqlselectcom(mysqlselect2);
                gooutnum = mysql_gate.Tables[0].Rows.Count;
                mysql_gate = mysqlconn0.mysqlselectcom(mysqlselect3);
                warningnum = mysql_gate.Tables[0].Rows.Count;
            }
            catch
            { }
        }
        #endregion

        #region  查询整理档案的方向性
        public int chenking_dilldata(string tid)
        {
            //首先查询id数据条数
            int chenum = 0;
            DataSet mysql1 = new DataSet();
            string mysqlcom1 = "use note19";
            string mysqlselecttid = "select*from dilltable where tid=" + "\"" + tid + "\"";        //查tid
            //查询表
            mysqlconn0.mysqlcom(mysqlcom1);                  //切换数据库
            //选中模板表
            try
            {
                //归还时查询应还盒数
                mysql1 = mysqlconn0.mysqlselectcom(mysqlselecttid);
                chenum = mysql1.Tables[0].Rows.Count;
                return chenum;
            }
            catch
            {
                return chenum;
            }
        }
        #endregion

        #region   show屏幕键盘
        private void screentabshow()
        {
            try
            {
                string dirpath = Application.StartupPath + "\\FreeVK.exe";  //获取应用程序路径
                dynamic file = dirpath;
                if (!System.IO.File.Exists(file))
                { return; }
                Process.Start(file);
            }
            catch (Exception)
            {
            }

        }
        #endregion

        #region  二维码简单时间验证
        public string qrpass(string str1)
        {

            return str1;
        }

        #region  设备连接失败点击重连事件

        //数据库重连
        private void button5_Click(object sender, EventArgs e)
        {
            //数据库链接初始化
            mysqlinit("192.168.21.21");
        }

        #endregion

        #endregion

        #region   定时关闭MessageBox

        public void startkill()
        {
            System.Timers.Timer t1 = new System.Timers.Timer(1000);  //2min定时
            t1.Elapsed += new System.Timers.ElapsedEventHandler(theout3);
            t1.AutoReset = false;  //true一直执行,false执行一次
            t1.Enabled = true;     //是否执行Elapsed事件
        }

        private void theout3(object sender, EventArgs e)
        {
            //查找MessageBox的弹出窗口,注意MessageBox对应的标题
            IntPtr ptr = FindWindow(null, killtxt);
            if (ptr != IntPtr.Zero)
            {
                //查找到窗口则关闭
                PostMessage(ptr, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
        }

        #endregion

        #region  进水传感器消警
        //进水蜂鸣消警
        private void button20_Click_1(object sender, EventArgs e)
        {
            serialport1 serial = new serialport1();
            List<byte[]> xiaojlist = new List<byte[]> { };  //进水
            List<byte> databack4 = new List<byte>(7);
            byte[] xiaoj = { 0x14, 0x06, 0x00, 0x07, 0x00, 0x01, 0x00, 0x00 };  //查1地址进水状态
            xiaoj = crce(xiaoj);
            xiaojlist.Add(xiaoj);

            if (textBox3.Text == "浸 水")
            {
                t.Stop();
                for (;;)
                {
                    databack4 = serial.senddata3(xiaojlist[0], portlist1, port32);
                    if (databack4.Count != 0)
                    {
                        break;
                    }
                }
                t.Start();
            }
        }
        #endregion

        #region  紧急开门
        public void urgentopen(string borednum)
        {
            //t.Stop();
            serialport1 port1 = new serialport1();
            //打开特定门test
            if (borednum == " ")
            {
                MessageBox.Show("请填写柜体编号", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            byte[] testopen = { 0x02, 0x06, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00 };  //读门的状态
            testopen[0] = Convert.ToByte(borednum);   
            //ch1-ch16
            testopen[3] = 0x01;
            testopen[4] = 0xFF;
            testopen[5] = 0xFF;
            testopen = crce(testopen);
            Thread.Sleep(300);
            port1.senddata1(testopen, portlist1, portlist1[0]);

            //ch17-ch30
            testopen[3] = 0x02;
            testopen[4] = 0xFF;
            testopen[5] = 0xFF;
            testopen = crce(testopen);
            Thread.Sleep(20);
            port1.senddata1(testopen, portlist1, portlist1[0]);
            //t.Start();
            Thread.Sleep(20);
            
        }
        #endregion

        #region 刷新连接控件状态
        /// <summary>
        /// 各类数据刷新
        /// </summary>
        public int cominnum = 0, gooutnum = 0; //进出通道门人数统计
        public int warningnum=0;  //检测门异常数据
        public int borrownum = 0; //取走数量
        public int should_borrownum = 0; //应还档案数量
        public int newwnum = 0;   //新增数量
        public int borandretnum = 0; //借还新增数量
        public int transfernum = 0; //应还档案数量
        public int usenownum =0;  //档案柜在用数量，即目前存放了多少柜子
        public int door_closenum = 0; //档案柜关闭数量
        public int door_opennum = 0;  //档案柜打开数量
        
        private void refresh()
        {
            //设备连接异常情况
            if (dataserver == false || gatedoorstatus == false || controlboard == false || RFIDread == false || qrdevicestatu == false || printstatu == false)
            {
                label22.Text = "异  常";
            }
            if (dataserver == true && gatedoorstatus == true && controlboard == true && RFIDread == true && qrdevicestatu == true && printstatu == true)
            {
                label22.Text = "正  常";
            }

            //档案盒状态数据管理
            textBox21.Text = cominnum.ToString();    //通道门进入数量
            textBox26.Text = gooutnum.ToString();    //通道门出去数量
            textBox25.Text = warningnum.ToString();  //通道门异常数量
            textBox20.Text = ((int.Parse(controlsum)+1) * 30*3).ToString();   //档案盒总容量(一期示范库房总容量）
            textBox19.Text = ((count - 1)- borrownum).ToString(); //现存档案总数
            textBox24.Text = (count - 1).ToString(); //新增档案数量（与现存有重复）                                                 
            textBox16.Text = borrownum.ToString();   //取走档案数量
            textBox17.Text = borandretnum.ToString();   //借还档案数量
            textBox18.Text = should_borrownum.ToString(); //应还档案数
            textBox15.Text = transfernum.ToString();   //转递档案数量
            //档案柜状态数据
            textBox10.Text = ((int.Parse(controlsum)+1) * 30).ToString();  //档案柜总容量          
            textBox13.Text = door_opennum.ToString();    //门打开数量
            textBox14.Text = door_closenum.ToString();   //门关闭数量
            textBox11.Text = usenownum.ToString();       //在用档案柜数量
            if (int.Parse(time2) >= 11)
            {
                label9.Text = "（数据时限：" + (int.Parse(time2) - 10) + "年）";
            }
            else
            {
                label9.Text = "（数据时限：" + (int.Parse(time2)+1) + "个月）";
            }

            try
            {
                //打开柜门显示
                if (checkmysqlflag)
                {
                    dataGridView3.DataSource = mysql_openview.Tables[0];
                }
            }
            catch
            { }
           
        }
        #endregion

        #region  历史数据查询
        /// <summary>
        /// 历史数据查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button27_Click(object sender, EventArgs e)
        {
            hisd_load_exdata("note19", "note1901");
            form_datacheck f9 = new form_datacheck(this, mysql8,false,"note19",0);
            f9.Text = "出入库历史查询";
            f9.ShowDialog();
        }
        #endregion

        #region  RFID桌面读卡器功能函数

        private byte fComAdr = 0xff; //当前操作的ComAdr
        private byte fBaud = 5;  //波特率57600
        private int frmcomportindex;
        private int fOpenComIndex; //打开的串口索引号
        private bool ComOpen = false;
        private int fCmdRet = 30; //所有执行指令的返回值
        public int port = 0;

        /// <summary>
        /// 打开串口操作
        /// </summary>
        public void openport()
        {
            //自动连接并打开串口
            int openresult;
            string Edit_CmdComAddr = "FF";
            fComAdr = Convert.ToByte(Edit_CmdComAddr, 16); // $FF;
            openresult = StaticClassReaderB.AutoOpenComPort(ref port, ref fComAdr, fBaud, ref frmcomportindex);
            fOpenComIndex = frmcomportindex;
            if (openresult == 0)
            {
                ComOpen = true;
                if (fBaud > 3)
                {

                }
                else
                {

                }
                autoread(); //自动执行读取写卡器信息
                if ((fCmdRet == 0x35) | (fCmdRet == 0x30))
                {
                    MessageBox.Show("串口通讯错误", "信息提示");
                    StaticClassReaderB.CloseSpecComPort(frmcomportindex);
                    ComOpen = false;
                }
            }
        }

        /// <summary>
        /// 读取写卡器信息
        /// </summary>
        private double fdminfre;
        private double fdmaxfre;
        private void autoread()
        {
            byte[] TrType = new byte[2];
            byte[] VersionInfo = new byte[2];
            byte ReaderType = 0;
            byte ScanTime = 0;
            byte dmaxfre = 0;
            byte dminfre = 0;
            byte powerdBm = 0;
            byte FreBand = 0;
            string Edit_Version = "";  //版本号
            string Edit_ComAdr = "";   //设备地址
            string Edit_scantime = "";  //询查命令最大响应时间
            string Edit_Type = "";   //设备型号
            bool ISO180006B = false;   //设备ISO180006B选择标识
            bool EPCC1G2 = false;      //设备EPCC1G2选择标识
            string Edit_powerdBm = "";  //设备功率
            string Edit_dminfre = "";  //设备最低频率
            string Edit_dmaxfre = "";  //设备最高频率
            //获得读写器信息
            fCmdRet = StaticClassReaderB.GetReaderInformation(ref fComAdr, VersionInfo, ref ReaderType, TrType, ref dmaxfre, ref dminfre, ref powerdBm, ref ScanTime, frmcomportindex);
            if (fCmdRet == 0)
            {
                Edit_Version = Convert.ToString(VersionInfo[0], 10).PadLeft(2, '0') + "." + Convert.ToString(VersionInfo[1], 10).PadLeft(2, '0');
                if (powerdBm > 13) { }
                //ComboBox_PowerDbm.SelectedIndex = 13;
                else
                    //ComboBox_PowerDbm.SelectedIndex = powerdBm;
                    Edit_ComAdr = Convert.ToString(fComAdr, 16).PadLeft(2, '0');
                string Edit_NewComAdr = Convert.ToString(fComAdr, 16).PadLeft(2, '0');  //设置读写器参数
                Edit_scantime = Convert.ToString(ScanTime, 10).PadLeft(2, '0') + "*100ms";
                //ComboBox_scantime.SelectedIndex = ScanTime - 3;  //最大响应时间
                Edit_powerdBm = Convert.ToString(powerdBm, 10).PadLeft(2, '0');

                FreBand = Convert.ToByte(((dmaxfre & 0xc0) >> 4) | (dminfre >> 6));
                switch (FreBand)
                {
                    case 0:
                        {
                            //radioButton_band1.Checked = true;   //频段选择1
                            fdminfre = 902.6 + (dminfre & 0x3F) * 0.4;
                            fdmaxfre = 902.6 + (dmaxfre & 0x3F) * 0.4;
                        }
                        break;
                    case 1:
                        {
                            //radioButton_band2.Checked = true; //频段选择2
                            fdminfre = 920.125 + (dminfre & 0x3F) * 0.25;
                            fdmaxfre = 920.125 + (dmaxfre & 0x3F) * 0.25;
                        }
                        break;
                    case 2:
                        {
                            //radioButton_band3.Checked = true; //频段选择3
                            fdminfre = 902.75 + (dminfre & 0x3F) * 0.5;
                            fdmaxfre = 902.75 + (dmaxfre & 0x3F) * 0.5;
                        }
                        break;
                    case 3:
                        {
                            //radioButton_band4.Checked = true; //频段选择4
                            fdminfre = 917.1 + (dminfre & 0x3F) * 0.2;
                            fdmaxfre = 917.1 + (dmaxfre & 0x3F) * 0.2;
                        }
                        break;
                    case 4:
                        {
                            //radioButton_band5.Checked = true; //频段选择5
                            fdminfre = 865.1 + (dminfre & 0x3F) * 0.2;
                            fdmaxfre = 865.1 + (dmaxfre & 0x3F) * 0.2;
                        }
                        break;
                }
                Edit_dminfre = Convert.ToString(fdminfre) + "MHz";
                Edit_dmaxfre = Convert.ToString(fdmaxfre) + "MHz";
                if (fdmaxfre != fdminfre)
                    //CheckBox_SameFre.Checked = false;   //单频点选择框
                    //ComboBox_dminfre.SelectedIndex = dminfre & 0x3F;
                    //ComboBox_dmaxfre.SelectedIndex = dmaxfre & 0x3F;
                    if (ReaderType == 0x08)
                        Edit_Type = "UHFReader09";
                if ((TrType[0] & 0x02) == 0x02) //第二个字节低第四位代表支持的协议“ISO/IEC 15693”
                {
                    ISO180006B = true;
                    EPCC1G2 = true;
                }
                else
                {
                    ISO180006B = false;
                    EPCC1G2 = false;
                }
            }
            //AddCmdLog("GetReaderInformation", "获取读写器信息", fCmdRet);
        }

        private bool CheckBox_TID = true;  //TID查询标志位
        //启动标签查询
        public void readrfid()
        {
            Inventory();
        }

        private bool fIsInventoryScan;      //标签标志位       
        private string fInventory_EPC_List; //存贮询查列表（如果读取的数据没有变化，则不进行刷新）
        ComboBox ListView1_EPC = new ComboBox();
        private bool fAppClosed; //在测试模式下响应关闭应用程序
        public int CardNum = 0;  //检测到的标签数
        public string sEPC;

        //标签查询函数
        private void Inventory()
        {
            int i;
            int Totallen = 0;
            int EPClen, m;
            byte[] EPC = new byte[5000];
            int CardIndex;
            string temps;
            string s;
            bool isonlistview;
            fIsInventoryScan = true;
            ListViewItem aListItem = new ListViewItem();
            byte AdrTID = 0;
            byte LenTID = 0;
            byte TIDFlag = 0;
            if (CheckBox_TID == false)   //原software的TID_check的复选框状态
            {
                AdrTID = Convert.ToByte("02", 16);  //起始地址
                LenTID = Convert.ToByte("04", 16);  //数据字数
                TIDFlag = 1;
            }
            else
            {
                AdrTID = 0;
                LenTID = 0;
                TIDFlag = 0;
            }
            fCmdRet = StaticClassReaderB.Inventory_G2(ref fComAdr, AdrTID, LenTID, TIDFlag, EPC, ref Totallen, ref CardNum, frmcomportindex);
            if ((fCmdRet == 1) | (fCmdRet == 2) | (fCmdRet == 3) | (fCmdRet == 4) | (fCmdRet == 0xFB))//代表已查找结束，
            {
                byte[] daw = new byte[Totallen];   //Totallen：输出变量，EPClenandEPC的字节数
                Array.Copy(EPC, daw, Totallen);    //指向输出数组变量（输出的是每字节都转化为字符的数据）。是读到的电子标签的EPC数据，是一张标签的EPC长度+一张标签的EPC号，依此累加。
                temps = ByteArrayToHexString(daw);
                fInventory_EPC_List = temps;            //存贮记录
                m = 0;

                /*while (ListView1_EPC.Items.Count < CardNum)
                  {
                      aListItem = ListView1_EPC.Items.Add((ListView1_EPC.Items.Count + 1).ToString());
                      aListItem.SubItems.Add("");
                      aListItem.SubItems.Add("");
                      aListItem.SubItems.Add("");
                 * 
                  }*/
                if (CardNum == 0)    //CardNum：输出变量，电子标签的张数。
                {
                    fIsInventoryScan = false;
                    return;
                }
                for (CardIndex = 0; CardIndex < CardNum; CardIndex++)
                {
                        EPClen = daw[m];
                        sEPC = temps.Substring(m * 2 + 2, EPClen * 2);
                        m = m + EPClen + 1;
                        if (sEPC.Length != EPClen * 2)
                            return;
                        isonlistview = false;
                        for (i = 0; i < ListView1_EPC.Items.Count; i++)     //判断是否在Listview列表内
                        {
                            if (sEPC == "EPC号")   //ListView1_EPC.Items[i].SubItems[1].Text="EPC号"
                            {
                                //aListItem = ListView1_EPC.Items[i];
                                ChangeSubItem(aListItem, 1, sEPC);
                                isonlistview = true;
                            }
                        }
                        if (!isonlistview)
                        {
                            //aListItem = ListView1_EPC.Items.Add((ListView1_EPC.Items.Count + 1).ToString());   //加标签
                            aListItem.SubItems.Add("");
                            aListItem.SubItems.Add("");
                            aListItem.SubItems.Add("");
                            s = sEPC;
                            ChangeSubItem(aListItem, 1, s);
                            s = (sEPC.Length / 2).ToString().PadLeft(2, '0');
                            ChangeSubItem(aListItem, 2, s);
                            if (!CheckBox_TID)
                            {

                            }
                        }
                }
            }

            fIsInventoryScan = false;
            if (fAppClosed)
                Close();
        }

        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            return sb.ToString().ToUpper();

        }

        public void ChangeSubItem(ListViewItem ListItem, int subItemIndex, string ItemText)
        {
            if (subItemIndex == 1)
            {
                if (ItemText == "")
                {
                    ListItem.SubItems[subItemIndex].Text = ItemText;
                    if (ListItem.SubItems[subItemIndex + 2].Text == "")
                    {
                        ListItem.SubItems[subItemIndex + 2].Text = "1";
                    }
                    else
                    {
                        ListItem.SubItems[subItemIndex + 2].Text = Convert.ToString(Convert.ToInt32(ListItem.SubItems[subItemIndex + 2].Text) + 1);
                    }
                }
                else
                if (ListItem.SubItems[subItemIndex].Text != ItemText)
                {
                    ListItem.SubItems[subItemIndex].Text = ItemText;
                    ListItem.SubItems[subItemIndex + 2].Text = "1";
                }
                else
                {
                    ListItem.SubItems[subItemIndex + 2].Text = Convert.ToString(Convert.ToInt32(ListItem.SubItems[subItemIndex + 2].Text) + 1);
                    if ((Convert.ToUInt32(ListItem.SubItems[subItemIndex + 2].Text) > 9999))
                        ListItem.SubItems[subItemIndex + 2].Text = "1";
                }

            }
            if (subItemIndex == 2)
            {
                if (ListItem.SubItems[subItemIndex].Text != ItemText)
                {
                    ListItem.SubItems[subItemIndex].Text = ItemText;
                }
            }

        }

        #endregion

        #region  通道门功能函数
        //通道门初始化
        public void gatedoorinit()
        {
            if (btConnectTcp("192.168.1.190", "6000"))
            {
                bt_GetDeviceInfo();
            }
            timer2.Enabled = true;
        }

        string tb_Port, text_ipaddr;
        private byte ControllerAdr = 0xff;  //设备地址
        private int PortHandle = -1;        //返回代码
        private byte IRStatus;
        private byte fModel;
        public bool gatestatus = false;

        //打开设备连接网口
        private bool btConnectTcp(string text_ipaddr, string tb_Port)
        {
            try
            {
                if (tb_Port == "" || text_ipaddr == "")
                {
                    return gatestatus;
                }
                int Port = Convert.ToInt32(tb_Port, 10);
                string ipAddr = text_ipaddr;
                fCmdRet = Device.OpenNetPort(Port, ipAddr, ref ControllerAdr, ref PortHandle);
                if (fCmdRet == 0)
                {
                    //bt_GetDeviceInfo_Click(null, null);  //读取设备信息
                    //EnableForm();
                    //btConnectTcp.Enabled = false;
                    //btDisConnectTcp.Enabled = true;
                    fModel = 0;
                    fCmdRet = Device.ModeSwitch(ref ControllerAdr, ref fModel, ref IRStatus, PortHandle);  //通道机有两种工作模式：通道管理模式，EAS 模式和强制EAS模式。
                    if (fCmdRet == 0)
                    {
                        gatestatus = true;
                        //连接成功，模式判断
                        if (fModel == 0)
                        {
                            //panel2.Enabled = true;
                            //panel3.Enabled = false;
                            //com_mode.SelectedIndex = 0;
                        }
                        else
                        {
                            //panel2.Enabled = false;
                            //panel3.Enabled = true;
                            //com_mode.SelectedIndex = 1;
                        }
                        //RefreshFreeRate(IRStatus);  //红外状态
                        return gatestatus;
                    }
                    return gatestatus;
                }
                else
                {
                    MessageBox.Show("通讯错误");
                    return gatestatus;
                }
                //AddCmdLog("OpenComPort", "打开串口", fCmdRet);
            }
            catch (System.Exception ex)
            {
                return gatestatus;
            }
        }

        //关闭网络通信链接
        private void btDisConnectTcp()
        {
            fCmdRet = Device.CloseNetPort(PortHandle);  //关闭网口
            if (fCmdRet == 0)
            {
                //DisableForm();
                //btConnectTcp.Enabled = true;
                //btDisConnectTcp.Enabled = false;
            }
            //AddCmdLog("CloseSpecComPort", "关闭串口", fCmdRet);
        }

        //获取设备信息
        private void bt_GetDeviceInfo()
        {
            byte Productcode = 0;
            byte MainVer = 0;
            byte SubVer = 0;
            string tb_version;
            string textBox1 = "90";
            string textBox2 = "91";
            fCmdRet = Device.GetControllerInfo(ref ControllerAdr, ref Productcode, ref MainVer, ref SubVer, ref IRStatus, PortHandle);
            if (fCmdRet == 0)
            {
                textBox1 = Convert.ToString(Productcode, 16).PadLeft(2, '0').ToUpper();
                if (textBox1 == "90")
                {
                    tb_version = "RRU-CH-WL";
                }
                if (textBox1 == "91")
                {
                    tb_version = "RRU-CH-C16058";
                }
                textBox2 = Convert.ToString(MainVer, 16).PadLeft(2, '0').ToUpper() + "." + Convert.ToString(SubVer, 16).PadLeft(2, '0').ToUpper();
            }
            //AddCmdLog("I", "基本信息", fCmdRet);
        }

        private void RefreshFreeRate(byte IRStatus)
        {

            //if ((IRStatus & 0x04) == 0)
            //    MessageBox.Show("红外3：正常", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //else
            //    MessageBox.Show("红外3：被阻挡", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //if ((IRStatus & 0x08) == 0)
            //    MessageBox.Show("红外4：正常", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //else
            //    MessageBox.Show("红外4：被阻挡", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }


        string txt_Num;//标签张数

        //存盘模式，停止询查
        private void btStop(object sender, EventArgs e)
        {
            timer2.Enabled = false;
        }

        public bool IsGetting;         //是否正在执行'C'命令标志,

        //存盘模式，开始询查
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (IsGetting) return;
            IsGetting = true;
            GetNMessage();
            //清空通道门寄存器
            gate_cleardata();
            IsGetting = false;
        }

        //清空寄存器
        public void gate_cleardata()
        {
            fCmdRet = Device.ClearControllerBuffer(ref ControllerAdr, ref IRStatus, PortHandle);
            if (fCmdRet == 0)
                RefreshFreeRate(IRStatus);
        }

        //过通道门标签list
        public List<string> cardlist = new List<string>();  //实时标签
        public List<string> inlist = new List<string>();    //进入标签
        public bool comin = false, goout = false;
        public bool warning=false;
        public List<string> checkout = new List<string>();    //check结果标签
        public bool borrow_checkout = false;
        public bool borrow_checkout_r = false;
        private void GetNMessage()
        {
            int tm = 0;
            string InOrOut;
            byte InFlag, MsgLength, MsgType;
            byte[] Msg = new byte[300];
            //string year, month, Dates, Hour, minutes, second;
            string tstr="";
            
            IsGetting = true;     //进入该过程时将正在执行标志位.
            MsgLength = 0;
            MsgType = 0;
            InOrOut = "";
            fCmdRet = Device.GetChannelMessage(ref ControllerAdr, Msg, ref MsgLength, ref MsgType, ref IRStatus, PortHandle);
            //AddCmdLog("Get", "获取盘存信息", fCmdRet);
            if ((fCmdRet == 0) && (MsgType == 0))
            {
                int CardNum = Msg[6];
                if (CardNum == 0)
                {
                    if (comin == true || goout == true)
                    {
                        if (comin == true)
                        {
                            //整理出入库档案避警
                            tm = cardlist.Count;
                            for (i = 0; i < tm; i++)
                            {
                                try
                                {
                                    if (chenking_dilldata(cardlist[i]) != 0)
                                    {
                                        cardlist.Remove(cardlist[i]);
                                        i--;
                                    }
                                }
                                catch
                                { }
                            }
                            //计数
                            cominnum += cardlist.Count;
                            checkout = cardcheck(cardlist);
                            if (checkout.Count != 0)
                            {
                                warning = true;
                                //检测门异常数据
                                warningnum+=checkout.Count;
                                //报警
                                Buzzer((byte)1, (byte)1, (byte)10, (byte)1, (byte)1, (byte)10);
                                //存储数据
                                if (b1 != true && b2 != true)
                                {
                                    savedate();
                                }
                                cardlist.Clear();
                                for (int i = 0; i < checkout.Count; i++)
                                {
                                    tstr += checkout[i] + " ";
                                }
                                
                                //屏幕警告显示
                                killtxt = "Warning";
                                startkill();
                                MessageBox.Show("请注意，编号为" + tstr + "的档案非法！请检查...", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            }
                            else
                            {
                                warning = false;
                                //存储数据
                                if (b1 != true && b2 != true)
                                {
                                    savedate();
                                }
                                cardlist.Clear();
                                //提示音 
                                Buzzer((byte)1, (byte)1, (byte)1, (byte)1, (byte)1, (byte)1);
                            }
                        }
                        if (goout == true)
                        {
                            //整理出入库档案避警
                            tm = cardlist.Count;
                            for (i = 0; i < tm; i++)
                            {
                                try
                                {
                                    if (chenking_dilldata(cardlist[i]) != 0)
                                    {
                                        cardlist.Remove(cardlist[i]);
                                        i--;
                                    }
                                }
                                catch
                                { }
                            }
                            gooutnum += cardlist.Count;
                            checkout =cardcheck(cardlist);
                            if (checkout.Count != 0)
                            {
                                if (borrow_checkout == false)
                                {
                                    warning = true;
                                    //检测门异常数据
                                    warningnum += checkout.Count;
                                    //报警
                                    Buzzer((byte)1, (byte)1, (byte)10, (byte)1, (byte)1, (byte)10);
                                    //存储数据
                                    if (b1 != true && b2 != true)
                                    {
                                        savedate();
                                    }
                                    cardlist.Clear();
                                    for (int i = 0; i < checkout.Count; i++)
                                    {
                                        tstr += checkout[i] + " ";
                                    }
                                    //屏幕警告显示
                                    killtxt = "Warning";
                                    startkill();
                                    MessageBox.Show("请注意，编号为" + tstr + "的档案非法出库！请检查...", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                                else
                                {
                                    borrow_checkout_r = false;
                                }
                            }
                            else
                            {
                                warning = false;
                                //提示音  
                                Buzzer((byte)1, (byte)1, (byte)1, (byte)1, (byte)1, (byte)1);
                                //存储数据
                                if (b1 != true && b2 != true)
                                {
                                    savedate();
                                }
                                cardlist.Clear();
                            }                            
                            
                        }
                        comin = false;
                        goout = false;
                        return;
                    }
                    return;
                }
                byte[] daw = new byte[MsgLength - 7];          //除去前面6个字节的时间和1个字节的长度
                Array.Copy(Msg, 7, daw, 0, MsgLength - 7);
                string temps = ByteArrayToHexString(daw);
                int m = 0;
                for (int CardIndex = 0; CardIndex < CardNum; CardIndex++)
                {
                    int EPClen = daw[m];
                    string sEPC = temps.Substring(m * 2 + 2, EPClen * 2);
                    m = m + EPClen + 1;
                    if (sEPC.Length != EPClen * 2)
                        return;

                    //新入id识别
                    if (cardlist.Contains(sEPC) == false)
                    {
                        cardlist.Add(sEPC);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else if ((fCmdRet == 0) && (MsgType == 1))
            {
                InFlag = Convert.ToByte(Msg[0]);
                switch (InFlag)
                {
                    //正向判断
                    case 0:
                        comin = true;
                        break;
                    //反向判断
                    case 1:
                        goout = true;
                        break;
                }
            }
            Device.Acknowledge(ref ControllerAdr, PortHandle);

        }


        //通道门鸣响提示
        public void Buzzer(byte BuzzerOnTime, byte BuzzerOffTime, byte BuzzerActTimes, byte LEDOnTime, byte LEDOffTime, byte LEDFlashTimes)
        {
            //BuzzerOnTime = (byte)1;  //鸣叫时间1*100ms
            //BuzzerOffTime = (byte)1; //静默时间1*100ms
            //BuzzerActTimes = (byte)10; //鸣叫次数10次
            //LEDOnTime = (byte)1;  //点亮时间1*100ms
            //LEDOffTime = (byte)1; //熄灭时间1*100ms
            //LEDFlashTimes = (byte)10; //闪烁次数10次
            fCmdRet = Device.BuzzerAndLEDControl(ref ControllerAdr, BuzzerOnTime, BuzzerOffTime, BuzzerActTimes, LEDOnTime, LEDOffTime, LEDFlashTimes, ref IRStatus, PortHandle);
        }

        //标签校验，防止非法出库
        public List<string> cardcheck(List<string> cardlist)
        {
            List<string> checkoutlist = new List<string>();    //check结果标签
            List<string> chenstr1;   //数据解析结果
            try
            {
                for (int i = 0; i < cardlist.Count; i++)
                {
                    chenstr1=chenking(cardlist[i]);
                    if (chenstr1[0] !="1")
                    {
                        checkoutlist.Add(cardlist[i]);
                    }
                }                
            }
            catch { return checkoutlist; }
            return checkoutlist;
        }
        #endregion

        #region  盘点机数据源

        //在库盘点
        private void button4_Click_1(object sender, EventArgs e)
        {

            try
            {
                this.tabPage4.Parent = tabControl1;
                this.tabPage5.Parent = null;
                return;
            }
            catch
            {
                MessageBox.Show("请检查盘点机网口连接！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            }
        }


        //开始盘点按钮（past）
        private void button14_Click(object sender, EventArgs e)
        {
            checknote();
        }

        //开始盘点
        private void button45_Click(object sender, EventArgs e)
        {
            if (button45.Text == "盘 点")
            {
                try
                {
                    int pernum = 0,outnum=0;
                    mysqlinitnossh("192.168.1.26");
                    load_pandian("debug", "_pdj_data");   //获取数据
                    textBox23.Text = mysql3.Tables[0].Rows.Count.ToString();
                    pandiansum = textBox23.Text;   //盘点数量全局变量                  
                    button45.Text = "查 看";
                    MessageBox.Show("盘点数据获取成功！", "Success", MessageBoxButtons.OKCancel, MessageBoxIcon.None);
                    for (int i = 0; i < mysql.Tables[0].Rows.Count; i++)
                    {
                        if (mysql.Tables[0].Rows[i][2].ToString() == "0")
                        {
                            outnum++;
                            continue;
                        }
                        for (int j = 0; j < mysql3.Tables[0].Rows.Count; j++)
                        {
                            if (mysql3.Tables[0].Rows[j][1].ToString() == mysql.Tables[0].Rows[i][1].ToString())
                            {
                                pernum++;
                            }
                        }
                    }
                    textBox22.Text = ((mysql3.Tables[0].Rows.Count-pernum)+(mysql.Tables[0].Rows.Count-pernum-outnum)).ToString();
                    pandinawarning = textBox22.Text;  //盘点异常数量全局变量
                    return;
                }
                catch
                {
                    MessageBox.Show("盘点失败，请检查网线连接！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
            }
            if (button45.Text == "查 看")
            {
                checknote();
                button45.Text = "盘 点";
            }
        }

        //开始盘点2019.2
        public bool check_pand()
        {
            bool flg = false;
            try
            {
                int pernum = 0, outnum = 0;
                mysqlinitnossh("192.168.1.26");
                load_pandian_borrow("debug", "_pdj_data");   //获取数据
                DialogResult rusult = MessageBox.Show("核对扫描数据获取成功！","扫描数据", MessageBoxButtons.OKCancel, MessageBoxIcon.None);
                if (rusult == DialogResult.OK)
                {
                    flg = true;
                    return flg;
                }
                else
                {
                    return flg;
                }  
            }
            catch
            {
                MessageBox.Show("核对扫描数据获取失败，请检查网线连接！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return flg;
            }
            
        }

        /// <summary>
        /// 盘点结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button19_Click(object sender, EventArgs e)
        {
            this.tabPage4.Parent = null;
            this.tabPage5.Parent = tabControl1;
            progressBar6.Value = 0;
            dataGridView4.DataSource = null;
            return;
        }

        public void checknote()
        {
            try
            {                        
                form_pandain f10 = new form_pandain(mysql3,mysql);
                f10.Text = "盘点结果";
                f10.ShowDialog();
            }
            catch
            { }
        }

        /// <summary>
        /// 盘点异常结果查看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button42_Click(object sender, EventArgs e)
        {
            try
            {
                form_pandain f10 = new form_pandain(mysql3, mysql);
                f10.Text = "盘点结果";
                f10.ShowDialog();
            }
            catch
            { }
        }

        #endregion

        #region    档案转递数量查询
        private void button50_Click(object sender, EventArgs e)
        {
            mysql_status = load_mysql("note19", "transfernote", "transfer");
            form_datacheck f9 = new form_datacheck(this, mysql_status, false, "transfer", 0);
            f9.Text = "档案转递数量";
            f9.ShowDialog();
        }
        #endregion

        #region 参数设置
        public string temph, weth, templ, wetl, time1, time2,controlsum,persumh,borrowday,warningday,pandiansum="0",pandinawarning="0";
        private void button24_Click(object sender, EventArgs e)
        {
            //填写对应信息
            form_settingpasswd f4 = new form_settingpasswd();
            f4.ShowDialog();
            if (f4.textBox1.Text == "")
            { return; }
            temph = f4.temph;
            templ = f4.templ;
            weth = f4.weth;
            wetl = f4.wetl;
            time1 = f4.time1;
            time2 = f4.time2;
            controlsum = f4.controlsum;
            persumh = f4.persumh;
            borrowday = f4.borrowday;
            warningday = f4.warningday;            
            //数据库更新查询
            chenkcount();
            refresh_checkdata();
        }
        #endregion

        #region 紧急开锁
        private void button46_Click(object sender, EventArgs e)
        {
            //form_caozuoyuanpasswd f6 = new form_caozuoyuanpasswd(this,(int.Parse(controlsum)+1),1);
            //f6.ShowDialog();
            form_shizhurenpasswd szr = new form_shizhurenpasswd(this, (int.Parse(controlsum) + 1),null );
            szr.Text = "紧急开锁认证-室主任权限";
            szr.ShowDialog();
        }
        #endregion

        #region  查看设备状态
        private void button26_Click(object sender, EventArgs e)
        {
            while (true)
            {
                form_devicestatus f7 = new form_devicestatus(dataserver, monitorpc, gatedoorstatus, controlboard, RFIDread, pdjsytatu, qrdevicestatu, printstatu);
                f7.ShowDialog();
                //刷新设备
                if (f7.refresh == true)
                { continue; }
                //关闭设备
                if (f7.close == true)
                { break; }
            }
        }
        #endregion

        #region  查询参数
        private void button25_Click(object sender, EventArgs e)
        {
            form_settingview f8 = new form_settingview();
            f8.ShowDialog();
        }
        #endregion

        #region  档案盒状态数据功能函数
        /// <summary>
        /// 现存档案总数量查看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button38_Click(object sender, EventArgs e)
        {
            mysql_status=load_mysql("store", "tablename","nowstore");
            form_datacheck f9 = new form_datacheck(this, mysql_status,false,"nowstore",0);
            f9.Text = "现存档案总数量查询"; 
            f9.ShowDialog();
        }

        /// <summary>
        /// 取走档案盒查看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button39_Click(object sender, EventArgs e)
        {
            mysql_status = load_mysql("store", "tablename", "borrow");
            form_datacheck f9 = new form_datacheck(this, mysql_status, false, "borrow",0);
            f9.Text = "借阅档案盒数量查询";
            f9.ShowDialog();
        }

        /// <summary>
        /// 应还档案盒查看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button33_Click(object sender, EventArgs e)
        {
            mysql_status = load_mysql("store", "tablename", "borrow");
            form_datacheck f9 = new form_datacheck(this, mysql_status, false, "should_return",3);
            f9.Text = "应还档案盒数量查询";
            f9.ShowDialog();
        }

        /// <summary>
        /// 借还档案盒查看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button35_Click(object sender, EventArgs e)
        {
            mysql_borrow = load_mysql("store", "outtable", "borrow");
            mysql_return = load_mysql("store", "intable", "return");
            form_borandret bandt = new form_borandret(mysql_borrow,mysql_return,"borrowandreturn");
            bandt.Text = "借还档案盒数量";
            bandt.ShowDialog();
        }

        /// <summary>
        /// 档案盒新增数量查看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button40_Click(object sender, EventArgs e)
        {
            mysql_status = load_mysql("store", "tablename", "new");
            form_datacheck f9 = new form_datacheck(this, mysql_status, false, "new", 0);
            f9.Text = "档案盒新增数量";
            f9.ShowDialog();
        }

        /// <summary>
        /// 检测门入门数量查看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button43_Click(object sender, EventArgs e)
        {
            mysql_status = load_mysql("gatenote", "gatenote1", "gate_in");
            form_datacheck f9 = new form_datacheck(this, mysql_status, false, "gate_in", 0);
            f9.Text = "检测门入门数量";
            f9.ShowDialog();
        }

        /// <summary>
        /// 检测门出门数量查看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button44_Click(object sender, EventArgs e)
        {
            mysql_status = load_mysql("gatenote", "gatenote1", "gate_out");
            form_datacheck f9 = new form_datacheck(this, mysql_status, false, "gate_out", 0);
            f9.Text = "检测门出门数量";
            f9.ShowDialog();
        }

        /// <summary>
        /// 出入异常数量查看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button41_Click(object sender, EventArgs e)
        {
            mysql_status = load_mysql("gatenote", "gatenote1", "gate_warning");
            form_datacheck f9 = new form_datacheck(this, mysql_status, false, "gate_warning", 0);
            f9.Text = "出入异常数量";
            f9.ShowDialog();
        }
        #endregion

        #region 调试用特定开锁
        /// <summary>
        /// 特定开锁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            t.Stop();
            //打开特定门test
            if (textBox7.Text == "" || textBox8.Text == "")
            {
                MessageBox.Show("请填写柜体编号", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            firtdoor f1 = new firtdoor(0, null);
            byte[] testopen = { 0x02, 0x06, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00 };  //读门的状态
            testopen[0] = Convert.ToByte(textBox7.Text);
            //box换算
            int boxx = int.Parse(textBox8.Text);
            //ch1-ch16
            if (boxx < 17)
            {
                testopen[3] = 0x01;   //柜号,CH1-CH16
                if (boxx < 9)
                {
                    testopen[4] = 0x00;
                    testopen[5] = (byte)(0x01 << (boxx - 1));
                }
                else
                {
                    testopen[5] = 0x00;
                    testopen[4] = (byte)(0x01 << (boxx - 9));
                }
            }
            //ch17-ch30
            else
            {
                testopen[3] = 0x02;   //柜号,CH17-CH30
                if (boxx < 25)
                {
                    testopen[4] = 0x00;
                    testopen[5] = (byte)(0x01 << (boxx - 17));
                }
                else
                {
                    testopen[5] = 0x00;
                    testopen[4] = (byte)(0x01 << (boxx - 25));
                }
            }
            testopen = f1.crce(testopen);
            serialport1 port1 = new serialport1();
            port1.senddata1(testopen, portlist1, portlist1[0]);
            t.Start();
        }
        #endregion

        #region DataError处理
        /// <summary>
        /// DataError处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataerror_ditel(object sender, DataGridViewDataErrorEventArgs e)
        {
        }
        #endregion

        #region 查看可用柜体
        /// <summary>
        /// 可用柜体查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button47_Click(object sender, EventArgs e)
        {
            mysql_status = load_mysql("store", "tablename", "nowstore");//查现存档案数据
            form_waituseid wause = new form_waituseid(int.Parse(controlsum), mysql_status);
            wause.ShowDialog();
        }
        #endregion

        #region   便捷退出软件
        /// <summary>
        /// 软件便捷退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button21_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

    }

    #endregion

    #region Class 类define

        #region 登录类

    //密码登录类
    public class passwdtext
    {
        public string logFile;
        private StreamWriter writer;
        private FileStream fileStream = null;

        public void Log(string fileName)
        {
            logFile = fileName;
            CreateDirectory(logFile);
        }

        public void log(string info)
        {

            try
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(logFile);
                if (!fileInfo.Exists)      //判断passwdtxt文件的存在，不存在则创建
                {
                    fileStream = fileInfo.Create();
                    writer = new StreamWriter(fileStream);
                }
                else
                {
                    fileStream = fileInfo.Open(FileMode.Append, FileAccess.Write);   //选择写入模式为添加新行并写入 
                    writer = new StreamWriter(fileStream);
                }
                writer.WriteLine(info);    //写入内容
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
        }
        public void CreateDirectory(string infoPath)
        {
            DirectoryInfo directoryInfo = Directory.GetParent(infoPath);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
        }

        //清空text内容
        public void cleartext(string textname)
        {
            string appdir = System.AppDomain.CurrentDomain.BaseDirectory + @textname;
            FileStream stream = File.Open(appdir, FileMode.OpenOrCreate, FileAccess.Write);
            stream.Seek(0, SeekOrigin.Begin);
            stream.SetLength(0);
            stream.Close();       //关闭文件流
        }

        public bool logincheck(string countname, string passwd)
        {
            StreamReader xiao_str1 = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + @"passwd.txt");
            string text = "**";
            while (text != null)
            {
                text = xiao_str1.ReadLine();
                if (countname == text)
                {
                    countname = "right";
                    continue;
                }
                if (countname == "right")
                {
                    if (passwd == text)
                    {
                        xiao_str1.Close();
                        return true;
                    }
                    else
                    {
                        xiao_str1.Close();
                        return false;
                    }

                }
            }
            xiao_str1.Close();
            return false;
        }

        /// <summary>
        /// 查询设置参数
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string logincheckset(string name)
        {
            StreamReader xiao_str1 = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + @"seting.txt");
            string text="";
            string text1="";
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

        public List<string> logincheckset_contin(string name)
        {
            StreamReader xiao_str1 = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + @"passwd.txt");
            string text = "";
            List<string> text1 = new List<string>();
            while (text != null)
            {
                text = xiao_str1.ReadLine();
                if (text == "manager")
                {
                    xiao_str1.Close();
                    return text1;
                }
                if (text.Contains(name))
                {
                    text1.Add(text);
                    continue;
                } 
            }
            xiao_str1.Close();
            return text1;
        }
        public string logincheckset_lingdao(string name)
        {
            StreamReader xiao_str1 = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + @"guanlingdao_keywd.txt");
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

        /// <summary>
        /// 密码修改
        /// </summary>
        /// <param name="name"></param>
        /// <param name="passwd"></param>
        /// <returns></returns>
        public bool texthandle(string name, string passwd)
        {
            bool flagxiu = false;
            Log(AppDomain.CurrentDomain.BaseDirectory + @"passwd.txt");
            StreamReader xiao_str1 = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + @"passwd.txt");
            string text = "";
            string[] ary = null;
            int i = 0;
            var newpasswd = passwd.Split(new char[2] { ':',' ' });
            try
            {
                while (text != null)
                {
                    text = xiao_str1.ReadLine();
                    i++;
                    if (name == text)
                    {
                        ary = File.ReadAllLines(System.AppDomain.CurrentDomain.BaseDirectory + @"passwd.txt", Encoding.UTF8);   //读取txt文件所有内容
                        ary[i] = newpasswd[3];
                    }
                }
                xiao_str1.Close();
                int j = 0;
                cleartext("passwd.txt");
                while (j < i - 1)
                {
                    log(ary[j]);
                    j++;
                }
                flagxiu = true;
                return flagxiu;
            }
            catch
            {
                return flagxiu;
            }
        }


        /// <summary>
        /// 用户添加
        /// </summary>
        /// <param name="name"></param>
        /// <param name="passwd"></param>
        /// <returns></returns>
        public bool texthandle_add(string name, string passwd)
        {
            bool flagxiu = false;
            Log(AppDomain.CurrentDomain.BaseDirectory + @"passwd.txt");
            StreamReader xiao_str1 = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + @"passwd.txt");
            string text = "";
            string[] ary = new string[20];
            string[] ary1 = new string[4];
            int i = 0;
            var newname = name.Split(new char[2] { ':', ' ' });
            var newpasswd = passwd.Split(new char[2] { ':', ' ' });
            try
            {
                
                while (text != null)
                {
                    text = xiao_str1.ReadLine();
                    i++;
                    if (text == "manager")
                    {
                        ary1[0] = text;
                        text = xiao_str1.ReadLine();
                        ary1[1] = text;
                        xiao_str1.Close();
                        break;
                    }
                }
                ary = File.ReadAllLines(System.AppDomain.CurrentDomain.BaseDirectory + @"passwd.txt", Encoding.UTF8);   //读取txt文件所有内容
                ary[i - 1] = newname[3].ToString();
                ary[i++] = newpasswd[3].ToString();
                List<string> temp = ary.ToList();
                temp.Add(ary1[0]);
                temp.Add(ary1[1]);
                ary = temp.ToArray();
                int j = 0;
                cleartext("passwd.txt");
                while (j < i + 2)
                {
                    log(ary[j]);
                    j++;
                }
                flagxiu = true;
                return flagxiu;
            }
            catch
            {
                return flagxiu;
            }
        }


        /// <summary>
        /// 用户删除
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool texthandle_cut(string name)
        {
            bool flagxiu = false;
            Log(AppDomain.CurrentDomain.BaseDirectory + @"passwd.txt");
            StreamReader xiao_str1 = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + @"passwd.txt");
            string text = "";
            string[] ary = new string[20];
            string[] ary1 = new string[4];
            int i = 0,m=0;
            try
            {

                while (text != null)
                {
                    text = xiao_str1.ReadLine();
                    i++;
                    if (text == name)
                    {
                        m = i;
                        continue;
                    }
                    if (text == "manager")
                    {
                        xiao_str1.Close();
                        break;
                    }
                }
                ary = File.ReadAllLines(System.AppDomain.CurrentDomain.BaseDirectory + @"passwd.txt", Encoding.UTF8);   //读取txt文件所有内容                
                List<string> temp = ary.ToList();
                temp.RemoveAt(m--);
                temp.RemoveAt(m);
                ary = temp.ToArray();
                int j = 0;
                cleartext("passwd.txt");
                while (j < i-1)
                {
                    log(ary[j]);
                    j++;
                }
                flagxiu = true;
                return flagxiu;
            }
            catch
            {
                return flagxiu;
            }
        }

        /// <summary>
        /// 设置参数修改
        /// </summary>
        /// <param name="name"></param>
        /// <param name="passwd"></param>
        /// <returns></returns>
        public bool texthandl_seting(string name, string passwd)
        {
            bool flagxiu = false;
            Log(AppDomain.CurrentDomain.BaseDirectory + @"seting.txt");
            StreamReader xiao_str1 = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + @"seting.txt");
            string text = "";
            string[] ary = null;
            int i = 0;
            var newpasswd = passwd.Split(new char[2] { ':', ' ' });
            try
            {
                while (text != null)
                {
                    text = xiao_str1.ReadLine();
                    i++;
                    if (name == text)
                    {
                        if (newpasswd.Count() > 2)
                        {
                            ary = File.ReadAllLines(System.AppDomain.CurrentDomain.BaseDirectory + @"seting.txt", Encoding.UTF8);   //读取txt文件所有内容
                            ary[i] = newpasswd[3];
                        }
                        if (newpasswd.Count() ==1)
                        {
                            ary = File.ReadAllLines(System.AppDomain.CurrentDomain.BaseDirectory + @"seting.txt", Encoding.UTF8);   //读取txt文件所有内容
                            ary[i] = passwd;
                        }
                    }
                }
                xiao_str1.Close();
                int j = 0;
                cleartext("seting.txt");
                while (j < i - 1)
                {
                    log(ary[j]);
                    j++;
                }
                flagxiu = true;
                return flagxiu;
            }
            catch
            {
                return flagxiu;
            }
        }

        /// <summary>
        /// 口令写入
        /// </summary>
        /// <param name="name"></param>
        /// <param name="passwd"></param>
        /// <returns></returns>
        public bool texthandl_ldkeywd(string name,string key)
        {
            bool flagxiu = false;
            Log(AppDomain.CurrentDomain.BaseDirectory + @"guanlingdao_keywd.txt");
            StreamReader xiao_str1 = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + @"guanlingdao_keywd.txt");
            string text = "";
            string[] ary = null;
            int i = 0;
            try
            {
                while (text != null)
                {
                    text = xiao_str1.ReadLine();
                    i++;
                    if (name == text)
                    {
                            ary = File.ReadAllLines(System.AppDomain.CurrentDomain.BaseDirectory + @"guanlingdao_keywd.txt", Encoding.UTF8);   //读取txt文件所有内容
                            ary[i] = key;
                    }
                }
                xiao_str1.Close();
                int j = 0;
                cleartext("guanlingdao_keywd.txt");
                while (j < i - 1)
                {
                    log(ary[j]);
                    j++;
                }
                flagxiu = true;
                return flagxiu;
            }
            catch
            {
                return flagxiu;
            }
        }


    }
    #endregion 

        #region  串口通信类

    //串口通信类
    public class serialport
    {
        public List<string> serialportcheck()
        {
            bool flag = false;
            List<string> portstr = new List<string>();
            SerialPort sp = new SerialPort();
            sp.BaudRate = 115200;
            sp.StopBits = System.IO.Ports.StopBits.One;
            sp.Parity = System.IO.Ports.Parity.None;
            sp.DataBits = 8;

            for (int i = 3; i < 20; i++)
            {
                sp.PortName = "COM" + i.ToString();
                try
                {
                    sp.Open();
                    sp.Close();
                    portstr.Add("COM" + i.ToString());
                }
                catch (Exception)
                {

                }
            }
            return portstr;
        }
    }
    #endregion

        #region  tcp通信类
    public class socketxiao
    {

        public static Socket socketlistener;
        public  Socket socketlistener1;
        public IPEndPoint clientipe1;
        int i = 0;
        public  string numlist1;
        public  string numlist2;
        public  string numlist3;
        public bool status=false;
        public int values=1;
        public Dictionary<string, Socket> client = new Dictionary<string, Socket> { };  //连接client信息存储,socket信息字典
        public firtdoor f1;
        public void refresh(firtdoor f11, List<string> st1, List<string> st2, List<string> st3)
        {
            //温度
            numlist1 = (((float.Parse(st1[0])-2.5) + (float.Parse(st1[1])-3.6)) / 2).ToString("#0.0");
            //湿度
            numlist2 = (((float.Parse(st2[0])+1.9) + (float.Parse(st2[1])+7)) / 2).ToString("#0.0");
            //浸水状态
            numlist3 = st3[0];
            f1 =f11;
        }


        //start listening
        public void socketinit(string preip, int port, int maxport)
        {

            IPAddress[] ips = Dns.GetHostAddresses("");
            for (i=0; i < ips.Count(); i++)
            {
                if (ips[i].ToString() == preip)
                {
                    break;
                }
            }
            socketlistener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketlistener.Bind(new IPEndPoint( ips[i], port));  //网口不对或脱落时报错
            socketlistener.Listen(maxport);
            Thread handlesocket = new Thread(new ThreadStart(handlesocket1));
            handlesocket.IsBackground = true;   //线程转为后台
            handlesocket.Start();
        }


        //listening link
        private void handlesocket1()
        {
            writelog log = new writelog();
            //log.Log(AppDomain.CurrentDomain.BaseDirectory + @"/log/Log.txt");

            while (true)
            {
                try
                {
                    Socket currocket = socketlistener.Accept();
                    Thread processthred = new Thread(new ParameterizedThreadStart(processthred1));
                    processthred.IsBackground = true;  //线程转为后台
                    processthred.Start(currocket);
                    clientipe1 = (IPEndPoint)currocket.RemoteEndPoint;   //client:ip+端口
                    if (currocket.Connected)
                    {
                        //log.log("link from " + clientipe1.ToString() + "\n");
                    }
                }
                catch
                {
                    break;
                }
            }
        }

        //dill the socket information
        public List<string> tidstrlist = new List<string>();
        public int innum = 0, innum1=0,warning=0;
        public void processthred1(object obj)
        {
            int flag_online = 0,county=0;
            writelog log = new writelog();
            writelog openlog = new writelog();
            //log.Log(AppDomain.CurrentDomain.BaseDirectory + @"/log/Log.txt");
            Socket currsocket = (Socket)obj;
            IPEndPoint clientipe2 = (IPEndPoint)currsocket.RemoteEndPoint;   //client:ip+端口
            //log.log("linking from " + clientipe2.ToString() + "\n");
            
            //连接ip终端列表
            try
            {
                if (!client.ContainsKey(clientipe2.ToString())) { client.Add(clientipe2.Address.ToString(), currsocket); }
                else { client.Add(clientipe2.Address.ToString(), currsocket); }
            }
            catch { }

            while (currsocket.Connected)
            {
                //log.Log(AppDomain.CurrentDomain.BaseDirectory + @"/log/Log.txt");
                //通道门连接标志
                if (clientipe2.Address.ToString() == "192.168.1.23")
                {
                    status = true;
                }

                try
                {
                    byte[] recvbytes = new byte[1048576];
                    int recbytes = 0;
                    recbytes = currsocket.Receive(recvbytes, recvbytes.Length, 0);
                    var contentstr = Encoding.UTF8.GetString(recvbytes, 0, recbytes);

                    var imgpath = contentstr.Split(new char[1] { '%' });    //字符分割

                    if (imgpath[1] == "read")
                    {
                        byte[] num_message;
                        if (f1.warning == true)
                        { warning = 1; }
                        else
                        { warning = 0; }
                        //tcp数据帧;
                        string str = "%" + numlist1 + "%" + numlist2 + "%" + numlist3 + "%" + f1.cominnum + "%" +f1.gooutnum + "%" +f1.warningnum + "%" +f1.borrownum + "%" +f1.borandretnum + "%" +f1.should_borrownum + "%" +f1.count + "%" +f1.controlsum + "%" + f1.pandiansum + "%" + f1.pandinawarning + "%" +warning+ "%" + f1.transfernum+"%"+"Imok";
                        //数据消息
                        num_message = Encoding.UTF8.GetBytes(str);
                        Thread.Sleep(50);
                        client[imgpath[2]].Send(num_message, num_message.Length, SocketFlags.None);

                    }
                    if (imgpath[1] == "readok")
                    {
                        //log.log("online replay" + clientipe.ToString() + "\n");
                        flag_online = 1;
                    }

                    //通道门模拟test
                    if (imgpath[1] == "I")
                    {
                        var tidstr = imgpath[2].Split(new char[1] { ',' });
                        innum1 = int.Parse(tidstr[3]);
                        innum = innum + innum1;

                        for (int i = 0; i < innum1; i++)
                        {
                            tidstrlist.Add(tidstr[4 + i]);
                        }
                    }
                    if (imgpath[1] == "O")
                    {
                        try
                        {
                            var tidstr1 = imgpath[2].Split(new char[1] { ',' });
                            List<string> tidstrlist1 = new List<string>();

                            if (int.Parse(tidstr1[3]) != 0)
                            {
                                for (int i = 0; i < int.Parse(tidstr1[3]); i++)
                                {
                                    for (int j = 0; j < innum; j++)
                                    {
                                        if (tidstr1[4 + i] == tidstrlist[j])
                                        {
                                            MessageBox.Show("请注意，编号为" + tidstrlist[j] + "的档案未归还！请检查...", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                }
                            }
                            tidstrlist.Clear();
                            innum = 0;
                        }
                        catch { }
                    }

                }
                catch (Exception ex)
                {
                    //log.log("error post request   ");
                    //log.log("获取数据异常！  \n");
                    //MessageBox.Show(ex.ToString());
                    client.Remove(clientipe2.Address.ToString());
                    if (clientipe2.Address.ToString() == "192.168.1.23")
                    {
                        f1.killtxt = "Warning";
                        f1.startkill();
                        MessageBox.Show("1号监控计算机连接断开！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    }
                    if (clientipe2.Address.ToString() == "192.168.1.23")
                    {
                        status = false;
                    }
                    break;
                }
            }
        }



        //close the socket
        public static void close()
        {
            socketlistener.Close();
        }

        public Socket c;
        public string paysult = "";
        int bytes = 0;
        string recvStr = "";
        byte[] recvBytes = new byte[1024];

        //send,tcp发送数据，并开启线程等待返回结果
        public string socketxiaokai(string encodstr, int who, string clientip, string method)
        {

            writelog log = new writelog();
            log.Log(AppDomain.CurrentDomain.BaseDirectory + @"/log/Log.txt");

            try
            {

                int port = 60020;
                string host = "120.78.137.138";

                IPAddress ip = IPAddress.Parse(host);
                IPEndPoint ipe = new IPEndPoint(ip, port); //把ip和端口转化为IPEndPoint实例  

                c = null;
                c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建一个Socket
                c.ReceiveTimeout = 3000;//通讯超时

                c.Connect(ipe);  //连接tcp   

                byte[] bs = Encoding.ASCII.GetBytes(encodstr);
                c.Send(bs, bs.Length, 0);//发送数据

                if (who == 1) { c.Close(); throw new Exception("...."); }

                Thread.Sleep(500);
                while (true)
                {
                    bytes = c.Receive(recvBytes, recvBytes.Length, 0);//接收返回数据 
                    recvStr = Encoding.ASCII.GetString(recvBytes, 0, bytes);
                    if (bytes != 0) { break; }
                }

                switch (method)
                {

                }
            }
            catch (SocketException)
            {
                log.log("socketxiaokai function error");
            }

            return recvStr;

        }
    }
    #endregion

        #region  mysql数据库连接类
    /// <summary>
    /// mysql类
    /// </summary>
    public class mysqlclass
    {
        //mysqlssh连接
        //MySqlConnection mysqlconn1 = new MySqlConnection(mysqlconn);  //创建sql连接对象
        //ySqlDataAdapter mysqldata = new MySqlDataAdapter();     //数据库执行命令方法,适合大数据量
        //MySqlDataReader tablename = null;   //数据库命令返回数据，适合小数据量
        public int conn = 0;
        public string mysqlconn;
        public MySqlConnection mysqlconn1 = new MySqlConnection();
        public MySqlDataAdapter mysqldata = new MySqlDataAdapter();     //数据库执行命令方法,适合大数据量
        public SshClient client;
        public MySqlConnection mysqlsshconn(string text3)
        {
            firtdoor f1 = new firtdoor(0, mysqlconn1);
            f1.progressBar1.Value = 10;
            if (text3 == "")
            {
                MessageBox.Show("请输入ip地址！", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            f1.progressBar1.Value = 20;
            string sshhost = text3;  //"192.168.21.21"
            int sshport = 22;
            string sshuser = "root";
            string sshpassword = "1234";
            string ip = "127.0.0.2";  //映射地址
            string mysqlhost = text3;
            uint mysqlport = 3306;
            //local-connect
            mysqlconn = "Database=;Data source=localhost;Port=3306;User Id=root;password=1234;CharSet=utf8;Allow Zero Datetime=True";
            //mysqlconn = "Database=;Data source=127.0.0.2;Port=3306;User id=xiaomysql;Password=1234;CharSet=utf8;Allow Zero Datetime=True";
            //mysqlconn = "server=127.0.0.2;User Id=root;password=1234;Database=db_test";  //Allow Zero Datetime=True解决time数据类型的转换问题
            f1.progressBar1.Value = 40;

            PasswordConnectionInfo connectinfo = new PasswordConnectionInfo(sshhost, sshport, sshuser, sshpassword);
            connectinfo.Timeout = TimeSpan.FromSeconds(30);   //ssh连接超时

            //using (var client = new SshClient(connectinfo))    //ssh连接
            client = new SshClient(connectinfo);
            //{
            //try
            //{
            //    client.Connect();
            //    if (!client.IsConnected)
            //    {
            //        MessageBox.Show("SSH链接失败！", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        f1.progressBar1.Value = 0;
            //        return null;
            //    }
            //    f1.progressBar1.Value = 60;
            //    //映射本地端口（？）
            //    var portfwd = new ForwardedPortLocal(ip, mysqlport, mysqlhost, mysqlport); //映射到本地的ip及端口3306
            //    client.AddForwardedPort(portfwd);
            //    portfwd.Start();
            //    if (!client.IsConnected)
            //    {
            //        MessageBox.Show("端口映射错误！", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        f1.progressBar1.Value = 0;
            //        return null;
            //    }

                mysqlconn1.ConnectionString = mysqlconn;
                try
                {
                    mysqlconn1.Open();
                    conn = 1;
                    //MessageBox.Show("数据库链接成功！", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.None);
                }
                catch
                {
                    MessageBox.Show("连接数据库失败！", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    f1.progressBar1.Value = 0;
                    return null;
                }
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show("链接异常", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return null;
            //}
            //}
            mysqlconn1.Dispose();
            f1.progressBar1.Value = 80;
            return mysqlconn1;
        }

        public MySqlConnection mysqlconn2 = new MySqlConnection();
        public MySqlConnection mysqlconnn(string text3)
        {
            firtdoor f1 = new firtdoor(0, mysqlconn1);
            f1.progressBar6.Value = 10;
            if (text3 == "")
            {
                MessageBox.Show("请输入ip地址！", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            f1.progressBar1.Value = 20;
            string sshhost = text3;  //"192.168.21.21"
            string ip = "127.0.0.2";  //映射地址
            string mysqlhost = text3;
            uint mysqlport = 3306;
            mysqlconn = "Database=;Data source=192.168.1.26;Port=3306;User Id=root;password=654321;CharSet=utf8;Allow Zero Datetime=True";
            f1.progressBar6.Value = 40;

            try
            {


                mysqlconn2.ConnectionString = mysqlconn;
                f1.progressBar6.Value = 70;
                try
                {
                    mysqlconn2.Open();
                    conn = 1;
                    //MessageBox.Show("数据库链接成功！", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.None);
                }
                catch
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
            //}
            mysqlconn2.Dispose();
            f1.progressBar6.Value = 80;
            return mysqlconn2;
        }

        /// <summary>
        /// 读取返回SQL1
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<string> mysqlshow(string sql)
        {
            List<string> lis_name = new List<string>();
            MySqlDataReader tablename = null;   //数据库命令返回数据，适合小数据量
            MySqlCommand mysqlcom_showtable = new MySqlCommand(sql, mysqlconn1);  //sql-show
            try
            {
                mysqlconn1.Open();
                tablename = mysqlcom_showtable.ExecuteReader();
                if (tablename.HasRows)   //判断返回表name是否为空
                {
                    string t;
                    while (tablename.Read())
                    {
                        t = tablename.GetString(0);
                        lis_name.Add(t);
                    }
                    tablename.Close(); //关闭读数据库返回命令
                }
                else { MessageBox.Show("数据库无任何表！", "error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                mysqlcom_showtable.Dispose();  //关闭数据库命令
                mysqlconn1.Dispose();
                return lis_name;
            }
            catch { return lis_name; }

        }

        /// <summary>
        /// 读取返回SQL2
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<string> mysqlshow2(string sql)
        {
            List<string> lis_name = new List<string>();
            MySqlDataReader tablename = null;   //数据库命令返回数据，适合小数据量
            MySqlCommand mysqlcom_showtable = new MySqlCommand(sql, mysqlconn2);  //sql-show
            try
            {
                mysqlconn2.Open();
                tablename = mysqlcom_showtable.ExecuteReader();
                if (tablename.HasRows)   //判断返回表name是否为空
                {
                    string t;
                    while (tablename.Read())
                    {
                        t = tablename.GetString(0);
                        lis_name.Add(t);
                    }
                    tablename.Close(); //关闭读数据库返回命令
                }
                else { MessageBox.Show("数据库无任何表！", "error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                mysqlcom_showtable.Dispose();  //关闭数据库命令
                mysqlconn2.Dispose();
                return lis_name;
            }
            catch { return lis_name; }

        }


        /// <summary>
        /// mysql select命令
        /// </summary>
        /// <param name="mysqlselect"></param>
        /// <returns></returns>
        public DataSet mysqlselectcom(string mysqlselect)
        {
            MySqlCommand mysqlcom_select = new MySqlCommand(mysqlselect, mysqlconn1);  //sql-select命令
            DataSet mysql = new DataSet();
            try
            {
                mysqlconn1.Open();
                if (conn == 1)
                {
                    mysqldata.SelectCommand = mysqlcom_select;
                    mysqldata.Fill(mysql);
                    mysqlcom_select.Dispose();
                }
                else
                {
                    MessageBox.Show("数据库链接中断！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                mysqlconn1.Dispose();
                return mysql;
            }
            catch { return mysql; }
        }

        //mysql select命令2
        public DataSet mysqlselectcom2(string mysqlselect)
        {
            MySqlCommand mysqlcom_select = new MySqlCommand(mysqlselect, mysqlconn2);  //sql-select命令
            DataSet mysql = new DataSet();
            try
            {
                mysqlconn2.Open();
                if (conn == 1)
                {
                    //mysqldata.SelectCommand.Connection = mysqlconn1;
                    mysqldata.SelectCommand = mysqlcom_select;
                    mysqldata.Fill(mysql);
                    mysqlcom_select.Dispose();
                }
                else
                {
                    MessageBox.Show("数据库链接中断！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                mysqlconn2.Dispose();
                return mysql;
            }
            catch { return mysql; }
        }

        //mysql 保存更新数据命令
        public bool mysqlupdatecomall(string mysqlcom, DataSet table)
        {
            bool file = true;
            List<string> comlist = new List<string>();
            StringBuilder com = new StringBuilder();
            MySqlCommand mysqlcom_save = new MySqlCommand();  //sql-show  


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
                        com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[i][j] + "\" ");
                    }
                }
                com.Append("where id=" + table.Tables[0].Rows[i][0]);
                comlist.Add(com.ToString());
            }

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

        /// <summary>
        /// 储存数据
        /// </summary>
        /// <param name="mysqlcom"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public bool mysqlupdatecomarow(string mysqlcom, DataSet table)
        {
            bool file = true;
            List<string> comlist = new List<string>();
            StringBuilder com = new StringBuilder();
            MySqlCommand mysqlcom_save = new MySqlCommand();  //sql-show           
       
            for (int i = 0; i <= table.Tables[0].Rows.Count-2; i++)
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
            //com.Append("where id=" + table.Tables[0].Rows[table.Tables[0].Rows.Count - 2][0]);
            

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

        /// <summary>
        /// 批量入库存储数据
        /// </summary>
        /// <param name="mysqlcom"></param>
        /// <param name="table"></param>
        /// <param name="tt"></param>
        /// <returns></returns>
        public bool mysqlupdatecomarow1(string mysqlcom, DataSet table,int tt)
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
                    com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[table.Tables[0].Rows.Count - 2-tt][j] + "\",");
                }
                else
                {
                    com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[table.Tables[0].Rows.Count - 2-tt][j] + "\"");
                }
            }
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


        /// <summary>
        /// 通道门储存数据（新建表）
        /// </summary>
        /// <param name="mysqlcom"></param>
        /// <param name="table"></param>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public bool mysqlsavecom2(string mysqlcom, DataSet table, string tablename)
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
                MessageBox.Show("00002！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return file;
        }


        /// <summary>
        /// 存储通道门数据
        /// </summary>
        /// <param name="mysqlcom"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public bool mysqlupdatecomarow2(string mysqlcom, DataSet table)
        {
            bool file = true;
            List<string> comlist = new List<string>();
            StringBuilder com = new StringBuilder();
            MySqlCommand mysqlcom_save = new MySqlCommand();  //sql-show           

            for (int i = 0; i <= table.Tables[0].Rows.Count -1; i++)
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


        /// <summary>
        /// 批量入库创建及存储数据
        /// </summary>
        /// <param name="mysqlcom"></param>
        /// <param name="table"></param>
        /// <param name="tablename"></param>
        /// <param name="ttt"></param>
        /// <returns></returns>
        public bool mysqlsavecom1(string mysqlcom, DataSet table, string tablename,int ttt)
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

            com.Clear();
            com.Append(mysqlcom);
            for (int j = 0; j < table.Tables[0].Columns.Count; j++)
            {
                if (j != table.Tables[0].Columns.Count - 1)
                {
                    com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[table.Tables[0].Rows.Count - 2 - ttt][j] + "\",");
                }
                else
                {
                    com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[table.Tables[0].Rows.Count - 2 - ttt][j] + "\"");
                }
            }

            comlist.Add(com.ToString());
            try
            {
                //int.Parse(i)//string转int
                mysqlconn1.Open();
                try
                {
                    for (int i = 0; i < comlist.Count() ; i++)
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

        /// <summary>
        /// 普通mysql命令1
        /// </summary>
        /// <param name="sql"></param>
        public void mysqlcom(string sql)
        {

            MySqlDataReader tablename = null;   //数据库命令返回数据，适合小数据量
            MySqlCommand mysqlcom_showtable = new MySqlCommand(sql, mysqlconn1);  //sql-show
            try
            {
                if (mysqlconn1.State.ToString() == "Closed")
                {
                    mysqlconn1.Open();
                }
                tablename = mysqlcom_showtable.ExecuteReader();
                mysqlcom_showtable.Dispose();  //关闭数据库命令
                mysqlconn1.Dispose();

            }
            catch
            {
                //if (mysqlconn1.State.ToString() == "Open")
                //{
                //    mysqlconn1.Dispose();
                //}
                //MessageBox.Show("数据库链接中断！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
            }

        }

        /// <summary>
        /// 普通mysql命令2
        /// </summary>
        /// <param name="sql"></param>
        public void mysqlcom2(string sql)
        {

            MySqlDataReader tablename = null;   //数据库命令返回数据，适合小数据量
            MySqlCommand mysqlcom_showtable = new MySqlCommand(sql, mysqlconn2);  //sql-show
            try
            {
                mysqlconn2.Open();
                tablename = mysqlcom_showtable.ExecuteReader();
                mysqlcom_showtable.Dispose();  //关闭数据库命令
                mysqlconn2.Dispose();

            }
            catch { MessageBox.Show("数据库链接中断！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

        }
    }
    #endregion

        #region  Access数据库类

    //ACCESS数据库类
    public class AccessDbclass
    {
        public OleDbConnection conn;    //
        public string connstring;      //连接字符串

        //构造函数
        public AccessDbclass(string dbpath)
        {
            connstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + dbpath + ";Persist Security Info=False;";   //存在版本问题，Provider=Microsoft.Jet.OLEDB.4.0安装问题在成程序错误

            //connstring = "Provider = Microsoft.Jet.OLEDB.4.0;Data Source=" + dbpath + ";Persist Security Info=False;"; 
            conn = new OleDbConnection(connstring);
            conn.Open();
        }

        //打开数据源
        public OleDbConnection dbconn()
        {
            conn.Open();
            return conn;
        }

        //关闭数据源
        public void close()
        {
            conn.Close();    //关闭conn
            conn.Dispose();  //释放conn对象，释放内存，重新建立须new重定义
        }

        //数据库基本操作，直接作为dataGridView控件的数据源
        public DataTable selettodatatable(string sql)
        {
            OleDbDataAdapter adapter = new OleDbDataAdapter();  //dataset与数据源连接，使用 Fill 将数据从数据源加载到 DataSet 中，并使用 Update 将 DataSet 中所作的更改发回数据源
            OleDbCommand command = new OleDbCommand(sql, conn);  //SQL语句，存储过程？
            adapter.SelectCommand = command;
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        //创建数据集新表
        public void selecttodataset(string sql)
        {
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            OleDbCommand command = new OleDbCommand(sql, conn);
            command.ExecuteNonQuery();   //执行SQL语句   
        }

        //写入数据
        public void writetable(string sql)
        {
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            OleDbCommand command = new OleDbCommand(sql, conn);
            command.ExecuteNonQuery();   //执行SQL语句
        }

        //新建表
        public string[] buildtable(string tablename, string sheet_name1)
        {
            // Form1 f1 = new Form1();
            int tableid = 0;
            //List<string> listring = new List<string>(4096); //数组定义
            string[] listring = new string[200];
            bool b = false, a = true;

            var sheet_name = sheet_name1.Split('$');

            while (a)
            {
                try
                {
                    string sql = "select * from " + tablename + sheet_name[0] + tableid;
                    selettodatatable(sql);
                    //listring[tableid] = tablename + tableid;
                    listring[tableid] = tablename + sheet_name[0] + tableid;


                }
                catch     //异常捕获
                {
                    b = true;
                }
                if (b == true) { a = false; }
                else { tableid++; b = false; }
            }

            //listring[tableid] = tablename + tableid;
            listring[tableid] = tablename + sheet_name[0] + tableid;


            //连接数据源建表
            string sql1 = "create table " + tablename + sheet_name[0] + tableid + " ([ID] int primary key,[名称] string(30),[规格] string(40),[单价] string(10),[出厂日期] string(30),[单位] string(10),[数量] int,[质保期] string(30),[生产厂家] string(40),[分类I] string(30),[分类II] string(30),[领用记录] string(60),[拼音码] string(30),[条码] string(30),[入库员编号] string(30),[入库时间] string(30),[保存方式] string(40),[备注信息] string(80))";
            selecttodataset(sql1);
            //close();
            //tablename = tablename + tableid;
            return listring;
        }

        //模板建表
        public string buildtable_ex(string tablename, string zidaunlist)
        {
            //List<string> listring = new List<string>(4096); //数组定义
            //string[] listring = new string[200];
            string listring = "";
            string sqlpart2 = "";
            string sql = "";
            bool b = false, a = true;
            int na = 0;

            var tabtitle = zidaunlist.Split('，');   //字段1，字段2，字段3，字段4，字段5，字段6分解

            while (a)
            {
                try
                {
                    sql = "select * from " + tablename;
                    selettodatatable(sql);
                    na = 1;   //表单存在操作
                    a = false;

                }
                catch     //通过异常捕获，创建序列表
                {
                    b = true;
                }
                if (b == true) { a = false; }
            }

            listring = tablename;   //表名

            for (int i = 0; i < tabtitle.Count(); i++)
            {
                sqlpart2 += ",[" + tabtitle[i].ToString() + "] string(50)";
            }
            if (na == 1)   //表单存在时，删除原表单
            {
                sql = "drop table " + tablename;  //删除表
                selecttodataset(sql);  //执行sql命令
            }

            //连接数据源建表
            sql = "create table " + tablename + " ([ID] int primary key" + sqlpart2 + ")";
            selecttodataset(sql);


            return listring;

        }

        //返回数据库表名
        public string[] tabnamesql()
        {
            DataTable shematab = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            string[] strTableNames = new string[shematab.Rows.Count];
            for (int k = 0; k < shematab.Rows.Count; k++)
            {
                strTableNames[k] = shematab.Rows[k]["TABLE_NAME"].ToString();
            }
            return strTableNames;
            //DataTable columtab = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns,new object[] { null,null,"TABLE_NAME",null});
            //string[] strTableNames = new string[columtab.Rows.Count];
            //for (int k = 0; k < columtab.Rows.Count; k++)
            //{
            //    strTableNames[k] = columtab.Rows[k]["TABLE_NAME"].ToString();
            //}
            //return strTableNames;
        }
    }
    #endregion

        #region  写日志类
    public class writelog
    {
        public string logFile;
        private StreamWriter writer;
        private FileStream fileStream = null;

        public void Log(string fileName)
        {
            logFile = fileName;
            CreateDirectory(logFile);
        }

        public void log(string info)
        {

            try
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(logFile);
                if (!fileInfo.Exists)
                {
                    fileStream = fileInfo.Create();
                    writer = new StreamWriter(fileStream);
                }
                else
                {
                    fileStream = fileInfo.Open(FileMode.Append, FileAccess.Write);
                    writer = new StreamWriter(fileStream);
                }
                writer.WriteLine(DateTime.Now + ": " + info);

            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
        }

        public void CreateDirectory(string infoPath)
        {
            DirectoryInfo directoryInfo = Directory.GetParent(infoPath);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
        }
    }

    #endregion

        #region  串口通信总类
    public class serialport1
    {
        #region  串口通信类,类485通信,主要和控制柜通信

        //开门信号下发，并检测正产打开
        //发送温度采集信号，等待100ms
        public List<byte> senddata(byte[] senddata, List<SerialPort> portlist1, SerialPort port32)
        {
            List<byte> backdata = new List<byte>(8);  //定义列表成员，存放buf数据

            //16进制组包协议
            byte[] str = { 0x65, 0x03, 0x00, 0x01, 0x00, 0x01, 0xDD, 0xEE };   //查询101站温度

            port32.Write(str, 0, str.Length);  //质询下位机端口,寻找连接端口,查101温度               
            Thread.Sleep(50);                      //等待数据接收完成，100ms
            if (port32.BytesToRead != 0)
            {
                port32.DiscardInBuffer();
                //int d = 1;
                //backdata.Clear();
                //for (; d > 0; d--)
                //{
                //    port32.DiscardInBuffer();
                port32.Write(senddata, 0, senddata.Length);   //写
                Thread.Sleep(50);                  //等待100ms,开门检测
                if (port32.BytesToRead != 0)         //读到的数据长度
                {
                    backdata.Clear();
                    int n = port32.BytesToRead;
                    byte[] buf = new byte[n];          //定义一个新的buf存储
                    port32.Read(buf, 0, n);            //读取数据
                    for (int i = 0; i < n; i++)
                    {
                        backdata.Add(buf[i]);
                    }
                    //if (backdata.Count >= 5)
                    //{
                        //break;
                        return backdata;
                    //} 
                }
            }
            //}
            return backdata;

        }

        //发送锁控信号，等待1s收
        public bool start;
        public List<byte> senddata1(byte[] senddata, List<SerialPort> portlist1, SerialPort port32)
        {
            firtdoor f1 = new firtdoor(0, null);
            List<byte> backdata = new List<byte>(5);  //定义列表成员，存放buf数据
            byte[] str = { 0x00, 0x04, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00 };  //读锁头反馈
            str[0] = senddata[0];
            //ch1-ch16
            if (senddata[3] - 16 <= 0)
            {
                str[3] = 0x03;
            }
            //ch17-ch30
            else
            {
                str[3] = 0x04;
            }
            //crc校验
            int c = 10;
            for (; c > 0; c--)
            {
                port32.DiscardInBuffer();
                str = f1.crce(str);
                Thread.Sleep(10);
                port32.Write(senddata, 0, senddata.Length);   //写开锁
                Thread.Sleep(50);
                port32.Write(str, 0, str.Length);    //读锁头反馈
                Thread.Sleep(50);
                if (port32.BytesToRead != 0)         //读到的数据长度
                {
                    int n = port32.BytesToRead;
                    byte[] buf = new byte[n];          //定义一个新的buf存储
                    port32.Read(buf, 0, n);            //读取数据
                    for (int i = 0; i < n; i++)
                    {
                        backdata.Add(buf[i]);
                    }
                    //1903加
                    if (backdata.Count>=5)
                    {
                        break;
                    }
                    //
                    //break;
                }
            }
            return backdata;
        }

        //进水传感协议数据发送
        public List<byte> senddata3(byte[] senddata, List<SerialPort> portlist1, SerialPort port32)
        {
            List<byte> backdata = new List<byte>(8);     //定义列表成员，存放buf数据

            port32.DiscardInBuffer();
            port32.Write(senddata, 0, senddata.Length);  //质询下位机端口,寻找连接端口,查101温度               
            Thread.Sleep(100);                            //等待数据接收完成，100ms
            if (port32.BytesToRead != 0)
            {
                backdata.Clear();
                int n = port32.BytesToRead;
                if (n <7)
                {
                    return backdata;
                }
                byte[] buf = new byte[n];          //定义一个新的buf存储
                port32.Read(buf, 0, n);            //读取数据
                for (int i = 0; i < n; i++)
                {
                    backdata.Add(buf[i]);
                }
                return backdata;
            }
            return backdata;

        }
        #endregion

    }
    #endregion

        #region  生成二维码类
    public class qrimage_code
    {
        public Image qrimage(string qrstr, int size, int qulity)
        {
            if (qrstr != "")
            {
                //字符类型
                QRCodeEncoder qrencoder = new QRCodeEncoder();
                qrencoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                //尺寸 默认12
                qrencoder.QRCodeScale = size;
                //打印容量  默认12
                qrencoder.QRCodeVersion = qulity;
                //条形码质量
                qrencoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;               //加入内容
                Image qrimage;
                qrimage = qrencoder.Encode(qrstr);
                return qrimage;
            }
            else
            {
                return null;
            }

        }
    }


    #endregion

        #region  时效性计算类
    public class timestoend
    {
        /// <summary>
        /// 天数时效性计算
        /// </summary>
        /// <returns></returns>
        public int time_day(string timestart,string timeend)
        {
            int days1=0;
            try
            {
                var timenyr1 = timestart.Split(new char[1] { ' ' });
                var timl = timenyr1[0].ToString().Split(new char[1] { '/' });  //注意时间格式，我的系统是‘-’，触摸屏上是‘/’
                int timestart1_s = Int32.Parse(timl[1].ToString());
                int timestart1_g = Int32.Parse(timl[2].ToString());
                timenyr1 = timeend.Split(new char[1] { ' ' });
                timl = timenyr1[0].ToString().Split(new char[1] { '/' });
                int timeend1_s = Int32.Parse(timl[1].ToString());
                int timeend1_g = Int32.Parse(timl[2].ToString());
                if (timestart1_s == timeend1_s)
                {
                    days1 = timeend1_g - timestart1_g;
                    return days1;
                }
                else
                {
                    days1 = timeend1_g + 30 - timestart1_g;
                    return days1;
                }   
            }
            catch
            { return days1; }

        }

        /// <summary>
        /// 小时时效性计算
        /// </summary>
        /// <param name="timestart"></param>
        /// <param name="timeend"></param>
        /// <returns></returns>
        public int time_hours(string timestart, string timeend)
        {
            int hours = 0;
            try
            {
                TimeSpan ts1 = new TimeSpan(Convert.ToDateTime(timestart).Ticks);
                TimeSpan ts2 = new TimeSpan(Convert.ToDateTime(timeend).Ticks);
                TimeSpan ts = ts2.Subtract(ts1).Duration();
                hours=Convert.ToInt32(ts.TotalHours);
                return hours;
            }
            catch
            { return hours; }
        }
    }
    #endregion

        #region  重写dataGridview重画类
    public class DataGridViewForWs : DataGridView
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
            }
            catch
            {
                Invalidate();
            }
        }
    }
    #endregion

    #endregion

}
