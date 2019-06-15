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
        static readonly Lazy<Logger> LazyLogger = new Lazy<Logger>(() => new Logger());

        StreamWriter LogWritter;
        object WriteSync;

        private Logger()
        {
            LogWritter = new StreamWriter("..\\log.txt", true);
            LogWritter.AutoFlush = true;
            WriteSync = new object();
        }

        public static Logger Instance => LazyLogger.Value;

        public static void Log(string message)
        {
            try
            {
                lock (Instance.WriteSync)
                {
                    Instance.LogWritter.WriteLine($"[{DateTime.Now}] {message}");
                }
            }
            catch(Exception e)
            {
                //MessageBox.Show($"Log exception {e.ToString()}");
            }
        }
    }
}
