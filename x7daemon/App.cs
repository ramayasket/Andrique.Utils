using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using Kw.Common;
using Kw.Common.Threading;
using Kw.WinAPI;
using Kw.Windows.Forms;

namespace x7daemon
{
	public class App
	{
		[STAThread]
		public static void Main()
		{
			var i = User.GetDoubleClickTime();

			Console.WriteLine(Console.Title);
			
			HookManager.KeyDown += DisplayKeyDown;
			HookManager.KeyUp += DisplayKeyUp;

			//HookManager.KeyDown += Daemon.OnKeyDown;
			//HookManager.KeyUp += Daemon.OnKeyUp;

			Console.WriteLine(@"Press ENTER to quit...");
			ExecutionThread.StartNew(ReadLineAgent);

			Application.Run();

			AppCore.Stop();

			HookManager.KeyDown -= DisplayKeyDown;
			HookManager.KeyUp -= DisplayKeyUp;

			//HookManager.KeyDown -= Daemon.OnKeyDown;
			//HookManager.KeyUp -= Daemon.OnKeyUp;
		}

		private static void ReadLineAgent()
		{
			Console.ReadLine();
			Application.Exit();
		}

		private static int _counter = 0;

		private static void DisplayKeyUp(object sender, KeyEventArgs keyEventArgs)
		{
			Console.WriteLine($@"UP: {keyEventArgs.KeyCode} ({_counter++})");
			keyEventArgs.Handled = true;
		}

		private static void DisplayKeyDown(object sender, KeyEventArgs keyEventArgs)
		{
			Console.WriteLine($@"DOWN: {keyEventArgs.KeyCode} ({_counter++})");

			if (keyEventArgs.KeyCode == Keys.Return)
				Application.Exit();

			else
				keyEventArgs.Handled = true;
		}
	}
}
