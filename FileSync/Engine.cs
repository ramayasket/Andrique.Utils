using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Kw.Common;

namespace Andrique.Utils.FileSync
{
	public static class Engine
	{
		private static string SourceRoot;
		private static string TargetRoot;
		private static FileSystemWatcher Watcher;

		public static void Initialize(string sourceRoot, string targetRoot)
		{
			SourceRoot = sourceRoot;
			TargetRoot = targetRoot;

			Watcher = new FileSystemWatcher();
			Watcher.Path = SourceRoot;

			Watcher.IncludeSubdirectories = true;
			Watcher.NotifyFilter = NotifyFilters.DirectoryName;

			Watcher.NotifyFilter = Watcher.NotifyFilter | NotifyFilters.FileName;
			Watcher.NotifyFilter = Watcher.NotifyFilter | NotifyFilters.Attributes;

			Watcher.Changed += Changed;
			Watcher.Created += Created;
			Watcher.Deleted += Deleted;

			Watcher.Renamed += Renamed;

			// And at last.. We connect our EventHandles to the system API (that is all
			// wrapped up in System.IO)
			try
			{
				Watcher.EnableRaisingEvents = true;

				//ParallelTask.StartNew(StartupUpdate);
			}
			catch (ArgumentException ex)
			{
				AppCore.WriteLine("@PX Watch failed: " + ex.Message);
			}
		}
		private static string[] EnumerateFolder(string folder)
		{
			var list = new List<string>();

			var files = Directory.EnumerateFiles(folder).ToArray();
			var folders = Directory.EnumerateDirectories(folder).ToArray();

			list.AddRange(files);

			foreach (var f in folders)
			{
				var path = Path.Combine(folder, f);
				var children = EnumerateFolder(path);
				list.AddRange(children);
			}

			return list.ToArray();
		}

		private static void UpdateSource(string source)
		{
			var target = source.Replace(SourceRoot, TargetRoot);
			var sourceTime = File.GetLastWriteTime(source);

			//	The file could have been deleted by this time
			if (sourceTime.Year < 1900)
				return;

			var sourceDir = Path.GetDirectoryName(source);

			Debug.Assert(null != sourceDir);

			if (sourceDir != SourceRoot)
			{
				var targetDir = sourceDir.Replace(SourceRoot, TargetRoot);

				if (!Directory.Exists(targetDir))
				{
					Directory.CreateDirectory(targetDir);
				}
			}

			var targetTime = File.GetLastWriteTime(target);

			if (targetTime < sourceTime)
			{
				var args = new FileSystemEventArgs(WatcherChangeTypes.Changed, sourceDir, Path.GetFileName(source));
				CopyFile(args);
			}
		}

		private static void UpdateTargetFolder(string folder)
		{

		}

		private static void StartupUpdate()
		{
			string[] sources = EnumerateFolder(SourceRoot);
			string[] targets = EnumerateFolder(TargetRoot);

			AppCore.WriteLine("@PX Starting source update");

			var sourceFolders = sources.Select(Path.GetDirectoryName).Distinct().ToArray();

			foreach (var f in sourceFolders)
			{
			}

			foreach (var source in sources)
			{
				UpdateSource(source);
			}

			AppCore.WriteLine("@PX Source update finished");
			AppCore.WriteLine("@PX Starting target update");

			var targetFolders = targets.Select(Path.GetDirectoryName).Distinct().ToArray();

			foreach (var f in targetFolders)
			{
			}
		}

		private static void Changed(object sender, FileSystemEventArgs e)
		{
			CopyFile(e);
		}

		private static void Created(object sender, FileSystemEventArgs e)
		{
			CopyFile(e);
		}

		private static void Deleted(object sender, FileSystemEventArgs e)
		{
			DeleteFile(e);
		}

		public static void Renamed(object sender, RenamedEventArgs e)
		{
			RenameFile(e);
		}

		private static void CopyFile(FileSystemEventArgs e)
		{
			try
			{
				string source = e.FullPath;
				bool exists = Directory.Exists(source);
				string dest = source.Replace(SourceRoot, TargetRoot);

				if (exists)
				{
					Directory.CreateDirectory(dest);
				}
				else
				{
					File.Copy(source, dest, true);
				}
			}
			catch (Exception ioe)
			{
				AppCore.WriteLine("@PX Copy failed: " + ioe.Message);
			}
		}

		private static void RenameFile(RenamedEventArgs e)
		{
			try
			{
				string source = e.OldFullPath.Replace(SourceRoot, TargetRoot);
				string dest = e.FullPath.Replace(SourceRoot, TargetRoot);
				bool exists = Directory.Exists(source);
				if (exists)
				{
					Directory.Move(source, dest);
				}
				else
				{
					File.Move(source, dest);
				}
			}
			catch (Exception ioe)
			{
				AppCore.WriteLine("@PX Rename failed: " + ioe.Message);
			}
		}

		private static void DeleteFile(FileSystemEventArgs e)
		{
			try
			{
				string dest = e.FullPath.Replace(SourceRoot, TargetRoot);
				bool exists = Directory.Exists(dest);

				if (exists)
				{
					Directory.Delete(dest, true);
				}
				else
				{
					File.Delete(dest);
				}
			}
			catch (Exception ioe)
			{
				AppCore.WriteLine("@PX Delete failed: " + ioe.Message);
			}
		}
	}
}
