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
    public partial class form_devicestatus : Form
    {
        public bool dataserver = false, monitorpc = false, gatedoorstatus = false, controlboard = false, RFIDread = false, pdjsytatu = false, qrdevicestatu = false, printstatu = false;
        public bool refresh = false, close = false;
       
        public form_devicestatus(bool dataserver1, bool monitorpc1, bool gatedoorstatus1, bool controlboard1, bool RFIDread1,bool pdjsytatu1, bool qrdevicestatu1, bool printstatu1)
        {
            InitializeComponent();
            dataserver = dataserver1;
            monitorpc = monitorpc1;
            gatedoorstatus = gatedoorstatus1;
            controlboard = controlboard1;
            RFIDread = RFIDread1;
            pdjsytatu = pdjsytatu1;
            qrdevicestatu = qrdevicestatu1;
            printstatu = printstatu1;
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            if (dataserver == true)
            {
                button5.BackgroundImage = Properties.Resources.sql1;
                progressBar1.Value = 100;
            }
            else
            {
                button5.BackgroundImage = Properties.Resources.sql2;
                progressBar1.Value = 0;
            }
            if (monitorpc == true)
            {
                button1.BackgroundImage = Properties.Resources.devlink1;
                progressBar6.Value = 100;
            }
            else
            {
                button1.BackgroundImage = Properties.Resources.devlink2;
                progressBar6.Value = 0;
            }
            if (gatedoorstatus == true)
            {
                button7.BackgroundImage = Properties.Resources.devlink1;
                progressBar3.Value = 100;
            }
            else
            {
                button7.BackgroundImage = Properties.Resources.devlink2;
                progressBar3.Value = 0;
            }
            if (controlboard == true)
            {
                button2.BackgroundImage = Properties.Resources.devlink1;
                progressBar8.Value = 100;
            }
            else
            {
                button2.BackgroundImage = Properties.Resources.devlink2;
                progressBar8.Value = 0;
            }
            if (RFIDread == true)
            {
                button6.BackgroundImage = Properties.Resources.devlink1;
                progressBar2.Value = 100;
            }
            else
            {
                button6.BackgroundImage = Properties.Resources.devlink2;
                progressBar2.Value = 0;
            }
            if (pdjsytatu == true)
            {
                button11.BackgroundImage = Properties.Resources.devlink1;
                progressBar7.Value = 100;
            }
            else
            {
                button11.BackgroundImage = Properties.Resources.devlink2;
                progressBar7.Value = 0;
            }
            if (qrdevicestatu == true)
            {
                button8.BackgroundImage = Properties.Resources.devlink1;
                progressBar4.Value = 100;
            }
            else
            {
                button8.BackgroundImage = Properties.Resources.devlink2;
                progressBar4.Value = 0;
            }
            if (printstatu == true)
            {
                button9.BackgroundImage = Properties.Resources.devlink1;
                progressBar5.Value = 100;
            }
            else
            {
                button9.BackgroundImage = Properties.Resources.devlink2;
                progressBar5.Value = 0;
            }
        }

        //刷新
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            refresh = true;
        }

        //关闭
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
            close = true;
        }

    }
}
