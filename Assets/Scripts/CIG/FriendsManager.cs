using SparkLinq;
using System;

namespace CIG
{
	public class FriendsManager
	{
		public delegate void FriendListChangedEventHandler(FriendList friendList);

		public delegate void FriendSuggestionsChangedEventHandler(FriendList friendSuggestions);

		public delegate void GiftsChangedEventHandler();

		private static readonly TimeSpan FriendsListCacheExpirationTime = TimeSpan.FromMinutes(5.0);

		private static readonly TimeSpan FriendSuggestionsCacheExpirationTime = TimeSpan.FromHours(2.0);

		private static readonly TimeSpan FailureExpirationTime = TimeSpan.FromMinutes(1.0);

		private readonly StorageDictionary _storage;

		private readonly GameState _gameState;

		private readonly GameSparksAuthenticator _authenticator;

		private readonly GameSparksFriends _gameSparksFriends;

		private readonly FriendsManagerProperties _properties;

		private FriendList _friendList;

		private DateTime _friendsListExpirationDate;

		private bool _isUpdatingFriendsList;

		private FriendList _friendSuggestions;

		private DateTime _friendSuggestionsExpirationDate;

		private bool _isUpdatingFriendSuggestion;

		private const string FriendsListKey = "FriendsList";

		private const string FriendsListExpirationDateKey = "FriendsListExpirationDate";

		private const string FriendSuggestionsKey = "FriendSuggestions";

		private const string FriendSuggestionsExpirationDateKey = "FriendSuggestionsExpirationDate";

		public int FriendsCount => _friendList.Friends.Count;

		public bool HasPendingGift => _friendList.Friends.Any((Friend f) => f.CanReceiveGift);

		public bool HasFriendRequest => _friendList.Friends.Any((Friend f) => f.FriendStatus == FriendStatusType.Received);

		public event FriendListChangedEventHandler FriendListChangedEvent;

		public event FriendSuggestionsChangedEventHandler FriendSuggestionsChangedEvent;

		public event GiftsChangedEventHandler GiftsChangedEvent;

		private void FireFriendListChangedEvent(FriendList friendList)
		{
			this.FriendListChangedEvent?.Invoke(friendList);
		}

		private void FireFriendSuggestionsChangedEvent(FriendList friendSuggestions)
		{
			this.FriendSuggestionsChangedEvent?.Invoke(friendSuggestions);
		}

		private void FireGiftsChangedEvent()
		{
			this.GiftsChangedEvent?.Invoke();
		}

		public FriendsManager(StorageDictionary storage, GameState gameState, GameSparksFriends gameSparksFriends, GameSparksAuthenticator authenticator, FriendsManagerProperties properties)
		{
			_storage = storage;
			_gameState = gameState;
			_authenticator = authenticator;
			_gameSparksFriends = gameSparksFriends;
			_properties = properties;
			_friendList = _storage.GetModel("FriendsList", (StorageDictionary sd) => new FriendList(sd), new FriendList());
			_friendsListExpirationDate = _storage.GetDateTime("FriendsListExpirationDate", AntiCheatDateTime.UtcNow);
			_friendSuggestions = _storage.GetModel("FriendSuggestions", (StorageDictionary sd) => new FriendList(sd), new FriendList());
			_friendSuggestionsExpirationDate = _storage.GetDateTime("FriendSuggestionsExpirationDate", AntiCheatDateTime.UtcNow);
			_gameSparksFriends.FriendRequestReceivedEvent += OnFriendRequestReceived;
			_gameSparksFriends.FriendRequestAcceptedEvent += OnFriendRequestAccepted;
			_gameSparksFriends.FriendRequestDeclinedEvent += OnFriendRequestDeclined;
			_gameSparksFriends.FriendGiftReceivedEvent += OnFriendGiftReceived;
			_authenticator.AuthenticationChangedEvent += OnAuthenticationChanged;
			if (_authenticator.CurrentAuthentication.IsAuthenticated)
			{
				GetFriendsList();
			}
		}

		public void Release()
		{
			_gameSparksFriends.FriendRequestReceivedEvent -= OnFriendRequestReceived;
			_gameSparksFriends.FriendRequestAcceptedEvent -= OnFriendRequestAccepted;
			_gameSparksFriends.FriendRequestDeclinedEvent -= OnFriendRequestDeclined;
			_gameSparksFriends.FriendGiftReceivedEvent -= OnFriendGiftReceived;
			_authenticator.AuthenticationChangedEvent -= OnAuthenticationChanged;
		}

		public void GetFriendsList()
		{
			if (!_isUpdatingFriendsList)
			{
				if (AntiCheatDateTime.UtcNow < _friendsListExpirationDate)
				{
					FireFriendListChangedEvent(_friendList);
					return;
				}
				_isUpdatingFriendsList = true;
				_gameSparksFriends.GetFriendsList(delegate(FriendList friendsList)
				{
					_isUpdatingFriendsList = false;
					_friendList = friendsList;
					_friendsListExpirationDate = AntiCheatDateTime.UtcNow + FriendsListCacheExpirationTime;
					FireFriendListChangedEvent(_friendList);
					FireGiftsChangedEvent();
				}, delegate
				{
					_isUpdatingFriendsList = false;
					_friendsListExpirationDate = AntiCheatDateTime.UtcNow + FailureExpirationTime;
					FireFriendListChangedEvent(null);
					FireGiftsChangedEvent();
				});
			}
		}

