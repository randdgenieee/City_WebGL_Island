using UnityEngine;

namespace CIG
{
	public class IslandBootstrapper : MonoBehaviour
	{
		[SerializeField]
		private IslandSetup _islandSetup;

		[SerializeField]
		private IsometricIsland _isometricIsland;

		[SerializeField]
		private Builder _builder;

		[SerializeField]
		private RoadBuilder _roadBuilder;

		[SerializeField]
		private CIGExpansions _expansions;

		[SerializeField]
		private CIGSceneryManager _sceneryManager;

		[SerializeField]
		private CIGIslandState _islandState;

		[SerializeField]
		private AirshipPlatformManager _airshipPlatformManager;

		[SerializeField]
		private RoadAgentSpawner _roadAgentSpawner;

		[SerializeField]
		private WalkerBalloonSpawner _walkerBalloonSpawner;

		[SerializeField]
		private WeatherManager _weatherManager;

		[SerializeField]
		private FishingLocationManager _fishingLocationManager;

		[SerializeField]
		private BackgroundLoader _backgroundLoader;

		[SerializeField]
		private IsometricGrid _isometricGrid;

		[SerializeField]
		private GridOverlay _gridOverlay;

		[SerializeField]
		private GridColorManager _gridColorManager;

		[SerializeField]
		private IslandCameraOperator _cameraOperator;

		[SerializeField]
		private IslandInput _islandInput;

		[SerializeField]
		private OverlayManager _overlayManager;

		private string _screenViewName;

		public const string IslandStateStorageKey = "IslandState";

		public const string IsometricGridStorageKey = "IsometricGrid";

		private const string ExpansionsStorageKey = "Expansions";

		private const string SceneryManagerStorageKey = "SceneryManager";

		private const string AirshipPlatformManagerStorageKey = "AirshipPlatformManager";

		private const string WeatherManagerStorageKey = "WeatherManager";

		public IsometricIsland IsometricIsland => _isometricIsland;

		public void Initialize(GameStats gameStats, GameState gameState, ExpansionCostManager expansionCostManager, LocalNotificationManager localNotificationManager, BuildingWarehouseManager buildingWarehouseManager, VideoAds1Manager videoAds1Manager, VideoAds2Manager videoAds2Manager, CityAdvisor cityAdvisor, TreasureChestManager treasureChestManager, TutorialManager tutorialManager, CurrencyConversionManager currencyConversionManager, CraneManager craneManager, BuildingsManager buildingsManager, IslandsManagerView islandsManagerView, SaleManager saleManager, WorldMap worldMap, PopupManager popupManager, WebService webService, Timing timing, Properties properties, FishingEvent fishingEvent, RoutineRunner routineRunner)
		{
			StorageDictionary storageDict = StorageController.GameRoot.GetStorageDict(GetIslandStorageKey(_islandSetup.IslandId));
			_backgroundLoader.Initialize(_islandSetup.BackgroundImageBytes, _islandSetup.XBackgroundImages);
			_gridOverlay.Initialize(_islandSetup.Size, _islandSetup.Offset, readOnly: false);
			_islandState.Initialize(storageDict.GetStorageDict("IslandState"), gameState, _islandSetup.BaseHappiness, readOnly: false);
			_islandInput.Initialize();
			gameStats.SetIslandId(_islandSetup.IslandId);
			_isometricGrid.Initialize(storageDict.GetStorageDict("IsometricGrid"), _islandSetup, islandsManagerView, worldMap, buildingWarehouseManager, craneManager, gameStats, gameState, popupManager, webService.Multipliers, timing, routineRunner, properties, _overlayManager, _gridOverlay, _islandState, readOnly: false);
			_roadBuilder.Initialize(_isometricGrid, _islandInput, islandsManagerView, worldMap, buildingWarehouseManager, craneManager, gameStats, gameState, popupManager, webService.Multipliers, timing, routineRunner, properties, _overlayManager, _islandState);
			_builder.Initialize(_cameraOperator, _isometricGrid, _islandInput, islandsManagerView, worldMap, buildingWarehouseManager, craneManager, gameStats, gameState, popupManager, webService.Multipliers, timing, routineRunner, _overlayManager, _islandState);
			_expansions.Initialize(storageDict.GetStorageDict("Expansions"), webService.Multipliers, _islandSetup, gameStats, _isometricGrid, _builder, expansionCostManager, properties);
			_cameraOperator.Initialize(_islandInput, _islandSetup.IslandBounds, _expansions, _isometricGrid, timing);
			_sceneryManager.Initialize(storageDict.GetStorageDict("SceneryManager"), _isometricGrid, _builder, _expansions, properties.AllScenery);
			_roadAgentSpawner.Initialize(_isometricGrid, _roadBuilder, timing, _overlayManager);
			_walkerBalloonSpawner.Initialize(gameState, popupManager, routineRunner, _roadAgentSpawner, tutorialManager, cityAdvisor, webService, timing, properties.AllWalkerBalloonProperties);
			_gridColorManager.Initialize(_isometricGrid, _builder, _expansions);
			_isometricIsland.Initialize(_islandSetup, _islandInput, _cameraOperator, _isometricGrid, _islandState, _builder, _roadBuilder, _expansions, localNotificationManager, gameStats, gameState, popupManager, webService, _fishingLocationManager, _overlayManager);
			_airshipPlatformManager.Initialize(storageDict.GetStorageDict("AirshipPlatformManager"), _islandSetup, _isometricGrid, _builder, properties);
			_weatherManager.Initialize(storageDict.GetStorageDict("WeatherManager"), timing, routineRunner);
			_fishingLocationManager.Initialize(_islandSetup, fishingEvent, popupManager, _overlayManager);
			_screenViewName = $"island_{_islandSetup.IslandId.ToString()}";
			ScreenView.SetBottomScreenView(_screenViewName);
		}

		private void OnDestroy()
		{
			if (!string.IsNullOrEmpty(_screenViewName))
			{
				ScreenView.RemoveBottomScreenView(_screenViewName);
			}
		}

		public static string GetIslandStorageKey(IslandId islandId)
		{
			return islandId.ToString();
		}
	}
}
