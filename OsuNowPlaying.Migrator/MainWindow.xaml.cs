﻿using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using OsuNowPlaying.Migrator.Updater;

namespace OsuNowPlaying.Migrator
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
			Loaded += (sender, args) => SizeToContent = SizeToContent.WidthAndHeight;
		}

		private void OnHelpButtonClick(object sender, RoutedEventArgs e)
		{
			Process.Start("explorer.exe", "https://github.com/TheOmyNomy/OsuNowPlaying/blob/master/README.md");
		}

		private void OnButtonClick(object sender, RoutedEventArgs e)
		{
			switch (Button.Content)
			{
				case "Start":
					Button.IsEnabled = false;
					StatusGrid.Visibility = Visibility.Visible;
					Task.Run(MigrateAsync);
					break;
				case "Launch":
					Process.Start("osu-np.exe", "--migration");
					Application.Current.Shutdown();
					break;
				case "Exit":
					Application.Current.Shutdown();
					break;
			}
		}

		private async Task MigrateAsync()
		{
			const int steps = 6;
			const double incrementAmount = 100.0 / steps;

			if (!DotNetManager.IsInstalled)
			{
				UpdateStatusInformation("Downloading .NET...", incrementAmount * 1);
				await DotNetManager.DownloadAsync();

				UpdateStatusInformation("Installing .NET...", incrementAmount * 2);

				if (!DotNetManager.Install() || !DotNetManager.IsInstalled)
				{
					ShowErrorMessage("Error: Failed to install .NET\r\nPlease click the help button below for instructions on installing manually");
					UpdateHelpButtonState(true, true);
					UpdateButtonState("Exit", true);

					return;
				}
			}

			UpdateStatusInformation("Downloading latest osu!np version...", incrementAmount * 3);

			Directory.CreateDirectory(App.WorkingPath);

			if (!await UpdateManager.ApplyAsync())
			{
				ShowErrorMessage("Error: Failed to install osu!np\r\nPlease click the help button below for instructions on installing manually");
				UpdateHelpButtonState(true, true);
				UpdateButtonState("Exit", true);

				return;
			}

			UpdateStatusInformation("Migrating configuration...", incrementAmount * 4);
			await ConvertKlserjhtConfigurationAsync();

			UpdateStatusInformation("Removing Klserjht files...", incrementAmount * 5);
			RemoveKlserjhtFiles();

			UpdateStatusInformation("Migration complete!", incrementAmount * 6);

			UpdateButtonState("Launch", true);
		}

		private async Task ConvertKlserjhtConfigurationAsync()
		{
			KlserjhtConfiguration configuration = KlserjhtConfiguration.Parse("Klserjht.json");

			if (configuration == null)
				return;

			FileStream stream = File.OpenWrite(Path.Combine(App.WorkingPath, "settings.ini"));
			StreamWriter writer = new StreamWriter(stream);

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

			writer.Close();
			stream.Close();
		}

		private void RemoveKlserjhtFiles()
		{
			string klserjhtPath = Path.Combine(App.WorkingPath, "Klserjht");

			if (Directory.Exists(klserjhtPath))
				Directory.Delete(klserjhtPath, true);

			Directory.CreateDirectory(klserjhtPath);

			string[] files =
			{
				"Klserjht.exe",
				"Klserjht.json",
				"Klserjht.Updater.exe",
				"Newtonsoft.Json.dll",
				"OsuMemoryDataProvider.dll",
				"ProcessMemoryDataFinder.dll",
				"AdonisUI.ClassicTheme.dll",
				"AdonisUI.dll"
			};

			foreach (string file in files)
			{
				if (File.Exists(file))
					File.Move(file, Path.Combine(klserjhtPath, file));
			}
		}

		private void ShowErrorMessage(string text)
		{
			Dispatcher.Invoke(() =>
			{
				StatusGrid.Visibility = Visibility.Collapsed;

				ErrorTextBlock.Text = text;
				ErrorGroupBox.Visibility = Visibility.Visible;
			});
		}

		private void UpdateStatusInformation(string text, double value)
		{
			Dispatcher.Invoke(() =>
			{
				StatusTextBlock.Text = text;
				StatusProgressBar.Value = value;
			});
		}

		private void UpdateHelpButtonState(bool state, bool focus)
		{
			Dispatcher.Invoke(() =>
			{
				HelpButton.Visibility = state ? Visibility.Visible : Visibility.Collapsed;
				if (focus) HelpButton.Focus();
			});
		}

		private void UpdateButtonState(string text, bool state)
		{
			Dispatcher.Invoke(() =>
			{
				Button.Content = text;
				Button.IsEnabled = state;
			});
		}
	}
}