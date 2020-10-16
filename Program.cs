using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kw.Common;
using Kw.Common.Threading;
using Kw.WinAPI;

namespace disturb
{
    class Program
    {
        private static bool _beep = false;
        private static bool _attention = true;

        private static void AttentionSignal()
        {
            while (_attention)
            {
                if(_beep)
                    Console.Beep(700, 1000);

                Interruptable.Wait(2000, 100, () => !_attention);
            }
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, uint Msg);

        private const uint SW_RESTORE = 0x09;

        private static void Main()
        {
            int interval;

            var guid = Guid.NewGuid().ToString().Replace("-", "");
            Console.Title = guid;

            IntPtr handle = FindWindowByCaption(IntPtr.Zero, Console.Title);

            Console.Title = "Remote Display Disturber";

            try
            {
                interval = AppConfig.RequiredSetting<int>("Interval");
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                return;
            }

            new Thread(AttentionSignal).Start();

            while (true)
            {
                ShowWindow(handle, SW_RESTORE);
                User.SetForegroundWindow(handle);

                Console.WriteLine($"{DateTime.Now} I need attention!");

                _beep = true;

                Console.Write("Do you want to quit the disturber? ");
                var answer = (Console.ReadLine() ?? "Y").ToUpper();

                Console.WriteLine();

                _beep = false;

                if ("Y" == answer)
                    break;

                Thread.Sleep(interval);
            }

            _attention = false;
        }
    }
}
