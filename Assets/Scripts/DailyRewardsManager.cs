using CIG;
using CIG.Translation;
using System;
using System.Collections.Generic;

public class DailyRewardsManager : DailyManager, IHasNotification
{
	public const int RewardMultiplier = 2;

	private readonly GameState _gameState;

	private readonly GameStats _gameStats;

	private readonly PopupManager _popupManager;

	private readonly TutorialManager _tutorialManager;

	private readonly DailyRewardsProperties _properties;

	private readonly VideoAds3Manager _videoAds3Manager;

	private List<Currency> _streak;

	private readonly List<Currency> _storedRewards;

	private DateTime _showDailyRewardWithoutVideoTimeTreshhold;

	private int _streakIndex;

	private int _dayIndex;

	private bool _hasCollected;

	private bool _hasStoredLastReward;

	private bool _hasShownPopup;

	private bool _triedToOpenDailyReward;

	private const string StreakIndexKey = "streakIndex";

	private const string DayIndexKey = "dayIndex";

	private const string HasCollectedKey = "hasCollected";

	private const string StoredRewardsKey = "storedRewards";

	private const string ShowDailyRewardWithoutVideoTimeTreshholdKey = "showDailyRewardWithoutVideoTimeTreshhold";

	private const string TriedToOpenDailyRewardKey = "triedToOpenDailyReward";

	private const string RewardIsDoubledKey = "rewardIsDoubled";

	private const string HasStoredLastRewardKey = "hasStoredLastReward";

	private bool IsEnabled
	{
		get
		{
			if (_gameState.Level >= _properties.UnlockLevel)
			{
				return _tutorialManager.InitialTutorialFinished;
			}
			return false;
		}
	}

	private bool HasStarted => _streak != null;

	public bool RewardIsDoubled
	{
		get;
		private set;
	}

	public bool IsBusy
	{
		get;
		private set;
	}

	public bool CanDoubleReward => _videoAds3Manager.IsReady;

	private bool CanOpenPopup
	{
		get
		{
			if (IsEnabled && HasStarted && !_hasShownPopup && !_hasCollected)
			{
				return _storedRewards.Count > 0;
			}
			return false;
		}
	}

	public DailyRewardsManager(StorageDictionary storage, GameState gameState, LocalNotificationManager localNotificationManager, PopupManager popupManager, TutorialManager tutorialManager, GameStats gameStats, DailyRewardsProperties properties, VideoAds3Manager videoAds3Manager)
		: base(storage)
	{
		_gameState = gameState;
		_popupManager = popupManager;
		_tutorialManager = tutorialManager;
		_gameStats = gameStats;
		_properties = properties;
		_videoAds3Manager = videoAds3Manager;
		_streakIndex = _storage.Get("streakIndex", 0);
		_dayIndex = _storage.Get("dayIndex", 0);
		_hasCollected = _storage.Get("hasCollected", defaultValue: false);
		_showDailyRewardWithoutVideoTimeTreshhold = _storage.GetDateTime("showDailyRewardWithoutVideoTimeTreshhold", DateTime.MinValue);
		_triedToOpenDailyReward = _storage.Get("triedToOpenDailyReward", defaultValue: false);
		_storedRewards = _storage.GetModels("storedRewards", (StorageDictionary sd) => new Currency(sd));
		RewardIsDoubled = _storage.Get("rewardIsDoubled", defaultValue: false);
		_hasStoredLastReward = _storage.Get("hasStoredLastReward", defaultValue: false);
		localNotificationManager.HasNotification(this);
		if (_gameState.Level < _properties.UnlockLevel)
		{
			_gameState.VisuallyLevelledUpEvent += OnVisuallyLevelledUp;
		}
		if (!_tutorialManager.InitialTutorialFinished)
		{
			_tutorialManager.TutorialFinishedEvent += OnTutorialFinished;
		}
		_streak = GetStreak(_streakIndex);
		if (IsEnabled)
		{
			TryLapseDay();
		}
		TryOpenPopup();
		_videoAds3Manager.AvailabilityChangedEvent += OnAvailabilityChanged;
	}

	public void Collect()
	{
		if (HasStarted && !_hasCollected)
		{
			_hasCollected = true;
			RewardIsDoubled = false;
			if (_dayIndex == _streak.Count - 1)
			{
				_gameStats.AddNumberOfDailyBonusStreaksCompleted(1);
			}
			CollectRewards();
		}
	}

	public void WatchVideo(Action<Currency> videoWatchedCallback, Action videoCanceledCallback)
	{
		if (!IsBusy)
		{
			IsBusy = true;
			_videoAds3Manager.ShowAd(delegate(bool success, bool clicked)
			{
				if (success)
				{
					videoWatchedCallback?.Invoke(DoubleReward());
				}
				else
				{
					videoCanceledCallback?.Invoke();
				}
				IsBusy = false;
			}, VideoSource.DoubleDailyReward);
		}
	}

	protected override void OnDayLapsed(TimeSpan timeSinceLapse)
	{
		if (IsEnabled)
		{
			IncrementStreak(timeSinceLapse.TotalDays);
			StoreTodaysReward();
			TryOpenPopup();
		}
	}

	private Currency DoubleReward()
	{
		int index = _storedRewards.Count - 1;
		Currency currency = _storedRewards[index];
		_storedRewards.RemoveAt(index);
		_storedRewards.Add(new Currency(currency.Name, currency.Value * 2m));
		RewardIsDoubled = true;
		return _storedRewards[index];
	}

