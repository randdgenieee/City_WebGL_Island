using CIG;
using CIG.Translation;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MoreCashGoldPopup : IAPPopup
{
	[SerializeField]
	private LocalizedText _titletext;

	[SerializeField]
	private LocalizedText _bodyText;

	[SerializeField]
	private Transform _buttonsContainerTransform;

	[SerializeField]
	private GameObject _bankButton;

	[SerializeField]
	private BuyMoreCurrencyButton _paymentButtonPrefab;

	private readonly List<BuyMoreCurrencyButton> _instances = new List<BuyMoreCurrencyButton>();

	private BuyMoreCurrencyButton.BuyMoreCurrency[] _paymentButtons;

	private GameState _gameState;

	private SaleManager _saleManager;

	private IAPStore<TOCIStoreProduct> _iapStore;

	private Currencies _purchasePrice;

	private Currencies _spentCurrency;

	private TOCIStoreProduct _currentOffer;

	private Action<bool, Currencies> _purchaseCallback;

	private int _maxGoldPrice;

	private bool _showBankButton;

	public override string AnalyticsScreenName => "more_cash_gold";

	public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
	{
		base.Initialize(model, canvasScaler);
		_gameState = model.Game.GameState;
		_saleManager = model.Game.SaleManager;
		_iapStore = model.GameServer.IAPStore;
	}

	public override void Open(PopupRequest request)
	{
		base.Open(request);
		MoreCashGoldPopupRequest request2 = GetRequest<MoreCashGoldPopupRequest>();
		_showBankButton = request2.ShowBankButton;
		_bankButton.SetActive(_showBankButton);
		UpdateInfo(request2.PurchasePrice, request2.MaxGoldPrice, request2.PurchaseCallback);
	}

	public override void Close(bool instant)
	{
		if (_purchaseCallback != null)
		{
			FirePurchaseDone(success: false, spendCurrencies: false);
		}
		_currentOffer = null;
		base.Close(instant);
	}

	public void OnGoToBankClicked()
	{
		ShopMenuTabs value = (!_gameState.Balance.MissingCurrencies(_purchasePrice).Contains("Gold")) ? ShopMenuTabs.Cash : ShopMenuTabs.Gold;
		_popupManager.RequestPopup(new ShopPopupRequest(value));
		_popupManager.CloseAllOpenPopups(instant: false);
	}

	private void UpdateContent(ILocalizedString titleText, ILocalizedString bodyText, BuyMoreCurrencyButton.BuyMoreCurrency[] paymentOptions)
	{
		_titletext.LocalizedString = titleText;
		_bodyText.LocalizedString = bodyText;
		_paymentButtons = paymentOptions;
		int i = 0;
		for (int count = _instances.Count; i < count; i++)
		{
			UnityEngine.Object.Destroy(_instances[i].gameObject);
		}
		_instances.Clear();
		int j = 0;
		for (int num = _paymentButtons.Length; j < num; j++)
		{
			BuyMoreCurrencyButton buyMoreCurrencyButton = UnityEngine.Object.Instantiate(_paymentButtonPrefab, _buttonsContainerTransform);
			buyMoreCurrencyButton.Data = _paymentButtons[j];
			_instances.Add(buyMoreCurrencyButton);
		}
	}

	private void UpdateInfo(Currencies purchasePrice, int maxGoldPrice, Action<bool, Currencies> purchaseCallback)
	{
		_purchasePrice = purchasePrice;
		_purchaseCallback = purchaseCallback;
		_maxGoldPrice = maxGoldPrice;
		Currencies currencies = _gameState.Balance.MissingCurrencies(purchasePrice);
		bool flag = currencies.Contains("Gold");
		bool flag2 = currencies.Contains("Cash");
		List<BuyMoreCurrencyButton.BuyMoreCurrency> list = new List<BuyMoreCurrencyButton.BuyMoreCurrency>();
		ILocalizedString titleText;
		ILocalizedString bodyText;
		if (flag && flag2)
		{
			decimal value = currencies.GetValue("Gold");
			List<BuyMoreCurrencyButton.BuyMoreCurrency> buttonsForMissingCashAndGold = GetButtonsForMissingCashAndGold(value, out _currentOffer, out titleText, out bodyText);
			list.AddRange(buttonsForMissingCashAndGold);
		}
		else if (flag)
		{
			decimal value2 = currencies.GetValue("Gold");
			List<BuyMoreCurrencyButton.BuyMoreCurrency> buttonsForMissingGold = GetButtonsForMissingGold(value2, out _currentOffer, out titleText, out bodyText);
			list.AddRange(buttonsForMissingGold);
		}
		else
		{
			if (!flag2)
			{
				OnCloseClicked();
				return;
			}
			decimal value3 = currencies.GetValue("Cash");
			List<BuyMoreCurrencyButton.BuyMoreCurrency> buttonsForMissingCash = GetButtonsForMissingCash(value3, purchasePrice.GetValue("Cash"), out _currentOffer, out titleText, out bodyText);
			list.AddRange(buttonsForMissingCash);
		}
		if (_currentOffer != null)
		{
			list.Insert(0, new BuyMoreCurrencyButton.BuyMoreCurrency(_currentOffer, _currentOffer.Currencies, TryBuyAndClosePopup, Localization.Format(" ({0})", Localization.Literal(_currentOffer.FormattedPrice))));
		}
		UpdateContent(titleText, bodyText, list.ToArray());
	}

	private List<BuyMoreCurrencyButton.BuyMoreCurrency> GetButtonsForMissingCashAndGold(decimal missingGold, out TOCIStoreProduct iap, out ILocalizedString titleText, out ILocalizedString bodyText)
	{
		iap = FindIAP("Gold", missingGold);
		titleText = Localization.Format(Localization.Key("not_enough_x"), Localization.Key("gold"));
		bodyText = Localization.Key("you_need_more_gold_and_cash");
		return new List<BuyMoreCurrencyButton.BuyMoreCurrency>();
	}

	private List<BuyMoreCurrencyButton.BuyMoreCurrency> GetButtonsForMissingGold(decimal missingGold, out TOCIStoreProduct iap, out ILocalizedString titleText, out ILocalizedString bodyText)
	{
		iap = FindIAP("Gold", missingGold);
		titleText = Localization.Format(Localization.Key("not_enough_x"), Localization.Key("gold"));
		bodyText = Localization.Key("you_need_more_gold");
		return new List<BuyMoreCurrencyButton.BuyMoreCurrency>();
	}

	private List<BuyMoreCurrencyButton.BuyMoreCurrency> GetButtonsForMissingCash(decimal missingCash, decimal cashCost, out TOCIStoreProduct iap, out ILocalizedString titleText, out ILocalizedString bodyText)
	{
		List<BuyMoreCurrencyButton.BuyMoreCurrency> list = new List<BuyMoreCurrencyButton.BuyMoreCurrency>();
		decimal value = _gameState.Balance.GetValue("Cash");
		decimal goldCostForMissingCash = GoldCostUtility.GetGoldCostForMissingCash(value, cashCost, _maxGoldPrice);
		if (value > decimal.Zero && goldCostForMissingCash > decimal.Zero && goldCostForMissingCash <= _gameState.Balance.GetValue("Gold"))
		{
			list.Add(new BuyMoreCurrencyButton.BuyMoreCurrency(null, new Currencies(Currency.GoldCurrency(goldCostForMissingCash), Currency.CashCurrency(value)), TryBuyAndClosePopup, Localization.EmptyLocalizedString));
		}
		if (_maxGoldPrice > 0 && (decimal)_maxGoldPrice <= _gameState.Balance.GetValue("Gold"))
		{
			list.Add(new BuyMoreCurrencyButton.BuyMoreCurrency(null, new Currencies(Currency.GoldCurrency(_maxGoldPrice)), TryBuyAndClosePopup, Localization.EmptyLocalizedString));
		}
		iap = FindIAP("Cash", missingCash);
		titleText = Localization.Format(Localization.Key("not_enough_x"), Localization.Key("cash"));
		bodyText = Localization.Key("you_need_more_cash");
		return list;
	}

	private void TryBuyAndClosePopup(BuyMoreCurrencyButton.BuyMoreCurrency paymentOption)
	{
		if (paymentOption.IsIAP)
		{
			BuyIAP();
			return;
		}
		_gameState.SpendCurrencies(paymentOption.Currency, _maxGoldPrice, _showBankButton, CurrenciesSpentReason.CashExchange, null);
		_spentCurrency = paymentOption.Currency;
		FirePurchaseDone(success: true, spendCurrencies: false);
	}

	private void FirePurchaseDone(bool success, bool spendCurrencies)
	{
		if (_purchaseCallback != null)
		{
			if (success && spendCurrencies)
			{
				_gameState.SpendCurrencies(_purchasePrice, CurrenciesSpentReason.CashExchange, _purchaseCallback);
			}
			else if (success)
			{
				_purchaseCallback(arg1: true, _spentCurrency);
				_spentCurrency = null;
			}
			else
			{
				_purchaseCallback(arg1: false, null);
			}
		}
		_purchaseCallback = null;
		OnCloseClicked();
	}

	private void BuyIAP()
	{
		_purchaseHandler.InitiatePurchase(_currentOffer, delegate
		{
			FirePurchaseDone(success: true, spendCurrencies: true);
		});
	}

	private TOCIStoreProduct FindIAP(string currencyType, decimal currencyAmount)
	{
		TOCIStoreProduct tOCIStoreProduct = null;
		List<TOCIStoreProduct> list = new List<TOCIStoreProduct>(_iapStore.GetProducts((TOCIStoreProduct p) => (p.Category != StoreProductCategory.Shop) ? (p.Category == StoreProductCategory.ShopSale) : true));
		int i = 0;
		for (int count = list.Count; i < count; i++)
		{
			TOCIStoreProduct tOCIStoreProduct2 = list[i];
			Currencies currencies = tOCIStoreProduct2.Currencies;
			if ((!currencies.ContainsApproximate("Cash") || tOCIStoreProduct2.Category == StoreProductCategory.ShopSale == _saleManager.HasCashSale) && (!currencies.ContainsApproximate("Gold") || tOCIStoreProduct2.Category == StoreProductCategory.ShopSale == _saleManager.HasGoldSale) && currencies.ContainsPositive(currencyType) && currencies.GetValue(currencyType) > currencyAmount && (tOCIStoreProduct == null || currencies.GetValue(currencyType) < tOCIStoreProduct.Currencies.GetValue(currencyType)))
			{
				tOCIStoreProduct = tOCIStoreProduct2;
			}
		}
		return tOCIStoreProduct;
	}
}
