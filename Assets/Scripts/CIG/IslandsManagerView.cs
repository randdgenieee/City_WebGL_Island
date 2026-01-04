using System;
using System.Collections;
using UnityEngine;

namespace CIG
{
	public class IslandsManagerView : MonoBehaviour
	{
		public delegate void IslandLoadingStartedEventHandler(IslandId islandId);

		public delegate void IslandChangedEventHandler(IsometricIsland isometricIsland);

		private GameStats _gameStats;

		private GameState _gameState;

		private ExpansionCostManager _expansionCostManager;

		private LocalNotificationManager _localNotificationManager;

		private BuildingWarehouseManager _buildingWarehouseManager;

		private VideoAds1Manager _videoAds1Manager;

		private VideoAds2Manager _videoAds2Manager;

		private CityAdvisor _cityAdvisor;

		private TreasureChestManager _treasureChestManager;

		private TutorialManager _tutorialManager;

		private CurrencyConversionManager _currencyConversionManager;

		private BuildingsManager _buildingsManager;

		private CraneManager _craneManager;

		private SaleManager _saleManager;

		private IslandVisitingManager _islandVisitingManager;

		private IslandVisitingCurrencyManager _islandVisitingCurrencyManager;

		private WorldMap _worldMap;

		private PopupManager _popupManager;

		private WebService _webService;

		private Timing _timing;

		private Properties _properties;

		private FishingEvent _fishingEvent;

		private RoutineRunner _routineRunner;

		private GameObject _currentIslandObject;

		public bool IsLoading
		{
			get;
			private set;
		}

		public IslandsManager IslandsManager
		{
			get;
			private set;
		}

		public event IslandLoadingStartedEventHandler IslandLoadingStartedEvent;

		public event IslandChangedEventHandler IslandChangedEvent;

		private void FireIslandLoadingStartedEvent(IslandId islandId)
		{
			this.IslandLoadingStartedEvent?.Invoke(islandId);
		}

		private void FireIslandChangedEvent(IsometricIsland isometricIsland)
		{
			this.IslandChangedEvent?.Invoke(isometricIsland);
		}

		public void Initialize(GameStats gameStats, GameState gameState, IslandsManager islandsManager, ExpansionCostManager expansionCostManager, LocalNotificationManager localNotificationManager, BuildingWarehouseManager buildingWarehouseManager, VideoAds1Manager videoAds1Manager, VideoAds2Manager videoAds2Manager, CityAdvisor cityAdvisor, TreasureChestManager treasureChestManager, TutorialManager tutorialManager, CurrencyConversionManager currencyConversionManager, BuildingsManager buildingsManager, CraneManager craneManager, SaleManager saleManager, IslandVisitingManager islandVisitingManager, IslandVisitingCurrencyManager islandVisitingCurrencyManager, WorldMap worldMap, PopupManager popupManager, WebService webService, Timing timing, Properties properties, FishingEvent fishingEvent, RoutineRunner routineRunner)
		{
			_gameStats = gameStats;
			_gameState = gameState;
			IslandsManager = islandsManager;
			_expansionCostManager = expansionCostManager;
			_localNotificationManager = localNotificationManager;
			_buildingWarehouseManager = buildingWarehouseManager;
			_videoAds1Manager = videoAds1Manager;
			_videoAds2Manager = videoAds2Manager;
			_cityAdvisor = cityAdvisor;
			_treasureChestManager = treasureChestManager;
			_tutorialManager = tutorialManager;
			_currencyConversionManager = currencyConversionManager;
			_buildingsManager = buildingsManager;
			_craneManager = craneManager;
			_saleManager = saleManager;
			_islandVisitingManager = islandVisitingManager;
			_islandVisitingCurrencyManager = islandVisitingCurrencyManager;
			_worldMap = worldMap;
			_popupManager = popupManager;
			_webService = webService;
			_timing = timing;
			_properties = properties;
			_fishingEvent = fishingEvent;
			_routineRunner = routineRunner;
			IslandsManager.IslandChangedEvent += OnIslandChanged;
			IslandsManager.VisitingStartedEvent += OnStartVisiting;
			IslandsManager.VisitingStoppedEvent += OnStopVisiting;
			LoadIsland(IslandsManager.CurrentIsland, IslandsManager.IsVisiting, animateIn: false);
		}

		private void Update()
		{
		}

		private void OnDestroy()
		{
			_gameStats = null;
			_islandVisitingManager = null;
			if (IslandsManager != null)
			{
				IslandsManager.IslandChangedEvent -= OnIslandChanged;
				IslandsManager.VisitingStartedEvent -= OnStartVisiting;
				IslandsManager.VisitingStoppedEvent -= OnStopVisiting;
				IslandsManager = null;
			}
		}

		private void LoadIsland(IslandId islandId, bool isVisiting, bool animateIn)
		{
			if (isVisiting)
			{
				StartCoroutine(LoadVisitingIslandRoutine(islandId));
			}
			else
			{
				StartCoroutine(LoadIslandRoutine(islandId, animateIn));
				Analytics.IslandOpened(islandId.ToString());
			}
			FireIslandLoadingStartedEvent(islandId);
		}

