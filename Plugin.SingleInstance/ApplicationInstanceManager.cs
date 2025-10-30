using System;
using System.Diagnostics;
using System.Threading;
using Plugin.SingleInstance.Logic;

namespace Plugin.SingleInstance
{
	/// <summary>Application Instance Manager</summary>
	public static class ApplicationInstanceManager
	{
		/// <summary>Creates the single instance.</summary>
		/// <param name="name">The name.</param>
		/// <param name="callback">The callback.</param>
		/// <returns></returns>
		public static Boolean CreateSingleInstance(TraceSource trace, String name, EventHandler<InstanceCallbackEventArgs> callback)
		{
			EventWaitHandle eventWaitHandle = null;
			String eventName = $"{Environment.MachineName}-{name}";

			InstanceProxy.IsFirstInstance = false;
			InstanceProxy.CommandLineArgs = Environment.GetCommandLineArgs();

			try
			{
				// try opening existing wait handle
				eventWaitHandle = EventWaitHandle.OpenExisting(eventName);
			} catch
			{
				// init handle
				eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, eventName);

				// got exception = handle wasn't created yet
				InstanceProxy.IsFirstInstance = true;
			}

			IInstanceCommunicator communicator =
#if NETFRAMEWORK
				new LegacyRemotingCommunicator();
#else
				new NamedPipeCommunicator();
#endif

			if(InstanceProxy.IsFirstInstance)
			{
				communicator.RegisterServer(name);

				// register wait handle for this instance (process)
				ThreadPool.RegisterWaitForSingleObject(eventWaitHandle, (_, __) =>
				{
					callback?.Invoke(null, new InstanceCallbackEventArgs(InstanceProxy.IsFirstInstance, Environment.GetCommandLineArgs()));
				}, null, Timeout.Infinite, false);
			} else
			{
				// pass console arguments to shared object
				communicator.SendMessage(name, Environment.GetCommandLineArgs());

				trace.TraceEvent(TraceEventType.Stop, 1, "Another instance already running");

				// kill current process
				Environment.Exit(0);
			}

			return InstanceProxy.IsFirstInstance;
		}
	}
}