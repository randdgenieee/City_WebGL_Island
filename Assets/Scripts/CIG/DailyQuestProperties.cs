using System.Collections.Generic;

namespace CIG
{
	[BalancePropertyClass("dailyQuest", false)]
	[BalanceEquallySizedProperties(new string[]
	{
		"randomTargets",
		"randomRewards"
	})]
	public class DailyQuestProperties : QuestProperties
	{
		private const string WeightKey = "weight";

		private const string RandomTargetsKey = "randomTargets";

		private const string RandomRewardsKey = "randomRewards";

		[BalanceProperty("weight", RequiredKey = false)]
		public int Weight
		{
			get;
			private set;
		}

		[BalanceProperty("randomTargets")]
		public List<int> RandomTargets
		{
			get;
			private set;
		}

		[BalanceProperty("randomRewards")]
		public List<Currencies> RandomRewards
		{
			get;
			private set;
		}

		public DailyQuestProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			Weight = GetProperty("weight", 0, optional: true);
			RandomTargets = GetProperty("randomTargets", new List<int>());
			RandomRewards = GetProperty("randomRewards", new List<Currencies>());
		}
	}
}
