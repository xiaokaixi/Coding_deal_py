using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace ConsoleApp1
{
    class global
    {
        // 全局变量
        public static bool sjConnect = false;//手动新增的串口连接状态标志位
        public static bool MultiReadStatus = false;
        public static int m_connect_type = transfer.CONNECT_SERIAL;
        public static int m_cbBaute = 0;        // 当前串口连接的波特率
        public static int m_cbCOM = 0;          // 当前串口连接的端口
        public static string ipStr;
        public const byte FASTID_ON = 1;
        public const byte FASTID_OFF = 0;
        public const byte CARRIER_ON = 1;
        public const byte CARRIER_OFF = 0;
        public const byte TAGFOCUS_ON = 1;
        public const byte TAGFOCUS_OFF = 0;
        public static int InStatistics = 0;
        public static int OutStatistics = 0;

        public const int OPER_OK = 0;          // 表示sr api函数是否成功执行

        public const int READ_FLAG = 1;
        public const int WRITE_FLAG = 2;

        // 频点
        public const int OUTPUT_FREQUENCY_NUM = 128;


        // packet
        public const int RECV_PACKET_NUM = 50;			// 循环寻标签时，接收一次，最多包含的完整的返回命令包的个数
        public const int PACKET_128 = 128;
        public const int PACKET_MIN = 32;
        public const int FOUR_CHANNELS = 1;          // 是否为四通道，1：四通道。0：单通道。
        public const int PACKET_SIZE = 23;			// 四通道的一个完整数据包为23字节，单通道的为22个字节
        public const int PACKET_MID = (RECV_PACKET_NUM * PACKET_SIZE);  // 如果 RECV_PACKET_NUM > 10,PACKET_MID	=(RECV_PACKET_NUM*PACKET_SIZE).否则  PACKET_MID = 256
        //     public const int PACKET_MID = (256);  // 如果 RECV_PACKET_NUM > 10,PACKET_MID	=(RECV_PACKET_NUM*PACKET_SIZE).否则  PACKET_MID = 256

        public const int PACKET_MAX = (PACKET_MID * 2);

        // 密码的字节长度
        public const int PASSWORD_LEN = 4;


        // 网络模块搜索最大数
        public const int DEVICE_AMOUNT = 50;
    }
}
