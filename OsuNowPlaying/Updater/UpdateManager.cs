using System;
using System.IO;
using System.IO.Compression;
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

	private const string WorkingPath = "Updater\\";

	private static GitHubRelease? _latestRelease;

	public static void Clean()
	{
		if (Directory.Exists(WorkingPath))
			Directory.Delete(WorkingPath, true);
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

		GitHubAsset? asset = _latestRelease.Assets?.FirstOrDefault(x => x.Name == "osu!np.zip");

		if (asset == null)
			return false;

		Clean();

		DirectoryInfo workingDirectoryInfo = Directory.CreateDirectory(WorkingPath);
		workingDirectoryInfo.Attributes |= FileAttributes.Hidden;

		string zipPath = $"{WorkingPath}{asset.Name}";
		string extractPath = $"{zipPath.Remove(zipPath.LastIndexOf('.'))}\\";

		await using Stream stream = await Client.GetStreamAsync(asset.BrowserDownloadUrl);
		await using FileStream fileStream = new FileStream(zipPath, FileMode.Create);

		await stream.CopyToAsync(fileStream);
		await fileStream.FlushAsync();

		fileStream.Close();
		stream.Close();

		ZipFile.ExtractToDirectory(zipPath, extractPath);

		foreach (string file in Directory.EnumerateFiles(extractPath))
		{
			string existingFile = Path.GetFileName(file);

			try
			{
				File.Delete(existingFile);
			}
			catch
			{
				File.Move(existingFile, $"{WorkingPath}{existingFile}");
			}

			File.Move(file, existingFile);
		}

		return true;
	}

	private static async Task<GitHubRelease?> GetLatestReleaseAsync()
	{
		string contents = await Client.GetStringAsync("https://api.github.com/repos/TheOmyNomy/OsuNowPlaying/releases/latest");
		return JsonConvert.DeserializeObject<GitHubRelease>(contents);
	}
}