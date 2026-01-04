namespace CIG
{
	public abstract class QuestTarget
	{
		public abstract long TargetAmount
		{
			get;
		}

		public abstract Currencies Reward
		{
			get;
		}

		public abstract bool Completed
		{
			get;
		}

		public abstract Currencies Collect();

		public virtual StorageDictionary Serialize()
		{
			return new StorageDictionary();
		}
	}
}
