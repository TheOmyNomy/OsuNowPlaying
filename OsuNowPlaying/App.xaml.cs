using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using OsuNowPlaying.Config;
using OsuNowPlaying.Shared.Utilities;
using OsuNowPlaying.Windows;

namespace OsuNowPlaying;

public partial class App
{
#if DEBUG
	[DllImport("kernel32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool AllocConsole();

	[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
	private static extern bool FreeConsole();
#endif

	public static readonly string WorkingPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "osu!np");

	public static string CurrentExecutablePath =>
		Process.GetCurrentProcess().MainModule?.FileName ?? $"{Directory.GetCurrentDirectory()}\\{Process.GetCurrentProcess().ProcessName}.exe";

	private readonly IServiceProvider _serviceProvider;

	public App()
	{
		_serviceProvider = new ServiceCollection()
			.AddSingleton<Configuration>()
			.AddSingleton<MainWindow>()
			.BuildServiceProvider();
	}

	private void OnStartup(object sender, StartupEventArgs e)
	{
#if DEBUG
		AllocConsole();
#endif

		if (new Version(OSVersionInfo.MajorVersion, OSVersionInfo.MinorVersion) < new Version(6, 2))
			ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

		Directory.CreateDirectory(WorkingPath);
		_serviceProvider.GetRequiredService<Configuration>();

		MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
		mainWindow.Show();

		string[] args = Environment.GetCommandLineArgs();

		foreach (string arg in args)
		{
			if (arg == "--migration")
			{
				new MigrationWindow
				{
					Owner = mainWindow
				}.Show();
			}
		}
	}

#if DEBUG
	protected override void OnExit(ExitEventArgs e)
	{
		FreeConsole();
		base.OnExit(e);
	}
#endif
}