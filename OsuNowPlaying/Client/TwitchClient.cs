using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using OsuNowPlaying.Utilities;

namespace OsuNowPlaying.Client;

public class TwitchClient
{
	private const string Hostname = "irc.chat.twitch.tv";
	private const int Port = 6667;

	private TcpClient? _client;
	private StreamReader _reader = null!;
	private StreamWriter _writer = null!;

	private bool _shouldDisconnect;

	public bool Connected => (_client?.Connected ?? false) && !_shouldDisconnect;

	public async Task ConnectAsync(string username, string token)
	{
		if (Connected)
			return;

		_client = new TcpClient();
		await _client.ConnectAsync(Hostname, Port);

		_reader = new StreamReader(_client.GetStream());

		_writer = new StreamWriter(_client.GetStream())
		{
			NewLine = "\r\n",
			AutoFlush = true
		};

		ListenerAsync().SafeFireAndForget(Logger.Error);

		await _writer.WriteLineAsync($"PASS {token}");
		await _writer.WriteLineAsync($"NICK {username.ToLower()}");

		// The Twitch IRC guide (https://dev.twitch.tv/docs/irc/guide) does not
		// send the "USER" command in the example, so we won't send it either.
	}

	public void Disconnect()
	{
		_shouldDisconnect = true;
	}

	private async Task ListenerAsync()
	{
		while (Connected)
		{
			string? line = await _reader.ReadLineAsync();

			if (string.IsNullOrWhiteSpace(line))
				continue;

			Logger.Debug(line);

			string[] parts = line.Split();

			string? prefix = null;

			if (parts[0].StartsWith(':'))
			{
				prefix = parts[0];
				parts = parts[1..];
			}

			string command = parts[0];
			string[] parameters = parts[1..];

			switch (command)
			{
				case "PING":
					await ProcessPingCommandAsync(parameters);
					break;
				default:
					Logger.Warning($"Command \"{command}\" wasn't handled.");
					continue;
			}

			Logger.Information($"Command \"{command}\" was handled.");
		}

		if (_client!.Connected)
			_client.Close();

		_shouldDisconnect = false;
	}

	private async Task ProcessPingCommandAsync(string[] parameters)
	{
		await _writer.WriteLineAsync($"PONG {parameters[0]}");
	}
}