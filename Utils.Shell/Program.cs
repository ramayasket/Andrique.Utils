using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kw.WinAPI;
using Microsoft.Win32;

using Kw.Common;
using Kw.Common.Containers;

namespace Utils.Shell
{
    public static class KeyboardHelper
    {
        public static Keys VK2Keys(VK vk)
        {
            return (Keys) ((int) vk);
        }

        public static VK Keys2VK(Keys keys)
        {
            switch (keys)
            {
                case Keys.Alt: return VK.MENU;
                case Keys.Control: return VK.LCONTROL;
                case Keys.Shift: return VK.LSHIFT;
                case Keys.KeyCode:
                case Keys.LineFeed:
                case Keys.Modifiers:
                case Keys.None:
                    return (VK) 0;
                default:
                    return (VK) keys;
            }

            throw new NotImplementedException();
        }
    }

    class Program
    {
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        static void Main(string[] args)
        {
            var hwnd = IntPtr.Zero;

            foreach (Process pList in Process.GetProcesses()) {
                if (pList.MainWindowTitle.Contains("MSK-BACKEND30 - Desktop Viewer")) {
                    hwnd = pList.MainWindowHandle;
                    break;
                }
            }

            if (IntPtr.Zero != hwnd) {

                IntPtr wParam = new IntPtr(0x41);

                var s1 = PostMessage(hwnd, WM_KEYDOWN, wParam, IntPtr.Zero);
                var s2 = PostMessage(hwnd, WM_KEYUP, wParam, IntPtr.Zero);
            }
        }

        private static void SomethingWithVKs()
        { //var mtx = Mutex.OpenExisting("DBWinMutex1");

            var vks = EnumHelper.GetValues<VK>().Cast<int>().Distinct().Cast<VK>().OrderBy(k => k.ToString()).ToArray();
            var keys = EnumHelper.GetValues<Keys>().Cast<int>().Distinct().Cast<Keys>().OrderBy(k => k.ToString()).ToArray();

            var commons = vks.Cast<int>().Intersect(keys.Cast<int>()).ToArray();

            Console.WriteLine("Common values");

            var scommons = new StringBuilder();

            foreach (var k in commons) {
                Console.WriteLine($"{(VK) k} {(Keys) k}");
                scommons.AppendLine($"{(VK) k} {(Keys) k}");
            }

            var scs = scommons.ToString();

            Console.WriteLine("Specific VK values");

            var vks_e = vks.Cast<int>().Except(commons).Cast<VK>().ToArray();

            foreach (var k in vks_e) {
                Console.WriteLine($"{k}");
            }

            Console.WriteLine("Specific Keys values");

            var keys_e = keys.Cast<int>().Except(commons).Cast<Keys>().ToArray();

            foreach (var k in keys_e) {
                Console.WriteLine($"{k}");
            }
        }
    }
}
