using CIG.Translation;
using UnityEngine;

namespace CIG
{
	public abstract class SSPFriendsContentView : SSPMenuContentView
	{
		[SerializeField]
		private GameObject _loggedInGroup;

		[SerializeField]
		private GameObject _loginGroup;

		[SerializeField]
		private GameObject _friendListGroup;

		[SerializeField]
		private GameObject _noFriendsGroup;

		[SerializeField]
		private LocalizedText _friendCodeText;

		[SerializeField]
		private FriendsListView _friendsListView;

		[SerializeField]
		private GameObject _scrollRectGroup;

		[SerializeField]
		private LocalizedText _middleText;

		[SerializeField]
		private SocialLoginButton _socialLoginButton;

		protected FriendsManager _friendsManager;

		private IslandsManager _islandsManager;

		private GameSparksAuthenticator _authenticator;

		public override void Initialize(SSPMenuPopup popup, Model model)
		{
			base.Initialize(popup, model);
			_friendsManager = model.Game.FriendsManager;
			_islandsManager = model.Game.IslandsManager;
			_authenticator = model.GameServer.GameSparksServer.Authenticator;
			_socialLoginButton.Initialize(model.Game.PopupManager, model.Device.Settings, model.GameServer.GameSparksServer);
		}

		protected virtual void OnDestroy()
		{
			if (_authenticator != null)
			{
				_authenticator.AuthenticationChangedEvent -= OnAuthenticationChanged;
				_authenticator = null;
			}
			_friendsManager = null;
			_islandsManager = null;
		}

		protected override void Open()
		{
			base.Open();
			_authenticator.AuthenticationChangedEvent += OnAuthenticationChanged;
			UpdateContent();
		}

		protected override void Close()
		{
			_authenticator.AuthenticationChangedEvent -= OnAuthenticationChanged;
			_friendsListView.Deinitialize();
			base.Close();
		}

		protected void OnFriendsListChanged(FriendList friendList)
		{
			if (friendList == null)
			{
				SetText(Localization.Key("SSP_ERROR"));
				return;
			}
			_friendListGroup.SetActive(!friendList.IsEmpty);
			_noFriendsGroup.SetActive(friendList.IsEmpty);
			SetText(null);
			_friendsListView.Initialize(_friendsManager, _islandsManager, _popupManager, _authenticator, friendList);
		}

		protected abstract void GetFriendsList();

		private void UpdateContent()
		{
			if (_authenticator.CurrentAuthentication.IsAuthenticated)
			{
				_loginGroup.SetActive(value: false);
				_loggedInGroup.SetActive(value: true);
				_friendListGroup.SetActive(value: true);
				_noFriendsGroup.SetActive(value: false);
				_friendCodeText.LocalizedString = Localization.Format(Localization.Key("loading_screen.social_your_invite_code"), Localization.Literal(_authenticator.CurrentAuthentication.FriendCode));
				SetText(Localization.Key("loading"));
				GetFriendsList();
			}
			else
			{
				_loginGroup.SetActive(value: true);
				_loggedInGroup.SetActive(value: false);
			}
		}

		private void SetText(ILocalizedString text)
		{
			bool flag = Localization.IsNullOrEmpty(text);
			_scrollRectGroup.SetActive(flag);
			_middleText.gameObject.SetActive(!flag);
			_middleText.LocalizedString = text;
		}

		private void OnAuthenticationChanged(GameSparksAuthentication newAuthentication, GameSparksAuthentication previousAuthentication)
		{
			UpdateContent();
		}
	}
}
