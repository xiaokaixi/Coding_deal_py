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
    public partial class form_transfer : Form
    {
        #region  
        public bool ok_flg = false;
        public List<string> perid_listcheck = new List<string>();
        public List<string> str1 = new List<string>();
        string unitname,name,idcard,transtype,tounitname,transreason;
        string peridlist;
        #endregion
        public form_transfer(string unitname1,string name1,string idcard1,string transtype1,string tounitname1,string transreason1,string peridlist1)
        {
            InitializeComponent();
            unitname = unitname1;
            name = name1;
            idcard = idcard1;
            transtype = transtype1;
            tounitname = tounitname1;
            transreason = transreason1;
            peridlist = peridlist1;
        }

        private void form_transfer_Load(object sender, EventArgs e)
        {
            textBox2.Text = unitname;
            textBox1.Text = name;
            textBox3.Text = idcard;
            textBox4.Text = transtype;
            textBox5.Text = tounitname;
            textBox6.Text = transreason;
            var str = peridlist.Split(new char[1] { '*' });
            for (int i=1;i<str.Count()-1;i++)
            {
                str1.Add(str[i]);
                str[i]=str[i].Replace("FFFFFFFFFFFFFFFF", "**");
                checkedListBox2.Items.Add(str[i]);
            }
            if (textBox4.Text == "全部转出")
            {
                for (int i = 0; i < checkedListBox2.Items.Count; i++)
                {
                    checkedListBox2.SetItemChecked(i,true);
                }
            }
        }

        #region  
        private void button1_Click(object sender, EventArgs e)
        {
            ok_flg = false;
            this.Close();
        }

        #endregion

        #region   
        private void button6_Click(object sender, EventArgs e)
        {
            if (checkedListBox2.CheckedItems.Count == 0)
            {
                MessageBox.Show("请选择转递档案！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("请再次确认是否转递该干部档案，确认后系统会根据转递类型对档案进行移除！", "Question", MessageBoxButtons.OK, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                str1.Sort();
                for (int i = 0; i < checkedListBox2.Items.Count; i++)
                {
                    if (checkedListBox2.GetItemChecked(i))
                    {
                        perid_listcheck.Add(str1[i]);
                    }   
                }
                ok_flg = true;
                this.Close();
            }
            else
            {
                ok_flg = false;
                this.Close();
            }
        }
        #endregion
    }
}
