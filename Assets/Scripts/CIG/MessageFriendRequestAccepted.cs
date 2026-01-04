using GameSparks.Api.Messages;
using GameSparks.Core;
using System;

namespace CIG
{
	public class MessageFriendRequestAccepted : ScriptMessage
	{
		public new static Action<MessageFriendRequestAccepted> Listener;

		static MessageFriendRequestAccepted()
		{
			GSMessage.handlers.Add(".ScriptMessage_FriendRequestAccepted", Create);
		}

		public MessageFriendRequestAccepted(GSData data)
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

		private static MessageFriendRequestAccepted Create(GSData data)
		{
			return new MessageFriendRequestAccepted(data);
		}
	}
}
