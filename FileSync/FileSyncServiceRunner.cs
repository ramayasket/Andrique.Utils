using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Kw.Common;
using Kw.Common.Threading;
using Kw.ServiceProcess;

namespace Andrique.Utils.FileSync
{
	public class FileSyncServiceRunner : ServiceRunner
	{
		public override string Name => "FileSync Service";

		protected override void Configure()
		{
			var sourceRoot = AppConfig.RequiredSetting("SourceRoot");
			var targetRoot = AppConfig.RequiredSetting("TargetRoot");

			if (!Directory.Exists(sourceRoot))
			{
				throw new InvalidOperationException(string.Format("@PX Source directory does not exist: " + sourceRoot));
			}

			if (!Directory.Exists(targetRoot))
			{
				throw new InvalidOperationException(string.Format("@PX Target directory does not exist: " + targetRoot));
			}
		}

		protected override void Initialize(params string[] parameters)
		{
			if (AppCore.Exiting)
			{
				this.Stop();
			}

		}

		protected override void Cleanup()
		{
		}
	}
}
