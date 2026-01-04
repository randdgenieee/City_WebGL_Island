using System;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class CurrencyConversionButton : MonoBehaviour
	{
		[SerializeField]
		private Button _button;

		[SerializeField]
		private CurrencyConversionLine _line;

		private CIGCommercialBuilding _commercialBuilding;

		private CurrencyConversionProperties _properties;

		private Action<CIGCommercialBuilding, CurrencyConversionProperties> _onClick;

		public void Initialize(GameState gameState, CIGCommercialBuilding commercialBuilding, CurrencyConversionProperties properties, Action<CIGCommercialBuilding, CurrencyConversionProperties> onClick)
		{
			_commercialBuilding = commercialBuilding;
			_properties = properties;
			_onClick = onClick;
			_button.interactable = gameState.CanAfford(properties.FromCurrency);
			Currencies currencies = commercialBuilding.ApplyMultiplierToCurrencyConversion(properties.ToCurrency);
			if (currencies.IsEmpty())
			{
				base.gameObject.SetActive(value: false);
				return;
			}
			base.gameObject.SetActive(value: true);
			_line.Initialize(properties.FromCurrency, currencies);
		}

		public void OnClicked()
		{
			EventTools.Fire(_onClick, _commercialBuilding, _properties);
		}
	}
}
