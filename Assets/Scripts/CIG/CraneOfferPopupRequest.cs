namespace CIG
{
	public class CraneOfferPopupRequest : PopupRequest
	{
		public CraneOffer CraneOffer
		{
			get;
		}

		public CraneOfferPopupRequest(CraneOffer craneOffer)
			: base(typeof(CraneOfferPopup), enqueue: true, dismissable: false, showModalBackground: true, firstInQueue: false, HUDRegionType.All)
		{
			CraneOffer = craneOffer;
		}
	}
}
