namespace CIG
{
	public enum IAPStorePurchaseFailedError
	{
		ProductNull,
		StoreLoadingError,
		StoreNull,
		PurchasingUnavailable,
		ExistingPurchasePending,
		ProductUnavailable,
		SignatureInvalid,
		UserCancelled,
		PaymentDeclined,
		DuplicateTransaction,
		Unknown
	}
}
