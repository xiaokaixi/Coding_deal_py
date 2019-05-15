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
    public partial class form_returninput : Form
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
        public DataSet mysql_tid_1 = new DataSet();
        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        public static extern bool MoveWindow(System.IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        //
        delegate void datagrid1dill(DataGridView datagrid1, string strbuf);
        public static bool bIsLoop = false;
        public static Thread DecodeThread = null;
        Vbarapi qrsm = new Vbarapi();
        public SpeechSynthesizer speech; //TTS
        #endregion

        #region  
        public form_returninput(firtdoor f11)
        {
            InitializeComponent();
            f1 = f11;
        }

        private void form_returninput_Load(object sender, EventArgs e)
        {
            //
            moveandtext3();          
            //
            initfunction();
            //
            timenow();
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
                    textBox12.Text = DateTime.Now.ToString();
                    if (countt == 2)
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
            label2.Visible = false;
            label5.Visible = false;
            label11.Visible = false;
            label13.Visible = false;
            //
            button1.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button17.Enabled = false;
            label9.Enabled = false;
            button6.Enabled = false;
            label4.Enabled = false;
            button5.Enabled = false;
            //
            f1.init1();
            qrresult();
            //
            f1.gate_cleardata();
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
        // 
        private void DecodeThreadMethod()
        {
            datagrid1dill datagrid1dillRef_Instance = new datagrid1dill(listenport_receive);
            do
            {
                try
                {
                    //decoderesult = Decoder();  //
                    f1.readrfid();
                    if (f1.CardNum != 0 || decoderesult != null)
                    {
                        if (decoderesult != null)
                        {
                            qrsm.beepControl(1);
                        }
                        this.Invoke(datagrid1dillRef_Instance, new object[] { this.dataGridView1, decoderesult });
                        decoderesult = null;
                        f1.CardNum = 0;
                    }
                }
                catch
                { }
            }
            while (bIsLoop);
        }

        public int mysqlcount = 0;
        private void listenport_receive(DataGridView datagrid1, string strbuf)
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
                    //
                    //if (mysql_tid.Tables[0].Rows.Count==int.Parse(textBox5.Text))
                    //{
                    //    return;
                    //}
                    //
                    chenstr1 = f1.chenking(messagex[0]);
                    //-
                    for (i = 0; i < mysql_tid.Tables[0].Rows.Count; i++)
                    {
                        if (mysql_tid.Tables[0].Rows[i][0].ToString() == messagex[0]|| button1.Enabled == true)
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
                    if (chenstr1.Count == 0)
                    {
                        f1.killtxt = "ERROR";
                        f1.startkill();
                        MessageBox.Show("该档案不存在，请检查！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);   //档案不存在（或未入库）判断
                        return;
                    }
                    else if (chenstr1[0] == "1")
                    {
                        //
                        if (f1.mysqll.Tables[0].Rows[0][2].ToString() == "1")
                        {
                            f1.killtxt = "ERROR";
                            f1.startkill();
                            MessageBox.Show("该档案未借出！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);   //档案不存在（或未入库）判断
                            return;
                        }
                        //
                        else
                        {
                            //
                            if (mysql_tid.Tables[0].Rows.Count != 0 && textBox7.Text != "")
                            {
                                if (f1.mysqll.Tables[0].Rows[0][4].ToString() != f1.mysql.Tables[0].Rows[mysqlcount - 1][4].ToString())
                                {
                                    if (button17.Enabled == false)
                                    { return; }
                                    mysql_tid.Tables[0].Rows.Add();
                                    mysql_tid.Tables[0].Rows[mysql_tid.Tables[0].Rows.Count - 1][0] = messagex[0];
                                    mysql_tid.Tables[0].Rows[mysql_tid.Tables[0].Rows.Count - 1][1] = "！请清除";
                                    dataGridView1.Rows[mysql_tid.Tables[0].Rows.Count - 1].Cells[0].Style.ForeColor = System.Drawing.Color.Red;
                                    dataGridView1.Rows[mysql_tid.Tables[0].Rows.Count - 1].Cells[1].Style.ForeColor = System.Drawing.Color.Red;
                                    button1.Enabled = true;
                                    button17.Enabled = false;
                                    label9.Enabled = false;
                                    return;
                                }
                            }
                            //
                            if (mysql_tid.Tables[0].Rows.Count >= 0)
                            {
                                //
                                textBox1.Text= f1.mysqll.Tables[0].Rows[0][3].ToString();  //-name 
                                //
                                textBox8.Text = f1.mysqll.Tables[0].Rows[0][7].ToString();  //unit_name  
                                //
                                textBox7.Text= f1.mysqll.Tables[0].Rows[0][4].ToString();  //perid 
                                //
                                f1.check_b1perid = true;
                                chenstr1 = f1.chenking(textBox7.Text);
                                textBox5.Text = chenstr1.Count.ToString();  //()
                                f1.check_b1perid = false;
                                //
                                textBox9.Text= f1.mysqll.Tables[0].Rows[0][5].ToString(); //+
                                
                            }
                           

                            mysql_tid.Tables[0].Rows.Add();
                            mysql_tid_1.Tables[0].Rows.Add();

                            f1.mysql.Tables[0].Rows.Add();
                            f1.mysql2.Tables[0].Rows.Add();
                            f1.mysql4.Tables[0].Rows.Add();
                            f1.mysql5.Tables[0].Rows.Add();
                            mysqlcount++;
                            datagrid1.DataSource = f1.mysql2.Tables[0];
                            f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][0] = f1.mysqll.Tables[0].Rows[0][0];    //id 
                            f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][0] = f1.mysqll.Tables[0].Rows[0][0];    //id                                                                                                                     
                            f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][1] = f1.mysqll.Tables[0].Rows[0][1];    //tid
                            f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][1] = f1.mysqll.Tables[0].Rows[0][1];    //tid
                            f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][0] = f1.mysqll.Tables[0].Rows[0][1];  //TID 
                            f1.mysql2.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][0] = f1.mysqll.Tables[0].Rows[0][1].ToString().Replace("00000", "*");  //TID 
                            f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][2] = "return";  //出库(借阅)
                               
                            mysql_tid.Tables[0].Rows[mysql_tid.Tables[0].Rows.Count - 1][0] = messagex[0];
                            mysql_tid.Tables[0].Rows[mysql_tid.Tables[0].Rows.Count - 1][1] = "等待确认";
                            mysql_tid_1.Tables[0].Rows[mysql_tid_1.Tables[0].Rows.Count - 1][0] = messagex[0];

                            f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][2] = f1.mysqll.Tables[0].Rows[0][2];  //status1
                            f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][3] = f1.mysqll.Tables[0].Rows[0][3];  //   
                            f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][3] = f1.mysqll.Tables[0].Rows[0][3];  //                                                                                                                                              
                            f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][4] = f1.mysqll.Tables[0].Rows[0][4];  //perid 
                            f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][4] = f1.mysqll.Tables[0].Rows[0][4];  //perid 
                            f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][1] = f1.mysqll.Tables[0].Rows[0][4];  //perid 
                            f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][5] = f1.mysqll.Tables[0].Rows[0][5];  //,position
                            f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][5] = f1.mysqll.Tables[0].Rows[0][5];  //,position
                            f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][4] = f1.mysqll.Tables[0].Rows[0][5];  //+
                            f1.mysql2.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][1] = f1.mysqll.Tables[0].Rows[0][5];  //+ 
                            f1.openlist.Add(f1.comm3);
                            f1.closelist.Add(f1.mysqll.Tables[0].Rows[0][5].ToString());
                            f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());  //
                            f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                            f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][2] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());

                            f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][7] = f1.mysqll.Tables[0].Rows[0][7];        //
                            f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][7] = f1.mysqll.Tables[0].Rows[0][7]; ;       //
                            f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][8] = f1.username;         //
                            f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][8] = f1.username;        //
                            f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][3] = f1.username;        //
                            datagrid1.DataSource = mysql_tid.Tables[0];
                            //datagrid1.Rows[0].Selected = true;  //
                            datagrid1.Columns[0].Width = 200;  //id
                            datagrid1.Columns[1].Width = 80;  //tid
                            

                            //
                            if (mysql_tid.Tables[0].Rows.Count >= 1)
                            {
                                button5.Enabled = true;
                                button17.Enabled = true;
                                label9.Enabled = true;
                            }
                            else
                            {
                                button5.Enabled = false;
                                button17.Enabled = false;
                                label9.Enabled = false;
                            }
                            return;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("二维码已过期，请在得到二维码4小时内操作！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);   //此代码在归还入库时有效
            }
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
                //
                vsbar_t.Stop();
            }
            catch { }
        }
        private void refreshtime1(object sender, ElapsedEventArgs e)
        {
            if (f1.b1 == false) { StopDecodeThread(); this.Close(); return; }
            refresh2();
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
                    try
                    {
                        if (runflg)
                        {
                            return;
                        }
                        runflg = true;

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
                                doorname = f1.closestr;
                                //
                                speech.SpeakAsync(f1.closestr + "" + num[0].ToString() + "");
                            }
                        }
                        //}

                        if (f1.label24text == "入库成功" && (nucount != f1.closelist.Count || (nucount - f1.closelist.Count) == 0))
                        {
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
                        if (num.Count == 0 && f1.b1 == true)
                        {
                            if (f1.dflag==1)
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
                                Thread.Sleep(7000);
                                f1.b1 = false;
                                f1.dflag = 0;
                                speech.Dispose();
                                mysql_tid.Clear();
                                mysql_tid_1.Clear();
                            }   
                            
                        }

                    }
                    catch
                    { }
                }
                runflg = false;
            }
            catch
            { }
        }

        #endregion

        #region  
        private void button7_Click(object sender, EventArgs e)
        {
            button7.Enabled = false;
            //
            if (button7.Text == "打开输入键盘")
            {
                button7.Text = "关闭输入键盘";
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
                button7.Enabled = true;
            }
            else
            {
                button7.Text = "打开输入键盘";
                p1.CloseMainWindow();
                button7.Enabled = true;
            }
        }

        #endregion

        #region  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click_1(object sender, EventArgs e)
        {
            try
            {
                f1.b1 = false;
                f1.dflag = 0;
                vsbar_t.Stop();
                p1.CloseMainWindow();
                closeinit();
                speech.Dispose();
                StopDecodeThread();
                this.Close();
            }
            catch
            {
                f1.b1 = false;
                f1.dflag = 0;
                vsbar_t.Stop();
                closeinit();
                speech.Dispose();
                StopDecodeThread();
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
        private void button5_Click(object sender, EventArgs e)
        {
            if (button6.Enabled == false)
            {
                textBox1.Clear();
                textBox8.Clear();
                textBox7.Clear();
                textBox5.Clear();
                textBox9.Clear();
                label2.Visible = false;
                label5.Visible = false;
                label11.Visible = false;
                label13.Visible = false;
                //
                button1.Enabled = false;
                button17.Enabled = false;
                label9.Text = "干部档案信息等待确认！";
                label9.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                label4.Enabled = false;
                //   
                if (button1.Enabled == true)
                {
                    for (int i = 0; i < mysql_tid.Tables[0].Rows.Count - 1; i++)
                    {
                        int m = f1.mysql.Tables[0].Rows.Count - 2;
                        f1.mysql.Tables[0].Rows.RemoveAt(m);
                        f1.mysql2.Tables[0].Rows.RemoveAt(m);
                        f1.mysql4.Tables[0].Rows.RemoveAt(m);
                        f1.mysql5.Tables[0].Rows.RemoveAt(m);
                        f1.openlist.RemoveAt(f1.openlist.Count - 1);
                        f1.closelist.RemoveAt(f1.closelist.Count - 1);
                        mysqlcount--;
                    }
                    mysql_tid.Clear();
                    mysql_tid_1.Clear();
                }
                else
                {
                    for (int i = 0; i < mysql_tid.Tables[0].Rows.Count; i++)
                    {
                        int m = f1.mysql.Tables[0].Rows.Count - 2;
                        f1.mysql.Tables[0].Rows.RemoveAt(m);
                        f1.mysql2.Tables[0].Rows.RemoveAt(m);
                        f1.mysql4.Tables[0].Rows.RemoveAt(m);
                        f1.mysql5.Tables[0].Rows.RemoveAt(m);
                        f1.openlist.RemoveAt(f1.openlist.Count - 1);
                        f1.closelist.RemoveAt(f1.closelist.Count - 1);
                        mysqlcount--;
                    }
                    mysql_tid.Clear();
                    mysql_tid_1.Clear();
                }
            }
            else
            {
                //                
                textBox1.Clear();
                textBox8.Clear();
                textBox7.Clear();
                textBox5.Clear();
                textBox9.Clear();
                label2.Visible = false;
                label5.Visible = false;
                label11.Visible = false;
                label13.Visible = false;
                //
                button1.Enabled = false;
                button17.Enabled = false;
                label9.Text = "干部档案信息等待确认！";
                label9.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                label4.Enabled = false;

                //   
                for (int i = 0; i < mysql_tid.Tables[0].Rows.Count; i++)
                {
                    int m = f1.mysql.Tables[0].Rows.Count - 2;
                    f1.mysql.Tables[0].Rows.RemoveAt(m);
                    f1.mysql2.Tables[0].Rows.RemoveAt(m);
                    f1.mysql4.Tables[0].Rows.RemoveAt(m);
                    f1.mysql5.Tables[0].Rows.RemoveAt(m);
                    f1.openlist.RemoveAt(f1.openlist.Count - 1);
                    f1.closelist.RemoveAt(f1.closelist.Count - 1);
                    mysqlcount--;
                }
                mysql_tid.Clear();
                mysql_tid_1.Clear();
            }
        }

        #endregion

        #region  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button17_Click_1(object sender, EventArgs e)
        {
            try
            {
                int sum = 0;
                //
                //
                if (textBox8.Text == "")
                {
                    label5.Text = "× 输入为空！";
                    label5.ForeColor = System.Drawing.Color.Red;
                    label5.Visible = true;
                }
                else
                {
                    label5.Text = "√";
                    label5.ForeColor = System.Drawing.Color.Green;
                    label5.Visible = true;
                    sum++;
                }
                //
                if (textBox7.Text == "")
                {
                    label11.Text = "× 输入为空！";
                    label11.ForeColor = System.Drawing.Color.Red;
                    label11.Visible = true;
                }
                else if (textBox7.Text.Length != 18)
                {
                    label11.Text = "× 格式有误！";
                    label11.ForeColor = System.Drawing.Color.Red;
                    label11.Visible = true;
                }
                else
                {
                    label11.Text = "√";
                    label11.ForeColor = System.Drawing.Color.Green;
                    label11.Visible = true;
                    sum++;
                }
               
                //
                if (textBox9.Text == "")
                {
                    label13.Text = "× 输入为空！";
                    label13.ForeColor = System.Drawing.Color.Red;
                    label13.Visible = true;
                }
                else if (int.Parse(textBox9.Text.Split(new char[1] { '-'})[1].ToString()) > 30)
                {
                    label13.Text = "× 柜体号非法！";
                    label13.ForeColor = System.Drawing.Color.Red;
                    label13.Visible = true;
                }
                else
                {
                    label13.Text = "√";
                    label13.ForeColor = System.Drawing.Color.Green;
                    label13.Visible = true;
                    sum++;
                }

                //
                if (sum == 3)
                {
                    
                    //
                    if (int.Parse(textBox5.Text) > mysql_tid.Tables[0].Rows.Count)
                    {
                        label2.Text = "× 请进行差额确认！";
                        label2.ForeColor = System.Drawing.Color.Red;
                        label2.Visible = true;
                        label9.Text = "存在差额，请确认！";
                        button5.Enabled = true;
                        button6.Enabled = true;
                        label4.Enabled = true;
                        button2.Enabled = false;
                        radioButton3.Enabled = false;
                        radioButton4.Enabled = false;
                        button3.Enabled = false;
                        //
                        for (int i = 0; i < mysql_tid.Tables[0].Rows.Count; i++)
                        {
                            mysql_tid.Tables[0].Rows[i][1] = "！差额确认";
                            dataGridView1.Rows[i].Cells[1].Style.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                    else
                    {
                        label2.Text = "√";
                        label2.ForeColor = System.Drawing.Color.Green;
                        label2.Visible = true;
                        label9.Text = "干部档案信息核对成功！";
                        button5.Enabled = false;
                        button2.Enabled = true;
                        radioButton3.Enabled = true;
                        radioButton4.Enabled = true;
                        button3.Enabled = true;
                        //
                        num.Add(mysql_tid.Tables[0].Rows.Count);
                        button17.Enabled =false;
                        label9.Enabled=false;
                        //
                        for (int i = 0; i < mysql_tid.Tables[0].Rows.Count; i++)
                        {
                            mysql_tid.Tables[0].Rows[i][1] = "确认成功";
                            dataGridView1.Rows[i].Cells[1].Style.ForeColor = System.Drawing.Color.Green;
                        }
                    }
                    
                }
                else
                {
                    label9.Text = "干部档案信息核对有误！";
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click_1(object sender, EventArgs e)
        {
            //
            textBox1.Clear();
            textBox8.Clear();
            textBox7.Clear();
            textBox5.Clear();
            textBox9.Clear();
            label2.Visible = false;
            label5.Visible = false;
            label11.Visible = false;
            label13.Visible = false;
            //
            button1.Enabled = false;
            button17.Enabled = false;
            label9.Text = "干部档案信息等待确认！";
            label9.Enabled = false;
            //
            button2.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            button3.Enabled = false;
            //
            mysql_tid.Tables[0].Clear();
            mysql_tid_1.Tables[0].Clear();
        }

        #endregion

        #region  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void button2_Click_1(object sender, EventArgs e)
        {
            vsbar_t.Start();
            //
            mysql_tid.Clear();
            mysql_tid_1.Clear();
            textBox1.Clear();
            textBox8.Clear();
            textBox7.Clear();
            textBox5.Clear();
            textBox9.Clear();
            label2.Visible = false;
            label5.Visible = false;
            label11.Visible = false;
            label13.Visible = false;
            //
            button1.Enabled = false;
            button17.Enabled = false;
            label9.Enabled = false;
            //
            button17.Enabled = false;
            label9.Enabled = false;
            button1.Enabled = false;
            button3.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            //
            if (radioButton4.Checked == true)
            {
                //
                speech.SpeakAsync("");
                button2.Enabled = false;
                f1.dflag = 2;
                f1.t.Stop();
                f1.newinstore();
            }
            //
            else
            {
                //
                speech.SpeakAsync("");
                f1.dflag = 1;
                f1.t.Stop();
                f1.newinstore();
            }
        }



        #endregion

        #region  
        public List<int> num = new List<int>();
        private void button6_Click(object sender, EventArgs e)
        {
            label2.Text = "√";
            label2.Visible = true;
            label2.ForeColor = System.Drawing.Color.Green;
            label4.Text = "档案盒数差额确认成功！";
            button6.Enabled = false;
            label4.Enabled = false;
            button17.Enabled = false;
            label9.Enabled = false;
            button5.Enabled = false;
            //
            button2.Enabled = true;
            radioButton3.Enabled = true;
            radioButton4.Enabled = true;
            button3.Enabled = true;
            //
            for (int i = 0; i < mysql_tid.Tables[0].Rows.Count; i++)
            {
                mysql_tid.Tables[0].Rows[i][1] = "确认成功";
                dataGridView1.Rows[i].Cells[1].Style.ForeColor = System.Drawing.Color.Green;
            }
            //
            num.Add(mysql_tid.Tables[0].Rows.Count);

        }
        #endregion

        #region   
        private void button1_Click(object sender, EventArgs e)
        {
            mysql_tid.Tables[0].Rows.RemoveAt(mysql_tid.Tables[0].Rows.Count-1);
            button17.Enabled = true;
            label9.Enabled = true;
            button1.Enabled = false;
        }
        #endregion

    }
}

