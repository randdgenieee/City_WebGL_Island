using SparkLinq;
using System;
using System.Collections.Generic;

namespace CIG
{
	public class DailyQuestsManager
	{
		public delegate void DailyQuestsChangedEventHandler();

		private const int MaxDailyQuests = 3;

		private readonly QuestFactory _questFactory;

		private readonly GameState _gameState;

		private readonly GameStats _gameStats;

		private readonly DailyQuestsManagerProperties _properties;

		private int? _dailyQuestGroupIndex;

		private const string DailyQuestsKey = "DailyQuests";

		private const string DailyQuestGroupIndexKey = "DailyQuestGroupIndex";

		private const string AllDailyQuestsQuestKey = "AllDailyQuestsQuest";

		public List<Quest> DailyQuests
		{
			get;
		}

		public Quest AllDailyQuestsQuest
		{
			get;
			private set;
		}

		private int DailyQuestsCompleted => DailyQuests.Count((Quest q) => q.State == QuestState.Completed);

		public event DailyQuestsChangedEventHandler DailyQuestsChangedEvent;

		private void FireDailyQuestsChangedEvent()
		{
			this.DailyQuestsChangedEvent?.Invoke();
		}

		public DailyQuestsManager(StorageDictionary storage, DailyQuestsManagerProperties properties, QuestFactory questFactory, GameState gameState, GameStats gameStats)
		{
			_properties = properties;
			_questFactory = questFactory;
			_gameState = gameState;
			_gameStats = gameStats;
			_dailyQuestGroupIndex = storage.Get<int?>("DailyQuestGroupIndex", null);
			DailyQuests = new List<Quest>();
			StorageDictionary storageDict = storage.GetStorageDict("DailyQuests");
			foreach (KeyValuePair<string, object> kvp in storageDict.InternalDictionary)
			{
				DailyQuestProperties dailyQuestProperties = _properties.DailyQuestProperties.Find((DailyQuestProperties d) => d.Identifier == kvp.Key);
				if (dailyQuestProperties != null)
				{
					Quest quest = new Quest(storageDict.GetStorageDict(kvp.Key), dailyQuestProperties, _questFactory, gameState, QuestProgressOriginType.Current);
					quest.StateChangedEvent += OnStateChanged;
					DailyQuests.Add(quest);
				}
			}
			_gameStats.DailyQuestsCompleted = DailyQuestsCompleted;
			AllDailyQuestsQuest = CreateAllDailyQuestsQuest(storage.Contains("AllDailyQuestsQuest") ? storage.GetStorageDict("AllDailyQuestsQuest") : new StorageDictionary());
		}

		public void OnDayLapsed()
		{
			GenerateDailyQuests();
		}

		private void GenerateDailyQuests()
		{
			DiscardDailyQuests();
			_dailyQuestGroupIndex = (_dailyQuestGroupIndex.HasValue ? Math.Min(_dailyQuestGroupIndex.Value + 1, _properties.QuestGroupProperties.Count - 1) : 0);
			List<string> list = new List<string>(_properties.QuestGroupProperties[_dailyQuestGroupIndex.Value].Identifiers);
			list.Shuffle();
			int i = 0;
			for (int count = list.Count; i < count && i < 3; i++)
			{
				string questIdentifier = list[i];
				DailyQuestProperties dailyQuestProperties = _properties.DailyQuestProperties.Find((DailyQuestProperties d) => d.Identifier == questIdentifier);
				if (dailyQuestProperties != null)
				{
					Quest quest = new Quest(new StorageDictionary(), dailyQuestProperties, _questFactory, _gameState, QuestProgressOriginType.Current);
					quest.StateChangedEvent += OnStateChanged;
					DailyQuests.Add(quest);
					Analytics.QuestAccepted(quest.Identifier);
				}
			}
			_gameStats.DailyQuestsCompleted = DailyQuestsCompleted;
			AllDailyQuestsQuest = CreateAllDailyQuestsQuest(new StorageDictionary());
			Analytics.QuestAccepted(AllDailyQuestsQuest.Identifier);
			FireDailyQuestsChangedEvent();
		}

		private Quest CreateAllDailyQuestsQuest(StorageDictionary storage)
		{
			return new Quest(storage, _properties.AllDailyQuestsQuestTargetProperties, _questFactory, _gameState, QuestProgressOriginType.Zero);
		}

		private void DiscardDailyQuests()
		{
			int i = 0;
			for (int count = DailyQuests.Count; i < count; i++)
			{
				Quest quest = DailyQuests[i];
				quest.StateChangedEvent -= OnStateChanged;
				quest.Release();
			}
			DailyQuests.Clear();
		}

		private void OnStateChanged(Quest quest, QuestState state)
		{
			_gameStats.DailyQuestsCompleted = DailyQuestsCompleted;
			if (state == QuestState.Achieved)
			{
				_gameStats.QuestCompleted(quest.Identifier, quest.Reward);
			}
		}

		public StorageDictionary Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("AllDailyQuestsQuest", AllDailyQuestsQuest.Serialize());
			StorageDictionary storageDictionary2 = new StorageDictionary();
			int i = 0;
			for (int count = DailyQuests.Count; i < count; i++)
			{
				Quest quest = DailyQuests[i];
				storageDictionary2.Set(quest.Identifier, quest.Serialize());
			}
			storageDictionary.Set("DailyQuests", storageDictionary2);
			storageDictionary.SetOrRemove("DailyQuestGroupIndex", _dailyQuestGroupIndex);
			return storageDictionary;
		}
	}
}
