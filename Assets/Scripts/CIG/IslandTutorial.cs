namespace CIG
{
	public abstract class IslandTutorial : Tutorial
	{
		private readonly WorldMap _worldMap;

		public override bool CanBegin
		{
			get
			{
				if (base.CanBegin)
				{
					return !_worldMap.IsVisible;
				}
				return false;
			}
		}

		protected IslandTutorial(StorageDictionary storage, TutorialManager tutorialManager, IslandsManager islandsManager, PopupManager popupManager, WorldMap worldMap)
			: base(storage, tutorialManager, islandsManager, popupManager)
		{
			_worldMap = worldMap;
		}
	}
}
