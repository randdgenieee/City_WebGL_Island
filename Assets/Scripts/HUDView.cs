using CIG;
using UnityEngine;

public class HUDView : MonoBehaviour
{
	[SerializeField]
	private HUDRegionUpdater _hudRegionUpdater;

	[SerializeField]
	private HUDToggleButton _magnifyingGlassToggle;

	[SerializeField]
	private HUDToggleButton _upgradesToggle;

	[SerializeField]
	private HUDWarehouseButton _warehouseButton;

	[SerializeField]
	private HUDSaleButtons _saleButtons;

	[SerializeField]
	private HUDQuestButtons _questButtons;

	[SerializeField]
	private HUDMapButton _mapButton;

	[SerializeField]
	private HUDLevelView _levelView;

	[SerializeField]
	private HUDShopButton _shopButton;

	[SerializeField]
	private HUDKeyDealsButton _keyDealsButton;

	[SerializeField]
	private HUDCurrencyBar _goldCurrencyBar;

	[SerializeField]
	private HUDCurrencyBar _cashCurrencyBar;

	[SerializeField]
	private CraneCurrencyBarHUD _craneCurrencyBar;

	[SerializeField]
	private HUDCraneOffer _craneOffer;

	[SerializeField]
	private HUDGameStateCityProgressBars _cityProgressBars;

	[SerializeField]
	private HUDMinigamesButton _minigamesButton;

	[SerializeField]
	private RectTransform _roadsButtonMaskTransform;

	[SerializeField]
	private HUDFlyingStartDealButton _flyingStartDealButton;

	[SerializeField]
	private HUDSparkSocButton _sparkSocButton;

	private PopupManager _popupManager;

	private WorldMap _worldMap;

	private bool _isHidingHud;

	private IsometricIsland _subscribedIsland;

	public HUDWarehouseButton WarehouseButton => _warehouseButton;

	public HUDShopButton ShopButton => _shopButton;

	public RectTransform RoadsButtonMaskTransform => _roadsButtonMaskTransform;

	public HUDMapButton MapButton => _mapButton;

	public void Initialize(GameState gameState, GameStats gameStats, PopupManager popupManager, WorldMap worldMap, BuildingWarehouseManager buildingWarehouseManager, SaleManager saleManager, BuildingsManager buildingsManager, IslandsManager islandsManager, CraneManager craneManager, QuestsManager questsManager, VideoAds2Manager videoAds2Manager, TreasureChestManager treasureChestManager, ArcadeManager arcadeManager, FishingEvent fishingEvent, CraneOfferManager craneOfferManager, Properties properties, FlyingStartDealManager flyingStartDealManager, FriendsManager friendsManager)
	{
		_popupManager = popupManager;
		_worldMap = worldMap;
		_warehouseButton.Initialize(_popupManager, buildingWarehouseManager);
		_saleButtons.Initialize(saleManager);
		_questButtons.Initialize(popupManager, questsManager, fishingEvent);
		_mapButton.Initialize(gameStats, worldMap, islandsManager);
		_levelView.Initialize(gameState, _popupManager);
		_shopButton.Initialize(gameState, _popupManager, buildingsManager, treasureChestManager, properties);
		_keyDealsButton.Initialize(_popupManager);
		_goldCurrencyBar.Initialize(gameState);
		_cashCurrencyBar.Initialize(gameState);
		_craneCurrencyBar.Initialize(craneManager);
		_craneOffer.Initialize(craneOfferManager, _popupManager);
		_cityProgressBars.Initialize(gameState);
		_minigamesButton.Initialize(gameState, _popupManager, arcadeManager);
		_flyingStartDealButton.Initialize(flyingStartDealManager, popupManager);
		_sparkSocButton.Initialize(_popupManager, friendsManager);
		_worldMap.VisibilityChangedEvent += OnWorldMapVisibilityChanged;
		OnWorldMapVisibilityChanged(_worldMap.IsVisible);
		IsometricIsland.IslandLoadedEvent += OnIslandLoaded;
		if (IsometricIsland.Current != null)
		{
			OnIslandLoaded(IsometricIsland.Current);
			UpdateVisibility(_worldMap.IsVisible, IsometricIsland.Current.CinematicPlaying);
		}
		else
		{
			UpdateVisibility(_worldMap.IsVisible, islandCinematicPlaying: false);
		}
		Builder.TilesHiddenChangedEvent += OnTilesHiddenChanged;
		OnTilesHiddenChanged(Builder.TilesHidden);
		UpgradableBuilding.ToggleLevelIconEvent += OnToggleLevelIcon;
		OnToggleLevelIcon(UpgradableBuilding.LevelIconsActive);
	}

