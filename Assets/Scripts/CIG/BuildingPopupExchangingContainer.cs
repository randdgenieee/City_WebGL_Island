using CIG.Translation;
using System;
using System.Collections;
using UnityEngine;

namespace CIG
{
	public class BuildingPopupExchangingContainer : MonoBehaviour
	{
		[SerializeField]
		private CurrencyConversionLine _conversionLine;

		[SerializeField]
		private LocalizedText _durationText;

		[SerializeField]
		private InteractableButton _videoButton;

		[SerializeField]
		private GameObject _videoButtonContainer;

		[SerializeField]
		private ButtonStyleView _waitButtonstyle;

		private VideoAds2Manager _videoAds2Manager;

		private CurrencyConversionProcess _conversionProcess;

		private Timing _timing;

		private IEnumerator _timerRoutine;

		public void Initialize(VideoAds2Manager videoAds2Manager, Timing timing)
		{
			_videoAds2Manager = videoAds2Manager;
			_timing = timing;
		}

		private void OnDestroy()
		{
			if (_videoAds2Manager != null)
			{
				_videoAds2Manager.AvailabilityChangedEvent -= OnAdAvailabilityChanged;
				_videoAds2Manager = null;
			}
		}

		public void Show(CIGCommercialBuilding commercialBuilding)
		{
			base.gameObject.SetActive(value: true);
			_conversionProcess = commercialBuilding.CurrencyConversionProcess;
			_conversionLine.Initialize(commercialBuilding.CurrencyConversionProcess.FromCurrencies, commercialBuilding.CurrencyConversionProcess.ToCurrencies);
			_videoAds2Manager.AvailabilityChangedEvent += OnAdAvailabilityChanged;
			OnAdAvailabilityChanged();
			StopRoutine();
			StartCoroutine(_timerRoutine = TimerRoutine());
		}

		public void Hide()
		{
			StopRoutine();
			_conversionProcess = null;
			_videoAds2Manager.AvailabilityChangedEvent -= OnAdAvailabilityChanged;
			base.gameObject.SetActive(value: false);
		}

		private void StopRoutine()
		{
			if (_timerRoutine != null)
			{
				StopCoroutine(_timerRoutine);
				_timerRoutine = null;
			}
		}

		private void OnAdAvailabilityChanged()
		{
			_videoButton.interactable = _videoAds2Manager.IsReady;
		}

		private IEnumerator TimerRoutine()
		{
			while (true)
			{
				_durationText.LocalizedString = Localization.TimeSpan(TimeSpan.FromSeconds(_conversionProcess.TimeLeft), hideSecondPartWhenZero: true);
				yield return new WaitForGameTimeSeconds(_timing, 1.0);
			}
		}
	}
}
