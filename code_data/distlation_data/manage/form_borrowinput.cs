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
using System.Speech.Synthesis;  //

namespace manage
{
    public partial class form_borrowinput : Form
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
        delegate void datagrid1dill(DataGridView datagrid1, TextBox textbox5, string strbuf, string strbuf_qr);
        public static bool bIsLoop = false;
        public static Thread DecodeThread = null;
        Vbarapi qrsm = new Vbarapi();
        public bool goon = false;
        public string perid = "";
        public SpeechSynthesizer speech; //TTS
        public bool run_qr=false;
        #endregion

        #region  
        public form_borrowinput(firtdoor f11)
        {
            InitializeComponent();
            f1 = f11;
        }

        private void form_borrowinput_Load(object sender, EventArgs e)
        {
            //
            //this.TopMost = true;           
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
            dataGridView2.DataSource = mysql_tid.Tables[0];
            dataGridView2.Columns[1].Width = 80;
            mysql_tid_1.Tables.Add();
            mysql_tid_1.Tables[0].Columns.Add();
            mysql_tid_1.Tables[0].Columns[0].ColumnName = "test";
           
            //
            textBox4.Enabled = false;
            textBox6.Enabled = false;
            textBox7.Enabled = false;
            textBox8.Enabled = false;
            textBox11.Enabled = false;
            //
            button9.Enabled = false;
            label5.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;
            button2.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            label4.Visible = false;
            textBox2.Visible = false;
            //
            f1.openlist.Clear();
            f1.closelist.Clear();
            //
            run_qr = true;
            qrresult();
            //
            speech = new SpeechSynthesizer();
            speech.Volume = 100;
            speech.Rate = 0;
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
                    //
                    //for (int i = 0; i < f1.mysql_openview.Tables[0].Rows.Count; i++)
                    //{
                    if (f1.doorlistopen.IndexOf(f1.closestr) >= 0)
                    {
                        if (textBox3.Text.Contains(f1.closestr))
                        { }
                        else
                        {
                            textBox3.AppendText("\r\n"); //
                            textBox3.AppendText(DateTime.Now.ToString() + "  " + f1.closestr + "  ");
                            textBox3.AppendText("\r\n"); //
                            textBox3.AppendText(f1.mysql.Tables[0].Rows[0][7].ToString()+"  "+ f1.mysql.Tables[0].Rows[0][3].ToString()+" ");
                            textBox3.AppendText("\r\n"); //
                            //
                            //speech.SpeakAsync(f1.closestr + "" + num[0].ToString() + "");
                            speech.SpeakAsync(f1.closestr + "");
                        }
                    }
                    //}
                    if (f1.label52text == "出库成功")
                    {
                        speech.SpeakAsync(f1.closestr + "");
                        textBox3.AppendText(DateTime.Now.ToString() + "  " + f1.closestr + "  ");
                        textBox3.AppendText("\r\n"); //
                        f1.label52text = "";
                    }

                    if (f1.openlist.Count == 0)
                    {
                        num.Clear();
                    }

                    //
                    if (f1.label52text == "出库异常")
                    {
                        f1.label52text = "";
                        //
                        speech.SpeakAsync(f1.closestr + "");
                        textBox3.AppendText(DateTime.Now.ToString() + "  " + f1.closestr + "  ");
                        textBox3.AppendText("\r\n"); //
                        button2.Enabled = true;
                        vsbar_t.Stop();
                    }
                    //
                    if (f1.openlist.Count == 0&& textflg == true&& num.Count==0)
                    {
                        //
                        speech.SpeakAsync("");
                        textBox3.AppendText("\r\n"); //
                        textBox3.AppendText(DateTime.Now.ToString() + "  ");
                        textBox3.AppendText("\r\n"); //
                        Thread.Sleep(2000);
                        StopDecodeThread();
                        textflg = false;
                        run_qr = false;
                        f1.dflag = 0;
                        //
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

        #region 
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

        #region   
        private void closeinit()
        {
            //
            f1.openlist.Clear();
            f1.closelist.Clear();
            f1.init1close();
        }
        #endregion

        #region   
        private void button6_Click(object sender, EventArgs e)
        {
            //
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

        #region  1
        private void button8_Click(object sender, EventArgs e)
        {
            List<string> chenstr1, chenstr2, chenstr3;   //
            textBox2.Text = "0";
            if (textBox2.Text == "" || textBox5.Text == "")
            {
                MessageBox.Show("请输入借阅信息！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                //
                chenstr1 = f1.chenking(textBox5.Text);
                if (chenstr1[0]=="1")
                {
                    MessageBox.Show("该身份信息不在库，请检查！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    return;
                }
                //
                if (chenstr1[0] == "0" && chenstr1.Count < int.Parse(textBox2.Text))
                {
                    MessageBox.Show("请检查档案盒数，库中档案小于请求盒数！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    return;
                }
                //
                if (chenstr1.Contains("mm"))
                {
                    MessageBox.Show("该干部档案已全部被借出！请检查！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    return;
                }
                //
                if (textBox5.Text.Length !=18 )
                {
                    MessageBox.Show("请检查身份证号码位数（18位）！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    return;
                }
                //
                textBox4.Enabled = true;
                textBox6.Enabled = true;
                textBox7.Enabled = true;
                textBox8.Enabled = true;
                textBox11.Enabled = true;
                //datagridview2
                mysql_tid.Tables[0].Clear();
                for (int i = 0; i < f1.mysqll.Tables[0].Rows.Count; i++)
                {
                    mysql_tid.Tables[0].Rows.Add();
                    mysql_tid.Tables[0].Rows[i][0] = f1.mysqll.Tables[0].Rows[i][1];  //
                    mysql_tid.Tables[0].Rows[i][1] = "等待确认";
                }
                //
                textBox11.Text= f1.mysqll.Tables[0].Rows[0][3].ToString();   //name
                textBox8.Text = f1.mysqll.Tables[0].Rows[0][7].ToString();   //unit_name 
                textBox6.Text = f1.mysqll.Tables[0].Rows.Count.ToString();   //
                textBox7.Text = f1.mysqll.Tables[0].Rows[0][4].ToString();   //perid
                textBox4.Text = f1.mysqll.Tables[0].Rows[0][5].ToString();   //+ 

                button9.Enabled = true;
                label5.Enabled = true;
            }
        }
        #endregion

        #region  2
        public List<int> num = new List<int>();
        private void button9_Click(object sender, EventArgs e)
        {
            if (int.Parse(textBox2.Text) <= int.Parse(textBox6.Text))
            {
                //
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
                //
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

        #region  
        private void button2_Click(object sender, EventArgs e)
        {
            vsbar_t.Start();
            //
            mysql_tid.Tables[0].Rows.Clear();
            textBox2.Clear();
            textBox5.Clear();
            //
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
            //
            button9.Enabled = false;
            label5.Enabled = false;
            button3.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            //
            if (radioButton4.Checked == true)
            {
                //
                speech.SpeakAsync("");
                Thread.Sleep(1000);
                button2.Enabled = false;
                f1.dflag = 2;
                f1.t.Stop();
                f1.newinstore();
            }
            //
            else
            {
                //
                //
                speech.SpeakAsync("");
                f1.dflag = 1;
                f1.t.Stop();
                f1.newinstore();
            }
        }
        #endregion

        #region  
        private void button3_Click(object sender, EventArgs e)
        {            
            //
            mysql_tid.Tables[0].Rows.Clear();
            textBox2.Clear();
            textBox5.Clear();
            //
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
            //            
            button9.Enabled = false;
            label5.Enabled = false;
            button3.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            button2.Enabled = false;
        }

        //
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
        // 
        private void DecodeThreadMethod()
        {
            datagrid1dill datagrid1dillRef_Instance = new datagrid1dill(listenport_receive);
            do
            {
                try
                {
                    if (run_qr)
                    {
                        decoderesult_qr = f1.Decoder();  //
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

        passwdtext check = new passwdtext();        //txt
        timestoend timestoend = new timestoend();   //
        private void listenport_receive(DataGridView datagrid1, TextBox textbox5, string strbuf, string strbuf_qr)
        {
            string showstr1 = "", showstr2 = "";
            List<byte> buffer = new List<byte>();  //buf
            List<string> messagex;   //
            List<string> chenstr1, chenstr2, chenstr3;   //
            int n = 0, i = 0;
            byte[] buf = new byte[2000];
            if (strbuf != null)
            {
                n = strbuf.Length;
                //
                for (i = 0; i < strbuf.Length; i++)
                {
                    buf[i] = Convert.ToByte(strbuf[i]);
                }
                //buf
                buffer.AddRange(buf);              //buffererror        
            }
            if (strbuf_qr != null)
            {
                n = 0;
                //
                for (i = 0; i < strbuf_qr.Length; i++)
                {
                    if (Convert.ToByte(strbuf_qr[i]).ToString() == "0"&& Convert.ToByte(strbuf_qr[i+1]).ToString() == "0"&& Convert.ToByte(strbuf_qr[i+2]).ToString() == "0")
                    {
                        break;
                    }
                    buf[n] = Convert.ToByte(strbuf_qr[i]);
                    n++;
                }
                //buf
                buffer.AddRange(buf);              //buffererror        
            }
              
            //RFID
            if (buffer[0] == '%')         //
            {
                //ASCII byte[]string
                string buffer1 = Encoding.ASCII.GetString(buf, 0, n);
                buffer.RemoveRange(0, n);

                //
                messagex = f1.danalysis(buffer1, DateTime.Now.ToString());

                //
                var data2 = messagex[2].Split(new char[1] { ';' });
                for (i = 0; i < data2.Count(); i++)
                {
                    var data3 = data2[i].Split(new char[2] { ':', ',' });
                    for (int j = 1; j < data3.Count() - 1; j++)
                    {
                        // 
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

                        f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][0] = f1.mysqll.Tables[0].Rows[0][0];    //id 
                        f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][0] = f1.mysqll.Tables[0].Rows[0][0];    //id
                        f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][1] = f1.mysqll.Tables[0].Rows[0][1];    //tid
                        f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][1] = f1.mysqll.Tables[0].Rows[0][1];    //tid
                        f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][0] = f1.mysqll.Tables[0].Rows[0][1];  //TID 
                        f1.mysql2.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][0] = f1.mysqll.Tables[0].Rows[0][1].ToString().Replace("00000", "*");  //TID 
                        f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][2] = "borrow";  //出库    
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
                        f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][7] = f1.mysqll.Tables[0].Rows[0][7];  //
                        f1.openlist.Add(f1.comm3);
                        f1.closelist.Add(f1.mysqll.Tables[0].Rows[0][5].ToString());
                        f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());  //
                        f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                        f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][2] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                        f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][8] = f1.username;        //
                        f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][8] = f1.username;
                        f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 2][3] = f1.username;        //
                    }
                }
                var str4 = f1.mysqll.Tables[0].Rows[0][5].ToString().Split(new char[1] { '-' });
                f1.num = int.Parse(str4[0]);
                f1.box = int.Parse(str4[1]);
                f1.danalysis1(f1.num, f1.box);
                f1.num1 = f1.num;
                f1.box1 = f1.box;
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
                string passkey=jiema(passkey1);
                string codestr = f1.des_string(buffer1, passkey);

                if (codestr[0] == '%')         //
                {

                    var str_1 = codestr.Split(new char[1] { '%' });
                    var str_2 = str_1[3].Split(new char[1] { ',' });
                    string idcard=str_2[0];
                    //
                    if (str_1[1] != "O1")
                    {
                        MessageBox.Show("无效二维码，请扫描借阅二维码！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);  //此代码在归还入库时有效
                        return;
                    }
                    //
                    if (timestoend.time_hours(str_1[2], DateTime.Now.ToString())>24)
                    {
                        MessageBox.Show("二维码已过期，请在申请二维码24小时内操作！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);   //二维码有效性判断
                        return;
                    }
                    //
                    textbox5.Text = idcard;

                    //
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

        public System.Timers.Timer time_gatecheck = new System.Timers.Timer();
        //
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                //1
                if (f1.pdjsytatu == false)
                {
                    MessageBox.Show("请检查移动盘点机网线连接！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    return;
                }
                //mysqlmysql3
                if (!f1.check_pand()) {
                    return;
                } 
                //
                data_refresh();
                textBox9.AppendText(DateTime.Now.ToString() + "  "+ (f1.mysql.Tables[0].Rows.Count).ToString()+"");
                textBox9.AppendText("\r\n"); //
                //
                speech.SpeakAsync("" + (f1.mysql.Tables[0].Rows.Count).ToString() + "");

                //2
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

        //
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
                f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][0] = f1.mysqll.Tables[0].Rows[0][0];   //id 
                f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][0] = f1.mysqll.Tables[0].Rows[0][0];  //id                                                                                                                     
                f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][1] = f1.mysqll.Tables[0].Rows[0][1];   //tid
                f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][1] = f1.mysqll.Tables[0].Rows[0][1];  //tid
                f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][0] = f1.mysqll.Tables[0].Rows[0][1];  //TID 
                textBox9.AppendText(DateTime.Now.ToString() + "  RFID  "+f1.mysqll.Tables[0].Rows[0][1]);   //RFID
                textBox9.AppendText("\r\n"); //
                f1.openlist.Add(f1.comm3);
                f1.closelist.Add(f1.mysqll.Tables[0].Rows[0][5].ToString());
                f1.mysql2.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][0] = f1.mysqll.Tables[0].Rows[0][1].ToString().Replace("00000", "*");  //TID 
                f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][2] = "borrow";  //出库    
                f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][2] = f1.mysqll.Tables[0].Rows[0][2];  //status1
                f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][3] = f1.mysqll.Tables[0].Rows[0][3];  // 
                f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][3] = f1.mysqll.Tables[0].Rows[0][3];  //                                                                                                                                                
                f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][4] = f1.mysqll.Tables[0].Rows[0][4];  //perid 
                f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][4] = f1.mysqll.Tables[0].Rows[0][4];  //perid 
                f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][1] = f1.mysqll.Tables[0].Rows[0][4];  //perid 
                f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][5] = f1.mysqll.Tables[0].Rows[0][5];  //,position
                f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][5] = f1.mysqll.Tables[0].Rows[0][5];  //,position
                f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][4] = f1.mysqll.Tables[0].Rows[0][5];  //+
                f1.mysql2.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][1] = f1.mysqll.Tables[0].Rows[0][5];  //+ 
                f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());  //
                f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][6] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][2] = new MySql.Data.Types.MySqlDateTime(DateTime.Now.ToString());
                f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][7] = f1.mysqll.Tables[0].Rows[0][7];        //
                f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][7] = f1.mysqll.Tables[0].Rows[0][7];       //
                f1.mysql.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][8] = f1.username;        //
                f1.mysql4.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][8] = f1.username;
                f1.mysql5.Tables[0].Rows[f1.mysql.Tables[0].Rows.Count - 1][3] = f1.username;        //
            }
        }

        //
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

        //
        private delegate void doorid();
        public List<string> cardlist1 = new List<string>();  //
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
                            //
                            f1.borrow_save = true;
                            int count = f1.mysql.Tables[0].Rows.Count;
                            for (int i = 0; i < count; i++)
                            {
                                f1.savedate();
                                textBox10.AppendText(DateTime.Now.ToString() + "  RFID  " + f1.mysql.Tables[0].Rows[0][1] + "");
                                textBox10.AppendText("\r\n"); //
                                f1.mysql.Tables[0].Rows.RemoveAt(0); //status0 
                                f1.mysql4.Tables[0].Rows.RemoveAt(0);
                            }
                            //
                            f1.produceqr();
                            //
                            closeinit();
                            //
                            f1.checkout = null;
                            f1.borrow_checkout = false;
                            f1.borrow_save = false;
                            //
                            f1.gate_cleardata();
                            //
                            //
                            //speech.SpeakAsync("");
                            //textBox3.AppendText("\r\n"); //
                            //textBox10.AppendText(DateTime.Now.ToString() + "  ");
                            //textBox10.AppendText("\r\n"); //
                            speech.SpeakAsync("");
                            textBox10.AppendText("\r\n"); //
                            textBox10.AppendText(DateTime.Now.ToString() + "");
                            textBox10.AppendText("\r\n"); //
                            //7
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
                                        speech.SpeakAsync(cardlist1.Count - ranf.Count + "");
                                        spcoun++;
                                        f1.Buzzer((byte)1, (byte)1, (byte)10, (byte)1, (byte)1, (byte)10);
                                    }
                                    textBox10.AppendText(DateTime.Now.ToString() + "  RFID  " + cardlist1[i] + "");
                                    textBox10.AppendText("\r\n"); //                                    
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
                                        speech.SpeakAsync(f1.mysql3.Tables[0].Rows.Count - cardlist1.Count + "");
                                        spcoun++;
                                        f1.Buzzer((byte)1, (byte)1, (byte)10, (byte)1, (byte)1, (byte)10);
                                    }
                                    textBox10.AppendText(DateTime.Now.ToString() + "  RFID  " + f1.mysql3.Tables[0].Rows[i][1] + "");
                                    textBox10.AppendText("\r\n"); //
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
