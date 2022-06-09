using System.Diagnostics;
using OsuNowPlaying.Migrator.Updater;

namespace OsuNowPlaying.Migrator;

internal static class Program
{
	public static readonly string WorkingPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "osu!np");

	private static async Task Main()
	{
		Directory.CreateDirectory(WorkingPath);

		bool result = await UpdateManager.ApplyAsync();

		if (!result)
		{
			Process.Start("explorer.exe", "https://github.com/TheOmyNomy/OsuNowPlaying/releases/latest");
			return;
		}

		await ConvertKlserjhtConfigurationAsync("Klserjht.json", Path.Combine(WorkingPath, "settings.ini"));

		File.Move("Klserjht.exe", Path.Combine(WorkingPath, "Klserjht.exe"), true);

		File.Delete("Klserjht.json");
		File.Delete("Klserjht.Updater.exe");
		File.Delete("Newtonsoft.Json.dll");
		File.Delete("OsuMemoryDataProvider.dll");
		File.Delete("ProcessMemoryDataFinder.dll");

		Process.Start("osu-np.exe", "--migration");
	}

	private static async Task ConvertKlserjhtConfigurationAsync(string inputPath, string outputPath)
	{
		KlserjhtConfiguration? configuration = KlserjhtConfiguration.Parse(inputPath);

		if (configuration == null)
			return;

		await using FileStream stream = File.OpenWrite(outputPath);
		await using StreamWriter writer = new StreamWriter(stream);

		if (!string.IsNullOrWhiteSpace(configuration.Username))
			await writer.WriteLineAsync($"Username = {configuration.Username}");

		if (!string.IsNullOrWhiteSpace(configuration.Token))
			await writer.WriteLineAsync($"Token = {configuration.Token}");

		if (!string.IsNullOrWhiteSpace(configuration.Channel))
			await writer.WriteLineAsync($"Channel = {configuration.Channel}");

		if (!string.IsNullOrWhiteSpace(configuration.Command))
			await writer.WriteLineAsync($"Command = {configuration.Command}");

		if (!string.IsNullOrWhiteSpace(configuration.Format))
		{
			string format = configuration.Format.Replace("!link!", "https://osu.ppy.sh/beatmaps/!id!");
			await writer.WriteLineAsync($"Format = {format}");
		}
	}
}