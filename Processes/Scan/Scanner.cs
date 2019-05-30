using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace Processes.Scan
{
    public class ProcessInfo
    {
        public Icon Icon;
        public string Path;
        public string Name;
        public int PID;
    }

    public class ModuleInfo
    {
        public string Path;
        public string Name;
        public string Result;
    }

    class Scanner
    {
        private List<IModuleScan> ScanMethods;

        private ConcurrentDictionary<string, string> ScanResultCache;
        private int CacheWins;
        private int CacheMisses;


        public Scanner()
        {
            ScanResultCache = new ConcurrentDictionary<string, string>();
            ScanMethods = new List<IModuleScan>();
            ScanMethods.Add(new WinTrustScan());
            ScanMethods.Add(new HSBScan());
            ScanMethods.Add(new NDUScan());
        }


        public List<ProcessInfo> GetProcessList()
        {
            var processList = new List<ProcessInfo>();
            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    var info = new ProcessInfo();
                    info.Name = process.MainModule.ModuleName;
                    info.Path = process.MainModule.FileName;
                    info.Icon = Icon.ExtractAssociatedIcon(process.MainModule.FileName);
                    info.PID = process.Id;
                    processList.Add(info);
                }
                catch { }
                /*catch(System.ComponentModel.Win32Exception e)
                {
                    Logger.Log("Refresh exception on process: \"" + process.ProcessName + "\" : " + e.Message);
                }
                catch(Exception e)
                {
                    Logger.Log("Refresh exception: " + e.Message);
                }*/
            }
            return processList;
        }



        public void BeginScan(List<ProcessInfo> processList, Action<ModuleInfo> addModuleCallback)
        {
            CacheWins = 0;
            CacheMisses = 0;

            Logger.Log("Started scanning...");
            var startTime = DateTime.Now;
            Parallel.ForEach(processList, (process) =>
            {
                try
                {
                    ScanProcess(process, addModuleCallback);                
                }
                catch (Exception e)
                {
                    Logger.Log($"BeginScan exception: {e.Message}");
                }
            });
            var scanTime = DateTime.Now - startTime;
            Logger.Log($"Scan completed, scan time: {scanTime.TotalSeconds} seconds, " +
                $"{CacheMisses} unique files scanned, " +
                $"total count {(CacheWins + CacheMisses)}");
        }

        public void ScanProcess(ProcessInfo info, Action<ModuleInfo> addModuleCallback)
        {
            try
            {
                var proc = Process.GetProcessById(info.PID);
                var modules = proc.Modules;
                Parallel.ForEach(modules.Cast<ProcessModule>(), (module) =>
                {
                    ScanModule(proc, module, addModuleCallback);
                });
            }
            catch (Exception e)
            {
                Logger.Log($"ScanProcess exception: {e.Message}");
            }
        }


        public void ScanModule(Process proc, ProcessModule module, Action<ModuleInfo> addModuleCallback)
        {
            try
            {
                var info = new ModuleInfo();
                info.Path = module.FileName;
                info.Name = $"[{proc.ProcessName}]->{module.ModuleName}";

                if (!ScanResultCache.TryGetValue(info.Path, out info.Result))
                {
                    byte[] cachedFile = File.ReadAllBytes(info.Path); ;
                    foreach (var method in ScanMethods)
                    {
                        if (method.Scan(info.Path, ref info.Result, cachedFile) == ScanStatus.Stop)
                        {
                            break;
                        }
                    }
                    ScanResultCache.TryAdd(info.Path, info.Result);
                    Interlocked.Increment(ref CacheMisses);
                }
                else
                {
                    Interlocked.Increment(ref CacheWins);
                }
                addModuleCallback(info);

            }
            catch (Exception e)
            {
                Logger.Log($"ScanModule exception: {e.Message}");
            }
        }
    }
}
