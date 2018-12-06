using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Klserjht
{
    class TwitchClient
    {
        private TcpClient _client;
        private StreamReader _reader;
        private StreamWriter _writer;

        public readonly string Username, Token, Channel;
        public bool Connected => _client?.Connected ?? false;

        private Thread _thread;

        public event EventHandler<OnMessageReceivedArgs> OnMessageReceived;

        public TwitchClient(string username, string token, string channel)
        {
            Username = username.ToLower();
            Token = token;
            Channel = (channel.StartsWith("#") ? channel : "#" + channel).ToLower();
        }

        public bool Connect()
        {
            _client = new TcpClient("irc.chat.twitch.tv", 6667);
            if (!_client.Connected) return false;

            _reader = new StreamReader(_client.GetStream());
            _writer = new StreamWriter(_client.GetStream());

            _writer.WriteLine($"PASS {Token}");
            _writer.WriteLine($"NICK {Username}");
            _writer.WriteLine("CAP REQ :twitch.tv/tags");
            _writer.WriteLine($"JOIN {Channel}");
            _writer.Flush();

            _thread = new Thread(() =>
            {
                while (true)
                {
                    var line = _reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (line.StartsWith("PING"))
                    {
                        _writer.WriteLine("PONG :tmi.twitch.tv");
                        _writer.Flush();
                    }

                    var tokens = line.Split();
                    var tagsIncluded = tokens[0].StartsWith("@");

                    if (tokens.Length > (tagsIncluded ? 4 : 3) && tokens[tagsIncluded ? 2 : 1] == "PRIVMSG")
                    {
                        var sender = string.Empty;

                        if (tagsIncluded)
                        {
                            var tags = tokens[0].Split(';');

                            foreach (var tag in tags)
                            {
                                var split = tag.Split('=');
                                if (split[0] == "display-name")
                                {
                                    sender = split[1];
                                    break;
                                }
                            }
                        }
                        else sender = tokens[0].Split('!')[0].Substring(1);

                        var message = tokens[tagsIncluded ? 4 : 3].Substring(1);
                        for (var i = tagsIncluded ? 5 : 4; i < tokens.Length; i++) message += ' ' + tokens[i];

                        OnMessageReceived?.Invoke(this, new OnMessageReceivedArgs
                        {
                            Message = message,
                            Sender = sender
                        });
                    }
                }
            });

            _thread.Start();
            return true;
        }

        public bool Disconnect()
        {
            _thread.Abort();

            _writer.Close();
            _reader.Close();
            _client.Close();

            return true;
        }

        public void SendMessage(string message)
        {
            _writer.WriteLine($"PRIVMSG {Channel} :{message}");
            _writer.Flush();
        }

        public class OnMessageReceivedArgs : EventArgs
        {
            public string Sender;
            public string Message;
        }
    }
}
