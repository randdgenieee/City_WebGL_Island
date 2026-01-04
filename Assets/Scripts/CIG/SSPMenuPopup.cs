using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class SSPMenuPopup : Popup
	{
		public enum SSPMenuTab
		{
			NewsLetter,
			OtherGames,
			Faq,
			Login,
			Gifts,
			Friends,
			FriendSuggestions
		}

		private static readonly SSPMenuTab[] TemporarilyDisabledTabs = new SSPMenuTab[1]
		{
			SSPMenuTab.Gifts
		};

		[SerializeField]
		private TabView _tabView;

		[SerializeField]
		private List<SSPMenuContentView> _contentViews;

		[SerializeField]
		private LocalizedText _headerText;

		[SerializeField]
		private GameObject _pendingFriendGiftBadge;

		private SSPMenuTab _lastOpenedTab = SSPMenuTab.Login;

		private FriendsManager _friendsManager;

		public override string AnalyticsScreenName => $"ssp_{_lastOpenedTab.ToString()}";

		public ServerGifts ServerGifts
		{
			get;
			private set;
		}

		public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
		{
			base.Initialize(model, canvasScaler);
			ServerGifts = model.Game.ServerGifts;
			_friendsManager = model.Game.FriendsManager;
			int i = 0;
			for (int count = _contentViews.Count; i < count; i++)
			{
				_contentViews[i].Initialize(this, model);
			}
			int j = 0;
			for (int num = TemporarilyDisabledTabs.Length; j < num; j++)
			{
				_tabView.SetTabVisible((int)TemporarilyDisabledTabs[j], visible: false);
			}
			_tabView.TabIndexChangedEvent += OnTabIndexChanged;
		}

		protected override void OnDestroy()
		{
			if (_tabView != null)
			{
				_tabView.TabIndexChangedEvent -= OnTabIndexChanged;
			}
			int i = 0;
			for (int count = _contentViews.Count; i < count; i++)
			{
				SSPMenuContentView sSPMenuContentView = _contentViews[i];
				if (sSPMenuContentView != null)
				{
					sSPMenuContentView.Deinitialize();
				}
			}
			if (_friendsManager != null)
			{
				_friendsManager.GiftsChangedEvent -= OnGiftsChanged;
				_friendsManager = null;
			}
			base.OnDestroy();
		}

		public override void Open(PopupRequest request)
		{
			base.Open(request);
			SSPMenuTab activeTab = GetRequest<SSPMenuPopupRequest>().TabToOpen ?? _lastOpenedTab;
			_tabView.SetActiveTab((int)activeTab);
			ToggleContent(_lastOpenedTab);
			_friendsManager.GiftsChangedEvent += OnGiftsChanged;
			OnGiftsChanged();
		}

		protected override void Closed()
		{
			_friendsManager.GiftsChangedEvent -= OnGiftsChanged;
			int i = 0;
			for (int count = _contentViews.Count; i < count; i++)
			{
				_contentViews[i].Toggle(active: false);
			}
			base.Closed();
		}

		private void ToggleContent(SSPMenuTab tab)
		{
			_lastOpenedTab = tab;
			int i = 0;
			for (int count = _contentViews.Count; i < count; i++)
			{
				SSPMenuContentView sSPMenuContentView = _contentViews[i];
				bool flag = sSPMenuContentView.Tab == tab;
				sSPMenuContentView.Toggle(flag);
				if (flag)
				{
					_headerText.LocalizedString = sSPMenuContentView.HeaderText;
				}
			}
		}

		private void OnTabIndexChanged(int? oldIndex, int newIndex)
		{
			SSPMenuTab tab = (SSPMenuTab)newIndex;
			ToggleContent(tab);
			Analytics.SSPTabClicked(tab.ToString());
			PushScreenView();
		}

		private void OnGiftsChanged()
		{
			_pendingFriendGiftBadge.SetActive(_friendsManager.HasPendingGift);
		}
	}
}
