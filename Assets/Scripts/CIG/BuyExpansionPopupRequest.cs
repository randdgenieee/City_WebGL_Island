namespace CIG
{
	public class BuyExpansionPopupRequest : PopupRequest
	{
		public ExpansionBlock Block
		{
			get;
			private set;
		}

		public BuyExpansionPopupRequest(ExpansionBlock block)
			: base(typeof(BuyExpansionPopup))
		{
			Block = block;
		}
	}
}
