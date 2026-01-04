namespace CIG
{
	public class FullProfitCollectBehaviour : ProfitCollectBehaviour
	{
		private int _collectTimesRemaining;

		private bool _firstCollectGiven;

		private const string CollectTimesRemainingKey = "CollectTimeRemaining";

		private const string FirstCollectGivenKey = "FirstCollectGiven";

		private const string MaxedKey = "Maxed";

		public override bool CanCollect => _collectTimesRemaining > 0;

		public override BehaviourType ProfitCollectBehaviourType => BehaviourType.Full;

		public bool Maxed
		{
			get;
		}

		public FullProfitCollectBehaviour(Currencies totalProfit, CurrenciesEarnedReason currenciesEarnedReason, bool maxed, int extraCollectTimes = 0)
			: base(totalProfit, currenciesEarnedReason)
		{
			Maxed = maxed;
			_collectTimesRemaining = 1 + extraCollectTimes;
		}

		public override Currencies Collect()
		{
			_collectTimesRemaining--;
			if (_firstCollectGiven)
			{
				return base.TotalProfit.Filter("XP");
			}
			_firstCollectGiven = true;
			return base.TotalProfit;
		}

		public FullProfitCollectBehaviour(StorageDictionary storage)
			: base(storage)
		{
			_collectTimesRemaining = storage.Get("CollectTimeRemaining", 0);
			_firstCollectGiven = storage.Get("FirstCollectGiven", defaultValue: false);
			Maxed = storage.Get("Maxed", defaultValue: false);
		}

		public override StorageDictionary Serialize()
		{
			StorageDictionary storageDictionary = base.Serialize();
			storageDictionary.Set("CollectTimeRemaining", _collectTimesRemaining);
			storageDictionary.Set("FirstCollectGiven", _firstCollectGiven);
			storageDictionary.Set("Maxed", Maxed);
			return storageDictionary;
		}
	}
}
