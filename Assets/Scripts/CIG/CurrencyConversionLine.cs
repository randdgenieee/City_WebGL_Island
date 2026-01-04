using CIG.Translation;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class CurrencyConversionLine : MonoBehaviour
	{
		[SerializeField]
		private LocalizedText _fromText;

		[SerializeField]
		private Image _fromIcon;

		[SerializeField]
		private LocalizedText _toText;

		[SerializeField]
		private Image _toIcon;

		public void Initialize(Currencies fromCurrencies, Currencies toCurrencies)
		{
			Currency currency = fromCurrencies.GetCurrency(0);
			_fromText.LocalizedString = Localization.Integer(currency.Value);
			_fromIcon.sprite = SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(currency);
			Currency currency2 = toCurrencies.GetCurrency(0);
			_toText.LocalizedString = Localization.Integer(currency2.Value);
			_toIcon.sprite = SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(currency2);
		}
	}
}
