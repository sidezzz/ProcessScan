using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace Processes.Scanning
{

    public class DriverObjectInfo
    {
        public string Name;
        public string Result;
    }

    public class ModuleInfo
    {
        public readonly string Path;
        public readonly string Name;
        public string Result;

        public ModuleInfo(ProcessModule m, string processName)
        {
            Path = m.FileName;
            Name = m.ModuleName;//$"[{processName}]->{m.ModuleName}";
            Result = "Success";
            m.Dispose();
        }
    }
    public class ProcessInfo : IDisposable
    {
        private Process Proc;

        public readonly Icon Icon;
        public readonly string Path;
        public readonly string Name;
        public readonly int PID;

        public ProcessInfo(Process p)
        {
            Proc = p;
            Icon = Icon.ExtractAssociatedIcon(Proc.MainModule.FileName);
            Path = Proc.MainModule.FileName;
            Name = Proc.ProcessName;
            PID = Proc.Id;
        }

        ~ProcessInfo()
        {
            Dispose();
        }
        public void Dispose()
        {
            Proc.Dispose();
            Icon?.Dispose();
        }

        public List<ModuleInfo> GetModules()
        {
            return Proc.Modules.Cast<ProcessModule>().Select(m => new ModuleInfo(m, Name)).ToList();
        }
    }

    sealed class Utils
    {

        public static List<ProcessInfo> GetProcessList()
        {
            List<ProcessInfo> processList = new List<ProcessInfo>();
            foreach(var p in Process.GetProcesses())
            {
                try
                {
                    processList.Add(new ProcessInfo(p));
                }
                catch { }
            }
            return processList;
        }

        public static byte[] StringToByteArray(string str)
        {
            return Enumerable.Range(0, str.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(str.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string ByteArrayToString(byte[] arr)
        {
            var sb = new StringBuilder();
            foreach (var b in arr)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        public static byte[] GetFileHash(byte[] file)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                return md5Hash.ComputeHash(file);
            }
        }

        public static void ParseFileByLine(string file, Action<string> parseLine)
        {
            using (StreamReader readStream = new StreamReader(file))
            {
                while (!readStream.EndOfStream)
                {
                    var line = readStream.ReadLine();
                    try
                    {
                        parseLine(line);
                    }
                    catch (Exception e)
                    {
                        Logger.Log("ParseFileByLine exception: " + e.Message+", "+ line);
                    }
                }
            }
        }
    }
}
