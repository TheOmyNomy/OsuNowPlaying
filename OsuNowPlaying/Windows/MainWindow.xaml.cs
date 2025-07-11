﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using AsyncAwaitBestPractices;
using OsuNowPlaying.Client;
using OsuNowPlaying.Client.Events;
using OsuNowPlaying.Config;
using OsuNowPlaying.Logging;
using OsuNowPlaying.Updater;
using ProcessMemoryDataFinder;

namespace OsuNowPlaying.Windows;

public partial class MainWindow
{
	private readonly Configuration _configuration;

	private readonly StructuredOsuMemoryReader _osuMemoryReader;
	private readonly TwitchClient _twitchClient;

	private AboutWindow? _aboutWindow;

	public MainWindow(Configuration configuration)
	{
		_configuration = configuration;

		_osuMemoryReader = new StructuredOsuMemoryReader(new ProcessTargetOptions("osu!"));
		_twitchClient = new TwitchClient();

		_twitchClient.Connected += OnTwitchClientConnected;
		_twitchClient.Authenticated += OnTwitchClientAuthenticated;
		_twitchClient.ChannelJoined += OnTwitchClientChannelJoined;
		_twitchClient.MessageReceived += OnTwitchClientMessageReceived;
		_twitchClient.Disconnected += OnTwitchClientDisconnected;

		InitializeComponent();
		Loaded += OnLoaded;
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		SizeToContent = SizeToContent.WidthAndHeight;

		UsernameTextBox.Text = _configuration.GetValue<string>(ConfigurationSetting.Username);
		TokenTextBox.Password = _configuration.GetValue<string>(ConfigurationSetting.Token);

		ChannelTextBox.Text = _configuration.GetValue<string>(ConfigurationSetting.Channel);

		if (!_configuration.IsDefaultValue(ConfigurationSetting.Command))
			CommandTextBox.Text = _configuration.GetValue<string>(ConfigurationSetting.Command);

		if (!_configuration.IsDefaultValue(ConfigurationSetting.Format))
			FormatTextBox.Text = _configuration.GetValue<string>(ConfigurationSetting.Format);

		string defaultCommand = _configuration.GetDefaultValue<string>(ConfigurationSetting.Command);
		AdonisUI.Extensions.WatermarkExtension.SetWatermark(CommandTextBox, defaultCommand);

		string defaultFormat = _configuration.GetDefaultValue<string>(ConfigurationSetting.Format);
		AdonisUI.Extensions.WatermarkExtension.SetWatermark(FormatTextBox, defaultFormat);

		if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
			UsernameTextBox.Focus();
		else if (string.IsNullOrWhiteSpace(TokenTextBox.Password))
			TokenTextBox.Focus();
		else
			LoginButton.Focus();

		Task.Run(async () =>
		{
			UpdateManager.Clean();

			bool result = await UpdateManager.CheckAsync();
			Logger.Debug($"Current: {UpdateManager.CurrentVersion}, Latest: {UpdateManager.LatestVersion}");

			if (!result)
			{
				Logger.Debug("No updates are available!");
				return;
			}

			Logger.Debug("An update is available!");

			Dispatcher.Invoke(() =>
			{
				UpdateBannerMessageTextBlock.Text = $"Update {UpdateManager.LatestVersion} is available!";
				UpdateBannerGroupBox.Visibility = Visibility.Visible;
			});
		});
	}

	private void OnTwitchClientConnected(object? sender, ConnectedEventArgs e)
	{
		SetState(ConnectionState.Authenticating);
	}

	private void OnTwitchClientAuthenticated(object? sender, AuthenticatedEventArgs e)
	{
		SetState(ConnectionState.Joining);
	}

	private void OnTwitchClientChannelJoined(object? sender, ChannelJoinedEventArgs e)
	{
		_configuration.SetValue(ConfigurationSetting.Username, UsernameTextBox.Text);
		_configuration.SetValue(ConfigurationSetting.Token, TokenTextBox.Password);

		_configuration.Save();

		SetState(ConnectionState.Online);
		ErrorBannerGroupBox.Visibility = Visibility.Collapsed;
	}

