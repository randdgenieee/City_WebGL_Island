using UnityEngine;

namespace CIG
{
	public class ReadOnlyIslandBootstrapper : MonoBehaviour
	{
		[SerializeField]
		private IslandSetup _islandSetup;

		[SerializeField]
		private IsometricIsland _isometricIsland;

		[SerializeField]
		private IslandInput _islandInput;

		[SerializeField]
		private IslandCameraOperator _cameraOperator;

		[SerializeField]
		private Builder _builder;

		[SerializeField]
		private RoadBuilder _roadBuilder;

		[SerializeField]
		private CIGExpansions _expansions;

		[SerializeField]
		private CIGIslandState _islandState;

		[SerializeField]
		private RoadAgentSpawner _roadAgentSpawner;

		[SerializeField]
		private WeatherManager _weatherManager;

		[SerializeField]
		private IslandVisitingCurrencySpawner _islandVisitingCurrencySpawner;

		[SerializeField]
		private BackgroundLoader _backgroundLoader;

		[SerializeField]
		private IsometricGrid _isometricGrid;

		[SerializeField]
		private GridOverlay _gridOverlay;

		[SerializeField]
		private GridColorManager _gridColorManager;

		[SerializeField]
		private OverlayManager _overlayManager;

		private string _screenViewName;

		private const string ReadOnlyIslandStateStorageKey = "IslandState";

		public const string ReadOnlyIsometricGridStorageKey = "IsometricGrid";

		private const string ReadOnlyExpansionsStorageKey = "Expansions";

		private const string ReadOnlyWeatherManagerStorageKey = "WeatherManager";

		public IsometricIsland IsometricIsland => _isometricIsland;

		public void Initialize(GameStats gameStats, GameState gameState, IslandsManagerView islandsManagerView, ExpansionCostManager expansionCostManager, LocalNotificationManager localNotificationManager, IslandVisitingCurrencyManager islandVisitingCurrencyManager, BuildingWarehouseManager buildingWarehouseManager, WorldMap worldMap, CraneManager craneManager, PopupManager popupManager, WebService webService, Timing timing, RoutineRunner routineRunner, Properties properties)
		{
			StorageDictionary storageDict = StorageController.GameRoot.GetStorageDict(GetReadOnlyIslandStorageKey(_islandSetup.IslandId));
			_backgroundLoader.Initialize(_islandSetup.BackgroundImageBytes, _islandSetup.XBackgroundImages);
			_gridOverlay.Initialize(_islandSetup.Size, _islandSetup.Offset, readOnly: true);
			_islandState.Initialize(storageDict.GetStorageDict("IslandState"), gameState, _islandSetup.BaseHappiness, readOnly: true);
			_islandInput.Initialize();
			_isometricGrid.Initialize(storageDict.GetStorageDict("IsometricGrid"), _islandSetup, islandsManagerView, worldMap, buildingWarehouseManager, craneManager, gameStats, gameState, popupManager, webService.Multipliers, timing, routineRunner, properties, _overlayManager, _gridOverlay, _islandState, readOnly: true);
			_roadBuilder.Initialize(_isometricGrid, _islandInput, islandsManagerView, worldMap, buildingWarehouseManager, craneManager, gameStats, gameState, popupManager, webService.Multipliers, timing, routineRunner, properties, _overlayManager, _islandState);
			_builder.Initialize(_cameraOperator, _isometricGrid, _islandInput, islandsManagerView, worldMap, buildingWarehouseManager, craneManager, gameStats, gameState, popupManager, webService.Multipliers, timing, routineRunner, _overlayManager, _islandState);
			_expansions.Initialize(storageDict.GetStorageDict("Expansions"), webService.Multipliers, _islandSetup, gameStats, _isometricGrid, _builder, expansionCostManager, properties);
			_cameraOperator.Initialize(_islandInput, _islandSetup.IslandBounds, _expansions, _isometricGrid, timing);
			_roadAgentSpawner.Initialize(_isometricGrid, _roadBuilder, timing, _overlayManager);
			_gridColorManager.Initialize(_isometricGrid, _builder, _expansions);
			_islandVisitingCurrencySpawner.Initialize(islandVisitingCurrencyManager, _isometricGrid, _isometricIsland.IslandId, islandsManagerView.IslandsManager.VisitingUserId);
			_isometricIsland.Initialize(_islandSetup, _islandInput, _cameraOperator, _isometricGrid, _islandState, _builder, _roadBuilder, _expansions, localNotificationManager, gameStats, gameState, popupManager, webService, null, _overlayManager);
			_weatherManager.Initialize(storageDict.GetStorageDict("WeatherManager"), timing, routineRunner);
			_screenViewName = $"island_visiting_{_islandSetup.IslandId.ToString()}";
			ScreenView.SetBottomScreenView(_screenViewName);
		}

		private void OnDestroy()
		{
			if (!string.IsNullOrEmpty(_screenViewName))
			{
				ScreenView.RemoveBottomScreenView(_screenViewName);
			}
		}

		public static string GetReadOnlyIslandStorageKey(IslandId islandId)
		{
			return islandId + "ReadOnly";
		}
	}
}
