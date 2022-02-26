using System;
using System.Reflection;

namespace OsuNowPlaying.Utilities;

public static class VersionManager
{
	private static string? _version;

	public static string Version
	{
		get
		{
			if (!string.IsNullOrWhiteSpace(_version))
				return _version;

			Version? assemblyVersion = Assembly.GetEntryAssembly()?.GetName().Version;
			_version = assemblyVersion?.Major > 1 ? $"{assemblyVersion.Major}.{assemblyVersion.Minor:D2}.{assemblyVersion.Build:D2}.{assemblyVersion.Revision}" : "Unknown";

			return _version;
		}
	}
}