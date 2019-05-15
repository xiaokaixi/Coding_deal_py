using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialPort_ViewSWUST1205
{
    public partial class form_seletsave : Form
    {
        //
        public bool select_rw = false;
        public List<string> namelist = new List<string>();
        Form1 f1;

        public form_seletsave(Form1 f11)
        {
            InitializeComponent();
            f1 = f11;
        }

        private void form_seletsave_Load(object sender, EventArgs e)
        {
            proinit();
        }


        //
        private void proinit()
        {
            int k = 0;
            namelist.Clear();
            for (int m = 0; m < f1.list_savetable.Count; m++)
            {
                try
                {
                    if (namelist.IndexOf(f1.savenamestr[m].Substring(0, 6)) < 0)
                    {
                        namelist.Add(f1.savenamestr[m].Substring(0, 6));
                        checkedListBox1.Items.Add(namelist[k]);
                        k++;
                    }
                }
                catch
                { }
            }  
        }

        //
        private void button1_Click(object sender, EventArgs e)
        {
            List<string> indexlist = new List<string>();
            List<DataTable> datablelist = new List<DataTable>();
            int k = 0;
            try
            {
                if (checkedListBox1.CheckedItems.Count == 0)
                {
                    MessageBox.Show("请选择导出数据时间！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (checkedListBox1.GetItemChecked(i))
                    {
                        for (int j = 0; j < f1.savenamestr.Count; j++)
                        {
                            if (f1.savenamestr[j].IndexOf(namelist[i]) >= 0)
                            {
                                indexlist.Add(f1.savenamestr[j]);
                                datablelist.Add(f1.list_savetable[j]);
                            }
                        }


                    }
                }
                f1.savenamestr.Clear();
                f1.savenamestr=indexlist;
                f1.list_savetable.Clear();
                f1.list_savetable=datablelist;
                select_rw = true;
                this.Close();
            }
            catch
            {
                return;
            }
        }

        //
        private void button2_Click(object sender, EventArgs e)
        {
            select_rw = false;
            this.Close();
        }
    }
}
