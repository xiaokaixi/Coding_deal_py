using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using Renci.SshNet;


namespace ConsoleApp1
{
    #region  mysql数据库连接类
    /// <summary>
    /// mysql类
    /// </summary>
    class mysqlclass_1
    {
        //mysqlssh连接
        //MySqlConnection mysqlconn1 = new MySqlConnection(mysqlconn);  //创建sql连接对象
        //ySqlDataAdapter mysqldata = new MySqlDataAdapter();     //数据库执行命令方法,适合大数据量
        //MySqlDataReader tablename = null;   //数据库命令返回数据，适合小数据量
        public int conn = 0;
        public string mysqlconn;
        public MySqlConnection mysqlconn1 = new MySqlConnection();
        public MySqlDataAdapter mysqldata = new MySqlDataAdapter();     //数据库执行命令方法,适合大数据量
        public SshClient client;
        public MySqlConnection mysqlsshconn(string text3)
        {
            if (text3 == "")
            {
                MessageBox.Show("请输入ip地址！", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            string sshhost = text3;  //"192.168.21.21"
            int sshport = 22;
            string sshuser = "root";
            string sshpassword = "1234";
            string ip = "127.0.0.2";  //映射地址
            string mysqlhost = text3;
            uint mysqlport = 3306;
            //local-connect
            mysqlconn = "Database=;Data source=localhost;Port=3306;User Id=root;password=1234;CharSet=utf8;Allow Zero Datetime=True";
            //mysqlconn = "Database=;Data source=127.0.0.2;Port=3306;User id=xiaomysql;Password=1234;CharSet=utf8;Allow Zero Datetime=True";
            //mysqlconn = "server=127.0.0.2;User Id=root;password=1234;Database=db_test";  //Allow Zero Datetime=True解决time数据类型的转换问题

            PasswordConnectionInfo connectinfo = new PasswordConnectionInfo(sshhost, sshport, sshuser, sshpassword);
            connectinfo.Timeout = TimeSpan.FromSeconds(30);   //ssh连接超时

            //using (var client = new SshClient(connectinfo))    //ssh连接
            client = new SshClient(connectinfo);
            //{
            //try
            //{
            //    client.Connect();
            //    if (!client.IsConnected)
            //    {
            //        MessageBox.Show("SSH链接失败！", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        f1.progressBar1.Value = 0;
            //        return null;
            //    }
            //    f1.progressBar1.Value = 60;
            //    //映射本地端口（？）
            //    var portfwd = new ForwardedPortLocal(ip, mysqlport, mysqlhost, mysqlport); //映射到本地的ip及端口3306
            //    client.AddForwardedPort(portfwd);
            //    portfwd.Start();
            //    if (!client.IsConnected)
            //    {
            //        MessageBox.Show("端口映射错误！", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        f1.progressBar1.Value = 0;
            //        return null;
            //    }

            mysqlconn1.ConnectionString = mysqlconn;
            try
            {
                mysqlconn1.Open();
                conn = 1;
                //MessageBox.Show("数据库链接成功！", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
            catch
            {
                MessageBox.Show("连接数据库失败！", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show("链接异常", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return null;
            //}
            //}
            mysqlconn1.Dispose();
            return mysqlconn1;
        }

        public MySqlConnection mysqlconn2 = new MySqlConnection();
        public MySqlConnection mysqlconnn(string text3)
        {
            if (text3 == "")
            {
                MessageBox.Show("请输入ip地址！", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            string sshhost = text3;  //"192.168.21.21"
            string ip = "127.0.0.2";  //映射地址
            string mysqlhost = text3;
            uint mysqlport = 3306;
            mysqlconn = "Database=;Data source=192.168.1.222;Port=3306;User Id=root;password=1234;CharSet=utf8;Allow Zero Datetime=True";
            try
            {
                mysqlconn2.ConnectionString = mysqlconn;
                try
                {
                    mysqlconn2.Open();
                    conn = 1;                  
                    MessageBox.Show("数据库链接成功！", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.None);
                }
                catch
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
            //}
            mysqlconn2.Dispose();
            return mysqlconn2;
        }

        /// <summary>
        /// 读取返回SQL1
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<string> mysqlshow(string sql)
        {
            List<string> lis_name = new List<string>();
            MySqlDataReader tablename = null;   //数据库命令返回数据，适合小数据量
            MySqlCommand mysqlcom_showtable = new MySqlCommand(sql, mysqlconn1);  //sql-show
            try
            {
                mysqlconn1.Open();
                tablename = mysqlcom_showtable.ExecuteReader();
                if (tablename.HasRows)   //判断返回表name是否为空
                {
                    string t;
                    while (tablename.Read())
                    {
                        t = tablename.GetString(0);
                        lis_name.Add(t);
                    }
                    tablename.Close(); //关闭读数据库返回命令
                }
                else { MessageBox.Show("数据库无任何表！", "error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                mysqlcom_showtable.Dispose();  //关闭数据库命令
                mysqlconn1.Dispose();
                return lis_name;
            }
            catch { return lis_name; }

        }

        /// <summary>
        /// 读取返回SQL2
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<string> mysqlshow2(string sql)
        {
            List<string> lis_name = new List<string>();
            MySqlDataReader tablename = null;   //数据库命令返回数据，适合小数据量
            MySqlCommand mysqlcom_showtable = new MySqlCommand(sql, mysqlconn2);  //sql-show
            try
            {
                mysqlconn2.Open();
                tablename = mysqlcom_showtable.ExecuteReader();
                if (tablename.HasRows)   //判断返回表name是否为空
                {
                    string t;
                    while (tablename.Read())
                    {
                        t = tablename.GetString(0);
                        lis_name.Add(t);
                    }
                    tablename.Close(); //关闭读数据库返回命令
                }
                else { MessageBox.Show("数据库无任何表！", "error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                mysqlcom_showtable.Dispose();  //关闭数据库命令
                mysqlconn2.Dispose();
                return lis_name;
            }
            catch { return lis_name; }

        }


        /// <summary>
        /// mysql select命令
        /// </summary>
        /// <param name="mysqlselect"></param>
        /// <returns></returns>
        public DataSet mysqlselectcom(string mysqlselect)
        {
            MySqlCommand mysqlcom_select = new MySqlCommand(mysqlselect, mysqlconn1);  //sql-select命令
            DataSet mysql = new DataSet();
            try
            {
                mysqlconn1.Open();
                if (conn == 1)
                {
                    mysqldata.SelectCommand = mysqlcom_select;
                    mysqldata.Fill(mysql);
                    mysqlcom_select.Dispose();
                }
                else
                {
                    MessageBox.Show("数据库链接中断！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                mysqlconn1.Dispose();
                return mysql;
            }
            catch { return mysql; }
        }

        //mysql select命令2
        public DataSet mysqlselectcom2(string mysqlselect)
        {
            MySqlCommand mysqlcom_select = new MySqlCommand(mysqlselect, mysqlconn2);  //sql-select命令
            DataSet mysql = new DataSet();
            try
            {
                mysqlconn2.Open();
                if (conn == 1)
                {
                    //mysqldata.SelectCommand.Connection = mysqlconn1;
                    mysqldata.SelectCommand = mysqlcom_select;
                    mysqldata.Fill(mysql);
                    mysqlcom_select.Dispose();
                }
                else
                {
                    MessageBox.Show("数据库链接中断！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                mysqlconn2.Dispose();
                return mysql;
            }
            catch { return mysql; }
        }

        /// <summary>
        /// 保存更新数据命令
        /// </summary>
        /// <param name="mysqlcom"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public bool mysqlupdatecomall(string mysqlcom, DataSet table)
        {
            bool file = true;
            List<string> comlist = new List<string>();
            StringBuilder com = new StringBuilder();
            MySqlCommand mysqlcom_save = new MySqlCommand();  //sql-show  


            for (int i = 0; i < table.Tables[0].Rows.Count; i++)
            {
                com.Clear();
                com.Append(mysqlcom);
                for (int j = 0; j < table.Tables[0].Columns.Count; j++)
                {
                    if (j != table.Tables[0].Columns.Count - 1)
                    {
                        com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[i][j] + "\",");
                    }
                    else
                    {
                        com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[i][j] + "\" ");
                    }
                }
                com.Append("where id=" + table.Tables[0].Rows[i][0]);
                comlist.Add(com.ToString());
            }

            try
            {
                //int.Parse(i)//string转int
                mysqlconn1.Open();
                try
                {
                    for (int i = 0; i < comlist.Count(); i++)
                    {
                        mysqlcom_save = new MySqlCommand(comlist[i], mysqlconn1);  //sql-save
                        mysqlcom_save.ExecuteNonQuery();
                    }

                }
                catch
                {
                    file = false;
                }
                mysqlconn1.Dispose();
            }
            catch
            {
                MessageBox.Show("保存数据失败！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return file;
        }

        /// <summary>
        /// 储存数据
        /// </summary>
        /// <param name="mysqlcom"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public bool mysqlupdatecomarow(string mysqlcom, DataSet table)
        {
            bool file = true;
            List<string> comlist = new List<string>();
            StringBuilder com = new StringBuilder();
            MySqlCommand mysqlcom_save = new MySqlCommand();  //sql-show           

            for (int i = 0; i <= table.Tables[0].Rows.Count - 2; i++)
            {
                com.Clear();
                com.Append(mysqlcom);
                for (int j = 0; j < table.Tables[0].Columns.Count; j++)
                {

                    if (j != table.Tables[0].Columns.Count - 1)
                    {
                        com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[i][j] + "\",");
                    }
                    else
                    {
                        com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[i][j] + "\"");
                    }
                }
                comlist.Add(com.ToString());
            }
            //com.Append("where id=" + table.Tables[0].Rows[table.Tables[0].Rows.Count - 2][0]);

            try
            {
                //int.Parse(i)//string转int
                mysqlconn1.Open();
                try
                {
                    for (int i = 0; i < comlist.Count(); i++)
                    {
                        mysqlcom_save = new MySqlCommand(comlist[i], mysqlconn1);  //sql-save
                        mysqlcom_save.ExecuteNonQuery();
                    }
                }
                catch
                {
                    file = false;
                }
                mysqlconn1.Dispose();
            }
            catch
            {
                MessageBox.Show("保存数据失败！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return file;
        }

        /// <summary>
        /// 批量入库存储数据
        /// </summary>
        /// <param name="mysqlcom"></param>
        /// <param name="table"></param>
        /// <param name="tt"></param>
        /// <returns></returns>
        public bool mysqlupdatecomarow1(string mysqlcom, DataSet table, int tt)
        {
            bool file = true;
            List<string> comlist = new List<string>();
            StringBuilder com = new StringBuilder();
            MySqlCommand mysqlcom_save = new MySqlCommand();  //sql-show           

            com.Clear();
            com.Append(mysqlcom);
            for (int j = 0; j < table.Tables[0].Columns.Count; j++)
            {

                if (j != table.Tables[0].Columns.Count - 1)
                {
                    com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[table.Tables[0].Rows.Count - 2 - tt][j] + "\",");
                }
                else
                {
                    com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[table.Tables[0].Rows.Count - 2 - tt][j] + "\"");
                }
            }
            comlist.Add(com.ToString());


            try
            {
                //int.Parse(i)//string转int
                mysqlconn1.Open();
                try
                {
                    for (int i = 0; i < comlist.Count(); i++)
                    {
                        mysqlcom_save = new MySqlCommand(comlist[i], mysqlconn1);  //sql-save
                        mysqlcom_save.ExecuteNonQuery();
                    }
                }
                catch
                {
                    file = false;
                }
                mysqlconn1.Dispose();
            }
            catch
            {
                MessageBox.Show("保存数据失败！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return file;
        }
        public bool mysqlsavecom(string mysqlcom, DataSet table, string tablename)
        {
            bool file = true;
            List<string> comlist = new List<string>();
            StringBuilder com = new StringBuilder();
            MySqlCommand mysqlcom_save = new MySqlCommand();  //sql-show        
            com.Clear();
            com.Append(mysqlcom);
            try
            {
                for (int i = 0; i < table.Tables[0].Columns.Count; i++)
                {
                    if (i == table.Tables[0].Columns.Count - 1)
                    {
                        com.Append(table.Tables[0].Columns[i] + " char(100))");
                    }
                    else
                    {
                        com.Append(table.Tables[0].Columns[i] + " char(100),");
                    }
                }

                mysqlconn1.Open();
                mysqlcom_save = new MySqlCommand(com.ToString(), mysqlconn1);  //sql-save
                mysqlcom_save.ExecuteNonQuery();
                mysqlconn1.Dispose();

            }
            catch
            {
                MessageBox.Show("00001", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            mysqlcom = "insert into " + tablename + " set ";


            for (int i = 0; i < table.Tables[0].Rows.Count; i++)
            {
                com.Clear();
                com.Append(mysqlcom);
                for (int j = 0; j < table.Tables[0].Columns.Count; j++)
                {
                    if (j != table.Tables[0].Columns.Count - 1)
                    {
                        com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[i][j] + "\",");
                    }
                    else
                    {
                        com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[i][j] + "\"");
                    }
                }

                comlist.Add(com.ToString());
            }

            try
            {
                //int.Parse(i)//string转int
                mysqlconn1.Open();
                try
                {
                    for (int i = 0; i < comlist.Count() - 1; i++)
                    {
                        mysqlcom_save = new MySqlCommand(comlist[i], mysqlconn1);  //sql-save
                        mysqlcom_save.ExecuteNonQuery();
                    }

                }
                catch
                {
                    file = false;
                }
                mysqlconn1.Dispose();
            }
            catch
            {
                MessageBox.Show("00002！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return file;
        }

        /// <summary>
        /// 通道门储存数据（新建表）
        /// </summary>
        /// <param name="mysqlcom"></param>
        /// <param name="table"></param>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public bool mysqlsavecom2(string mysqlcom, DataSet table, string tablename)
        {
            bool file = true;
            List<string> comlist = new List<string>();
            StringBuilder com = new StringBuilder();
            MySqlCommand mysqlcom_save = new MySqlCommand();  //sql-show        
            com.Clear();
            com.Append(mysqlcom);
            try
            {
                for (int i = 0; i < table.Tables[0].Columns.Count; i++)
                {
                    if (i == table.Tables[0].Columns.Count - 1)
                    {
                        com.Append(table.Tables[0].Columns[i] + " char(100))");
                    }
                    else
                    {
                        com.Append(table.Tables[0].Columns[i] + " char(100),");
                    }
                }

                mysqlconn1.Open();
                mysqlcom_save = new MySqlCommand(com.ToString(), mysqlconn1);  //sql-save
                mysqlcom_save.ExecuteNonQuery();
                mysqlconn1.Dispose();

            }
            catch
            {
                MessageBox.Show("00001", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            mysqlcom = "insert into " + tablename + " set ";


            for (int i = 0; i < table.Tables[0].Rows.Count; i++)
            {
                com.Clear();
                com.Append(mysqlcom);
                for (int j = 0; j < table.Tables[0].Columns.Count; j++)
                {
                    if (j != table.Tables[0].Columns.Count - 1)
                    {
                        com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[i][j] + "\",");
                    }
                    else
                    {
                        com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[i][j] + "\"");
                    }
                }

                comlist.Add(com.ToString());
            }

            try
            {
                //int.Parse(i)//string转int
                mysqlconn1.Open();
                try
                {
                    for (int i = 0; i < comlist.Count(); i++)
                    {
                        mysqlcom_save = new MySqlCommand(comlist[i], mysqlconn1);  //sql-save
                        mysqlcom_save.ExecuteNonQuery();
                    }

                }
                catch
                {
                    file = false;
                }
                mysqlconn1.Dispose();
            }
            catch
            {
                MessageBox.Show("00002！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return file;
        }

        /// <summary>
        /// 存储通道门数据
        /// </summary>
        /// <param name="mysqlcom"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public bool mysqlupdatecomarow2(string mysqlcom, DataSet table)
        {
            bool file = true;
            List<string> comlist = new List<string>();
            StringBuilder com = new StringBuilder();
            MySqlCommand mysqlcom_save = new MySqlCommand();  //sql-show           

            for (int i = 0; i <= table.Tables[0].Rows.Count - 1; i++)
            {
                com.Clear();
                com.Append(mysqlcom);
                for (int j = 0; j < table.Tables[0].Columns.Count; j++)
                {

                    if (j != table.Tables[0].Columns.Count - 1)
                    {
                        com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[i][j] + "\",");
                    }
                    else
                    {
                        com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[i][j] + "\"");
                    }
                }
                comlist.Add(com.ToString());
            }

            try
            {
                mysqlconn1.Open();
                try
                {
                    for (int i = 0; i < comlist.Count(); i++)
                    {
                        mysqlcom_save = new MySqlCommand(comlist[i], mysqlconn1);  //sql-save
                        mysqlcom_save.ExecuteNonQuery();
                    }
                }
                catch
                {
                    file = false;
                }
                mysqlconn1.Dispose();
            }
            catch
            {
                MessageBox.Show("保存数据失败！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return file;
        }

        /// <summary>
        /// 批量入库创建及存储数据
        /// </summary>
        /// <param name="mysqlcom"></param>
        /// <param name="table"></param>
        /// <param name="tablename"></param>
        /// <param name="ttt"></param>
        /// <returns></returns>
        public bool mysqlsavecom1(string mysqlcom, DataSet table, string tablename, int ttt)
        {
            bool file = true;
            List<string> comlist = new List<string>();
            StringBuilder com = new StringBuilder();
            MySqlCommand mysqlcom_save = new MySqlCommand();  //sql-show        
            com.Clear();
            com.Append(mysqlcom);
            try
            {
                for (int i = 0; i < table.Tables[0].Columns.Count; i++)
                {
                    if (i == table.Tables[0].Columns.Count - 1)
                    {
                        com.Append(table.Tables[0].Columns[i] + " char(100))");
                    }
                    else
                    {
                        com.Append(table.Tables[0].Columns[i] + " char(100),");
                    }
                }

                mysqlconn1.Open();
                mysqlcom_save = new MySqlCommand(com.ToString(), mysqlconn1);  //sql-save
                mysqlcom_save.ExecuteNonQuery();
                mysqlconn1.Dispose();

            }
            catch
            {
                MessageBox.Show("00001", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            mysqlcom = "insert into " + tablename + " set ";

            com.Clear();
            com.Append(mysqlcom);
            for (int j = 0; j < table.Tables[0].Columns.Count; j++)
            {
                if (j != table.Tables[0].Columns.Count - 1)
                {
                    com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[table.Tables[0].Rows.Count - 2 - ttt][j] + "\",");
                }
                else
                {
                    com.Append(table.Tables[0].Columns[j] + "=\"" + table.Tables[0].Rows[table.Tables[0].Rows.Count - 2 - ttt][j] + "\"");
                }
            }

            comlist.Add(com.ToString());
            try
            {
                //int.Parse(i)//string转int
                mysqlconn1.Open();
                try
                {
                    for (int i = 0; i < comlist.Count(); i++)
                    {
                        mysqlcom_save = new MySqlCommand(comlist[i], mysqlconn1);  //sql-save
                        mysqlcom_save.ExecuteNonQuery();
                    }

                }
                catch
                {
                    file = false;
                }
                mysqlconn1.Dispose();
            }
            catch
            {
                MessageBox.Show("00002！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return file;
        }

        /// <summary>
        /// 普通mysql命令1
        /// </summary>
        /// <param name="sql"></param>
        public void mysqlcom(string sql)
        {

            MySqlDataReader tablename = null;   //数据库命令返回数据，适合小数据量
            MySqlCommand mysqlcom_showtable = new MySqlCommand(sql, mysqlconn1);  //sql-show
            try
            {
                if (mysqlconn1.State.ToString() == "Closed")
                {
                    mysqlconn1.Open();
                }
                tablename = mysqlcom_showtable.ExecuteReader();
                mysqlcom_showtable.Dispose();  //关闭数据库命令
                mysqlconn1.Dispose();

            }
            catch
            {
                //if (mysqlconn1.State.ToString() == "Open")
                //{
                //    mysqlconn1.Dispose();
                //}
                //MessageBox.Show("数据库链接中断！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        /// <summary>
        /// 普通mysql命令2
        /// </summary>
        /// <param name="sql"></param>
        public void mysqlcom2(string sql)
        {

            MySqlDataReader tablename = null;   //数据库命令返回数据，适合小数据量
            MySqlCommand mysqlcom_showtable = new MySqlCommand(sql, mysqlconn2);  //sql-show
            try
            {
                mysqlconn2.Open();
                tablename = mysqlcom_showtable.ExecuteReader();
                mysqlcom_showtable.Dispose();  //关闭数据库命令
                mysqlconn2.Dispose();

            }
            catch { MessageBox.Show("数据库链接中断！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
    }
    #endregion
}
