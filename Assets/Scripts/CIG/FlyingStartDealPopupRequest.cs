namespace CIG
{
	public class FlyingStartDealPopupRequest : PopupRequest
	{
		private readonly FlyingStartDealManager _flyingStartDealManager;

		public override bool IsValid
		{
			get
			{
				if (_flyingStartDealManager.IsActive)
				{
					return _flyingStartDealManager.StoreProduct != null;
				}
				return false;
			}
		}

		public FlyingStartDealPopupRequest(FlyingStartDealManager flyingStartDealManager)
			: base(typeof(FlyingStartDealPopup))
		{
			_flyingStartDealManager = flyingStartDealManager;
		}
	}
}
