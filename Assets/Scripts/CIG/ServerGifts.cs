using System.Collections.Generic;

namespace CIG
{
	public class ServerGifts
	{
		private const string GiftPrefix = "gift.";

		private readonly StorageDictionary _storage;

		private readonly WebService _webService;

		private readonly PopupManager _popupManager;

		private readonly Properties _properties;

		private long _ownerId;

		private List<string> _consumedGiftIds;

		private Reward _undeliveredReward;

		private Reward _pendingReward;

		private const string OwnerIdKey = "OwnerId";

		private const string UndeliveredKey = "Undelivered";

		private const string ConsumedGiftIdsKey = "ConsumedGifts";

		public ServerGifts(StorageDictionary storage, WebService webService, PopupManager popupManager, Properties properties)
		{
			_storage = storage;
			_webService = webService;
			_popupManager = popupManager;
			_properties = properties;
			_ownerId = _storage.Get("OwnerId", -1L);
			_undeliveredReward = _storage.GetModel("Undelivered", (StorageDictionary sd) => new Reward(sd, _properties), new Reward());
			_consumedGiftIds = _storage.GetList<string>("ConsumedGifts");
			_webService.PullRequestCompletedEvent += OnPullRequestCompleted;
			OnPullRequestCompleted(_webService.Properties);
			if (FirebaseManager.IsAvailable)
			{
				FirebaseManager.MessagingGifts.ReceivedGiftEvent += OnReceivedFirebaseMessagingGift;
				if (FirebaseManager.MessagingGifts.HasUnclaimedGift)
				{
					AddGift(FirebaseManager.MessagingGifts.ClaimGift(_properties));
				}
			}
			ReceiveUndeliveredGifts();
		}

		public void Release()
		{
			FirebaseManager.MessagingGifts.ReceivedGiftEvent -= OnReceivedFirebaseMessagingGift;
			_webService.PullRequestCompletedEvent -= OnPullRequestCompleted;
		}

		public void AddGift(Reward reward)
		{
			_undeliveredReward += reward;
		}

		public void AddGift(Currencies currencies)
		{
			_undeliveredReward.Currencies += currencies;
		}

		public void ReceiveUndeliveredGifts(bool enqueue = true)
		{
			if (!_undeliveredReward.IsEmpty)
			{
				_pendingReward += _undeliveredReward;
				_popupManager.RequestPopup(new ReceiveRewardPopupRequest(new Reward(_undeliveredReward), enqueue, OnCollectedGift));
				_undeliveredReward = new Reward();
			}
		}

		private void OnReceivedFirebaseMessagingGift()
		{
			AddGift(FirebaseManager.MessagingGifts.ClaimGift(_properties));
			ReceiveUndeliveredGifts();
		}

		private void OnPullRequestCompleted(Dictionary<string, string> properties)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> property in properties)
			{
				if (property.Key.StartsWith("gift."))
				{
					dictionary.Add(property.Key, property.Value);
				}
			}
			long userId = _webService.UserId;
			if (_ownerId != userId || userId < 0)
			{
				_ownerId = userId;
			}
			else
			{
				foreach (KeyValuePair<string, string> item in dictionary)
				{
					if (!_consumedGiftIds.Contains(item.Key))
					{
						Reward reward = Reward.ParseFromServer(item.Value, _properties);
						AddGift(reward);
					}
				}
			}
			_consumedGiftIds = new List<string>(dictionary.Keys);
			ReceiveUndeliveredGifts();
		}

		private void OnCollectedGift(Reward reward)
		{
			_pendingReward -= reward;
		}

		public void Serialize()
		{
			_storage.Set("OwnerId", _ownerId);
			_storage.SetOrRemoveStorable("Undelivered", _undeliveredReward + _pendingReward, remove: false);
			_storage.Set("ConsumedGifts", _consumedGiftIds);
		}
	}
}
