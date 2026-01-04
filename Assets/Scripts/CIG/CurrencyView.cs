using CIG.Translation;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class CurrencyView : MonoBehaviour
	{
		[SerializeField]
		protected LocalizedText _amountText;

		[SerializeField]
		private Image _icon;

		[SerializeField]
		private CurrencySpriteSize _size;

		public LocalizedText AmountText => _amountText;

		public void Initialize(Currency currency)
		{
			Initialize(currency.ToCurrencyType(), currency.Value);
		}

		public virtual void Initialize(CurrencyType currencyType, decimal value)
		{
			_amountText.LocalizedString = GetText(currencyType, value);
			_icon.sprite = SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(currencyType, _size);
		}

		private ILocalizedString GetText(CurrencyType currencyType, decimal value)
		{
			if (currencyType == CurrencyType.LevelUp)
			{
				return Localization.Format(Localization.PluralKey("iap.title.levels", (long)value), Localization.Integer(value));
			}
			return Localization.Integer(value);
		}
	}
}
