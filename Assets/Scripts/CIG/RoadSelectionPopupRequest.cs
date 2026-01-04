namespace CIG
{
	public class RoadSelectionPopupRequest : PopupRequest
	{
		public RoadSelectionPopupRequest()
			: base(typeof(RoadSelectionPopup), enqueue: true, dismissable: true, showModalBackground: false, firstInQueue: false, HUDRegionType.Level | HUDRegionType.CurrencyBars | HUDRegionType.PopulationBars | HUDRegionType.Quests | HUDRegionType.ShopButton | HUDRegionType.RoadsButton | HUDRegionType.MapButton | HUDRegionType.MinigamesButton | HUDRegionType.LeaderboardButton | HUDRegionType.SocialButton | HUDRegionType.SettingsButton | HUDRegionType.UpgradesButton | HUDRegionType.KeyDealsButton | HUDRegionType.WarehouseButton | HUDRegionType.FlyingStartDealButton)
		{
		}
	}
}
