using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Processes.Scanning
{
    class HSBScan : IModuleScan
    {
        class ComparableHash : IComparable
        {
            byte[] ByteHash;
            public ComparableHash(string hash)
            {
                ByteHash = Utils.StringToByteArray(hash);
            }
            public ComparableHash(byte[] hash)
            {
                ByteHash = hash;
            }
            public int CompareTo(object obj)
            {
                ComparableHash otherHash = obj as ComparableHash;
                if (otherHash != null)
                {
                    for (int a = 0; a < ByteHash.Length; a++)
                    {
                        if (ByteHash[a] != otherHash.ByteHash[a])
                        {
                            return ByteHash[a] - otherHash.ByteHash[a];
                        }
                    }
                    return 0;
                }
                else
                {
                    throw new ArgumentException("Object is not a Hash");
                }
            }
        }

        private readonly SortedDictionary<ComparableHash, string> HSBContainer = new SortedDictionary<ComparableHash, string>();
        private bool IsInit;
        private Task InitTask;
        public HSBScan()
        {
            InitHSB();
        }

        public ScanStatus Scan(string fileName, ref string result, byte[] cachedFile)
        {
            if (!IsInit)
            {
                InitTask.Wait();
            }
            try
            {
                var hash = Utils.GetFileHash(cachedFile);

                if (hash.Length == 0)
                {
                    throw new Exception("Invalid file hash");
                }

                string outResult="";
                if (HSBContainer.TryGetValue(new ComparableHash(hash), out outResult))
                {
                    result = outResult;
                    return ScanStatus.Stop;
                }
                    
            }
            catch (Exception e)
            {
                Logger.Log($"HSBScan exception: {e.Message}");
            }
            return ScanStatus.Continiue;
        }

        private void ParseHSBLine(string line)
        {
            var words = line.Split(':');
            if (words.Length > 2)
            {
                var hash = words[0];
                if (hash.Length == 32)
                {
                    try
                    {
                        HSBContainer.Add(new ComparableHash(hash), words[2]);
                    }
                    catch(ArgumentException)
                    {

                    }
                }
            }
            else
            {
                Logger.Log($"ParseHSBLine invalid line {line}");
            }
        }

        private void InitHSB()
        {
            InitTask = new Task(() =>
             {
                 try
                 {
                     Utils.ParseFileByLine("database\\daily.hsb", ParseHSBLine);
                     Utils.ParseFileByLine("database\\daily.hdb", ParseHSBLine);
                     Logger.Log($"InitHSB loaded {HSBContainer.Count} signatures");
                 }
                 catch (Exception e)
                 {
                     Logger.Log($"InitHSB exception: {e.Message}");
                 }
                 IsInit = true;
             });
            InitTask.Start();
        }
    }
}
