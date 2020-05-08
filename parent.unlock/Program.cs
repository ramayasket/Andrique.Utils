using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Kw.Common;
using Kw.Common.Containers;
using Kw.Common.Threading;

namespace parent.unlock
{
	class HandleInfo : Pair<string, string>
	{
		public HandleInfo(string pid, string handle)
		{
			First = pid;
			Second = handle;
		}

		public string Pid { get { return First; } }
		public string Handle { get { return Second; } }
	}

	static class Program
	{
		private static volatile bool GuardSignal;
		private static string Postoffice;

		private const string PARENT_LOCK = "parent.lock";

		static void Guard()
		{
			while (AppCore.Runnable)
			{
				var signal = Interruptable.Wait(60000, 1000, () => GuardSignal);

				if (signal.In(Interruptable.Signal.Application, Interruptable.Signal.Elapsed))
				{
					CheckAndUnlock();
					GuardSignal = false;
				}
			}
		}

		private static void CheckAndUnlock()
		{
			var lines = RunHandle();
			var handles = OutputToHandles(lines);

			Unlock(handles);
		}

		static string[] RunHandle(string arguments = null)
		{
			var outputs = new List<string>();

			var pinfo = new ProcessStartInfo("handle.exe", arguments ?? PARENT_LOCK)
			{
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
			};

			var process = new Process()
			{
				StartInfo = pinfo,
			};

			bool header = true;
			bool info = false;

			process.OutputDataReceived += (sender, args) =>
			{
				var line = args.Data;

				if (null == line)
					return;

				if (info)
				{
					if("No matching handles found." != line)
						outputs.Add(line);
				}
				else
				{
					if (string.Empty == line)
					{
						if (header)
							header = false;
						else
							info = true;
					}
				}
			};

			process.Start();
			process.BeginOutputReadLine();
			process.WaitForExit();

			return outputs.ToArray();
		}

		static HandleInfo[] OutputToHandles(string[] lines)
		{
			var output = new List<HandleInfo>();

			//
			//	ReSharper disable once LoopCanBeConvertedToQuery
			//
			foreach (var line in lines)
			{
				var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);

				if (parts.Length >= 7)
				{
					var path = parts[6];
					
					var dir = Path.GetDirectoryName(path);
					var file = Path.GetFileName(path);

					if (dir == Postoffice && PARENT_LOCK == file)
					{
						var pid = parts[2];
						var handle = parts[5].Replace(":", string.Empty);

						output.Add(new HandleInfo(pid, handle));
					}
				}
			}

			return output.ToArray();
		}

		static void Unlock(HandleInfo[] handles)
		{
			var path = Path.Combine(Postoffice, PARENT_LOCK);

			foreach (var handle in handles)
			{
				var arguments = string.Format("-c {1} -y -p {0} {2}", handle.Pid, handle.Handle, path);

				RunHandle(arguments);

				AppCore.WriteLine("@PX Closed handle {0} to {1}", handle.Handle, path);
			}
		}

		[STAThread]
		public static void Main()
		{
			try
			{
				var postofficeSetting = AppConfig.RequiredSetting("postoffice");

				Postoffice = Path.GetDirectoryName(postofficeSetting);

				//	ReSharper disable once AssignNullToNotNullAttribute
				var fsw = new FileSystemWatcher(Postoffice) { EnableRaisingEvents = true, Filter = PARENT_LOCK };
				
				fsw.Changed += OnParentLock;
				fsw.Created += OnParentLock;
			}
			catch (Exception)
			{
				var message = string.Format("Ошибка подписки на события postoffice {0}.\n\nПроверь настройки в файле конфигурации.", Postoffice ?? "???");
				MessageBox.Show(message, @"parent.unlock");
				return;
			}

			//	ReSharper disable once UnusedVariable
			var icon = new NotifyIcon
			{
				Icon = MainResources.splash_1,
				Visible = true,

				ContextMenu = new ContextMenu( new[]
				{
					new MenuItem("Exit", OnExit),
				})
			};

			CheckAndUnlock();
			
			ParallelTask.StartNew(Guard);

			Application.Run();

			AppCore.Stop();
		}

		private static void OnParentLock(object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			GuardSignal = true;
		}

		private static void OnExit(object sender, EventArgs eventArgs)
		{
			Application.Exit();
		}
	}
}
