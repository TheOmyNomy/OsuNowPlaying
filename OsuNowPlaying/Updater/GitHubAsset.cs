using Newtonsoft.Json;

namespace OsuNowPlaying.Updater;

public class GitHubAsset
{
	[JsonProperty("name")]
	public string Name { get; set; } = null!;

	[JsonProperty("browser_download_url")]
	public string BrowserDownloadUrl { get; set; } = null!;
}