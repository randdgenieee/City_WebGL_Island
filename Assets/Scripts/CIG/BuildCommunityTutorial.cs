namespace CIG
{
	public class BuildCommunityTutorial : BuildingTutorial
	{
		public override TutorialType TutorialType => TutorialType.BuildCommunity;

		public override bool CanBegin
		{
			get
			{
				if (base.CanBegin)
				{
					return _tutorialManager.IsTutorialFinished(TutorialType.BuildCommercial);
				}
				return false;
			}
		}

		public override bool CanQuit => true;

		public BuildCommunityTutorial(StorageDictionary storage, TutorialManager tutorialManager, IslandsManager islandsManager, PopupManager popupManager, WorldMap worldMap)
			: base(storage, tutorialManager, islandsManager, popupManager, worldMap)
		{
		}
	}
}