		public void GetFriendSuggestions()
		{
			if (!_isUpdatingFriendSuggestion)
			{
				if (AntiCheatDateTime.UtcNow < _friendSuggestionsExpirationDate)
				{
					FireFriendSuggestionsChangedEvent(_friendSuggestions);
					return;
				}
				_isUpdatingFriendSuggestion = true;
				_gameSparksFriends.GetFriendSuggestions(3, _gameState.Level, delegate(FriendList friendSuggestions)
				{
					_isUpdatingFriendSuggestion = false;
					_friendSuggestions = friendSuggestions;
					_friendSuggestionsExpirationDate = AntiCheatDateTime.UtcNow + FriendSuggestionsCacheExpirationTime;
					FireFriendSuggestionsChangedEvent(_friendSuggestions);
				}, delegate
				{
					_isUpdatingFriendSuggestion = false;
					_friendSuggestionsExpirationDate = AntiCheatDateTime.UtcNow + FailureExpirationTime;
					FireFriendSuggestionsChangedEvent(null);
				});
			}
		}

		public void SendFriendRequest(string friendCode, string analyticsEventName, Action<FriendData> onSuccess, Action onError)
		{
			_gameSparksFriends.SendFriendRequest(friendCode, delegate(FriendData friendData)
			{
				_friendList.UpdateFriendEntry(friendData, add: true);
				_friendSuggestions.UpdateFriendEntry(friendData, add: false);
				Analytics.LogEvent(analyticsEventName);
				EventTools.Fire(onSuccess, friendData);
			}, delegate
			{
				EventTools.Fire(onError);
			});
		}

		public void DeclineFriendRequest(string friendCode, Action callback)
		{
			_gameSparksFriends.DeclineFriendRequest(friendCode, delegate
			{
				_friendList.RemoveFriendEntry(friendCode);
				_friendSuggestions.RemoveFriendEntry(friendCode);
				Analytics.LogEvent("friend_request_declined");
				EventTools.Fire(callback);
			}, delegate
			{
				EventTools.Fire(callback);
			});
		}

		public void SendGift(Friend friend, Action callback)
		{
			_gameSparksFriends.SendGift(friend.UserId, _properties.GiftCurrencies, delegate(double cooldown)
			{
				friend.SetGiftCooldown(cooldown);
				Analytics.FriendGiftSent(_properties.GiftCurrencies);
				EventTools.Fire(callback);
			}, delegate
			{
				EventTools.Fire(callback);
			});
		}

		public void ReceiveGift(Friend friend, Action callback)
		{
			_gameSparksFriends.ReceiveGift(friend.UserId, delegate(Currencies gift)
			{
				friend.CanReceiveGift = false;
				_gameState.EarnCurrencies(gift, CurrenciesEarnedReason.FriendGift, new FlyingCurrenciesData());
				Analytics.FriendGiftReceived(gift);
				FireGiftsChangedEvent();
				EventTools.Fire(callback);
			}, delegate
			{
				EventTools.Fire(callback);
			});
		}

		public bool HasFriend(string userId)
		{
			return _friendList.HasFriend(userId);
		}

		private void OnFriendRequestReceived(FriendData friendData)
		{
			_friendList.UpdateFriendEntry(friendData, add: true);
		}

		private void OnFriendRequestAccepted(FriendData friendData)
		{
			_friendList.UpdateFriendEntry(friendData, add: true);
		}

		private void OnFriendRequestDeclined(string friendCode)
		{
			_friendList.RemoveFriendEntry(friendCode);
		}

		private void OnFriendGiftReceived(FriendData friendData)
		{
			_friendList.UpdateFriendEntry(friendData, add: false);
			_friendSuggestions.UpdateFriendEntry(friendData, add: false);
			FireGiftsChangedEvent();
		}

		private void OnAuthenticationChanged(GameSparksAuthentication newAuthentication, GameSparksAuthentication previousAuthentication)
		{
			_friendList.Friends.Clear();
			_friendSuggestions.Friends.Clear();
			_friendsListExpirationDate = DateTime.MinValue;
			_friendSuggestionsExpirationDate = DateTime.MinValue;
			FireGiftsChangedEvent();
		}

		public void Serialize()
		{
			_storage.Set("FriendsList", _friendList);
			_storage.Set("FriendsListExpirationDate", _friendsListExpirationDate);
			_storage.Set("FriendSuggestions", _friendSuggestions);
			_storage.Set("FriendSuggestionsExpirationDate", _friendSuggestionsExpirationDate);
		}
	}
}
