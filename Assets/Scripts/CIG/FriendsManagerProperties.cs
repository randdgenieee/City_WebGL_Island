namespace CIG
{
	[BalancePropertyClass("friends", true)]
	public class FriendsManagerProperties : BaseProperties
	{
		private const string GiftCurrenciesKey = "giftCurrencies";

		[BalanceProperty("giftCurrencies")]
		public Currencies GiftCurrencies
		{
			get;
		}

		public FriendsManagerProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			GiftCurrencies = GetProperty("giftCurrencies", new Currencies());
		}
	}
}
