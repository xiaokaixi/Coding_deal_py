using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Threading;
using System.IO;
using System.IO.Ports;
using System.Timers;



namespace manage
{

    #region  窗体类
    public partial class form_qrshow : Form
    {

        

        public Image img;

        #region  窗体引导
        public form_qrshow(Image image,string txt)
        {
            InitializeComponent();
            pictureBox1.BackgroundImage = image;
            img = image;
            label1.Text = txt;
        }


        System.Timers.Timer timer = new System.Timers.Timer(1000);
        private void Form3_Load(object sender, EventArgs e)
        {
            label2.Text = System.DateTime.Now.ToString();
            printqr print = new printqr();
            //print.portname();
            PrintPage();
            progressBar1.Value = 60;
            Thread.Sleep(300);
            progressBar1.Value = 80;
            Thread.Sleep(400);
            progressBar1.Value = 100;
            //timer           
            //timer.Elapsed += new System.Timers.ElapsedEventHandler(timestop);
            //timer.AutoReset = true; //每到指定时间Elapsed事件是触发一次（false），还是一直触发（true）
            //timer.Enabled = true; //是否触发Elapsed事件
            //timer.Start();  //开始计时
            //Control.CheckForIllegalCrossThreadCalls = false;  //跨线程调用控件
        }
        private void timestop(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Close();  
        }

        #endregion       

        #region 二维码小票打印
        public void PrintPage()
        {
            PrintPreviewDialog printPreviewDialog2 = new PrintPreviewDialog();//新建打印预览窗体   
            PrintDocument PrintDocument2 = new PrintDocument();//新建打印对象                     
            PageSetupDialog PageSetupDialog2 = new PageSetupDialog();//新建打印设置 
            PrintDocument2.PrintPage += new PrintPageEventHandler(printDocument2_PrintPage);//新建打印输出                         
            PrintDocument2.DefaultPageSettings.Landscape = false; //False 横打
            PaperSize pkCustomSize1 = new PaperSize("6cun", 580, 300);	//新建一个页面尺寸（6寸照片4*6英寸）
            PrintDocument2.DefaultPageSettings.PaperSize = pkCustomSize1;
            printPreviewDialog2.Document = PrintDocument2;//获取打印预览          
            //printPreviewDialog2.ShowDialog();//打开打印预览窗口 
            //PrintDocument2.Print();
            progressBar1.Value = 50;
        }
        public void printDocument2_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Image obj = img;
            //Bitmap b = new Bitmap(obj);
            //obj = resizeimage(b, new Size(30, 30));           
            e.Graphics.DrawString ("入库二维码", new Font("黑体", 9, FontStyle.Regular), new SolidBrush(Color.Red), 60, 0);//写string 
            e.Graphics.DrawImage(obj, 15, 15);//绘制二维码 
            e.Graphics.DrawString(label1.Text.ToString(), new Font("黑体", 9, FontStyle.Regular), new SolidBrush(Color.Red), 15, 180);//写string  
            e.Graphics.DrawString(label2.Text.ToString(), new Font("黑体", 9, FontStyle.Regular), new SolidBrush(Color.Red), 25, 195);//写string 
            PaperSize pkCustomSize1 = new PaperSize("6cun",500, 300);
            e.PageSettings.PaperSize = pkCustomSize1;
        }
        #endregion

    }
    #endregion


    #region  打印类
    public class printqr
    {
        public bool init;
        public SerialPort serialport;

        #region  端口初始化
        public void initopen()
        {
           
        }
        #endregion

        #region 预览测试

        public void PrintPageTwo()
        {
            PrintPreviewDialog printPreviewDialog2 = new PrintPreviewDialog();//新建打印预览窗体   
            PrintDocument PrintDocument2 = new PrintDocument();//新建打印对象                     
            PageSetupDialog PageSetupDialog2 = new PageSetupDialog();//新建打印设置 
            PrintDocument2.PrintPage += new PrintPageEventHandler(printDocument2_PrintPage);//新建打印输出                         
            PrintDocument2.DefaultPageSettings.Landscape = false; //False 横打

            PaperSize pkCustomSize1 = new PaperSize("6cun", 58, 40);	//新建一个页面尺寸（6寸照片4*6英寸）
            PrintDocument2.DefaultPageSettings.PaperSize = pkCustomSize1;
            printPreviewDialog2.Document = PrintDocument2;//获取打印预览          
            printPreviewDialog2.ShowDialog();//打开打印预览窗口 
            //PrintDocument2.Print();  //打印
            
        }
        public void printDocument2_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //    Image obj = f3.pictureBox1.Image;
            //    e.Graphics.DrawImage(obj, 0, 0);//绘制打印预览方法二   
            //    PaperSize pkCustomSize1 = new PaperSize("6cun", 58, 40);
            //    e.PageSettings.PaperSize = pkCustomSize1;
        }
        #endregion
    }
    #endregion

}
