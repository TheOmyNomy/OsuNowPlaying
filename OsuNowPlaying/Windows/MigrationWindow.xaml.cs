using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace OsuNowPlaying.Windows;

public partial class MigrationWindow
{
	public MigrationWindow()
	{
		InitializeComponent();
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