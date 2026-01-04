namespace CIG
{
	public abstract class ProfitCollectBehaviour : IStorable
	{
		public enum BehaviourType
		{
			Unknown = -1,
			Full,
			MultiTap
		}

		private const string TotalProfitKey = "TotalProfit";

		private const string CurrencyEarnedReasonKey = "CurrencyEarnedReason";

		public abstract bool CanCollect
		{
			get;
		}

		public abstract BehaviourType ProfitCollectBehaviourType
		{
			get;
		}

		public Currencies TotalProfit
		{
			get;
		}

		public CurrenciesEarnedReason CurrenciesEarnedReason
		{
			get;
		}

		protected ProfitCollectBehaviour(Currencies totalProfit, CurrenciesEarnedReason currenciesEarnedReason)
		{
			TotalProfit = totalProfit;
			CurrenciesEarnedReason = currenciesEarnedReason;
		}

		public abstract Currencies Collect();

		protected ProfitCollectBehaviour(StorageDictionary storage)
		{
			TotalProfit = storage.GetModel("TotalProfit", (StorageDictionary sd) => new Currencies(sd), new Currencies());
			CurrenciesEarnedReason = (CurrenciesEarnedReason)storage.Get("CurrencyEarnedReason", 0);
		}

		public virtual StorageDictionary Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("TotalProfit", TotalProfit);
			storageDictionary.Set("CurrencyEarnedReason", (int)CurrenciesEarnedReason);
			return storageDictionary;
		}
	}
}
