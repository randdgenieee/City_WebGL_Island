using CIG;
using CIG.Translation;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BuyMoreCurrencyButton : MonoBehaviour
{
	public class BuyMoreCurrency
	{
		public TOCIStoreProduct StoreProduct
		{
			get;
			private set;
		}

		public bool IsIAP => StoreProduct != null;

		public Currencies Currency
		{
			get;
			private set;
		}

		public ILocalizedString Price
		{
			get;
			private set;
		}

		public Action<BuyMoreCurrency> Callback
		{
			get;
			private set;
		}

		public BuyMoreCurrency(TOCIStoreProduct storeProduct, Currencies curr, Action<BuyMoreCurrency> callback, ILocalizedString price)
		{
			StoreProduct = storeProduct;
			Currency = curr;
			Price = price;
			Callback = callback;
		}
	}

	[SerializeField]
	private LocalizedText _amount1Label;

	[SerializeField]
	private Image _icon1;

	[SerializeField]
	private LocalizedText _amount2Label;

	[SerializeField]
	private Image _icon2;

	private BuyMoreCurrency _data;

	public BuyMoreCurrency Data
	{
		get
		{
			return _data;
		}
		set
		{
			if (value != _data)
			{
				_data = value;
				bool flag = _data.Currency.Contains("Gold");
				bool flag2 = _data.Currency.Contains("Cash");
				ILocalizedString localizedString = Localization.Integer(_data.Currency.GetValue(flag2 ? "Cash" : "Gold"));
				ILocalizedString localizedString2 = (flag && flag2) ? Localization.Literal("+") : Localization.EmptyLocalizedString;
				_amount1Label.LocalizedString = Localization.Concat(localizedString, _data.Price, localizedString2);
				_icon1.sprite = SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite((!flag2) ? CurrencyType.Gold : CurrencyType.Cash);
				_icon1.SetNativeSize();
				if (flag)
				{
					_amount2Label.LocalizedString = Localization.Integer(_data.Currency.GetValue("Gold"));
				}
				_amount2Label.gameObject.SetActive(flag && flag2);
				_icon2.gameObject.SetActive(flag && flag2);
			}
		}
	}

	public void OnButtonClicked()
	{
		if (Data != null && Data.Callback != null)
		{
			Data.Callback(Data);
		}
	}
}
