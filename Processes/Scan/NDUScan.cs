using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Processes.Scan
{
    class NDUScan : IModuleScan
    {
        class ExtendedSignature
        {
            public enum OffsetType
            {
                Any,
                EOF,
                EP
            }

            public string Name;
            public Regex Signature;
            public OffsetType Type;
            public int Offset;
            public int Section;
            public ExtendedSignature(string name, string sig, OffsetType type, int offset)
            {
                Name = name;
                Signature = new Regex(sig);
                Type = type;
                Offset = offset;
            }
        }

        private readonly List<ExtendedSignature> NDUContainer;
        private bool IsInit;

        public NDUScan()
        {
            NDUContainer = new List<ExtendedSignature>();
            InitNDU();
        }

        public ScanStatus Scan(string fileName, ref string result, ref byte[] cachedFile)
        {
            while (!IsInit)
            { Task.Delay(25).Wait(); }

            try
            {
                if (cachedFile.Length == 0)
                {
                    cachedFile = File.ReadAllBytes(fileName);
                }

                var sb = new StringBuilder();
                foreach (var b in cachedFile)
                    sb.Append(b.ToString("x2"));
                var file = sb.ToString();


                foreach (var sig in NDUContainer)
                {
                    var offset = 0;
                    if(sig.Type==ExtendedSignature.OffsetType.EP)
                    {
                        var entry=GetEntryRawOffset(cachedFile);
                        //Logger.Log(fileName+" Entry: " + entry.ToString());
                        offset = entry;
                    }

                    if(sig.Signature.IsMatch(file,offset))//Regex.IsMatch(file, sig.Signature))
                    {
                        result = sig.Name;
                        return ScanStatus.Stop;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log("NDUScan exception: " + e.Message);
            }

            return ScanStatus.Continiue;
        }


        private string ParseSignature(string sig)
        {
            var ret = sig.Replace('?','.');
            ret=ret.Replace("*", ".*");

            ret = ret.Replace("{", ".{");
            ret = ret.Replace("{-", "{0,");

            ret = ret.Replace("-", ",");
            ret = ret.Replace("[", ".{");
            ret = ret.Replace("]", "}");
            //ret.Replace()

            //Logger.Log("ParseSignature " + sig+" -> "+ ret);

            return ret;
        }

        [DllImport("PEParser.dll")]
        private static extern int GetEntryRawOffset(byte[] image);

        private void ParseNDULine(string line)
        {
            var words = line.Split(':');
            if (words.Length > 3)
            {
                if (words[1] == "1")
                {
                    ExtendedSignature.OffsetType type = ExtendedSignature.OffsetType.Any;
                    int offset = 0;
                    if (words[2] != "*")
                    {
                        var offsetLine = words[2].Split(new char[] { '+', '-' });
                        if(offsetLine.Length==1)
                        {
                            //offsetLine
                            try
                            {
                                offset = Int32.Parse(offsetLine[0]);
                            }
                            catch(Exception e)
                            {
                                Logger.Log("Int32.Parse exception: "  + words[2]);
                            }
                        }
                        else if(offsetLine.Length==2)
                        {
                            try
                            {
                                offset = Int32.Parse(offsetLine[1]);
                            }
                            catch (Exception e)
                            {
                                Logger.Log("Int32.Parse exception: " + words[2]);
                            }
                            if (offsetLine[0]=="EP")
                            {
                                type = ExtendedSignature.OffsetType.EP;
                                Logger.Log("NDULine EP " + offset.ToString());
                            }
                            else if(offsetLine[0]=="EOF")
                            {
                                type = ExtendedSignature.OffsetType.EOF;
                                Logger.Log("NDULine EOF " + offset.ToString());
                            }
                            else
                            {
                                Logger.Log("NDULine UNKNOWN WORD " + offsetLine[0]);
                            }
                        }
                    }

                    NDUContainer.Add(new ExtendedSignature(words[0], ParseSignature(words[3]), type, offset));
                }
                else
                {
                    //Logger.Log("NDULine not PE signature "+ line);
                }
            }
            else
            {
                Logger.Log("ParseNDULine invalid line " + line);
            }
        }

        private async void InitNDU()
        {
            await Task.Run(() =>
            {
                try
                {
                    Utils.ParseFileByLine("database\\daily.ndb", ParseNDULine);
                    Utils.ParseFileByLine("database\\daily.ndu", ParseNDULine);
                    Logger.Log("InitNDU loaded " + NDUContainer.Count.ToString() + " signatures");
                }
                catch (Exception e)
                {
                    Logger.Log("InitNDU exception: " + e.Message);
                }
                IsInit = true;
            });
        }
    }
}
