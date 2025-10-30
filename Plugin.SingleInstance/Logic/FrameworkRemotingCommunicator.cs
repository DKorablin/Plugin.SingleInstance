#if NETFRAMEWORK

using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;

namespace Plugin.SingleInstance.Logic
{
	internal sealed class LegacyRemotingCommunicator : IInstanceCommunicator
	{
		void IInstanceCommunicator.RegisterServer(String name)
		{
			IpcServerChannel serverChannel = new IpcServerChannel(Environment.MachineName + name);
			ChannelServices.RegisterChannel(serverChannel, true);
			RemotingConfiguration.RegisterWellKnownServiceType(typeof(InstanceProxy), name, WellKnownObjectMode.Singleton);
		}

		void IInstanceCommunicator.SendMessage(String name, String[] args)
		{
			IpcClientChannel clientChannel = new IpcClientChannel();
			ChannelServices.RegisterChannel(clientChannel, true);
			InstanceProxy proxy = Activator.GetObject(typeof(InstanceProxy),
				$"ipc://{Environment.MachineName}{name}/{name}") as InstanceProxy;
			proxy?.SetCommandLineArgs(false, args);
			ChannelServices.UnregisterChannel(clientChannel);
		}
	}
}
#endif