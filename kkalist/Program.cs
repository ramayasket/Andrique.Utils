using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kkalist
{
	class Program
	{
		static void Main(string[] args)
		{
			var actors = File.ReadAllText("S:\\24a.txt").Replace(Environment.NewLine, "");
		}
	}
}
