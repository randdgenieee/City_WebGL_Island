namespace CIG
{
	public class TutorialIntermediary
	{
		public delegate void TutorialStartedEventHandler(Tutorial tutorial);

		public delegate void TutorialStoppedEventHandler(Tutorial tutorial);

		public delegate void TutorialFinishedEventHandler(Tutorial tutorial);

		private TutorialManager _tutorialManager;

		public bool InitialTutorialFinished => _tutorialManager?.InitialTutorialFinished ?? false;

		public bool HasActiveTutorial => _tutorialManager?.ActiveTutorial != null;

		public event TutorialStartedEventHandler TutorialStartedEvent;

		public event TutorialStoppedEventHandler TutorialStoppedEvent;

		public event TutorialFinishedEventHandler TutorialFinishedEvent;

		private void FireTutorialStartedEvent(Tutorial tutorial)
		{
			this.TutorialStartedEvent?.Invoke(tutorial);
		}

		private void FireTutorialStoppedEvent(Tutorial tutorial)
		{
			this.TutorialStoppedEvent?.Invoke(tutorial);
		}

		private void FireTutorialFinishedEvent(Tutorial tutorial)
		{
			this.TutorialFinishedEvent?.Invoke(tutorial);
		}

		public void SetTutorialManager(TutorialManager tutorialManager)
		{
			if (_tutorialManager != null)
			{
				_tutorialManager.TutorialStartedEvent -= OnTutorialStarted;
				_tutorialManager.TutorialStoppedEvent -= OnTutorialStopped;
				_tutorialManager.TutorialFinishedEvent -= OnTutorialFinished;
			}
			_tutorialManager = tutorialManager;
			_tutorialManager.TutorialStartedEvent += OnTutorialStarted;
			_tutorialManager.TutorialStoppedEvent += OnTutorialStopped;
			_tutorialManager.TutorialFinishedEvent += OnTutorialFinished;
		}

		private void OnTutorialStarted(Tutorial tutorial)
		{
			FireTutorialStartedEvent(tutorial);
		}

		private void OnTutorialStopped(Tutorial tutorial)
		{
			FireTutorialStoppedEvent(tutorial);
		}

		private void OnTutorialFinished(Tutorial tutorial)
		{
			FireTutorialFinishedEvent(tutorial);
		}
	}
}
