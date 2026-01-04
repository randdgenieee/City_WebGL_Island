using System;
using System.Collections;

namespace CIG
{
	public class Friend : IStorable
	{
		public delegate void DataChangedEventHandler();

		private DateTime _sendGiftCooldownEnd;

		private IEnumerator _cooldownRoutine;

		private bool _canVisit;

		private const string UserIdKey = "UserId";

		private const string DisplayNameKey = "DisplayName";

		private const string FriendCodeKey = "FriendCode";

		private const string ScoreKey = "LeaderboardScore";

		private const string LevelKey = "PlayerLevel";

		private const string StatusKey = "Status";

		private const string CanVisitKey = "CanVisit";

		private const string SendGiftCooldownEndKey = "SendGiftCooldownEnd";

		public string UserId
		{
			get;
			private set;
		}

		public string DisplayName
		{
			get;
			private set;
		}

		public string FriendCode
		{
			get;
			private set;
		}

		public int Level
		{
			get;
			private set;
		}

		public int Score
		{
			get;
			private set;
		}

		public FriendStatusType FriendStatus
		{
			get;
			private set;
		}

		public bool CanInvite
		{
			get
			{
				if (FriendStatus != 0)
				{
					return FriendStatus == FriendStatusType.Suggested;
				}
				return true;
			}
		}

		public bool CanDecline => FriendStatus == FriendStatusType.Received;

		public bool CanReceiveGift
		{
			get;
			set;
		}

		public bool CanSendGift => _sendGiftCooldownEnd <= AntiCheatDateTime.UtcNow;

		public TimeSpan SendGiftCooldown => _sendGiftCooldownEnd - AntiCheatDateTime.UtcNow;

		public event DataChangedEventHandler DataChangedEvent;

		private void FireDataChangedEvent()
		{
			this.DataChangedEvent?.Invoke();
		}

		public Friend(FriendData data)
		{
			UpdateData(data);
		}

		public Friend(StorageDictionary storage)
		{
			UserId = storage.Get("UserId", string.Empty);
			DisplayName = storage.Get("DisplayName", string.Empty);
			FriendCode = storage.Get("FriendCode", string.Empty);
			Level = storage.Get("PlayerLevel", 0);
			Score = storage.Get("LeaderboardScore", 0);
			FriendStatus = (FriendStatusType)storage.Get("Status", -1);
			_canVisit = storage.Get("CanVisit", defaultValue: false);
			_sendGiftCooldownEnd = storage.GetDateTime("SendGiftCooldownEnd", AntiCheatDateTime.UtcNow);
		}

		public void UpdateData(FriendData data)
		{
			UserId = data.UserId;
			DisplayName = data.DisplayName;
			FriendCode = data.FriendCode;
			Level = data.Level;
			Score = data.Score;
			FriendStatus = data.FriendStatus;
			_canVisit = data.CanVisit;
			CanReceiveGift = data.CanReceiveGift;
			SetGiftCooldown(data.RemainingCooldown);
			FireDataChangedEvent();
		}

		public void SetGiftCooldown(double cooldown)
		{
			_sendGiftCooldownEnd = AntiCheatDateTime.UtcNow + TimeSpan.FromMilliseconds(cooldown);
		}

		public bool CanVisit(string currentUserId)
		{
			if (_canVisit)
			{
				return currentUserId != UserId;
			}
			return false;
		}

		public override string ToString()
		{
			return $"{DisplayName}, userId: {UserId}, friendCode: {FriendCode}, level: {Level}, score: {Score}, status: {FriendStatus}, CanVisit: {_canVisit}, CanReceiveGift: {CanReceiveGift}, CanSendGift: {CanSendGift}";
		}

		StorageDictionary IStorable.Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("UserId", UserId);
			storageDictionary.Set("DisplayName", DisplayName);
			storageDictionary.Set("FriendCode", FriendCode);
			storageDictionary.Set("PlayerLevel", Level);
			storageDictionary.Set("LeaderboardScore", Score);
			storageDictionary.Set("Status", (int)FriendStatus);
			storageDictionary.Set("CanVisit", _canVisit);
			storageDictionary.Set("SendGiftCooldownEnd", _sendGiftCooldownEnd);
			return storageDictionary;
		}
	}
}
