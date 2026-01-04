namespace CIG
{
	public abstract class WorldMapTutorial : Tutorial
	{
		protected readonly WorldMap _worldMap;

		public override bool CanBegin
		{
			get
			{
				if (base.CanBegin)
				{
					return _worldMap.IsVisible;
				}
				return false;
			}
		}

		protected WorldMapTutorial(StorageDictionary storage, TutorialManager tutorialManager, IslandsManager islandsManager, PopupManager popupManager, WorldMap worldMap)
			: base(storage, tutorialManager, islandsManager, popupManager)
		{
			_worldMap = worldMap;
		}
	}
}
