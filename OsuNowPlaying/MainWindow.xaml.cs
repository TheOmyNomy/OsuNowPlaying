using System.Windows;
using AsyncAwaitBestPractices;
using OsuNowPlaying.Client;
using OsuNowPlaying.Client.Events;
using OsuNowPlaying.Config;
using OsuNowPlaying.Utilities;

namespace OsuNowPlaying;

public partial class MainWindow
{
	private readonly Configuration _configuration;
	private readonly TwitchClient _twitchClient;

	public MainWindow(Configuration configuration)
	{
		_configuration = configuration;

		_twitchClient = new TwitchClient();

		_twitchClient.Connected += OnTwitchClientConnected;
		_twitchClient.Authenticated += OnTwitchClientAuthenticated;
		_twitchClient.ChannelJoined += OnTwitchClientChannelJoined;
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
		StatusTextBlock.Text = "Status: Authenticating...";
	}

	private void OnTwitchClientAuthenticated(object? sender, AuthenticatedEventArgs e)
	{
		// We've successfully connected to the IRC server and authenticated
		// with the specified username and token.

		StatusTextBlock.Text = "Status: Joining...";
	}

	private void OnTwitchClientChannelJoined(object? sender, ChannelJoinedEventArgs e)
	{
		_configuration.Save();

		StatusTextBlock.Text = "Status: Online";

		// We can now allow the user to disconnect / logout.
		LoginButton.Content = "Logout";
		LoginButton.IsEnabled = true;
	}

	private void OnTwitchClientDisconnected(object? sender, DisconnectedEventArgs e)
	{
		// We've disconnected from the IRC server.

		// TODO: Display why we were disconnected if it wasn't a normal disconnection.

		// Since this event can be called from a different thread,
		// we must wrap UI calls inside of a Dispatcher.Invoke() action.
		Dispatcher.Invoke(() =>
		{
			StatusTextBlock.Text = "Status: Offline";

			LoginButton.Content = "Login";
			LoginButton.IsEnabled = true;
		});
	}

	private void OnLoginButtonClick(object sender, RoutedEventArgs e)
	{
		if (LoginButton.Content as string == "Logout")
		{
			_twitchClient.DisconnectAsync().GetAwaiter().GetResult();
			return;
		}

		LoginButton.IsEnabled = false;

		string username = UsernameTextBox.Text;
		string token = TokenTextBox.Password;

		// TODO: Implement proper validation.
		// TODO: Add UI feedback for validation.
		if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(token))
		{
			LoginButton.IsEnabled = true;
			return;
		}

		_configuration.SetValue(ConfigurationSetting.Username, username);
		_configuration.SetValue(ConfigurationSetting.Token, token);

		StatusTextBlock.Text = "Status: Connecting...";

		_twitchClient.ConnectAsync(username, token).SafeFireAndForget(exception =>
		{
			Logger.Error(exception);
			LoginButton.IsEnabled = true;
		});
	}

	private void OnExitButtonClick(object sender, RoutedEventArgs e)
	{
		_twitchClient.DisconnectAsync().GetAwaiter().GetResult();
		Application.Current.Shutdown();
	}
}