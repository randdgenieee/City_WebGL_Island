using CIG.Translation;
using System;
using System.Collections.Generic;

namespace CIG
{
	public class FishingEvent : IHasNotification
	{
		public delegate void IsActiveChangedEventHandler(bool isActive);

		public delegate void ActiveLocationsChangedEventHandler(int count);

		public delegate void QuestStartedEventHandler();

		public delegate void QuestAchievedEventHandler();

		public delegate void QuestStoppedEventHandler();

		private const int NotificationMinutesBeforeQuestEnd = 15;

		private readonly StorageDictionary _storage;

		private readonly QuestFactory _questFactory;

		private readonly GameState _gameState;

		private readonly PopupManager _popupManager;

		private readonly RoutineRunner _routineRunner;

		private readonly Timing _timing;

		private readonly FishingEventProperties _properties;

		private FishingMinigame _currentMinigame;

		private int _timesPlayed;

		private const string IsActiveKey = "IsActive";

		private const string FishingQuestKey = "FishingQuest";

		private const string TimesPlayedKey = "TimesPlayed";

		public SpecialQuest FishingQuest
		{
			get;
			private set;
		}

		public bool IsActive
		{
			get;
			private set;
		}

		public int ActiveLocations
		{
			get;
			private set;
		}

		private int LocationsActiveOverTime
		{
			get
			{
				if (FishingQuest != null && FishingQuest.IsActive)
				{
					return Math.Max(0, (int)((FishingQuest.DurationSeconds - FishingQuest.TimeLeft.TotalSeconds) / (double)_properties.LocationsIntervalSeconds));
				}
				return 0;
			}
		}

		private bool CanStartQuest => _gameState.Balance.GetValue("FishingRod") > decimal.Zero;

		public event IsActiveChangedEventHandler IsActiveChangedEvent;

		public event ActiveLocationsChangedEventHandler ActiveLocationsChangedEvent;

		public event QuestStartedEventHandler QuestStartedEvent;

		public event QuestAchievedEventHandler QuestAchievedEvent;

		public event QuestStoppedEventHandler QuestStoppedEvent;

		private void FireIsActiveChangedEvent(bool isActive)
		{
			this.IsActiveChangedEvent?.Invoke(isActive);
		}

		private void FireActiveLocationsChangedEvent(int count)
		{
			this.ActiveLocationsChangedEvent?.Invoke(count);
		}

		private void FireQuestStartedEvent()
		{
			this.QuestStartedEvent?.Invoke();
		}

		private void FireQuestAchievedEvent()
		{
			this.QuestAchievedEvent?.Invoke();
		}

		private void FireQuestStoppedEvent()
		{
			this.QuestStoppedEvent?.Invoke();
		}

		public FishingEvent(StorageDictionary storage, FishingEventProperties properties, QuestFactory questFactory, GameState gameState, LocalNotificationManager localNotificationManager, PopupManager popupManager, RoutineRunner routineRunner, Timing timing)
		{
			_storage = storage;
			_properties = properties;
			_questFactory = questFactory;
			_gameState = gameState;
			_popupManager = popupManager;
			_routineRunner = routineRunner;
			_timing = timing;
			localNotificationManager.HasNotification(this);
			IsActive = _storage.Get("IsActive", defaultValue: false);
			_timesPlayed = _storage.Get("TimesPlayed", 0);
			if (IsActive)
			{
				StartQuest();
			}
			UpdateActiveLocations();
			_gameState.SecondsPlayedChangedEvent += OnSecondsPlayedChanged;
			_gameState.BalanceChangedEvent += OnBalanceChanged;
			TryStopQuest();
			TryStopEvent();
		}

		public void OpenQuestPopup()
		{
			if (FishingQuest != null)
			{
				_popupManager.RequestPopup(new SpecialQuestPopupRequest(FishingQuest, Localization.Key("fishing"), "FISHING", SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.FishingQuestIcon), "fishing"));
			}
		}

		public FishingMinigame StartMinigame()
		{
			if (_currentMinigame == null)
			{
				_currentMinigame = new FishingMinigame(_routineRunner, _timing, _properties.MinigameProperties);
				_currentMinigame.FinishedEvent += OnMinigameFinished;
			}
			return _currentMinigame;
		}

		public void StopMinigame()
		{
			if (_currentMinigame != null)
			{
				_gameState.EarnCurrencies(new Currencies("Fish", _currentMinigame.Score), CurrenciesEarnedReason.Fishing, new FlyingCurrenciesData());
				_currentMinigame.FinishedEvent -= OnMinigameFinished;
				_currentMinigame.StopPlaying();
				_currentMinigame = null;
				UpdateActiveLocations();
				TryStopQuest();
				TryStopEvent();
			}
		}

