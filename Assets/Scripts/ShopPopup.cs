using CIG;
using CIG.Translation;
using SparkLinq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPopup : IAPPopup
{
	public delegate void TabChangedHandler(ShopMenuTabs tab);

	private struct CachedScrollPosition
	{
		public float ContentWidth
		{
			get;
		}

		public float NormalizedHorizontalPosition
		{
			get;
		}

		public CachedScrollPosition(float contentWidth, float normalizedHorizontalPosition)
		{
			ContentWidth = contentWidth;
			NormalizedHorizontalPosition = normalizedHorizontalPosition;
		}
	}

	private static readonly Dictionary<ShopMenuTabs, ShopType> TabIndexToShopTypeMapping = new Dictionary<ShopMenuTabs, ShopType>
	{
		{
			ShopMenuTabs.Gold,
			ShopType.Shop_gold
		},
		{
			ShopMenuTabs.Cash,
			ShopType.Shop_cash
		},
		{
			ShopMenuTabs.Chests,
			ShopType.Shop_chests
		},
		{
			ShopMenuTabs.Landmarks,
			ShopType.Shop_landmarks
		},
		{
			ShopMenuTabs.Residential,
			ShopType.Shop_residential
		},
		{
			ShopMenuTabs.Commercial,
			ShopType.Shop_commercial
		},
		{
			ShopMenuTabs.Community,
			ShopType.Shop_community
		},
		{
			ShopMenuTabs.Decorations,
			ShopType.Shop_decorations
		}
	};

	private static readonly ShopType[] IAPShopCategories = new ShopType[6]
	{
		ShopType.Shop_gold,
		ShopType.Shop_goldSale,
		ShopType.Shop_cash,
		ShopType.Shop_cashSale,
		ShopType.Shop_chests,
		ShopType.Shop_landmarks
	};

	private static readonly ShopMenuTabs[] DisableOnWorldMapTabs = new ShopMenuTabs[4]
	{
		ShopMenuTabs.Residential,
		ShopMenuTabs.Commercial,
		ShopMenuTabs.Community,
		ShopMenuTabs.Decorations
	};

	[SerializeField]
	private TabView _tabView;

	[SerializeField]
	private ScrollRect _scrollRect;

	[SerializeField]
	private Image _treasureHeaderImage;

	[SerializeField]
	private Image _buildHeaderImage;

	[SerializeField]
	private LocalizedText _headerTitle;

	[SerializeField]
	private GameObject _regularCurrencies;

	[SerializeField]
	private LocalizedText _cashText;

	[SerializeField]
	private LocalizedText _goldText;

	[SerializeField]
	private LocalizedText _craneText;

	[SerializeField]
	private DateTimeTimerView _saleTimer;

	[SerializeField]
	[Header("Iaps")]
	private GameObject _goldSaleIcon;

	[SerializeField]
	private GameObject _cashSaleIcon;

	[SerializeField]
	private CurrencyIAPShopItem _currencyIapPrefab;

	[SerializeField]
	private LandmarkIAPShopItem _landmarkIapPrefab;

	[SerializeField]
	private RectTransform _currencyIapItemParent;

	[SerializeField]
	private RectTransform _landmarkIapItemParent;

	[SerializeField]
	private GameObject _landmarkIapTab;

	[SerializeField]
	[Header("Buildings")]
	private RecyclerGridLayoutGroup _recyclerView;

	[SerializeField]
	private RectTransform _buildingItemParent;

	[SerializeField]
	private NewBuildingsBadge _residentialBuildingsBadge;

	[SerializeField]
	private NewBuildingsBadge _commercialBuildingsBadge;

	[SerializeField]
	private NewBuildingsBadge _communityBuildingsBadge;

	[SerializeField]
	[Header("Chests")]
	private GameObject _chestsBadge;

	[SerializeField]
	private GameObject _keyCurrencies;

	[SerializeField]
	private LocalizedText _silverKeysText;

	[SerializeField]
	private LocalizedText _goldKeysText;

	[SerializeField]
	private Button _videoButton;

	[SerializeField]
	private RegularChestShopItem _regularChestPrefab;

	[SerializeField]
	private WoodenChestShopItem _woodenChestPrefab;

	[SerializeField]
	private RectTransform _chestItemParent;

	[SerializeField]
	[Header("Tutorial")]
	[Tooltip("Should be mapped to the ShopMenuTabs enum")]
	private RectTransform[] _tutorialShopMenuTabTargets;

	private readonly Dictionary<ShopType, List<ShopItem>> _sortedStoreProductViews = new Dictionary<ShopType, List<ShopItem>>();

	private readonly Dictionary<GameObject, BuildingShopItem> _buildingShopItemViews = new Dictionary<GameObject, BuildingShopItem>();

	private readonly Dictionary<int, CachedScrollPosition> _lastScrollPositionsPerTab = new Dictionary<int, CachedScrollPosition>();

	private TreasureChestManager _treasureChestManager;

	private GameState _gameState;

	private GameStats _gameStats;

	private BuildingWarehouseManager _buildingWarehouseManager;

	private CraneManager _craneManager;

	private VideoAds1Manager _videoAds1Manager;

	private WorldMap _worldMap;

	private SaleManager _saleManager;

	private BuildingsManager _buildingsManager;

	private IAPStore<TOCIStoreProduct> _iapStore;

	private Properties _properties;

	private List<BuildingProperties> _shopBuildings = new List<BuildingProperties>();

	private int _firstLockedIndex = int.MaxValue;

	private ShopMenuTabs _lastOpenedTab;

	private ShopType _currentCategory;

	private readonly List<string> _badgeShopItems = new List<string>();

	public override string AnalyticsScreenName => $"shop_{_currentCategory.ToString()}";

	public ShopMenuTabs LastOpenedTab => _lastOpenedTab;

	public RectTransform FirstShopItemTransform
	{
		get
		{
			using (Dictionary<GameObject, BuildingShopItem>.Enumerator enumerator = _buildingShopItemViews.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return (RectTransform)enumerator.Current.Key.transform;
				}
			}
			return null;
		}
	}

	public event TabChangedHandler TabChangedEvent;

	private void FireTabChangedEvent(ShopMenuTabs tab)
	{
		this.TabChangedEvent?.Invoke(tab);
	}

	protected override void OnDestroy()
	{
		_tabView.TabIndexChangedEvent -= OnTabIndexChanged;
		if (_iapStore != null)
		{
			_iapStore.InitializedEvent -= OnStoreInitialized;
			_iapStore = null;
		}
		if (_gameState != null)
		{
			_gameState.BalanceChangedEvent -= OnBalanceChanged;
			_gameState = null;
		}
		if (_craneManager != null)
		{
			_craneManager.BuildCountChangedEvent -= OnBuildCountChanged;
			_craneManager = null;
		}
		if (_videoAds1Manager != null)
		{
			_videoAds1Manager.AvailabilityChangedEvent -= OnVideoAvailabilityChanged;
			_videoAds1Manager = null;
		}
		if (_treasureChestManager != null)
		{
			_treasureChestManager.ChestOpenableChangedEvent -= OnChestOpenableChanged;
			_treasureChestManager = null;
		}
		if (SingletonMonobehaviour<FPSLimiter>.IsAvailable)
		{
			SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
		}
		base.OnDestroy();
	}

	public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
	{
		base.Initialize(model, canvasScaler);
		_iapStore = model.GameServer.IAPStore;
		_craneManager = model.Game.CraneManager;
		_gameState = model.Game.GameState;
		_gameStats = model.Game.GameStats;
		_buildingWarehouseManager = model.Game.BuildingWarehouseManager;
		_videoAds1Manager = model.Game.VideoAds1Manager;
		_treasureChestManager = model.Game.TreasureChestManager;
		_saleManager = model.Game.SaleManager;
		_worldMap = model.Game.WorldMap;
		_buildingsManager = model.Game.BuildingsManager;
		_properties = model.Game.Properties;
		IAPStoreError loadingError = _iapStore.LoadingError;
		if ((uint)(loadingError - 1) <= 1u)
		{
			_iapStore.InitializedEvent += OnStoreInitialized;
		}
		_communityBuildingsBadge.Initialize(_gameState);
		_residentialBuildingsBadge.Initialize(_gameState);
		_commercialBuildingsBadge.Initialize(_gameState);
		UpdateIapItems();
		_tabView.TabIndexChangedEvent += OnTabIndexChanged;
	}

	public override void Open(PopupRequest request)
	{
		base.Open(request);
		SingletonMonobehaviour<FPSLimiter>.Instance.PushUnlimitedFPSRequest(this);
		_landmarkIapTab.SetActive(_properties.FeatureFlagProperties.LandmarkIAPsEnabled);
		_gameState.BalanceChangedEvent += OnBalanceChanged;
		OnBalanceChanged(_gameState.Balance, _gameState.Balance, new FlyingCurrenciesData());
		_craneManager.BuildCountChangedEvent += OnBuildCountChanged;
		OnBuildCountChanged(_craneManager.CurrentBuildCount, _craneManager.MaxBuildCount);
		SetSaleIcons(_saleManager.HasGoldSale, _saleManager.HasCashSale);
		_treasureChestManager.ChestOpenableChangedEvent += OnChestOpenableChanged;
		OnChestOpenableChanged();
		_videoAds1Manager.AvailabilityChangedEvent += OnVideoAvailabilityChanged;
		OnVideoAvailabilityChanged();
		int i = 0;
		for (int num = DisableOnWorldMapTabs.Length; i < num; i++)
		{
			_tabView.SetTabVisible((int)DisableOnWorldMapTabs[i], !_worldMap.IsVisible);
		}
		ShopMenuTabs shopMenuTabs = GetRequest<ShopPopupRequest>().Tab ?? _lastOpenedTab;
		_tabView.SetActiveTab((int)shopMenuTabs);
		Show(shopMenuTabs);
	}

	public override void Close(bool instant)
	{
		if (_tabView.ActiveTabIndex.HasValue)
		{
			_lastScrollPositionsPerTab[_tabView.ActiveTabIndex.Value] = new CachedScrollPosition(_scrollRect.content.rect.width, _scrollRect.horizontalNormalizedPosition);
		}
		_gameState.BalanceChangedEvent -= OnBalanceChanged;
		_craneManager.BuildCountChangedEvent -= OnBuildCountChanged;
		_videoAds1Manager.AvailabilityChangedEvent -= OnVideoAvailabilityChanged;
		_treasureChestManager.ChestOpenableChangedEvent -= OnChestOpenableChanged;
		_badgeShopItems.Clear();
		base.Close(instant);
	}

	public void Show(ShopMenuTabs shopMenuTab)
	{
		SetNewBuildingsBadges(_buildingsManager.ViewedBuildingHistory);
		SwitchContent(shopMenuTab);
		_currentCategory = TabIndexToShopTypeMapping[shopMenuTab];
		switch (_currentCategory)
		{
		case ShopType.Shop_cash:
			if (_saleManager.HasCashSale)
			{
				_currentCategory = ShopType.Shop_cashSale;
			}
			break;
		case ShopType.Shop_gold:
			if (_saleManager.HasGoldSale)
			{
				_currentCategory = ShopType.Shop_goldSale;
			}
			break;
		}
		ShowItems(_currentCategory);
		_lastOpenedTab = shopMenuTab;
		FireTabChangedEvent(shopMenuTab);
	}

	public void SetScrollingEnabled(bool scrollingEnabled)
	{
		_scrollRect.horizontal = scrollingEnabled;
	}

	public void SetScrollPosition(float position)
	{
		_scrollRect.horizontalNormalizedPosition = position;
	}

	public RectTransform GetTutorialTabTarget(ShopMenuTabs tab)
	{
		return _tutorialShopMenuTabTargets[(int)tab];
	}

	public List<ShopItem> GetShopItemViews(ShopType shopType)
	{
		return _sortedStoreProductViews[shopType];
	}

	public void OnWatchVideoClicked()
	{
		_videoAds1Manager.WatchVideoForKeys();
	}

	protected override void Closed()
	{
		SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
		_recyclerView.PushInstances();
		int i = 0;
		for (int num = IAPShopCategories.Length; i < num; i++)
		{
			List<ShopItem> list = _sortedStoreProductViews[IAPShopCategories[i]];
			int j = 0;
			for (int count = list.Count; j < count; j++)
			{
				list[j].SetVisible(visible: false);
			}
		}
		base.Closed();
	}

	private void SwitchContent(ShopMenuTabs activeTab)
	{
		_regularCurrencies.SetActive(activeTab != ShopMenuTabs.Chests);
		_keyCurrencies.SetActive(activeTab == ShopMenuTabs.Chests);
		if (activeTab == ShopMenuTabs.Cash || activeTab == ShopMenuTabs.Gold || activeTab == ShopMenuTabs.Chests || activeTab == ShopMenuTabs.Landmarks)
		{
			_currencyIapItemParent.gameObject.SetActive(activeTab == ShopMenuTabs.Cash || activeTab == ShopMenuTabs.Gold);
			_chestItemParent.gameObject.SetActive(activeTab == ShopMenuTabs.Chests);
			_landmarkIapItemParent.gameObject.SetActive(activeTab == ShopMenuTabs.Landmarks);
			_buildingItemParent.gameObject.SetActive(value: false);
			_scrollRect.content = ((activeTab == ShopMenuTabs.Landmarks) ? _landmarkIapItemParent : _currencyIapItemParent);
			_treasureHeaderImage.gameObject.SetActive(value: true);
			_buildHeaderImage.gameObject.SetActive(value: false);
			_headerTitle.LocalizedString = Localization.Key("treasures");
			if ((_saleManager.HasCashSale && activeTab == ShopMenuTabs.Cash) || (_saleManager.HasGoldSale && activeTab == ShopMenuTabs.Gold))
			{
				_saleTimer.StartTimer(_saleManager.CurrentSale.Expiration);
			}
			else
			{
				_saleTimer.StopTimer();
			}
		}
		else
		{
			_currencyIapItemParent.gameObject.SetActive(value: false);
			_chestItemParent.gameObject.SetActive(value: false);
			_landmarkIapItemParent.gameObject.SetActive(value: false);
			_buildingItemParent.gameObject.SetActive(value: true);
			_scrollRect.content = _buildingItemParent;
			_treasureHeaderImage.gameObject.SetActive(value: false);
			_buildHeaderImage.gameObject.SetActive(value: true);
			_headerTitle.LocalizedString = Localization.Key("build");
			_saleTimer.StopTimer();
		}
		float scrollPosition = 0f;
		if (_tabView.ActiveTabIndex.HasValue && _lastScrollPositionsPerTab.TryGetValue(_tabView.ActiveTabIndex.Value, out CachedScrollPosition value))
		{
			scrollPosition = value.ContentWidth / _scrollRect.content.rect.width * value.NormalizedHorizontalPosition;
		}
		SetScrollPosition(scrollPosition);
	}

	private void ShowItems(ShopType category)
	{
		switch (category)
		{
		case ShopType.Shop_gold:
		case ShopType.Shop_goldSale:
		case ShopType.Shop_cash:
		case ShopType.Shop_cashSale:
		case ShopType.Shop_chests:
		case ShopType.Shop_landmarks:
		{
			ShopType[] iAPShopCategories = IAPShopCategories;
			foreach (ShopType shopType in iAPShopCategories)
			{
				List<ShopItem> list = _sortedStoreProductViews[shopType];
				int j = 0;
				for (int count = list.Count; j < count; j++)
				{
					list[j].SetVisible(shopType == category);
				}
			}
			_gameStats.CurrencyMenuWatched(1);
			break;
		}
		case ShopType.Shop_residential:
		case ShopType.Shop_commercial:
		case ShopType.Shop_community:
		case ShopType.Shop_decorations:
			ShowBuildings(category);
			break;
		}
	}

	private void SetSaleIcons(bool hasGoldSale, bool hasCashSale)
	{
		_goldSaleIcon.SetActive(hasGoldSale);
		_cashSaleIcon.SetActive(hasCashSale);
	}

	private void UpdateIapItems()
	{
		foreach (KeyValuePair<ShopType, List<ShopItem>> sortedStoreProductView in _sortedStoreProductViews)
		{
			int count = sortedStoreProductView.Value.Count;
			for (int i = 0; i < count; i++)
			{
				UnityEngine.Object.Destroy(sortedStoreProductView.Value[i].gameObject);
			}
		}
		_sortedStoreProductViews.Clear();
		TOCIStoreProduct[] products = _iapStore.GetProducts((TOCIStoreProduct p) => p.Category == StoreProductCategory.Shop && p.Currencies.Contains("Gold"));
		decimal lowestAmountPerPrice = (products.Length != 0) ? GetAmountPerPrice(products[0]) : decimal.Zero;
		CreateIapItems(products, ShopType.Shop_gold, isSale: false, lowestAmountPerPrice);
		CreateIapItems(_iapStore.GetProducts((TOCIStoreProduct p) => p.Category == StoreProductCategory.ShopSale && p.Currencies.Contains("Gold")), ShopType.Shop_goldSale, isSale: true, lowestAmountPerPrice);
		TOCIStoreProduct[] products2 = _iapStore.GetProducts((TOCIStoreProduct p) => p.Category == StoreProductCategory.Shop && p.Currencies.Contains("Cash"));
		decimal lowestAmountPerPrice2 = (products2.Length != 0) ? GetAmountPerPrice(products2[0]) : decimal.Zero;
		CreateIapItems(products2, ShopType.Shop_cash, isSale: false, lowestAmountPerPrice2);
		CreateIapItems(_iapStore.GetProducts((TOCIStoreProduct p) => p.Category == StoreProductCategory.ShopSale && p.Currencies.Contains("Cash")), ShopType.Shop_cashSale, isSale: true, lowestAmountPerPrice2);
		List<ShopItem> list = new List<ShopItem>();
		WoodenChestShopItem woodenChestShopItem = UnityEngine.Object.Instantiate(_woodenChestPrefab, _chestItemParent);
		woodenChestShopItem.Initialize(_treasureChestManager, OnWoodenChestClick);
		woodenChestShopItem.SetVisible(visible: false);
		list.Add(woodenChestShopItem);
		int j = 0;
		for (int num = TreasureChestManager.PaidChestTypes.Length; j < num; j++)
		{
			TreasureChestType type = TreasureChestManager.PaidChestTypes[j];
			TreasureChest chest = _treasureChestManager.GetChest(type);
			TOCIStoreProduct tOCIStoreProduct = _iapStore.FindProduct((TOCIStoreProduct x) => x.Category == StoreProductCategory.Chest && x.GameProductName == chest.Properties.IAPGameProductName);
			if (tOCIStoreProduct != null)
			{
				RegularChestShopItem regularChestShopItem = UnityEngine.Object.Instantiate(_regularChestPrefab, _chestItemParent);
				regularChestShopItem.Initialize(_gameState, chest, OnIAPItemClick, OnChestKeysClick, OnChestContentsClick, tOCIStoreProduct);
				regularChestShopItem.SetVisible(visible: false);
				list.Add(regularChestShopItem);
			}
			else
			{
				UnityEngine.Debug.LogWarning("Cannot find chest IAP with game product name '" + chest.Properties.IAPGameProductName + "'.");
			}
		}
		_sortedStoreProductViews.Add(ShopType.Shop_chests, list);
		List<ShopItem> list2 = new List<ShopItem>();
		TOCIStoreProduct[] products3 = _iapStore.GetProducts((TOCIStoreProduct x) => x.Category == StoreProductCategory.Landmark);
		List<BuildingProperties> list3 = _properties.AllLandmarkBuildings.ToList();
		list3.Sort((BuildingProperties a, BuildingProperties b) => string.CompareOrdinal(a.LocalizedName.Translate(), b.LocalizedName.Translate()));
		int k = 0;
		for (int count2 = list3.Count; k < count2; k++)
		{
			BuildingProperties properties = list3[k];
			TOCIStoreProduct tOCIStoreProduct2 = products3.Find((TOCIStoreProduct product) => product.LandmarkName == properties.BaseKey);
			if (tOCIStoreProduct2 != null)
			{
				LandmarkIAPShopItem landmarkIAPShopItem = UnityEngine.Object.Instantiate(_landmarkIapPrefab, _landmarkIapItemParent);
				landmarkIAPShopItem.Initialize(_gameStats, _buildingWarehouseManager, _popupManager, properties, tOCIStoreProduct2, OnIAPItemClick);
				landmarkIAPShopItem.SetVisible(visible: false);
				list2.Add(landmarkIAPShopItem);
			}
			else
			{
				UnityEngine.Debug.LogWarning("Cannot find landmark IAP for landmark with name '" + properties.BaseKey + "'.");
			}
		}
		_sortedStoreProductViews.Add(ShopType.Shop_landmarks, list2);
		if (base.IsOpen)
		{
			Show(LastOpenedTab);
		}
	}

	private decimal GetAmountPerPrice(TOCIStoreProduct product)
	{
		decimal num = Math.Round(product.Price);
		decimal sumValues = product.Currencies.SumValues;
		if (!(num > decimal.Zero))
		{
			return decimal.Zero;
		}
		return sumValues / num;
	}

	private void CreateIapItems(TOCIStoreProduct[] products, ShopType category, bool isSale, decimal lowestAmountPerPrice)
	{
		List<ShopItem> list = new List<ShopItem>();
		for (int num = products.Length - 1; num >= 0; num--)
		{
			TOCIStoreProduct product = products[num];
			decimal amountPerPrice = GetAmountPerPrice(product);
			decimal valuePercentage = (lowestAmountPerPrice > decimal.Zero) ? (amountPerPrice / lowestAmountPerPrice) : decimal.Zero;
			CurrencyIAPShopItem currencyIAPShopItem = UnityEngine.Object.Instantiate(_currencyIapPrefab, _currencyIapItemParent);
			currencyIAPShopItem.transform.localScale = new Vector3(-1f, 1f, 1f);
			currencyIAPShopItem.Initialize(product, category, num, valuePercentage, OnIAPItemClick, isSale);
			currencyIAPShopItem.SetVisible(visible: false);
			list.Add(currencyIAPShopItem);
		}
		_sortedStoreProductViews.Add(category, list);
	}

	private void ShowBuildings(ShopType category)
	{
		_recyclerView.PushInstances();
		_shopBuildings.Clear();
		_badgeShopItems.Clear();
		List<BuildingProperties> buildings;
		switch (category)
		{
		case ShopType.Shop_residential:
			buildings = _properties.AllResidentialBuildings;
			break;
		case ShopType.Shop_commercial:
			buildings = _properties.AllCommercialBuildings;
			break;
		case ShopType.Shop_community:
			buildings = _properties.AllCommunityBuildings;
			break;
		default:
			buildings = _properties.AllDecorationBuildings;
			break;
		}
		_shopBuildings = FilterBuildings(buildings);
		_recyclerView.Init(_shopBuildings.Count, InitBuildingItem);
	}

	private BuildingShopItem GetBuildingShopItemView(GameObject instance)
	{
		if (!_buildingShopItemViews.ContainsKey(instance))
		{
			_buildingShopItemViews[instance] = instance.GetComponent<BuildingShopItem>();
		}
		return _buildingShopItemViews[instance];
	}

	private bool InitBuildingItem(GameObject go, int index)
	{
		if (index < 0 || index >= _shopBuildings.Count)
		{
			return false;
		}
		BuildingShopItem buildingShopItemView = GetBuildingShopItemView(go);
		BuildingProperties buildingProperties = _shopBuildings[index];
		bool flag = index >= _firstLockedIndex;
		bool showNewItem = false;
		if (!flag && !(buildingProperties is DecorationBuildingProperties))
		{
			if (_buildingsManager.AddViewedBuilding(buildingProperties.BaseKey))
			{
				_badgeShopItems.Add(buildingProperties.BaseKey);
				showNewItem = true;
			}
			else if (_badgeShopItems.Contains(buildingProperties.BaseKey))
			{
				showNewItem = true;
			}
		}
		buildingShopItemView.Init(buildingProperties, buildingProperties.GetConstructionCost(_gameState, _gameStats, _buildingWarehouseManager), _gameState.Level, flag, showNewItem, OnBuildingItemClick);
		return true;
	}

	private void OnTabIndexChanged(int? oldIndex, int newIndex)
	{
		if (oldIndex.HasValue)
		{
			_lastScrollPositionsPerTab[oldIndex.Value] = new CachedScrollPosition(_scrollRect.content.rect.width, _scrollRect.horizontalNormalizedPosition);
		}
		ShopMenuTabs shopMenuTab = (ShopMenuTabs)newIndex;
		Show(shopMenuTab);
		Analytics.ShopTabClicked(shopMenuTab.ToString());
		PushScreenView();
	}

	private void OnStoreInitialized()
	{
		_iapStore.InitializedEvent -= OnStoreInitialized;
		UpdateIapItems();
	}

	private List<BuildingProperties> FilterBuildings(List<BuildingProperties> buildings)
	{
		int currentLevel = _gameState.Level;
		int maxLevel = _gameState.MaxLevel;
		bool cashBuildingsForGoldAfterMaxEnabled = _properties.FeatureFlagProperties.CashBuildingsForGoldAfterMaxEnabled;
		List<BuildingProperties> list = new List<BuildingProperties>();
		List<BuildingProperties> list2 = new List<BuildingProperties>();
		int i = 0;
		for (int count = buildings.Count; i < count; i++)
		{
			BuildingProperties buildingProperties = buildings[i];
			if (!buildingProperties.Activatable && buildingProperties.UnlockLevels.First() <= maxLevel)
			{
				if (buildingProperties.IsUnlocked(currentLevel) && (cashBuildingsForGoldAfterMaxEnabled || buildingProperties.CanBeBuilt(currentLevel, _gameStats, _buildingWarehouseManager)))
				{
					list.Add(buildingProperties);
				}
				if (buildingProperties.GetNextUnlockLevel(currentLevel) != int.MaxValue)
				{
					list2.Add(buildingProperties);
				}
			}
		}
		_firstLockedIndex = list.Count;
		list.Sort(delegate(BuildingProperties a, BuildingProperties b)
		{
			int num = a.UnlockLevels.First();
			int num2 = b.UnlockLevels.First();
			return num - num2;
		});
		list2.Sort((BuildingProperties a, BuildingProperties b) => a.GetNextUnlockLevel(currentLevel) - b.GetNextUnlockLevel(currentLevel));
		list.AddRange(list2);
		return list;
	}

	private void OnIAPItemClick(TOCIStoreProduct product)
	{
		_purchaseHandler.InitiatePurchase(product);
	}

	private void OnBuildingItemClick(BuildingProperties buildingProperties)
	{
		_popupManager.RequestPopup(new BuildingPopupRequest(buildingProperties, BuildingPopupContent.Preview));
	}

	private void OnWoodenChestClick()
	{
		_treasureChestManager.OpenWoodenChest();
	}

	private void OnChestKeysClick(TreasureChest chest)
	{
		Currency currency = chest.Properties.Cost.WithoutEmpty().GetCurrency(0);
		GenericPopupRequest request = new GenericPopupRequest("buy_chest_confirm").SetTexts(chest.Name, Localization.Format(Localization.Key("buy_chest_confirm"), chest.Name)).SetIcon(SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetChestSprite(chest.Properties.TreasureChestType)).SetGreenButton(Localization.Integer(currency.Value), null, delegate
		{
			_treasureChestManager.BuyChestWithKeys(chest);
		}, SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(currency))
			.SetRedButton(Localization.Key("cancel"));
		_popupManager.RequestPopup(request);
	}

	private void OnChestContentsClick(TreasureChest treasureChest, TOCIStoreProduct storeProduct)
	{
		_popupManager.RequestPopup(new TreasureChestContentsPopupRequest(treasureChest, _purchaseHandler, storeProduct));
	}

	private void OnBalanceChanged(Currencies oldBalance, Currencies newBalance, FlyingCurrenciesData flyingCurrenciesData)
	{
		_cashText.LocalizedString = Localization.Integer(newBalance.GetValue("Cash"));
		_goldText.LocalizedString = Localization.Integer(newBalance.GetValue("Gold"));
		_silverKeysText.LocalizedString = Localization.Integer(newBalance.GetValue("SilverKey"));
		_goldKeysText.LocalizedString = Localization.Integer(newBalance.GetValue("GoldKey"));
	}

	private void OnBuildCountChanged(int used, int total)
	{
		int i = Mathf.Max(total - used, 0);
		_craneText.LocalizedString = Localization.Format("{0}/{1}", Localization.Integer(i), Localization.Integer(total));
	}

	private void OnChestOpenableChanged()
	{
		_chestsBadge.SetActive(_treasureChestManager.HasOpenableChest);
	}

	private void OnVideoAvailabilityChanged()
	{
		_videoButton.interactable = _videoAds1Manager.IsReady;
	}

	private void SetNewBuildingsBadges(List<string> viewedBuildingHistory)
	{
		_residentialBuildingsBadge.UpdateNewBuildingsBadge(new List<BuildingProperties>(_properties.AllResidentialBuildings), viewedBuildingHistory);
		_commercialBuildingsBadge.UpdateNewBuildingsBadge(new List<BuildingProperties>(_properties.AllCommercialBuildings), viewedBuildingHistory);
		_communityBuildingsBadge.UpdateNewBuildingsBadge(new List<BuildingProperties>(_properties.AllCommunityBuildings), viewedBuildingHistory);
	}
}
