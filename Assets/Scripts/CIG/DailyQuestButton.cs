namespace CIG
{
	public class DailyQuestButton : QuestButton
	{
		private PopupManager _popupManager;

		public void Initialize(Quest quest, PopupManager popupManager)
		{
			Initialize(quest);
			_popupManager = popupManager;
		}

		public override void OnClicked()
		{
			_popupManager.RequestPopup(new QuestPopupRequest(openDailyQuests: true));
		}
	}
}
