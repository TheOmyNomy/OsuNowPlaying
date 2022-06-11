using Newtonsoft.Json;

namespace OsuNowPlaying.Migrator.Updater
{
	public class GitHubRelease
	{
		[JsonProperty("tag_name")]
		public string TagName { get; set; }

		[JsonProperty("assets")]
		public GitHubAsset[] Assets { get; set; }
	}
}