using CIG.Translation;
using System;
using System.Collections;
using Tweening;
using UnityEngine;

namespace CIG
{
	public class OneTimeOfferBuildingPopupContent : OneTimeOfferPopupContentBase
	{
		[SerializeField]
		private BuildingImage _buildingImage;

		[SerializeField]
		private Tweener _originalStrikeTweener;

		[SerializeField]
		private NumberTweenHelper _discountTextTweener;

		[SerializeField]
		private Tweener _discountedTextTweener;

		[SerializeField]
		private LocalizedText _originalPriceText;

		[SerializeField]
		private LocalizedText _discountedPriceText;

		[SerializeField]
		private LocalizedText _buildingNameText;

		private OneTimeOfferBuilding _oneTimeOffer;

		private Action _onPurchaseStarted;

		private Action _onClose;

		private Action _onPurchaseCanceled;

		public void Initialize(OneTimeOfferBuilding oneTimeOffer, Action onPurchaseStarted, Action onClose, Action onPurchaseCanceled)
		{
			StopAnimation();
			_oneTimeOffer = oneTimeOffer;
			_buildingImage.Initialize(_oneTimeOffer.BuildingProperties);
			_buildingNameText.LocalizedString = _oneTimeOffer.BuildingProperties.LocalizedName;
			_originalPriceText.LocalizedString = Localization.Integer(_oneTimeOffer.NormalPrice);
			_discountedPriceText.LocalizedString = Localization.Integer(_oneTimeOffer.DiscountedPrice);
			_badgeLocalizedText.LocalizedString = Localization.Percentage(0f - _oneTimeOffer.CurrentDiscountPercentage, 0);
			_onPurchaseStarted = onPurchaseStarted;
			_onClose = onClose;
			_onPurchaseCanceled = onPurchaseCanceled;
		}

		public void OnBuyClicked()
		{
			_onPurchaseStarted?.Invoke();
			_oneTimeOffer.Purchase(_onClose, _onPurchaseCanceled);
		}

		public override IEnumerator AnimateInRoutine()
		{
			_originalStrikeTweener.Play();
			yield return new WaitWhile(() => _originalStrikeTweener.IsPlaying);
			yield return new WaitForSeconds(0.2f);
			_discountTextTweener.TweenTo(_oneTimeOffer.NormalPrice, _oneTimeOffer.DiscountedPrice);
			yield return new WaitWhile(() => _discountTextTweener.Running);
			_discountBadge.SetActive(value: true);
			_badgeTweener.Play();
			_discountedTextTweener.Play();
		}

		public override void StopAnimation()
		{
			_originalStrikeTweener.StopAndReset();
			_discountTextTweener.StopTweening();
			_badgeTweener.StopAndReset();
			_discountedTextTweener.StopAndReset();
			_discountBadge.SetActive(value: false);
		}
	}
}
