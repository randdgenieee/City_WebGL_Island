using GameSparks.Api.Messages;
using GameSparks.Core;
using System;

namespace CIG
{
	public class MessageFriendRequestReceived : ScriptMessage
	{
		public new static Action<MessageFriendRequestReceived> Listener;

		static MessageFriendRequestReceived()
		{
			GSMessage.handlers.Add(".ScriptMessage_FriendRequestReceived", Create);
		}

		public MessageFriendRequestReceived(GSData data)
			: base(data)
		{
		}

		public override void NotifyListeners()
		{
			if (Listener != null)
			{
				Listener(this);
			}
		}

		private static MessageFriendRequestReceived Create(GSData data)
		{
			return new MessageFriendRequestReceived(data);
		}
	}
}
