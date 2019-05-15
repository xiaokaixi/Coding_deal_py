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
    public partial class form_guanlingdaopasswd : Form
    {

        #region  
        public passwdtext check = new passwdtext();
        public string dirpath = Application.StartupPath + "\\FreeVK.exe";  //
        public dynamic file;
        public Process p1;
        public firtdoor f1;
        public int controlnum;
        public bool rightorwrang=false;
        List<string> positionlist = new List<string>();
        #endregion

        public form_guanlingdaopasswd(firtdoor f,int controlnum1,List<string> positionlist1)
        {
            InitializeComponent();
            f1 = f;
            controlnum = controlnum1;
            positionlist = positionlist1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void form_guanlingdao_Load(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            try
            {
                //
                //file = dirpath;
                //if (!System.IO.File.Exists(file))
                //    return;
                //p1 = Process.Start(file);
                //
                if (positionlist == null)
                {
                    list = check.logincheckset_contin("g");  //
                    for (int i = 0; i < list.Count; i++)
                    {
                        comboBox1.Items.Add(list[i]);
                    }
                    
                }
                else
                {
                    label2.Text = "账户编号：";
                    label1.Text = "登录密码：";
                    list = check.logincheckset_contin("c");  //
                    for (int i = 0; i < list.Count; i++)
                    {
                        comboBox1.Items.Add(list[i]);
                    }
                    list = check.logincheckset_contin("s");  //
                    for (int i = 0; i < list.Count; i++)
                    {
                        comboBox1.Items.Add(list[i]);
                    }
                    list = check.logincheckset_contin("g");  //
                    for (int i = 0; i < list.Count; i++)
                    {
                        comboBox1.Items.Add(list[i]);
                    }
                    comboBox1.Items.Remove(positionlist[positionlist.Count - 1]);
                }
                textBox1.PasswordChar = '*';
                //
                speech = new SpeechSynthesizer();
                speech.Volume = 100;
                speech.Rate = 1;

            }
            catch (Exception)
            {
                int i = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        public SpeechSynthesizer speech;
        private void button17_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.ToString() == "")
            {
                MessageBox.Show("请输入密码！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    passwdtext check = new passwdtext();
                    if (check.logincheck(comboBox1.Text, textBox1.Text.ToString()))
                    {
                        if (controlnum != 0)
                        {
                            Hide();
                            //p1.CloseMainWindow();
                            this.Close();
                            speech.SpeakAsync("");
                            DialogResult result = MessageBox.Show("确定紧急开锁？", "紧急开锁", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            rightorwrang = true;
                            if (result == DialogResult.OK)
                            {
                                f1.t.Stop();
                                //
                                Thread.Sleep(200);
                                for (int i = 1; i < controlnum + 1; i++)
                                {
                                    f1.urgentopen(i.ToString());
                                }
                                f1.killtxt = "urgopen";
                                f1.startkill();
                                MessageBox.Show("紧急开锁完成！", "urgopen", MessageBoxButtons.OK, MessageBoxIcon.None);
                                f1.t.Start();
                            }
                            else
                            {
                                return;
                            }
                        }
                        else if (positionlist != null)
                        {
                            positionlist.RemoveAt(positionlist.Count - 1);
                            Hide();
                            this.Close();
                            rightorwrang = true;
                            try
                            {
                                //
                                //
                                DialogResult result = MessageBox.Show("确定开锁？", "开锁确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                                if (result == DialogResult.OK)
                                {
                                    positionlist.Sort();
                                    f1.t.Stop();
                                    //
                                    for (int i = 0; i < positionlist.Count; i++)
                                    {
                                        var fenl = positionlist[i].Split(new char[1] { '-' });
                                        int num = int.Parse(fenl[0]);
                                        int box = int.Parse(fenl[1]);
                                        f1.danalysis1(num, box);
                                        f1.contorldoor();
                                    }
                                    f1.t.Start();
                                    f1.killtxt = "urgopen";
                                    f1.startkill();
                                    MessageBox.Show("开锁完成！", "urgopen", MessageBoxButtons.OK, MessageBoxIcon.None);
                                }
                                else
                                {
                                    return;
                                }
                                //rightorwrang = true;
                                //Hide();
                                //p1.CloseMainWindow();
                                //speech.Dispose();
                                //this.Close();
                            }
                            catch
                            {
                                rightorwrang = false;
                                Hide();
                                speech.Dispose();
                                this.Close();
                                return;
                            }
                        }
                        else
                        {
                            rightorwrang = true;
                            Hide();
                            speech.Dispose();
                            this.Close();
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("请检查密码！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        textBox1.Text = "";
                        textBox1.Focus();
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = "";
                p1.CloseMainWindow();
                rightorwrang = false;
                speech.Dispose();
                this.Close();
            }
            catch
            {
                textBox1.Text = "";
                rightorwrang = false;
                speech.Dispose();
                this.Close();
            }
        }
    }
}
