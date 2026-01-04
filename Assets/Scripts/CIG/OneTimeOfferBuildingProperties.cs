namespace CIG
{
	[BalancePropertyClass("oneTimeOfferBuilding", true)]
	public class OneTimeOfferBuildingProperties : OneTimeOfferBaseProperties
	{
		private const string InitialDiscountKey = "initialDiscount";

		private const string DiscountIncrementKey = "discountIncrement";

		private const string DiscountDecrementKey = "discountDecrement";

		private const string MinimumDiscountKey = "minimumDiscount";

		private const string MaximumDiscountKey = "maximumDiscount";

		private const string ExpirationSecondsKey = "expirationSeconds";

		[BalanceProperty("initialDiscount")]
		public float InitialDiscount
		{
			get;
			private set;
		}

		[BalanceProperty("discountIncrement")]
		public float DiscountIncrement
		{
			get;
			private set;
		}

		[BalanceProperty("discountDecrement")]
		public float DiscountDecrement
		{
			get;
			private set;
		}

		[BalanceProperty("minimumDiscount")]
		public float MinimumDiscount
		{
			get;
			private set;
		}

		[BalanceProperty("maximumDiscount")]
		public float MaximumDiscount
		{
			get;
			private set;
		}

		[BalanceProperty("expirationSeconds")]
		public int ExpirationSeconds
		{
			get;
			private set;
		}

		public OneTimeOfferBuildingProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			InitialDiscount = GetProperty("initialDiscount", 0.35f);
			DiscountIncrement = GetProperty("discountIncrement", 0.05f);
			DiscountDecrement = GetProperty("discountDecrement", 0.05f);
			MinimumDiscount = GetProperty("minimumDiscount", 0.25f);
			MaximumDiscount = GetProperty("maximumDiscount", 0.75f);
			ExpirationSeconds = GetProperty("expirationSeconds", 259200);
		}
	}
}
