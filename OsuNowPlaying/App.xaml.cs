using System.Runtime.InteropServices;
using System.Windows;

namespace OsuNowPlaying;

public partial class App
{
#if DEBUG
	[DllImport("kernel32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool AllocConsole();

	[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
	private static extern bool FreeConsole();

	protected override void OnStartup(StartupEventArgs e)
	{
		AllocConsole();
		base.OnStartup(e);
	}

	protected override void OnExit(ExitEventArgs e)
	{
		FreeConsole();
		base.OnExit(e);
	}
#endif
}