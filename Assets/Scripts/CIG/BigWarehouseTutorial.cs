namespace CIG
{
	public class BigWarehouseTutorial : IslandTutorial
	{
		public enum TutorialState
		{
			None = 0,
			IntroText1 = 1,
			IntroText2 = 2,
			OpenBuilding = 3,
			BuildingOpened = 4,
			OpenWarehouse = 6,
			WarehouseOpened1 = 7,
			WarehouseOpened2 = 8,
			FinishText = 9
		}

		private readonly GameStats _gameStats;

		private readonly BuildingWarehouseManager _buildingWarehouseManager;

		private readonly bool _expansionWarehouseTutorialsEnabled;

		private TutorialState _state;

		public override TutorialType TutorialType => TutorialType.BigWarehouse;

		public override bool CanBegin
		{
			get
			{
				if (base.CanBegin && _expansionWarehouseTutorialsEnabled && _gameStats.NumberOfBuildings >= 10)
				{
					return FindBuilding();
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
				_state = value;
				StateChanged((int)_state);
			}
		}

		public Building Building
		{
			get;
			private set;
		}

		public BigWarehouseTutorial(StorageDictionary storage, TutorialManager tutorialManager, IslandsManager islandsManager, PopupManager popupManager, WorldMap worldMap, GameStats gameStats, BuildingWarehouseManager buildingWarehouseManager, bool expansionWarehouseTutorialsEnabled)
			: base(storage, tutorialManager, islandsManager, popupManager, worldMap)
		{
			_gameStats = gameStats;
			_buildingWarehouseManager = buildingWarehouseManager;
			_expansionWarehouseTutorialsEnabled = expansionWarehouseTutorialsEnabled;
		}

		public override void Release()
		{
			base.Release();
			_buildingWarehouseManager.SlotUnlockedEvent -= OnSlotUnlocked;
		}

		public override void Begin()
		{
			base.Begin();
			State = TutorialState.IntroText1;
			_buildingWarehouseManager.SlotUnlockedEvent += OnSlotUnlocked;
		}

		public void TextDismissed()
		{
			switch (_state)
			{
			case TutorialState.OpenBuilding:
			case (TutorialState)5:
			case TutorialState.OpenWarehouse:
				break;
			case TutorialState.None:
				State = TutorialState.IntroText1;
				break;
			case TutorialState.IntroText1:
				State = TutorialState.IntroText2;
				break;
			case TutorialState.IntroText2:
				State = TutorialState.OpenBuilding;
				break;
			case TutorialState.BuildingOpened:
				State = TutorialState.OpenWarehouse;
				break;
			case TutorialState.WarehouseOpened1:
				State = TutorialState.WarehouseOpened2;
				break;
			case TutorialState.WarehouseOpened2:
				State = TutorialState.FinishText;
				break;
			case TutorialState.FinishText:
				Finish();
				break;
			}
		}

		public void OnIslandChanged(IsometricIsland isometricIsland)
		{
			Building = null;
			if (isometricIsland == null || !FindBuilding())
			{
				Stop();
			}
		}

		public void TileRemoved()
		{
			if (FindBuilding())
			{
				FireStateChangedEvent();
			}
			else
			{
				Stop();
			}
		}

		protected override void OnPopupOpened(PopupRequest request)
		{
			base.OnPopupOpened(request);
			BuildingPopupRequest buildingPopupRequest;
			if ((buildingPopupRequest = (request as BuildingPopupRequest)) != null && buildingPopupRequest.Content == BuildingPopupContent.Upgrade)
			{
				State = TutorialState.BuildingOpened;
			}
			else if (request is BuildingWarehousePopupRequest)
			{
				State = TutorialState.WarehouseOpened1;
			}
		}

		protected override void OnPopupClosed(PopupRequest request, bool instant)
		{
			base.OnPopupClosed(request, instant);
			BuildingPopupRequest buildingPopupRequest;
			if ((State == TutorialState.BuildingOpened && (buildingPopupRequest = (request as BuildingPopupRequest)) != null && buildingPopupRequest.Content == BuildingPopupContent.Upgrade) || (State == TutorialState.WarehouseOpened1 && request is BuildingWarehousePopupRequest))
			{
				State = TutorialState.OpenWarehouse;
			}
			else if (State == TutorialState.FinishText && request is BuildingWarehousePopupRequest)
			{
				Finish();
			}
		}

		private bool FindBuilding()
		{
			IsometricIsland current = IsometricIsland.Current;
			if (current != null && Building == null && current.BuildingsOnIsland.Count > 0)
			{
				Building = current.BuildingsOnIsland[0];
			}
			return Building != null;
		}

		private void OnSlotUnlocked()
		{
			if (State == TutorialState.WarehouseOpened2)
			{
				State = TutorialState.FinishText;
			}
		}
	}
}
