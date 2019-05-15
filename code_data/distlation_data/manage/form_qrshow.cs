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

    #region  
    public partial class form_qrshow : Form
    {

        

        public Image img;

        #region  
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
            //timer.AutoReset = true; //Elapsedfalsetrue
            //timer.Enabled = true; //Elapsed
            //timer.Start();  //
            //Control.CheckForIllegalCrossThreadCalls = false;  //
        }
        private void timestop(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Close();  
        }

        #endregion       

        #region 
        public void PrintPage()
        {
            PrintPreviewDialog printPreviewDialog2 = new PrintPreviewDialog();//   
            PrintDocument PrintDocument2 = new PrintDocument();//                     
            PageSetupDialog PageSetupDialog2 = new PageSetupDialog();// 
            PrintDocument2.PrintPage += new PrintPageEventHandler(printDocument2_PrintPage);//                         
            PrintDocument2.DefaultPageSettings.Landscape = false; //False 
            PaperSize pkCustomSize1 = new PaperSize("6cun", 580, 300);	//64*6
            PrintDocument2.DefaultPageSettings.PaperSize = pkCustomSize1;
            printPreviewDialog2.Document = PrintDocument2;//          
            //printPreviewDialog2.ShowDialog();// 
            //PrintDocument2.Print();
            progressBar1.Value = 50;
        }
        public void printDocument2_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Image obj = img;
            //Bitmap b = new Bitmap(obj);
            //obj = resizeimage(b, new Size(30, 30));           
            e.Graphics.DrawString ("", new Font("", 9, FontStyle.Regular), new SolidBrush(Color.Red), 60, 0);//string 
            e.Graphics.DrawImage(obj, 15, 15);// 
            e.Graphics.DrawString(label1.Text.ToString(), new Font("", 9, FontStyle.Regular), new SolidBrush(Color.Red), 15, 180);//string  
            e.Graphics.DrawString(label2.Text.ToString(), new Font("", 9, FontStyle.Regular), new SolidBrush(Color.Red), 25, 195);//string 
            PaperSize pkCustomSize1 = new PaperSize("6cun",500, 300);
            e.PageSettings.PaperSize = pkCustomSize1;
        }
        #endregion

    }
    #endregion


    #region  
    public class printqr
    {
        public bool init;
        public SerialPort serialport;

        #region  
        public void initopen()
        {
           
        }
        #endregion

        #region 

        public void PrintPageTwo()
        {
            PrintPreviewDialog printPreviewDialog2 = new PrintPreviewDialog();//   
            PrintDocument PrintDocument2 = new PrintDocument();//                     
            PageSetupDialog PageSetupDialog2 = new PageSetupDialog();// 
            PrintDocument2.PrintPage += new PrintPageEventHandler(printDocument2_PrintPage);//                         
            PrintDocument2.DefaultPageSettings.Landscape = false; //False 

            PaperSize pkCustomSize1 = new PaperSize("6cun", 58, 40);	//64*6
            PrintDocument2.DefaultPageSettings.PaperSize = pkCustomSize1;
            printPreviewDialog2.Document = PrintDocument2;//          
            printPreviewDialog2.ShowDialog();// 
            //PrintDocument2.Print();  //
            
        }
        public void printDocument2_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //    Image obj = f3.pictureBox1.Image;
            //    e.Graphics.DrawImage(obj, 0, 0);//   
            //    PaperSize pkCustomSize1 = new PaperSize("6cun", 58, 40);
            //    e.PageSettings.PaperSize = pkCustomSize1;
        }
        #endregion
    }
    #endregion

}
