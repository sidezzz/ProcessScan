using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processes.Scanning
{
    public enum ScanStatus
    {
        Continiue,
        Stop
    }

    class FileCache
    {
        public readonly string Path;

        Lazy<byte[]> LazyFile; //optimizing file reading with delayed initialization
        public byte[] Content => LazyFile.Value;

        public FileCache(string path)
        {
            Path = path;
            if (!File.Exists(path))
                throw new ArgumentException("File path doesn't exist!");
            LazyFile = new Lazy<byte[]>(() =>
            {
                Console.WriteLine("Reading file");
                var ret = File.ReadAllBytes(path);
                return ret;
            });
        }
    }

    interface IModuleScan
    {
        ScanStatus Scan(FileCache file, ref string result);
    }
}
