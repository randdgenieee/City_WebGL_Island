namespace CIG
{
	public class QuestsManagerProperties
	{
		public OngoingQuestsManagerProperties OngoingQuestsManagerProperties
		{
			get;
			private set;
		}

		public DailyQuestsManagerProperties DailyQuestsManagerProperties
		{
			get;
			private set;
		}

		public QuestsManagerProperties(OngoingQuestsManagerProperties ongoingQuestsManagerProperties, DailyQuestsManagerProperties dailyQuestsManagerProperties)
		{
			OngoingQuestsManagerProperties = ongoingQuestsManagerProperties;
			DailyQuestsManagerProperties = dailyQuestsManagerProperties;
		}
	}
}
