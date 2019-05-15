using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Speech;  //语音朗读
using System.Speech.Synthesis;

namespace manage
{
    public partial class form_Loading : Form
    {
        public firtdoor f1;
        public System.Timers.Timer t5 = new System.Timers.Timer();
        public static Thread DecodeThread = null;
        delegate void datagrid1dill(form_Loading f_loading);
        public IntPtr han;
        public SpeechSynthesizer speech=new SpeechSynthesizer();
        
        public form_Loading(firtdoor f11)
        {
            //this.TopMost = true;
            InitializeComponent();
            f1 = f11;
            mm();
            //语音朗读
            speech.Volume = 100;
            speech.Rate = 0;
            speech.SpeakAsync("软件正在初始化，请稍后！");
        }

        bool bIsLoop = false;
        private void mm()
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
            datagrid1dill datagrid1dillRef_Instance = new datagrid1dill(mm1);          
            do
            {
                try
                {
                    this.Invoke(datagrid1dillRef_Instance, new object[] { this });
                }
                catch
                { }
            }
            while (bIsLoop);
        }

        /// <summary>
        /// loading窗体引导
        /// </summary>
        bool flg = false;
        private void mm1(form_Loading f_loading)
        {
            try
            {
                if (flg)
                { return; }
                flg = true;
                int m = 0,m1=0;
                while (true)
                {
                    Thread.Sleep(100);
                    if (m <= 100)
                    {
                        progressBar1.Value = m++;
                    }
                    if (progressBar1.Value == 100)
                    {
                        speech.SpeakAsync("欢迎使用！");
                        t5.Stop();
                        break;
                    }
                }
                f_loading.Close();
                flg = false;
            }
            catch
            { }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
