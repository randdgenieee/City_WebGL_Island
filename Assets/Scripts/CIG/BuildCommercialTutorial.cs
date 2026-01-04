namespace CIG
{
	public class BuildCommercialTutorial : BuildingTutorial
	{
		public override TutorialType TutorialType => TutorialType.BuildCommercial;

		public override bool CanBegin
		{
			get
			{
				if (base.CanBegin)
				{
					return _tutorialManager.IsTutorialFinished(TutorialType.BuildRoad);
				}
				return false;
			}
		}

		public override bool CanQuit => true;

		public BuildCommercialTutorial(StorageDictionary storage, TutorialManager tutorialManager, IslandsManager islandsManager, PopupManager popupManager, WorldMap worldMap)
			: base(storage, tutorialManager, islandsManager, popupManager, worldMap)
		{
		}
	}
}
