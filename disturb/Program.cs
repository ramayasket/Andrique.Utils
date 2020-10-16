using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kw.Common;
using Kw.Common.Threading;
using Kw.WinAPI;
using Kw.Windows.Forms;

namespace disturb
{
    class Program
    {
        private static bool _beep = false;
        private static bool _attention = true;

        private static IntPtr _handle;
        private static int _interval;
        private static DateTime _start;

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

        private static DateTime _lastInput = DateTime.Now;

        private static void SubscribeInput()
        {
            HookManager.KeyDown += (_, __) => _lastInput = DateTime.Now;
            HookManager.KeyUp += (_, __) => _lastInput = DateTime.Now;
            HookManager.MouseMove += (_, __) => _lastInput = DateTime.Now;
            HookManager.MouseDown += (_, __) => _lastInput = DateTime.Now;
            HookManager.MouseUp += (_, __) => _lastInput = DateTime.Now;
            HookManager.MouseWheel += (_, __) => _lastInput = DateTime.Now;
        }

        private static void SleepWithoutInput(int interval)
        {
            _start = DateTime.Now;

            while (_start + TimeSpan.FromMilliseconds(interval) > DateTime.Now)
            {
                Thread.Sleep(100);
            }
        }

        private static void DisturberProc()
        {
            while (true) {
                ShowWindow(_handle, SW_RESTORE);
                User.SetForegroundWindow(_handle);

                Console.WriteLine($"{DateTime.Now} I need attention!");

                _beep = true;

                Console.Write("Do you want to quit the disturber? ");
                var answer = (Console.ReadLine() ?? "Y").ToUpper();

                Console.WriteLine();

                _beep = false;

                if ("Y" == answer)
                    break;

                SleepWithoutInput(_interval);
            }

            _attention = false;
            Application.Exit();
        }

        private static void Main()
        {
            var guid = Guid.NewGuid().ToString().Replace("-", "");
            Console.Title = guid;

            _handle = FindWindowByCaption(IntPtr.Zero, Console.Title);

            Console.Title = "Remote Display Disturber";

            try
            {
                _interval = AppConfig.RequiredSetting<int>("Interval");
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                return;
            }

            SubscribeInput();

            ExecutionThread.StartNew(AttentionSignal);
            ExecutionThread.StartNew(DisturberProc);

            Application.Run();
        }
    }
}
