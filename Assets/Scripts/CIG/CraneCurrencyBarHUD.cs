using CIG.Translation;
using System;
using Tweening;
using UnityEngine;

namespace CIG
{
	public class CraneCurrencyBarHUD : HUDCurrencyTweenHelper
	{
		[SerializeField]
		private Tweener _iconTweener;

		[SerializeField]
		private LocalizedText _currencyLabel;

		private CraneManager _craneManager;

		private decimal _currentValue;

		private decimal _endValue;

		public void Initialize(CraneManager craneManager)
		{
			_craneManager = craneManager;
			Initialize();
			_craneManager.BuildCountChangedEvent += OnBuildCountChanged;
			_endValue = _craneManager.MaxBuildCount;
			SetActiveTweener(_iconTweener);
			UpdateValue(_endValue);
		}

		protected override void OnDestroy()
		{
			if (_craneManager != null)
			{
				_craneManager.BuildCountChangedEvent -= OnBuildCountChanged;
				_craneManager = null;
			}
			base.OnDestroy();
		}

		public override void FlyingCurrencyFinishedPlaying(Currency earnedCurrency, bool animateHudElement = true)
		{
			_endValue += earnedCurrency.Value;
			if (animateHudElement)
			{
				TweenTo(_currentValue, _endValue);
			}
			else
			{
				UpdateValue(_endValue);
			}
			base.FlyingCurrencyFinishedPlaying(earnedCurrency, animateHudElement);
		}

		protected override void UpdateValue(decimal value)
		{
			_currentValue = value;
			decimal m = Math.Max(_currentValue - (decimal)_craneManager.CurrentBuildCount, decimal.Zero);
			_currencyLabel.LocalizedString = Localization.Format("{0}/{1}", Localization.Integer(m), Localization.Integer(_currentValue));
		}

		private void OnBuildCountChanged(int used, int total)
		{
			UpdateValue(_currentValue);
		}
	}
}
