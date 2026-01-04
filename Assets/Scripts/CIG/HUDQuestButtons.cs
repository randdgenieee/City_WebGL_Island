using CIG.Translation;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class HUDQuestButtons : HUDRegionElement
	{
		private const int MaxDailyQuestButtons = 3;

		[SerializeField]
		private GameObject _rewardBadge;

		[SerializeField]
		private LocalizedText _rewardBadgeLabel;

		[SerializeField]
		private DailyQuestButton _dailyQuestButtonPrefab;

		[SerializeField]
		private RectTransform _dailyQuestButtonParent;

		[SerializeField]
		private FishingEventButton _fishingEventButton;

		private PopupManager _popupManager;

		private DailyQuestsManager _dailyQuestsManager;

		private OngoingQuestsManager _ongoingQuestsManager;

		private readonly List<DailyQuestButton> _dailyQuestButtons = new List<DailyQuestButton>();

		public void Initialize(PopupManager popupManager, QuestsManager questsManager, FishingEvent fishingEvent)
		{
			_popupManager = popupManager;
			_dailyQuestsManager = questsManager.DailyQuestsManager;
			_ongoingQuestsManager = questsManager.OngoingQuestsManager;
			int i = 0;
			for (int num = 3; i < num; i++)
			{
				DailyQuestButton item = Object.Instantiate(_dailyQuestButtonPrefab, _dailyQuestButtonParent);
				_dailyQuestButtons.Add(item);
			}
			_fishingEventButton.Initialize(fishingEvent);
			_ongoingQuestsManager.AchievedQuestCountChangedEvent += OnAchievedQuestCountChanged;
			OnAchievedQuestCountChanged(_ongoingQuestsManager.OngoingQuestsAchievedCount);
			_dailyQuestsManager.DailyQuestsChangedEvent += OnDailyQuestsChanged;
			OnDailyQuestsChanged();
		}

		private void OnDestroy()
		{
			if (_dailyQuestsManager != null)
			{
				_dailyQuestsManager.DailyQuestsChangedEvent -= OnDailyQuestsChanged;
				_dailyQuestsManager = null;
			}
			if (_ongoingQuestsManager != null)
			{
				_ongoingQuestsManager.AchievedQuestCountChangedEvent -= OnAchievedQuestCountChanged;
				_ongoingQuestsManager = null;
			}
		}

		public void OnOngoingQuestsClicked()
		{
			_popupManager.RequestPopup(new QuestPopupRequest(openDailyQuests: false));
		}

		private void OnDailyQuestsChanged()
		{
			int i = 0;
			for (int count = _dailyQuestButtons.Count; i < count; i++)
			{
				UnityEngine.Object.Destroy(_dailyQuestButtons[i].gameObject);
			}
			_dailyQuestButtons.Clear();
			int j = 0;
			for (int count2 = _dailyQuestsManager.DailyQuests.Count; j < count2 && j < 3; j++)
			{
				Quest quest = _dailyQuestsManager.DailyQuests[j];
				DailyQuestButton dailyQuestButton = Object.Instantiate(_dailyQuestButtonPrefab, _dailyQuestButtonParent);
				dailyQuestButton.Initialize(quest, _popupManager);
				_dailyQuestButtons.Add(dailyQuestButton);
			}
		}

		private void OnAchievedQuestCountChanged(int count)
		{
			_rewardBadge.SetActive(count > 0);
			_rewardBadgeLabel.LocalizedString = Localization.Integer(count);
		}
	}
}
