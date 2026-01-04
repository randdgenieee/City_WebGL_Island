namespace CIG
{
	public class WorldMapUnlockIslandTutorial : Tutorial
	{
		public enum TutorialState
		{
			None,
			ClickIsland,
			PressUnlockIsland,
			WaitForUnlock,
			WaitForVisit,
			ReturnToWorldMap
		}

		public const IslandId UnlockIsland = IslandId.Island03;

		public const int OverrideTravelDuration = 5;

		public const decimal OverrideCashCost = 0m;

		public const decimal OverrideGoldCost = 0m;

		private readonly WorldMap _worldMap;

		private readonly AirshipState _airship;

		private TutorialState _state;

		public override TutorialType TutorialType => TutorialType.WorldMapUnlockIsland;

		public override bool CanBegin
		{
			get
			{
				if (base.CanBegin && _tutorialManager.IsTutorialFinished(TutorialType.WorldMapStart))
				{
					return _airship.CurrentState == AirshipState.State.Landed;
				}
				return false;
			}
		}

		public override bool CanQuit => false;

		public TutorialState State
		{
			get
			{
				return _state;
			}
			private set
			{
				if (CanChangeState(value))
				{
					_state = value;
					StateChanged((int)_state);
				}
			}
		}

		public bool IsIslandUnlocked => _islandsManager.IsUnlocked(IslandId.Island03);

		public WorldMapUnlockIslandTutorial(StorageDictionary storage, TutorialManager tutorialManager, IslandsManager islandsManager, PopupManager popupManager, WorldMap worldMap)
			: base(storage, tutorialManager, islandsManager, popupManager)
		{
			_state = TutorialState.None;
			_worldMap = worldMap;
			_airship = _worldMap.Airship;
		}

		public override void Begin()
		{
			base.Begin();
			bool flag = _airship.CurrentState == AirshipState.State.Hovering && _airship.CurrentIslandId == IslandId.Island03;
			if (IsIslandUnlocked && !flag)
			{
				Finish();
				return;
			}
			_islandsManager.IslandUnlockedEvent += OnIslandUnlocked;
			_islandsManager.IslandChangedEvent += OnIslandChanged;
			_airship.StateChangedEvent += OnAirshipStateChanged;
			_worldMap.VisibilityChangedEvent += OnWorldMapVisibilityChanged;
			State = TutorialState.ClickIsland;
		}

		public override void Release()
		{
			_state = TutorialState.None;
			if (_islandsManager != null)
			{
				_islandsManager.IslandUnlockedEvent -= OnIslandUnlocked;
				_islandsManager.IslandChangedEvent -= OnIslandChanged;
			}
			if (_airship != null)
			{
				_airship.StateChangedEvent -= OnAirshipStateChanged;
			}
			if (_worldMap != null)
			{
				_worldMap.VisibilityChangedEvent -= OnWorldMapVisibilityChanged;
			}
			base.Release();
		}

		public void OverlayToggled(bool active)
		{
			if (!IsIslandUnlocked)
			{
				State = ((!active) ? TutorialState.ClickIsland : TutorialState.PressUnlockIsland);
			}
		}

		private void OnIslandChanged(IslandId islandId, bool isVisiting)
		{
			if (islandId == IslandId.Island03)
			{
				Finish();
			}
		}

		private bool CanChangeState(TutorialState newState)
		{
			switch (_state)
			{
			case TutorialState.None:
				return true;
			case TutorialState.ClickIsland:
			case TutorialState.PressUnlockIsland:
			case TutorialState.WaitForVisit:
				if ((uint)newState <= 5u)
				{
					return true;
				}
				break;
			case TutorialState.WaitForUnlock:
				if ((uint)newState <= 1u || newState == TutorialState.ReturnToWorldMap)
				{
					return IsIslandUnlocked;
				}
				break;
			case TutorialState.ReturnToWorldMap:
				if ((uint)newState <= 2u || (uint)(newState - 4) <= 1u)
				{
					return true;
				}
				break;
			}
			return false;
		}

		private void OnIslandUnlocked(IslandId islandId)
		{
			State = (_worldMap.IsVisible ? TutorialState.ClickIsland : TutorialState.ReturnToWorldMap);
		}

		private void OnAirshipStateChanged(AirshipState.State previousState, AirshipState.State newState)
		{
			if (newState == AirshipState.State.Travelling)
			{
				_worldMap.Airship.OverrideTravelDuration(5.0);
				State = (_worldMap.IsVisible ? TutorialState.WaitForUnlock : TutorialState.ReturnToWorldMap);
			}
		}

		private void OnWorldMapVisibilityChanged(bool visible)
		{
			State = (visible ? TutorialState.WaitForVisit : TutorialState.ReturnToWorldMap);
		}
	}
}
