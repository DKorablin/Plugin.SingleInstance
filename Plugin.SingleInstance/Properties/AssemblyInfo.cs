using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: Guid("0837f1bb-fb6b-42fe-8f4d-1ee0706caa47")]
[assembly: System.CLSCompliant(true)]

#if NETCOREAPP
[assembly: AssemblyMetadata("ProjectUrl", "https://dkorablin.ru/project/Default.aspx?File=121")]
#else

[assembly: AssemblyTitle("Plugin.SingleInstance")]
[assembly: AssemblyDescription("Limit application to single instance")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("Danila Korablin")]
[assembly: AssemblyProduct("Plugin.SingleInstance")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2011-2024")]
#endif