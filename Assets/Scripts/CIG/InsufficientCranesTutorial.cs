namespace CIG
{
	public class InsufficientCranesTutorial : IslandTutorial
	{
		public enum TutorialState
		{
			None,
			InsufficientCranesText
		}

		private readonly CraneManager _craneManager;

		private TutorialState _state;

		public override TutorialType TutorialType => TutorialType.InsufficientCranes;

		public override bool CanBegin
		{
			get
			{
				if (base.CanBegin && _tutorialManager.InitialTutorialFinished)
				{
					return _craneManager.CurrentBuildCount >= _craneManager.MaxBuildCount;
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

		public InsufficientCranesTutorial(StorageDictionary storage, TutorialManager tutorialManager, IslandsManager islandsManager, PopupManager popupManager, WorldMap worldMap, CraneManager craneManager)
			: base(storage, tutorialManager, islandsManager, popupManager, worldMap)
		{
			_craneManager = craneManager;
		}

		public override void Begin()
		{
			base.Begin();
			State = TutorialState.InsufficientCranesText;
		}

		public void TextShown()
		{
			TutorialState state = _state;
			if (state != TutorialState.InsufficientCranesText)
			{
				State = TutorialState.InsufficientCranesText;
			}
			else
			{
				Finish();
			}
		}
	}
}
