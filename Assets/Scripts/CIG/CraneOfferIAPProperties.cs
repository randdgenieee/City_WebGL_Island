namespace CIG
{
	[BalancePropertyClass("craneOfferIAP", false)]
	public class CraneOfferIAPProperties : BaseProperties
	{
		private const string IAPKey = "iap";

		[BalanceProperty("iap")]
		public string IAP
		{
			get;
		}

		public CraneOfferIAPProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			IAP = GetProperty("iap", string.Empty);
		}
	}
}
