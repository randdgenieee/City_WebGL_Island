namespace CIG
{
	[BalancePropertyClass("featureFlags", true)]
	public class FeatureFlagProperties : BaseProperties
	{
		private const string LandmarkIAPsEnabledKey = "landmarkIAPsEnabled";

		private const string CashBuildingsForGoldAfterMaxEnabledKey = "cashBuildingsForGoldAfterMaxEnabled";

		[BalanceProperty("landmarkIAPsEnabled")]
		public bool LandmarkIAPsEnabled
		{
			get;
			private set;
		}

		[BalanceProperty("cashBuildingsForGoldAfterMaxEnabled")]
		public bool CashBuildingsForGoldAfterMaxEnabled
		{
			get;
			private set;
		}

		public FeatureFlagProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			LandmarkIAPsEnabled = GetProperty("landmarkIAPsEnabled", defaultValue: false);
			CashBuildingsForGoldAfterMaxEnabled = GetProperty("cashBuildingsForGoldAfterMaxEnabled", defaultValue: true);
		}
	}
}
