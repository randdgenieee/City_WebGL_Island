using CIG;

public class TreasureChestPopupRequest : PopupRequest
{
	public TreasureChestRewards Rewards
	{
		get;
		private set;
	}

	public TreasureChestPopupRequest(TreasureChestRewards rewards)
		: base(typeof(TreasureChestPopup), enqueue: true, dismissable: false, showModalBackground: true, firstInQueue: true, HUDRegionType.All)
	{
		Rewards = rewards;
	}
}
