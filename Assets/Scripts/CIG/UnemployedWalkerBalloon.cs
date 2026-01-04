namespace CIG
{
	public class UnemployedWalkerBalloon : WalkerBalloon
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
				return islandState.Population - islandState.Jobs >= 5;
			}
		}

		public UnemployedWalkerBalloon(WalkerBalloonProperties properties, GameState gameState, PopupManager popupManager, RoutineRunner routineRunner)
			: base(properties, gameState, popupManager, routineRunner)
		{
		}

		protected override void OnCollected()
		{
			base.OnCollected();
			ShowPopup("unemployed_walker_balloon", "feedback_build_commercial_buildings", SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.JobsIcon));
			Analytics.WalkerExclamationBalloonClicked(_properties.BalloonType.ToString());
		}

		protected override void PopupAction()
		{
			_popupManager.RequestPopup(new ShopPopupRequest(ShopMenuTabs.Commercial));
		}
	}
}
