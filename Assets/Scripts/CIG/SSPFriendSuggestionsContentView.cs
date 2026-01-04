using CIG.Translation;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class SSPFriendSuggestionsContentView : SSPFriendsContentView
	{
		[SerializeField]
		private Button _inviteButton;

		[SerializeField]
		private InputField _inviteCodeInputField;

		private bool _waitingForCallback;

		public override SSPMenuPopup.SSPMenuTab Tab => SSPMenuPopup.SSPMenuTab.FriendSuggestions;

		public override ILocalizedString HeaderText => Localization.Key("find_friends");

		private void Awake()
		{
			UpdateInviteButton();
		}

		protected override void OnDestroy()
		{
			if (_friendsManager != null)
			{
				_friendsManager.FriendSuggestionsChangedEvent -= base.OnFriendsListChanged;
			}
			base.OnDestroy();
		}

		public void OnSendRequestClicked()
		{
			_waitingForCallback = true;
			UpdateInviteButton();
			_friendsManager.SendFriendRequest(_inviteCodeInputField.text, "friend_request_sent_suggestion", OnSendFriendRequestSuccess, OnSendFriendRequestError);
		}

		public void OnInviteCodeInputChanged(string input)
		{
			UpdateInviteButton();
		}

		protected override void Open()
		{
			_friendsManager.FriendSuggestionsChangedEvent += base.OnFriendsListChanged;
			base.Open();
		}

		protected override void Close()
		{
			_friendsManager.FriendSuggestionsChangedEvent -= base.OnFriendsListChanged;
			base.Close();
		}

		protected override void GetFriendsList()
		{
			_friendsManager.GetFriendSuggestions();
		}

		private void UpdateInviteButton()
		{
			_inviteButton.interactable = (!_waitingForCallback && !string.IsNullOrEmpty(_inviteCodeInputField.text));
		}

		private void OnSendFriendRequestSuccess(FriendData friendData)
		{
			OnCallbackReceived();
			GenericPopupRequest request = new GenericPopupRequest("friend_request_sent_success").SetTexts(Localization.Key("sspmenu.friend_invite_sent"), Localization.Format(Localization.Key("friend_request_sent"), Localization.Literal(friendData.DisplayName))).SetGreenOkButton();
			_popupManager.RequestPopup(request);
		}

		private void OnSendFriendRequestError()
		{
			OnCallbackReceived();
			GenericPopupRequest request = new GenericPopupRequest("friend_invite_code_error").SetTexts(Localization.Key("oops_something_went_wrong"), Localization.Key("invite_code_error")).SetGreenOkButton();
			_popupManager.RequestPopup(request);
		}

		private void OnCallbackReceived()
		{
			_waitingForCallback = false;
			UpdateInviteButton();
		}
	}
}
