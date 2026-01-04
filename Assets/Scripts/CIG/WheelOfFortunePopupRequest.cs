namespace CIG
{
	public class WheelOfFortunePopupRequest : PopupRequest
	{
		public WheelOfFortunePopupRequest()
			: base(typeof(WheelOfFortunePopup), enqueue: true, dismissable: true, showModalBackground: true, firstInQueue: false, HUDRegionType.All)
		{
		}
	}
}
