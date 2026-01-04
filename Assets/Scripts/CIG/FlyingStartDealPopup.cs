using CIG.Translation;
using System.Collections;
using UnityEngine;

namespace CIG
{
	public class FlyingStartDealPopup : IAPPopup
	{
		[SerializeField]
		private LocalizedText _goldAmountText;

		[SerializeField]
		private LocalizedText _cashAmountText;

		[SerializeField]
		private LocalizedText _buyButtonText;

		[SerializeField]
		private LocalizedText _totalValueText;

		[SerializeField]
		private GameObject _timerRoot;

		[SerializeField]
		private LocalizedText _timerText;

		private FlyingStartDealManager _flyingStartDealManager;

		public override string AnalyticsScreenName => "flying_start_deal";

		public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
		{
			base.Initialize(model, canvasScaler);
			_flyingStartDealManager = model.Game.FlyingStartDealManager;
			if (_flyingStartDealManager.StoreProduct != null)
			{
				_goldAmountText.LocalizedString = Localization.Integer(_flyingStartDealManager.StoreProduct.Currencies.GetValue("Gold"));
				_cashAmountText.LocalizedString = Localization.Integer(_flyingStartDealManager.StoreProduct.Currencies.GetValue("Cash"));
				_buyButtonText.LocalizedString = Localization.Literal(_flyingStartDealManager.StoreProduct.FormattedPrice);
			}
			if (_flyingStartDealManager.AlternativeStoreProduct != null)
			{
				_totalValueText.LocalizedString = Localization.Format(Localization.Key("total_value_more_than"), Localization.Literal(_flyingStartDealManager.AlternativeStoreProduct.FormattedPrice).FontSize(30));
			}
		}

		public override void Open(PopupRequest request)
		{
			base.Open(request);
			StartCoroutine(TimerRoutine());
			Analytics.IAPViewed(_flyingStartDealManager.StoreProduct.Identifier);
		}

		public void OnBuyClicked()
		{
			_purchaseHandler.InitiatePurchase(_flyingStartDealManager.StoreProduct, OnPurchaseSuccess);
		}

		private void OnPurchaseSuccess()
		{
			OnCloseClicked();
		}

		private IEnumerator TimerRoutine()
		{
			_timerRoot.SetActive(value: true);
			while (_flyingStartDealManager.IsActive)
			{
				_timerText.LocalizedString = Localization.TimeSpan(_flyingStartDealManager.TimeLeft, hideSecondPartWhenZero: true);
				yield return new WaitForSeconds(1f);
			}
			_timerRoot.SetActive(value: false);
		}
	}
}
