namespace CIG
{
	public class BuildCountTutorialTrigger : TutorialTrigger
	{
		private readonly CraneManager _craneManager;

		private readonly int _diff;

		protected override bool CanStart => _craneManager.MaxBuildCount - _craneManager.CurrentBuildCount <= _diff;

		public BuildCountTutorialTrigger(TutorialManager tutorialManager, CraneManager craneManager, int diff)
			: base(tutorialManager)
		{
			_craneManager = craneManager;
			_diff = diff;
		}

		protected override void EnableImpl()
		{
			_craneManager.BuildCountChangedEvent += OnBuildCountChanged;
		}

		protected override void DisableImpl()
		{
			if (_craneManager != null)
			{
				_craneManager.BuildCountChangedEvent -= OnBuildCountChanged;
			}
		}

		private void OnBuildCountChanged(int used, int total)
		{
			TryStartTutorial();
		}
	}
}
