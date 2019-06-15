using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processes.Scanning
{
    public enum ScanStatus
    {
        Continiue,
        Stop
    }

    interface IModuleScan
    {
        ScanStatus Scan(string filePath, ref string result, byte[] cachedFile);
    }
}
