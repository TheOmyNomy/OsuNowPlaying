using System;
using System.Collections.Generic;
using System.IO;

namespace OsuNowPlaying.Config;

public class Configuration
{
	private readonly string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "osu!np", "settings.ini");

	private readonly Dictionary<ConfigurationSetting, ConfigurationObject> _settings;

	public Configuration()
	{
		_settings = new Dictionary<ConfigurationSetting, ConfigurationObject>
		{
			{
				ConfigurationSetting.Username,
				new ConfigurationObject(string.Empty)
			},
			{
				ConfigurationSetting.Token,
				new ConfigurationObject(string.Empty)
			},
			{
				ConfigurationSetting.Channel,
				new ConfigurationObject(string.Empty)
			},
			{
				ConfigurationSetting.Command,
				new ConfigurationObject("!np")
			},
			{
				ConfigurationSetting.Format,
				new ConfigurationObject("@!sender! !artist! - !title! (!creator!) [!version!] - https://osu.ppy.sh/beatmaps/!id!")
			}
		};

		Load();
	}

	public T GetDefaultValue<T>(ConfigurationSetting setting) => (T) _settings[setting].DefaultValue;
	public T GetValue<T>(ConfigurationSetting setting) => (T) _settings[setting].Value;

	public void SetValue(ConfigurationSetting setting, object value) => _settings[setting].Value = value;

	public bool IsDefaultValue(ConfigurationSetting setting) => _settings[setting].DefaultValue.Equals(_settings[setting].Value);

	public void Load()
	{
		if (!File.Exists(_path))
			return;

		using FileStream stream = File.OpenRead(_path);
		using StreamReader reader = new StreamReader(stream);

		while (!reader.EndOfStream)
		{
			string? line = reader.ReadLine();

			if (string.IsNullOrWhiteSpace(line) || line.StartsWith(';'))
				continue;

			string[] tokens = line.Split('=');
			string key = tokens[0].Trim(), value = string.Join(' ', tokens[1..]).Trim();

			if (Enum.TryParse(key, out ConfigurationSetting setting))
			{
				_settings[setting].Value = value;
			}
		}
	}

	public void Save()
	{
		Directory.CreateDirectory(Path.GetDirectoryName(_path));

		using FileStream stream = File.Open(_path, FileMode.Create);
		using StreamWriter writer = new StreamWriter(stream);

		foreach (var setting in _settings)
			writer.WriteLine($"{setting.Key} = {setting.Value.Value}");

		writer.Flush();
	}

	private class ConfigurationObject
	{
		public object Value { get; set; }

		public readonly object DefaultValue;

		public ConfigurationObject(object defaultValue)
		{
			DefaultValue = defaultValue;
			Value = defaultValue;
		}
	}
}