namespace CIG
{
	public class WorldMapTutorialTrigger : TutorialTrigger
	{
		private readonly WorldMap _worldMap;

		protected override bool CanStart => true;

		public WorldMapTutorialTrigger(TutorialManager tutorialManager, WorldMap worldMap)
			: base(tutorialManager)
		{
			_worldMap = worldMap;
		}

		protected override void EnableImpl()
		{
			_worldMap.VisibilityChangedEvent += OnWorlMapVisibilityChanged;
		}

		protected override void DisableImpl()
		{
			if (_worldMap != null)
			{
				_worldMap.VisibilityChangedEvent -= OnWorlMapVisibilityChanged;
			}
		}

		private void OnWorlMapVisibilityChanged(bool visible)
		{
			TryStartTutorial();
		}
	}
}
