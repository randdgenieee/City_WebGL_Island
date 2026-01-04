using CIG.Translation;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class IsometricIsland : MonoBehaviour, IHasNotification
	{
		public delegate void IslandLoadedEventHandler(IsometricIsland island);

		public delegate void CinematicPlayingChangedEventHandler(bool playing);

		private WebService _webService;

		private GameStats _gameStats;

		private GameState _gameState;

		private PopupManager _popupManager;

		private LocalNotificationManager _localNotificationManager;

		public static IsometricIsland Current
		{
			get;
			private set;
		}

		public IslandId IslandId
		{
			get;
			private set;
		}

		public IslandInput IslandInput
		{
			get;
			private set;
		}

		public IslandCameraOperator CameraOperator
		{
			get;
			private set;
		}

		public IsometricGrid IsometricGrid
		{
			get;
			private set;
		}

		public CIGIslandState IslandState
		{
			get;
			private set;
		}

		public Builder Builder
		{
			get;
			private set;
		}

		public RoadBuilder RoadBuilder
		{
			get;
			private set;
		}

		public CIGExpansions Expansions
		{
			get;
			private set;
		}

		public FishingLocationManager FishingLocationManager
		{
			get;
			private set;
		}

		public List<Building> BuildingsOnIsland
		{
			get;
			private set;
		} = new List<Building>();


		public Bounds IslandBounds
		{
			get;
			private set;
		}

		public Bounds IslandWorldBounds
		{
			get
			{
				Vector3 center = base.transform.TransformPoint(IslandBounds.center);
				Vector3 size = base.transform.TransformDirection(IslandBounds.size);
				return new Bounds(center, size);
			}
		}

		public float DPI
		{
			get;
			private set;
		}

		public bool CinematicPlaying
		{
			get;
			private set;
		}

		public OverlayManager OverlayManager
		{
			get;
			private set;
		}

		public static event IslandLoadedEventHandler IslandLoadedEvent;

		public event CinematicPlayingChangedEventHandler CinematicPlayingChangedEvent;

		private static void FireIslandLoadedEvent(IsometricIsland island)
		{
			IsometricIsland.IslandLoadedEvent?.Invoke(island);
		}

		private void FireCinematicPlayingChangedEvent(bool playing)
		{
			this.CinematicPlayingChangedEvent?.Invoke(playing);
		}

		public void Initialize(IslandSetup islandSetup, IslandInput islandInput, IslandCameraOperator cameraOperator, IsometricGrid isometricGrid, CIGIslandState islandState, Builder builder, RoadBuilder roadBuilder, CIGExpansions expansions, LocalNotificationManager localNotificationManager, GameStats gameStats, GameState gameState, PopupManager popupManager, WebService webService, FishingLocationManager fishingLocationManager, OverlayManager overlayManager)
		{
			Current = this;
			IslandId = islandSetup.IslandId;
			IslandInput = islandInput;
			CameraOperator = cameraOperator;
			IsometricGrid = isometricGrid;
			IslandState = islandState;
			Builder = builder;
			RoadBuilder = roadBuilder;
			Expansions = expansions;
			_localNotificationManager = localNotificationManager;
			_gameStats = gameStats;
			_gameState = gameState;
			_popupManager = popupManager;
			_webService = webService;
			FishingLocationManager = fishingLocationManager;
			OverlayManager = overlayManager;
			IslandBounds = islandSetup.IslandBounds;
			DPI = ((Screen.dpi <= 0f) ? 72f : Screen.dpi);
			Builder.BuildingBuiltEvent += OnBuildingBuilt;
			IsometricGrid.GridTileRemovedEvent += OnTileDestroyed;
			BuildingsOnIsland = IsometricGrid.FindAll<Building>();
			_webService.PullRequestCompletedEvent += OnPullRequestCompleted;
			OnPullRequestCompleted(_webService.Properties);
			_localNotificationManager.HasNotification(this);
			IslandState.LoadValuesFromBuildings();
			_gameStats.SetCurrentIsland(this);
			FireIslandLoadedEvent(this);
		}

		private void OnDestroy()
		{
			if (Builder != null)
			{
				Builder.BuildingBuiltEvent -= OnBuildingBuilt;
			}
			if (IsometricGrid != null)
			{
				IsometricGrid.GridTileRemovedEvent -= OnTileDestroyed;
			}
			if (_localNotificationManager != null)
			{
				_localNotificationManager.NoLongerHasNotification(this);
				_localNotificationManager = null;
			}
			if (_gameStats != null)
			{
				_gameStats.SetCurrentIsland(null);
				_gameStats = null;
			}
			_gameState = null;
			if (_webService != null)
			{
				_webService.PullRequestCompletedEvent -= OnPullRequestCompleted;
				_webService = null;
			}
			Current = null;
		}

		public PlannedNotification[] GetNotifications()
		{
			List<PlannedNotification> list = new List<PlannedNotification>();
			double num = 0.0;
			int num2 = 0;
			double num3 = 0.0;
			int num4 = 0;
			int i = 0;
			for (int count = BuildingsOnIsland.Count; i < count; i++)
			{
				CIGCommercialBuilding cIGCommercialBuilding = BuildingsOnIsland[i] as CIGCommercialBuilding;
				if (cIGCommercialBuilding != null)
				{
					if (cIGCommercialBuilding.CurrencyConversionProcess != null)
					{
						num3 = Math.Max(num3, cIGCommercialBuilding.ProfitTimeLeft);
						num4++;
					}
					else
					{
						num = Math.Max(num, cIGCommercialBuilding.ProfitTimeLeft);
						num2++;
					}
				}
			}
			if (num > 0.0 && num2 >= 5)
			{
				int seconds = (int)Math.Max(300.0, num + 60.0);
				list.Add(new PlannedNotification(seconds, Localization.Key("notification_commercial_buildings_ready"), sound: false, 1));
			}
			if (num3 > 0.0 && num4 >= 1)
			{
				int seconds2 = (int)Math.Max(300.0, num3 + 60.0);
				list.Add(new PlannedNotification(seconds2, Localization.Key("notification_currency_exchange_ready"), sound: false, 5));
			}
			return list.ToArray();
		}

		public void SetCinematicPlaying(bool playing)
		{
			bool cinematicPlaying = CinematicPlaying;
			CinematicPlaying = playing;
			if (cinematicPlaying != CinematicPlaying)
			{
				FireCinematicPlayingChangedEvent(CinematicPlaying);
			}
		}

		private void UpdateConfirmed()
		{
			Application.OpenURL(CIGGameConstants.UpdateUrl);
		}

		private void OnPullRequestCompleted(Dictionary<string, string> properties)
		{
			if (properties.Get("update_available", defaultValue: false))
			{
				bool flag = true;
				string shownUpdatePopupVersion = _gameState.ShownUpdatePopupVersion;
				if (shownUpdatePopupVersion != null && shownUpdatePopupVersion == "1.11.8")
				{
					double shownUpdatePopupTimestamp = _gameState.ShownUpdatePopupTimestamp;
					flag = (Timing.UtcNow - shownUpdatePopupTimestamp >= 172800.0);
				}
				if (flag)
				{
					_gameState.ShownUpdatePopupVersion = "1.11.8";
					_gameState.ShownUpdatePopupTimestamp = Timing.UtcNow;
					GenericPopupRequest request = new GenericPopupRequest("update_available").SetDismissable(dismissable: false).SetTexts(Localization.Key("update_available"), Localization.Key("update_older_version")).SetGreenButton(Localization.Key("update_go"), null, UpdateConfirmed)
						.SetRedButton(Localization.Key("not_now"));
					_popupManager.RequestPopup(request);
				}
			}
		}

		private void OnTileDestroyed(GridTile tile)
		{
			Building building = tile as Building;
			if (building == null || !BuildingsOnIsland.Contains(building))
			{
				if (building != null)
				{
					UnityEngine.Debug.LogWarning("The buildings list doesnt contain the destroyed Building");
				}
			}
			else
			{
				BuildingsOnIsland.Remove(building);
			}
		}

		private void OnBuildingBuilt(GridTile tile, bool isNewBuilding)
		{
			Building building = tile as Building;
			if (!(building == null) && !BuildingsOnIsland.Contains(building))
			{
				BuildingsOnIsland.Add(building);
			}
		}
	}
}
