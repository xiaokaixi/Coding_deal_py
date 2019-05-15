using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SerialPort_ViewSWUST1205
{
    public partial class form_remotedata : Form
    {

        #region  全局变量
        public int cominnum = 0, gooutnum = 0, warningnum = 0, borrownum = 0, borandretnum = 0, should_borrownum = 0, count=0, controlsum=0, pandiansum=0, pandinawarning=0, transfernum=0;
        Form1 f1;
       
        #endregion

        #region  窗体初始化
        public form_remotedata(Form1 f11, int cominnum1, int gooutnum1 , int warningnum1 , int borrownum1, int borandretnum1 , int should_borrownum1 ,int count1,int controlsum1,int pandiansum1,int pandinawarning1,int transfernum1)
        {
            InitializeComponent();
            f1 = f11;
            cominnum = cominnum1;
            gooutnum = gooutnum1;
            warningnum = warningnum1;
            borrownum = borrownum1;
            borandretnum = borandretnum1;
            should_borrownum = should_borrownum1;
            count = count1;
            controlsum = controlsum1;
            pandiansum = pandiansum1;
            pandinawarning = pandinawarning1;
            transfernum = transfernum1;
        }

        private void form_remotedata_Load(object sender, EventArgs e)
        {
            //参数初始化
            preinit();
        }
        #endregion

        #region   初始化
        private void preinit()
        {
            textBox2.Text = (((count - 1) * 100 )/ ((controlsum + 1) * 30 * 3)).ToString();  //库房档案柜使用率
            textBox6.Text = cominnum.ToString();    //通道门进入数量
            textBox7.Text = gooutnum.ToString();    //通道门出去数量
            textBox8.Text = warningnum.ToString();  //通道门异常数量  
            textBox1.Text = ((controlsum + 1) * 30 * 3).ToString();   //档案盒总容量(一期示范库房总容量)
            textBox3.Text = ((count - 1) - borrownum).ToString(); //现存档案总数
            textBox12.Text = (count - 1).ToString(); //新增档案数量                                               
            textBox9.Text = borrownum.ToString();   //取走档案数量
            textBox11.Text = borandretnum.ToString();   //借还档案数量
            textBox10.Text = should_borrownum.ToString(); //应还档案数
            textBox4.Text = pandiansum.ToString();   //上次盘点数量
            textBox5.Text = pandinawarning.ToString();  //上次盘点异常数据
            textBox13.Text = transfernum.ToString();   //转递档案数量
        }
        #endregion

        #region    数据导出
        public List<DataTable> list_savetable = new List<DataTable>();
        public List<string> savenamestr = new List<string>();
        public bool currentflg = false;

        private void button4_Click(object sender, EventArgs e)
        {
            f1.data_get = true;
            string foldPath = "";  //保存文件位置
            //获取数据
            mysqldata_get("192.168.1.222");
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择保存文件路径";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foldPath = dialog.SelectedPath;
            }
            else
            {
                return;
            }
            form_save fsave = new form_save(f1, foldPath, "库房数据-" + DateTime.Now.ToString().Split(new char[1] { ' ' })[0]);
            fsave.ShowDialog();
            f1.data_get = false;
        }

        #endregion

        #region  远程数据获取
       mysqlclass mysqlconn00 = new mysqlclass();  //盘点mysql
        private bool mysqldata_get(string ipstr)
        {
            bool flg = false;
            List<string> list0 = new List<string>();
            string mysqlshowdatabases = "show databases";
            string mysqlcom1 = "use store";
            string mysqlselect__tablename_n = "select*from tablename where status = \"1\"";  //在库总表查询
            string mysqlselect__tablename_b = "select*from tablename where status = \"0\"";  //不在库总表查询
            //string mysqlselect_outtable = "select*from outtable";   //借出记录总表查询
            //string mysqlselect_intable = "select*from intable";     //归还记录总表查询
            string mysqlcom2 = "use note19";
            string mysqlselect_outtable = "select*from note1901 where status = \"borrow\"";    //借出记录总表查询
            string mysqlselect_intable = "select*from note1901 where status = \"return\"";     //归还记录总表查询
            string mysqlselect_new = "select*from note1901 where status = \"new\"";        //新增记录总表查询
            string mysqlcom3 = "use gatenote";
            string mysqlselect_gatenote1 = "select*from gatenote1";   //通道门数据
            string mysqlselect_transfernote = "select*from transfernote";   //通道门数据


            try
            {
                mysqlconn00.mysqlconnn(ipstr);  //初始化连接数据库
                list0 = mysqlconn00.mysqlshow2(mysqlshowdatabases);   //获取得到数据库查询
                f1.savenamestr.Clear();
                f1.list_savetable.Clear();
                if (list0.Count() != 0)
                {
                    //获取在库、不在库表tablename
                    mysqlconn00.mysqlcom2(mysqlcom1);                  
                    f1.mysql_tablename_n = mysqlconn00.mysqlselectcom2(mysqlselect__tablename_n);
                    f1.savenamestr.Add("在库档案");
                    f1.list_savetable.Add(f1.mysql_tablename_n.Tables[0]);
                    f1.mysql_tablename_b = mysqlconn00.mysqlselectcom2(mysqlselect__tablename_b);
                    f1.savenamestr.Add("不在库(借出)档案");
                    f1.list_savetable.Add(f1.mysql_tablename_b.Tables[0]);
                    //获取借阅、归还表outtable、intable
                    mysqlconn00.mysqlcom2(mysqlcom2);
                    f1.mysql_outtable = mysqlconn00.mysqlselectcom2(mysqlselect_outtable);
                    f1.savenamestr.Add("借阅历史记录");
                    f1.list_savetable.Add(f1.mysql_outtable.Tables[0]);
                    f1.mysql_intable = mysqlconn00.mysqlselectcom2(mysqlselect_intable);
                    f1.savenamestr.Add("归还历史记录");
                    f1.list_savetable.Add(f1.mysql_intable.Tables[0]);
                    f1.mysql_new = mysqlconn00.mysqlselectcom2(mysqlselect_new);
                    f1.savenamestr.Add("新增历史记录");
                    f1.list_savetable.Add(f1.mysql_new.Tables[0]);
                    //转递档案记录获取
                    f1.mysql_transfer = mysqlconn00.mysqlselectcom2(mysqlselect_transfernote);
                    f1.savenamestr.Add("档案转递记录");
                    f1.list_savetable.Add(f1.mysql_transfer.Tables[0]);
                    //获取通道门数据
                    mysqlconn00.mysqlcom2(mysqlcom3);
                    f1.mysql_gatenote1 = mysqlconn00.mysqlselectcom2(mysqlselect_gatenote1);
                    f1.savenamestr.Add("通道门历史记录");
                    f1.list_savetable.Add(f1.mysql_gatenote1.Tables[0]);
                    flg = true;
                    return flg;
                }
                else
                {
                    MessageBox.Show("获取数据失败，请检查网络连接！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    mysqlconn00.mysqlconn2.Dispose();
                    return flg;
                }
            }
            catch
            {
                MessageBox.Show("获取数据失败，请检查网络连接！", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                mysqlconn00.mysqlconn2.Dispose();
                return flg;
            }
        }
        #endregion

        #region  退出 
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion


    }
}
