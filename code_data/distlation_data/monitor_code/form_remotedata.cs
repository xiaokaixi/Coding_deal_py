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

        #region  
        public int cominnum = 0, gooutnum = 0, warningnum = 0, borrownum = 0, borandretnum = 0, should_borrownum = 0, count=0, controlsum=0, pandiansum=0, pandinawarning=0, transfernum=0;
        Form1 f1;
       
        #endregion

        #region  
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
            //
            preinit();
        }
        #endregion

        #region   
        private void preinit()
        {
            textBox2.Text = (((count - 1) * 100 )/ ((controlsum + 1) * 30 * 3)).ToString();  //
            textBox6.Text = cominnum.ToString();    //
            textBox7.Text = gooutnum.ToString();    //
            textBox8.Text = warningnum.ToString();  //  
            textBox1.Text = ((controlsum + 1) * 30 * 3).ToString();   //()
            textBox3.Text = ((count - 1) - borrownum).ToString(); //
            textBox12.Text = (count - 1).ToString(); //                                               
            textBox9.Text = borrownum.ToString();   //
            textBox11.Text = borandretnum.ToString();   //
            textBox10.Text = should_borrownum.ToString(); //
            textBox4.Text = pandiansum.ToString();   //
            textBox5.Text = pandinawarning.ToString();  //
            textBox13.Text = transfernum.ToString();   //
        }
        #endregion

        #region    
        public List<DataTable> list_savetable = new List<DataTable>();
        public List<string> savenamestr = new List<string>();
        public bool currentflg = false;

        private void button4_Click(object sender, EventArgs e)
        {
            f1.data_get = true;
            string foldPath = "";  //保存文件位置
            //
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
            form_save fsave = new form_save(f1, foldPath, "-" + DateTime.Now.ToString().Split(new char[1] { ' ' })[0]);
            fsave.ShowDialog();
            f1.data_get = false;
        }

        #endregion

        #region  
       mysqlclass mysqlconn00 = new mysqlclass();  //mysql
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
                mysqlconn00.mysqlconnn(ipstr);  //
                list0 = mysqlconn00.mysqlshow2(mysqlshowdatabases);   //
                f1.savenamestr.Clear();
                f1.list_savetable.Clear();
                if (list0.Count() != 0)
                {
                    //tablename
                    mysqlconn00.mysqlcom2(mysqlcom1);                  
                    f1.mysql_tablename_n = mysqlconn00.mysqlselectcom2(mysqlselect__tablename_n);
                    f1.savenamestr.Add("");
                    f1.list_savetable.Add(f1.mysql_tablename_n.Tables[0]);
                    f1.mysql_tablename_b = mysqlconn00.mysqlselectcom2(mysqlselect__tablename_b);
                    f1.savenamestr.Add("()");
                    f1.list_savetable.Add(f1.mysql_tablename_b.Tables[0]);
                    //outtableintable
                    mysqlconn00.mysqlcom2(mysqlcom2);
                    f1.mysql_outtable = mysqlconn00.mysqlselectcom2(mysqlselect_outtable);
                    f1.savenamestr.Add("");
                    f1.list_savetable.Add(f1.mysql_outtable.Tables[0]);
                    f1.mysql_intable = mysqlconn00.mysqlselectcom2(mysqlselect_intable);
                    f1.savenamestr.Add("");
                    f1.list_savetable.Add(f1.mysql_intable.Tables[0]);
                    f1.mysql_new = mysqlconn00.mysqlselectcom2(mysqlselect_new);
                    f1.savenamestr.Add("");
                    f1.list_savetable.Add(f1.mysql_new.Tables[0]);
                    //
                    f1.mysql_transfer = mysqlconn00.mysqlselectcom2(mysqlselect_transfernote);
                    f1.savenamestr.Add("");
                    f1.list_savetable.Add(f1.mysql_transfer.Tables[0]);
                    //
                    mysqlconn00.mysqlcom2(mysqlcom3);
                    f1.mysql_gatenote1 = mysqlconn00.mysqlselectcom2(mysqlselect_gatenote1);
                    f1.savenamestr.Add("");
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

        #region   
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion


    }
}
