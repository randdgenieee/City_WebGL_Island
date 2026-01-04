namespace CIG
{
	public class WarehouseBuildingAddedTutorialTrigger : TutorialTrigger
	{
		private readonly BuildingWarehouseManager _warehouseManager;

		protected override bool CanStart => _warehouseManager.AllBuildingsCount > 0;

		public WarehouseBuildingAddedTutorialTrigger(TutorialManager tutorialManager, BuildingWarehouseManager warehouseManager)
			: base(tutorialManager)
		{
			_warehouseManager = warehouseManager;
		}

		protected override void EnableImpl()
		{
			_warehouseManager.WarehouseBuildingAddedEvent += OnBuildingAdded;
		}

		protected override void DisableImpl()
		{
			_warehouseManager.WarehouseBuildingAddedEvent -= OnBuildingAdded;
		}

		private void OnBuildingAdded(BuildingProperties buildingProperties)
		{
			TryStartTutorial();
		}
	}
}
