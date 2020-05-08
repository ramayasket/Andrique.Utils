using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace torrentsync
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Directory.SetCurrentDirectory (@"c:\inbox");

			var folders = Directory.EnumerateDirectories(@".").Select(d => d.Replace(@".\", string.Empty)).Where(d => !d.StartsWith(".")).ToArray();

			List<string> files = new List<string>();

			Directory.SetCurrentDirectory (@"c:\inbox\.torrents.completed");
			files.AddRange (Directory.EnumerateFiles (@"."));

			Directory.SetCurrentDirectory (@"c:\inbox\.torrents.active");
			files.AddRange (Directory.EnumerateFiles (@"."));

			files = files.Select (f => f.Replace(@".\", string.Empty).Replace(".torrent", string.Empty)).ToList ();

			var extra = folders.Except (files).Where(e => e.Contains("kinokopilka")).ToList();

			Directory.SetCurrentDirectory (@"c:\");

			using (var w = new StreamWriter("kk.torrentsync.txt"))
			{
				foreach (var ex in extra)
				{
					w.WriteLine (ex);
				}
			}
			//
		}
	}
}
