namespace CIG
{
	public class FishingMinigamePopupRequest : PopupRequest
	{
		public FishingMinigame FishingMinigame
		{
			get;
		}

		public FishingMinigamePopupRequest(FishingMinigame fishingMinigame)
			: base(typeof(FishingMinigamePopup), enqueue: true, dismissable: false, showModalBackground: false, firstInQueue: false, HUDRegionType.All)
		{
			FishingMinigame = fishingMinigame;
		}
	}
}
