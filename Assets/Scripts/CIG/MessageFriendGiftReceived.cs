using GameSparks.Api.Messages;
using GameSparks.Core;
using System;

namespace CIG
{
	public class MessageFriendGiftReceived : ScriptMessage
	{
		public new static Action<MessageFriendGiftReceived> Listener;

		static MessageFriendGiftReceived()
		{
			GSMessage.handlers.Add(".ScriptMessage_FriendGiftReceived", Create);
		}

		public MessageFriendGiftReceived(GSData data)
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

		private static MessageFriendGiftReceived Create(GSData data)
		{
			return new MessageFriendGiftReceived(data);
		}
	}
}
