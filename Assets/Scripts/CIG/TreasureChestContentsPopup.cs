using CIG.Translation;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class TreasureChestContentsPopup : Popup
	{
		[SerializeField]
		private Image _chestImage;

		[SerializeField]
		private LocalizedText _chestTitle;

		[SerializeField]
		private TreasureChestContentsRegularElement _regularElement;

		[SerializeField]
		private TreasureChestContentsPlatinumElement _platinumElement;

		[SerializeField]
		private CurrencyView _costCurrencyView;

		[SerializeField]
		private InteractableButton _costButton;

		[SerializeField]
		private LocalizedText _iapButtonText;

		private TreasureChestManager _treasureChestManager;

		private PurchaseHandler _purchaseHandler;

		private TOCIStoreProduct _storeProduct;

		private TreasureChest _treasureChest;

		public override string AnalyticsScreenName => "treasure_chest_contents";

		public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
		{
			base.Initialize(model, canvasScaler);
			_treasureChestManager = model.Game.TreasureChestManager;
			_regularElement.Initialize(model.Game.Timing);
			_platinumElement.Initialize(model.Game.Timing);
		}

		public override void Open(PopupRequest request)
		{
			base.Open(request);
			TreasureChestContentsPopupRequest request2 = GetRequest<TreasureChestContentsPopupRequest>();
			_purchaseHandler = request2.PurchaseHandler;
			_storeProduct = request2.StoreProduct;
			_treasureChest = request2.TreasureChest;
			switch (_treasureChest.Properties.TreasureChestType)
			{
			case TreasureChestType.Silver:
			case TreasureChestType.Gold:
				_regularElement.Enable(_treasureChest);
				_platinumElement.Disable();
				break;
			case TreasureChestType.Platinum:
				_platinumElement.Enable();
				_regularElement.Disable();
				break;
			default:
				UnityEngine.Debug.LogError($"Treasure chest type '{_treasureChest.Properties.TreasureChestType}' is not supported!");
				_regularElement.Disable();
				_platinumElement.Disable();
				break;
			}
			_chestImage.sprite = SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetChestSprite(_treasureChest.Properties.TreasureChestType);
			_chestTitle.LocalizedString = _treasureChest.Name;
			_costCurrencyView.Initialize(_treasureChest.Properties.Cost.GetCurrency(0));
			_costButton.interactable = _treasureChest.CanAfford;
			_iapButtonText.LocalizedString = Localization.Literal(request2.StoreProduct.FormattedPrice);
		}

		public void OnKeysButtonClicked()
		{
			_treasureChestManager.BuyChestWithKeys(_treasureChest);
		}

		public void OnIAPButtonPressed()
		{
			_purchaseHandler.InitiatePurchase(_storeProduct);
		}

		protected override void Closed()
		{
			_regularElement.Disable();
			_platinumElement.Disable();
			base.Closed();
		}
	}
}
