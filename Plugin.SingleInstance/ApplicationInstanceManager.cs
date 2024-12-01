using System;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;

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
			String eventName = String.Join("-", new String[] { Environment.MachineName, name });

			InstanceProxy.IsFirstInstance = false;
			InstanceProxy.CommandLineArgs = Environment.GetCommandLineArgs();

			try
			{
				// try opening existing wait handle
				eventWaitHandle = EventWaitHandle.OpenExisting(eventName);
			} catch
			{
				// got exception = handle wasn't created yet
				InstanceProxy.IsFirstInstance = true;
			}

			if(InstanceProxy.IsFirstInstance)
			{
				// init handle
				eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, eventName);

				// register wait handle for this instance (process)
				ThreadPool.RegisterWaitForSingleObject(eventWaitHandle, WaitOrTimerCallback, callback, Timeout.Infinite, false);
				eventWaitHandle.Close();

				// register shared type (used to pass data between processes)
				RegisterRemoteType(name);
			} else
			{
				// pass console arguments to shared object
				UpdateRemoteObject(name);

				// invoke (signal) wait handle on other process
				eventWaitHandle?.Set();

				trace.TraceEvent(TraceEventType.Stop, 1, "Anoter instance already running");
				// kill current process
				Environment.Exit(0);
			}

			return InstanceProxy.IsFirstInstance;
		}

		/// <summary>Updates the remote object.</summary>
		/// <param name="url">The remote URI.</param>
		private static void UpdateRemoteObject(String url)
		{
			// register net-pipe channel
			IpcClientChannel clientChannel = new IpcClientChannel();
			ChannelServices.RegisterChannel(clientChannel, true);

			// get shared object from other process
			InstanceProxy proxy =
				Activator.GetObject(typeof(InstanceProxy),
				$"ipc://{Environment.MachineName}{url}/{url}") as InstanceProxy;

			// pass current command line args to proxy
			proxy?.SetCommandLineArgs(InstanceProxy.IsFirstInstance, InstanceProxy.CommandLineArgs);

			// close current client channel
			ChannelServices.UnregisterChannel(clientChannel);
		}

		/// <summary>Registers the remote type.</summary>
		/// <param name="url">The URI.</param>
		private static void RegisterRemoteType(String url)
		{
			// register remote channel (net-pipes)
			IpcServerChannel serverChannel = new IpcServerChannel(Environment.MachineName + url);
			ChannelServices.RegisterChannel(serverChannel, true);

			// register shared type
			RemotingConfiguration.RegisterWellKnownServiceType(
				typeof(InstanceProxy), url, WellKnownObjectMode.Singleton);

			// close channel, on process exit
			Process process = Process.GetCurrentProcess();
			process.Exited += delegate { ChannelServices.UnregisterChannel(serverChannel); };
		}

		/// <summary>Wait Or Timer Callback Handler</summary>
		/// <param name="state">The state.</param>
		/// <param name="timedOut">if set to <c>true</c> [timed out].</param>
		private static void WaitOrTimerCallback(Object state, Boolean timedOut)
		{
			if(state is EventHandler<InstanceCallbackEventArgs> callback)
				callback(state, new InstanceCallbackEventArgs(InstanceProxy.IsFirstInstance, InstanceProxy.CommandLineArgs));
		}
	}
}