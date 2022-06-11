using System;
using System.IO;

namespace OsuNowPlaying.Migrator
{
	public partial class App
	{
		public static readonly string WorkingPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "osu!np");
	}
}