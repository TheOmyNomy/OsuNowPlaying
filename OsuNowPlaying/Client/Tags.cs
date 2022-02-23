namespace OsuNowPlaying.Client;

public class Tags
{
	public string? DisplayName { get; private set; }

	public Tags(string raw)
	{
		Parse(raw);
	}

	private void Parse(string raw)
	{
		string[] tags = raw.Split(';');

		foreach (string tag in tags)
		{
			string[] tokens = tag.Split('=');
			string key = tokens[0].Trim(), value = string.Join('=', tokens[1..]);

			if (key == "display-name")
				DisplayName = value;
		}
	}
}