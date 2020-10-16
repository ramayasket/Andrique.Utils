using Kw.Common;
using Kw.Common.Threading;
using Kw.WinAPI;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Andrique.Utils.X7Actions
{
    /// ReSharper disable EmptyGeneralCatchClause
    static class Button7
    {
        static void Guard()
        {
            Interruptable.Wait(3000, 100);

            //
            //    if not processed gracefully, terminate
            //
            if (!AppCore.Exiting)
            {
                KernelUtils.TerminateCurrentProcess();
            }
        }

        [STAThread]
        static void Main()
        {
            //
            //    Make sure we're terminated
            //
            ExecutionThread.StartNew(Guard);

            ButtonProc();

            AppCore.Exiting = true;
        }

        private static void ButtonProc()
        {
            try
            {
                //
                //    Find window under the mouse
                //
                var pos = User.GetCursorPos();
                var hwnd = User.WindowFromPoint(pos);

                var hwndForeground = User.GetForegroundWindow();

                if (hwndForeground != hwnd)
                {
                    User.SetForegroundWindow(hwnd);

                    ////
                    //// Каким-то образом переключение окна от Winamp стало срабатывать и без AllocConsole...
                    //// что дало возможность его выпилить (тем более что процесс распределения консоли стал заметен на экране...)
                    ////

                    //hwndForeground = User.GetForegroundWindow();

                    //if (hwndForeground != hwnd)
                    //{
                    //    //
                    //    //    Hack to steal focus from ANY window (e.g. Winamp etc.)
                    //    //
                    //    Kernel.AllocConsole();
                    //    var hWndConsole = Kernel.GetConsoleWindow();
                    //    User.SetWindowPos(hWndConsole, IntPtr.Zero, 0, 0, 0, 0, SetWindowPosFlags.IgnoreZOrder);
                    //    Kernel.FreeConsole();

                    //    //
                    //    //    Now go to the window we want
                    //    //
                    //    User.SetForegroundWindow(hwnd);
                    //}
                }

                //Console.Beep(700, 200);

                if (Control.ModifierKeys == Keys.Shift)
                {
                    SendKeys.SendWait("%{F4}");
                }

                if (Control.ModifierKeys == Keys.Control)
                {
                    SendKeys.SendWait("^(cc)");
                }
                else if (Control.ModifierKeys == Keys.None)
                {
                    SendKeys.SendWait("^{F4}");
                }
            }
            catch { }
        }
    }
}
