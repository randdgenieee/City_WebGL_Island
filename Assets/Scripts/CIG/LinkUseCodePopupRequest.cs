namespace CIG
{
	public class LinkUseCodePopupRequest : PopupRequest
	{
		public LinkUseCodePopupRequest()
			: base(typeof(LinkUseCodePopup), enqueue: false)
		{
		}
	}
}
