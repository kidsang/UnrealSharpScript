using System.Runtime.InteropServices;

namespace SharpScript
{
	public static class Preload
	{
		// Mixed debugging in Visual Studio requires breakpoints to be set after CoreCLR is loaded.
		// The purpose of this function is to load CoreCLR as early as possible.
		[UnmanagedCallersOnly]
		static int PreloadForDebugger()
		{
			Console.WriteLine("SharpScript PreloadForDebugger done");
			return 0;
		}
	}
}
