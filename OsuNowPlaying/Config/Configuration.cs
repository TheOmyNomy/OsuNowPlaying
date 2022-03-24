using System;
using System.Collections.Generic;
using System.IO;

namespace OsuNowPlaying.Config;

public class Configuration
{
	private const string Path = "settings.ini";

	private readonly Dictionary<ConfigurationSetting, object> _settings;

	public Configuration()
	{
		_settings = new Dictionary<ConfigurationSetting, object>
		{
			{
				ConfigurationSetting.Username,
				string.Empty
			},
			{
				ConfigurationSetting.Token,
				string.Empty
			},
			{
				ConfigurationSetting.Channel,
				string.Empty
			},
			{
				ConfigurationSetting.Command,
				"!np"
			},
			{
				ConfigurationSetting.Format,
				"@!sender! !artist! - !title! (!creator!) [!version!] - https://osu.ppy.sh/beatmaps/!id!"
			}
		};

		Load();
	}

	public T GetValue<T>(ConfigurationSetting setting) => (T) _settings[setting];
	public void SetValue(ConfigurationSetting setting, object value) => _settings[setting] = value;

	public void Load()
	{
		if (!File.Exists(Path))
			return;

		using FileStream stream = File.OpenRead(Path);
		using StreamReader reader = new StreamReader(stream);

		while (!reader.EndOfStream)
		{
			string? line = reader.ReadLine();

			if (string.IsNullOrWhiteSpace(line) || line.StartsWith(';'))
				continue;

			string[] tokens = line.Split('=');
			string key = tokens[0].Trim(), value = string.Join(' ', tokens[1..]).Trim();

			if (Enum.TryParse(key, out ConfigurationSetting setting))
				_settings[setting] = value;
		}
	}

	public void Save()
	{
		using FileStream stream = File.Open(Path, FileMode.Create);
		using StreamWriter writer = new StreamWriter(stream);

		foreach (var setting in _settings)
			writer.WriteLine($"{setting.Key} = {setting.Value}");

		writer.Flush();
	}
}