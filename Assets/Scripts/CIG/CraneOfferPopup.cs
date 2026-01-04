using CIG.Translation;
using System;
using System.Collections;
using Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class CraneOfferPopup : IAPPopup
	{
		[SerializeField]
		private Tweener _starTweener;

		[SerializeField]
		private LocalizedText _craneCountText;

		[SerializeField]
		private LocalizedText _timerText;

		[SerializeField]
		private LocalizedText _iapPriceText;

		[SerializeField]
		private InteractableButton _buyButton;

		[SerializeField]
		private Button _backgroundButton;

		[SerializeField]
		private CurrencyView _currencyView;

		[SerializeField]
		private GameObject _4CranesContainer;

		private CraneOfferManager _craneOfferManager;

		private GameState _gameState;

		private CraneOffer _craneOffer;

		private IEnumerator _updateRemainingTimeRoutine;

		public override string AnalyticsScreenName => "crane_offer";

		public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
		{
			base.Initialize(model, canvasScaler);
			_craneOfferManager = model.Game.CraneOfferManager;
			_gameState = model.Game.GameState;
		}

		public override void Open(PopupRequest request)
		{
			base.Open(request);
			if (!_craneOfferManager.HasCraneOffer)
			{
				UnityEngine.Debug.LogError("There is no crane offer active, so the crane offer popup can't be opened.");
				OnCloseClicked();
				return;
			}
			_craneOffer = GetRequest<CraneOfferPopupRequest>().CraneOffer;
			_craneCountText.LocalizedString = Localization.Format(Localization.Key("cranes$n"), Localization.Integer(_craneOffer.Cranes));
			_iapPriceText.gameObject.SetActive(_craneOffer is CraneOfferIAP);
			_currencyView.gameObject.SetActive(_craneOffer is CraneOfferCurrency);
			CraneOffer craneOffer = _craneOffer;
			if (craneOffer != null)
			{
				CraneOfferIAP craneOfferIAP;
				if ((craneOfferIAP = (craneOffer as CraneOfferIAP)) == null)
				{
					CraneOfferCurrency craneOfferCurrency;
					if ((craneOfferCurrency = (craneOffer as CraneOfferCurrency)) != null)
					{
						Currency currency = craneOfferCurrency.Price.GetCurrency(0);
						_currencyView.Initialize(currency);
						InteractableButton buyButton = _buyButton;
						bool interactable = _backgroundButton.interactable = (_gameState.CanAfford(currency) || _gameState.CanShowMoreCashGold(currency));
						buyButton.interactable = interactable;
					}
				}
				else
				{
					CraneOfferIAP craneOfferIAP2 = craneOfferIAP;
					_iapPriceText.LocalizedString = Localization.Literal(craneOfferIAP2.Product.FormattedPrice);
					InteractableButton buyButton2 = _buyButton;
					bool interactable = _backgroundButton.interactable = true;
					buyButton2.interactable = interactable;
				}
			}
			_4CranesContainer.SetActive(_craneOffer.Cranes >= 4);
			_starTweener.Play();
			StartCoroutine(_updateRemainingTimeRoutine = UpdateRemainingTimeRoutine(_craneOffer));
		}

		public void OnItemClicked()
		{
			if (_craneOffer == null)
			{
				return;
			}
			CraneOffer craneOffer = _craneOffer;
			if (craneOffer == null)
			{
				return;
			}
			CraneOfferIAP craneOfferIAP;
			if ((craneOfferIAP = (craneOffer as CraneOfferIAP)) == null)
			{
				CraneOfferCurrency craneOfferCurrency;
				if ((craneOfferCurrency = (craneOffer as CraneOfferCurrency)) != null)
				{
					CraneOfferCurrency craneOfferCurrency2 = craneOfferCurrency;
					_gameState.SpendCurrencies(craneOfferCurrency2.Price, CurrenciesSpentReason.CraneOffer, delegate(bool succes, Currencies spent)
					{
						if (succes)
						{
							craneOfferCurrency2.RewardOffer();
							OnCloseClicked();
						}
					});
				}
			}
			else
			{
				CraneOfferIAP craneOfferIAP2 = craneOfferIAP;
				_purchaseHandler.InitiatePurchase(craneOfferIAP2.Product);
			}
		}

		protected override void Closed()
		{
			base.Closed();
			if (_updateRemainingTimeRoutine != null)
			{
				StopCoroutine(_updateRemainingTimeRoutine);
				_updateRemainingTimeRoutine = null;
			}
			_starTweener.StopAndReset();
			_craneOffer = null;
		}

		private IEnumerator UpdateRemainingTimeRoutine(CraneOffer craneOffer)
		{
			while (true)
			{
				_timerText.LocalizedString = Localization.TimeSpan((craneOffer.TimeRemaining > TimeSpan.Zero) ? craneOffer.TimeRemaining : TimeSpan.Zero, hideSecondPartWhenZero: false);
				yield return new WaitForSecondsRealtime(1f);
			}
		}
	}
}
