namespace CIG
{
	public class TreasureChestAvailableTrigger : TutorialTrigger
	{
		private readonly TreasureChestManager _treasureChestManager;

		protected override bool CanStart => _treasureChestManager.HasOpenableChest;

		public TreasureChestAvailableTrigger(TutorialManager tutorialManager, TreasureChestManager treasureChestManager)
			: base(tutorialManager)
		{
			_treasureChestManager = treasureChestManager;
		}

		protected override void EnableImpl()
		{
			_treasureChestManager.ChestOpenableChangedEvent += OnTreasureChestOpenableChanged;
		}

		protected override void DisableImpl()
		{
			if (_treasureChestManager != null)
			{
				_treasureChestManager.ChestOpenableChangedEvent -= OnTreasureChestOpenableChanged;
			}
		}

		private void OnTreasureChestOpenableChanged()
		{
			if (CanStart)
			{
				TryStartTutorial();
			}
		}
	}
}
