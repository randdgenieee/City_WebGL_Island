using GameSparks.Api.Messages;
using GameSparks.Core;
using System;

namespace CIG
{
	public class MessageFriendRequestDeclined : ScriptMessage
	{
		public new static Action<MessageFriendRequestDeclined> Listener;

		static MessageFriendRequestDeclined()
		{
			GSMessage.handlers.Add(".ScriptMessage_FriendRequestDeclined", Create);
		}

		public MessageFriendRequestDeclined(GSData data)
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

		private static MessageFriendRequestDeclined Create(GSData data)
		{
			return new MessageFriendRequestDeclined(data);
		}
	}
}
