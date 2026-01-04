namespace CIG
{
	public class OneTimeOfferTreasureChest : OneTimeOfferBase
	{
		private const float DefaultDiscount = 0.5f;

		private const TreasureChestType DefaultOfferedChest = TreasureChestType.Silver;

		private readonly StorageDictionary _storage;

		private readonly TreasureChestManager _treasureChestManager;

		private readonly IAPStore<TOCIStoreProduct> _iapStore;

		private readonly OneTimeOfferTreasureChestProperties _properties;

		private float _currentDiscount;

		private double _offerLastShown;

		private double? _lastIAPMade;

		private TreasureChestType _offeredChestType = TreasureChestType.Silver;

		public const string StorageKey = "OneTimeOfferTreasureChest";

		private const string DiscountPercentageKey = "CurrentDiscountPercentageKey";

		private const string ChestTypeKey = "CurrentChestType";

		private const string OfferLastShownKey = "OfferLastShown";

		private const string LastIAPMadeKey = "LastIAPMade";

		public override float CurrentDiscountPercentage
		{
			get
			{
				return _currentDiscount;
			}
			protected set
			{
				_currentDiscount = value;
			}
		}

		public override decimal NormalPrice => DiscountedChestIAP.Price;

		public TreasureChestType ChestType
		{
			get
			{
				return _offeredChestType;
			}
			private set
			{
				if (value == TreasureChestType.Wooden)
				{
					value = TreasureChestType.Silver;
				}
				_offeredChestType = value;
				OfferedChest = _treasureChestManager.GetChest(_offeredChestType);
			}
		}

		public TreasureChest OfferedChest
		{
			get;
			private set;
		}

		public string DiscountedFormattedStorePrice
		{
			get
			{
				if (DiscountedChestIAP == null)
				{
					return string.Empty;
				}
				return DiscountedChestIAP.FormattedPrice;
			}
		}

		public string OriginalFormattedStorePrice
		{
			get
			{
				if (OriginalChestIAP == null)
				{
					return string.Empty;
				}
				return OriginalChestIAP.FormattedPrice;
			}
		}

		public Currencies KeyPrice => OfferedChest.Properties.Cost;

		public Currencies DiscountedKeyPrice => (KeyPrice * (decimal.One - (decimal)CurrentDiscountPercentage)).Round(RoundingMethod.Nearest);

		private TOCIStoreProduct OriginalChestIAP
		{
			get
			{
				string gameProductName = _treasureChestManager.GetIAPGameProductNameFromChestType(ChestType);
				return _iapStore.FindProduct((TOCIStoreProduct storeProduct) => storeProduct.Category == StoreProductCategory.Chest && storeProduct.GameProductName == gameProductName);
			}
		}

		public TOCIStoreProduct DiscountedChestIAP
		{
			get
			{
				string gameProductName = _treasureChestManager.GetIAPGameProductNameFromChestType(ChestType);
				return _iapStore.FindProduct((TOCIStoreProduct storeProduct) => storeProduct.Category == StoreProductCategory.ChestOTO && storeProduct.GameProductName == gameProductName);
			}
		}

		public OneTimeOfferTreasureChest(StorageDictionary storage, TreasureChestManager treasureManager, IAPStore<TOCIStoreProduct> iapStore, OneTimeOfferTreasureChestProperties properties)
			: base(properties)
		{
			_storage = storage;
			_treasureChestManager = treasureManager;
			_iapStore = iapStore;
			_properties = properties;
			_iapStore.PurchaseSuccessEvent += OnIAPPerformed;
			_currentDiscount = storage.Get("CurrentDiscountPercentageKey", 0.5f);
			_offerLastShown = storage.Get("OfferLastShown", 0.0);
			_lastIAPMade = storage.Get<double?>("LastIAPMade", null);
			ChestType = (TreasureChestType)storage.Get("CurrentChestType", 1);
		}

		public void Release()
		{
			_iapStore.PurchaseSuccessEvent -= OnIAPPerformed;
		}

		public override bool CanDealBeOffered(int level)
		{
			if (base.OfferEnabled && level >= _properties.MinimumLevelRequired && Timing.UtcNow - _offerLastShown >= (double)_properties.CooldownSeconds && (!_lastIAPMade.HasValue || Timing.UtcNow - _lastIAPMade.Value >= (double)_properties.IAPCooldownSeconds) && OriginalChestIAP != null)
			{
				return DiscountedChestIAP != null;
			}
			return false;
		}

		public override void IgnoreOffer()
		{
			OnOfferShown();
		}

		protected override void OnOfferShown()
		{
			switch (ChestType)
			{
			case TreasureChestType.Silver:
				ChestType = TreasureChestType.Gold;
				break;
			case TreasureChestType.Gold:
				ChestType = TreasureChestType.Platinum;
				break;
			default:
				ChestType = TreasureChestType.Silver;
				break;
			}
			_offerLastShown = Timing.UtcNow;
		}

		public void Purchase()
		{
			Analytics.OneTimeOfferTreasureChestPurchased(ChestType.ToString(), DiscountedChestIAP.FormattedPrice);
			OnOfferShown();
		}

		private void OnIAPPerformed(Purchase<TOCIStoreProduct> purchase)
		{
			_lastIAPMade = Timing.UtcNow;
		}

		public StorageDictionary Serialize()
		{
			_storage.Set("CurrentDiscountPercentageKey", _currentDiscount);
			_storage.Set("CurrentChestType", (int)_offeredChestType);
			_storage.Set("OfferLastShown", _offerLastShown);
			_storage.SetOrRemove("LastIAPMade", _lastIAPMade);
			return _storage;
		}
	}
}
