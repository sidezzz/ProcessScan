using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics.Contracts;

namespace Processes.Scanner
{
    class Database
    {
        public class HashSigBased
        {
            public byte[] Hash;
            public int Size;
            public string Name;
            public HashSigBased(string hash,int size, string name)
            {
                Hash = StringToByteArray(hash);
                Size = size;
                Name = name;
            }
        }

        public readonly List<HashSigBased> HSBList;

        public Database()
        {
            HSBList = new List<HashSigBased>();
            InitHSB();
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private void LoadHSBFromFile(string file)
        {
            using (StreamReader HSBStream = new StreamReader(file))
            {
                while (!HSBStream.EndOfStream)
                {
                    var line = HSBStream.ReadLine();
                    var words = line.Split(':');

                    if (words.Length > 2)
                    {
                        if (words[0].Length == 32)
                        {
                            int file_size = 0;
                            if (words[1][0] != '*')
                            {
                                file_size = Int32.Parse(words[1]);
                            }
                            HSBList.Add(new HashSigBased(words[0], file_size, words[2]));
                        }
                    }
                    else
                    {
                        Logger.Log("LoadHSBFromFile invalid line " + line);
                    }
                }
            }
        }

        private async void InitHSB()
        {
            await Task.Run(() =>
            {
                try
                {
                    LoadHSBFromFile("database\\daily.hsb");
                    LoadHSBFromFile("database\\daily.hdb");
                    Logger.Log("InitHSB loaded " + HSBList.Count.ToString()+" signatures");
                }
                catch (Exception e)
                {
                    Logger.Log("InitHSB exception: " + e.Message);
                }
            });
        }



    }

}
