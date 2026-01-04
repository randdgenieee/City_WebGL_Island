using UnityEngine;

namespace CIG
{
	public class HUDSparkSocButton : HUDRegionElement
	{
		[SerializeField]
		private GameObject _pendingFriendGiftBadge;

		private PopupManager _popupManager;

		private FriendsManager _friendsManager;

		public void Initialize(PopupManager popupManager, FriendsManager friendsManager)
		{
			_popupManager = popupManager;
			_friendsManager = friendsManager;
			_friendsManager.GiftsChangedEvent += OnGiftsChanged;
			_friendsManager.FriendListChangedEvent += OnFriendsListChanged;
			UpdateBadge();
		}

		private void OnDestroy()
		{
			if (_friendsManager != null)
			{
				_friendsManager.GiftsChangedEvent -= OnGiftsChanged;
				_friendsManager.FriendListChangedEvent -= OnFriendsListChanged;
				_friendsManager = null;
			}
		}

		public void OnClicked()
		{
			_popupManager.RequestPopup(new SSPMenuPopupRequest());
		}

		private void UpdateBadge()
		{
			_pendingFriendGiftBadge.SetActive(_friendsManager.HasPendingGift || _friendsManager.HasFriendRequest);
		}

		private void OnGiftsChanged()
		{
			UpdateBadge();
		}

		private void OnFriendsListChanged(FriendList friendList)
		{
			UpdateBadge();
		}
	}
}
