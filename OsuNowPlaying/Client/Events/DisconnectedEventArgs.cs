using System;

namespace OsuNowPlaying.Client.Events;

public class DisconnectedEventArgs : EventArgs
{
	public readonly DisconnectReason Reason;

	public DisconnectedEventArgs(DisconnectReason reason)
	{
		Reason = reason;
	}
}

public enum DisconnectReason
{
	Normal,
	ConnectionAborted,
	InvalidAuthenticationToken,
	InvalidChannel
}