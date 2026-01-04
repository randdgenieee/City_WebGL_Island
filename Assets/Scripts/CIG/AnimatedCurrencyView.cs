using CIG.Translation;
using UnityEngine;

namespace CIG
{
	public class AnimatedCurrencyView : CurrencyView
	{
		[SerializeField]
		private NumberTweenHelper _numberTweenHelper;

		private CurrencyType _currency;

		private decimal _currentValue;

		public decimal CurrentValue => _currentValue;

		public override void Initialize(CurrencyType currency, decimal value)
		{
			base.Initialize(currency, value);
			_currency = currency;
			SetText(value);
		}

		public void UpdateValue(decimal value)
		{
			SetText(value);
		}

		private void SetText(decimal value)
		{
			CurrencyType currency = _currency;
			if (currency == CurrencyType.LevelUp)
			{
				_amountText.LocalizedString = Localization.Format(Localization.PluralKey("iap.title.levels", (long)value), Localization.Integer(value));
				return;
			}
			if (_numberTweenHelper != null)
			{
				_numberTweenHelper.TweenTo(_currentValue, value);
			}
			else
			{
				_amountText.LocalizedString = Localization.Integer(value);
			}
			_currentValue = value;
		}
	}
}
