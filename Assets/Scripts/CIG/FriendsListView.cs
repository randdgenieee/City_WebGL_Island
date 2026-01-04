using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class FriendsListView : MonoBehaviour
	{
		[SerializeField]
		private RecyclerGridLayoutGroup _recyclerGrid;

		private readonly Dictionary<GameObject, FriendItemView> _gameObjectToViewMapping = new Dictionary<GameObject, FriendItemView>();

		private readonly Dictionary<Friend, FriendItemView> _friendToViewMapping = new Dictionary<Friend, FriendItemView>();

		private FriendsManager _friendsManager;

		private IslandsManager _islandsManager;

		private PopupManager _popupManager;

		private GameSparksAuthenticator _authenticator;

		private FriendList _friendList;

		public void Initialize(FriendsManager friendsManager, IslandsManager islandsManager, PopupManager popupManager, GameSparksAuthenticator authenticator, FriendList friendList)
		{
			_friendsManager = friendsManager;
			_islandsManager = islandsManager;
			_popupManager = popupManager;
			_authenticator = authenticator;
			_friendList = friendList;
			_friendList.FriendRemovedEvent += OnFriendRemoved;
			_recyclerGrid.PushInstances();
			_recyclerGrid.Init(_friendList.Friends.Count, InitializeFriendItemView);
			SingletonMonobehaviour<FPSLimiter>.Instance.PushUnlimitedFPSRequest(this);
		}

		public void Deinitialize()
		{
			foreach (KeyValuePair<Friend, FriendItemView> item in _friendToViewMapping)
			{
				item.Value.Deinitialize();
			}
			_friendToViewMapping.Clear();
			_recyclerGrid.PushInstances();
			if (_friendList != null)
			{
				_friendList.FriendRemovedEvent -= OnFriendRemoved;
			}
			if (SingletonMonobehaviour<FPSLimiter>.IsAvailable)
			{
				SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
			}
		}

		private void OnDestroy()
		{
			Deinitialize();
		}

		private bool InitializeFriendItemView(GameObject go, int index)
		{
			if (index < 0 || index >= _friendList.Friends.Count)
			{
				return false;
			}
			FriendItemView friendItemView = GetFriendItemView(go);
			Friend friend = _friendList.Friends[index];
			friendItemView.Initialize(_friendsManager, _islandsManager, _popupManager, _authenticator, friend);
			_friendToViewMapping[friend] = friendItemView;
			return true;
		}

		private FriendItemView GetFriendItemView(GameObject instance)
		{
			if (!_gameObjectToViewMapping.TryGetValue(instance, out FriendItemView value))
			{
				value = (_gameObjectToViewMapping[instance] = instance.GetComponent<FriendItemView>());
			}
			return value;
		}

		private void OnFriendRemoved(Friend friend)
		{
			if (_friendToViewMapping.TryGetValue(friend, out FriendItemView value))
			{
				value.gameObject.SetActive(value: false);
			}
		}
	}
}
