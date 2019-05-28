using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32.SafeHandles;
using Processes.Processes.Kernel;

namespace Processes.Driver
{
    abstract class BaseDriver
    {
        public string DeviceName { get; }
        public string FilePath { get; }

        protected SafeFileHandle DriverHandle = null;

        public BaseDriver(string path, string deviceName)
        {
            DeviceName = deviceName;
            FilePath = path;
            EnsureLoaded();
        }


        private void AdjustPrivilege()
        {
            bool enabled = false;
            RtlAdjustPrivilege(10u, true, false, ref enabled);
            if(!enabled)
            {
                throw new UnauthorizedAccessException("Can't adjust prevelege!");
            }
        }

        private string AddDriverToRegistry()
        {
            string registryPath = "System\\CurrentControlSet\\Services\\" + Path.GetFileNameWithoutExtension(FilePath);
            var key = Registry.LocalMachine.CreateSubKey(registryPath);
            key.SetValue("ImagePath", "System32\\drivers\\" + Path.GetFileName(FilePath));
            return "\\Registry\\Machine\\" + registryPath;
        }

        private void LoadDriver()
        {
            try
            {
                File.Delete(Environment.SystemDirectory + "\\drivers\\" + Path.GetFileName(FilePath));
            }
            catch { }
            File.Copy(FilePath, Environment.SystemDirectory + "\\drivers\\" + Path.GetFileName(FilePath));

            var registryPath = AddDriverToRegistry();

            AdjustPrivilege();

            var unicodePath = new UNICODE_STRING(registryPath);
            var status = NtLoadDriver(ref unicodePath);
            if(status != NtStatus.Success)
            {
                throw new Exception("NtLoadDriver failed with " + status.ToString());
            }
        }

        private void EnsureLoaded()
        {
            DriverHandle = CreateFile("\\\\.\\" + DeviceName, FileAccess.ReadWrite, FileShare.ReadWrite,
                    IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            if(DriverHandle.IsInvalid)
            {
                try
                {
                    LoadDriver();
                    DriverHandle = CreateFile("\\\\.\\" + DeviceName, FileAccess.ReadWrite, FileShare.ReadWrite,
                    IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
                }
                catch(Exception e)
                {
                    Logger.Log("LoadDriver exception: " + e.Message);
                }
            }
        }

        private bool EnsureUnloaded()
        {
            return false;
        }



        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct UNICODE_STRING
        {
            public int Length;
            public int MaximumLenght;
            private string Buffer;

            public UNICODE_STRING(string str)
            {
                Buffer = str;
                Length = str.Length * 2;
                MaximumLenght = str.Length * 2;
            }
        }

        [DllImport("ntdll.dll")]
        private static extern NtStatus RtlAdjustPrivilege(uint Privelege, bool Enable, bool CurrentThread, ref bool Enabled);
        [DllImport("ntdll.dll")]
        private static extern NtStatus NtLoadDriver(ref UNICODE_STRING SourceRegistry);
        [DllImport("ntdll.dll")]
        private static extern NtStatus NtUnloadDriver(ref UNICODE_STRING SourceRegistry);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess,
            [MarshalAs(UnmanagedType.U4)] FileShare fileShare,
            IntPtr securityAttributes, // optional SECURITY_ATTRIBUTES structure can be passed
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr template);
    }
}
