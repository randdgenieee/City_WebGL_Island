namespace CIG
{
	[BalancePropertyClass("currencyConversion", false)]
	public class CurrencyConversionProperties : BaseProperties
	{
		private const string FromCurrencyKey = "fromCurrency";

		private const string ToCurrencyKey = "toCurrency";

		private const string DurationKey = "timeInSeconds";

		[BalanceProperty("fromCurrency")]
		public Currencies FromCurrency
		{
			get;
			private set;
		}

		[BalanceProperty("toCurrency")]
		public Currencies ToCurrency
		{
			get;
			private set;
		}

		[BalanceProperty("timeInSeconds")]
		public float Duration
		{
			get;
			private set;
		}

		public CurrencyConversionProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			FromCurrency = GetProperty("fromCurrency", new Currencies());
			ToCurrency = GetProperty("toCurrency", new Currencies());
			Duration = GetProperty("timeInSeconds", 0f);
		}
	}
}
