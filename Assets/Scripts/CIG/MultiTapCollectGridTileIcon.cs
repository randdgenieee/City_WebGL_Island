using CIG.Translation;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class MultiTapCollectGridTileIcon : ButtonGridTileIcon
	{
		[SerializeField]
		private LocalizedText _amountText;

		[SerializeField]
		private Image _currencyIcon;

		private MultiTapProfitCollectBehaviour _multiTapProfitCollectBehaviour;

		public void Initialize(Action onClicked, MultiTapProfitCollectBehaviour multiTapCollectProfitBehaviour)
		{
			Init(onClicked);
			_multiTapProfitCollectBehaviour = multiTapCollectProfitBehaviour;
			_multiTapProfitCollectBehaviour.CollectedEvent += OnCollected;
			UpdateContent();
		}

		private void OnDestroy()
		{
			if (_multiTapProfitCollectBehaviour != null)
			{
				_multiTapProfitCollectBehaviour.CollectedEvent -= OnCollected;
				_multiTapProfitCollectBehaviour = null;
			}
		}

		private void UpdateContent()
		{
			bool flag = _multiTapProfitCollectBehaviour.RemainingProfit.IsEmpty();
			_amountText.gameObject.SetActive(!flag);
			if (!flag)
			{
				Currency currency = _multiTapProfitCollectBehaviour.RemainingProfit.GetCurrency(0);
				_amountText.LocalizedString = Localization.Integer(currency.Value);
				_currencyIcon.sprite = SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(currency);
			}
		}

		private void OnCollected()
		{
			UpdateContent();
		}
	}
}
