namespace CIG
{
	public class CommunityWalkerBalloon : WalkerBalloon
	{
		public override bool IsAvailable
		{
			get
			{
				if (!base.IsAvailable)
				{
					return false;
				}
				CIGIslandState islandState = IsometricIsland.Current.IslandState;
				return islandState.Housing - islandState.Happiness >= 5;
			}
		}

		public CommunityWalkerBalloon(WalkerBalloonProperties properties, GameState gameState, PopupManager popupManager, RoutineRunner routineRunner)
			: base(properties, gameState, popupManager, routineRunner)
		{
		}

		protected override void OnCollected()
		{
			base.OnCollected();
			ShowPopup("community_walker_balloon", "feedback_build_community_buildings", SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.HappinessIcon));
			Analytics.WalkerExclamationBalloonClicked(_properties.BalloonType.ToString());
		}

		protected override void PopupAction()
		{
			_popupManager.RequestPopup(new ShopPopupRequest(ShopMenuTabs.Community));
		}
	}
}
