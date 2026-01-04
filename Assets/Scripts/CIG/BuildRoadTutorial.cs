namespace CIG
{
	public class BuildRoadTutorial : IslandTutorial
	{
		public override TutorialType TutorialType => TutorialType.BuildRoad;

		public override bool CanBegin
		{
			get
			{
				if (base.CanBegin)
				{
					return _tutorialManager.IsTutorialFinished(TutorialType.BuildResidential);
				}
				return false;
			}
		}

		public override bool CanQuit => true;

		public BuildRoadTutorial(StorageDictionary storage, TutorialManager tutorialManager, IslandsManager islandsManager, PopupManager popupManager, WorldMap worldMap)
			: base(storage, tutorialManager, islandsManager, popupManager, worldMap)
		{
		}

		public void OnRoadBuilt()
		{
			Finish();
		}
	}
}
