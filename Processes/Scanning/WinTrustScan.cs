using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Processes.Scanning.WinT;

namespace Processes.Scanning
{
    class WinTrustScan : IModuleScan
    {
        public ScanStatus Scan(FileCache file, ref string result)
        {
            if(WinTrust.VerifyEmbeddedSignature(file.Path) == WinVerifyTrustResult.Success)
            {
                result = "Success";
                return ScanStatus.Stop;
            }
            else
            {
                result = "No digital cert";
                return ScanStatus.Continiue;
            }
        }
    }
}
