namespace CIG
{
	[BalancePropertyClass("keyDeals", true)]
	public class KeyDealsProperties : BaseProperties
	{
		private const string RefreshTimeSecondsKey = "refreshTimeSeconds";

		private const string MaxLevelsHigherCashBuildingKey = "maxLevelsHigherCashBuilding";

		private const string MaxLevelsHigherGoldBuildingKey = "maxLevelsHigherGoldBuilding";

		private const string AmountKey = "amount";

		[BalanceProperty("refreshTimeSeconds")]
		public int RefreshTimeSeconds
		{
			get;
			private set;
		}

		[BalanceProperty("maxLevelsHigherCashBuilding")]
		public int MaxLevelsHigherCashBuilding
		{
			get;
			private set;
		}

		[BalanceProperty("maxLevelsHigherGoldBuilding")]
		public int MaxLevelsHigherGoldBuilding
		{
			get;
			private set;
		}

		[BalanceProperty("amount")]
		public int Amount
		{
			get;
			private set;
		}

		public KeyDealsProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			RefreshTimeSeconds = GetProperty("refreshTimeSeconds", 7200);
			MaxLevelsHigherCashBuilding = GetProperty("maxLevelsHigherCashBuilding", 40);
			MaxLevelsHigherGoldBuilding = GetProperty("maxLevelsHigherGoldBuilding", 20);
			Amount = GetProperty("amount", 4);
		}
	}
}
