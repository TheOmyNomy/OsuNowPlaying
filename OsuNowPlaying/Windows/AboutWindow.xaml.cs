using OsuNowPlaying.Updater;

namespace OsuNowPlaying.Windows;

public partial class AboutWindow
{
	public AboutWindow()
	{
		InitializeComponent();
		Loaded += (_, _) => VersionTextBlock.Text = $"Version: {UpdateManager.CurrentVersion}";
	}
}