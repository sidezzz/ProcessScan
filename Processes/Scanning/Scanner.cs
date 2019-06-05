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

namespace Processes.Scanning
{
    class Scanner
    {
        private List<IModuleScan> ScanMethods;

        private ConcurrentDictionary<string, string> ScanResultCache;
        private int CacheWins;
        private int CacheMisses;

        public readonly Driver.DriverScanner KernelScanner;


        public Scanner()
        {
            ScanResultCache = new ConcurrentDictionary<string, string>();
            ScanMethods = new List<IModuleScan>();
            ScanMethods.Add(new WinTrustScan());
            ScanMethods.Add(new HSBScan());
            ScanMethods.Add(new NDUScan());
            KernelScanner = new Driver.DriverScanner();
        }


        public void BeginScan(List<ProcessInfo> processList, Action<ModuleInfo> addModuleCallback, Action moduleScannedCallback)
        {
            CacheWins = 0;
            CacheMisses = 0;

            Logger.Log("Started scanning...");
            var sw = Stopwatch.StartNew();
            Parallel.ForEach(processList, (process) =>
            {
                try
                {
                    ScanProcess(process, addModuleCallback, moduleScannedCallback);                
                }
                catch (Exception e)
                {
                    Logger.Log($"BeginScan exception: {e.Message}");
                }
            });
            Logger.Log($"Scan completed, scan time: {sw.Elapsed.TotalSeconds} seconds, " +
                $"{CacheMisses} unique files scanned, " +
                $"total count {(CacheWins + CacheMisses)}");
        }

        public void ScanProcess(ProcessInfo processInfo, Action<ModuleInfo> addModuleCallback, Action moduleScannedCallback)
        {
            try
            {
                Parallel.ForEach(processInfo.Modules, (module) =>
                {
                    var sw = Stopwatch.StartNew();
                    ScanModule(module, addModuleCallback);
                    //if (sw.Elapsed.TotalSeconds > 1)
                    //{
                    //    Logger.Log($"Scan {module.Path}, scan time: {sw.Elapsed.TotalSeconds} seconds");
                    //}

                    moduleScannedCallback();
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
                    ScanResultCache.TryAdd(moduleInfo.Path, null);
                    byte[] cachedFile = File.ReadAllBytes(moduleInfo.Path);

                    foreach (var method in ScanMethods)
                    {
                        if (method.Scan(moduleInfo.Path, ref moduleInfo.Result, cachedFile) == ScanStatus.Stop)
                        {
                            break;
                        }
                    }

                    ScanResultCache[moduleInfo.Path] = moduleInfo.Result;
                    Interlocked.Increment(ref CacheMisses);
                }
                else
                {
                    Interlocked.Increment(ref CacheWins);
                }

                if (moduleInfo.Result != null)
                {
                    addModuleCallback(moduleInfo);
                }

            }
            catch (Exception e)
            {
                Logger.Log($"ScanModule exception: {e.Message}");
            }
        }

        public List<DriverObjectInfo> ScanKernel()
        {
            return KernelScanner.ScanKernel();
        }
    }
}
