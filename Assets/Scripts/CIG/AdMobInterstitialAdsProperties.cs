namespace CIG
{
	[BalancePropertyClass("admobInterstitials", true)]
	public class AdMobInterstitialAdsProperties : BaseProperties
	{
		private const string PlaystorePayerAdIdKey = "playstorePayerAdId";

		private const string PlaystoreNonPayerAdIdKey = "playstoreNonPayerAdId";

		private const string iOSPayerAdIdKey = "iosPayerAdId";

		private const string iOSNonPayerAdIdKey = "iosNonPayerAdId";

		[BalanceProperty("playstorePayerAdId")]
		public string PlaystorePayerAdId
		{
			get;
		}

		[BalanceProperty("playstoreNonPayerAdId")]
		public string PlaystoreNonPayerAdId
		{
			get;
		}

		[BalanceProperty("iosPayerAdId")]
		public string iOSPayerAdId
		{
			get;
		}

		[BalanceProperty("iosNonPayerAdId")]
		public string iOSNonPayerAdId
		{
			get;
		}

		public AdMobInterstitialAdsProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			PlaystorePayerAdId = GetProperty("playstorePayerAdId", string.Empty);
			PlaystoreNonPayerAdId = GetProperty("playstoreNonPayerAdId", string.Empty);
			iOSPayerAdId = GetProperty("iosPayerAdId", string.Empty);
			iOSNonPayerAdId = GetProperty("iosNonPayerAdId", string.Empty);
		}
	}
}
