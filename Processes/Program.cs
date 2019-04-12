using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Processes
{
    static class Program
    {
        static public Scan.Scanner scanner;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            scanner = new Scan.Scanner();
            Application.Run(new MainForm());
        }

    }
}
