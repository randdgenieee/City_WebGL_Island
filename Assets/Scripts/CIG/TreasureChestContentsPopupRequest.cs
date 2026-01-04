namespace CIG
{
	public class TreasureChestContentsPopupRequest : PopupRequest
	{
		public TreasureChest TreasureChest
		{
			get;
		}

		public PurchaseHandler PurchaseHandler
		{
			get;
		}

		public TOCIStoreProduct StoreProduct
		{
			get;
		}

		public TreasureChestContentsPopupRequest(TreasureChest treasureChest, PurchaseHandler purchaseHandler, TOCIStoreProduct storeProduct)
			: base(typeof(TreasureChestContentsPopup), enqueue: false)
		{
			TreasureChest = treasureChest;
			PurchaseHandler = purchaseHandler;
			StoreProduct = storeProduct;
		}
	}
}
