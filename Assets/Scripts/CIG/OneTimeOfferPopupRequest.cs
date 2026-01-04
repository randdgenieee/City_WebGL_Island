namespace CIG
{
	public class OneTimeOfferPopupRequest : PopupRequest
	{
		public OneTimeOfferBase OneTimeOffer
		{
			get;
		}

		public OneTimeOfferPopupRequest(OneTimeOfferBase oneTimeOffer, bool enqueue)
			: base(typeof(OneTimeOfferPopup), enqueue, dismissable: false, showModalBackground: true, firstInQueue: false, HUDRegionType.All)
		{
			OneTimeOffer = oneTimeOffer;
		}
	}
}
