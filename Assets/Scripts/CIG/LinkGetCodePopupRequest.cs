namespace CIG
{
	public class LinkGetCodePopupRequest : PopupRequest
	{
		public GameSparksUser.LinkCode LinkCode
		{
			get;
			private set;
		}

		public LinkGetCodePopupRequest(GameSparksUser.LinkCode linkCode)
			: base(typeof(LinkGetCodePopup), enqueue: false)
		{
			LinkCode = linkCode;
		}
	}
}
