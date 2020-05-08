using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace cleartmp
{
	class Program
	{
		static void Main(string[] args)
		{
			var temp = ConfigurationManager.AppSettings["location"] ?? @"C:\var\temp";
			Clean(temp);
		}

		//
		//	ReSharper disable EmptyGeneralCatchClause
		//
		static void Clean(string @from)
		{
			Console.WriteLine("Cleaning {0}", from);

			var folders = Directory.EnumerateDirectories(@from, "*", SearchOption.TopDirectoryOnly).ToList();
			var files = Directory.EnumerateFiles(@from, "*", SearchOption.TopDirectoryOnly).ToList();

			foreach (var file in files)
			{
				try
				{
					File.Delete(file);
				}
				catch {}
			}

			foreach (var folder in folders)
			{
				Clean(folder);
				try
				{
					Directory.Delete(folder);
				}
				catch { }
			}
		}
		//
		//	ReSharper restore EmptyGeneralCatchClause
		//
	}
}
