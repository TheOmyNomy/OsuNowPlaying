using System.Windows;
using AsyncAwaitBestPractices;
using OsuNowPlaying.Client;
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

		InitializeComponent();
		Loaded += OnLoaded;
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		UsernameTextBox.Text = _configuration.GetValue<string>(ConfigurationSetting.Username);
		TokenTextBox.Password = _configuration.GetValue<string>(ConfigurationSetting.Token);
	}

	private void OnLoginButtonClick(object sender, RoutedEventArgs e)
	{
		string username = UsernameTextBox.Text;
		string token = TokenTextBox.Password;

		// TODO: Implement proper validation.
		// TODO: Add UI feedback for validation.
		if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(token))
			return;

		_configuration.SetValue(ConfigurationSetting.Username, username);
		_configuration.SetValue(ConfigurationSetting.Token, token);

		_twitchClient.ConnectAsync(username, token).SafeFireAndForget(Logger.Error);

		// TODO: Only save if authentication was successful.
		_configuration.Save();
	}

	private void OnExitButtonClick(object sender, RoutedEventArgs e)
	{
		if (_twitchClient.Connected)
			_twitchClient.Disconnect();

		Application.Current.Shutdown();
	}
}