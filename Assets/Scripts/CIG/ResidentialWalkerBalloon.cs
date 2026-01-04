namespace CIG
{
	public class ResidentialWalkerBalloon : WalkerBalloon
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
				return islandState.Jobs - islandState.Housing >= 5;
			}
		}

		public ResidentialWalkerBalloon(WalkerBalloonProperties properties, GameState gameState, PopupManager popupManager, RoutineRunner routineRunner)
			: base(properties, gameState, popupManager, routineRunner)
		{
		}

		protected override void OnCollected()
		{
			base.OnCollected();
			ShowPopup("residential_walker_balloon", "feedback_build_residential_buildings", SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.CitizensIcon));
			Analytics.WalkerExclamationBalloonClicked(_properties.BalloonType.ToString());
		}

		protected override void PopupAction()
		{
			_popupManager.RequestPopup(new ShopPopupRequest(ShopMenuTabs.Residential));
		}
	}
}
