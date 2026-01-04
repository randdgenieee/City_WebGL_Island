using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CIG
{
	public class WorldMapView : MonoBehaviour
	{
		public delegate void VisibilityChangedEventHandler(bool visible);

		public delegate void NewIslandUnlockedEventHandler(WorldMapIsland worldMapIsland);

		public delegate void ShowNewIslandsAnimationPlayingChangedEventHandler(bool isPlaying);

		private const float DefaultDPI = 72f;

		private const float WantedPixelThresholdAt72DPI = 7f;

		private const float LineCenterOffset = 10f;

		[SerializeField]
		private WorldMapIsland[] _islands;

		[SerializeField]
		private GameObject _root;

		[SerializeField]
		private EventSystem _eventSystem;

		[SerializeField]
		private WorldMapCameraOperator _cameraOperator;

		[SerializeField]
		private AirshipView _airshipView;

		[SerializeField]
		private OverlayManager _overlayManager;

		[SerializeField]
		private WorldMapCloudController _cloudController;

		[SerializeField]
		private GameObject _connectionLinePrefab;

		[SerializeField]
		private RectTransform _connectionLineParent;

		[SerializeField]
		private GameObject _markerPrefab;

		[SerializeField]
		private RectTransform _markerParent;

		private GameStats _gameStats;

		private IslandsManagerView _islandsManagerView;

		private WorldMap _worldMap;

		private List<IslandProperties> _islandProperties;

		private IEnumerator _showNewVisibleIslandsRoutine;

		public bool IsVisible => _worldMap.IsVisible;

		public WorldMapCameraOperator CameraOperator => _cameraOperator;

		public bool IsShowingNewIslands => _showNewVisibleIslandsRoutine != null;

		public WorldMapIsland NewlyUnlockedIsland
		{
			get;
			private set;
		}

		public AirshipView AirshipView => _airshipView;

		public event VisibilityChangedEventHandler VisibilityChangedEvent;

		public event NewIslandUnlockedEventHandler NewIslandUnlockedEvent;

		public event ShowNewIslandsAnimationPlayingChangedEventHandler ShowNewIslandsAnimationPlayingChangedEvent;

		private void FireVisibilityChangedEvent(bool visible)
		{
			this.VisibilityChangedEvent?.Invoke(visible);
		}

		private void FireNewIslandUnlockedEvent(WorldMapIsland worldMapIsland)
		{
			this.NewIslandUnlockedEvent?.Invoke(worldMapIsland);
		}

		private void FireShowNewIslandsAnimationPlayingChangedEvent(bool isPlaying)
		{
			this.ShowNewIslandsAnimationPlayingChangedEvent?.Invoke(isPlaying);
		}

		public void Initialize(GameStats gameStats, Timing timing, PopupManager popupManager, IslandsManagerView islandsManagerView, WorldMap worldMap, List<IslandProperties> islandProperties)
		{
			_gameStats = gameStats;
			_islandsManagerView = islandsManagerView;
			_worldMap = worldMap;
			_islandProperties = islandProperties;
			_cameraOperator.Initialize(timing);
			CreateConnections();
			SetDragThreshold(_eventSystem);
			_cloudController.Initialize(timing, this, GetVisibleWorldMapIslands());
			SetIslandsInteractable(interactable: true);
			_airshipView.Initialize(_worldMap.Airship, timing, popupManager, _islandsManagerView.IslandsManager, this, _overlayManager);
			_islandsManagerView.IslandLoadingStartedEvent += OnIslandLoadingStarted;
			_islandsManagerView.IslandChangedEvent += OnIslandChanged;
			_worldMap.NewIslandUnlockedEvent += OnNewIslandUnlocked;
			_worldMap.VisibilityChangedEvent += OnVisiblilityChanged;
			if (_worldMap.NewlyUnlockedIslandId != IslandId.None)
			{
				OnNewIslandUnlocked(_worldMap.NewlyUnlockedIslandId);
			}
			OnVisiblilityChanged(_worldMap.IsVisible);
		}

		private void OnDestroy()
		{
			if (_worldMap != null)
			{
				_worldMap.NewIslandUnlockedEvent -= OnNewIslandUnlocked;
				_worldMap.VisibilityChangedEvent -= OnVisiblilityChanged;
				_worldMap = null;
			}
			if (_islandsManagerView != null)
			{
				_islandsManagerView.IslandChangedEvent -= OnIslandChanged;
				_islandsManagerView.IslandLoadingStartedEvent -= OnIslandLoadingStarted;
				_islandsManagerView = null;
			}
			PopScreenViews();
		}

		public WorldMapIsland GetWorldMapIsland(IslandId islandId)
		{
			int i = 0;
			for (int num = _islands.Length; i < num; i++)
			{
				if (_islands[i].IslandId == islandId)
				{
					return _islands[i];
				}
			}
			UnityEngine.Debug.LogError($"WorldMapIsland for island `{islandId}` could not be found.");
			return null;
		}

		public Vector3 GetWorldMapIslandPosition(IslandId islandId)
		{
			WorldMapIsland worldMapIsland = GetWorldMapIsland(islandId);
			if (worldMapIsland == null)
			{
				return Vector3.zero;
			}
			return worldMapIsland.transform.localPosition;
		}

		public void SetTutorialFocusIsland(IslandId islandId)
		{
			SetIslandsInteractable(interactable: false);
			_cameraOperator.DisableInput();
			WorldMapIsland worldMapIsland = GetWorldMapIsland(islandId);
			worldMapIsland.SetInteractable(interactable: true);
			_cameraOperator.ScrollTo(worldMapIsland.transform.position);
		}

		private List<WorldMapIsland> GetVisibleWorldMapIslands()
		{
			List<WorldMapIsland> list = new List<WorldMapIsland>();
			foreach (IslandId visibleIsland in _worldMap.VisibleIslands)
			{
				WorldMapIsland worldMapIsland = GetWorldMapIsland(visibleIsland);
				if (worldMapIsland != null)
				{
					list.Add(worldMapIsland);
				}
			}
			return list;
		}

		private void CreateConnections()
		{
			Dictionary<IslandConnection, GameObject> dictionary = new Dictionary<IslandConnection, GameObject>();
			int i = 0;
			for (int num = _islands.Length; i < num; i++)
			{
				WorldMapIsland island = _islands[i];
				List<IslandConnection> connections = _worldMap.IslandConnectionGraph.GetConnections(island.IslandId);
				List<GameObject> list = new List<GameObject>();
				int j = 0;
				for (int count = connections.Count; j < count; j++)
				{
					IslandConnection islandConnection = connections[j];
					if (!dictionary.TryGetValue(islandConnection, out GameObject value))
					{
						Vector3 worldMapIslandPosition = GetWorldMapIslandPosition(islandConnection.From);
						Vector3 a = GetWorldMapIslandPosition(islandConnection.To) - worldMapIslandPosition;
						value = UnityEngine.Object.Instantiate(_connectionLinePrefab, _connectionLineParent);
						value.name = "ConnectionFrom" + islandConnection.From + "To" + islandConnection.To;
						RectTransform rectTransform = (RectTransform)value.transform;
						rectTransform.sizeDelta = new Vector2(a.magnitude, rectTransform.sizeDelta.y);
						rectTransform.right = -a;
						rectTransform.position = worldMapIslandPosition + a * 0.5f + rectTransform.up * 10f;
						value.gameObject.SetActive(value: false);
						dictionary[islandConnection] = value;
					}
					list.Add(value);
				}
				island.Initialize(_gameStats, _worldMap, this, _islandsManagerView.IslandsManager, _overlayManager, _islandProperties.Find((IslandProperties p) => p.IslandId == island.IslandId), list, _markerPrefab, _markerParent);
			}
		}

		private void SetIslandsInteractable(bool interactable)
		{
			int i = 0;
			for (int num = _islands.Length; i < num; i++)
			{
				WorldMapIsland worldMapIsland = _islands[i];
				worldMapIsland.SetInteractable(interactable && _worldMap.VisibleIslands.Contains(worldMapIsland.IslandId));
			}
		}

		private void ToggleWorldMap(bool visible)
		{
			_root.SetActive(visible);
			if (visible)
			{
				if (_worldMap.NewlyUnlockedIslandId != IslandId.None)
				{
					OnNewIslandUnlocked(_worldMap.NewlyUnlockedIslandId);
				}
				else
				{
					_cameraOperator.ZoomTo(1400f, 800f, 1.2f);
					if (!_islandsManagerView.IslandsManager.IsVisiting && _showNewVisibleIslandsRoutine == null)
					{
						StartCoroutine(_showNewVisibleIslandsRoutine = ShowNewVisibleIslandsRoutine());
					}
				}
				ScreenView.SetBottomScreenView(_islandsManagerView.IslandsManager.IsVisiting ? "worldmap_visiting" : "worldmap");
			}
			else
			{
				PopScreenViews();
			}
		}

		private static void PopScreenViews()
		{
			ScreenView.RemoveBottomScreenView("worldmap");
			ScreenView.RemoveBottomScreenView("worldmap_visiting");
		}

		private void OnNewIslandUnlocked(IslandId islandId)
		{
			if (IsVisible)
			{
				SetIslandsInteractable(interactable: false);
				NewlyUnlockedIsland = GetWorldMapIsland(islandId);
				NewlyUnlockedIsland.SetInteractable(interactable: true);
				_cameraOperator.DisableInput();
				_cameraOperator.ScrollTo(NewlyUnlockedIsland.transform.position);
				FireNewIslandUnlockedEvent(NewlyUnlockedIsland);
			}
		}

		private void OnVisiblilityChanged(bool visible)
		{
			ToggleWorldMap(visible);
			if (visible)
			{
				_cameraOperator.ZoomTo(1400f, 800f, 1.2f);
			}
			else
			{
				_cameraOperator.EnableInput();
				SetIslandsInteractable(interactable: true);
			}
			FireVisibilityChangedEvent(visible);
		}

		private void SetDragThreshold(EventSystem eventSystem)
		{
			int num2 = eventSystem.pixelDragThreshold = Mathf.RoundToInt(7f / 72f * Screen.dpi);
		}

		private void OnIslandChanged(IsometricIsland isometricIsland)
		{
			_worldMap.ToggleWorldMap(isometricIsland == null);
		}

		private void OnIslandLoadingStarted(IslandId islandId)
		{
			if (islandId != IslandId.None)
			{
				WorldMapIsland worldMapIsland = GetWorldMapIsland(islandId);
				_cameraOperator.ScrollAndZoom(worldMapIsland.gameObject, 800f, null, null, 1.2f);
			}
		}

		private IEnumerator ShowNewVisibleIslandsRoutine()
		{
			yield return new WaitWhile(() => SingletonMonobehaviour<CinematicEffect>.Instance.IsShowing);
			FireShowNewIslandsAnimationPlayingChangedEvent(isPlaying: true);
			SetIslandsInteractable(interactable: false);
			_cameraOperator.DisableInput();
			int i = 0;
			for (int length = _islands.Length; i < length; i++)
			{
				WorldMapIsland worldMapIsland = _islands[i];
				IslandId islandId = worldMapIsland.IslandId;
				if (_worldMap.IslandConnectionGraph.GetConnections(islandId).Find((IslandConnection c) => _islandsManagerView.IslandsManager.IsUnlocked(c.From)) != null && !_worldMap.IsIslandVisible(islandId))
				{
					_worldMap.IslandShown(islandId);
					worldMapIsland.SetVisible(isVisible: true);
					CameraOperator.ScrollTo(worldMapIsland.transform.position);
					yield return _cloudController.FadeCloudsAround(worldMapIsland);
				}
			}
			_cameraOperator.EnableInput();
			SetIslandsInteractable(interactable: true);
			_showNewVisibleIslandsRoutine = null;
			FireShowNewIslandsAnimationPlayingChangedEvent(isPlaying: false);
		}
	}
}
