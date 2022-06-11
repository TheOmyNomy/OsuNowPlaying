using Newtonsoft.Json;

namespace OsuNowPlaying.Migrator.Updater
{
	public class GitHubAsset
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("browser_download_url")]
		public string BrowserDownloadUrl { get; set; }
	}
}