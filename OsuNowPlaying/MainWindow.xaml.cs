using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using AsyncAwaitBestPractices;
using OsuNowPlaying.Client;
using OsuNowPlaying.Client.Events;
using OsuNowPlaying.Config;
using OsuNowPlaying.Logging;

namespace OsuNowPlaying;

public partial class MainWindow
{
	private readonly Configuration _configuration;

	private readonly StructuredOsuMemoryReader _osuMemoryReader;
	private readonly TwitchClient _twitchClient;

	private AboutWindow? _aboutWindow;

	public MainWindow(Configuration configuration)
	{
		_configuration = configuration;

		_osuMemoryReader = new StructuredOsuMemoryReader();
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
		UsernameTextBox.Text = _configuration.GetValue<string>(ConfigurationSetting.Username);
		TokenTextBox.Password = _configuration.GetValue<string>(ConfigurationSetting.Token);
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
		_configuration.Save();
		SetState(ConnectionState.Online);
	}

	private void OnTwitchClientMessageReceived(object? sender, MessageReceivedEventArgs e)
	{
		string command = _configuration.GetValue<string>(ConfigurationSetting.Command);
		string firstPart = e.Message.Split()[0].Trim();

		if (!firstPart.Equals(command, StringComparison.OrdinalIgnoreCase))
			return;

		if (!_osuMemoryReader.CanRead || !_osuMemoryReader.TryRead(_osuMemoryReader.OsuMemoryAddresses.GeneralData))
		{
			string username = _configuration.GetValue<string>(ConfigurationSetting.Username);
			_twitchClient.SendMessageAsync($"@{username} osu! isn't running!").GetAwaiter().GetResult();

			return;
		}

		string format = _configuration.GetValue<string>(ConfigurationSetting.Format);

		string response = format
			.Replace("!artist!", _osuMemoryReader.ReadBeatmapArtist(), StringComparison.OrdinalIgnoreCase)
			.Replace("!title!", _osuMemoryReader.ReadBeatmapTitle(), StringComparison.OrdinalIgnoreCase)
			.Replace("!creator!", _osuMemoryReader.ReadBeatmapCreator(), StringComparison.OrdinalIgnoreCase)
			.Replace("!version!", _osuMemoryReader.ReadBeatmapVersion(), StringComparison.OrdinalIgnoreCase)
			.Replace("!sender!", e.Sender, StringComparison.OrdinalIgnoreCase)
			.Replace("!link!", $"https://osu.ppy.sh/beatmaps/{_osuMemoryReader.ReadBeatmapId()}", StringComparison.OrdinalIgnoreCase);

		_twitchClient.SendMessageAsync(response).GetAwaiter().GetResult();
	}

	private void OnTwitchClientDisconnected(object? sender, DisconnectedEventArgs e)
	{
		// TODO: Display why we were disconnected if it wasn't a normal disconnection.
		Dispatcher.Invoke(() => SetState(ConnectionState.Offline));
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

	private void OnHyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
	{
		Process.Start("explorer.exe", e.Uri.AbsoluteUri);
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

		// TODO: Implement proper validation.
		// TODO: Add UI feedback for validation.
		if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(token))
		{
			SetState(ConnectionState.Offline);
			return;
		}

		_configuration.SetValue(ConfigurationSetting.Username, username);
		_configuration.SetValue(ConfigurationSetting.Token, token);

		_twitchClient.ConnectAsync(username, token).SafeFireAndForget(exception =>
		{
			Logger.Error(exception);
			SetState(ConnectionState.Offline);
		});
	}

	private void OnExitButtonClick(object sender, RoutedEventArgs e)
	{
		_twitchClient.DisconnectAsync().GetAwaiter().GetResult();
		Application.Current.Shutdown();
	}

	private void SetState(ConnectionState state)
	{
		switch (state)
		{
			case ConnectionState.Connecting:
				UsernameTextBox.IsEnabled = false;
				TokenTextBox.IsEnabled = false;
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