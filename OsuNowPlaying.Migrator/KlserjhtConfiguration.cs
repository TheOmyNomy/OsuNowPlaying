using Newtonsoft.Json;

namespace OsuNowPlaying.Migrator;

public class KlserjhtConfiguration
{
	[JsonProperty("username")]
	public string? Username { get; set; }

	[JsonProperty("token")]
	public string? Token { get; set; }

	[JsonProperty("channel")]
	public string? Channel { get; set; }

	[JsonProperty("format")]
	public string? Format { get; set; }

	[JsonProperty("command")]
	public string? Command { get; set; }

	public static KlserjhtConfiguration? Parse(string path)
	{
		if (!File.Exists(path))
			return null;

		string contents = File.ReadAllText(path);
		KlserjhtConfiguration? configuration = JsonConvert.DeserializeObject<KlserjhtConfiguration>(contents);

		return configuration;
	}
}