	private void OnTwitchClientMessageReceived(object? sender, MessageReceivedEventArgs e)
	{
		string command = _configuration.GetValue<string>(ConfigurationSetting.Command);

		if (string.IsNullOrWhiteSpace(command))
			command = _configuration.GetDefaultValue<string>(ConfigurationSetting.Command);

		string firstPart = e.Message.Split()[0].Trim();

		if (!firstPart.Equals(command, StringComparison.OrdinalIgnoreCase))
			return;

		if (!_osuMemoryReader.CanRead || !_osuMemoryReader.TryRead(_osuMemoryReader.OsuMemoryAddresses.GeneralData))
		{
			string channel = _configuration.GetValue<string>(ConfigurationSetting.Channel);

			if (string.IsNullOrWhiteSpace(channel))
				channel = _configuration.GetValue<string>(ConfigurationSetting.Username);

			_twitchClient.SendMessageAsync($"@{channel} osu! isn't running!").GetAwaiter().GetResult();

			return;
		}

		string format = _configuration.GetValue<string>(ConfigurationSetting.Format);

		if (string.IsNullOrWhiteSpace(format))
			format = _configuration.GetDefaultValue<string>(ConfigurationSetting.Format);

		string response = format
			.Replace("!artist!", _osuMemoryReader.ReadBeatmapArtist(), StringComparison.OrdinalIgnoreCase)
			.Replace("!title!", _osuMemoryReader.ReadBeatmapTitle(), StringComparison.OrdinalIgnoreCase)
			.Replace("!creator!", _osuMemoryReader.ReadBeatmapCreator(), StringComparison.OrdinalIgnoreCase)
			.Replace("!version!", _osuMemoryReader.ReadBeatmapVersion(), StringComparison.OrdinalIgnoreCase)
			.Replace("!sender!", e.Sender, StringComparison.OrdinalIgnoreCase)
			.Replace("!id!", "" + _osuMemoryReader.ReadBeatmapId(), StringComparison.OrdinalIgnoreCase)
			.Replace("!set-id!", "" + _osuMemoryReader.ReadBeatmapSetId(), StringComparison.OrdinalIgnoreCase);

		_twitchClient.SendMessageAsync(response).GetAwaiter().GetResult();
	}

	private void OnTwitchClientDisconnected(object? sender, DisconnectedEventArgs e)
	{
		Dispatcher.Invoke(() =>
		{
			SetState(ConnectionState.Offline);

			if (e.Reason == DisconnectReason.Normal)
				return;

			switch (e.Reason)
			{
				case DisconnectReason.ConnectionAborted:
					ErrorBannerMessageTextBlock.Text = "Error: Connection lost";
					break;
				case DisconnectReason.InvalidAuthenticationToken:
				case DisconnectReason.LoginAuthenticationFailed:
					ErrorBannerMessageTextBlock.Text = "Error: Invalid authentication token";
					break;
				case DisconnectReason.InvalidChannel:
					ErrorBannerMessageTextBlock.Text = "Error: Invalid channel";
					break;
				default:
					ErrorBannerMessageTextBlock.Text = "Error: Unknown error";
					break;
			}

			ErrorBannerGroupBox.Visibility = Visibility.Visible;
		});
	}

	private void OnAboutButtonClick(object sender, RoutedEventArgs e)
	{
		if (_aboutWindow == null)
		{
			_aboutWindow = new AboutWindow
			{
				Owner = this
			};

			_aboutWindow.Closing += (_, _) => _aboutWindow = null;
		}

		if (_aboutWindow.IsVisible)
			_aboutWindow.Focus();
		else
			_aboutWindow.Show();
	}

	private void OnUpdateBannerUpdateButtonClick(object sender, RoutedEventArgs e)
	{
		Logger.Debug("Updating...");

		UpdateBannerMessageTextBlock.Text = "Updating...";
		UpdateBannerButtonDockPanel.Visibility = Visibility.Hidden;

		Task.Run(async () =>
		{
			bool result = await UpdateManager.ApplyAsync();

			if (!result)
			{
				Logger.Error("Failed to automatically update!");
				Logger.Debug("Opening latest release in browser...");

				Dispatcher.Invoke(() => UpdateBannerMessageTextBlock.Text = "Opening latest release in browser...");

				Process.Start("explorer.exe", $"https://github.com/TheOmyNomy/OsuNowPlaying/releases/{UpdateManager.LatestVersion}");
				return;
			}

			Logger.Debug("Update complete! Restarting...");

			Process.Start(App.CurrentExecutablePath);
			Dispatcher.Invoke(Application.Current.Shutdown);
		});
	}

