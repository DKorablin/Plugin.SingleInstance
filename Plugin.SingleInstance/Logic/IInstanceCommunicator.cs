using System;

namespace Plugin.SingleInstance.Logic
{
	public interface IInstanceCommunicator
	{
		void RegisterServer(String name);
		void SendMessage(String name, String[] args);
	}
}