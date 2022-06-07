﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using OsuNowPlaying.Config;
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

		Directory.CreateDirectory(WorkingPath);

		_serviceProvider.GetRequiredService<Configuration>();
		_serviceProvider.GetRequiredService<MainWindow>().Show();
	}

#if DEBUG
	protected override void OnExit(ExitEventArgs e)
	{
		FreeConsole();
		base.OnExit(e);
	}
#endif
}