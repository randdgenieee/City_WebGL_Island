namespace CIG
{
	public class StartTutorial : IslandTutorial
	{
		public enum TutorialState
		{
			None,
			WelcomeText,
			BuildCityText,
			LetsGoText
		}

		private TutorialState _state;

		public override TutorialType TutorialType => TutorialType.Start;

		public override bool CanQuit => true;

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

		public StartTutorial(StorageDictionary storage, TutorialManager tutorialManager, IslandsManager islandsManager, PopupManager popupManager, WorldMap worldMap)
			: base(storage, tutorialManager, islandsManager, popupManager, worldMap)
		{
		}

		public override void Begin()
		{
			base.Begin();
			State = TutorialState.WelcomeText;
		}

		public void TextShown()
		{
			switch (_state)
			{
			default:
				State = TutorialState.WelcomeText;
				break;
			case TutorialState.WelcomeText:
				State = TutorialState.BuildCityText;
				break;
			case TutorialState.BuildCityText:
				State = TutorialState.LetsGoText;
				break;
			case TutorialState.LetsGoText:
				Finish();
				break;
			}
		}
	}
}
