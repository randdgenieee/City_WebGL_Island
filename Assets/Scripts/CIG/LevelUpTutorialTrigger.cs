namespace CIG
{
	public class LevelUpTutorialTrigger : TutorialTrigger
	{
		private readonly GameState _gameState;

		protected override bool CanStart => true;

		public LevelUpTutorialTrigger(TutorialManager tutorialManager, GameState gameState)
			: base(tutorialManager)
		{
			_gameState = gameState;
		}

		protected override void EnableImpl()
		{
			_gameState.VisuallyLevelledUpEvent += OnVisuallyLevelledUp;
		}

		protected override void DisableImpl()
		{
			if (_gameState != null)
			{
				_gameState.VisuallyLevelledUpEvent -= OnVisuallyLevelledUp;
			}
		}

		private void OnVisuallyLevelledUp(int level)
		{
			TryStartTutorial();
		}
	}
}
