using System.Collections.Generic;

namespace CIG
{
	[BalancePropertyClass("wheelOfFortuneStreak", false)]
	public class WheelOfFortuneStreakProperties : BaseProperties
	{
		private const string RewardsKey = "rewards";

		[BalanceProperty("rewards")]
		public List<Currencies> Rewards
		{
			get;
			private set;
		}

		public WheelOfFortuneStreakProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			Rewards = GetProperty("rewards", new List<Currencies>());
		}
	}
}
