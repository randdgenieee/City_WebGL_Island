namespace CIG
{
	public class BuildResidentialTutorial : BuildingTutorial
	{
		public override TutorialType TutorialType => TutorialType.BuildResidential;

		public override bool CanBegin
		{
			get
			{
				if (base.CanBegin)
				{
					return _tutorialManager.IsTutorialFinished(TutorialType.Start);
				}
				return false;
			}
		}

		public override bool CanQuit => true;

		public BuildResidentialTutorial(StorageDictionary storage, TutorialManager tutorialManager, IslandsManager islandsManager, PopupManager popupManager, WorldMap worldMap)
			: base(storage, tutorialManager, islandsManager, popupManager, worldMap)
		{
		}
	}
}
