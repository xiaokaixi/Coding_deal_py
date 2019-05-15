using System;  //

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

    public partial class form_everonmentview : Form

    {

        public firtdoor f1;

        public DataSet mysql_1=new DataSet();

        public DataSet mysql_2=new DataSet();

        public form_everonmentview(firtdoor ff1)

        {

            InitializeComponent();

            f1 = ff1;

        }



        private void form_everonmentview_Load(object sender, EventArgs e)

        {

            mysql_1.Tables.Add();

            mysql_1.Tables[0].Columns.Add();

            mysql_1.Tables[0].Columns[0].ColumnName = "传感器类型";

            mysql_1.Tables[0].Columns.Add();

            mysql_1.Tables[0].Columns[1].ColumnName = "数 值";

            mysql_1.Tables[0].Columns.Add();

            mysql_1.Tables[0].Columns[2].ColumnName = "正常范围";

            mysql_1.Tables[0].Columns.Add();

            mysql_1.Tables[0].Columns[3].ColumnName = "状 态";

            mysql_1.Tables[0].Columns.Add();

            mysql_1.Tables[0].Columns[4].ColumnName = "安装位置";

            mysql_1.Tables[0].Rows.Add();

            mysql_1.Tables[0].Rows.Add();

            mysql_1.Tables[0].Rows.Add();

            mysql_1.Tables[0].Rows[0][0] = "温度传感器";

            mysql_1.Tables[0].Rows[1][0] = "湿度传感器";

            mysql_1.Tables[0].Rows[2][0] = "浸水传感器";

            mysql_1.Tables[0].Rows[0][4] = "1号柜体";

            mysql_1.Tables[0].Rows[1][4] = "1号柜体";

            mysql_1.Tables[0].Rows[2][4] = "10号柜体";

            dataGridView1.DataSource = mysql_1.Tables[0];

            dataGridView1.ClearSelection();

            mysql_2.Tables.Add();

            mysql_2.Tables[0].Columns.Add();

            mysql_2.Tables[0].Columns[0].ColumnName = "传感器类型";

            mysql_2.Tables[0].Columns.Add();

            mysql_2.Tables[0].Columns[1].ColumnName = "数 值";

            mysql_2.Tables[0].Columns.Add();

            mysql_2.Tables[0].Columns[2].ColumnName = "正常范围";

            mysql_2.Tables[0].Columns.Add();

            mysql_2.Tables[0].Columns[3].ColumnName = "状 态";

            mysql_2.Tables[0].Columns.Add();

            mysql_2.Tables[0].Columns[4].ColumnName = "安装位置";

            mysql_2.Tables[0].Rows.Add();

            mysql_2.Tables[0].Rows.Add();

            mysql_2.Tables[0].Rows.Add();

            mysql_2.Tables[0].Rows[0][0] = "温度传感器";

            mysql_2.Tables[0].Rows[1][0] = "湿度传感器";

            //mysql_2.Tables[0].Rows[2][0] = "进水传感器";

            mysql_2.Tables[0].Rows[0][4] = "16号柜体";

            mysql_2.Tables[0].Rows[1][4] = "16号柜体";

            //mysql_2.Tables[0].Rows[2][4] = "";

            dataGridView2.DataSource = mysql_2.Tables[0];

            dataGridView2.ClearSelection();



            System.Timers.Timer t = new System.Timers.Timer(500);       //500ms

            t.Elapsed += new System.Timers.ElapsedEventHandler(theoutime);

            t.AutoReset = true;  //true,false

            t.Enabled = true;

        }



       /// <summary>

       /// 

       /// </summary>

       /// <param name="source"></param>

       /// <param name="e"></param>

        public void theoutime(object source, System.Timers.ElapsedEventArgs e)

        {

            SetData1();

        }



        //,

        private delegate void SetDataDelegate1();

        private void SetData1()

        {

            if (this.InvokeRequired)

            {

                this.Invoke(new SetDataDelegate1(SetData1));

            }

            else

            {

                //101

                mysql_1.Tables[0].Rows[0][1] = (float.Parse(f1.numlist1[0]) - 2.5).ToString("#0.0"); //  

                mysql_1.Tables[0].Rows[0][2] = f1.templ+ "°C" + "-"+ f1.temph+ "°C"; //

                if ((float.Parse(f1.numlist1[0]) - 2.5) >= float.Parse(f1.templ) && (float.Parse(f1.numlist1[0]) - 2.5) <= float.Parse(f1.temph))

                {

                    mysql_1.Tables[0].Rows[0][3] = "正 常"; //状态

                }

                else

                {

                    mysql_1.Tables[0].Rows[0][3] = "异 常"; //状态

                }

                //

                mysql_1.Tables[0].Rows[1][1] = (float.Parse(f1.numlist2[0]) + 1.9).ToString("#0.0"); //  

                mysql_1.Tables[0].Rows[1][2] = f1.wetl + "%" + "-" + f1.weth + "%"; //

                if ((float.Parse(f1.numlist2[0]) + 1.9) >= float.Parse(f1.wetl)&&(float.Parse(f1.numlist2[0]) + 1.9) <= float.Parse(f1.weth))

                {

                    mysql_1.Tables[0].Rows[1][3] = "正 常"; //状态

                }

                else

                {

                    mysql_1.Tables[0].Rows[1][3] = "异 常"; //状态

                }

                

                //

                if (f1.textBox3.Text == "正 常")

                {

                    mysql_1.Tables[0].Rows[2][1] = "1"; //数值

                    mysql_1.Tables[0].Rows[2][3] = "正 常"; //状态

                }

                else

                {

                    mysql_1.Tables[0].Rows[2][1] = "0"; //数值

                    mysql_1.Tables[0].Rows[2][3] = "浸 水"; //状态

                }

                mysql_1.Tables[0].Rows[2][2] = "1"; //正常范围



                //102

                mysql_2.Tables[0].Rows[0][1] = (float.Parse(f1.numlist1[1]) - 3.6).ToString("#0.0"); //

                mysql_2.Tables[0].Rows[0][2] = f1.templ + "°C" + "-" + f1.temph + "°C"; //

                if ((float.Parse(f1.numlist1[1]) - 3.6) >= float.Parse(f1.templ) && (float.Parse(f1.numlist1[1]) - 3.6) <= float.Parse(f1.temph))

                {

                    mysql_2.Tables[0].Rows[0][3] = "正 常"; //状态

                }

                else

                {

                    mysql_2.Tables[0].Rows[0][3] = "异 常"; //状态

                }

                //

                mysql_2.Tables[0].Rows[1][1] = (float.Parse(f1.numlist2[1]) + 7).ToString("#0.0"); //

                mysql_2.Tables[0].Rows[1][2] = f1.wetl + "%" + "-" + f1.weth + "%"; //

                if ((float.Parse(f1.numlist2[1]) + 7) >= float.Parse(f1.wetl) && (float.Parse(f1.numlist2[1]) + 7) <= float.Parse(f1.weth))

                {

                    mysql_2.Tables[0].Rows[1][3] = "正 常"; //状态

                }

                else

                {

                    mysql_2.Tables[0].Rows[1][3] = "异 常"; //状态

                }

            }

        }

    }

}