	private void StoreTodaysReward()
	{
		_storedRewards.Add(_streak[_dayIndex]);
		_hasStoredLastReward = true;
	}

	private void CollectRewards()
	{
		int num = _storedRewards.Count - 1;
		for (int num2 = num; num2 >= 0; num2--)
		{
			int amountOfFlyingCurrencies = (num2 == num) ? 1 : 0;
			_gameState.EarnCurrencies(new Currencies(_storedRewards[num2]), CurrenciesEarnedReason.DailyReward, new FlyingCurrenciesData(null, amountOfFlyingCurrencies));
		}
		_storedRewards.Clear();
	}

	private void OnAvailabilityChanged()
	{
		TryOpenPopup();
	}

	private void OnSecondsPlayedChanged()
	{
		TryOpenPopup();
	}

	private void TryOpenPopup()
	{
		if (!CanOpenPopup)
		{
			return;
		}
		if (CanDoubleReward)
		{
			OpenPopup();
		}
		else if (!_triedToOpenDailyReward && !RewardIsDoubled)
		{
			SetTimeToShowDailyRewardWithoutVideo();
		}
		else if (_showDailyRewardWithoutVideoTimeTreshhold <= AntiCheatDateTime.UtcNow)
		{
			if (!RewardIsDoubled)
			{
				DoubleReward();
			}
			OpenPopup();
		}
	}

	private void SetTimeToShowDailyRewardWithoutVideo()
	{
		_triedToOpenDailyReward = true;
		_gameState.SecondsPlayedChangedEvent += OnSecondsPlayedChanged;
		_showDailyRewardWithoutVideoTimeTreshhold = AntiCheatDateTime.UtcNow.AddMinutes(_properties.MinutesUntilDailyRewardIsShownWithoutAVideo);
	}

	private void OpenPopup()
	{
		_showDailyRewardWithoutVideoTimeTreshhold = DateTime.MinValue;
		_hasShownPopup = true;
		_videoAds3Manager.AvailabilityChangedEvent -= OnAvailabilityChanged;
		if (_triedToOpenDailyReward)
		{
			_gameState.SecondsPlayedChangedEvent -= OnSecondsPlayedChanged;
			_triedToOpenDailyReward = false;
		}
		_popupManager.RequestPopup(new DailyRewardPopupRequest(_streak, _dayIndex, _storedRewards[_storedRewards.Count - 1]));
	}

	private List<Currency> GetStreak(int index)
	{
		return _properties.StreakProperties[index % _properties.StreakProperties.Count].Rewards;
	}

	private void IncrementStreak(double daysLapsed)
	{
		_triedToOpenDailyReward = false;
		if (_hasStoredLastReward && daysLapsed < 1.0)
		{
			_dayIndex++;
			_hasCollected = false;
			if (_dayIndex >= _streak.Count)
			{
				ResetStreak();
			}
		}
		else
		{
			ResetStreak();
			_hasStoredLastReward = false;
		}
	}

	private void ResetStreak()
	{
		_dayIndex = 0;
		_streakIndex++;
		_hasCollected = false;
		_streak = GetStreak(_streakIndex);
	}

	private void OnTutorialFinished(Tutorial tutorial)
	{
		if (_tutorialManager.InitialTutorialFinished)
		{
			_tutorialManager.TutorialFinishedEvent -= OnTutorialFinished;
			if (IsEnabled)
			{
				TryLapseDay();
			}
		}
	}

	private void OnVisuallyLevelledUp(int level)
	{
		if (IsEnabled)
		{
			TryLapseDay();
		}
	}

	PlannedNotification[] IHasNotification.GetNotifications()
	{
		List<PlannedNotification> list = new List<PlannedNotification>();
		if (IsEnabled && base.EndTimestampUTC.HasValue)
		{
			DateTime dateTime = AntiCheatDateTime.ConvertToSystemDateTime(base.EndTimestampUTC.Value, DateTimeKind.Utc);
			if (_hasCollected)
			{
				list.Add(new PlannedNotification((int)(dateTime.AddDays(0.5) - DateTime.UtcNow).TotalSeconds, Localization.Key("notification_daily_reward1"), sound: false, 3));
				list.Add(new PlannedNotification((int)(dateTime.AddDays(0.75) - DateTime.UtcNow).TotalSeconds, Localization.Key("notification_daily_reward2"), sound: false, 3));
			}
			else
			{
				int num = (int)(dateTime.AddDays(-0.5) - DateTime.UtcNow).TotalSeconds;
				int num2 = (int)(dateTime.AddDays(-0.25) - DateTime.UtcNow).TotalSeconds;
				if (num > 0)
				{
					list.Add(new PlannedNotification(num, Localization.Key("notification_daily_reward1"), sound: false, 3));
				}
				if (num2 > 0)
				{
					list.Add(new PlannedNotification(num2, Localization.Key("notification_daily_reward2"), sound: false, 3));
				}
			}
		}
		return list.ToArray();
	}

	public override void Serialize()
	{
		base.Serialize();
		_storage.Set("streakIndex", _streakIndex);
		_storage.Set("dayIndex", _dayIndex);
		_storage.Set("hasCollected", _hasCollected);
		_storage.Set("storedRewards", _storedRewards);
		_storage.Set("showDailyRewardWithoutVideoTimeTreshhold", _showDailyRewardWithoutVideoTimeTreshhold);
		_storage.Set("triedToOpenDailyReward", _triedToOpenDailyReward);
		_storage.Set("rewardIsDoubled", RewardIsDoubled);
		_storage.Set("hasStoredLastReward", _hasStoredLastReward);
	}
}
