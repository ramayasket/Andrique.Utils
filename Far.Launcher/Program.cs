using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Linq;

namespace Andrique.Utils.Far.Launcher
{
    [SecurityCritical]
    class Program
    {
        const string INSTALL_DIR = "InstallDir";
        const string INSTALL_DIR_X64 = "InstallDir_x64";
        const string SOFTWARE_FAR = @"SOFTWARE\Far Manager";
        const string FAR_EXE = @"Far.exe";
        
        [SecurityCritical]
        [STAThread]
        static void Main(string[] args)
        {
            var asmName = Assembly.GetExecutingAssembly().GetName().Name;

            try
            {
                if (1 != args.Count())
                {
                    MessageBox.Show(
                        string.Format("Неправильное количество параметров. Ожидается: 1 Принято: {0}\nИспользование: {1}.exe <путь>", args.Count(), asmName),
                        asmName,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);

                    return;
                }

                var target = args[0];

                var is64 = (8 == IntPtr.Size);
                var installDirParam = is64 ? INSTALL_DIR_X64 : INSTALL_DIR;

                var keySoftwareFar = Registry.LocalMachine.OpenSubKey(SOFTWARE_FAR);

                //
                //    If null, consider Far Manager not installed
                //
                if (null == keySoftwareFar)
                {
                    MessageBox.Show(
                        @"Программа не может найти установочный путь Far Manager",
                        asmName,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);

                    return;
                }

                var installDir = (string)keySoftwareFar.GetValue(installDirParam);

                var farPath = Path.Combine(installDir, FAR_EXE);

                var info = new ProcessStartInfo(farPath)
                    {
                        Arguments = target.Contains(" ") ? string.Format("\"{0}\"", target) : target,
                        WorkingDirectory = installDir,
                        UseShellExecute = true,
                    };

                Process.Start(info);
            }
            catch (Exception x)
            {
                MessageBox.Show(
                    string.Format("Ошибка обнаружения и/или запуска программы Far Manager\n\n{0}", x.Message),
                    asmName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
