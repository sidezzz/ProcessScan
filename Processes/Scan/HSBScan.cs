using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Processes.Scan
{
    class HSBScan : IModuleScan
    {
        public class ComparableHash : IComparable
        {
            public byte[] byteHash;
            public ComparableHash(string hash)
            {
                byteHash = Utils.StringToByteArray(hash);
            }
            public ComparableHash(byte[] hash)
            {
                byteHash = hash;
            }
            public int CompareTo(object obj)
            {
                ComparableHash otherHash = obj as ComparableHash;
                if (otherHash != null)
                {
                    for (int a = 0; a < byteHash.Length; a++)
                    {
                        if (byteHash[a] != otherHash.byteHash[a])
                        {
                            return byteHash[a] - otherHash.byteHash[a];
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

        private readonly SortedDictionary<ComparableHash, string> HSBContainer;
        private bool IsInit;

        public HSBScan()
        {
            HSBContainer = new SortedDictionary<ComparableHash, string>();
            InitHSB();
        }

        public ScanStatus Scan(string fileName, ref string result, ref byte[] cachedFile)
        {
            while(!IsInit)
            { Task.Delay(25).Wait(); }
            try
            {
                if (cachedFile.Length == 0)
                {
                    cachedFile = File.ReadAllBytes(fileName);
                }
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
                Logger.Log("HSBScan exception: " + e.Message);
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
                    try
                    {
                        HSBContainer.Add(new ComparableHash(words[0]), words[2]);
                    }
                    catch(ArgumentException)
                    {

                    }
                }
            }
            else
            {
                Logger.Log("ParseHSBLine invalid line " + line);
            }
        }

        private async void InitHSB()
        {
            await Task.Run(() =>
            {
                try
                {
                    Utils.ParseFileByLine("database\\daily.hsb", ParseHSBLine);
                    Utils.ParseFileByLine("database\\daily.hdb", ParseHSBLine);
                    Logger.Log("InitHSB loaded " + HSBContainer.Count.ToString() + " signatures");
                }
                catch (Exception e)
                {
                    Logger.Log("InitHSB exception: " + e.Message);
                }
                IsInit = true;
            });
        }
    }
}
