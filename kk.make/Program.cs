using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Andrique.Utils;

namespace Andrique.Utils.KK_MakeFolder
{
	class Program : ConsoleUtilityBase
	{
		static int Main(string[] args)
		{
			return new Program().Run(args);
		}

		const string KK = @"(www.kinokopilka.tv)";
		const string BD = @"[BDRip-{0}p]";
		const string MKV = @".mkv";
		const string JPG = @".jpg";
		const string URLFILE = @"KinoKopilka.tv - Best Movies BitTorrent Network.url";
		const string WRNFILE = @"Внимание формат MKV.txt";

		const int BD10 = 1080;
		const int BD7 = 720;

		const string OPTION_BD10 = @"/10";
		const string OPTION_BD10A = @"-10";
		const string OPTION_BD7 = @"/7";
		const string OPTION_BD7A = @"-7";

		public override int RequiredArgumentCount
		{
			get { return 1; }
		}

		public override int UtilityRun(string[] args)
		{
			int ix = 1;
			int bdRip = 0;

			var nameParts = new List<string> {args[0]};

			while (ix < args.Length)
			{
				switch(args[ix++].ToLower())
				{
					case OPTION_BD10:
					case OPTION_BD10A:
						bdRip = BD10;
						break;

					case OPTION_BD7:
					case OPTION_BD7A:
						bdRip = BD7;
						break;
				}
			}

			if (0 != bdRip)
			{
				nameParts.Add(string.Format(BD, bdRip));
			}

			nameParts.Add(KK);

			var name = string.Join(" ", nameParts);

			var cwd = Directory.GetCurrentDirectory();
			var target = Path.Combine(cwd, name);

			Directory.CreateDirectory(target);

			SaveToFile(TextData.URL + Environment.NewLine, Path.Combine(target, URLFILE));
			SaveToFile(TextData.WRN, Path.Combine(target, WRNFILE));

			SaveToFile(string.Empty, Path.Combine(target, name + MKV));
			SaveToFile(string.Empty, Path.Combine(target, name + JPG));

			return 0;
		}

		void SaveToFile(string data, string path)
		{
			using (var writer = new StreamWriter(path))
			{
				writer.Write(data);
			}
		}
	}
}
