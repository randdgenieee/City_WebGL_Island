using CIG.Translation;
using Tweening;
using UnityEngine;

namespace CIG
{
	public class HUDCurrencyBar : HUDCurrencyTweenHelper
	{
		[SerializeField]
		private Tweener _iconTweener;

		[SerializeField]
		private string _currencyType;

		[SerializeField]
		private LocalizedText _currencyLabel;

		private GameState _gameState;

		private decimal _currentValue;

		private decimal _endValue;

		public void Initialize(GameState gameState)
		{
			_gameState = gameState;
			Initialize();
			_gameState.BalanceChangedEvent += OnBalanceChanged;
			_endValue = _gameState.Balance.GetValue(_currencyType);
			SetActiveTweener(_iconTweener);
			UpdateValue(_endValue);
		}

		protected override void OnDestroy()
		{
			if (_gameState != null)
			{
				_gameState.BalanceChangedEvent -= OnBalanceChanged;
				_gameState = null;
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
			_currencyLabel.LocalizedString = Localization.Integer((long)_currentValue);
		}

		private void OnBalanceChanged(Currencies oldBalance, Currencies newBalance, FlyingCurrenciesData flyingCurrenciesData)
		{
			decimal num = newBalance.GetValue(_currencyType) - oldBalance.GetValue(_currencyType);
			if (num < decimal.Zero)
			{
				_endValue += num;
				TweenTo(_currentValue, _endValue);
			}
		}
	}
}
