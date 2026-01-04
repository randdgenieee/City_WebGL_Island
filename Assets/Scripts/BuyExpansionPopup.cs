using CIG;
using CIG.Translation;
using UnityEngine;

public class BuyExpansionPopup : Popup
{
	[SerializeField]
	private LocalizedText _goldPriceText;

	[SerializeField]
	private LocalizedText _cashPriceText;

	[SerializeField]
	private RectTransform _buyWithGoldMaskTransform;

	private GameStats _gameStats;

	private GameState _gameState;

	private ExpansionBlock _block;

	public override string AnalyticsScreenName => "buy_expansion";

	public RectTransform BuyWithGoldMaskTransform => _buyWithGoldMaskTransform;

	public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
	{
		base.Initialize(model, canvasScaler);
		_gameStats = model.Game.GameStats;
		_gameState = model.Game.GameState;
	}

	public override void Open(PopupRequest request)
	{
		base.Open(request);
		BuyExpansionPopupRequest request2 = GetRequest<BuyExpansionPopupRequest>();
		_block = request2.Block;
		_goldPriceText.LocalizedString = Localization.Integer(_block.Price.GetValue("Gold"));
		_cashPriceText.LocalizedString = Localization.Integer(_block.Price.GetValue("Cash"));
	}

	public void OnBuyWithGoldClicked()
	{
		BuyWith("Gold");
	}

	public void OnBuyWithCashClicked()
	{
		BuyWith("Cash");
	}

	private void BuyWith(string currency)
	{
		decimal value = _block.Price.GetValue(currency);
		Currencies toSpend = new Currencies(currency, value);
		_gameState.SpendCurrencies(toSpend, CurrenciesSpentReason.Expansion, delegate(bool success, Currencies spent)
		{
			if (success)
			{
				_block.Unlock(toSpend);
				if (_gameStats != null)
				{
					_gameStats.AddNumberOfExpansionsPurchased(1);
					decimal value2 = spent.GetValue("Gold");
					decimal value3 = spent.GetValue("Cash");
					if (value2 > decimal.Zero)
					{
						_gameStats.AddNumberOfExpansionsPurchasedWithGold(1);
					}
					else if (value3 > decimal.Zero)
					{
						_gameStats.AddNumberOfExpansionsPurchasedWithCash(1);
					}
				}
			}
		});
		OnCloseClicked();
	}
}
