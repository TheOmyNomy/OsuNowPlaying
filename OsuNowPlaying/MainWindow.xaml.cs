using System.Windows;
using OsuNowPlaying.Config;

namespace OsuNowPlaying;

public partial class MainWindow
{
	private readonly Configuration _configuration;

	public MainWindow(Configuration configuration)
	{
		_configuration = configuration;

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

		// TODO: Connect to the IRC server with the given username and token.

		// TODO: Only save if authentication was successful.
		_configuration.Save();
	}

	private void OnExitButtonClick(object sender, RoutedEventArgs e)
	{
		Application.Current.Shutdown();
	}
}