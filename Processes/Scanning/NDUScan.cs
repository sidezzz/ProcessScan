using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Processes.Scanning
{
    class NDUScan : IModuleScan
    {
        class ExtendedSignature
        {
            public enum OffsetType
            {
                Any,
                EndOfFile,
                EntryPoint,
                Section
            }

            public string Name;
            public Regex Signature;
            public OffsetType Type;
            public int Offset;
            public int Shift;
            public int Section;
            public ExtendedSignature(string name, string sig, OffsetType type, int offset, int shift)
            {
                Name = name;
                Signature = new Regex(sig, RegexOptions.Compiled);
                Type = type;
                Offset = offset;
                Shift = shift;
            }
        }

        private readonly List<ExtendedSignature> NDUContainer;
        private bool IsInit;
        private Task InitTask;

        public NDUScan()
        {
            NDUContainer = new List<ExtendedSignature>();
            InitNDU();
        }

        public ScanStatus Scan(string fileName, ref string result, byte[] cachedFile)
        {
            if (!IsInit)
            {
                InitTask.Wait();
            }
            try
            {
                var file = Utils.ByteArrayToString(cachedFile);

                foreach (var sig in NDUContainer)
                {
                    try
                    {
                        if (sig.Offset + sig.Shift > file.Length)
                            continue;
                        var offset = sig.Offset;
                        if (sig.Type == ExtendedSignature.OffsetType.EntryPoint)
                        {
                            var entry = GetEntryRawOffset(cachedFile);
                            //Logger.Log(fileName+" Entry: " + entry.ToString());
                            offset += entry;
                        }
                        else if (sig.Type == ExtendedSignature.OffsetType.EndOfFile)
                        {
                            offset = file.Length - sig.Offset;
                        }

                        if (sig.Shift > 0)
                        {
                            var scanRange = file.Substring(offset, sig.Shift);
                            if (sig.Signature.IsMatch(scanRange))
                            {
                                result = sig.Name;
                                return ScanStatus.Stop;
                            }
                        }
                        else
                        {
                            if (sig.Signature.IsMatch(file, offset))
                            {
                                result = sig.Name;
                                return ScanStatus.Stop;
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        Logger.Log($"NDUScan exception: {sig.Name}, {e.Message}");
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
            ret = ret.Replace("*", ".*");

            ret = ret.Replace("{", ".{");
            ret = ret.Replace("{-", "{0,");

            ret = ret.Replace("-", ",");
            ret = ret.Replace("[", ".{");
            ret = ret.Replace("]", "}");

            return ret;
        }

        [DllImport("PEParser.dll")]
        private static extern int GetEntryRawOffset(byte[] image);

        private void ParseNDULine(string line)
        {
            var signatureFields = line.Split(':');
            if (signatureFields.Length > 3)
            {
                var type = signatureFields[1];
                if (type == "1")// || type == "0")
                {
                    ExtendedSignature.OffsetType offsetType = ExtendedSignature.OffsetType.Any;
                    int offset = 0;
                    int shift = 0;
                    var offsetExpression = signatureFields[2];
                    if (offsetExpression != "*")
                    {
                        var offsetExpressionParts = offsetExpression.Split(',');
                        if(offsetExpressionParts.Length > 1)
                        {
                            shift = Int32.Parse(offsetExpressionParts[1]);
                        }

                        var offsetLine = offsetExpressionParts[0].Split(new char[] { '+', '-' });
                        if (offsetLine.Length == 1)
                        {
                            offset = Int32.Parse(offsetLine[0]);
                        }
                        else if (offsetLine.Length == 2)
                        {
                            offset = Int32.Parse(offsetLine[1]);
                            if (offsetLine[0] == "EP")
                            {
                                offsetType = ExtendedSignature.OffsetType.EntryPoint;
                            }
                            else if (offsetLine[0] == "EOF")
                            {
                                offsetType = ExtendedSignature.OffsetType.EndOfFile;
                                Logger.Log("NDULine EOF " + offset.ToString());
                            }
                            else
                            {
                                Logger.Log("NDULine UNKNOWN WORD " + offsetLine[0]);
                            }
                        }
                    }

                    NDUContainer.Add(new ExtendedSignature(signatureFields[0],
                        ParseSignature(signatureFields[3]), offsetType, offset, shift));
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

        private void InitNDU()
        {
            InitTask = new Task(() =>
            {
                try
                {
                    Utils.ParseFileByLine("database\\daily.ndb", ParseNDULine);
                    Utils.ParseFileByLine("database\\daily.ndu", ParseNDULine);
                    Logger.Log($"InitNDU loaded {NDUContainer.Count} signatures");
                }
                catch (Exception e)
                {
                    Logger.Log($"InitNDU exception: {e.Message}");
                }
                IsInit = true;
            });
            InitTask.Start();
        }
    }
}
