using ProcessMemoryDataFinder.Structured;

namespace OsuNowPlaying;

public class StructuredOsuMemoryReader : OsuMemoryDataProvider.StructuredOsuMemoryReader
{
	private readonly CurrentBeatmap _currentBeatmap = new();

	public string? ReadBeatmapArtist()
	{
		return ReadString(_currentBeatmap, nameof(CurrentBeatmap.Artist));
	}

	public string? ReadBeatmapTitle()
	{
		return ReadString(_currentBeatmap, nameof(CurrentBeatmap.Title));
	}

	public string? ReadBeatmapCreator()
	{
		return ReadString(_currentBeatmap, nameof(CurrentBeatmap.Creator));
	}

	public string? ReadBeatmapVersion()
	{
		return ReadString(_currentBeatmap, nameof(CurrentBeatmap.Version));
	}

	public int ReadBeatmapId()
	{
		return ReadInt(_currentBeatmap, nameof(CurrentBeatmap.Id));
	}

	public int ReadBeatmapSetId()
	{
		return ReadInt(_currentBeatmap, nameof(CurrentBeatmap.SetId));
	}

	// The ReadString(), ReadInt(), ReadClassProperty(), and ReadProperty()
	// methods are from the ProcessMemoryDataFinder project (https://github.com/Piotrekol/ProcessMemoryDataFinder).
	// Thanks!

	private string? ReadString(object obj, string propertyName)
	{
		return ReadClassProperty<string>(obj, propertyName);
	}

	private int ReadInt(object obj, string propertyName)
	{
		return ReadProperty<int>(obj, propertyName);
	}

	private T? ReadClassProperty<T>(object obj, string propertyName, T? defaultValue = default) where T : class
	{
		if (TryReadProperty(obj, propertyName, out object result))
			return (T) result;

		return defaultValue;
	}

	private T ReadProperty<T>(object obj, string propertyName, T defaultValue = default) where T : struct
	{
		if (TryReadProperty(obj, propertyName, out object result))
			return (T) result;

		return defaultValue;
	}

	[MemoryAddress("[CurrentBeatmap]")]
	private class CurrentBeatmap : OsuMemoryDataProvider.OsuMemoryModels.Direct.CurrentBeatmap
	{
		// Memory addresses are from the gosumemory! project (https://github.com/l3lackShark/gosumemory).
		// Thanks!

		[MemoryAddress("+0x18")]
		public string? Artist { get; set; }

		[MemoryAddress("+0x24")]
		public string? Title { get; set; }

		[MemoryAddress("+0x7C")]
		public string? Creator { get; set; }

		[MemoryAddress("+0xB0")]
		public string? Version { get; set; }
	}
}