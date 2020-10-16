using Kw.WinAPI;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Andrique.Shell
{
    class Program
    {
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, ushort wParam, uint lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool SendMessage(IntPtr hWnd, uint Msg, ushort wParam, uint lParam);

        /// <summary>
        /// Synthesizes keystrokes, mouse motions, and button clicks.
        /// </summary>
        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs,
            [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs,
            int cbSize);

        class ForegroundWindowSwitch : IDisposable
        {
            public IntPtr CurrentWindow { get; }
            public IntPtr TargetWindow { get; }

            public bool Switched { get; }

            public ForegroundWindowSwitch(IntPtr target)
            {
                CurrentWindow = User.GetForegroundWindow();
                TargetWindow = target;

                Switched = SwitchForegroundWindow(TargetWindow, "target");
            }


            /// <inheritdoc />
            public void Dispose() => SwitchForegroundWindow(CurrentWindow, "back");
        }

        internal static bool SwitchForegroundWindow(IntPtr hwnd, string what)
        {
            User.SetForegroundWindow(hwnd);

            if (User.GetForegroundWindow() != hwnd)
                ForceForegroundWindow(hwnd);

            if (User.GetForegroundWindow() != hwnd)
            {
                Debug.WriteLine($"Couldn't switch to {what} {hwnd.ToInt32():x8}");
                return false;
            }

            Debug.WriteLine($"Switched to {what} {hwnd.ToInt32():x8}");
            return true;
        }

        internal static void ForceForegroundWindow(IntPtr hwnd)
        {
            //
            //    Hack to steal focus from ANY window
            //
            Kernel.AllocConsole();
            var hWndConsole = Kernel.GetConsoleWindow();
            User.SetWindowPos(hWndConsole, IntPtr.Zero, 0, 0, 0, 0, SetWindowPosFlags.IgnoreZOrder);
            Kernel.FreeConsole();

            //
            //    Now go to the window we want
            //
            User.SetForegroundWindow(hwnd);
        }

        static IntPtr FindWindow(string title)
        {
            var hwnd = IntPtr.Zero;

            foreach (Process pList in Process.GetProcesses())
            {
                if (pList.MainWindowTitle.Contains(title))
                {
                    hwnd = pList.MainWindowHandle;
                    Debug.WriteLine($"Found window '{title}' = {hwnd}");
                    break;
                }
            }

            return hwnd;
        }

        private NotifyIcon _icon;
        private ContextMenu _menu;
        private MenuItem _exit;
        private IContainer components;

        private void DestroyIcon()
        {
            _icon.Visible = false;
            _icon = null;
        }
        
        private void CreateNotifyicon()
        {
            components = new Container();
            _menu = new ContextMenu();
            _exit = new MenuItem { Index = 0, Text = "E&xit" };

            _exit.Click += ExitClick;

            _menu.MenuItems.AddRange(new [] { _exit });

            _icon = new NotifyIcon(components)
            {
                Icon = new Icon("citrix.ico"),
                ContextMenu = _menu,
                Text = "Desktop Viewer Distruber",
                Visible = true
            };
        }

        private void ExitClick(object Sender, EventArgs e)
        {
            Application.Exit();
        }

        static void Main(string[] args)
        {

            Program pg = new Program();
            Application.Run();

            pg.DestroyIcon();
        }

        Program()
        {
            CreateNotifyicon();
        }

        static void Main1(string[] args)
        {
            var hdesktop = FindWindow("MSK-BACKEND300 - Desktop Viewer");
            var hed = FindWindow("Untitled - Notepad");

            bool ok;

            //ok = SwitchForegroundWindow(hdesktop, "desktop");
            //ok = SwitchForegroundWindow(hed, "ed");

            Debug.WriteLine($"Window handle is {hdesktop.ToInt32():x8}");

            if (IntPtr.Zero == hdesktop)
            {
                Debug.WriteLine("Couldn't find desktop viewer window");
                return;
            }
            
            while (true)
            {
                using (new ForegroundWindowSwitch(hdesktop))
                {
                    Thread.Sleep(300);
                    SendCtrlDownUp();
                    Thread.Sleep(300);
                }

                Thread.Sleep(5000);
            }
        }

        internal static void SendKeyboardMessages(IntPtr hwnd)
        {

        }

        internal static void SendCtrlDownUp()
        {
            var input = new INPUT
            {
                U = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        dwExtraInfo = UIntPtr.Zero,
                        dwFlags = default(KEYEVENTF),
                        wScan = ScanCodeShort.LCONTROL,
                        wVk = VK.LCONTROL,
                        time = 0,
                    }
                },
                type = 1,
            };

            SendInput(1, new[] { input }, INPUT.Size);

            input.U.ki.dwFlags = KEYEVENTF.KEYUP;

            SendInput(1, new[] { input }, INPUT.Size);
        }
    }
}
