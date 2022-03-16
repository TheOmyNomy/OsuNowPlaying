using OsuNowPlaying.Utilities;

namespace OsuNowPlaying.Windows;

public partial class AboutWindow
{
	public AboutWindow()
	{
		InitializeComponent();
		Loaded += (_, _) => VersionTextBlock.Text = $"Version: {VersionManager.Version}";
	}
}