using System.Collections.Generic;

namespace CIG
{
	[BalancePropertyClass("dailyRewards", true)]
	public class DailyRewardsProperties : BaseProperties
	{
		private const string UnlockLevelKey = "minimumLevel";

		private const string MinutesUntilDailyRewardIsShownWithoutAVideoKey = "minutesUntilDailyRewardIsShownWithoutAVideo";

		public List<DailyRewardStreakProperties> StreakProperties
		{
			get;
			private set;
		}

		[BalanceProperty("minimumLevel")]
		public int UnlockLevel
		{
			get;
			private set;
		}

		[BalanceProperty("minutesUntilDailyRewardIsShownWithoutAVideo")]
		public int MinutesUntilDailyRewardIsShownWithoutAVideo
		{
			get;
			private set;
		}

		public DailyRewardsProperties(PropertiesDictionary propsDict, string baseKey, List<DailyRewardStreakProperties> streakProperties)
			: base(propsDict, baseKey)
		{
			StreakProperties = streakProperties;
			UnlockLevel = GetProperty("minimumLevel", 5);
			MinutesUntilDailyRewardIsShownWithoutAVideo = GetProperty("minutesUntilDailyRewardIsShownWithoutAVideo", 2);
		}
	}
}
