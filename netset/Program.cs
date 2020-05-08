using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics.Contracts;

namespace netset
{
	class Program
	{
		const string _vpnName = "w.Andrik.HTSTS";

		const string _routeExe = "route.exe";
		const string _rasDialExe = "rasdial.exe";

		const string _ip0 = "0.0.0.0";
		const string _ip255 = "255.255.255.255";

		const string _vpnTargetsFile = @"c:\usr\etc\vpntargets";
		const string _localTargetsFile = @"c:\usr\etc\localtargets";

		/// <summary>
		/// Executes a console command inside the main console
		/// </summary>
		/// <param name="processName">Command</param>
		/// <param name="commandLine">Command arguments</param>
		/// <param name="stdout">Buffer for stdout</param>
		/// <param name="redirect">stdout redirection flag</param>
		/// <returns>Process exit code</returns>
		static int Execute(string processName, string commandLine, out string stdout, bool redirect = false)
		{
			if (!redirect)
			{
				Console.WriteLine("{0} {1}", processName, commandLine);
			}
			//
			//  Describe the process
			//
			using (var process = new Process { StartInfo = { RedirectStandardOutput = redirect, UseShellExecute = false, FileName = processName, Arguments = commandLine } })
			{
				process.Start();
				process.WaitForExit();

				//
				//	Get stdout if requested
				//
				stdout =
					redirect ?
						process.StandardOutput.ReadToEnd()
						:
						string.Empty;

				Console.WriteLine(string.Empty);

				if (0 != process.ExitCode)
				{
					int i = 0;
				}

				return process.ExitCode;
			}
		}

		/// <summary>
		/// Executes a console command inside the main console
		/// </summary>
		/// <param name="processName">Command</param>
		/// <param name="commandLine">Command arguments</param>
		/// <returns>Process exit code</returns>
		static int Execute(string processName, string commandLine)
		{
			string ignored;
			return Execute(processName, commandLine, out ignored);
		}

		/// <summary>
		/// Finds interface # for w.Andrik.HTSTS adapter inside output of 'route print -4'
		/// </summary>
		static string GetInterfaceId(string routePrint, string look4)
		{
			var line =
				routePrint.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).SingleOrDefault(
					l => l.Contains(look4));

			if (null == line)
				return null;

			return line.Split(new[] { ' ', '.' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
		}

		//
		//	Precise set-up of my home network
		//
		static int Main(string [] args)
		{
			string stdout;	//	буфер для получения консольного вывода вызываемой программы

			var hostName = Environment.MachineName;

			//
			//	Разъединяем w.Andrik.HTSTS
			//
			Execute(_rasDialExe, _vpnName + " /disconnect", out stdout);

			//
			//	Форматный шаблон для route add
			//
			const string routeAddPattern = "add {0} mask {1} {2} metric {3} if {4}";

			//
			//	Печатаем карту маршрутизации
			//
			Execute(_routeExe, "print -4", out stdout, true);

			var xanAdapter = GetInterfaceId(stdout, "Atheros");
			var planAdapter = GetInterfaceId(stdout, "D-Link");

			const string xanGateway = "77.37.212.1";
			const string planGateway = "192.168.0.1";

			string routeXan = string.Format(routeAddPattern, _ip0, _ip0, xanGateway, "100", xanAdapter);
			string routePlan = string.Format(routeAddPattern, _ip0, _ip0, planGateway, "150", planAdapter);

			//
			//	Rewrite routes so ONLIME goes first
			//
			Execute(_routeExe, "delete 0.0.0.0");
			Execute(_routeExe, routeXan);
			Execute(_routeExe, routePlan);

			//
			//  Connect w.Andrik.HTSTS
			//
			var rasDial = Execute(_rasDialExe, _vpnName + " a.samoylov Eve!tl0gue /domain:ORG", out stdout);

			if (0 != rasDial)
				return rasDial;

			//
			//	Get interface id for w.Andrik.HTSTS
			//
			Execute(_routeExe, "print -4", out stdout, true);

			var vpnAdapter = GetInterfaceId(stdout, _vpnName);
			if (null == vpnAdapter)
			{
				Console.WriteLine("Cannot find interface for ");
				return -1;
			}

			const string xanAddress = "77.37.212.43";
			const string planAddress = "192.168.0.254";

			var ethAddresses = new string[] { xanAddress, planAddress };

			var vpnAddresses = Dns
				.GetHostEntry(hostName)
				.AddressList
				.Where(a => a.AddressFamily == AddressFamily.InterNetwork)
				.Where(a => !ethAddresses.Contains(a.ToString()))
				.Select(a => a.ToString())
				;

			var vpnAddress = vpnAddresses.FirstOrDefault();

			Debug.Assert(null != vpnAddress);

			//
			//	Get target addresses on HTSTS network
			//
			using (var reader = new StreamReader(_vpnTargetsFile))
			{
				while (reader.Peek() >= 0)
					Execute(_routeExe, string.Format(routeAddPattern, reader.ReadLine(), _ip255, vpnAddress, "50", vpnAdapter), out stdout);
			}


			//
			//	My full computer name
			//
			var fqcn = "andrik.homeip.net";

			var localAddress = Dns
				.GetHostEntry(fqcn)
				.AddressList
				.Select(a => a.ToString())
				.FirstOrDefault();

			if (null != localAddress)
			{
				//
				//	Get target addresses on HomeTrunkA network
				//
				using (var reader = new StreamReader(_localTargetsFile))
				{
					while (reader.Peek() >= 0)
					{
						var localTarget = reader.ReadLine();
						if (null == localTarget)
							continue;

						if (localTarget.ToLower().StartsWith(hostName.ToLower()))
							continue;

						var targetAddress = Dns
							.GetHostEntry(localTarget)
							.AddressList
							.Select(a => a.ToString())
							.FirstOrDefault();

						if (null == targetAddress)
							continue;

						Execute(_routeExe, "delete " + targetAddress);
						Execute(_routeExe, string.Format(routeAddPattern, targetAddress, _ip255, localAddress, "1", xanAdapter), out stdout);
					}
				}
			}

			//
			//	Display what we finally got
			//
			Execute(_routeExe, "print -4", out stdout, false);

			return 0;
		}
	}
}