		private void TryStartEvent()
		{
			if (!IsActive && CanStartQuest)
			{
				StartEvent();
			}
		}

		private void StartEvent()
		{
			StartQuest();
			_timesPlayed = 0;
			UpdateActiveLocations();
			IsActive = true;
			FireIsActiveChangedEvent(isActive: true);
		}

		private void TryStopEvent()
		{
			bool flag = FishingQuest == null || (!FishingQuest.CanCollect && (FishingQuest.IsCompleted || FishingQuest.IsExpired));
			if (((IsActive && _currentMinigame == null) & flag) && !CanStartQuest)
			{
				StopEvent();
			}
		}

		private void StopEvent()
		{
			StopQuest();
			IsActive = false;
			FireIsActiveChangedEvent(isActive: false);
		}

		private void StartQuest()
		{
			if (FishingQuest == null)
			{
				FishingQuest = new SpecialQuest(_storage.GetStorageDict("FishingQuest"), _properties.QuestProperties, _questFactory, _gameState, _routineRunner);
				FishingQuest.StartedEvent += OnQuestStarted;
				FishingQuest.QuestStateChangedEvent += OnQuestStateChanged;
				FishingQuest.ExpiredEvent += OnQuestExpired;
			}
		}

		private void TryStopQuest()
		{
			bool flag = FishingQuest != null && !FishingQuest.CanCollect && FishingQuest.IsExpired;
			if (_currentMinigame == null && flag)
			{
				StopQuest();
			}
		}

		private void StopQuest()
		{
			if (FishingQuest != null)
			{
				FishingQuest.StartedEvent -= OnQuestStarted;
				FishingQuest.QuestStateChangedEvent -= OnQuestStateChanged;
				FishingQuest.ExpiredEvent -= OnQuestExpired;
				FishingQuest = null;
				ActiveLocations = 0;
				_timesPlayed = 0;
				_storage.Remove("FishingQuest");
				FireQuestStoppedEvent();
				if (CanStartQuest)
				{
					StartQuest();
				}
			}
		}

		private void UpdateActiveLocations()
		{
			int num = LocationsActiveOverTime + _properties.StartLocationsActive - _timesPlayed;
			if (ActiveLocations != num)
			{
				ActiveLocations = num;
				FireActiveLocationsChangedEvent(num);
			}
		}

		private void OnMinigameFinished(int score)
		{
			_timesPlayed++;
		}

		private void OnQuestStarted()
		{
			Analytics.FishingQuestStarted();
			FireQuestStartedEvent();
		}

		private void OnQuestExpired()
		{
			TryStopQuest();
			Analytics.FishingQuestExpired();
			TryStopEvent();
		}

		private void OnQuestStateChanged(QuestState state)
		{
			switch (state)
			{
			case QuestState.Achieved:
				FireQuestAchievedEvent();
				break;
			case QuestState.Completed:
				Analytics.FishingRewardCollected(FishingQuest.Quest.Reward, (long)FishingQuest.DurationSeconds);
				StopQuest();
				break;
			}
			TryStopEvent();
		}

		private void OnSecondsPlayedChanged()
		{
			UpdateActiveLocations();
		}

		private void OnBalanceChanged(Currencies oldBalance, Currencies newBalance, FlyingCurrenciesData flyingCurrenciesData)
		{
			TryStartEvent();
		}

		PlannedNotification[] IHasNotification.GetNotifications()
		{
			List<PlannedNotification> list = new List<PlannedNotification>();
			if (IsActive && FishingQuest.IsActive)
			{
				int num = (int)(AntiCheatDateTime.ConvertToSystemDateTime(FishingQuest.EndTime, DateTimeKind.Utc).AddMinutes(-15.0) - DateTime.UtcNow).TotalSeconds;
				if (num > 0)
				{
					list.Add(new PlannedNotification(num, Localization.Format(Localization.Key("fishing_quest_end_notification"), Localization.Integer(15))));
				}
			}
			return list.ToArray();
		}

		public void Serialize()
		{
			_storage.Set("IsActive", IsActive);
			_storage.Set("TimesPlayed", _timesPlayed);
			if (FishingQuest == null)
			{
				_storage.Remove("FishingQuest");
			}
			else
			{
				_storage.Set("FishingQuest", FishingQuest.Serialize());
			}
		}
	}
}
