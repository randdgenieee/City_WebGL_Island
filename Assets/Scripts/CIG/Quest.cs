using CIG.Translation;
using UnityEngine;

namespace CIG
{
	public class Quest
	{
		public delegate void StateChangedEventHandler(Quest quest, QuestState newState);

		public delegate void ProgressChangedEventHandler(long oldProgess, long newProgress);

		private readonly GameState _gameState;

		private readonly QuestProgressor _progressor;

		private readonly QuestTarget _target;

		private readonly QuestSpriteType _questSpriteType;

		private readonly QuestDescription _questDescription;

		private QuestState _state;

		private bool _isAchieved;

		private const string ProgressorKey = "Progressor";

		private const string TargetKey = "Target";

		private const string IsAchievedKey = "IsAchieved";

		public string Identifier
		{
			get;
		}

		public string QuestType
		{
			get;
		}

		public QuestState State
		{
			get
			{
				return _state;
			}
			set
			{
				if (_state != value)
				{
					_state = value;
					FireStateChangedEvent();
				}
			}
		}

		public ILocalizedString Description => _questDescription.Translate(TargetAmount);

		public QuestSpriteType QuestSpriteType => _questSpriteType;

		public long Progress
		{
			get
			{
				if (_isAchieved)
				{
					return TargetAmount;
				}
				return _progressor.Progress;
			}
		}

		public long TargetAmount => _target.TargetAmount;

		public Currencies Reward => _target.Reward;

		public float ProgressPercent
		{
			get
			{
				if (_isAchieved)
				{
					return 1f;
				}
				return Mathf.Clamp01((float)_progressor.Progress / (float)_target.TargetAmount);
			}
		}

		private bool Achieved
		{
			get
			{
				if (!_isAchieved)
				{
					if (!_target.Completed)
					{
						return _progressor.Progress >= _target.TargetAmount;
					}
					return false;
				}
				return true;
			}
		}

		public event StateChangedEventHandler StateChangedEvent;

		public event ProgressChangedEventHandler ProgressChangedEvent;

		private void FireStateChangedEvent()
		{
			this.StateChangedEvent?.Invoke(this, _state);
		}

		private void FireProgressChangedEvent(long oldProgess, long newProgress)
		{
			this.ProgressChangedEvent?.Invoke(oldProgess, newProgress);
		}

		public Quest(StorageDictionary storage, QuestProperties properties, QuestFactory questFactory, GameState gameState, QuestProgressOriginType progressOriginType)
		{
			_gameState = gameState;
			Identifier = properties.Identifier;
			QuestType = properties.QuestType;
			_isAchieved = storage.Get("IsAchieved", defaultValue: false);
			_target = questFactory.GetQuestTarget(storage.GetStorageDict("Target"), properties);
			_progressor = questFactory.GetQuestProgressor(storage.GetStorageDict("Progressor"), QuestType, progressOriginType, out _questSpriteType, out _questDescription);
			_progressor.ProgressChangedEvent += OnQuestProgressChanged;
			UpdateState();
		}

		public void Collect(FlyingCurrenciesData flyingCurrenciesProperties)
		{
			if (State == QuestState.Achieved)
			{
				Analytics.QuestRewardClaimed(Identifier);
				Currencies currencies = _target.Collect();
				_gameState.EarnCurrencies(currencies, CurrenciesEarnedReason.Quest, flyingCurrenciesProperties);
				_isAchieved = false;
				State = QuestState.InProgress;
				UpdateState();
			}
		}

		public void Release()
		{
			_progressor.ProgressChangedEvent -= OnQuestProgressChanged;
			_progressor.Release();
		}

		private void UpdateState()
		{
			if (_target.Completed)
			{
				State = QuestState.Completed;
			}
			else if (Achieved)
			{
				State = QuestState.Achieved;
			}
			else
			{
				State = QuestState.InProgress;
			}
		}

		private void OnQuestProgressChanged(long oldProgress, long newProgress)
		{
			_isAchieved |= Achieved;
			UpdateState();
			FireProgressChangedEvent(oldProgress, newProgress);
		}

		public virtual StorageDictionary Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("Progressor", _progressor.Serialize());
			storageDictionary.Set("Target", _target.Serialize());
			storageDictionary.Set("IsAchieved", _isAchieved);
			return storageDictionary;
		}
	}
}
