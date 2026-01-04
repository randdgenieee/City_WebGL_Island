namespace CIG
{
	public class BuildingBuiltTutorialTrigger : TutorialTrigger
	{
		private readonly GameStats _gameStats;

		protected override bool CanStart => true;

		public BuildingBuiltTutorialTrigger(TutorialManager tutorialManager, GameStats gameStats)
			: base(tutorialManager)
		{
			_gameStats = gameStats;
		}

		protected override void EnableImpl()
		{
			_gameStats.ValueChangedEvent += OnValueChanged;
		}

		protected override void DisableImpl()
		{
			_gameStats.ValueChangedEvent -= OnValueChanged;
		}

		private void OnValueChanged(string key, object oldValue, object newValue)
		{
			if (key == "NumberOfBuildings")
			{
				TryStartTutorial();
			}
		}
	}
}
