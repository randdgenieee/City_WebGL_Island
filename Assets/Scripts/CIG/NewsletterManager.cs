using CIG.Translation;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace CIG
{
	public class NewsletterManager
	{
		private readonly StorageDictionary _storage;

		private readonly WebService _webService;

		private readonly PopupManager _popupManager;

		private readonly ServerGifts _serverGifts;

		private readonly RoutineRunner _routineRunner;

		private readonly NewsletterProperties _properties;

		private bool _hasClaimedReward;

		private IEnumerator _submitRoutine;

		private const string HasClaimedRewardKey = "HasClaimedReward";

		public NewsletterManager(StorageDictionary storage, WebService webService, PopupManager popupManager, ServerGifts serverGifts, RoutineRunner routineRunner, NewsletterProperties properties)
		{
			_storage = storage;
			_webService = webService;
			_popupManager = popupManager;
			_serverGifts = serverGifts;
			_routineRunner = routineRunner;
			_properties = properties;
			_hasClaimedReward = _storage.Get("HasClaimedReward", defaultValue: false);
		}

		public void SubmitEmailAsync(string email, Action onSuccess, Action onError)
		{
			if (_submitRoutine == null)
			{
				_routineRunner.StartCoroutine(_submitRoutine = SubmitEmailRoutine(email, onSuccess, onError));
			}
		}

		private void OnPopupClosed()
		{
			_serverGifts.ReceiveUndeliveredGifts(enqueue: false);
		}

		private IEnumerator SubmitEmailRoutine(string email, Action onSuccess, Action onError)
		{
			UnityWebRequest webRequest = _webService.RegisterNewsLetter(email);
			yield return webRequest.SendWebRequest();
			if (!string.IsNullOrEmpty(webRequest.downloadHandler.text) && webRequest.downloadHandler.text == "SUCCESS")
			{
				if (!_hasClaimedReward)
				{
					_serverGifts.AddGift(_properties.SubscribeReward);
					_hasClaimedReward = true;
				}
				EventTools.Fire(onSuccess);
				GenericPopupRequest request = new GenericPopupRequest("newsletter_success").SetDismissable(dismissable: false).SetTexts(Localization.Key("thank_you"), Localization.Format(Localization.Key("register_success_confirmation"), Localization.Literal(email))).SetGreenOkButton(OnPopupClosed);
				_popupManager.RequestPopup(request);
				Analytics.LogEvent("newsletter_subscription");
			}
			else
			{
				UnityEngine.Debug.LogError(webRequest.downloadHandler.text);
				EventTools.Fire(onError);
				GenericPopupRequest request2 = new GenericPopupRequest("newsletter_error").SetTexts(Localization.Key("oops_something_went_wrong"), Localization.Key("SSP_ERROR")).SetGreenOkButton();
				_popupManager.RequestPopup(request2);
			}
			_submitRoutine = null;
		}

		public void Serialize()
		{
			_storage.Set("HasClaimedReward", _hasClaimedReward);
		}
	}
}
