namespace CIG
{
	public class FishingEventTutorialTrigger : TutorialTrigger
	{
		private readonly FishingEvent _fishingEvent;

		protected override bool CanStart
		{
			get
			{
				if (_fishingEvent.FishingQuest != null)
				{
					return _fishingEvent.FishingQuest.IsActive;
				}
				return false;
			}
		}

		public FishingEventTutorialTrigger(TutorialManager tutorialManager, FishingEvent fishingEvent)
			: base(tutorialManager)
		{
			_fishingEvent = fishingEvent;
		}

		protected override void EnableImpl()
		{
			_fishingEvent.QuestStartedEvent += OnQuestStarted;
		}

		protected override void DisableImpl()
		{
			_fishingEvent.QuestStartedEvent -= OnQuestStarted;
		}

		private void OnQuestStarted()
		{
			TryStartTutorial(TutorialType.FishingEvent);
		}
	}
}
