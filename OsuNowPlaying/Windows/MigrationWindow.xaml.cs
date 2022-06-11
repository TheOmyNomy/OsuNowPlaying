using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Navigation;

namespace OsuNowPlaying.Windows;

public partial class MigrationWindow
{
	public MigrationWindow()
	{
		InitializeComponent();
		Loaded += OnLoaded;
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		SizeToContent = SizeToContent.WidthAndHeight;

		string klserjhtPath = Path.Combine(App.WorkingPath, "Klserjht");

		if (Directory.Exists(klserjhtPath))
			Directory.Delete(klserjhtPath, true);

		File.Delete(Path.Combine(App.WorkingPath, "windowsdesktop-runtime-win-x64.exe"));
	}

	private void OnHyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
	{
		Process.Start("explorer.exe", e.Uri.AbsoluteUri);
	}

	private void OnDoneButtonClick(object sender, RoutedEventArgs e)
	{
		Close();
	}
}