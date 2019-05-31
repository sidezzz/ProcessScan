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

        public struct DriverObjectArray
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 300)]
            public DriverObject[] Array;
            public DriverObjectArray(int count)
            {
                Array = new DriverObject[300];
            }
        }

        public DriverScanner() :base("ScannerKernel.sys", "ScannerKernel")
        {

        }

        public List<DriverObjectInfo> ScanKernel()
        {
            if(DriverHandle.IsInvalid)
            {
                throw new Exception("No connection with kernel driver!");
            }
            List<DriverObjectInfo> ret = new List<DriverObjectInfo>();

            int count = 250;
            IntPtr objectArray = Marshal.AllocHGlobal(Marshal.SizeOf<DriverObject>() * count);
            uint bytesRead = 0;
            if (ReadFile(DriverHandle, objectArray, Marshal.SizeOf<DriverObject>() * count, out bytesRead, IntPtr.Zero))
            {
                for (int a = 0; a < bytesRead / Marshal.SizeOf<DriverObject>(); a++)
                {
                    var obj = Marshal.PtrToStructure<DriverObject>(objectArray + Marshal.SizeOf<DriverObject>() * a);
                    var info = new DriverObjectInfo();
                    info.Name = obj.Name;
                    info.Result = obj.Result.ToString();
                    ret.Add(info);
                }
            }
            else
            {
                Logger.Log($"ReadFile error: {Marshal.GetLastWin32Error()}");
            }
            Marshal.FreeHGlobal(objectArray);

            return ret;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadFile(
            SafeFileHandle hFile,
            IntPtr OutBuffer,
            int nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            IntPtr lpOverlapped);
    }
}
