using System;
using System.Collections;

namespace CIG
{
	public class SpecialQuest
	{
		public delegate void StartedEventHandler();

		public delegate void QuestStateChangedEventHandler(QuestState state);

		public delegate void ExpiredEventHandler();

		private readonly RoutineRunner _routineRunner;

		private readonly GameState _gameState;

		private readonly SpecialQuestProperties _properties;

		private DateTime _startTime;

		private const string QuestKey = "Quest";

		private const string IsActiveKey = "IsActive";

		private const string IsExpiredKey = "IsExpired";

		private const string StartTimeKey = "StartTime";

		private const string EndTimeKey = "EndTime";

		public Quest Quest
		{
			get;
		}

		public Currencies StartCost => _properties.StartCost;

		public double DurationSeconds => _properties.DurationSeconds;

		public bool IsActive
		{
			get;
			private set;
		}

		public bool IsExpired
		{
			get;
			private set;
		}

		public bool IsCompleted => Quest.State == QuestState.Completed;

		public DateTime EndTime
		{
			get;
			private set;
		}

		public TimeSpan TimeLeft => EndTime - AntiCheatDateTime.UtcNow;

		public bool CanCollect => Quest.State == QuestState.Achieved;

		public event StartedEventHandler StartedEvent;

		public event QuestStateChangedEventHandler QuestStateChangedEvent;

		public event ExpiredEventHandler ExpiredEvent;

		private void FireStartedEvent()
		{
			this.StartedEvent?.Invoke();
		}

		private void FireQuestStateChangedEvent(QuestState state)
		{
			this.QuestStateChangedEvent?.Invoke(state);
		}

		private void FireExpiredEvent()
		{
			this.ExpiredEvent?.Invoke();
		}

		public SpecialQuest(StorageDictionary storage, SpecialQuestProperties properties, QuestFactory questFactory, GameState gameState, RoutineRunner routineRunner)
		{
			_gameState = gameState;
			_routineRunner = routineRunner;
			_properties = properties;
			Quest = new Quest(storage.GetStorageDict("Quest"), properties, questFactory, gameState, QuestProgressOriginType.Current);
			IsActive = storage.Get("IsActive", defaultValue: false);
			IsExpired = storage.Get("IsExpired", defaultValue: false);
			_startTime = storage.GetDateTime("StartTime", DateTime.MinValue);
			EndTime = storage.GetDateTime("EndTime", DateTime.MinValue);
			Quest.StateChangedEvent += OnQuestStateChanged;
			if (IsActive && !IsExpired && !IsCompleted)
			{
				_routineRunner.StartCoroutine(LapseRoutine());
			}
		}

		public bool StartQuest()
		{
			if (!IsActive)
			{
				_gameState.SpendCurrencies(StartCost, CurrenciesSpentReason.SpecialQuest, delegate(bool success, Currencies spend)
				{
					if (success)
					{
						IsActive = true;
						_startTime = AntiCheatDateTime.UtcNow;
						EndTime = _startTime.AddSeconds(DurationSeconds);
						_routineRunner.StartCoroutine(LapseRoutine());
						FireStartedEvent();
					}
				});
			}
			return IsActive;
		}

		private void OnQuestStateChanged(Quest quest, QuestState newState)
		{
			FireQuestStateChangedEvent(newState);
		}

		private IEnumerator LapseRoutine()
		{
			yield return new WaitUntilUTCDateTime(EndTime);
			IsExpired = true;
			FireExpiredEvent();
		}

		public StorageDictionary Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("Quest", Quest.Serialize());
			storageDictionary.Set("IsActive", IsActive);
			storageDictionary.Set("IsExpired", IsExpired);
			storageDictionary.Set("StartTime", _startTime);
			storageDictionary.Set("EndTime", EndTime);
			return storageDictionary;
		}
	}
}
