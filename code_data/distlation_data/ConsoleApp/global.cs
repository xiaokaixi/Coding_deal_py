using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace ConsoleApp1
{
    class global
    {
        // 
        public static bool sjConnect = false;//
        public static bool MultiReadStatus = false;
        public static int m_connect_type = transfer.CONNECT_SERIAL;
        public static int m_cbBaute = 0;        // 
        public static int m_cbCOM = 0;          // 
        public static string ipStr;
        public const byte FASTID_ON = 1;
        public const byte FASTID_OFF = 0;
        public const byte CARRIER_ON = 1;
        public const byte CARRIER_OFF = 0;
        public const byte TAGFOCUS_ON = 1;
        public const byte TAGFOCUS_OFF = 0;
        public static int InStatistics = 0;
        public static int OutStatistics = 0;

        public const int OPER_OK = 0;          // sr api

        public const int READ_FLAG = 1;
        public const int WRITE_FLAG = 2;

        // 
        public const int OUTPUT_FREQUENCY_NUM = 128;


        // packet
        public const int RECV_PACKET_NUM = 50;			// 
        public const int PACKET_128 = 128;
        public const int PACKET_MIN = 32;
        public const int FOUR_CHANNELS = 1;          // 10
        public const int PACKET_SIZE = 23;			// 2322
        public const int PACKET_MID = (RECV_PACKET_NUM * PACKET_SIZE);  //  RECV_PACKET_NUM > 10,PACKET_MID	=(RECV_PACKET_NUM*PACKET_SIZE).  PACKET_MID = 256
        //     public const int PACKET_MID = (256);  //  RECV_PACKET_NUM > 10,PACKET_MID	=(RECV_PACKET_NUM*PACKET_SIZE).  PACKET_MID = 256

        public const int PACKET_MAX = (PACKET_MID * 2);

        // 
        public const int PASSWORD_LEN = 4;


        // 
        public const int DEVICE_AMOUNT = 50;
    }
}
