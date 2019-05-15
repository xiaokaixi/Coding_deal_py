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
using System.Speech.Synthesis;  //


namespace manage
{
    public partial class form_newinput : Form
    {
        #region  
        public firtdoor f1;
        public string dirpath = Application.StartupPath + "\\FreeVK.exe";  //
        public dynamic file;
        public Process p1;
        public int selecttype = 0;  //
        public int vertloction1 = 0;  //
        public System.Timers.Timer vsbar_t = new System.Timers.Timer();
        public DataSet mysql_tid = new DataSet();
        public DataSet mysql_tid_1 = new DataSet();   //
        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        public static extern bool MoveWindow(System.IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        //
        delegate void datagrid1dill(DataGridView datagrid1, TextBox textbox8, TextBox textbox6, TextBox textbox1, string strbuf, string strbuf_qr);
        public static bool bIsLoop = false;
        public static Thread DecodeThread = null;
        Vbarapi qrsm = new Vbarapi();
        public SpeechSynthesizer speech; //TTS
        #endregion

        #region  
        public form_newinput(firtdoor f11)
        {
            InitializeComponent();
            f1 = f11;
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            //textbox
            moveandtext3();
            //
            initfunction();
            //
            timenow();
            //
            System.Timers.Timer t1 = new System.Timers.Timer(50);
            t1.Elapsed += new ElapsedEventHandler(choose_doorid);
            t1.AutoReset = true;
            t1.Enabled = true;
            //
            Control.CheckForIllegalCrossThreadCalls = false;
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
                    //
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

        #region   
        //
        private void choose_doorid(object sender, ElapsedEventArgs e)
        {
            dothis();
        }

        //
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
                    //
                    if (radioButton1.Checked == true)
                    {
                        textBox4.Enabled = true;
                        textBox5.Enabled = false;
                        textBox7.Enabled = false;
                    }
                    //
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

        #region   
        /// <summary>
        /// 
        /// </summary>
        private void initfunction()
        {
            //tid
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

            //
            label17.Visible = false;
            label8.Visible = false;
            label10.Visible = false;
            label12.Visible = false;
            label14.Visible = false;
            label15.Visible = false;
            textBox4.Text = f1.num.ToString() + "-" + f1.box.ToString();
            //
            button1.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button17.Enabled = false;
            label9.Enabled = false;
            //
            f1.init1();
            qrresult();
            run_qr = true;
            //
            speech = new SpeechSynthesizer();
            speech.Volume = 100;
            speech.Rate = 0;
        }
        #endregion

        #region  

        //
        private void qrresult()
        {
            bIsLoop = true;
            DecodeThread = new Thread(new ThreadStart(DecodeThreadMethod));
            DecodeThread.IsBackground = true;
            DecodeThread.Start();
        }

        //
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
        // 
        private void DecodeThreadMethod()
        {
            datagrid1dill datagrid1dillRef_Instance = new datagrid1dill(listenport_receive);
            do
            {
                try
                {
                    //
                    if (run_qr)
                    {
                        decoderesult_qr = f1.Decoder();  //
                    }
                    //RFID
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

        passwdtext check = new passwdtext();        //txt
        timestoend timestoend = new timestoend();   //
        private void listenport_receive(DataGridView datagrid1, TextBox textbox8, TextBox textbox6, TextBox textbox1, string strbuf, string strbuf_qr)
        {
            string showstr1 = "", showstr2 = "";
            List<byte> buffer = new List<byte>();  //buf
            List<string> messagex;   //
            List<string> chenstr1, chenstr2, chenstr3;   //
            int n = 0, i = 0;
            byte[] buf = new byte[2000];
            if (decoderesult != null)
            {
                n = strbuf.Length;
                //
                for (i = 0; i < strbuf.Length; i++)
                {
                    buf[i] = Convert.ToByte(strbuf[i]);
                }
            }
            if (f1.CardNum != 0)
            {
                f1.sEPC = "%" + f1.sEPC;
                n = f1.sEPC.Length;
                //
                for (i = 0; i < f1.sEPC.Length; i++)
                {
                    buf[i] = Convert.ToByte(f1.sEPC[i]);
                }
            }
            if (strbuf_qr != null)
            {
                n = 0;
                //
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
            //buf
            buffer.AddRange(buf);              //buffererror          

            //RFID
            if (buffer[0] == '%')         //
            {
                //ASCII byte[]string
                string buffer1 = Encoding.ASCII.GetString(buf, 0, n);
                buffer.RemoveRange(0, n);
                //RFID
                messagex = f1.danalysis(buffer1, DateTime.Now.ToString());
                if (messagex != null)
                {
                    //
                    chenstr1 = f1.chenking(messagex[0]);
                    //
                    if (chenstr1.Count != 0)
                    {
                        MessageBox.Show("该档案已入库，请重新扫描！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    showstr2 = messagex[0].Replace("00000", "*");

                    //-
                    for (i = 0; i < mysql_tid.Tables[0].Rows.Count; i++)
                    {
                        if (mysql_tid.Tables[0].Rows[i][0].ToString() == messagex[0])
                        { return; }
                    }
                    //-
                    for (i = 0; i < f1.mysql.Tables[0].Rows.Count; i++)
                    {
                        if (f1.mysql.Tables[0].Rows[i][1].ToString() == messagex[0])
                        {
                            MessageBox.Show("该档案已经录入！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    //
                    f1.danalysis1(f1.num, f1.box);

                    //
                    f1.openlist.Add(f1.comm3);
                    f1.closelist.Add(f1.num.ToString() + "-" + f1.box.ToString());

                    mysql_tid.Tables[0].Rows.Add();
                    mysql_tid_1.Tables[0].Rows.Add();
                    f1.mysql.Tables[0].Rows.Add();
                    f1.mysql2.Tables[0].Rows.Add();
                    f1.mysql4.Tables[0].Rows.Add();
                    f1.mysql5.Tables[0].Rows.Add();
                    //operate_id

                    mysql_tid.Tables[0].Rows[mysql_tid.Tables[0].Rows.Count - 1][0] = messagex[0];
                    mysql_tid.Tables[0].Rows[mysql_tid.Tables[0].Rows.Count - 1][1] = "等待确认";
                    mysql_tid_1.Tables[0].Rows[mysql_tid_1.Tables[0].Rows.Count - 1][0] = messagex[0];
                    f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][0] = f1.count;  //id
                    f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][0] = f1.count++;  //id
                    f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][1] = messagex[0];  //TID
                    f1.mysql2.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][0] = showstr2;  //TID
                    f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][1] = messagex[0];  //TID
                    f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][0] = messagex[0];  //TID
                    f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][2] = "new";  //入库   
                    f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][2] = 1;  //10
                    counnm++;
                    f1.num1 = f1.num;
                    f1.box1 = f1.box;
                    f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][5] = f1.num1.ToString() + "-" + f1.box1.ToString();  //+
                    f1.mysql2.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][1] = f1.num1.ToString() + "-" + f1.box1.ToString();  //+
                    f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][5] = f1.num1.ToString() + "-" + f1.box1.ToString();  //+
                    f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][4] = f1.num1.ToString() + "-" + f1.box1.ToString();  //+
                    f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());  //mysqltime                                                                                                                                                   //}
                    f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());  //mysqltime 
                    f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][2] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                    datagrid1.DataSource = mysql_tid.Tables[0];
                    datagrid1.Columns[0].Width = 150;  //id
                    datagrid1.Columns[1].Width = 100;  //tid

