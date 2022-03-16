using Newtonsoft.Json;

namespace OsuNowPlaying.Updater;

public class GitHubRelease
{
	[JsonProperty("tag_name")]
	public string? TagName { get; set; }

	[JsonProperty("assets")]
	public GitHubAsset[]? Assets { get; set; }
}