#if NETCOREAPP
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.SingleInstance.Logic
{
	public sealed class NamedPipeCommunicator : IInstanceCommunicator
	{
		async void IInstanceCommunicator.RegisterServer(String name)
		{
			_ = Task.Run(async () =>
			{
				using NamedPipeServerStream pipeServer = new NamedPipeServerStream(name, PipeDirection.InOut, 1);
				while(true)
				{
					await pipeServer.WaitForConnectionAsync();
					using StreamReader reader = new StreamReader(pipeServer, Encoding.UTF8);
					String data = await reader.ReadToEndAsync();
					// TODO: trigger callback logic
					pipeServer.Disconnect();
				}
			});
		}

		async void IInstanceCommunicator.SendMessage(String name, String[] args)
		{
			using NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", name, PipeDirection.Out);
			await pipeClient.ConnectAsync(1000);
			String message = String.Join("|", args);
			using StreamWriter writer = new StreamWriter(pipeClient, Encoding.UTF8) { AutoFlush = true };
			await writer.WriteAsync(message);
		}
	}
}
#endif