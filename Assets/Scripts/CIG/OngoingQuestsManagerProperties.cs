using System.Collections.Generic;

namespace CIG
{
	public class OngoingQuestsManagerProperties
	{
		public List<OngoingQuestProperties> OngoingQuestProperties
		{
			get;
			private set;
		}

		public OngoingQuestsManagerProperties(List<OngoingQuestProperties> ongoingQuestProperties)
		{
			OngoingQuestProperties = ongoingQuestProperties;
		}
	}
}
