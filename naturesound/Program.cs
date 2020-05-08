using System;
using System.Windows.Forms;
using Andrique.Utils.Rain;

namespace Andrique.Utils.NatureSound
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!InstanceAgent.First)
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new MainWindow());
        }
    }
}
