using CIG.Translation;
using System;
using System.Collections.Generic;

namespace CIG
{
	public class RatingRequester
	{
		public delegate void QualifiedForRatingChangedEventHandler();

		private const string FeedbackEmail = "feedback@sparklingsociety.net";

		private const int MinimumLevel = 3;

		private const int MinimumSessionLengthSeconds = 60;

		private const int DelayAfterRatingDays = 7;

		private readonly StorageDictionary _storage;

		private readonly WebService _webService;

		private readonly PopupManager _popupManager;

		private readonly GameState _gameState;

		private readonly List<string> _tokens;

		private const string TokensKey = "tokens";

		private const string QualifiedForRatingKey = "qualified";

		private const string LastRatingKey = "lastRating";

		private const string HasReviewedKey = "hasReviewed";

		public bool QualifiedForRating
		{
			get;
			private set;
		}

		public DateTime LastRatingRequest
		{
			get;
			private set;
		}

		public bool HasReviewed
		{
			get;
			private set;
		}

		public bool IsShowingRatingRequest
		{
			get;
			private set;
		}

		public event QualifiedForRatingChangedEventHandler QualifiedForRatingChangedEvent;

		private void FireQualifiedForRatingChangedEvent()
		{
			this.QualifiedForRatingChangedEvent?.Invoke();
		}

		public RatingRequester(StorageDictionary storage, WebService webService, PopupManager popupManager, GameState gameState)
		{
			_storage = storage;
			_webService = webService;
			_popupManager = popupManager;
			_gameState = gameState;
			_tokens = _storage.GetList<string>("tokens");
			QualifiedForRating = _storage.Get("qualified", defaultValue: false);
			LastRatingRequest = _storage.GetDateTime("lastRating", DateTime.MinValue);
			HasReviewed = _storage.Get("hasReviewed", defaultValue: false);
			_webService.PullRequestCompletedEvent += OnPullRequestCompleted;
			OnPullRequestCompleted(_webService.Properties);
		}

		public void Release()
		{
			_webService.PullRequestCompletedEvent -= OnPullRequestCompleted;
		}

		public void TryRequestRating()
		{
			bool flag = !HasReviewed || (AntiCheatDateTime.UtcNow - LastRatingRequest).Days >= 7;
			if (((!IsShowingRatingRequest && QualifiedForRating) & flag) && _gameState.SecondsPlayedInThisSession >= 60 && _gameState.Level >= 3)
			{
				RequestRating();
			}
		}

		private void RequestRating()
		{
			IsShowingRatingRequest = true;
			GenericPopupRequest request = new GenericPopupRequest("rating_request").SetDismissable(dismissable: false).SetTexts(Localization.Key("rating_appreciate_your_feedback"), Localization.Format(Localization.Key("rating_do_like_playing"), Localization.Literal("City Island 5"))).SetGreenButton(Localization.Key("rating_yes"), null, OnLikeGameClicked)
				.SetRedButton(Localization.Key("rating_not_really"), null, OnDislikeGameClicked)
				.SetIcon(SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.GameIcon));
			_popupManager.RequestPopup(request);
		}

		private void AskedUserForRating(bool agreedToReview)
		{
			QualifiedForRating = false;
			LastRatingRequest = AntiCheatDateTime.UtcNow;
			HasReviewed = agreedToReview;
		}

		private void OnLikeGameClicked()
		{
			GenericPopupRequest request = new GenericPopupRequest("rating_like").SetDismissable(dismissable: false).SetTexts(Localization.Key("thank_you"), Localization.Key("rating_rate_game_description")).SetGreenButton(Localization.Key("rating_yeah_sure"), null, UserAgreedToRate)
				.SetRedButton(Localization.Key("no_thanks"), null, UserDeniedToRate)
				.SetIcon(SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.FiveStarIcon));
			_popupManager.RequestPopup(request);
		}

		private void OnDislikeGameClicked()
		{
			GenericPopupRequest request = new GenericPopupRequest("rating_dislike").SetDismissable(dismissable: false).SetTexts(Localization.Key("rating_sorry_to_hear"), Localization.Format(Localization.Key("rating_feedback_at"), Localization.Literal("feedback@sparklingsociety.net"))).SetGreenButton(Localization.Key("rating_will_do"), null, UserDeniedToRate)
				.SetRedButton(Localization.Key("no_thanks"), null, UserDeniedToRate)
				.SetIcon(SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.GameIcon));
			_popupManager.RequestPopup(request);
		}

		private void UserAgreedToRate()
		{
			IsShowingRatingRequest = false;
			AskedUserForRating(agreedToReview: true);
		}

		private void UserDeniedToRate()
		{
			IsShowingRatingRequest = false;
			AskedUserForRating(agreedToReview: false);
		}

		private void OnPullRequestCompleted(Dictionary<string, string> properties)
		{
			if (!properties.TryGetValue("adjust_tokens", out string value))
			{
				return;
			}
			string[] array = value.Split(',');
			int i = 0;
			for (int num = array.Length; i < num; i++)
			{
				string item = array[i];
				if (!_tokens.Contains(item))
				{
					_tokens.Add(item);
					if (!QualifiedForRating)
					{
						QualifiedForRating = true;
						FireQualifiedForRatingChangedEvent();
					}
				}
			}
		}

		public void Serialize()
		{
			_storage.Set("tokens", _tokens);
			_storage.Set("qualified", QualifiedForRating);
			_storage.Set("lastRating", LastRatingRequest);
			_storage.Set("hasReviewed", HasReviewed);
		}
	}
}
