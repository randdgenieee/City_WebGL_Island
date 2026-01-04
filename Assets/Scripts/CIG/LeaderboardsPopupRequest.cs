namespace CIG
{
	public class LeaderboardsPopupRequest : PopupRequest
	{
		public LeaderboardsPopup.RegionTab? RegionTab
		{
			get;
		}

		public LeaderboardsPopup.PositionTab? PositionTab
		{
			get;
		}

		public LeaderboardsPopupRequest(LeaderboardsPopup.RegionTab? regionTab = default(LeaderboardsPopup.RegionTab?), LeaderboardsPopup.PositionTab? positionTab = default(LeaderboardsPopup.PositionTab?))
			: base(typeof(LeaderboardsPopup))
		{
			RegionTab = regionTab;
			PositionTab = positionTab;
		}
	}
}
