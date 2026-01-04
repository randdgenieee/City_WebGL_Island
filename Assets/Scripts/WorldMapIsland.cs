using CIG;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapIsland : MonoBehaviour
{
	public delegate void IslandOverlayToggledEventHandler(Overlay overlay, bool active);

	[SerializeField]
	private Button _button;

	[SerializeField]
	private IslandId _island;

	[SerializeField]
	private Image _lockIcon;

	[SerializeField]
	private RectTransform _maskTransform;

	private GameStats _gameStats;

	private WorldMap _worldMap;

	private WorldMapView _worldMapView;

	private IslandsManager _islandsManager;

	private OverlayManager _overlayManager;

	private RectTransform _markerParent;

	private GameObject _markerPrefab;

	private IslandProperties _islandProperties;

	private List<GameObject> _connectionLines;

	private Overlay _currentOverlay;

	private GameObject _marker;

	public IslandId IslandId => _island;

	public RectTransform MaskTransform => _maskTransform;

	public IslandSelectedOverlay IslandButtons
	{
		get;
		private set;
	}

	private bool IsUnlocked => _islandsManager.IsUnlocked(IslandId);

	public event IslandOverlayToggledEventHandler IslandOverlayToggledEvent;

	private void FireIslandOverlayToggledEvent(Overlay overlay, bool active)
	{
		this.IslandOverlayToggledEvent?.Invoke(overlay, active);
	}

	public void Initialize(GameStats gameStats, WorldMap worldMap, WorldMapView worldMapView, IslandsManager islandsManager, OverlayManager overlayManager, IslandProperties islandProperties, List<GameObject> connectionLines, GameObject markerPrefab, RectTransform markerParent)
	{
		_gameStats = gameStats;
		_worldMap = worldMap;
		_worldMapView = worldMapView;
		_islandsManager = islandsManager;
		_overlayManager = overlayManager;
		_islandProperties = islandProperties;
		_markerParent = markerParent;
		_markerPrefab = markerPrefab;
		_islandsManager.IslandUnlockedEvent += OnIslandUnlocked;
		_islandsManager.VisitingStartedEvent += OnVisitingStarted;
		_islandsManager.VisitingStoppedEvent += OnVisitingStopped;
		_connectionLines = connectionLines;
		UpdateLock();
		SetVisible(_worldMap.IsIslandVisible(IslandId));
	}

	private void OnDestroy()
	{
		if (_islandsManager != null)
		{
			_islandsManager.IslandUnlockedEvent -= OnIslandUnlocked;
			_islandsManager.VisitingStartedEvent -= OnVisitingStarted;
			_islandsManager.VisitingStoppedEvent -= OnVisitingStopped;
			_islandsManager = null;
		}
		_worldMap = null;
		_worldMapView = null;
	}

	private void OnEnable()
	{
		GameEvents.Subscribe<UnemiShouldCloseEvent>(OnUnemiShouldCloseEvent);
	}

	private void OnDisable()
	{
		GameEvents.Unsubscribe<UnemiShouldCloseEvent>(OnUnemiShouldCloseEvent);
	}

	public void SetVisible(bool isVisible)
	{
		if (isVisible && _marker != null)
		{
			UnityEngine.Object.Destroy(_marker);
			_marker = null;
		}
		else if (!isVisible && _marker == null)
		{
			_marker = UnityEngine.Object.Instantiate(_markerPrefab, base.transform.position - base.transform.forward * 175f, base.transform.localRotation, _markerParent);
		}
	}

	public void OnIslandClicked()
	{
		if (!_worldMapView.CameraOperator.Interacting)
		{
			ToggleButtons();
			RemoveOverlay();
		}
	}

	public void SetInteractable(bool interactable)
	{
		_button.interactable = interactable;
	}

	private void ToggleConnections(bool active)
	{
		List<IslandConnection> connections = _worldMap.IslandConnectionGraph.GetConnections(_island);
		int i = 0;
		for (int count = _connectionLines.Count; i < count; i++)
		{
			IslandConnection islandConnection = connections[i];
			bool flag = _worldMap.IsIslandVisible((islandConnection.To == IslandId) ? islandConnection.From : islandConnection.To);
			_connectionLines[i].SetActive(active && flag);
		}
	}

	private void ToggleButtons()
	{
		if (IslandButtons == null)
		{
			ShowButtons();
		}
		else
		{
			RemoveButtons();
		}
	}

	private void ShowButtons()
	{
		if (IslandButtons == null)
		{
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ButtonClick);
			GameEvents.Invoke(new UnemiShouldCloseEvent(this));
			bool flag = _worldMap.IslandConnectionGraph.AreIslandsConnected(_worldMap.Airship.CurrentIslandId, _island);
			bool isUnlocked = IsUnlocked;
			IslandButtons = _overlayManager.CreateOverlay<IslandSelectedOverlay>(base.gameObject, OverlayType.IslandButtons);
			IslandButtons.Initialize(isUnlocked, OnOpenClicked, flag && _worldMap.Airship.CanTravel, !isUnlocked && !_islandsManager.IsVisiting, OnUnlockClicked, flag && _worldMap.Airship.CurrentIslandId != _island && _worldMap.Airship.CanTravel, !_islandsManager.IsVisiting && isUnlocked, OnMoveClicked, _islandProperties.SurfaceTypes, _worldMap.Airship.CurrentIslandId == _island);
			IslandButtons.Show();
			_worldMapView.CameraOperator.ScrollTo(base.gameObject);
		}
		ToggleConnections(active: true);
		FireIslandOverlayToggledEvent(IslandButtons, active: true);
	}

	private void RemoveButtons()
	{
		if (IslandButtons != null)
		{
			IslandButtons.Remove();
			FireIslandOverlayToggledEvent(IslandButtons, active: false);
			IslandButtons = null;
		}
		ToggleConnections(active: false);
	}

	private void RemoveOverlay()
	{
		if (_currentOverlay != null)
		{
			_currentOverlay.Remove();
			FireIslandOverlayToggledEvent(_currentOverlay, active: false);
			_currentOverlay = null;
		}
	}

	private void OnUnemiShouldCloseEvent(UnemiShouldCloseEvent e)
	{
		RemoveOverlay();
		RemoveButtons();
	}

	private void OnOpenClicked()
	{
		if (IsUnlocked)
		{
			_worldMap.GoToIsland(IslandId);
			GameEvents.Invoke(new UnemiShouldCloseEvent(this));
		}
	}

	private void OnUnlockClicked()
	{
		if (_currentOverlay != null && _currentOverlay is IslandUnlockOverlay)
		{
			RemoveOverlay();
			return;
		}
		RemoveOverlay();
		IslandUnlockOverlay islandUnlockOverlay = _overlayManager.CreateOverlay<IslandUnlockOverlay>(base.gameObject, OverlayType.IslandUnlock);
		islandUnlockOverlay.Initialize(_gameStats, _worldMap, _island, _islandProperties.CashCost, _islandProperties.GoldCost, _worldMap.GetTravelDuration(_island));
		islandUnlockOverlay.Show();
		_currentOverlay = islandUnlockOverlay;
		FireIslandOverlayToggledEvent(_currentOverlay, active: true);
	}

	private void OnMoveClicked()
	{
		if (_currentOverlay != null && _currentOverlay is IslandGoToOverlay)
		{
			RemoveOverlay();
			return;
		}
		RemoveOverlay();
		IslandGoToOverlay islandGoToOverlay = _overlayManager.CreateOverlay<IslandGoToOverlay>(base.gameObject, OverlayType.IslandGoTo);
		islandGoToOverlay.Initialize(_worldMap, _island, _worldMap.GetTravelDuration(_island));
		islandGoToOverlay.Show();
		_currentOverlay = islandGoToOverlay;
		FireIslandOverlayToggledEvent(_currentOverlay, active: true);
	}

	private void OnIslandUnlocked(IslandId islandId)
	{
		UpdateLock();
	}

	private void UpdateLock()
	{
		_lockIcon.enabled = !IsUnlocked;
	}

	private void OnVisitingStarted()
	{
		UpdateLock();
	}

	private void OnVisitingStopped()
	{
		UpdateLock();
	}
}