                    //
                    if (mysql_tid.Tables[0].Rows.Count == 1)
                    {
                        button1.Enabled = true;
                        button17.Enabled = true;
                        label9.Enabled = true;
                    }
                    return;
                }
            }

            //
            else
            {
                //ASCII byte[]string
                string buffer1 = Encoding.ASCII.GetString(buf, 0, n);
                buffer.RemoveRange(0, n);
                //string passkey = "A0123456789";  //解码密钥，要读取本地密钥
                //,
                string passkey1 = check.logincheckset_lingdao("guanlingdao_keywd");
                string passkey = jiema(passkey1);
                string codestr = f1.des_string(buffer1, passkey);

                if (codestr[0] == '%')         //
                {

                    var str_1 = codestr.Split(new char[1] { '%' });
                    var str_2 = str_1[3].Split(new char[1] { ',' });
                    //
                    if (str_1[1] != "I1")
                    {
                        MessageBox.Show("无效二维码，请扫描入库二维码！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);  //此代码在归还入库时有效
                        return;
                    }
                    //
                    if (timestoend.time_hours(str_1[2], DateTime.Now.ToString()) > 24)
                    {
                        MessageBox.Show("二维码已过期，请在申请二维码24小时内操作！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);   //二维码有效性判断
                        return;
                    }
                    //
                    textbox8.Text = f1.gb2312tochinese(str_2[0]);

                    //
                    textbox6.Text = str_2[1];
                    //
                    textbox1.Text = str_2[2];

                    //
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

        #region    
        /// <summary>
        /// ASII
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
        /// ASII
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

        #region  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private delegate void setrefresh1(); //
        public bool flag = false;
        public int count = 0;
        private void moveandtext3()
        {
            Thread moveandtext = new Thread(new ThreadStart(refreshtime1_0));
            moveandtext.IsBackground = true;  //
            moveandtext.Start();
        }
        //
        private void refreshtime1_0()
        {
            try
            {
                vsbar_t.Interval = 50;  //50ms    
                vsbar_t.Elapsed += new System.Timers.ElapsedEventHandler(refreshtime1);
                vsbar_t.AutoReset = true;  //true,false
                vsbar_t.Enabled = true;
                //textbox
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
                        //
                        //for (int i = 0; i < f1.mysql_openview.Tables[0].Rows.Count; i++)
                        //{
                        if (f1.doorlistopen.IndexOf(f1.closestr) >= 0)
                        {
                            if (textBox3.Text.Contains(f1.closestr))
                            {
                            }
                            else
                            {
                                textBox3.AppendText("\r\n"); //
                                textBox3.AppendText(DateTime.Now.ToString() + "  " + f1.closestr + "  ");
                                textBox3.AppendText("\r\n"); //
                                textBox3.AppendText(f1.mysql.Tables[0].Rows[0][7].ToString() + "  " + f1.mysql.Tables[0].Rows[0][3].ToString() + " ");
                                textBox3.AppendText("\r\n"); //
                                f1.label24text = "";
                                doorname = f1.closestr;
                                //
                                speech.SpeakAsync(f1.closestr + "" + num[0].ToString() + "");
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
                                //
                                speech.SpeakAsync(doorname + "");
                                textBox3.AppendText(DateTime.Now.ToString() + "  " + doorname + "  " + num[0].ToString() + "");
                                textBox3.AppendText("\r\n"); //
                                nucount = f1.closelist.Count;
                                f1.label24text = "";
                                num.RemoveAt(0);
                                nucount1 = 0;
                                if (f1.dflag == 1)
                                { vsbar_t.Stop(); } 
                            }
                            else
                            {
                                textBox3.AppendText(DateTime.Now.ToString() + "  " + doorname + "  1");
                                textBox3.AppendText("\r\n"); //
                                nucount1++;
                            }
                            
                        }

                        //
                        if (f1.label24text == "入库异常")
                        {
                            f1.label24text = "";
                            //
                            speech.SpeakAsync(f1.closestr + "");
                            textBox3.AppendText(DateTime.Now.ToString() + "  " + f1.closestr + "  1");
                            textBox3.AppendText("\r\n"); //
                            button2.Enabled = true;
                            vsbar_t.Stop();
                        }
                        //
                        if (num.Count == 0&&f1.b1 == true)
                        {
                            if (f1.dflag == 1)
                            {
                                //
                                speech.SpeakAsync("");
                                textBox3.AppendText("\r\n"); //
                                textBox3.AppendText(DateTime.Now.ToString() + "  " + doorname + "  ");
                                textBox3.AppendText("\r\n"); //
                                button2.Enabled = false;
                            }
                            else
                            {
                                //
                                speech.SpeakAsync("");
                                textBox3.AppendText("\r\n"); //
                                textBox3.AppendText(DateTime.Now.ToString() + "  " + doorname + "  ");
                                textBox3.AppendText("\r\n"); // 
                                //5
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

        #region  
        private void button5_Click(object sender, EventArgs e)
        {
            //
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

        #region  
        /// <summary>
        /// 
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

        #region   
        private void closeinit()
        {
            //
            int fCmdRet1;
            fCmdRet1 = StaticClassReaderB.CloseSpecComPort(f1.port);
            if (fCmdRet1 == 0)
            {
                //MessageBox.Show("关闭成功！");
            }
            //
            f1.init1close();
            //
            dataGridView1.DataSource = null;
            f1.openlist.Clear();
            f1.closelist.Clear();
        }
        #endregion

        #region  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //
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
            //
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
            //
            button1.Enabled = false;
            button17.Enabled = false;
            label9.Text = "干部档案信息等待核对！";
            label9.Enabled = false;
            //
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

        #region  
        /// <summary>
        /// 
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
                //
                saomaflg = false;
                //
                //
                if (textBox8.Text == "")
                {
                    label17.Text = "× 输入为空！";
                    label17.ForeColor = System.Drawing.Color.Red;
                    label17.Visible = true;
                }
                //
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
                //
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
                    //
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
                //
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
                    //3,3
                    if (int.Parse(chenumlist[2]) + int.Parse(textBox2.Text) <= 3)
                    {
                        label14.Text = "√ 新增档案！";
                        label14.ForeColor = System.Drawing.Color.Green;
                        label14.Visible = true;
                        textBox4.Text = chenumlist[0] + "-" + chenumlist[1];
                        changedata2();
                        sum++;
                    }
                    //33
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
                //
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
                    //
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
                        //
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

                //tid
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

                //
                if (sum == 4)
                {
                    //
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
                    //
                    radioButton1.Enabled = false;
                    radioButton2.Enabled = false;
                    //datagridview
                    for (int i=0;i< mysql_tid.Tables[0].Rows.Count;i++)
                    {
                        mysql_tid.Tables[0].Rows[i][1] = "确认正常";
                    }
                    
                    //
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

        //
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
                f1.mysql.Tables[0].Rows[i][5] = f1.num1.ToString() + "-" + f1.box1.ToString();  //+
                f1.mysql2.Tables[0].Rows[i][1] = f1.num1.ToString() + "-" + f1.box1.ToString();  //+
                f1.mysql4.Tables[0].Rows[i][5] = f1.num1.ToString() + "-" + f1.box1.ToString();  //+
                f1.mysql5.Tables[0].Rows[i][4] = f1.num1.ToString() + "-" + f1.box1.ToString();  //+
                f1.openlist[i]=f1.comm3;
                f1.closelist[i]=f1.num.ToString() + "-" + f1.box.ToString();
            }
        }

        //
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
                f1.mysql.Tables[0].Rows[i][5] = f1.num1.ToString() + "-" + f1.box1.ToString();  //+
                f1.mysql2.Tables[0].Rows[i][1] = f1.num1.ToString() + "-" + f1.box1.ToString();  //+
                f1.mysql4.Tables[0].Rows[i][5] = f1.num1.ToString() + "-" + f1.box1.ToString();  //+
                f1.mysql5.Tables[0].Rows[i][4] = f1.num1.ToString() + "-" + f1.box1.ToString();  //+
            }
        }

        public int counnm1 = 0;
        private bool changedata1()
        {
            bool flg = true;
            int m = counnm1 + mysql_tid.Tables[0].Rows.Count;
            //
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
                f1.mysql.Tables[0].Rows[i][3] = textBox8.Text;  //name  
                f1.mysql4.Tables[0].Rows[i][3] = textBox8.Text;  //name  
                f1.mysql.Tables[0].Rows[i][4] = textBox1.Text;  //perid 
                f1.mysql.Tables[0].Rows[i][7] = label10.Text;  //unit_name 
                f1.mysql4.Tables[0].Rows[i][4] = textBox1.Text;  //perid 
                f1.mysql4.Tables[0].Rows[i][7] = label10.Text;  //unit_name 
                f1.mysql5.Tables[0].Rows[i][1] = textBox1.Text;  //perid 
                f1.mysql.Tables[0].Rows[i][8] = f1.username;   //
                f1.mysql4.Tables[0].Rows[i][8] = f1.username;  //
                f1.mysql5.Tables[0].Rows[i][3] = f1.username;  //
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

        #region  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            //
            saomaflg = true;
            //
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
            //
            button1.Enabled = false;
            button17.Enabled = false;
            label9.Text = "干部档案信息等待核对！";
            label9.Enabled = false;
            //
            button2.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            button3.Enabled = false;
            //
            mysql_tid.Tables[0].Clear();
            mysql_tid_1.Tables[0].Clear();
            //-
            radioButton1.Enabled = true;
            radioButton2.Enabled = true;

            //-  
            while (true)
            {
                try
                {
                    //
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
                    //
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

        #region  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void button2_Click(object sender, EventArgs e)
        {
            vsbar_t.Start();
            //
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
            //
            //
            radioButton1.Enabled = false;
            radioButton2.Enabled = false;
            textBox4.Enabled = false;
            textBox5.Enabled = false;
            textBox7.Enabled = false;
            //
            button17.Enabled = false;
            label9.Enabled = false;
            button1.Enabled = false;
            button3.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            //
            if (radioButton4.Checked==true)
            {
                //
                speech.SpeakAsync("");
                Thread.Sleep(1000);
                button2.Enabled = false;
                f1.dflag = 2;
                //
                f1.t.Stop();
                f1.newinstore();
            }
            //
            else
            {
                //
                speech.SpeakAsync("");
                f1.dflag = 1;
                //
                f1.t.Stop();
                f1.newinstore();
            }
        }
        #endregion

        #region  
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
