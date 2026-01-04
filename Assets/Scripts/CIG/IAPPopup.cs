namespace CIG
{
	public abstract class IAPPopup : ToggleableInteractionPopup
	{
		protected PurchaseHandler _purchaseHandler;

		public void Initialize(PurchaseHandler purchaseHandler)
		{
			_purchaseHandler = purchaseHandler;
		}
	}
}
