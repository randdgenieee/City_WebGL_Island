namespace CIG
{
	public class QuestPopupRequest : PopupRequest
	{
		public bool OpenDailyQuests
		{
			get;
			private set;
		}

		public QuestPopupRequest(bool openDailyQuests)
			: base(typeof(QuestPopup))
		{
			OpenDailyQuests = openDailyQuests;
		}
	}
}
