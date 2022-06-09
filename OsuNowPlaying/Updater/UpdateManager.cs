using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OsuNowPlaying.Updater;

public static class UpdateManager
{
	private static string? _currentVersion;

	public static string CurrentVersion
	{
		get
		{
			if (!string.IsNullOrWhiteSpace(_currentVersion))
				return _currentVersion;

			Version? assemblyVersion = Assembly.GetEntryAssembly()?.GetName().Version;
			_currentVersion = assemblyVersion?.Major > 0 ? $"{assemblyVersion.Major}.{assemblyVersion.Minor:D2}.{assemblyVersion.Build:D2}.{assemblyVersion.Revision}" : "Unknown";

			return _currentVersion;
		}
	}

	public static string LatestVersion { get; private set; } = "Unknown";

	private static HttpClient? _client;

	private static HttpClient Client
	{
		get
		{
			if (_client == null)
			{
				_client = new HttpClient();
				_client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; rv:91.0) Gecko/20100101 Firefox/91.0");
			}

			return _client;
		}
	}

	private static readonly string OldExecutablePath = Path.Combine(App.WorkingPath, "_osu-np.exe");
	private static readonly string DownloadedExecutablePath = Path.Combine(App.WorkingPath, "osu-np.exe");

	private static GitHubRelease? _latestRelease;

	public static void Clean()
	{
		File.Delete(Path.Combine(App.WorkingPath, "Klserjht.exe"));

		File.Delete(OldExecutablePath);
		File.Delete(DownloadedExecutablePath);
	}

	public static async Task<bool> CheckAsync()
	{
		if (CurrentVersion == "Unknown")
			return false;

		_latestRelease = await GetLatestReleaseAsync();

		if (_latestRelease == null)
			return false;

		if (!string.IsNullOrWhiteSpace(_latestRelease.TagName))
			LatestVersion = _latestRelease.TagName;

		return LatestVersion != CurrentVersion;
	}

	public static async Task<bool> ApplyAsync()
	{
		if (_latestRelease == null)
			return false;

		GitHubAsset? asset = _latestRelease.Assets?.FirstOrDefault(x => x.Name == "osu-np.exe");

		if (asset == null)
			return false;

		Clean();

		await using Stream stream = await Client.GetStreamAsync(asset.BrowserDownloadUrl);
		await using FileStream fileStream = new FileStream(DownloadedExecutablePath, FileMode.Create);

		await stream.CopyToAsync(fileStream);
		await fileStream.FlushAsync();

		fileStream.Close();
		stream.Close();

		string currentExecutablePath = Directory.GetCurrentDirectory() + "\\osu-np.exe";

		File.Move(currentExecutablePath, OldExecutablePath, true);
		File.Move(DownloadedExecutablePath, currentExecutablePath, true);

		return true;
	}

	private static async Task<GitHubRelease?> GetLatestReleaseAsync()
	{
		try
		{
			string contents = await Client.GetStringAsync("https://api.github.com/repos/TheOmyNomy/OsuNowPlaying/releases/latest");
			return JsonConvert.DeserializeObject<GitHubRelease>(contents);
		}
		catch
		{
			return null;
		}
	}
}