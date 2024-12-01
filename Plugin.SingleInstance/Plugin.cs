using System;
using System.Diagnostics;
using System.Windows.Forms;
using SAL.Flatbed;

namespace Plugin.SingleInstance
{
	public class Plugin : IPlugin, IPluginSettings<PluginSettings>
	{
		private TraceSource _trace;
		private PluginSettings _settings;

		internal IHost Host { get; }

		/// <summary>Настройки для взаимодействия из хоста</summary>
		Object IPluginSettings.Settings { get => this.Settings; }

		/// <summary>Настройки для взаимодействия из плагина</summary>
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

		internal TraceSource Trace { get => this._trace ?? (this._trace = Plugin.CreateTraceSource<Plugin>()); }

		public Plugin(IHost host)
			=> this.Host = host ?? throw new ArgumentNullException(nameof(host));

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

		private static TraceSource CreateTraceSource<T>(String name = null) where T : IPlugin
		{
			TraceSource result = new TraceSource(typeof(T).Assembly.GetName().Name + name);
			result.Switch.Level = SourceLevels.All;
			result.Listeners.Remove("Default");
			result.Listeners.AddRange(System.Diagnostics.Trace.Listeners);
			return result;
		}
	}
}