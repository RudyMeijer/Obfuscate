using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Obfuscate
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0 && Directory.Exists(args[0]))
            {
                Properties.Settings.Default.InitialPath = args[0];
                Form1 form1 = new Form1();
                form1.btnGo_Click(null, null);
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
    }
}
