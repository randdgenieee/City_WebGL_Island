namespace CIG
{
	[BalancePropertyClass("interstitialAds", true)]
	public class InterstitialAdsProperties : BaseProperties
	{
		private const string IngameIntervalSecondsKey = "ingameIntervalSeconds";

		private const string IngameMinutesPlayedKey = "ingameMinutesPlayed";

		private const string SecondsToFirstKey = "secondsToFirst";

		private const string LevelAmountKey = "levelAmount";

		private const string LevelMinutesPlayedKey = "levelMinutesPlayed";

		private const string MaxPerSessionKey = "maxPerSession";

		private const string CooldownAfterIapSecondsKey = "cooldownAfterIapSeconds";

		[BalanceProperty("ingameIntervalSeconds")]
		public int IngameIntervalSeconds
		{
			get;
			private set;
		}

		[BalanceProperty("ingameMinutesPlayed")]
		public int IngameMinutesPlayed
		{
			get;
			private set;
		}

		[BalanceProperty("secondsToFirst")]
		public int SecondsToFirst
		{
			get;
			private set;
		}

		[BalanceProperty("levelAmount")]
		public int LevelAmount
		{
			get;
			private set;
		}

		[BalanceProperty("levelMinutesPlayed")]
		public int LevelMinutesPlayed
		{
			get;
			private set;
		}

		[BalanceProperty("maxPerSession")]
		public int MaxPerSession
		{
			get;
			private set;
		}

		[BalanceProperty("cooldownAfterIapSeconds")]
		public int CooldownAfterIapSeconds
		{
			get;
			private set;
		}

		public InterstitialAdsProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			IngameIntervalSeconds = GetProperty("ingameIntervalSeconds", 240);
			IngameMinutesPlayed = GetProperty("ingameMinutesPlayed", 0);
			SecondsToFirst = GetProperty("secondsToFirst", 240);
			LevelAmount = GetProperty("levelAmount", 5);
			LevelMinutesPlayed = GetProperty("levelMinutesPlayed", 10);
			MaxPerSession = GetProperty("maxPerSession", 100);
			CooldownAfterIapSeconds = GetProperty("cooldownAfterIapSeconds", 120);
		}
	}
}
