using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace manage
{
    public partial class form_login : Form
    {
        #region  全局变量
        public passwdtext check = new passwdtext();
        public string username = "";
        #endregion 
        public form_login()
        {
            InitializeComponent();
        }

        private void form_login_Load(object sender, EventArgs e)
        {
            perinit();
            timenow();
        }

        #region  初始化
        private void perinit()
        {
            List<string> list = new List<string>();
            try
            {
                list = check.logincheckset_contin("c");  //温度
                for (int i = 0; i < list.Count; i++)
                {
                    comboBox1.Items.Add(list[i]);
                }
                textBox2.PasswordChar = '*';
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region  确定登录
        private void button1_Click(object sender, EventArgs e)
        {
            if (check.logincheck(comboBox1.Text, textBox2.Text.ToString()))
            {
                username = comboBox1.Text;
                this.Close();
            }
            else
            {
                MessageBox.Show("请检查用户名和密码！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Text = "";
                textBox2.Focus();
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
                }
            }
            catch
            { }
        }
        #endregion
    }
}
