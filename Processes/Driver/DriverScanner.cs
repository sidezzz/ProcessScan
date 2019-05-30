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
            List<DriverObjectInfo> ret = new List<DriverObjectInfo>();

            var driverObjects = new DriverObjectArray(0);
            uint bytesRead = 0;
            if (ReadFile(DriverHandle, ref driverObjects, (uint)Marshal.SizeOf(driverObjects), out bytesRead, IntPtr.Zero))
            {
                for (int a = 0; a < bytesRead / Marshal.SizeOf<DriverObject>(); a++)
                {
                    var info = new DriverObjectInfo();
                    info.Name = driverObjects.Array[a].Name;
                    info.Result = driverObjects.Array[a].Result.ToString();
                    ret.Add(info);
                }
            }
            else
            {
                Logger.Log($"ReadFile error: {Marshal.GetLastWin32Error()}");
            }

            return ret;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadFile(
            SafeFileHandle hFile,
            [In, Out] ref DriverObjectArray OutBuffer,
            uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            IntPtr lpOverlapped);
    }
}
