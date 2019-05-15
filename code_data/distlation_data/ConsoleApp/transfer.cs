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
        public char[] strMAC;	//MAC,00.09.F6.01.02.03
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public char[] strIP;		//IP10.1.1.1 
        public uint dwModel;	// 
        public uint dwVersion;	//
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] strName;	//
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] pbUnused;//
    }



    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct C2000NETSETTING
    {
        public int nNetIndex;	//0
        public int bUseStaticIP; //0IP1IP
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public char[] ipAddr;	 //C3000IP
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public char[] ipNetMask;  //
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public char[] ipGateway;  //IP
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public char[] ipDnsServer;   //DNSIP
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
        public byte[] pbUnused;//
    }



    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct C2000SOCKETSETTING
    {
        public int nComIndex;	//
        //
        public int nSocketIndex;	//0
        public int nWorkMode; //0TCP1TCP2UDP1
        //3UDP24
        public int nLocalPort;  //
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public char[] szPeerName;	//IP
        public int nPeerPort;  //
        public int bUseSocket;  //Socket01
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
        public byte[] pbUnused;//
    }



    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct C2000COMSETTING
    {
        public int nComIndex;	//0
        public int nBaudrate;	//
        public int nDatabit;	  //
        public int nParity;   //
        public int nStopbit;    //
        public int nFlowCtrl;   //
        public uint nIntervalTimeout;   //
        public uint nMaxFrameLength;   //
        public int nInceptBytesLength;	//
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] pbInceptBytes;	//
        public int nTermBytesLength;	//
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] pbTermBytes;	//
        public int nWorkMode;  //0  1232    2485    3422
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
        public byte[] pbUnused;//
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct device_t
    {
        public DEVICEINFO device_info;	// 
        public C2000NETSETTING net_setting;	// 
        public C2000SOCKETSETTING socket_setting;	// socket
        public C2000COMSETTING com_setting;	// 
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct net_device_t
    {
        public int device_num;						// 
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
        public extern static int transfer_init(ref net_device_t param);		    //  
        [DllImport("ReaderApi.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static int transfer_open(ref device_t param);			    // 
        [DllImport("ReaderApi.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static int transfer_open_net(char[] connect_ip, int port);			    // 
        [DllImport("ReaderApi.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static int transfer_close();							    // 
        [DllImport("ReaderApi.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static int transfer_send_set(ref device_t param);		    // 
        [DllImport("ReaderApi.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static int transfer_recv_set();						    // 
    }
}
