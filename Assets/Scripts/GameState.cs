using CIG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : AbstractState, IProductConsumer
{
	public delegate void SecondsPlayedChangedEventHandler();

	public delegate void MinutePlayedEventHandler(long oldMinutesPlayed, long newMinutesPlayed);

	public delegate void LevelUpEventHandler(int oldLevel, int newLevel);

	public delegate void VisuallyLevelledUpEventHandler(int level);

	public delegate void BalanceChangedEventHandler(Currencies oldBalance, Currencies newBalance, FlyingCurrenciesData flyingCurrenciesData);

	public delegate void CurrenciesEarnedEventHandler(Currencies earned, CurrenciesEarnedReason reason);

	public delegate void CurrenciesSpentEventHandler(Currencies spent, CurrenciesSpentReason reason);

	[Serializable]
	public class LevelDefinition
	{
		[SerializeField]
		private int _xp;

		[SerializeField]
		private Currencies _reward;

		public int XP => _xp;

		public Currencies Reward => _reward;
	}

	public const int MagicElevenSeconds = 11;

	public const string TotalMinutesPlayedKey = "TotalMinutesPlayed";

	public const string TotalFiveMinuteSessionsKey = "TotalFiveMinuteSessions";

	public const string LevelKey = "Level";

	public const string EarnedBalanceKey = "EarnedBalance";

	public const string GiftedBalanceKey = "GiftedBalance";

	public const string CurrenciesEarnedKey = "CurrenciesEarned";

	public const string CurrenciesSpentKey = "CurrenciesSpent";

	public const string GlobalHappinessKey = "GlobalHappiness";

	public const string GlobalPopulationKey = "GlobalPopulation";

	public const string GlobalHousingKey = "GlobalHousing";

	public const string GlobalEmployeesKey = "GlobalEmployees";

	public const string GlobalJobsKey = "GlobalJobs";

	public const string CashCurrency = "Cash";

	public const string GoldCurrency = "Gold";

	public const string XPCurrency = "XP";

	public const string LevelUpCurrency = "LevelUp";

	public const string CraneCurrency = "Crane";

	public const string SilverKeyCurrency = "SilverKey";

	public const string GoldKeyCurrency = "GoldKey";

	public const string BuildingCurrency = "Building";

	public const string TokenCurrency = "Token";

	public const string FishCurrency = "Fish";

	public const string FishingRodCurrency = "FishingRod";

	public const string LevelUpRewardKey = "LevelUpReward";

	private const string ShownUpdatePopupVersionKey = "shownUpdatePopupVersion";

	private const string ShownUpdatePopupTimestampKey = "shownUpdatePopupTimestamp";

	private const string PopulationAbsDeltaKey = "PopulationAbsDelta";

	private const string EmployeesAbsDeltaKey = "EmployeesAbsDelta";

	private static readonly List<string> MoreCashGoldPopupCurrencies = new List<string>
	{
		"Gold",
		"Cash"
	};

	private const int AnalyticsPopulationEmployeesThreshold = 25;

	private readonly Multipliers _multipliers;

	private readonly PopupManager _popupManager;

	private readonly GameStateProperties _properties;

	private readonly string _startBalanceABGroup;

	private Currencies _earnedBalance;

	private float _secondsTimer;

	private float _minuteTimerInSeconds;

	private const string StartBalanceABGroupKey = "StartBalanceABGroup";

	public int MinutesPlayedInThisSession
	{
		get;
		protected set;
	}

	public int SecondsPlayedInThisSession
	{
		get;
		protected set;
	}

	public long TotalMinutesPlayed
	{
		get
		{
			return _storage.Get("TotalMinutesPlayed", 0L);
		}
		protected set
		{
			long totalMinutesPlayed = TotalMinutesPlayed;
			_storage.Set("TotalMinutesPlayed", value);
			OnValueChanged("TotalMinutesPlayed", totalMinutesPlayed, TotalMinutesPlayed);
		}
	}

	public long TotalFiveMinuteSessions
	{
		get
		{
			return _storage.Get("TotalFiveMinuteSessions", 0L);
		}
		protected set
		{
			long totalFiveMinuteSessions = TotalFiveMinuteSessions;
			_storage.Set("TotalFiveMinuteSessions", value);
			OnValueChanged("TotalFiveMinuteSessions", totalFiveMinuteSessions, TotalFiveMinuteSessions);
		}
	}

	public int MaxLevel => _properties.LevelProperties.Count;

	public int Level
	{
		get
		{
			return _storage.Get("Level", 1);
		}
		private set
		{
			int level = Level;
			_storage.Set("Level", value);
			OnValueChanged("Level", level, Level);
		}
	}

	public bool ReachedMaxLevel => Level >= MaxLevel;

	public float LevelProgress
	{
		get
		{
			if (ReachedMaxLevel)
			{
				return 1f;
			}
			float num = (float)Balance.GetValue("XP");
			float num2 = GetXpForLevel(Level);
			float num3 = GetXpForLevel(Level + 1);
			return Mathf.Clamp01((num - num2) / (num3 - num2));
		}
	}

	public Currencies GiftedBalance
	{
		get
		{
			return _storage.GetModel("GiftedBalance", (StorageDictionary sd) => new Currencies(sd), new Currencies());
		}
		private set
		{
			if (value.ContainsPositive("XP"))
			{
				EarnedBalance += Currency.XPCurrency(value.GetValue("XP"));
			}
			Currencies currencies = value.Filter("XP", "Invalid");
			Currencies giftedBalance = GiftedBalance;
			_storage.Set("GiftedBalance", currencies);
			OnValueChanged("GiftedBalance", giftedBalance, currencies);
		}
	}

	public Currencies EarnedBalance
	{
		get
		{
			return _earnedBalance;
		}
		private set
		{
			decimal value2 = _earnedBalance.GetValue("XP");
			decimal num = value.GetValue("XP");
			if (num < value2)
			{
				UnityEngine.Debug.LogError($"XP went down from {value2} to {num}. This change is ignored, as this should never happen!");
				num = value2;
			}
			ProcessXPGain(num);
			Currencies currencies = value.Filter("XP", "Invalid") + Currency.XPCurrency(ReachedMaxLevel ? ((decimal)GetXpForLevel(Level)) : num);
			Currencies earnedBalance = EarnedBalance;
			_earnedBalance = currencies;
			OnValueChanged("EarnedBalance", earnedBalance, currencies);
		}
	}

	public Currencies Balance
	{
		get
		{
			return EarnedBalance + GiftedBalance;
		}
		private set
		{
			EarnedBalance = (value - GiftedBalance).WithoutNegative();
			GiftedBalance = GiftedBalance.Cap(value - EarnedBalance, capUnknownWithZero: true);
		}
	}

	public Currencies CurrenciesEarned
	{
		get
		{
			return _storage.GetModel("CurrenciesEarned", (StorageDictionary sd) => new Currencies(sd), new Currencies());
		}
		private set
		{
			Currencies value2 = value.Filter("Invalid");
			Currencies currenciesEarned = CurrenciesEarned;
			_storage.Set("CurrenciesEarned", value2);
			OnValueChanged("CurrenciesEarned", currenciesEarned, CurrenciesEarned);
		}
	}

	public Currencies CurrenciesSpent
	{
		get
		{
			return _storage.GetModel("CurrenciesSpent", (StorageDictionary sd) => new Currencies(sd), new Currencies());
		}
		private set
		{
			Currencies value2 = value.Filter("Invalid");
			Currencies currenciesSpent = CurrenciesSpent;
			_storage.Set("CurrenciesSpent", value2);
			OnValueChanged("CurrenciesSpent", currenciesSpent, CurrenciesSpent);
		}
	}

	public int GlobalHappiness
	{
		get
		{
			return _storage.Get("GlobalHappiness", 0);
		}
		private set
		{
			int globalHappiness = GlobalHappiness;
			_storage.Set("GlobalHappiness", value);
			OnValueChanged("GlobalHappiness", globalHappiness, GlobalHappiness);
		}
	}

	public int GlobalPopulation
	{
		get
		{
			return _storage.Get("GlobalPopulation", 0);
		}
		private set
		{
			int globalPopulation = GlobalPopulation;
			_storage.Set("GlobalPopulation", value);
			OnValueChanged("GlobalPopulation", globalPopulation, value);
			PopulationAbsDelta += Mathf.Abs(value - globalPopulation);
			if (PopulationAbsDelta > 25)
			{
				Analytics.PopulationReached(value);
				PopulationAbsDelta = 0;
			}
		}
	}

	public int GlobalHousing
	{
		get
		{
			return _storage.Get("GlobalHousing", 0);
		}
		private set
		{
			int globalHousing = GlobalHousing;
			_storage.Set("GlobalHousing", value);
			OnValueChanged("GlobalHousing", globalHousing, GlobalHousing);
		}
	}

	public int GlobalEmployees
	{
		get
		{
			return _storage.Get("GlobalEmployees", 0);
		}
		private set
		{
			int globalEmployees = GlobalEmployees;
			_storage.Set("GlobalEmployees", value);
			OnValueChanged("GlobalEmployees", globalEmployees, value);
			EmployeesAbsDelta += Mathf.Abs(value - globalEmployees);
			if (EmployeesAbsDelta > 25)
			{
				Analytics.EmployeesReached(value);
				EmployeesAbsDelta = 0;
			}
		}
	}

	public int GlobalJobs
	{
		get
		{
			return _storage.Get("GlobalJobs", 0);
		}
		private set
		{
			int globalJobs = GlobalJobs;
			_storage.Set("GlobalJobs", value);
			OnValueChanged("GlobalJobs", globalJobs, GlobalJobs);
		}
	}

	public Currencies LevelUpReward
	{
		get
		{
			return _storage.GetModel("LevelUpReward", (StorageDictionary sd) => new Currencies(sd), new Currencies());
		}
		private set
		{
			Currencies levelUpReward = LevelUpReward;
			_storage.Set("LevelUpReward", value);
			OnValueChanged("LevelUpReward", levelUpReward, LevelUpReward);
		}
	}

	public string ShownUpdatePopupVersion
	{
		get
		{
			return _storage.Get("shownUpdatePopupVersion", string.Empty);
		}
		set
		{
			string shownUpdatePopupVersion = ShownUpdatePopupVersion;
			_storage.Set("shownUpdatePopupVersion", value);
			OnValueChanged("shownUpdatePopupVersion", shownUpdatePopupVersion, ShownUpdatePopupVersion);
		}
	}

	public double ShownUpdatePopupTimestamp
	{
		get
		{
			return _storage.Get("shownUpdatePopupTimestamp", 0.0);
		}
		set
		{
			double shownUpdatePopupTimestamp = ShownUpdatePopupTimestamp;
			_storage.Set("shownUpdatePopupTimestamp", value);
			OnValueChanged("shownUpdatePopupTimestamp", shownUpdatePopupTimestamp, ShownUpdatePopupTimestamp);
		}
	}

	private int PopulationAbsDelta
	{
		get
		{
			return _storage.Get("PopulationAbsDelta", 0);
		}
		set
		{
			_storage.Set("PopulationAbsDelta", value);
		}
	}

	private int EmployeesAbsDelta
	{
		get
		{
			return _storage.Get("EmployeesAbsDelta", 0);
		}
		set
		{
			_storage.Set("EmployeesAbsDelta", value);
		}
	}

	public event SecondsPlayedChangedEventHandler SecondsPlayedChangedEvent;

	public event MinutePlayedEventHandler MinutePlayedEvent;

	public event LevelUpEventHandler LevelUpEvent;

	public event VisuallyLevelledUpEventHandler VisuallyLevelledUpEvent;

	public event BalanceChangedEventHandler BalanceChangedEvent;

	public event CurrenciesEarnedEventHandler CurrenciesEarnedEvent;

	public event CurrenciesSpentEventHandler CurrenciesSpentEvent;

	private void FireSecondsPlayedChangedEvent()
	{
		this.SecondsPlayedChangedEvent?.Invoke();
	}

	private void FireMinutePlayedEvent(long oldMinutesPlayed, long newMinutesPlayed)
	{
		this.MinutePlayedEvent?.Invoke(oldMinutesPlayed, newMinutesPlayed);
	}

	private void FireLevelUpEvent(int oldLevel, int newLevel)
	{
		this.LevelUpEvent?.Invoke(oldLevel, newLevel);
	}

	private void FireVisuallyLevelledUp(int level)
	{
		this.VisuallyLevelledUpEvent?.Invoke(level);
	}

	private void FireBalanceChangedEvent(Currencies oldBalance, Currencies newBalance, FlyingCurrenciesData flyingCurrenciesData)
	{
		this.BalanceChangedEvent?.Invoke(oldBalance, newBalance, flyingCurrenciesData);
	}

	private void FireCurrenciesEarnedEvent(Currencies earned, CurrenciesEarnedReason reason)
	{
		this.CurrenciesEarnedEvent?.Invoke(earned, reason);
	}

	private void FireCurrenciesSpentEvent(Currencies spent, CurrenciesSpentReason reason)
	{
		this.CurrenciesSpentEvent?.Invoke(spent, reason);
	}

	public GameState(StorageDictionary storage, Multipliers multipliers, PopupManager popupManager, RoutineRunner routineRunner, GameStateProperties properties)
		: base(storage)
	{
		_multipliers = multipliers;
		_popupManager = popupManager;
		_properties = properties;
		_startBalanceABGroup = _storage.Get("StartBalanceABGroup", "unknown");
		if (_storage.Contains("EarnedBalance"))
		{
			_earnedBalance = _storage.GetModel("EarnedBalance", (StorageDictionary sd) => new Currencies(sd), new Currencies());
		}
		else
		{
			_startBalanceABGroup = _properties.GetRandomStartBalanceABGroup();
			_earnedBalance = _properties.GetStartBalance(_startBalanceABGroup);
			Analytics.StartBalanceGroupSet(_startBalanceABGroup);
		}
		Analytics.SetABGroupCollection(_startBalanceABGroup);
		CheckGlobalValues();
		GiveLevelUpReward();
		routineRunner.StartCoroutine(CountTimeRoutine());
	}

	public void EarnCurrencies(Currency c, CurrenciesEarnedReason earnedReason, FlyingCurrenciesData flyingCurrenciesProperties)
	{
		if (c.IsValid)
		{
			EarnCurrencies(new Currencies(c), earnedReason, flyingCurrenciesProperties);
		}
		else
		{
			UnityEngine.Debug.LogError($"Earned an invalid currency ({c}) from {earnedReason.ToString()}.");
		}
	}

	public void EarnCurrencies(Currencies currencies, CurrenciesEarnedReason earnedReason, FlyingCurrenciesData flyingCurrenciesProperties)
	{
		if (currencies.Contains("Invalid"))
		{
			UnityEngine.Debug.LogError(string.Format("Earned an invalid currency ({0}) from {1}. '{2}' will be removed.", currencies, earnedReason.ToString(), "Invalid"));
		}
		Currencies currencies2 = currencies.Filter("Invalid");
		if (!currencies2.IsEmpty())
		{
			Currencies balance = Balance;
			Balance += currencies2;
			CurrenciesEarned += currencies2;
			FireCurrenciesEarnedEvent(currencies2, earnedReason);
			FireBalanceChangedEvent(balance, Balance, flyingCurrenciesProperties);
		}
	}

	public void GiveCurrencies(Currencies currencies, CurrenciesEarnedReason earnedReason)
	{
		if (currencies.Contains("Invalid"))
		{
			UnityEngine.Debug.LogError(string.Format("Earned an invalid currency ({0}) from {1}. '{2}' will be removed.", currencies, earnedReason.ToString(), "Invalid"));
		}
		Currencies currencies2 = currencies.Filter("Invalid");
		if (!currencies2.IsEmpty())
		{
			Currencies balance = Balance;
			GiftedBalance += currencies2;
			FireCurrenciesEarnedEvent(currencies2, earnedReason);
			FireBalanceChangedEvent(balance, Balance, new FlyingCurrenciesData());
		}
	}

	public bool CanAfford(Currency c)
	{
		return !Balance.MissingCurrency(c);
	}

	public bool CanAfford(Currencies c)
	{
		return Balance.MissingCurrencies(c).IsEmpty();
	}

	public bool SpendCurrencies(Currency c, CurrenciesSpentReason spentReason, Action<bool, Currencies> purchaseCallback)
	{
		return SpendCurrencies(c, 0, allowShopPopup: true, spentReason, purchaseCallback);
	}

	public bool SpendCurrencies(Currency c, int maxGoldPrice, bool allowShopPopup, CurrenciesSpentReason spentReason, Action<bool, Currencies> purchaseCallback)
	{
		return SpendCurrencies(new Currencies(c), maxGoldPrice, allowShopPopup, spentReason, purchaseCallback);
	}

	public bool SpendCurrencies(Currencies c, CurrenciesSpentReason spentReason, Action<bool, Currencies> purchaseCallback)
	{
		return SpendCurrencies(c, 0, allowShopPopup: true, spentReason, purchaseCallback);
	}

	public bool SpendCurrencies(Currencies c, int maxGoldPrice, bool allowShopPopup, CurrenciesSpentReason spentReason, Action<bool, Currencies> purchaseCallback)
	{
		Currencies currencies = Balance.MissingCurrencies(c);
		if (!currencies.IsEmpty())
		{
			if (CanShowMoreCashGold(currencies) || maxGoldPrice > 0)
			{
				_popupManager.RequestPopup(new MoreCashGoldPopupRequest(c, maxGoldPrice, allowShopPopup, purchaseCallback));
			}
			else
			{
				UnityEngine.Debug.LogError($"Missing currencies is not supported for currency '{currencies}'.");
			}
			return false;
		}
		Currencies balance = Balance;
		Balance -= c;
		CurrenciesSpent += c;
		FireCurrenciesSpentEvent(c, spentReason);
		FireBalanceChangedEvent(balance, Balance, new FlyingCurrenciesData());
		purchaseCallback?.Invoke(arg1: true, c);
		SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.CoinsCashSpent);
		return true;
	}

	public bool CanShowMoreCashGold(Currencies currencies)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			if (!CanShowMoreCashGold(currencies.GetCurrency(i)))
			{
				return false;
			}
		}
		return true;
	}

	public bool CanShowMoreCashGold(Currency currency)
	{
		return MoreCashGoldPopupCurrencies.Contains(currency.Name);
	}

	public void AddGlobalPopulation(int pop)
	{
		GlobalPopulation += pop;
	}

	public void AddGlobalHousing(int room)
	{
		GlobalHousing += room;
	}

	public void AddGlobalHappiness(int smiles)
	{
		GlobalHappiness += smiles;
	}

	public void AddGlobalEmployees(int emp)
	{
		GlobalEmployees += emp;
	}

	public void AddGlobalJobs(int jobs)
	{
		GlobalJobs += jobs;
	}

	public void GiveLevelUpReward(Currencies reward)
	{
		if (!reward.IsEmpty())
		{
			Currencies currencies = LevelUpReward - reward;
			if (currencies.ContainsNegative())
			{
				UnityEngine.Debug.LogWarning("Something has requested to give more rewards for a level up than are currently available. Giving the remaining level up rewards instead.");
				GiveLevelUpReward();
			}
			else
			{
				GainLevelUpReward(reward);
				LevelUpReward = currencies;
			}
		}
	}

	public decimal GetXpForLevelsUp(decimal levelsUp)
	{
		decimal d = Math.Min(decimal.One - (decimal)LevelProgress, levelsUp);
		levelsUp -= d;
		int num = (int)Math.Floor(levelsUp);
		decimal d2 = levelsUp - (decimal)num;
		decimal d3 = (decimal)(GetXpForLevel(Level + 1) - GetXpForLevel(Level)) * d;
		decimal d4 = GetXpForLevel(Level + 1 + num) - GetXpForLevel(Level + 1);
		decimal d5 = (decimal)(GetXpForLevel(Level + num + 2) - GetXpForLevel(Level + 1 + num)) * d2;
		return d3 + d4 + d5;
	}

	public void VisuallyLevelledUp(int level)
	{
		FireVisuallyLevelledUp(level);
	}

	public Currencies GetLevelReward(int level)
	{
		Currencies reward = _properties.LevelProperties[Mathf.Clamp(level - 1, 0, _properties.LevelProperties.Count - 1)].Reward;
		reward.SetValue("Cash", Math.Round(reward.GetValue("Cash") * _multipliers.GetMultiplier(MultiplierType.LevelUpCashReward)));
		return reward;
	}

	private void MinutePlayed()
	{
		long oldMinutesPlayed = MinutesPlayedInThisSession;
		TotalMinutesPlayed++;
		MinutesPlayedInThisSession++;
		long newMinutesPlayed = MinutesPlayedInThisSession;
		if (MinutesPlayedInThisSession == 5)
		{
			TotalFiveMinuteSessions++;
		}
		FireMinutePlayedEvent(oldMinutesPlayed, newMinutesPlayed);
	}

	private void SecondPlayed11()
	{
		SecondsPlayedInThisSession += 11;
		FireSecondsPlayedChangedEvent();
	}

	private void CheckGlobalValues()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		IList<IslandId> allIslandIds = IslandExtensions.AllIslandIds;
		int i = 0;
		for (int count = allIslandIds.Count; i < count; i++)
		{
			IslandId islandId = allIslandIds[i];
			if (islandId.IsValid())
			{
				StorageDictionary storageDict = StorageController.GameRoot.GetStorageDict(IslandBootstrapper.GetIslandStorageKey(islandId)).GetStorageDict("IslandState");
				num += storageDict.Get("Happiness", 0);
				num2 += storageDict.Get("Housing", 0);
				num3 += storageDict.Get("Population", 0);
				num4 += storageDict.Get("Jobs", 0);
				num5 += storageDict.Get("Employees", 0);
			}
		}
		if (num != GlobalHappiness)
		{
			UnityEngine.Debug.LogWarning($"GlobalHappiness {GlobalHappiness} does not match sum of (island) happiness {num}.");
			GlobalHappiness = num;
		}
		if (num2 != GlobalHousing)
		{
			UnityEngine.Debug.LogWarning($"GlobalHousing {GlobalHousing} does not match sum of (island) housing {num2}.");
			GlobalHousing = num2;
		}
		if (num3 != GlobalPopulation)
		{
			UnityEngine.Debug.LogWarning($"GlobalPopulation {GlobalPopulation} does not match sum of (island) population {num3}.");
			GlobalPopulation = num3;
		}
		if (num4 != GlobalJobs)
		{
			UnityEngine.Debug.LogWarning($"GlobalJobs {GlobalJobs} does not match sum of (island) jobs {num4}.");
			GlobalJobs = num4;
		}
		if (num5 != GlobalEmployees)
		{
			UnityEngine.Debug.LogWarning($"GlobalEmployees {GlobalEmployees} does not match sum of (island) employees {num5}.");
			GlobalEmployees = num5;
		}
	}

	private void ProcessXPGain(decimal newXp)
	{
		if (ReachedMaxLevel)
		{
			return;
		}
		decimal d = GetXpForLevel(Level + 1);
		while (!ReachedMaxLevel && newXp >= d)
		{
			LevelUp();
			if (ReachedMaxLevel)
			{
				break;
			}
			d = GetXpForLevel(Level + 1);
		}
	}

	private void LevelUp()
	{
		if (ReachedMaxLevel)
		{
			UnityEngine.Debug.LogWarning("Can't level up because the maximum level has been reached already.");
			return;
		}
		Currencies reward = _properties.LevelProperties[Level].Reward;
		reward.SetValue("Cash", Math.Round(reward.GetValue("Cash") * _multipliers.GetMultiplier(MultiplierType.LevelUpCashReward)));
		IncreaseLevel();
		LevelUpReward += reward;
	}

	private void IncreaseLevel()
	{
		int level = Level;
		Level++;
		FireLevelUpEvent(level, Level);
	}

	private int GetXpForLevel(int level)
	{
		return _properties.LevelProperties[Mathf.Clamp(level - 1, 0, _properties.LevelProperties.Count - 1)].XpNeeded;
	}

	private void GiveLevelUpReward()
	{
		Currencies levelUpReward = LevelUpReward;
		if (!levelUpReward.IsEmpty())
		{
			GainLevelUpReward(levelUpReward);
			LevelUpReward = new Currencies();
		}
	}

	private void GainLevelUpReward(Currencies reward)
	{
		EarnCurrencies(reward, CurrenciesEarnedReason.LevelUp, new FlyingCurrenciesData());
	}

	private IEnumerator CountTimeRoutine()
	{
		while (true)
		{
			float unscaledDeltaTime = Time.unscaledDeltaTime;
			_secondsTimer += unscaledDeltaTime;
			_minuteTimerInSeconds += unscaledDeltaTime;
			if (_secondsTimer > 11f)
			{
				SecondPlayed11();
				_secondsTimer %= 11f;
			}
			if (_minuteTimerInSeconds > 60f)
			{
				MinutePlayed();
				_minuteTimerInSeconds %= 60f;
			}
			yield return null;
		}
	}

	bool IProductConsumer.ConsumeProduct(TOCIStoreProduct product)
	{
		StoreProductCategory category = product.Category;
		if ((uint)(category - 1) <= 1u)
		{
			Currencies currencies = product.Currencies;
			GiveCurrencies(currencies, CurrenciesEarnedReason.IAP);
			return true;
		}
		UnityEngine.Debug.LogErrorFormat("Missing product consumer for product category '{0}'", product.Category);
		return false;
	}

	public void Serialize()
	{
		_storage.Set("EarnedBalance", _earnedBalance);
		_storage.Set("StartBalanceABGroup", _startBalanceABGroup);
	}
}
