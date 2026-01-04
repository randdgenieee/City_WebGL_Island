namespace CIG
{
	public abstract class TutorialTrigger
	{
		private readonly TutorialManager _tutorialManager;

		private bool _enabled;

		protected abstract bool CanStart
		{
			get;
		}

		protected TutorialTrigger(TutorialManager tutorialManager)
		{
			_tutorialManager = tutorialManager;
		}

		public void Enable()
		{
			if (!_enabled)
			{
				EnableImpl();
				_enabled = true;
			}
		}

		public void Disable()
		{
			if (_enabled)
			{
				DisableImpl();
				_enabled = false;
			}
		}

		protected abstract void EnableImpl();

		protected abstract void DisableImpl();

		protected void TryStartTutorial()
		{
			if (CanStart)
			{
				_tutorialManager.TryStartTutorial();
			}
		}

		protected void TryStartTutorial(TutorialType type)
		{
			if (CanStart)
			{
				_tutorialManager.TryStartTutorial(type);
			}
		}
	}
}
