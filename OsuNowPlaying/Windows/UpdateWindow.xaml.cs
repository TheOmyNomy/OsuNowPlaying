using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using OsuNowPlaying.Logging;
using OsuNowPlaying.Updater;

namespace OsuNowPlaying.Windows;

public partial class UpdateWindow
{
	public UpdateWindow()
	{
		InitializeComponent();
		Loaded += OnLoaded;
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		CurrentVersionTextBlock.Text = $"Current: {UpdateManager.CurrentVersion}";
		LatestVersionTextBlock.Text = $"Latest: {UpdateManager.LatestVersion}";
	}

	private void OnUpdateButtonClick(object sender, RoutedEventArgs e)
	{
		UpdateButton.IsEnabled = false;
		CancelButton.IsEnabled = false;
		CloseButton.IsEnabled = false;

		Task.Run(async () =>
		{
			Logger.Debug("Updating...");
			bool result = await UpdateManager.ApplyAsync();

			if (!result)
			{
				Logger.Error("Something went wrong while updating!");
				return;
			}

			Logger.Debug("Update complete! Restarting...");

			Process.Start("osu!np.exe");
			Dispatcher.Invoke(Application.Current.Shutdown);
		});
	}

	private void OnCancelButtonClick(object sender, RoutedEventArgs e)
	{
		Close();
	}
}