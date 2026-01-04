using System.Collections.Generic;

namespace CIG
{
	[BalancePropertyClass("craneOfferManager", true)]
	public class CraneOfferManagerProperties : BaseProperties
	{
		private const string RequiredCloseHireCraneCountKey = "requiredCloseHireCraneCount";

		private const string MaxShowCountKey = "maxShowCount";

		private const string OfferDurationSecondsKey = "offerDurationSeconds";

		private const string OfferDelaySecondsKey = "offerDelaySeconds";

		private const string ProductCraneCountKey = "productCraneCount";

		private const string OfferOrderKey = "offerOrder";

		[BalanceProperty("requiredCloseHireCraneCount")]
		public int RequiredCloseHireCraneCount
		{
			get;
		}

		[BalanceProperty("maxShowCount")]
		public int MaxShowCount
		{
			get;
		}

		[BalanceProperty("offerDurationSeconds")]
		public double OfferDurationSeconds
		{
			get;
		}

		[BalanceProperty("offerDelaySeconds")]
		public double OfferDelaySeconds
		{
			get;
		}

		[BalanceProperty("productCraneCount")]
		public int ProductCraneCount
		{
			get;
		}

		public List<CraneOfferIAPProperties> OfferIAPProperties
		{
			get;
		}

		public List<CraneOfferCurrencyProperties> OfferCurrencyProperties
		{
			get;
		}

		[BalanceProperty("offerOrder")]
		public List<string> OfferOrder
		{
			get;
		}

		public CraneOfferManagerProperties(PropertiesDictionary propsDict, string baseKey, List<CraneOfferIAPProperties> offerIAPProperties, List<CraneOfferCurrencyProperties> offerCurrencyProperties)
			: base(propsDict, baseKey)
		{
			RequiredCloseHireCraneCount = GetProperty("requiredCloseHireCraneCount", 0);
			MaxShowCount = GetProperty("maxShowCount", 0);
			OfferDurationSeconds = GetProperty("offerDurationSeconds", 0.0);
			OfferDelaySeconds = GetProperty("offerDelaySeconds", 0.0);
			ProductCraneCount = GetProperty("productCraneCount", 0);
			OfferIAPProperties = offerIAPProperties;
			OfferCurrencyProperties = offerCurrencyProperties;
			OfferOrder = GetProperty("offerOrder", new List<string>());
		}
	}
}
