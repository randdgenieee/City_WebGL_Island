namespace CIG
{
	public class LinkConfirmPopupRequest : PopupRequest
	{
		public string LinkCode
		{
			get;
			private set;
		}

		public GameSparksUser.UserMetaData MetaData
		{
			get;
			private set;
		}

		public LinkConfirmPopupRequest(string linkCode, GameSparksUser.UserMetaData metaData)
			: base(typeof(LinkConfirmPopup), enqueue: false)
		{
			LinkCode = linkCode;
			MetaData = metaData;
		}
	}
}
