# Single Instance Plugin

[![Auto build](https://github.com/DKorablin/Plugin.SingleInstance/actions/workflows/release.yml/badge.svg)](https://github.com/DKorablin/Plugin.SingleInstance/releases/latest)
[![NuGet](https://img.shields.io/nuget/v/AlphaOmega.SAL.Plugin.SingleInstance)](https://www.nuget.org/packages/AlphaOmega.SAL.Plugin.SingleInstance)

A [SAL](https://github.com/DKorablin/SAL.Windows) plugin that restricts a Windows application to a single running instance. When a second instance is launched, its command-line arguments are forwarded to the first instance and the duplicate process exits immediately via `Environment.Exit(0)`.

## Features

- Enforces single-instance execution per machine
- Forwards command-line arguments from the duplicate instance to the running one
- Brings the main application window to the foreground when a second launch is attempted
- Dual IPC backend — automatically selected at compile time:
  - **.NET Framework** (`net48`): IPC channel via .NET Remoting (`System.Runtime.Remoting`)
  - **.NET 8+** (`net8.0-windows`): Named Pipes (`System.IO.Pipes`)

## Target Frameworks

| Framework | IPC Transport |
|---|---|
| .NET Framework 4.8 | .NET Remoting over IPC |
| .NET 8 Windows | Named Pipes |

## Settings

The plugin exposes one configurable property accessible through the SAL host settings UI:

| Property | Type | Category | Description |
|---|---|---|---|
| `Enable` | `Boolean` | Automation | Activates the single-instance restriction |

When `Enable` is `false` the plugin loads normally without enforcing any instance limit.

## How It Works

1. On connection (`IPlugin.OnConnection`), the plugin calls `ApplicationInstanceManager.CreateSingleInstance`.
2. An `EventWaitHandle` named `<MachineName>-<AppName>` is opened or created.
   - **First instance** — creates the handle, registers an IPC server, and sets up a thread-pool wait. When signalled, the registered callback fires and activates the main `Form`.
   - **Subsequent instances** — open the existing handle, send their command-line arguments to the first instance over IPC, log a `TraceEventType.Stop` event, and terminate.
3. The application name is derived from `Assembly.GetEntryAssembly()` (or `Process.GetCurrentProcess().ProcessName`) combined with the IDs of all registered `IPluginKernel` plugins, ensuring uniqueness per host configuration.

## Installation

1. Download the release archive (.zip or .nupkg).
2. Place the plugin assembly into the host application plugin directory (SAL / host supporting Windows environment):
	- [Flatbed.Dialog](https://dkorablin.github.io/Flatbed-Dialog/)
	- [Flatbed.Dialog (Lite)](https://dkorablin.github.io/Flatbed-Dialog-Lite)
	- [Flatbed.MDI](https://dkorablin.github.io/Flatbed-MDI)
	- [Flatbed.MDI (WPF)](https://dkorablin.github.io/Flatbed-MDI-Avalon)
	- [Flatbed.MDI (AvaloniaUI)](https://dkorablin.github.io/Flatbed-MDI-AvaloniaUI)

## License

[MIT](https://opensource.org/licenses/MIT) © Danila Korablin