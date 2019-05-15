using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace SerialPort_ViewSWUST1205
{
    public partial class form_save : Form
    {
        Form1 f1;
        System.Data.DataTable dt = new System.Data.DataTable();
        string file_name;
        //保存excel相关全局变量
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int pid);
        public string typename;
        public form_save(Form1 f11, string file_name1,string typename1)
        {
            InitializeComponent();
            f1 = f11;
            typename = typename1;
            file_name = file_name1;
        }

        private void form_save_Load(object sender, EventArgs e)
        {
            progressBar1.Visible = false;
            label1.Text = "是否确认导出数据?";
        }
    
        private void form_save_Load_1()
        {
            progressBar1.Value = 0;
            try
            {
                Microsoft.Office.Interop.Excel.Application myxls = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook mywkb = myxls.Workbooks.Add();
                for (int m = 0; m < f1.list_savetable.Count; m++)
                {
                    Microsoft.Office.Interop.Excel.Worksheet mysht = mywkb.Sheets.Add();
                    label1.Text = f1.savenamestr[m] + "正在导出。。。";
                    int mm = progressBar1.Value;
                    if ((mm + 100 / f1.list_savetable.Count+1)<=100)
                    {
                        progressBar1.Value += 100 / f1.list_savetable.Count+1;
                    }
                    else
                    {
                        progressBar1.Value = 100;
                    }
                    //save_excel(m,f1.list_savetable[m], file_name, f1.savenamestr[m], mysht, myxls, mywkb);
                    save_excel(m,f1.list_savetable[m], file_name, f1.savenamestr[m], mysht, myxls, mywkb,typename);
                }
            }
            catch
            { }
        }

        /// <summary>
        /// 多个sheet保存导出
        /// </summary>
        /// <param name="m"></param>
        /// <param name="dt"></param>
        /// <param name="file_name"></param>
        /// <param name="sheet_name"></param>
        /// <param name="mysht"></param>
        /// <param name="myxls"></param>
        /// <param name="mywkb"></param>
        private void save_excel(int m,System.Data.DataTable dt, string file_name, string sheet_name, Microsoft.Office.Interop.Excel.Worksheet mysht,Microsoft.Office.Interop.Excel.Application myxls, Microsoft.Office.Interop.Excel.Workbook mywkb,string tablenames)
        {

            mysht.Name = sheet_name;
            myxls.Visible = false;
            myxls.DisplayAlerts = false;
            try
            {
                //写入表头
                object[] arrheard = new object[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    arrheard[i] = dt.Columns[i].ColumnName;
                }
                mysht.Range[mysht.Cells[1, 1], mysht.Cells[1, dt.Columns.Count]].Value2 = arrheard;
                //写入表体数据
                object[,] arrbody = new object[dt.Rows.Count, dt.Columns.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        arrbody[i, j] = "'" + dt.Rows[i][j].ToString();
                    }
                }
                mysht.Range[mysht.Cells[2, 1], mysht.Cells[dt.Rows.Count + 1, dt.Columns.Count]].Value2 = arrbody;
                if (m == f1.list_savetable.Count - 1)
                {
                    if (mywkb != null)
                    {
                        mywkb.SaveAs(file_name + "\\" + tablenames);
                        mywkb.Close(Type.Missing, Type.Missing, Type.Missing);
                        mywkb = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示！");
            }

            finally
            {
                if (m == f1.list_savetable.Count - 1)
                {
                    //彻底关闭excel
                    if (myxls != null)
                    {
                        myxls.Quit();
                        try
                        {
                            if (myxls != null)
                            {
                                int pid;
                                GetWindowThreadProcessId(new IntPtr(myxls.Hwnd), out pid);
                                System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(pid);
                                p.Kill();
                                label1.Text = sheet_name + "导出成功！";
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("结束当前EXCEL进程失败：" + ex.Message);
                        }
                        myxls = null;
                    }
                    GC.Collect();
                }
            }
        }

        /// <summary>
        /// 多个table保存导出
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="file_name"></param>
        /// <param name="sheet_name"></param>
        private void save_excel_1(System.Data.DataTable dt, string file_name, string sheet_name)
        {
            Microsoft.Office.Interop.Excel.Application myxls = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook mywkb = myxls.Workbooks.Add();
            Microsoft.Office.Interop.Excel.Worksheet mysht = mywkb.ActiveSheet;
            mysht.Name = sheet_name;
            myxls.Visible = false;
            myxls.DisplayAlerts = false;
            try
            {
                //写入表头
                object[] arrheard = new object[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    arrheard[i] = dt.Columns[i].ColumnName;
                }
                mysht.Range[mysht.Cells[1, 1], mysht.Cells[1, dt.Columns.Count]].Value2 = arrheard;
                //写入表体数据
                object[,] arrbody = new object[dt.Rows.Count, dt.Columns.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        arrbody[i, j] = "'" + dt.Rows[i][j].ToString();
                    }
                }
                mysht.Range[mysht.Cells[2, 1], mysht.Cells[dt.Rows.Count + 1, dt.Columns.Count]].Value2 = arrbody;
                if (mywkb != null)
                {
                    mywkb.SaveAs(file_name + "\\" + sheet_name);
                    mywkb.Close(Type.Missing, Type.Missing, Type.Missing);
                    mywkb = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示！");
            }

            finally
            {
                //彻底关闭excel
                if (myxls != null)
                {
                    myxls.Quit();
                    try
                    {
                        if (myxls != null)
                        {
                            int pid;
                            GetWindowThreadProcessId(new IntPtr(myxls.Hwnd), out pid);
                            System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(pid);
                            p.Kill();
                            label1.Text = sheet_name + "导出成功！";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("结束当前EXCEL进程失败：" + ex.Message);
                    }
                    myxls = null;
                }
                GC.Collect();
            }
        }


        //确认导出数据
        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            button1.Visible = false;
            label1.Text = "稍等。。。";
            form_save_Load_1();
        }
    }
}
