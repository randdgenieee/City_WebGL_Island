namespace CIG
{
	public class MultiTapProfitCollectBehaviour : ProfitCollectBehaviour
	{
		public delegate void CollectedEventHandler();

		private const string RemainingProfitKey = "RemainingProfit";

		public override bool CanCollect => !RemainingProfit.IsEmpty();

		public override BehaviourType ProfitCollectBehaviourType => BehaviourType.MultiTap;

		public Currencies RemainingProfit
		{
			get;
			private set;
		}

		public event CollectedEventHandler CollectedEvent;

		private void FireCollectedEvent()
		{
			this.CollectedEvent?.Invoke();
		}

		public MultiTapProfitCollectBehaviour(Currencies totalProfit, CurrenciesEarnedReason currenciesEarnedReason)
			: base(totalProfit, currenciesEarnedReason)
		{
			RemainingProfit = totalProfit;
		}

		public override Currencies Collect()
		{
			int i = 0;
			for (int keyCount = RemainingProfit.KeyCount; i < keyCount; i++)
			{
				Currency currency = RemainingProfit.GetCurrency(i);
				if (currency.Value > decimal.Zero)
				{
					Currencies currencies = new Currencies(currency.Name, decimal.One);
					RemainingProfit = (RemainingProfit - currencies).WithoutEmpty();
					FireCollectedEvent();
					return currencies;
				}
			}
			return new Currencies();
		}

		public MultiTapProfitCollectBehaviour(StorageDictionary storage)
			: base(storage)
		{
			RemainingProfit = storage.GetModel("RemainingProfit", (StorageDictionary sd) => new Currencies(sd), new Currencies());
		}

		public override StorageDictionary Serialize()
		{
			StorageDictionary storageDictionary = base.Serialize();
			storageDictionary.Set("RemainingProfit", RemainingProfit);
			return storageDictionary;
		}
	}
}
