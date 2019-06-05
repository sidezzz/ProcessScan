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
        public class ComparableHash : IComparable
        {
            public byte[] ByteHash;
            public int FileSize;
            public ComparableHash(string hash, int fileSize)
            {
                ByteHash = Utils.StringToByteArray(hash);
                FileSize = fileSize;
            }
            public ComparableHash(byte[] hash, int fileSize)
            {
                ByteHash = hash;
                FileSize = fileSize;
            }
            public int CompareTo(object obj)
            {
                ComparableHash otherHash = obj as ComparableHash;
                if (otherHash != null)
                {
                    if (FileSize == 0 || otherHash.FileSize == 0 || FileSize == otherHash.FileSize)
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
                        return FileSize - otherHash.FileSize;
                    }
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

                string outResult = "";
                if (HSBContainer.TryGetValue(new ComparableHash(hash, cachedFile.Length), out outResult))
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
                if (words[0].Length == 32)
                {

                    int fileSize = 0;
                    if(words[1] != "*")
                    {
                        fileSize = Int32.Parse(words[1]);
                    }

                    try
                    {
                        HSBContainer.Add(new ComparableHash(words[0], fileSize), words[2]);
                    }
                    catch(ArgumentException)
                    {
                        //Logger.Log($"ParseHSBLine exception {e.Message} {words[0]}");
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
