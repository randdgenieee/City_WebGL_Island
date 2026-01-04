namespace CIG
{
	public class WorldMapWrapUpTutorial : IslandTutorial
	{
		public enum TutorialState
		{
			None,
			BuildMoreText,
			UnlockMoreIslandsText
		}

		private TutorialState _state;

		public override TutorialType TutorialType => TutorialType.WorldMapWrapUp;

		public override bool CanBegin
		{
			get
			{
				if (base.CanBegin && _tutorialManager.IsTutorialFinished(TutorialType.WorldMapUnlockIsland))
				{
					return _islandsManager.CurrentIsland != IslandId.Island01;
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

		public WorldMapWrapUpTutorial(StorageDictionary storage, TutorialManager tutorialManager, IslandsManager islandsManager, PopupManager popupManager, WorldMap worldMap)
			: base(storage, tutorialManager, islandsManager, popupManager, worldMap)
		{
		}

		public override void Begin()
		{
			base.Begin();
			State = TutorialState.BuildMoreText;
		}

		public void TextShown()
		{
			switch (_state)
			{
			default:
				State = TutorialState.BuildMoreText;
				break;
			case TutorialState.BuildMoreText:
				State = TutorialState.UnlockMoreIslandsText;
				break;
			case TutorialState.UnlockMoreIslandsText:
				Finish();
				break;
			}
		}
	}
}
