using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace Processes.Scan
{
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

        public void ScanProcess(ProcessInfo processInfo, Action<ModuleInfo> addModuleCallback)
        {
            try
            {
                Parallel.ForEach(processInfo.GetModules(), (module) =>
                {
                    ScanModule(module, addModuleCallback);
                });
            }
            catch (Exception e)
            {
                Logger.Log($"ScanProcess exception: {e.Message}");
            }
        }


        public void ScanModule(ModuleInfo moduleInfo, Action<ModuleInfo> addModuleCallback)
        {
            try
            {
                if (!ScanResultCache.TryGetValue(moduleInfo.Path, out moduleInfo.Result))
                {
                    byte[] cachedFile = File.ReadAllBytes(moduleInfo.Path); ;
                    foreach (var method in ScanMethods)
                    {
                        if (method.Scan(moduleInfo.Path, ref moduleInfo.Result, cachedFile) == ScanStatus.Stop)
                        {
                            break;
                        }
                    }
                    ScanResultCache.TryAdd(moduleInfo.Path, moduleInfo.Result);
                    Interlocked.Increment(ref CacheMisses);
                }
                else
                {
                    Interlocked.Increment(ref CacheWins);
                }
                addModuleCallback(moduleInfo);

            }
            catch (Exception e)
            {
                Logger.Log($"ScanModule exception: {e.Message}");
            }
        }
    }
}
