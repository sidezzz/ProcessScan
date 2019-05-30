using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Processes.Scanning;
using Microsoft.Win32.SafeHandles;
using Processes.Processes.Kernel;

namespace Processes.Driver
{
    class DriverScanner : BaseDriver
    {
        public enum EResult : int
        {
            Success = 0,
            Hijacked = 1
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
        public struct DriverObject
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string Name;
            public EResult Result;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct DriverObjectArray
        {
            public int Count;
            public int Max;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
            public DriverObject[] Array;
            public DriverObjectArray(int count)
            {
                Array = new DriverObject[200];
                Count = 0;
                Max = 200;
            }
        }

        public struct DriverObjectArray2
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
            public DriverObject[] Array;
            public DriverObjectArray2(int count)
            {
                Array = new DriverObject[200];
            }
        }

        private uint IO_SCANNER_KERNEL_REQUEST = CTL_CODE(0x22, 0x1337, 0x0, 0x0);

        public DriverScanner() :base("ScannerKernel.sys", "ScannerKernel")
        {

        }


        /*[DllImport("PEParser.dll")]
        private static extern void TestFunc([In, Out] ref DriverObjectArray objects);
        public List<DriverObjectInfo> ScanKernel()
        {
            List<DriverObjectInfo> ret = new List<DriverObjectInfo>();
            DriverObjectArray objs = new DriverObjectArray(5);
            TestFunc(ref objs);

            foreach (var obj in objs.Array)
            {
                var info = new DriverObjectInfo();
                info.Name = obj.Name;
                info.Result = obj.Result.ToString();
                ret.Add(info);
            }

            return ret;
        }*/

        public List<DriverObjectInfo> ScanKernel2()
        {
            List<DriverObjectInfo> ret = new List<DriverObjectInfo>();
            DriverObjectArray objs = new DriverObjectArray(0);
            int bytes = 0;
            Logger.Log($"DeviceIoControl {Marshal.SizeOf(objs)}");
            if (DeviceIoControl(DriverHandle, IO_SCANNER_KERNEL_REQUEST, ref objs, (uint)Marshal.SizeOf(objs),
                ref objs, (uint)Marshal.SizeOf(objs), ref bytes, IntPtr.Zero))
            {
                for(int a=0; a<objs.Count; a++)
                {
                    var info = new DriverObjectInfo();
                    info.Name = objs.Array[a].Name;
                    info.Result = objs.Array[a].Result.ToString();
                    ret.Add(info);
                }
            }
            else
            {
                Logger.Log($"DeviceIoControl error: {Marshal.GetLastWin32Error()}");
            }

            return ret;
        }

        public List<DriverObjectInfo> ScanKernel()
        {
            List<DriverObjectInfo> ret = new List<DriverObjectInfo>();
            DriverObjectArray2 objs = new DriverObjectArray2(0);
            uint bytes = 0;
            Logger.Log($"ReadFile {Marshal.SizeOf(objs)}");
            if (ReadFile(DriverHandle, ref objs, (uint)Marshal.SizeOf(objs), out bytes, IntPtr.Zero))
            {
                for (int a = 0; a < bytes / Marshal.SizeOf<DriverObject>(); a++)
                {
                    var info = new DriverObjectInfo();
                    info.Name = objs.Array[a].Name;
                    info.Result = objs.Array[a].Result.ToString();
                    ret.Add(info);
                }
            }
            else
            {
                Logger.Log($"ReadFile error: {Marshal.GetLastWin32Error()}");
            }

            return ret;
        }


        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool DeviceIoControl(SafeFileHandle hDevice, uint IoControlCode,
                [In, Out] ref DriverObjectArray InBuffer, uint nInBufferSize,
                [In, Out] ref DriverObjectArray OutBuffer, uint nOutBufferSize,
                ref int pBytesReturned, IntPtr Overlapped);

        private static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
        {
            return (((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method));
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadFile(
            SafeFileHandle hFile,
            [In, Out] ref DriverObjectArray2 OutBuffer,
            uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            IntPtr lpOverlapped);
    }
}