	private void OnDestroy()
	{
		if (_worldMap != null)
		{
			_worldMap.VisibilityChangedEvent -= OnWorldMapVisibilityChanged;
			_worldMap = null;
		}
		if (_subscribedIsland != null)
		{
			_subscribedIsland.CinematicPlayingChangedEvent -= OnCinematicPlayingChanged;
			_subscribedIsland = null;
		}
		Builder.TilesHiddenChangedEvent -= OnTilesHiddenChanged;
		UpgradableBuilding.ToggleLevelIconEvent -= OnToggleLevelIcon;
		IsometricIsland.IslandLoadedEvent -= OnIslandLoaded;
		_popupManager = null;
	}

	public void OnAddCashClicked()
	{
		_popupManager.RequestPopup(new ShopPopupRequest(ShopMenuTabs.Cash));
	}

	public void OnAddGoldClicked()
	{
		_popupManager.RequestPopup(new ShopPopupRequest(ShopMenuTabs.Gold));
	}

	public void OnSettingsClicked()
	{
		_popupManager.RequestPopup(new SettingsPopupRequest());
	}

	public void OnMagnifyingGlassClicked()
	{
		Builder.TilesHidden = !Builder.TilesHidden;
		SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ToggleClick);
	}

	public void OnShowUpgradesClicked()
	{
		UpgradableBuilding.ToggleLevelIcons(!UpgradableBuilding.LevelIconsActive);
		SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ToggleClick);
	}

	public void OnLeaderboardsClicked()
	{
		_popupManager.RequestPopup(new LeaderboardsPopupRequest());
	}

	public void OnRoadsClicked()
	{
		_popupManager.RequestPopup(new RoadSelectionPopupRequest());
	}

	private void OnWorldMapVisibilityChanged(bool visible)
	{
		UpdateVisibility(visible, IsometricIsland.Current != null && IsometricIsland.Current.CinematicPlaying);
	}

	private void OnTilesHiddenChanged(bool hidden)
	{
		_magnifyingGlassToggle.SetActive(hidden);
	}

	private void OnToggleLevelIcon(bool active)
	{
		_upgradesToggle.SetActive(active);
	}

	private void OnIslandLoaded(IsometricIsland island)
	{
		if (_subscribedIsland != null)
		{
			_subscribedIsland.CinematicPlayingChangedEvent -= OnCinematicPlayingChanged;
		}
		_subscribedIsland = island;
		_subscribedIsland.CinematicPlayingChangedEvent += OnCinematicPlayingChanged;
		UpdateVisibility(_worldMap.IsVisible, island.CinematicPlaying);
	}

	private void OnCinematicPlayingChanged(bool cinematicPlaying)
	{
		UpdateVisibility(_worldMap.IsVisible, cinematicPlaying);
	}

	private void UpdateVisibility(bool worldMapVisible, bool islandCinematicPlaying)
	{
		if (_isHidingHud)
		{
			_hudRegionUpdater.RequestShow(this);
			_isHidingHud = false;
		}
		if (islandCinematicPlaying)
		{
			_hudRegionUpdater.RequestHide(this, HUDRegionType.All);
			_isHidingHud = true;
		}
		else if (worldMapVisible)
		{
			_hudRegionUpdater.RequestHide(this, HUDRegionType.Quests | HUDRegionType.ShopButton | HUDRegionType.RoadsButton | HUDRegionType.MapButton | HUDRegionType.MinigamesButton | HUDRegionType.LeaderboardButton | HUDRegionType.SocialButton | HUDRegionType.MagnifyingGlassButton | HUDRegionType.UpgradesButton | HUDRegionType.KeyDealsButton | HUDRegionType.WarehouseButton);
			_isHidingHud = true;
		}
	}
}
