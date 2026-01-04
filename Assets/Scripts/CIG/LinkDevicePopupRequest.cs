namespace CIG
{
	public class LinkDevicePopupRequest : PopupRequest
	{
		public LinkDevicePopupRequest()
			: base(typeof(LinkDevicePopup), enqueue: false)
		{
		}
	}
}
