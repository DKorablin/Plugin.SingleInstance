using System.Reflection;
using System.Runtime.InteropServices;

[assembly: Guid("0837f1bb-fb6b-42fe-8f4d-1ee0706caa47")]
[assembly: System.CLSCompliant(true)]

#if NETCOREAPP
[assembly: AssemblyMetadata("ProjectUrl", "https://dkorablin.ru/project/Default.aspx?File=121")]
#else

[assembly: AssemblyDescription("Limit application to single instance")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2011-2025")]
#endif