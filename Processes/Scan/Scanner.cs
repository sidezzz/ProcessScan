using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Drawing;
using System.Diagnostics;
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
        public Processes.Kernel.EScanResult Result;
    }

    public class ModuleInfo
    {
        public string Path;
        public string Name;
        public string Result;
    }

    class Scanner
    {
        private List<IModuleScan> scanMethods;

        private ConcurrentDictionary<string, string> cachedScanResult;
        private int cacheWins;


        public Scanner()
        {
            var k=Processes.Kernel.KernelController.getInstance();
            cachedScanResult = new ConcurrentDictionary<string, string>();
            scanMethods = new List<IModuleScan>();
            scanMethods.Add(new WinTrustScan());
            scanMethods.Add(new NDUScan());
            scanMethods.Add(new HSBScan());
        }


        public List<ProcessInfo> Refresh()
        {
            var procs = Process.GetProcesses();
            var ProcessList= new List<ProcessInfo>();
            foreach (var p in procs)
            {
                try
                {
                    var info = new ProcessInfo();
                    info.Name = p.MainModule.ModuleName;
                    info.Path = p.MainModule.FileName;
                    info.Icon = Icon.ExtractAssociatedIcon(p.MainModule.FileName);
                    info.PID = p.Id;
                    ProcessList.Add(info);
                    
                }
                catch(System.ComponentModel.Win32Exception e)
                {
                    Logger.Log("Refresh exception on process: \"" + p.ProcessName + "\" : " + e.Message);
                }
                catch(Exception e)
                {
                    Logger.Log("Refresh exception: " + e.Message);
                }
            }
            return ProcessList;
        }



        public void BeginScan(List<ProcessInfo> procs, Action<ModuleInfo> modulecallback, Action<ProcessInfo> infocallback)
        {
            //cachedScanResult.Clear();
            cacheWins = 0;

            Logger.Log("Started scanning...");
            DateTime start = DateTime.Now;
            Parallel.ForEach(procs, (proc) =>
            {
                try
                {
                    ScanProcess(proc, modulecallback, infocallback);                
                }
                catch (Exception e)
                {
                    Logger.Log("BeginScan exception: " + e.Message);
                }
            });
            var scantime = DateTime.Now - start;
            Logger.Log("Scan completed, scan time: " + scantime.TotalSeconds + " seconds, "
                + cachedScanResult.Count+" unique files scanned, total count "+
                (cachedScanResult.Count+cacheWins).ToString());
        }

        public void ScanProcess(ProcessInfo info, Action<ModuleInfo> modulecallback, Action<ProcessInfo> infocallback)
        {
            try
            {
                var proc = Process.GetProcessById(info.PID);
                var modules = proc.Modules;
                info.Result=Processes.Kernel.KernelController.getInstance().CheckProcess(info.PID);
                infocallback(info);
                Parallel.ForEach(modules.Cast<ProcessModule>(), (m) =>
                {
                    ScanModule(proc,m, modulecallback);
                });
            }
            catch (Exception e)
            {
                Logger.Log("ScanProcess exception: " + e.Message);
            }
        }


        public void ScanModule(Process proc, ProcessModule m, Action<ModuleInfo> callback)
        {
            try
            {
                var info = new ModuleInfo();
                info.Path = m.FileName;
                info.Name = "[" + proc.ProcessName + "]->" + m.ModuleName;

                if (!cachedScanResult.TryGetValue(info.Path, out info.Result))
                {
                    byte[] cachedFile=new byte[0];
                    foreach (var scan in scanMethods)
                    {
                        if (scan.Scan(info.Path, ref info.Result, ref cachedFile) == ScanStatus.Stop)
                        {
                            break;
                        }
                    }
                    cachedScanResult.TryAdd(info.Path, info.Result);
                }
                else
                {
                    Interlocked.Increment(ref cacheWins);
                }
                callback(info);

            }
            catch (Exception e)
            {
                Logger.Log("ScanModule exception: " + e.Message);
            }
        }
    }
}
