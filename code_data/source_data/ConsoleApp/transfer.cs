using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ConsoleApp1
{
    // serial
    //       [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct uart_open_t
    {
        //       public byte[] com_name;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string com_name;
        public int com_baudrate;
        public int error_code;

        public uart_open_t(string uart_name, int baudrate)
        {
            com_name = uart_name;
            com_baudrate = baudrate;
            error_code = 0;
        }
    }

    //      [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct uart_close_t
    {
        public int error_code;
    }

    //       [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct uart_send_t
    {
        public int error_code;
    }

    //    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct uart_recv_t
    {
        public int error_code;
    }

    // net
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct DEVICEINFO
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public char[] strMAC;	//转换器MAC地址,格式为00.09.F6.01.02.03
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public char[] strIP;		//转换器IP地址，格式为10.1.1.1 
        public uint dwModel;	//转换器型号 
        public uint dwVersion;	//版本号
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] strName;	//名字
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] pbUnused;//未使用
    }



    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct C2000NETSETTING
    {
        public int nNetIndex;	//索引号，从0开始编号
        public int bUseStaticIP; //0：自动IP；1：指定IP
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public char[] ipAddr;	 //C3000的IP地址
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public char[] ipNetMask;  //网络所在掩码
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public char[] ipGateway;  //网关的IP地址
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public char[] ipDnsServer;   //DNS服务器的IP地址
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
        public byte[] pbUnused;//未使用
    }



    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct C2000SOCKETSETTING
    {
        public int nComIndex;	//串口号，该套接口参数是属于哪个串口的，
        //如果不属于任何串口，该值为负数，具体值另行解释
        public int nSocketIndex;	//该套接口在某个串口中编号，从0开始编号
        public int nWorkMode; //0：TCP客户；1：TCP服务器；2：UDP1或自动；
        //3：UDP2；4：自动，根据具体的产品而定
        public int nLocalPort;  //本地端口
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public char[] szPeerName;	//对方主机的域名或IP
        public int nPeerPort;  //对方端口
        public int bUseSocket;  //是否使用Socket。0：否，1：是
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
        public byte[] pbUnused;//未使用
    }



    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct C2000COMSETTING
    {
        public int nComIndex;	//串口号，该串口设置参数是属于哪个串口的，从0开始编号
        public int nBaudrate;	//波特率
        public int nDatabit;	  //数据位数
        public int nParity;   //校验方式
        public int nStopbit;    //停止位数
        public int nFlowCtrl;   //流控方式
        public uint nIntervalTimeout;   //间隔超时时间（最少发送时间）
        public uint nMaxFrameLength;   //最大帧长度（最小发送字节）
        public int nInceptBytesLength;	//包头长度
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] pbInceptBytes;	//包头字符
        public int nTermBytesLength;	//包尾长度
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] pbTermBytes;	//包尾字符
        public int nWorkMode;  //0：不变  1：232    2：485    3：422
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
        public byte[] pbUnused;//未使用
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct device_t
    {
        public DEVICEINFO device_info;	// 设备信息数组
        public C2000NETSETTING net_setting;	// 模块的网络信息
        public C2000SOCKETSETTING socket_setting;	// 模块的socket信息
        public C2000COMSETTING com_setting;	// 模块的串口信息
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct net_device_t
    {
        public int device_num;						// 转换器的数量
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = global.DEVICE_AMOUNT)]
        public device_t[] net_device;
    }

    class transfer
    {
        public const int CONNECT_SERIAL = 101;
        public const int CONNECT_NET = 102;

        // serial
        [DllImport("ReaderApi.dll", EntryPoint = "transfer_open", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static int transfer_open(ref uart_open_t param);
        [DllImport("ReaderApi.dll", EntryPoint = "transfer_close", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static int transfer_close(ref uart_close_t _uart_close);

        // net
        [DllImport("ReaderApi.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static int transfer_init(ref net_device_t param);		    // 初始化 
        [DllImport("ReaderApi.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static int transfer_open(ref device_t param);			    // 打开通讯
        [DllImport("ReaderApi.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static int transfer_open_net(char[] connect_ip, int port);			    // 打开通讯
        [DllImport("ReaderApi.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static int transfer_close();							    // 关闭通讯
        [DllImport("ReaderApi.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static int transfer_send_set(ref device_t param);		    // 发送设置
        [DllImport("ReaderApi.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static int transfer_recv_set();						    // 接收设置
    }
}
