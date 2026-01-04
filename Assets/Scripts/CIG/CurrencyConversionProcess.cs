namespace CIG
{
	public class CurrencyConversionProcess : UpspeedableProcess
	{
		private const string FromCurrenciesKey = "FromCurrencies";

		private const string ToCurrenciesKey = "ToCurrencies";

		public Currencies FromCurrencies
		{
			get;
		}

		public Currencies ToCurrencies
		{
			get;
		}

		public CurrencyConversionProcess(Timing timing, Multipliers multipliers, GameState gameState, Currencies fromCurrencies, Currencies toCurrencies, double duration)
			: base(timing, multipliers, gameState, duration, CurrenciesSpentReason.Unknown)
		{
			FromCurrencies = fromCurrencies;
			ToCurrencies = toCurrencies;
		}

		public CurrencyConversionProcess(StorageDictionary storage, Timing timing, Multipliers multipliers, GameState gameState)
			: base(storage, timing, multipliers, gameState)
		{
			FromCurrencies = storage.GetModel("FromCurrencies", (StorageDictionary sd) => new Currencies(sd), new Currencies());
			ToCurrencies = storage.GetModel("ToCurrencies", (StorageDictionary sd) => new Currencies(sd), new Currencies());
		}

		public override StorageDictionary Serialize()
		{
			StorageDictionary storageDictionary = base.Serialize();
			storageDictionary.Set("FromCurrencies", FromCurrencies);
			storageDictionary.Set("ToCurrencies", ToCurrencies);
			return storageDictionary;
		}
	}
}
