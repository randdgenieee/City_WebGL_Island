namespace CIG
{
	public class WorldMapStartTutorial : WorldMapTutorial
	{
		public enum TutorialState
		{
			None,
			UnlockNewIslandsText,
			MoveAirshipText
		}

		private TutorialState _state;

		public override TutorialType TutorialType => TutorialType.WorldMapStart;

		public override bool CanBegin
		{
			get
			{
				if (base.CanBegin && _tutorialManager.InitialTutorialFinished && _worldMap.Airship.CurrentState == AirshipState.State.Landed)
				{
					return _islandsManager.IslandsUnlocked <= 1;
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
			private set
			{
				_state = value;
				StateChanged((int)_state);
			}
		}

		public WorldMapStartTutorial(StorageDictionary storage, TutorialManager tutorialManager, IslandsManager islandsManager, WorldMap worldMap, PopupManager popupManager)
			: base(storage, tutorialManager, islandsManager, popupManager, worldMap)
		{
		}

		public override void Begin()
		{
			base.Begin();
			State = TutorialState.UnlockNewIslandsText;
		}

		public void TextShown()
		{
			switch (_state)
			{
			default:
				State = TutorialState.UnlockNewIslandsText;
				break;
			case TutorialState.UnlockNewIslandsText:
				State = TutorialState.MoveAirshipText;
				break;
			case TutorialState.MoveAirshipText:
				Finish();
				break;
			}
		}
	}
}
