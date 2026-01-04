using CIG.Translation;
using System.Collections;
using Tweening;
using UnityEngine;

namespace CIG
{
	public class OneTimeOfferPopup : IAPPopup
	{
		[SerializeField]
		private OneTimeOfferBuildingPopupContent _oneTimeOfferBuildingContent;

		[SerializeField]
		private OneTimeOfferTreasureChestPopupContent _oneTimeOfferTreasureChestContent;

		[SerializeField]
		private Tweener _starTweener;

		private GameState _gameState;

		private IEnumerator _animationRoutine;

		private OneTimeOfferPopupRequest _request;

		private OneTimeOfferPopupContentBase _activeOfferContent;

		private string _analyticsScreenName;

		public override string AnalyticsScreenName => _analyticsScreenName;

		public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
		{
			base.Initialize(model, canvasScaler);
			_gameState = model.Game.GameState;
		}

		public override void Open(PopupRequest request)
		{
			base.Open(request);
			_request = GetRequest<OneTimeOfferPopupRequest>();
			OneTimeOfferBase oneTimeOffer = _request.OneTimeOffer;
			if (oneTimeOffer == null)
			{
				goto IL_015c;
			}
			OneTimeOfferBuilding oneTimeOfferBuilding;
			string arg;
			if ((oneTimeOfferBuilding = (oneTimeOffer as OneTimeOfferBuilding)) == null)
			{
				OneTimeOfferTreasureChest oneTimeOfferTreasureChest;
				if ((oneTimeOfferTreasureChest = (oneTimeOffer as OneTimeOfferTreasureChest)) == null)
				{
					goto IL_015c;
				}
				OneTimeOfferTreasureChest oneTimeOfferTreasureChest2 = oneTimeOfferTreasureChest;
				_oneTimeOfferTreasureChestContent.Initialize(oneTimeOfferTreasureChest2, _purchaseHandler, OnPurchaseStarted, OnChestBought, OnPurchaseCanceled, oneTimeOfferTreasureChest2.OriginalFormattedStorePrice, oneTimeOfferTreasureChest2.DiscountedFormattedStorePrice, oneTimeOfferTreasureChest2.CurrentDiscountPercentage);
				ShowTreasureChestContent();
				StartCoroutine(_animationRoutine = AnimationRoutine());
				arg = "treasure_chest";
				Analytics.LogOneTimeOfferView("one_time_offer_building_seen", oneTimeOfferTreasureChest2.ChestType.ToString(), _gameState.Level);
			}
			else
			{
				OneTimeOfferBuilding oneTimeOfferBuilding2 = oneTimeOfferBuilding;
				_oneTimeOfferBuildingContent.Initialize(oneTimeOfferBuilding2, OnPurchaseStarted, base.ForceClose, OnPurchaseCanceled);
				ShowBuildngContent();
				StartCoroutine(_animationRoutine = AnimationRoutine());
				arg = "building";
				Analytics.LogOneTimeOfferView("one_time_offer_building_seen", oneTimeOfferBuilding2.BuildingProperties.BaseKey, _gameState.Level);
			}
			goto IL_0172;
			IL_0172:
			_analyticsScreenName = $"one_time_offer_{arg}";
			return;
			IL_015c:
			UnityEngine.Debug.LogError("Invalid OneTimeOffer type in OneTimeOfferPopup.");
			arg = string.Empty;
			OnCloseClicked();
			goto IL_0172;
		}

		public void OnIgnoreClicked()
		{
			base.Interactable = false;
			OneTimeOfferBase oneTimeOffer = _request.OneTimeOffer;
			GenericPopupRequest genericPopupRequest = new GenericPopupRequest("one_time_offer_confirm_close");
			genericPopupRequest.SetTexts(Localization.Key("confirmspend.sure"), Localization.Key("limited_time_offer_close"));
			genericPopupRequest.SetDismissable(dismissable: false);
			genericPopupRequest.SetGreenButton(Localization.Key("transfer_accept_confirm_yes"), null, delegate
			{
				oneTimeOffer.IgnoreOffer();
				ForceClose();
			});
			genericPopupRequest.SetRedButton(Localization.Key("transfer_accept_confirm_no"), null, delegate
			{
				base.Interactable = true;
			});
			_popupManager.RequestPopup(genericPopupRequest);
		}

		protected override void Closed()
		{
			base.Closed();
			if (_animationRoutine != null)
			{
				StopCoroutine(_animationRoutine);
				_animationRoutine = null;
			}
			_starTweener.StopAndReset();
			_activeOfferContent.StopAnimation();
		}

		private IEnumerator AnimationRoutine()
		{
			_starTweener.Play();
			yield return new WaitWhile(() => _starTweener.IsPlaying);
			yield return _activeOfferContent.AnimateInRoutine();
		}

		private void ShowTreasureChestContent()
		{
			_activeOfferContent = _oneTimeOfferTreasureChestContent;
			_oneTimeOfferTreasureChestContent.gameObject.SetActive(value: true);
			_oneTimeOfferBuildingContent.gameObject.SetActive(value: false);
		}

		private void ShowBuildngContent()
		{
			_activeOfferContent = _oneTimeOfferBuildingContent;
			_oneTimeOfferTreasureChestContent.gameObject.SetActive(value: false);
			_oneTimeOfferBuildingContent.gameObject.SetActive(value: true);
		}

		private void OnChestBought()
		{
			ForceClose();
		}

		private void OnPurchaseStarted()
		{
			base.Interactable = false;
		}

		private void OnPurchaseCanceled()
		{
			base.Interactable = true;
		}
	}
}
