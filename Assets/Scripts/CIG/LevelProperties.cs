namespace CIG
{
	[BalancePropertyClass("level", false)]
	public class LevelProperties : BaseProperties
	{
		private const string XpNeededKey = "xpNeeded";

		private const string RewardKey = "reward";

		[BalanceProperty("xpNeeded")]
		public int XpNeeded
		{
			get;
			private set;
		}

		[BalanceProperty("reward")]
		public Currencies Reward
		{
			get;
			private set;
		}

		public LevelProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			XpNeeded = GetProperty("xpNeeded", 0);
			Reward = GetProperty("reward", new Currencies());
		}
	}
}
