using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using OsuNowPlaying.Client.Events;
using OsuNowPlaying.Utilities;

namespace OsuNowPlaying.Client;

public class TwitchClient
{
	private const string Hostname = "irc.chat.twitch.tv";
	private const int Port = 6667;

	public event EventHandler<ConnectedEventArgs>? Connected;
	public event EventHandler<AuthenticationSuccessfulEventArgs>? AuthenticationSuccessful;
	public event EventHandler<AuthenticationFailedEventArgs>? AuthenticationFailed;
	public event EventHandler<ChannelJoinedEventArgs>? ChannelJoined;
	public event EventHandler<DisconnectedEventArgs>? Disconnected;

	private TcpClient? _client;
	private StreamReader _reader = null!;
	private StreamWriter _writer = null!;

	private CancellationTokenSource? _cancellationTokenSource;
	private bool _hasJoinedChannel;

	public bool IsConnected => (_client?.Connected ?? false) && (!_cancellationTokenSource?.IsCancellationRequested ?? false);

	private string _channel = null!;

	public async Task ConnectAsync(string username, string token)
	{
		if (IsConnected)
			return;

		_cancellationTokenSource = new CancellationTokenSource();

		_client = new TcpClient();
		await _client.ConnectAsync(Hostname, Port);

		Connected?.Invoke(this, new ConnectedEventArgs());

		_reader = new StreamReader(_client.GetStream());

		_writer = new StreamWriter(_client.GetStream())
		{
			NewLine = "\r\n",
			AutoFlush = true
		};

		_channel = (username.StartsWith('#') ? username : '#' + username).ToLower();

		ListenerAsync().SafeFireAndForget(exception =>
		{
			Logger.Error(exception);
			Disconnected?.Invoke(this, new DisconnectedEventArgs());
		});

		await _writer.WriteLineAsync($"PASS {token}");
		await _writer.WriteLineAsync($"NICK {username.ToLower()}");

		// The Twitch IRC guide (https://dev.twitch.tv/docs/irc/guide) does not
		// send the "USER" command in the example, so we won't send it either.
	}

	public void Disconnect()
	{
		_cancellationTokenSource?.Cancel();
	}

	private async Task ListenerAsync()
	{
		while (IsConnected)
		{
			string? line;

			try
			{
				line = await _reader.ReadLineAsync().WaitAsync(_cancellationTokenSource!.Token);
			}
			catch (Exception)
			{
				if (_cancellationTokenSource!.IsCancellationRequested)
					break;

				throw;
			}

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
				case "001":
					await Process001CommandAsync();
					break;
				case "JOIN":
					await ProcessJoinCommandAsync();
					break;
				case "NOTICE":
					await ProcessNoticeCommandAsync(parameters);
					break;
				case "PING":
					await ProcessPingCommandAsync(parameters);
					break;
				case "002":
				case "003":
				case "004":
				case "353":
				case "366":
				case "372":
				case "375":
				case "376":
					// Ignore commands that don't require any action or response.
					break;
				default:
					Logger.Warning($"Command \"{command}\" wasn't handled.");
					break;
			}
		}

		if (_client!.Connected)
		{
			await _writer.WriteLineAsync("QUIT");
			_client.Close();
		}

		Disconnected?.Invoke(this, new DisconnectedEventArgs());
	}

	private async Task Process001CommandAsync()
	{
		AuthenticationSuccessful?.Invoke(this, new AuthenticationSuccessfulEventArgs());
		await _writer.WriteLineAsync($"JOIN {_channel}");

		Task.Run(async () =>
		{
			DateTime startTime = DateTime.UtcNow;
			TimeSpan waitDuration = TimeSpan.FromSeconds(5);

			while (DateTime.UtcNow - startTime < waitDuration)
			{
				if (_hasJoinedChannel)
					return;

				await Task.Delay(250);
			}

			Disconnect();
		}).SafeFireAndForget();
	}

	private Task ProcessJoinCommandAsync()
	{
		_hasJoinedChannel = true;
		ChannelJoined?.Invoke(this, new ChannelJoinedEventArgs());

		return Task.CompletedTask;
	}

	private Task ProcessNoticeCommandAsync(string[] parameters)
	{
		// string[] receivers = parameters[0].Split(',');
		string message = string.Join(' ', parameters[1..])[1..];

		if (message == "Improperly formatted auth")
		{
			AuthenticationFailed?.Invoke(this, new AuthenticationFailedEventArgs());
			Disconnect();
		}

		return Task.CompletedTask;
	}

	private async Task ProcessPingCommandAsync(string[] parameters)
	{
		await _writer.WriteLineAsync($"PONG {parameters[0]}");
	}
}