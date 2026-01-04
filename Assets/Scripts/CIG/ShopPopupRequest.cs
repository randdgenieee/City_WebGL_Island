namespace CIG
{
	public class ShopPopupRequest : PopupRequest
	{
		public ShopMenuTabs? Tab
		{
			get;
		}

		public ShopPopupRequest(ShopMenuTabs? tab = default(ShopMenuTabs?), bool firstInQueue = false)
			: base(typeof(ShopPopup), enqueue: true, dismissable: true, showModalBackground: true, firstInQueue)
		{
			Tab = tab;
		}
	}
}
