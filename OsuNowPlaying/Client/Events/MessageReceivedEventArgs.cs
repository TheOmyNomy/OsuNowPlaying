using System;

namespace OsuNowPlaying.Client.Events;

public class MessageReceivedEventArgs : EventArgs
{
	public readonly string Sender;
	public readonly string Message;

	public MessageReceivedEventArgs(string sender, string message)
	{
		Sender = sender;
		Message = message;
	}
}