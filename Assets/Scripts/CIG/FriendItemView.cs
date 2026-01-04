using CIG.Translation;
using System.Collections;
using Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class FriendItemView : MonoBehaviour
	{
		[SerializeField]
		private LocalizedText _levelText;

		[SerializeField]
		private LocalizedText _leaderboardScoreText;

		[SerializeField]
		private LocalizedText _displayNameText;

		[SerializeField]
		private Button _frameButton;

		[SerializeField]
		private Button _visitButton;

		[SerializeField]
		private Button _requestButton;

		[SerializeField]
		private LocalizedText _requestButtonText;

		[SerializeField]
		private Button _giftButton;

		[SerializeField]
		private LocalizedText _giftButtonText;

		[SerializeField]
		private GameObject _giftReceiveBanner;

		[SerializeField]
		private Tweener _giftButtonTweener;

		[SerializeField]
		private Button _acceptButton;

		[SerializeField]
		private Button _declineButton;

		private FriendsManager _friendsManager;

		private IslandsManager _islandsManager;

		private PopupManager _popupManager;

		private GameSparksAuthenticator _authenticator;

		private bool _waitingForCallback;

		private IEnumerator _timerRoutine;

		public Friend Friend
		{
			get;
			private set;
		}

		public void Initialize(FriendsManager friendsManager, IslandsManager islandsManager, PopupManager popupManager, GameSparksAuthenticator authenticator, Friend friend)
		{
			Deinitialize();
			_friendsManager = friendsManager;
			_islandsManager = islandsManager;
			_popupManager = popupManager;
			_authenticator = authenticator;
			Friend = friend;
			Friend.DataChangedEvent += OnDataChanged;
			UpdateLook();
		}

		public void Deinitialize()
		{
			if (Friend != null)
			{
				Friend.DataChangedEvent -= OnDataChanged;
				Friend = null;
			}
			_friendsManager = null;
			_islandsManager = null;
		}

		private void OnDestroy()
		{
			Deinitialize();
		}

		private void OnEnable()
		{
			if (Friend != null)
			{
				UpdateTimerRoutine();
			}
		}

		private void OnDisable()
		{
			if (_timerRoutine != null)
			{
				StopCoroutine(_timerRoutine);
				_timerRoutine = null;
			}
		}

		public void OnFrameClicked()
		{
			if (Friend.CanReceiveGift)
			{
				ReceiveGift();
			}
		}

		public void OnSendRequestClicked()
		{
			SendFriendRequest("friend_request_sent_friendcode");
		}

		public void OnAcceptClicked()
		{
			SendFriendRequest("friend_request_accepted");
		}

		public void OnDeclineClicked()
		{
			if (Friend.CanDecline)
			{
				GenericPopupRequest request = new GenericPopupRequest("friend_decline_confirm").SetTexts(Localization.Key("friend_request"), Localization.Format(Localization.Key("confirm_friend_decline"), Localization.Literal(Friend.DisplayName))).SetGreenOkButton(OnDeclineConfirmed).SetRedCancelButton();
				_popupManager.RequestPopup(request);
			}
		}

		public void OnVisitClicked()
		{
			_islandsManager.StartVisiting(Friend.UserId);
			_popupManager.CloseAllOpenPopups(instant: false);
		}

		public void OnGiftClicked()
		{
			if (Friend.CanReceiveGift)
			{
				ReceiveGift();
			}
			else
			{
				if (!Friend.CanSendGift)
				{
					return;
				}
				_waitingForCallback = true;
				UpdateLook();
				_friendsManager.SendGift(Friend, OnCallbackReceived);
			}
			_giftButtonTweener.StopAndReset();
			_giftButtonTweener.Play();
		}

		private void ReceiveGift()
		{
			_waitingForCallback = true;
			UpdateLook();
			_friendsManager.ReceiveGift(Friend, OnCallbackReceived);
		}

		private void SendFriendRequest(string analyticsEventName)
		{
			if (Friend.CanInvite)
			{
				_waitingForCallback = true;
				UpdateLook();
				_friendsManager.SendFriendRequest(Friend.FriendCode, analyticsEventName, OnFriendRequestSuccess, OnFriendRequestError);
			}
		}

		private void UpdateLook()
		{
			if (Friend != null)
			{
				_levelText.LocalizedString = Localization.Integer(Friend.Level);
				_leaderboardScoreText.LocalizedString = Localization.Integer(Friend.Score);
				_displayNameText.LocalizedString = Localization.Literal(Friend.DisplayName);
				bool flag = Friend.FriendStatus == FriendStatusType.Received;
				bool flag2 = Friend.FriendStatus == FriendStatusType.Suggested;
				bool flag3 = Friend.FriendStatus == FriendStatusType.Sent;
				bool flag4 = Friend.FriendStatus == FriendStatusType.Accepted;
				_frameButton.interactable = Friend.CanReceiveGift;
				_requestButton.gameObject.SetActive(flag2 | flag3);
				_requestButton.interactable = (!_waitingForCallback && flag2);
				_requestButtonText.LocalizedString = (flag2 ? Localization.Key("send_request") : Localization.Key("request_sent"));
				_giftButton.gameObject.SetActive(flag4);
				_giftButton.interactable = (!_waitingForCallback && (Friend.CanReceiveGift || Friend.CanSendGift));
				_giftReceiveBanner.SetActive(flag4 && Friend.CanReceiveGift);
				if (Friend.CanReceiveGift || Friend.CanSendGift)
				{
					_giftButtonText.LocalizedString = (Friend.CanReceiveGift ? Localization.Key("receive_gift") : Localization.Key("sspmenu.sendgift"));
				}
				UpdateTimerRoutine();
				_visitButton.gameObject.SetActive(!flag);
				_visitButton.interactable = Friend.CanVisit(_authenticator.CurrentAuthentication.UserId);
				_acceptButton.gameObject.SetActive(flag);
				_acceptButton.interactable = (!_waitingForCallback && flag);
				_declineButton.gameObject.SetActive(flag);
				_declineButton.interactable = (!_waitingForCallback && flag);
			}
		}

		private void UpdateTimerRoutine()
		{
			if (_timerRoutine == null)
			{
				if (base.gameObject.activeInHierarchy && !Friend.CanReceiveGift && !Friend.CanSendGift)
				{
					StartCoroutine(_timerRoutine = TimerRoutine());
				}
			}
			else if (Friend.CanReceiveGift)
			{
				StopCoroutine(_timerRoutine);
				_timerRoutine = null;
			}
		}

		private void OnDataChanged()
		{
			UpdateLook();
		}

		private void OnFriendRequestSuccess(FriendData friendData)
		{
			OnCallbackReceived();
			GenericPopupRequest request = new GenericPopupRequest("friend_request_sent_success").SetTexts(Localization.Key((friendData.FriendStatus == FriendStatusType.Accepted) ? "friend_invite_accepted" : "sspmenu.friend_invite_sent"), Localization.Format(Localization.Key((friendData.FriendStatus == FriendStatusType.Accepted) ? "friend_request_accepted" : "friend_request_sent"), Localization.Literal(friendData.DisplayName))).SetGreenOkButton();
			_popupManager.RequestPopup(request);
		}

		private void OnFriendRequestError()
		{
			OnCallbackReceived();
			GenericPopupRequest request = new GenericPopupRequest("friend_invite_code_error").SetTexts(Localization.Key("oops_something_went_wrong"), Localization.Key("invite_code_error")).SetGreenOkButton();
			_popupManager.RequestPopup(request);
		}

		private void OnDeclineConfirmed()
		{
			_friendsManager.DeclineFriendRequest(Friend.FriendCode, OnCallbackReceived);
		}

		private void OnCallbackReceived()
		{
			_waitingForCallback = false;
			UpdateLook();
		}

		private IEnumerator TimerRoutine()
		{
			while (true)
			{
				_giftButtonText.LocalizedString = Localization.TimeSpan(Friend.SendGiftCooldown, hideSecondPartWhenZero: false);
				yield return new WaitForSecondsRealtime(1f);
			}
		}
	}
}
