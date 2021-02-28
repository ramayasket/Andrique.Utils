using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var c = new Container();

            var i = new NotifyIcon(c);
            i.Icon = GetIcon("m1.ico");
            i.Visible = true;

            i.ContextMenuStrip = new ContextMenuStrip { Items = { new ToolStripMenuItem("zlp")}};

            Application.Run();
        }

        private static Icon GetIcon(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var icon = assembly.GetName().Name + "." + name;
            using (var stream = assembly.GetManifestResourceStream(icon))
                return new Icon(stream);
        }
    }
}