	private void OnUpdateBannerIgnoreButtonClick(object sender, RoutedEventArgs e)
	{
		UpdateBannerGroupBox.Visibility = Visibility.Collapsed;
	}

	private void OnErrorBannerCloseButtonClick(object sender, RoutedEventArgs e)
	{
		ErrorBannerGroupBox.Visibility = Visibility.Collapsed;
	}

	private void OnTokenHyperlinkClick(object sender, RoutedEventArgs e)
	{
		string tokenUrl = _configuration.GetValue<string>(ConfigurationSetting.TokenUrl);
		Process.Start("explorer.exe", tokenUrl);
	}

	private void OnAdvancedHyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
	{
		Process.Start("explorer.exe", e.Uri.AbsoluteUri);
	}

	private void OnUsernameTextBoxTextChanged(object sender, TextChangedEventArgs e)
	{
		AdonisUI.Extensions.WatermarkExtension.SetWatermark(ChannelTextBox, UsernameTextBox.Text);
	}

	private void OnLoginButtonClick(object sender, RoutedEventArgs e)
	{
		if (LoginButton.Content as string == "Logout")
		{
			_twitchClient.DisconnectAsync().GetAwaiter().GetResult();
			return;
		}

		SetState(ConnectionState.Connecting);

		string username = UsernameTextBox.Text;
		string token = TokenTextBox.Password;

		if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(token))
		{
			SetState(ConnectionState.Offline);
			return;
		}

		string channel = ChannelTextBox.Text;

		_configuration.SetValue(ConfigurationSetting.Channel, channel);
		_configuration.SetValue(ConfigurationSetting.Command, CommandTextBox.Text);
		_configuration.SetValue(ConfigurationSetting.Format, FormatTextBox.Text);

		_configuration.Save();

		_twitchClient.ConnectAsync(username, token, channel).SafeFireAndForget(exception =>
		{
			Logger.Error(exception);
			Dispatcher.Invoke(() => SetState(ConnectionState.Offline));
		});
	}

	private void OnExitButtonClick(object sender, RoutedEventArgs e)
	{
		_twitchClient.DisconnectAsync().GetAwaiter().GetResult();
		Application.Current.Shutdown();
	}

	private void OnAdvancedCheckBoxChecked(object sender, RoutedEventArgs e)
	{
		if (sender is CheckBox checkBox)
			AdvancedGroupBox.Visibility = checkBox.IsChecked.GetValueOrDefault() ? Visibility.Visible : Visibility.Collapsed;
	}

	private void SetState(ConnectionState state)
	{
		switch (state)
		{
			case ConnectionState.Connecting:
				UsernameTextBox.IsEnabled = false;
				TokenTextBox.IsEnabled = false;
				ChannelTextBox.IsEnabled = false;
				CommandTextBox.IsEnabled = false;
				FormatTextBox.IsEnabled = false;

				LoginButton.IsEnabled = false;

				StatusTextBlock.Text = "Status: Connecting...";
				break;
			case ConnectionState.Authenticating:
				StatusTextBlock.Text = "Status: Authenticating...";
				break;
			case ConnectionState.Joining:
				StatusTextBlock.Text = "Status: Joining...";
				break;
			case ConnectionState.Online:
				LoginButton.Content = "Logout";
				LoginButton.IsEnabled = true;

				StatusTextBlock.Text = "Status: Online";
				break;
			default:
				UsernameTextBox.IsEnabled = true;
				TokenTextBox.IsEnabled = true;
				ChannelTextBox.IsEnabled = true;
				CommandTextBox.IsEnabled = true;
				FormatTextBox.IsEnabled = true;

				LoginButton.Content = "Login";
				LoginButton.IsEnabled = true;

				StatusTextBlock.Text = "Status: Offline";
				break;
		}
	}

	private enum ConnectionState
	{
		Offline,
		Connecting,
		Authenticating,
		Joining,
		Online
	}
}