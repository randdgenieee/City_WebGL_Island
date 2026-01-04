namespace CIG
{
	[BalancePropertyClass("craneOfferCurrency", false)]
	public class CraneOfferCurrencyProperties : BaseProperties
	{
		private const string CranesKey = "cranes";

		private const string PriceKey = "price";

		[BalanceProperty("cranes")]
		public int Cranes
		{
			get;
		}

		[BalanceProperty("price")]
		public Currencies Price
		{
			get;
		}

		public CraneOfferCurrencyProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			Cranes = GetProperty("cranes", 0);
			Price = GetProperty("price", new Currencies());
		}
	}
}
