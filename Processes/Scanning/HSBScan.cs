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

        class HSBStorage
        {
            private readonly SortedDictionary<ComparableHash, string> HSBContainer = new SortedDictionary<ComparableHash, string>();
            private bool IsInit;
            private Task InitTask;

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
                        catch (ArgumentException)
                        {

                        }
                    }
                }
                else
                {
                    Logger.Log($"ParseHSBLine invalid line {line}");
                }
            }
            public HSBStorage()
            {
                InitTask = new Task(() =>
                {
                    try
                    {
                        Utils.ParseFileByLine("..\\database\\daily.hsb", ParseHSBLine);
                        Utils.ParseFileByLine("..\\database\\daily.hdb", ParseHSBLine);
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

            public SortedDictionary<ComparableHash, string> Container
            {
                get
                {
                    if (!IsInit)
                    {
                        InitTask.Wait();
                    }
                    return HSBContainer;
                }
            }
        }
        private static Lazy<HSBStorage> LazySignatureStorage = new Lazy<HSBStorage>(() => new HSBStorage());
        private static HSBStorage SignatureStorage => LazySignatureStorage.Value;


        public ScanStatus Scan(FileCache file, ref string result)
        {
            try
            {
                var hash = Utils.GetFileHash(file.Content);

                if (hash.Length == 0)
                {
                    throw new Exception("Invalid file hash");
                }

                string outResult="";
                if (SignatureStorage.Container.TryGetValue(new ComparableHash(hash), out outResult))
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
    }
}
