using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class BuildingPopupExchangeContainer : MonoBehaviour
	{
		[SerializeField]
		private CurrencyConversionButton _currencyConversionButtonPrefab;

		[SerializeField]
		private RectTransform _currencyConversionButtonParent;

		private GameState _gameState;

		private readonly List<CurrencyConversionButton> _conversionButtons = new List<CurrencyConversionButton>();

		private CIGCommercialBuilding _commercialBuilding;

		public void Initialize(GameState gameState)
		{
			_gameState = gameState;
		}

		public void Show(CIGCommercialBuilding commercialBuilding, List<CurrencyConversionProperties> conversionProperties, Action<CIGCommercialBuilding, CurrencyConversionProperties> onClicked)
		{
			base.gameObject.SetActive(value: true);
			_commercialBuilding = commercialBuilding;
			int i = 0;
			for (int count = conversionProperties.Count; i < count; i++)
			{
				CurrencyConversionButton currencyConversionButton = UnityEngine.Object.Instantiate(_currencyConversionButtonPrefab, _currencyConversionButtonParent);
				currencyConversionButton.Initialize(_gameState, _commercialBuilding, conversionProperties[i], onClicked);
				_conversionButtons.Add(currencyConversionButton);
			}
		}

		public void Hide()
		{
			_commercialBuilding = null;
			base.gameObject.SetActive(value: false);
			int i = 0;
			for (int count = _conversionButtons.Count; i < count; i++)
			{
				UnityEngine.Object.Destroy(_conversionButtons[i].gameObject);
			}
			_conversionButtons.Clear();
		}
	}
}
