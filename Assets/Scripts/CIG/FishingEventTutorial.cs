namespace CIG
{
	public class FishingEventTutorial : Tutorial
	{
		public enum TutorialState
		{
			None,
			FocusOnLocation,
			FocusOnLocationAnimation,
			Dialog
		}

		private readonly FishingEvent _fishingEvent;

		private TutorialState _state;

		public override TutorialType TutorialType => TutorialType.FishingEvent;

		public override bool CanBegin
		{
			get
			{
				if (base.CanBegin && _fishingEvent.IsActive)
				{
					return _fishingEvent.FishingQuest.IsActive;
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
			set
			{
				if (_state != value)
				{
					_state = value;
					StateChanged((int)_state);
				}
			}
		}

		public FishingLocation FishingLocation
		{
			get;
			private set;
		}

		public FishingEventTutorial(StorageDictionary storage, TutorialManager tutorialManager, IslandsManager islandsManager, PopupManager popupManager, FishingEvent fishingEvent)
			: base(storage, tutorialManager, islandsManager, popupManager)
		{
			_fishingEvent = fishingEvent;
			_state = TutorialState.None;
		}

		public override void Begin()
		{
			base.Begin();
			State = TutorialState.FocusOnLocation;
		}

		public void OnIslandChanged(IsometricIsland isometricIsland)
		{
			if (isometricIsland == null)
			{
				Stop();
				return;
			}
			State = TutorialState.FocusOnLocationAnimation;
			isometricIsland.FishingLocationManager.FocusOnRandomLocation(OnFocussedOnLocation);
		}

		protected override void OnPopupOpened(PopupRequest request)
		{
			if (request is FishingMinigamePopupRequest)
			{
				Finish();
			}
			else
			{
				base.OnPopupOpened(request);
			}
		}

		private void OnFocussedOnLocation(FishingLocation fishingLocation)
		{
			FishingLocation = fishingLocation;
			State = TutorialState.Dialog;
		}
	}
}
