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

        public bool IsValid => !DriverHandle.IsInvalid;

        public BaseDriver(string path, string deviceName)
        {
            DeviceName = deviceName;
            FilePath = path;
            EnsureLoaded();
        }

        ~BaseDriver()
        {
            EnsureUnloaded();
        }

        private void AdjustPrivilege()
        {
            bool enabled = false;
            var status = RtlAdjustPrivilege(10u, true, false, ref enabled);
            if (status != NtStatus.Success && !enabled)
            {
                throw new UnauthorizedAccessException($"Can't adjust prevelege! {status.ToString()}");
            }
        }

        private void RemoveDriverFromRegistry()
        {
            try
            {
                string registryPath = "System\\CurrentControlSet\\Services\\" + Path.GetFileNameWithoutExtension(FilePath);
                Registry.LocalMachine.DeleteSubKey(registryPath);
            }
            catch { }
        }

        private void RemoveFromSystemDirectory()
        {
            try
            {
                File.Delete(Environment.SystemDirectory + "\\drivers\\" + Path.GetFileName(FilePath));
            }
            catch { }
        }

        private string AddDriverToRegistry()
        {
            string registryPath = "System\\CurrentControlSet\\Services\\" + Path.GetFileNameWithoutExtension(FilePath);
            using (var key = Registry.LocalMachine.CreateSubKey(registryPath))
            {
                key.SetValue("ImagePath", "System32\\drivers\\" + Path.GetFileName(FilePath));
                key.SetValue("Type", 1);
            }
            return "\\Registry\\Machine\\" + registryPath;
        }

        protected void LoadDriver()
        {
            RemoveFromSystemDirectory();
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

        protected void EnsureLoaded()
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
                    if(DriverHandle.IsInvalid)
                    {
                        throw new Exception($"Can't get driver handle {"\\\\.\\" + DeviceName}, {Marshal.GetLastWin32Error()}");
                    }
                }
                catch(Exception e)
                {
                    Logger.Log($"LoadDriver exception: {e.Message}");
                    EnsureUnloaded();
                }
            }
        }

        protected void EnsureUnloaded()
        {
            if(!DriverHandle.IsInvalid)
            {
                try
                {
                    AddDriverToRegistry();
                    AdjustPrivilege();

                    var registryUnicode = new UNICODE_STRING("\\Registry\\Machine\\System\\CurrentControlSet\\Services\\"
                        + Path.GetFileNameWithoutExtension(FilePath));
                    var status = NtUnloadDriver(ref registryUnicode);
                    if (status != NtStatus.Success)
                    {
                        Logger.Log($"NtUnloadDriver failed {status}");
                    }
                }
                catch (Exception e)
                {
                    Logger.Log($"LoadDriver exception: {e.Message}");
                    EnsureUnloaded();
                }

                DriverHandle.Close();
            }

            RemoveDriverFromRegistry();
            RemoveFromSystemDirectory();
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
        static extern SafeFileHandle CreateFile(
            string lpFileName,
            [MarshalAs(UnmanagedType.U4)] FileAccess dwDesiredAccess,
            [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode,
            IntPtr lpSecurityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes,
            IntPtr hTemplateFile);
    }
}
