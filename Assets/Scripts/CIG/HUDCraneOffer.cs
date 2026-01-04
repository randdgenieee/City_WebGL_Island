using CIG.Translation;
using System.Collections;
using UnityEngine;

namespace CIG
{
	public class HUDCraneOffer : MonoBehaviour
	{
		private const float BlinkingDurationSeconds = 60f;

		private static readonly Color TimerNormalColor = Color.white;

		private static readonly Color TimerBlinkColor = Color.red;

		[SerializeField]
		private GameObject _offerGroup;

		[SerializeField]
		private LocalizedText _timerText;

		private CraneOfferManager _craneOfferManager;

		private PopupManager _popupManager;

		private IEnumerator _updateRemainingTimeRoutine;

		public void Initialize(CraneOfferManager craneOfferManager, PopupManager popupManager)
		{
			_craneOfferManager = craneOfferManager;
			_popupManager = popupManager;
			_craneOfferManager.OfferStartedEvent += OnCraneOfferStarted;
			_craneOfferManager.OfferEndedEvent += OnCraneOfferEnded;
			CheckActive();
		}

		private void OnDestroy()
		{
			if (_craneOfferManager != null)
			{
				_craneOfferManager.OfferStartedEvent -= OnCraneOfferStarted;
				_craneOfferManager.OfferEndedEvent -= OnCraneOfferEnded;
				_craneOfferManager = null;
			}
		}

		private void OnEnable()
		{
			if (_craneOfferManager != null)
			{
				CheckActive();
			}
		}

		public void OnCraneOfferClicked()
		{
			_popupManager.RequestPopup(new CraneOfferPopupRequest(_craneOfferManager.CraneOffer));
		}

		private void CheckActive()
		{
			if (_craneOfferManager.HasCraneOffer && _craneOfferManager.CraneOffer.IsActive)
			{
				OnCraneOfferStarted(_craneOfferManager.CraneOffer);
			}
			else
			{
				OnCraneOfferEnded();
			}
		}

		private IEnumerator UpdateRemainingTimeRoutine(CraneOffer craneOffer)
		{
			bool blink = false;
			while (true)
			{
				_timerText.LocalizedString = Localization.TimeSpan(craneOffer.TimeRemaining, hideSecondPartWhenZero: false);
				if (craneOffer.TimeRemaining.TotalSeconds < 60.0)
				{
					_timerText.TextField.color = (blink ? TimerBlinkColor : TimerNormalColor);
					blink = !blink;
				}
				yield return new WaitForSecondsRealtime(1f);
			}
		}

		private void OnCraneOfferStarted(CraneOffer craneOffer)
		{
			_offerGroup.SetActive(value: true);
			_timerText.TextField.color = TimerNormalColor;
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine(_updateRemainingTimeRoutine = UpdateRemainingTimeRoutine(craneOffer));
			}
		}

		private void OnCraneOfferEnded()
		{
			_offerGroup.SetActive(value: false);
			if (_updateRemainingTimeRoutine != null)
			{
				StopCoroutine(_updateRemainingTimeRoutine);
				_updateRemainingTimeRoutine = null;
			}
		}
	}
}
