using System.Windows;
using OsuNowPlaying.Updater;

namespace OsuNowPlaying.Windows;

public partial class AboutWindow
{
	public AboutWindow()
	{
		InitializeComponent();
		Loaded += OnLoaded;
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		SizeToContent = SizeToContent.WidthAndHeight;
		VersionTextBlock.Text = $"Version: {UpdateManager.CurrentVersion}";
	}
}