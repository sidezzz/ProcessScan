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

        private readonly Driver.DriverScanner KernelScanner;
        public bool IsDriverLoaded => KernelScanner.IsValid;

        public delegate void ModuleScannedHandler(ModuleInfo module);
        public event ModuleScannedHandler ModuleScanned;


        public Scanner()
        {
            ScanResultCache = new ConcurrentDictionary<string, string>();
            ScanMethods = new List<IModuleScan>();

            //adding scan methods in complexity order, fastest method is the first
            ScanMethods.Add(new WinTrustScan());
            ScanMethods.Add(new HSBScan());
            ScanMethods.Add(new NDUScan());
            KernelScanner = new Driver.DriverScanner();
        }


        public void BeginScan(List<ProcessInfo> processList)
        {
            CacheWins = 0;
            CacheMisses = 0;

            Logger.Log("Started scanning...");
            var sw = Stopwatch.StartNew();
            Parallel.ForEach(processList, (process) =>
            {
                try
                {
                    ScanProcess(process);                
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

        private void ScanProcess(ProcessInfo processInfo)
        {
            try
            {
                Parallel.ForEach(processInfo.Modules, (module) =>
                {
                    //var sw = Stopwatch.StartNew();
                    ScanModule(module);
                    //if (sw.Elapsed.TotalSeconds > 1)
                    //{
                    //    Logger.Log($"Scan {module.Path}, scan time: {sw.Elapsed.TotalSeconds} seconds");
                    //}

                    ModuleScanned(module); //activating event
                });
            }
            catch (Exception e)
            {
                Logger.Log($"ScanProcess exception: {e.Message}");
            }
        }


        private void ScanModule(ModuleInfo moduleInfo)
        {
            try
            {
                if (ScanResultCache.TryAdd(moduleInfo.Path, null)) //avoiding multiple threads scanning same file
                {
                    var cachedFile = new FileCache(moduleInfo.Path);

                    foreach (var method in ScanMethods)
                    {
                        if (method.Scan(cachedFile, ref moduleInfo.Result) == ScanStatus.Stop)
                        {
                            break;
                        }
                    }

                    ScanResultCache[moduleInfo.Path] = moduleInfo.Result; //add result into cache
                    Interlocked.Increment(ref CacheMisses); 
                }
                else
                {
                    moduleInfo.Result = ScanResultCache[moduleInfo.Path]; //get result from cache
                    Interlocked.Increment(ref CacheWins);
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
