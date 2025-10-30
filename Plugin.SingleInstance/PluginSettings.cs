using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using SAL.Flatbed;

namespace Plugin.SingleInstance
{
	public class PluginSettings
	{
		private readonly Plugin _plugin;
		internal PluginSettings(Plugin plugin)
			=> this._plugin = plugin;

		[Category("Automation")]
		[Description("Limit application to the single instance")]
		public Boolean Enable { get; set; }

		/// <summary>Get the name of the application for which the autoStart function is registered</summary>
		internal String ApplicationName
		{
			get
			{
				StringBuilder application = new StringBuilder(Assembly.GetEntryAssembly() != null
					? Assembly.GetEntryAssembly().GetName().Name
					: Process.GetCurrentProcess().ProcessName);

				foreach(IPluginDescription kernel in this._plugin.Host.Plugins.FindPluginType<IPluginKernel>())
					application.Append("|" + kernel.ID);

				return application.ToString();
			}
		}
	}
}