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

        public ModuleInfo(ProcessModule m)
        {
            Path = m.FileName.ToLower();
            Name = m.ModuleName;//$"[{processName}]->{m.ModuleName}";
            Result = "Success";
            m.Dispose();
        }
    }
    public class ProcessInfo : IDisposable
    {
        public readonly Icon Icon;
        public readonly string Path;
        public readonly string Name;
        public readonly int PID;
        public readonly List<ModuleInfo> Modules;

        public ProcessInfo(Process proc)
        {
            Icon = Icon.ExtractAssociatedIcon(proc.MainModule.FileName);
            Path = proc.MainModule.FileName;
            Name = proc.ProcessName;
            PID = proc.Id;
            Modules = proc.Modules.Cast<ProcessModule>().Select(m => new ModuleInfo(m)).ToList();
        }

        ~ProcessInfo()
        {
            Dispose();
        }
        public void Dispose()
        {
            Icon?.Dispose();
        }
    }

    static class Utils
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

        public static void ParseFileByLine(string filePath, Action<string> parseLine)
        {
            using (StreamReader readStream = new StreamReader(filePath))
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
                        Logger.Log($"ParseFileByLine exception: {e.Message}, {line}");
                    }
                }
            }
        }
    }
}
