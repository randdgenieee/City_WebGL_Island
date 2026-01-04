using System.Collections.Generic;

namespace CIG
{
	[BalancePropertyClass("dailyRewardStreak", false)]
	public class DailyRewardStreakProperties : BaseProperties
	{
		private const string RewardsKey = "rewards";

		[BalanceProperty("rewards")]
		public List<Currency> Rewards
		{
			get;
		}

		public DailyRewardStreakProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			Rewards = GetProperty("rewards", new List<Currency>());
		}
	}
}
