using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Kw.Common;

namespace Andrique.Utils
{
	public abstract class UtilityBase
	{
		private const string COPYRIGHT = "Copyright (C) Andrei Samoylov 2004-";
		private const int LAST_YEAR = 2016;

		private string Copyright
		{
			get
			{
				var year = Math.Max(DateTime.Now.Year, LAST_YEAR);
				var copyright = COPYRIGHT + year;

				return copyright;
			}
		}

		public int Run(string[] args)
		{
			var type = GetType();
			var @namespace = type.Namespace;
			var assembly = type.Assembly;
			var full = assembly.FullName;

			const string VHEADER = "Version=";
			string version = string.Empty;
			
			if (!String.IsNullOrEmpty(full))
			{
				foreach (var part in full.Split(", "))
				{
					if (part.StartsWith(VHEADER))
					{
						version = " " + part.Replace(VHEADER, string.Empty);
					}
				}
			}
			
			Console.WriteLine(@namespace + version);
			Console.WriteLine(Copyright);
			
			var cmdLineArgs = Environment.GetCommandLineArgs();

			if (RequiredArgumentCount > args.Length)
			{
				var exeName = Path.GetFileNameWithoutExtension(cmdLineArgs[0]);

				PrintUsage(exeName);

				return (int)Constants.ReturnCodes.RequiredArgumentMissing;
			}

			return UtilityRun(args);
		}

		public virtual void PrintUsage(string exeName)
		{
		}

		public virtual int UtilityRun(string[] args)
		{
			return 0;
		}

		public virtual int RequiredArgumentCount
		{
			get { return 0; }
		}
	}
}
