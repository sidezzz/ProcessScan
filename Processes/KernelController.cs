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

namespace Processes
{
    namespace Processes.Kernel
    {
        public enum EScanResult
        {
            EUnknownError = 0,
            ESafe = 1,
            EDetected = 2,
            EInvalidProcess = 3,
            EUnknownIOCTL = 4
        };

        class KernelController
        {
            public readonly string DriverName = "ScannerKernel";
            public EDriverLoadStatus DriverLoadStatus= EDriverLoadStatus.ENotSet;
            public SafeFileHandle DriverHandle=null;
            private uint IO_SCANNER_KERNEL_REQUEST = CTL_CODE(0x22, 0x1337, 0x0, 0x0);

            private static KernelController instance;

            private KernelController()
            {
                LoadKernelScanner();

                DriverHandle = CreateFile("\\\\.\\" + DriverName, FileAccess.ReadWrite, FileShare.ReadWrite,
                    IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
                if (DriverHandle == null || DriverHandle.IsInvalid)
                {
                    DriverLoadStatus = EDriverLoadStatus.EDriverLoadFailed;
                    UnloadDriver();
                }

            }

            public static KernelController getInstance()
            {
                if(instance==null)
                {
                    instance = new KernelController();
                }
                return instance;
            }

            ~KernelController()
            {
                if(DriverHandle!=null)
                {
                    DriverHandle.Close();
                }
                UnloadDriver();
            }

            public EScanResult CheckProcess(int PID)
            {
                EScanResult result = EScanResult.EUnknownError;
                if (DriverLoadStatus == EDriverLoadStatus.EDriverLoaded)
                {
                    var request = new ScanRequest();
                    request.m_ProcessId = PID;
                    request.m_Result = EScanResult.EUnknownError;

                    int bytes = 0;

                    if (!DeviceIoControl(DriverHandle, IO_SCANNER_KERNEL_REQUEST, ref request, (uint)Marshal.SizeOf(request),
                        ref request, (uint)Marshal.SizeOf(request), ref bytes, IntPtr.Zero))
                    {
                        Logger.Log("DeviceIoControl error: "+((NtStatus)Marshal.GetLastWin32Error()).ToString());
                    }

                    result= request.m_Result;
                }
                return result;
            }

            private bool AdjustPrivilege()
            {
                bool Enabled = false;
                return RtlAdjustPrivilege(10u, true, false, ref Enabled) == NtStatus.Success || Enabled;
            }

            private bool AddDriverToRegistry()
            {
                bool ret = false;

                try
                {
                    string RegistryPath = "System\\CurrentControlSet\\Services\\" + DriverName;
                    var key = Registry.LocalMachine.CreateSubKey(RegistryPath);
                    key.SetValue("ImagePath", "System32\\drivers\\" + DriverName + ".sys");
                    key.SetValue("Type", 1);
                    ret = true;
                }
                catch (Exception e)
                {
                    Logger.Log("AddDriverToRegistry exception: " + e.Message);
                }


                return ret;
            }

            private bool LoadKernelScanner()
            {
                bool ret = false;

                try
                {
                    if(!AdjustPrivilege())
                    {
                        DriverLoadStatus = EDriverLoadStatus.EFailedToAdjustPrivileges;
                        throw new Exception("Unable to Adjust Priveleges!");
                    }
                    if(!AddDriverToRegistry())
                    {
                        DriverLoadStatus = EDriverLoadStatus.EFailedToAddDriverToRegistry;
                        throw new Exception("Unable to add Driver to Registry!");
                    }

                    try
                    {
                        File.Delete(Environment.SystemDirectory + "\\drivers\\" + DriverName + ".sys");
                    }
                    catch { }
                    finally
                    {
                        File.Copy(DriverName + ".sys", Environment.SystemDirectory + "\\drivers\\" + DriverName + ".sys");
                    }

                    var SourceRegistryUnicode=new UNICODE_STRING("\\Registry\\Machine\\System\\CurrentControlSet\\Services\\" + DriverName);

                    var Status = NtLoadDriver(ref SourceRegistryUnicode);
                    if(Status!=NtStatus.Success)
                    {
                        DriverLoadStatus = EDriverLoadStatus.EDriverLoadFailed;
                        throw new Exception("Unable to Load Driver, Status: "+Status.ToString());
                    }
                    ret = true;
                    DriverLoadStatus = EDriverLoadStatus.EDriverLoaded;
                }
                catch (Exception e)
                {
                    Logger.Log("LoadKernelScanner exception: " + e.Message);
                    UnloadDriver();
                }

                return ret;
            }


            private void UnloadDriver()
            {
                try
                {
                    var SourceRegistryUnicode = new UNICODE_STRING("\\Registry\\Machine\\System\\CurrentControlSet\\Services\\" + DriverName);
                    var Status = NtUnloadDriver(ref SourceRegistryUnicode);

                    if (Status != NtStatus.Success)
                    {
                        throw new Exception("Unable to Unload Driver, Status: " + Status.ToString());
                    }
                }
                catch (Exception e)
                {
                    Logger.Log("UnloadDriver exception: " + e.Message);
                }

                try
                {
                    string RegistryPath = "System\\CurrentControlSet\\Services\\" + DriverName;
                    Registry.LocalMachine.DeleteSubKey(RegistryPath);
                }
                catch (Exception e)
                {
                    Logger.Log("UnloadDriver exception: " + e.Message);
                }

                try
                {
                    File.Delete(Environment.SystemDirectory + "\\drivers\\" + DriverName + ".sys");
                }
                catch (Exception e)
                {
                    Logger.Log("UnloadDriver exception: " + e.Message);
                }

            }

            public enum EDriverLoadStatus
            {
                ENotSet,
                EDriverLoaded,
                EFailedToAdjustPrivileges,
                EFailedToAddDriverToRegistry,
                EDriverLoadFailed
            }


            [StructLayout(LayoutKind.Sequential)]
            private struct ScanRequest
            {
                public int m_ProcessId;
                public EScanResult m_Result;
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
                    Length = str.Length*2;
                    MaximumLenght = str.Length*2;
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

            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            private static extern bool DeviceIoControl(SafeFileHandle hDevice, uint IoControlCode,
                ref ScanRequest InBuffer, uint nInBufferSize,
                ref ScanRequest OutBuffer, uint nOutBufferSize,
                ref int pBytesReturned, IntPtr Overlapped);

            private static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
            {
                return (((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method));
            }
        }
    }
}
