namespace CIG
{
	public class WarehouseTutorial : IslandTutorial
	{
		public enum TutorialState
		{
			None,
			OpenDialog
		}

		private readonly BuildingWarehouseManager _warehouseManager;

		private TutorialState _state;

		public override TutorialType TutorialType => TutorialType.Warehouse;

		public override bool CanQuit => false;

		public override bool CanBegin
		{
			get
			{
				if (base.CanBegin && _tutorialManager.InitialTutorialFinished)
				{
					return _warehouseManager.NewBuildingCount > 0;
				}
				return false;
			}
		}

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

		public WarehouseTutorial(StorageDictionary storage, TutorialManager tutorialManager, IslandsManager islandsManager, PopupManager popupManager, WorldMap worldMap, BuildingWarehouseManager warehouseManager)
			: base(storage, tutorialManager, islandsManager, popupManager, worldMap)
		{
			State = TutorialState.None;
			_warehouseManager = warehouseManager;
		}

		public override void Begin()
		{
			base.Begin();
			State = TutorialState.OpenDialog;
		}
	}
}
