using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Andrique.Utils;

namespace Andrique.Utils.RenameTorrent
{
	class Program : ConsoleUtilityBase
	{
		const int PASSKEY_LENGTH = 32;

		static int Main(string[] args)
		{
			return new Program().Run(args);
		}

		int _torrentFileLength;
		byte[] _buffer;

		const string TARGET = @"S:\inbox\.torrents.completed";
		const string NAME = @"name";
		const string NAME_UTF_8 = @"name.utf-8";
		const string MASK = @"*.torrent";

		public override int RequiredArgumentCount
		{
			get { return 0; }
		}

		public override int UtilityRun(string[] args)
		{
			var torrents = Directory.EnumerateFiles(TARGET, MASK).ToArray();

			foreach (var torrent in torrents)
			{
				using (var torrentFile = File.Open(torrent, FileMode.Open, FileAccess.Read))
				{
					_torrentFileLength = Convert.ToInt32(torrentFile.Length);

					_buffer = new byte[_torrentFileLength];

					var bytesRead = torrentFile.Read(_buffer, 0, _torrentFileLength);

					Debug.Assert(_torrentFileLength == bytesRead);

				}

				var encoded = _buffer;//.Take(position).ToArray();
				var encodedText = Encoding.ASCII.GetString(encoded);

				var e = BencodeDecoder.Decode(encodedText);

				var info = ((BDictionary) e[0])["info"] as BDictionary;

				Debug.Assert(null != info);

				BString value = null;

				if (info.ContainsKey(NAME_UTF_8))
				{
					value = (BString)info[NAME_UTF_8];
				}
				else if (info.ContainsKey(NAME))
				{
					value = (BString)info[NAME];
				}

				if (null != value)
				{
					//

					if (!value.Value.Contains("?"))
					{
						try
						{
							var newname = Path.Combine(TARGET, value.Value + ".torrent");
							File.Copy(torrent, newname);
						}
						catch {}
					}
					else
					{
						Console.WriteLine("{0} --- {1}", torrent, value);
					}
				}
			}
			
			return 0;
		}

		int FindString(int start, params string[] whats)
		{
			foreach (var what in whats)
			{
				int ix = start;
				int ixLimit = _torrentFileLength - what.Length;

				var passKeyBuffer = Encoding.ASCII.GetBytes(what);

				while (ix <= ixLimit)
				{
					var found = true;

					for (int i = 0; i < what.Length; i++)
					{
						if (_buffer[ix + i] != passKeyBuffer[i])
						{
							found = false;
							break;
						}
					}

					if (found)
					{
						return ix;
					}

					ix++;
				}
			}

			return -1;
		}
	}
}
