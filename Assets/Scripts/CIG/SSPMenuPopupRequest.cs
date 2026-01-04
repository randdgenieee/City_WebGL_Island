namespace CIG
{
	public class SSPMenuPopupRequest : PopupRequest
	{
		public SSPMenuPopup.SSPMenuTab? TabToOpen
		{
			get;
		}

		public SSPMenuPopupRequest(SSPMenuPopup.SSPMenuTab? tabToOpen = default(SSPMenuPopup.SSPMenuTab?))
			: base(typeof(SSPMenuPopup), enqueue: false)
		{
			TabToOpen = tabToOpen;
		}
	}
}
