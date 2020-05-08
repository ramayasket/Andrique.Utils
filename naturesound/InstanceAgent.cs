using System.Diagnostics;
using System.Threading;

namespace Andrique.Utils.NatureSound
{
	/// <summary>
	/// Maintains singleton mutex
	/// </summary>
	internal static class InstanceAgent
	{
		public static bool First
		{
			get { return _first; }
		}

		static InstanceAgent()
		{
			var singletonMutexName = typeof (InstanceAgent).FullName;

			Debug.Assert(null != singletonMutexName);

			try
			{
				_singletonMutex = Mutex.OpenExisting(singletonMutexName);
			}
			catch (WaitHandleCannotBeOpenedException)
			{
				_singletonMutex = new Mutex(false, typeof(InstanceAgent).FullName);
				_first = true;
			}
		}

		public static void f()
		{
			_singletonMutex.WaitOne();
			_singletonMutex.ReleaseMutex();
		}

		internal static readonly Mutex _singletonMutex;
		static readonly bool _first;
	}
}
