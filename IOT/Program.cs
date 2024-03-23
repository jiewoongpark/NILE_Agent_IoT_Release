using System;
using System.Windows.Forms;
using System.Threading;
using IOT.Properties;

namespace IOT
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (Mutex mutex = new Mutex(false, "Global\\" + AppGuid))
            {
                if (!mutex.WaitOne(0, false))
                {
                    MessageBox.Show(Resources.AppAlreadyRunning, Resources.NILEAgentIoT, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    FormApp();
                }
            }
        }

        private static readonly string AppGuid = "f8441a77-6129-46bb-8694-4f93ba297089";

        private static void FormApp()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmIOTMain()); // Ensure frmIOTMain is a Form class
        }
    }
}