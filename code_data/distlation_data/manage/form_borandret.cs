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
    public partial class form_borandret : Form
    {
        public DataSet mysql_borrow, mysql_return;
        public DataSet mysql_borrow1 = new DataSet();
        public DataSet mysql_return1 = new DataSet();
        public string type;
        public form_borandret(DataSet mysql_borrow1,DataSet mysql_return1,string typestr)
        {
            InitializeComponent();
            mysql_borrow = mysql_borrow1;
            mysql_return = mysql_return1;
            type = typestr;
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void form_borandret_Load(object sender, EventArgs e)
        {
            //
            preinit();
            this.comboBox3.SelectedIndexChanged += new System.EventHandler(comboBox3_selectchanged);           
            switch (type)
            {
                case "borrowandreturn":
                    comboBox3.Items.Clear();
                    comboBox3.Items.Add("");
                    comboBox3.Items.Add("");
                    comboBox3.SelectedIndex = 0;
                    label4.Enabled = false;
                    comboBox4.Items.Clear();
                    comboBox4.Enabled = false;                                       
                    dataGridView1.DataSource = mysql_borrow.Tables[0];
                    dataGridView1.Columns[0].Width = 50;
                    dataGridView1.Columns[1].Width = 200;
                    for (int i = 0; i < mysql_borrow.Tables[0].Rows.Count; i++)
                    {
                        var mm = mysql_borrow.Tables[0].Rows[i][6].ToString().Split(new char[1] { ' ' });
                        int m = comboBox1.Items.IndexOf(mm[0]);
                        if ((comboBox1.Items.IndexOf(mm[0])) >= 0)
                        { continue; }
                        comboBox1.Items.Add(mm[0]);
                        if ((comboBox2.Items.IndexOf(mm[0])) >= 0)
                        { continue; }
                        comboBox2.Items.Add(mm[0]);
                    }
                    comboBox1.SelectedIndex = 0;
                    comboBox2.SelectedIndex = comboBox2.Items.Count - 1;
                    //
                    mysql_borrow1.Tables.Add();
                    for (int i = 0; i < mysql_borrow.Tables[0].Columns.Count; i++)
                    {
                        mysql_borrow1.Tables[0].Columns.Add();
                        mysql_borrow1.Tables[0].Columns[i].ColumnName = mysql_borrow.Tables[0].Columns[i].ColumnName;
                    }
                    //
                    mysql_return1.Tables.Add();
                    for (int i = 0; i < mysql_borrow.Tables[0].Columns.Count; i++)
                    {
                        mysql_return1.Tables[0].Columns.Add();
                        mysql_return1.Tables[0].Columns[i].ColumnName = mysql_return.Tables[0].Columns[i].ColumnName;
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void preinit()
        {
            try
            {
                for (int i = 0; i < mysql_borrow.Tables[0].Rows.Count; i++)
                {
                    mysql_borrow.Tables[0].Rows[i][2] = "借 阅";
                }
                for (int i = 0; i < mysql_return.Tables[0].Rows.Count; i++)
                {
                    mysql_return.Tables[0].Rows[i][2] = "归 还";
                }
            }
            catch
            { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public bool startime = false;
        private void button1_Click(object sender, EventArgs e)
        {
            int k = 0;
            mysql_borrow1.Tables[0].Rows.Clear();
            mysql_return1.Tables[0].Rows.Clear();
            //
            if (comboBox3.SelectedIndex == 0)
            {
                for (int i = 0; i < mysql_borrow.Tables[0].Rows.Count; i++)
                {
                    //
                    if ((mysql_borrow.Tables[0].Rows[i][6].ToString().IndexOf(comboBox2.Text.ToString())) >= 0)
                    {
                        startime = false;
                        mysql_borrow1.Tables[0].Rows.Add();
                        for (int j = 0; j < mysql_borrow.Tables[0].Columns.Count; j++)
                        {
                            mysql_borrow1.Tables[0].Rows[k][j] = mysql_borrow.Tables[0].Rows[i][j];
                        }
                        k++;
                        if (i + 1 == mysql_borrow.Tables[0].Rows.Count)
                        {
                            continue;
                        }
                        if ((mysql_borrow.Tables[0].Rows[i + 1][6].ToString().IndexOf(comboBox2.Text.ToString())) < 0)
                        {
                            i = mysql_borrow.Tables[0].Rows.Count;
                        }
                        continue;
                    }
                    //
                    if ((mysql_borrow.Tables[0].Rows[i][6].ToString().IndexOf(comboBox1.Text.ToString())) >= 0 || startime == true)
                    {
                        startime = true;
                        mysql_borrow1.Tables[0].Rows.Add();
                        for (int j = 0; j < mysql_borrow.Tables[0].Columns.Count; j++)
                        {
                            mysql_borrow1.Tables[0].Rows[k][j] = mysql_borrow.Tables[0].Rows[i][j];
                        }
                        k++;
                    }
                }
                dataGridView1.DataSource = mysql_borrow1.Tables[0];
            }
            //
            if (comboBox3.SelectedIndex == 1)
            {
                for (int i = 0; i < mysql_return.Tables[0].Rows.Count; i++)
                {
                    //
                    if ((mysql_return.Tables[0].Rows[i][6].ToString().IndexOf(comboBox2.Text.ToString())) >= 0)
                    {
                        startime = false;
                        mysql_return1.Tables[0].Rows.Add();
                        for (int j = 0; j < mysql_return.Tables[0].Columns.Count; j++)
                        {
                            mysql_return1.Tables[0].Rows[k][j] = mysql_return.Tables[0].Rows[i][j];
                        }
                        k++;
                        if (i + 1 == mysql_return.Tables[0].Rows.Count)
                        {
                            continue;
                        }
                        if ((mysql_return.Tables[0].Rows[i + 1][6].ToString().IndexOf(comboBox2.Text.ToString())) < 0)
                        {
                            i = mysql_return.Tables[0].Rows.Count;
                        }
                        continue;
                    }
                    //
                    if ((mysql_return.Tables[0].Rows[i][6].ToString().IndexOf(comboBox1.Text.ToString())) >= 0 || startime == true)
                    {
                        startime = true;
                        mysql_return1.Tables[0].Rows.Add();
                        for (int j = 0; j < mysql_return.Tables[0].Columns.Count; j++)
                        {
                            mysql_return1.Tables[0].Rows[k][j] = mysql_return.Tables[0].Rows[i][j];
                        }
                        k++;
                    }
                }
                dataGridView1.DataSource = mysql_return1.Tables[0];
            }
            
        }

        /// <summary>
        /// comboBox3
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox3_selectchanged(object sender, EventArgs e)
        {
            //
            if (comboBox3.SelectedIndex == 0)
            {
                for (int i = 0; i < mysql_borrow.Tables[0].Rows.Count; i++)
                {
                    var mm = mysql_borrow.Tables[0].Rows[i][6].ToString().Split(new char[1] { ' ' });
                    if ((comboBox1.Items.IndexOf(mm[0])) >= 0)
                    { continue; }
                    comboBox1.Items.Add(mm[0]);
                    if ((comboBox2.Items.IndexOf(mm[0])) >= 0)
                    { continue; }
                    comboBox2.Items.Add(mm[0]);
                }
                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = comboBox2.Items.Count - 1;
                dataGridView1.DataSource = mysql_borrow.Tables[0];
            }
            //
            if (comboBox3.SelectedIndex == 1)
            {
                for (int i = 0; i < mysql_return.Tables[0].Rows.Count; i++)
                {
                    var mm = mysql_return.Tables[0].Rows[i][6].ToString().Split(new char[1] { ' ' });
                    if ((comboBox1.Items.IndexOf(mm[0])) >= 0)
                    { continue; }
                    comboBox1.Items.Add(mm[0]);
                    if ((comboBox2.Items.IndexOf(mm[0])) >= 0)
                    { continue; }
                    comboBox2.Items.Add(mm[0]);
                }
                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = comboBox2.Items.Count - 1;
                dataGridView1.DataSource = mysql_return.Tables[0];
            }
        }


    }
}
