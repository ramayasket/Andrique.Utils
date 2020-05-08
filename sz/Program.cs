using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Web;
using Kw.Networking;

namespace sz
{
	class Program
	{
		static int Main(string[] args)
		{
			if (args.Length < 1)
			{
				Console.WriteLine("Example: sz.exe http://site.domain/path");
				return 1;
			}

			var wc = new HttpClient();
			var pathes = new List<string>();

			try
			{
				var path = args[0];

				var text = wc.DownloadString(path);
				var listStart = "var images=[";
				var listEnd = "];";

				var imageStart = text.IndexOf(listStart) + listStart.Length;
				var imageEnd = text.IndexOf(listEnd, imageStart);

				var imageList = text.Substring(imageStart, imageEnd - imageStart);
				var images = imageList.Split(',');

				pathes = images.Select(i => i.Replace("\"", string.Empty)).ToList();

				//var filename = url.Split('/').Last();

				//File.WriteAllBytes(filename, bytes);

				//Console.WriteLine("Downloaded #{0}", i);
			}
			catch
			{
				Console.Write("Failed: invalid page format \r");
			}

			foreach (var path in pathes)
			{
				var filename = path.Split('/').Last()+".jpg";
				var wcf = new WebClient();

				try
				{
					var bytes = wcf.DownloadData(path);
					File.WriteAllBytes(filename, bytes);
					
					Console.WriteLine("Downloaded: {0}", filename);
				}
				catch
				{
					Console.WriteLine("Unable to download {0}", filename);
				}
			}

			return 0;
		}
	}
}
