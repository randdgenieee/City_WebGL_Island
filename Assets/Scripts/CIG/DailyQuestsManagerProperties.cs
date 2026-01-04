using System.Collections.Generic;

namespace CIG
{
	public class DailyQuestsManagerProperties
	{
		public DailyQuestProperties AllDailyQuestsQuestTargetProperties
		{
			get;
			private set;
		}

		public List<DailyQuestProperties> DailyQuestProperties
		{
			get;
			private set;
		}

		public List<QuestGroupProperties> QuestGroupProperties
		{
			get;
			private set;
		}

		public DailyQuestsManagerProperties(DailyQuestProperties allDailyQuestsQuestTargetProperties, List<DailyQuestProperties> dailyQuestProperties, List<QuestGroupProperties> questGroupProperties)
		{
			AllDailyQuestsQuestTargetProperties = allDailyQuestsQuestTargetProperties;
			DailyQuestProperties = dailyQuestProperties;
			QuestGroupProperties = questGroupProperties;
		}
	}
}
