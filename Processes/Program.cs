using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Processes.Scan;

namespace Processes
{
    static class Program
    {
        static private readonly Lazy<Scanner> lazyScanner = new Lazy<Scanner>(() => new Scanner());
        public static Scanner Scanner => lazyScanner.Value;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

    }
}
