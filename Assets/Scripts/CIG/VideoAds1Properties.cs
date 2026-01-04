namespace CIG
{
	[BalancePropertyClass("videoAds1", true)]
	public class VideoAds1Properties : VideoAdsPropertiesBase
	{
		private const string MinSilverKeysKey = "minSilverKeys";

		private const string MaxSilverKeysKey = "maxSilverKeys";

		private const string MinGoldKeysKey = "minGoldKeys";

		private const string MaxGoldKeysKey = "maxGoldKeys";

		private const string GoldKeysChanceKey = "goldKeysChance";

		[BalanceProperty("minSilverKeys")]
		public int MinSilverKeys
		{
			get;
			private set;
		}

		[BalanceProperty("maxSilverKeys")]
		public int MaxSilverKeys
		{
			get;
			private set;
		}

		[BalanceProperty("minGoldKeys")]
		public int MinGoldKeys
		{
			get;
			private set;
		}

		[BalanceProperty("maxGoldKeys")]
		public int MaxGoldKeys
		{
			get;
			private set;
		}

		[BalanceProperty("goldKeysChance")]
		public float GoldKeysChance
		{
			get;
			private set;
		}

		public VideoAds1Properties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			MinSilverKeys = GetProperty("minSilverKeys", 0);
			MaxSilverKeys = GetProperty("maxSilverKeys", 0);
			MinGoldKeys = GetProperty("minGoldKeys", 0);
			MaxGoldKeys = GetProperty("maxGoldKeys", 0);
			GoldKeysChance = GetProperty("goldKeysChance", 0f);
		}
	}
}
