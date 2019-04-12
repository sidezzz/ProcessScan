using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Processes
{
    class Logger
    {
        static readonly Lazy<Logger> lazy = new Lazy<Logger>(() => new Logger());

        FileStream logStream;
        object sync;

        private Logger()
        {
            logStream = new FileStream("log.txt", FileMode.Append, FileAccess.Write, FileShare.Read);
            sync = new object();
        }

        public static Logger Instance => lazy.Value;

        public static void Log(string str)
        {
            try
            {
                string text = "[" + DateTime.Now + "] " + str + "\n";
                byte[] encodedText = Encoding.Unicode.GetBytes(text);
                lock (Instance.sync)
                {
                    Instance.logStream.Write(encodedText, 0, encodedText.Length);
                }
                //Instance.logStream.Flush();
            }
            catch(Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
        }
    }
}
