using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andrique.Utils.FileSync
{
	public enum Item
	{
		File,
		Folder
	}

	public enum Linkage
	{
		Direct,
		//
		//	http://troyparsons.com/blog/2012/03/symbolic-links-in-c-sharp/
		//
		Linkage
	}

	public enum Action
	{
		Change,
		Rename,
		Delete
	}

	public class WorkItem
	{
		public Item Item { get; set; }
		public Linkage Linkage { get; set; }
		public Action Action { get; set; }
		public string RelativePath { get; set; }

		public string GetAbsolutePath(string root)
		{
			return Path.Combine(root, RelativePath);
		}

		//public static WorkItem From
	}
}
