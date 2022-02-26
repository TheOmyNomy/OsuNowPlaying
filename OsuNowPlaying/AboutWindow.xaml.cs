using OsuNowPlaying.Utilities;

namespace OsuNowPlaying;

public partial class AboutWindow
{
	public AboutWindow()
	{
		InitializeComponent();
		Loaded += (_, _) => VersionTextBlock.Text = $"Version: {VersionManager.Version}";
	}
}