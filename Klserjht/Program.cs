using System;
using System.Windows.Forms;

namespace Klserjht
{
    static class Program
    {
        public static readonly Version Version = new Version(System.Diagnostics.FileVersionInfo
            .GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion);

        /// <summary>
        /// The main entry point for the application.
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
