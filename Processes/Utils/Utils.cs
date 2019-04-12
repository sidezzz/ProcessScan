using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Processes
{
    sealed class Utils
    {
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static byte[] GetFileHash(byte[] file)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                return md5Hash.ComputeHash(file);
            }
        }

        public static void ParseFileByLine(string file, Action<string> parseLine)
        {
            using (StreamReader readStream = new StreamReader(file))
            {
                while (!readStream.EndOfStream)
                {
                    var line = readStream.ReadLine();
                    try
                    {
                        parseLine(line);
                    }
                    catch (Exception e)
                    {
                        Logger.Log("ParseFileByLine exception: " + e.Message+", "+ line);
                    }
                }
            }
        }
    }
}
