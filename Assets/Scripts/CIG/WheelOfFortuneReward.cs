namespace CIG
{
	public class WheelOfFortuneReward : IStorable
	{
		private const string IndexKey = "Index";

		private const string RewardKey = "Reward";

		public int Index
		{
			get;
			private set;
		}

		public Currencies Reward
		{
			get;
			private set;
		}

		public WheelOfFortuneReward(int index, Currencies reward)
		{
			Index = index;
			Reward = reward;
		}

		public WheelOfFortuneReward(StorageDictionary storage)
		{
			Index = storage.Get("Index", 0);
			Reward = storage.GetModel("Reward", (StorageDictionary sd) => new Currencies(sd), new Currencies());
		}

		StorageDictionary IStorable.Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("Index", Index);
			storageDictionary.Set("Reward", Reward);
			return storageDictionary;
		}
	}
}
