using System.Collections.Generic;

namespace CIG
{
	public class OngoingQuestsManager
	{
		public delegate void AchievedQuestCountChangedEventHandler(int count);

		private readonly GameStats _gameStats;

		private const string OngoingQuestsKey = "OngoingQuests";

		public List<Quest> OngoingQuests
		{
			get;
		}

		public int OngoingQuestsAchievedCount => OngoingQuests.FindAll((Quest q) => q.State == QuestState.Achieved).Count;

		public event AchievedQuestCountChangedEventHandler AchievedQuestCountChangedEvent;

		private void FireAchievedQuestCountChangedEvent()
		{
			this.AchievedQuestCountChangedEvent?.Invoke(OngoingQuestsAchievedCount);
		}

		public OngoingQuestsManager(StorageDictionary storage, OngoingQuestsManagerProperties properties, QuestFactory questFactory, GameState gameState, GameStats gameStats)
		{
			_gameStats = gameStats;
			StorageDictionary storageDict = storage.GetStorageDict("OngoingQuests");
			OngoingQuests = new List<Quest>();
			int i = 0;
			for (int count = properties.OngoingQuestProperties.Count; i < count; i++)
			{
				OngoingQuestProperties ongoingQuestProperties = properties.OngoingQuestProperties[i];
				Quest quest = new Quest(storageDict.GetStorageDict(ongoingQuestProperties.Identifier), ongoingQuestProperties, questFactory, gameState, QuestProgressOriginType.Zero);
				quest.StateChangedEvent += OnStateChanged;
				OngoingQuests.Add(quest);
			}
		}

		private void OnStateChanged(Quest quest, QuestState state)
		{
			if (state == QuestState.Achieved)
			{
				_gameStats.QuestCompleted(quest.Identifier, quest.Reward);
			}
			FireAchievedQuestCountChangedEvent();
		}

		public StorageDictionary Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			StorageDictionary storageDictionary2 = new StorageDictionary();
			int i = 0;
			for (int count = OngoingQuests.Count; i < count; i++)
			{
				Quest quest = OngoingQuests[i];
				storageDictionary2.Set(quest.Identifier, quest.Serialize());
			}
			storageDictionary.Set("OngoingQuests", storageDictionary2);
			return storageDictionary;
		}
	}
}
