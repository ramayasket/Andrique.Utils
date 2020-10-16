using Kw.Common;
using Kw.Common.Threading;
using Kw.WinAPI;
using Kw.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;

//// ReSharper disable AssignNullToNotNullAttribute

namespace x7daemon
{
    public class App
    {
        #region Routes
        private static KeyRoute[] Routes = new[]
        {
            new KeyRoute
            {
                Filter = (args, move) => args.KeyCode.In(Keys.LControlKey, Keys.RControlKey),
                Descendants = new[]
                {
                    new KeyRoute
                    {
                        //Keys = Keys.F4,
                    }
                },
            }
        };
        class KeyRoute
        {
            public Func<KeyEventArgs, KeyMove, bool> Filter { get; set; }
            public KeyRoute[] Descendants { get; set; }

            public virtual bool Process(KeyEventArgs keyEventArgs, KeyMove move)
            {
                return false;
                //foreach (var route in Descendants ?? new KeyRoute[0])
                //{

                //}
            }

        }
        #endregion

        private static void ReadLineAgent()
        {
            Console.ReadLine();
            Application.Exit();
        }

        private static void DaemonProcDown(object sender, KeyEventArgs args) => DaemonProc(sender, args, KeyMove.Down);
        private static void DaemonProcUp(object sender, KeyEventArgs args) => DaemonProc(sender, args, KeyMove.Up);

        private static bool _ctrlPressed;
        private static bool _f4Pressed;

        public enum KeyMove
        {
            Up,
            Down
        }

        private static void DaemonProc(object sender, KeyEventArgs args, KeyMove move)
        {
            if (KeyMove.Down == move)
            {
                switch (args.KeyCode)
                {
                    case Keys.LControlKey:
                    case Keys.RControlKey:
                        _ctrlPressed = true;
                        break;

                    case Keys.F4:
                        if (_ctrlPressed && !_f4Pressed)
                        {
                            _f4Pressed = true;
                            SetForeground();
                        }
                        break;
                }
            }

            if (KeyMove.Up == move)
            {
                switch (args.KeyCode)
                {
                    case Keys.LControlKey:
                    case Keys.RControlKey:
                        _ctrlPressed = false;
                        break;

                    case Keys.F4:
                        _f4Pressed = false;
                        break;
                }
            }
        }

        private static void SetForeground()
        {
            var point = User.GetCursorPos();

            var under = User.WindowFromPoint(point);

            ReportWindowTitle(under);

            if (User.GetForegroundWindow() != under)
                User.SetForegroundWindow(under);

            if (User.GetForegroundWindow() == under)
                Debug.WriteLine($"Switched to {GetWindowTitle(under)}");

            else
                Debug.WriteLine($"Couldn't switch to {GetWindowTitle(under)}");
        }

        private static void ReportWindowTitle(IntPtr hwnd)
        {
            Debug.WriteLine($"Window title is '{GetWindowTitle(hwnd)}'");
        }

        private static string GetWindowTitle(IntPtr hwnd)
        {
            var buffer = new StringBuilder(1024);

            GetWindowText(hwnd, buffer, 1024);

            return buffer.ToString();
        }

        /// <summary>
        /// Synthesizes keystrokes, mouse motions, and button clicks.
        /// </summary>
        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs,
            [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs,
            int cbSize);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        private static void SendMouseClick()
        {
            var point = User.GetCursorPos();

            var input = new INPUT
            {
                U = new InputUnion
                {
                    mi = new MOUSEINPUT
                    {
                        dwFlags = MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.LEFTDOWN,
                        dwExtraInfo = UIntPtr.Zero,
                        dx = point.X,
                        dy = point.Y,
                        mouseData = 0,
                    },
                },
                type = 0,
            };

            SendInput(1, new[] { input }, INPUT.Size);

            input.U.mi.dwFlags = MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.LEFTUP;

            SendInput(1, new[] { input }, INPUT.Size);
        }

        private static NotifyIcon _icon;
        private static ContextMenu _menu;
        private static MenuItem _exit;
        private static IContainer _components;

        private static void DestroyIcon()
        {
            _icon.Visible = false;
            _icon = null;
        }

        private static void CreateIcon()
        {
            _components = new Container();
            _menu = new ContextMenu();
            _exit = new MenuItem { Index = 0, Text = @"E&xit" };

            _exit.Click += ExitClick;

            _menu.MenuItems.AddRange(new[] { _exit });

            _icon = new NotifyIcon(_components)
            {
                Icon = GetIcon("m1.ico"),
                ContextMenu = _menu,
                Text = "X7 Daemon",
                Visible = true
            };
        }

        private static void ExitClick(object Sender, EventArgs e)
        {
            Application.Exit();
        }

        [STAThread]
        public static void Main()
        {
            HookManager.KeyDown += DaemonProcDown;
            HookManager.KeyUp += DaemonProcUp;

            CreateIcon();
            
            try
            {
                Application.Run();
            }
            finally
            {
                DestroyIcon();

                HookManager.KeyDown -= DaemonProcDown;
                HookManager.KeyUp -= DaemonProcUp;
            }
        }

        /// <summary>
        /// Читает строку из ресурсов сборки.
        /// </summary>
        /// <param name="name">Имя ресурса.</param>
        /// <returns>Строковые данные.</returns>
        private static Icon GetIcon(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var @base = assembly.GetName().Name + ".";
            using (var stream = assembly.GetManifestResourceStream(@base + name))
                return new Icon(stream);
        }
    }
}
