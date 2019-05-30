using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Processes.WinT;

namespace Processes.Scanning
{
    class WinTrustScan : IModuleScan
    {
        public ScanStatus Scan(string fileName, ref string result, byte[] cachedFile)
        {
            if(WinTrust.VerifyEmbeddedSignature(fileName) == WinVerifyTrustResult.Success)
            {
                result = "Success";
                return ScanStatus.Stop;
            }
            else
            {
                result = "Unsafe";
                return ScanStatus.Continiue;
            }
        }
    }
}
