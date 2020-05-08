using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using Kw.Common;
using Kw.Common.Threading;

namespace roboexec
{
    class Program
    {
        static void Main(string[] args)
        {
            var program = new Program();

            Application.Run();
        }

        private NotifyIcon _icon;
        private ContextMenu _menu;
        private MenuItem _item;
        private IContainer _components;

        private ExecutionThread _task;

        Program()
        {
            var target = AppConfig.RequiredSetting("TargetDirectory");

            _task = ExecutionThread.StartNew(Monitor);

            CreateNotifyicon();
        }

        void Monitor()
        {
            while (!AppCore.Exiting)
            {
                var signal = Interruptable.Wait(1000, 50, AppSignal);

                if (Interruptable.Signal.Application == signal)
                {
                    
                }
            }
        }

        private bool AppSignal()
        {
            return false;
        }

        private void CreateNotifyicon()
        {
            _components = new Container();
            _menu = new ContextMenu();
            _item = new MenuItem
            {
                Index = 0,
                Text = @"E&xit"
            };

            _item.Click += OnExitClick;

            _menu.MenuItems.AddRange(new [] { _item });

            _icon = new NotifyIcon(_components)
            {
                Icon = Resources.roboexec,
                //new Icon("roboexec.ico"),
                ContextMenu = _menu,
                Text = @"Robotic Executor",
                Visible = true
            };
        }

        private void OnExitClick(object sender, EventArgs eventArgs)
        {
            AppCore.Stop();
            Application.Exit();
        }
    }
}
