using System;
using System.Windows.Forms;
using SAL.Flatbed;

namespace Plugin.SingleInstance
{
	public class Plugin : IPlugin, IPluginSettings<PluginSettings>
	{
		private PluginSettings _settings;

		internal IHost Host { get; }

		/// <summary>Settings for interaction from the host</summary>
		Object IPluginSettings.Settings => this.Settings;

		/// <summary>Settings for interaction from the plugin</summary>
		public PluginSettings Settings
		{
			get
			{
				if(this._settings == null)
				{
					this._settings = new PluginSettings(this);
					this.Host.Plugins.Settings(this).LoadAssemblyParameters(this._settings);
				}
				return this._settings;
			}
		}

		private ITraceSource Trace { get; }

		public Plugin(IHost host, ITraceSource trace)
		{
			this.Host = host ?? throw new ArgumentNullException(nameof(host));
			this.Trace = trace ?? throw new ArgumentNullException(nameof(trace));
		}

		Boolean IPlugin.OnConnection(ConnectMode mode)
		{
			if(this.Settings.Enable
				&& !ApplicationInstanceManager.CreateSingleInstance(this.Trace, this.Settings.ApplicationName, this.SingleInstanceCallback))
				return false;
			return true;
		}

		Boolean IPlugin.OnDisconnection(DisconnectMode mode)
			=> true;

		private void SingleInstanceCallback(Object sender, InstanceCallbackEventArgs args)
		{
			if(this.Host.Object is Form frm)
				frm.Activate();
		}
	}
}