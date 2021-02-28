using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;

//// ReSharper disable AssignNullToNotNullAttribute

namespace clipcopy
{
    class NamedString
    {
        public string Name;
        public string Value;
    }
    
    public class App
    {
        private static NotifyIcon _icon;
        private static ContextMenuStrip _menu;
        private static IContainer _components;

        private static string _items_path;
        private static NamedString[] _strings;

        private static void DestroyIcon()
        {
            _icon.Visible = false;
            _icon = null;
        }

        private static void CreateIcon()
        {
            _components = new Container();
            _menu = MakeMenu();

            _icon = new NotifyIcon(_components)
            {
                Icon = GetIcon("Main.ico"),
                ContextMenuStrip = _menu,
                Text = "Clipcopy",
                Visible = true
            };
        }

        private static void ExitClick(object Sender, EventArgs e)
        {
            Application.Exit();
        }

        private static ContextMenuStrip MakeMenu()
        {
            var menu = new ContextMenuStrip(); { };
            var items = new List<ToolStripItem>();

            foreach (var s in _strings)
            {
                var item = new ToolStripMenuItem { Text = s.Name, };
                item.Click += ItemOnClick;
                items.Add(item);
            }

            ToolStripMenuItem x;
            
            if(_strings.Any())
                items.Add(new ToolStripSeparator());

            items.Add(x = new ToolStripMenuItem { Text = "Launch &Far Manager" });
            x.Click += Edit;

            items.Add(x = new ToolStripMenuItem { Text = "&Exit", });
            x.Click += ExitClick;

            menu.Items.AddRange(items.ToArray());

            return menu;
        }

        private static void Edit(object sender, EventArgs e)
        {
            var proc = new Process { StartInfo = new ProcessStartInfo("far.exe", _items_path) };

            proc.Start();
        }

        private static void ItemOnClick(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;

            var s = _strings.Single(x => x.Name == item.Text);
            
            Clipboard.SetText(s.Value);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        private static NamedString[] ReadStrings()
        {
            var asm = Assembly.GetEntryAssembly()?.Location;

            _items_path = Path.Combine(Path.GetDirectoryName(asm), "clipitems");

            if (!Directory.Exists(_items_path))
            {
                // attempt to create
                try {
                    Directory.CreateDirectory(_items_path);
                }
                catch (Exception x) {
                    MessageBox.Show(x.Message, "clipcopy", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    //bye-bye
                    TerminateProcess(new IntPtr(-1), 1);
                }
            }

            var items = Directory
                .EnumerateFiles(_items_path)
                .OrderBy(x => x)
                .Select(
                    i => new NamedString { Name = Path.GetFileName(i), Value = File.ReadAllText(Path.Combine(_items_path, i)) }
                ).ToArray();

            return items;
        }

        [STAThread]
        public static void Main()
        {
            _strings = ReadStrings();

            FileSystemWatcher watcher = new FileSystemWatcher(_items_path);

            CreateIcon();

            watcher.Changed += OnFileEvent;
            watcher.Created += OnFileEvent;
            watcher.Deleted += OnFileEvent;
            watcher.Renamed += OnFileEvent;

            watcher.EnableRaisingEvents = true;
            
            try
            {
                Application.Run();
            }
            finally
            {
                DestroyIcon();
            }
        }

        private static void OnFileEvent(object sender, FileSystemEventArgs e)
        {
            _strings = ReadStrings();
            _menu = MakeMenu();
            _icon.ContextMenuStrip = _menu;
        }

        /// <summary>
        /// Читает строку из ресурсов сборки.
        /// </summary>
        /// <param name="name">Имя ресурса.</param>
        /// <returns>Строковые данные.</returns>
        private static Icon GetIcon(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var icon = assembly.GetName().Name + "." + name;
            using (var stream = assembly.GetManifestResourceStream(icon))
                return new Icon(stream);
        }
    }
}
