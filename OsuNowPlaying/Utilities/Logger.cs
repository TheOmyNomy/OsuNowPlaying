using System;

namespace OsuNowPlaying.Utilities;

public class Logger
{
#if DEBUG
	public static LogLevel LogLevel = LogLevel.Debug;
#else
	public static LogLevel LogLevel = LogLevel.Error;
#endif

	public static void Information(object value) => Log(LogLevel.Information, value);
	public static void Warning(object value) => Log(LogLevel.Warning, value);
	public static void Error(object value) => Log(LogLevel.Error, value);
	public static void Debug(object value) => Log(LogLevel.Debug, value);

	private static void Log(LogLevel logLevel, object value)
	{
		if (logLevel > LogLevel) return;

		ConsoleColor backgroundColor = Console.BackgroundColor;
		ConsoleColor foregroundColor = Console.ForegroundColor;

		Console.BackgroundColor = ConsoleColor.Black;

		if (logLevel == LogLevel.Debug) Console.ForegroundColor = ConsoleColor.DarkGray;
		else if (logLevel == LogLevel.Error) Console.ForegroundColor = ConsoleColor.Red;
		else if (logLevel == LogLevel.Warning) Console.ForegroundColor = ConsoleColor.Yellow;
		else Console.ForegroundColor = ConsoleColor.Gray;

		Console.WriteLine($"{DateTime.Now:HH:mm:ss} [{logLevel}]: {value}");

		Console.ForegroundColor = foregroundColor;
		Console.BackgroundColor = backgroundColor;
	}
}