		private void InstantiateIsland(IslandId islandId, bool isVisiting)
		{
			IslandsAssetCollection.IslandPrefabs asset = SingletonMonobehaviour<IslandsAssetCollection>.Instance.GetAsset(islandId);
			if (isVisiting)
			{
				ReadOnlyIslandBootstrapper readOnlyIslandBootstrapper = UnityEngine.Object.Instantiate(asset.ReadOnlyPrefab, base.transform);
				readOnlyIslandBootstrapper.Initialize(_gameStats, _gameState, this, _expansionCostManager, _localNotificationManager, _islandVisitingCurrencyManager, _buildingWarehouseManager, _worldMap, _craneManager, _popupManager, _webService, _timing, _routineRunner, _properties);
				_currentIslandObject = readOnlyIslandBootstrapper.gameObject;
				FireIslandChangedEvent(readOnlyIslandBootstrapper.IsometricIsland);
			}
			else
			{
				IslandBootstrapper islandBootstrapper = UnityEngine.Object.Instantiate(asset.NormalPrefab, base.transform);
				islandBootstrapper.Initialize(_gameStats, _gameState, _expansionCostManager, _localNotificationManager, _buildingWarehouseManager, _videoAds1Manager, _videoAds2Manager, _cityAdvisor, _treasureChestManager, _tutorialManager, _currencyConversionManager, _craneManager, _buildingsManager, this, _saleManager, _worldMap, _popupManager, _webService, _timing, _properties, _fishingEvent, _routineRunner);
				_currentIslandObject = islandBootstrapper.gameObject;
				FireIslandChangedEvent(islandBootstrapper.IsometricIsland);
			}
		}

		private void OnIslandChanged(IslandId islandId, bool isVisiting)
		{
			if (IsLoading)
			{
				UnityEngine.Debug.LogError($"Trying to open an island while loading. IsVisiting: {isVisiting}");
			}
			else if (islandId == IslandId.None)
			{
				StartCoroutine(UnloadCurrentIslandRoutine());
			}
			else
			{
				LoadIsland(islandId, isVisiting, animateIn: true);
			}
		}

		private void OnStartVisiting()
		{
			StartCoroutine(StartVisitingRoutine());
		}

		private void OnStopVisiting()
		{
			StartCoroutine(StopVisitingRoutine());
		}

		private IEnumerator UnloadCurrentIslandRoutine()
		{
			IsLoading = true;
			yield return SingletonMonobehaviour<CinematicEffect>.Instance.ShowAnimated(scaleUp: false);
			yield return DestroyIslandRoutine();
			yield return SingletonMonobehaviour<CinematicEffect>.Instance.HideAnimated(scaleUp: false);
			IsLoading = false;
		}

		private IEnumerator LoadIslandRoutine(IslandId newIslandId, bool animateIn)
		{
			if (newIslandId.IsValid())
			{
				IsLoading = true;
				if (animateIn)
				{
					yield return SingletonMonobehaviour<CinematicEffect>.Instance.ShowAnimated(scaleUp: true);
				}
				else
				{
					SingletonMonobehaviour<CinematicEffect>.Instance.ShowInstant();
				}
				SpinnerManager.PushSpinnerRequest(this);
				if (IslandsManager.CurrentIsland != IslandId.None)
				{
					yield return DestroyIslandRoutine();
				}
				InstantiateIsland(newIslandId, isVisiting: false);
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				SpinnerManager.PopSpinnerRequest(this);
				IsLoading = false;
				yield return SingletonMonobehaviour<CinematicEffect>.Instance.HideAnimated(scaleUp: true);
			}
		}

		private IEnumerator LoadVisitingIslandRoutine(IslandId visitingIslandId)
		{
			if (_islandVisitingManager.LoadVisitingIslandData(visitingIslandId, IslandsManager.VisitingUserId))
			{
				IsLoading = true;
				yield return SingletonMonobehaviour<CinematicEffect>.Instance.ShowAnimated(scaleUp: true);
				SpinnerManager.PushSpinnerRequest(this);
				InstantiateIsland(visitingIslandId, isVisiting: true);
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				SpinnerManager.PopSpinnerRequest(this);
				IsLoading = false;
				yield return SingletonMonobehaviour<CinematicEffect>.Instance.HideAnimated(scaleUp: true);
			}
			else
			{
				UnityEngine.Debug.LogError("Visiting Island data not loaded!");
			}
		}

		private IEnumerator StartVisitingRoutine()
		{
			IsLoading = true;
			yield return SingletonMonobehaviour<CinematicEffect>.Instance.ShowAnimated(scaleUp: false);
			if (IslandsManager.CurrentIsland != IslandId.None)
			{
				yield return DestroyIslandRoutine();
			}
			yield return SingletonMonobehaviour<CinematicEffect>.Instance.HideAnimated(scaleUp: false);
			IsLoading = false;
		}

		private IEnumerator StopVisitingRoutine()
		{
			IsLoading = true;
			yield return SingletonMonobehaviour<CinematicEffect>.Instance.ShowAnimated(scaleUp: true);
			yield return DestroyIslandRoutine();
			yield return SingletonMonobehaviour<CinematicEffect>.Instance.HideAnimated(scaleUp: true);
			IsLoading = false;
		}

		private IEnumerator DestroyIslandRoutine()
		{
			if (_currentIslandObject != null)
			{
				UnityEngine.Object.Destroy(_currentIslandObject);
				_currentIslandObject = null;
				FireIslandChangedEvent(null);
				yield return Resources.UnloadUnusedAssets();
				GC.Collect();
			}
		}
	}
}
