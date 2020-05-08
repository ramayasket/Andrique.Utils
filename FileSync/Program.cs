using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using Kw.ServiceProcess;

namespace Andrique.Utils.FileSync
{
    static class Program
    {
		static void Main(string[] args)
		{
			var runner = new FileSyncServiceRunner();
			var service = new ServiceProgram(runner);

			service.ServiceMain(args);
		}
	}
}