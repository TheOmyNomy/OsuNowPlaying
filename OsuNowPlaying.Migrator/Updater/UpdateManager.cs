using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OsuNowPlaying.Migrator.Updater
{
	public static class UpdateManager
	{
		private static HttpClient _client;

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

		private static readonly string DownloadedExecutablePath = Path.Combine(App.WorkingPath, "osu-np.exe");
		private static readonly string NewExecutablePath = Path.Combine(Directory.GetCurrentDirectory(), "osu-np.exe");

		private static void Clean()
		{
			File.Delete(DownloadedExecutablePath);
			File.Delete(NewExecutablePath);
		}

		public static async Task<bool> ApplyAsync()
		{
			GitHubRelease latestRelease = await GetLatestReleaseAsync();

			if (latestRelease == null)
				return false;

			GitHubAsset asset = latestRelease.Assets?.FirstOrDefault(x => x.Name == "osu-np.exe");

			if (asset == null)
				return false;

			Clean();

			Stream stream = await Client.GetStreamAsync(asset.BrowserDownloadUrl);
			FileStream fileStream = new FileStream(DownloadedExecutablePath, FileMode.Create);

			await stream.CopyToAsync(fileStream);
			await fileStream.FlushAsync();

			fileStream.Close();
			stream.Close();

			File.Move(DownloadedExecutablePath, NewExecutablePath);

			return true;
		}

		private static async Task<GitHubRelease> GetLatestReleaseAsync()
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
}