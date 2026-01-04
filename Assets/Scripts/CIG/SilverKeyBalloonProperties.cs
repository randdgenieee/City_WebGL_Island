namespace CIG
{
	[BalancePropertyClass("silverKeyBalloon", true)]
	public class SilverKeyBalloonProperties : WalkerBalloonProperties
	{
		private const string MinSilverKeyAmountKey = "minSilverKeys";

		private const string MaxSilverKeyAmountKey = "maxSilverKeys";

		[BalanceProperty("minSilverKeys")]
		public int MinSilverKeyAmount
		{
			get;
			private set;
		}

		[BalanceProperty("maxSilverKeys")]
		public int MaxSilverKeyAmount
		{
			get;
			private set;
		}

		public SilverKeyBalloonProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			MinSilverKeyAmount = GetProperty("minSilverKeys", 1);
			MaxSilverKeyAmount = GetProperty("maxSilverKeys", 1);
		}
	}
}
