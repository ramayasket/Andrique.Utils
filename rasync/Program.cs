using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace rasync
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] cmdArgs)
        {
            if (cmdArgs.Length < 1)
                return;

            var dir = new FileInfo(cmdArgs[0]);

            if (null != dir.DirectoryName)
            {
                var name = Path.Combine(dir.DirectoryName, dir.Name);
                var args = (cmdArgs.Length > 1) ? cmdArgs[1] : "";

                var stOptions = new ProcessStartInfo
                {
                    FileName = name,
                    Arguments = args
                };

                Process.Start(stOptions);
            }

        }
    }
}
