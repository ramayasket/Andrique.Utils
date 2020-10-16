using System;
using System.Runtime.InteropServices;

namespace Andrique.Utils.X7Actions
{
    static class WheelUp
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        private const int MOUSEEVENTF_WHEEL = 0x0800;

        [STAThread]
        static void Main()
        {
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, 120, 0);
        }
    }
}
