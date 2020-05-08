using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessM
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var driveM = new DirectoryInfo("M:\\");
				var mfiles = driveM.EnumerateFileSystemInfos("*").ToArray();
			}
			catch
			{
			}
		}
	}
}
