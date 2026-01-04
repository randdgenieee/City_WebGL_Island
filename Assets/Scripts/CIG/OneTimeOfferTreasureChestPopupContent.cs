using CIG.Translation;
using SparkLinq;
using System;
using System.Collections;
using Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class OneTimeOfferTreasureChestPopupContent : OneTimeOfferPopupContentBase
	{
		[SerializeField]
		private Tweener[] _strikeTweeners;

		[SerializeField]
		private Image _chestSpriteImage;

		[SerializeField]
		private LocalizedText _originalIAPPriceText;

		[SerializeField]
		private LocalizedText _newIAPPriceText;

		[SerializeField]
		private CurrencyView _keyCurrencyView;

		[SerializeField]
		private LocalizedText _iapNameText;

		private PurchaseHandler _purchaseHandler;

		private OneTimeOfferTreasureChest _oneTimeOffer;

		private Action _onPurchaseStarted;

		private Action _onPurchaseSuccess;

		private Action _onPurchaseCanceled;

		public void Initialize(OneTimeOfferTreasureChest oneTimeOffer, PurchaseHandler purchaseHandler, Action onPurcheseStarted, Action onPurchaseSuccess, Action onPucheseCanceled, string originalIAPPrice, string newIAPPrice, float discountPercentage)
		{
			_oneTimeOffer = oneTimeOffer;
			_purchaseHandler = purchaseHandler;
			_onPurchaseStarted = onPurcheseStarted;
			_onPurchaseSuccess = onPurchaseSuccess;
			_onPurchaseCanceled = onPucheseCanceled;
			StopAnimation();
			_badgeLocalizedText.LocalizedString = Localization.Percentage(0f - discountPercentage, 0);
			_chestSpriteImage.sprite = SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetChestSprite(_oneTimeOffer.OfferedChest.Properties.TreasureChestType);
			_originalIAPPriceText.LocalizedString = Localization.Literal(originalIAPPrice);
			_newIAPPriceText.LocalizedString = Localization.Literal(newIAPPrice);
			_iapNameText.LocalizedString = _oneTimeOffer.OfferedChest.Name;
			_keyCurrencyView.Initialize(_oneTimeOffer.OfferedChest.Properties.Cost.WithoutEmpty().GetCurrency(0));
		}

		public void OnBuyClicked()
		{
			_onPurchaseStarted?.Invoke();
			_purchaseHandler.InitiatePurchase(_oneTimeOffer.DiscountedChestIAP, delegate
			{
				_oneTimeOffer.Purchase();
				_onPurchaseSuccess?.Invoke();
			}, _onPurchaseCanceled);
			Analytics.LogEvent("one_time_offer_iap_intent_treasurechest");
		}

		public override IEnumerator AnimateInRoutine()
		{
			int i = 0;
			for (int num = _strikeTweeners.Length; i < num; i++)
			{
				_strikeTweeners[i].Play();
			}
			yield return new WaitWhile(() => _strikeTweeners.Any((Tweener tweener) => tweener.IsPlaying));
			yield return new WaitForSeconds(0.15f);
			_discountBadge.SetActive(value: true);
			_badgeTweener.Play();
		}

		public override void StopAnimation()
		{
			_discountBadge.SetActive(value: false);
			_badgeTweener.StopAndReset();
			int i = 0;
			for (int num = _strikeTweeners.Length; i < num; i++)
			{
				_strikeTweeners[i].StopAndReset();
			}
		}
	}
}
