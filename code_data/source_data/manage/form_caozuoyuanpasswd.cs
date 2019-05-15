using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Speech;
using System.Speech.Synthesis;

namespace manage
{

    public partial class form_caozuoyuanpasswd : Form
    {

        #region  全局变量
        public passwdtext check = new passwdtext();
        public string dirpath = Application.StartupPath + "\\FreeVK.exe";  //获取应用程序路径
        public dynamic file;
        public Process p1;
        public firtdoor f1;
        public int controlnum;
        public int handletype;
        #endregion
        public form_caozuoyuanpasswd(firtdoor f, int controlnum1,int handletype1)
        {
            InitializeComponent();
            f1 = f;
            controlnum = controlnum1;
            handletype = handletype1;
        }
       
        private void Form6_Load(object sender, EventArgs e)
        {
            List<string> list = new List<string>();            
            try
            {
                //打开虚拟键盘
                //file = dirpath;
                //if (!System.IO.File.Exists(file))
                //    return;
                //p1 = Process.Start(file);
                //查询用户名
                list = check.logincheckset_contin("c");  //温度
                for (int i = 0; i < list.Count; i++)
                {
                    comboBox1.Items.Add(list[i]);
                }
                textBox1.PasswordChar = '*';
            }
            catch (Exception)
            {
                int i = 0;
            }
        }

        //确定按钮
        public SpeechSynthesizer speech;
        public bool rightorwrang = false;
        private void button17_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.ToString() == ""|| comboBox1.Text.ToString() == "")
            {
                MessageBox.Show("请输入编号和密码！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    if (handletype == 1)
                    {
                        passwdtext check = new passwdtext();
                        if (check.logincheck(comboBox1.Text, textBox1.Text.ToString()))
                        {
                            Hide();
                            //p1.CloseMainWindow();
                            this.Close();
                            form_shizhurenpasswd szr = new form_shizhurenpasswd(f1, controlnum,null);
                            szr.Text = "紧急开锁认证-室主任权限";
                            szr.ShowDialog();
                            rightorwrang = szr.rightorwrang;

                            //Hide();
                            //p1.CloseMainWindow();
                            //this.Close();
                            //form_shizhurenpasswd fszr = new form_shizhurenpasswd(f1, controlnum);
                            //fszr.ShowDialog();

                            //speech = new SpeechSynthesizer();
                            //speech.Volume = 100;
                            //speech.Rate = 3;
                            //speech.Speak("请确定紧急开锁？紧急开锁后柜门将依次全部打开！");
                            //speech.SpeakAsyncCancelAll();
                            //speech.Dispose();
                            //DialogResult result = MessageBox.Show("确定紧急开锁？", "Question", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            //if (result == DialogResult.OK)
                            //{
                            //    //speech.Speak("正在开锁，请稍等！");
                            //    //speech.SpeakAsyncCancelAll();
                            //    //speech.Dispose();
                            //    //紧急依次全开
                            //    for (int i = 1; i <17; i++)
                            //    {
                            //        f1.urgentopen(i.ToString());
                            //    }
                            //    //speech.Speak("紧急开锁完成！");
                            //    //speech.SpeakAsyncCancelAll();
                            //    //speech.Dispose();
                            //    MessageBox.Show("紧急开锁完成！", "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
                            //}
                            //else
                            //{
                            //    return;
                            //}
                        }
                        else
                        {
                            MessageBox.Show("请检查密码！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            textBox1.Text = "";
                            textBox1.Focus();
                        }
                    }
                    if (handletype == 2)
                    {
                        passwdtext check = new passwdtext();
                        if (check.logincheck("caozuoyuan", textBox1.Text.ToString()))
                        {
                            Hide();
                            p1.CloseMainWindow();
                            this.Close();                          
                        }
                        else
                        {
                            MessageBox.Show("请检查密码！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            textBox1.Text = "";
                            textBox1.Focus();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        //取消按钮
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = "";
                p1.CloseMainWindow();
                handletype = 0;
                this.Close();
            }
            catch
            {
                textBox1.Text = "";
                handletype = 0;
                this.Close();
            }
        }
    }
}
