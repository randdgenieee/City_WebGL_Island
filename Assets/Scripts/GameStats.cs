using CIG;
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameStats : AbstractState
{
	public delegate void SpeedupExecutedEventHandler();

	public const string NumberOfKey = "NumberOf";

	public const string CashCountKey = "CashCount";

	public const string NumberOfExpansionsPurchasedKey = "NumberOfExpansionsPurchased";

	public const string NumberOfExpansionsPurchasedWithGoldKey = "NumberOfExpansionsPurchasedWithgold";

	public const string NumberOfExpansionsPurchasedWithCashKey = "NumberOfExpansionsPurchasedWithCash";

	public const string NumberOfUnlockedElementsKey = "NumberOfUnlockedElements";

	public const string NumberOfUsedElementsKey = "NumberOfUsedElements";

	private const string UnlockedSurfaceTypeCountsKey = "UnlockedSurfaceTypeCounts";

	public const string NumberOfUpgradesKey = "NumberOfUpgrades";

	public const string NumberOfBuildingsKey = "NumberOfBuildings";

	public const string NumberOfLevel10UpgradesKey = "NumberOfLevel10Upgrades";

	public const string GlobalBuildingsBuiltKey = "GlobalBuildingsBuilt";

	public const string GlobalBuildingsBuiltWithGoldKey = "GlobalBuildingsBuiltWithGold";

	public const string NumberOfGoldVideosWatchedKey = "GoldVideosWatched";

	public const string NumberOfVideosWatchedKey = "GoldVideosWatched";

	public const string NumberOfVideosClickedKey = "GoldVideosClicked";

	public const string NumberOfInterstitialsWatchedKey = "InterstitialsWatched";

	public const string NumberOfInterstitialsClickedKey = "InterstitialsClicked";

	public const string NumberOfGoldPackagesBoughtKey = "NumberOfGoldPackagesBought";

	public const string CashCollectedKey = "CashCollected";

	public const string TimesCollectedKey = "TimesCollected";

	public const string SpeedupsExecutedKey = "SpeedupsExecuted";

	public const string NumberOfIslandsUnlockedKey = "NumberOfIslandsUnlocked";

	public const string DaysPlayedStreakKey = "DaysPlayedStreak";

	public const string NumberOfTimesPlayedKey = "NumberOfTimesPlayed";

	public const string IslandStatsDataKey = "IslandStatsData";

	public const string FirstDateTimePlayedKey = "FirstDateTimePlayed";

	public const string LastDatePlayedKey = "LastDatePlayed";

	public const string NumberOfDaysPlayedKey = "NumberOfDaysPlayed";

	public const string NumberOfDailyBonusStreaksCompletedKey = "NumberOfDailyBonusStreaksCompleted";

	public const string NumberOfIAPsPurchasedKey = "NumberOfIAPsPurchased";

	public const string NumberOfCurrencyMenuWatchedKey = "NumberOfCurrencyMenuWatched";

	public const string NumberOfKeyDealsWithoutLandmarksPurchasedKey = "NumberOfKeyDealsWithoutLandmarksPurchased";

	public const string NumberOfLandmarksPurchasedKey = "NumberOfLandmarksPurchased";

	public const string NumberOfWoodenChestsOpenedKey = "NumberOfWoodenChestsOpened";

	public const string NumberOfSilverChestsOpenedKey = "NumberOfSilverChestsOpened";

	public const string NumberOfGoldChestsOpenedKey = "NumberOfGoldChestsOpened";

	public const string NumberOfPlatinumChestsOpenedKey = "NumberOfPlatinumChestsOpened";

	public const string NumberOfSilverKeysSpentOnSilverChestsKey = "NumberOfSilverKeysSpentOnSilverChests";

	public const string NumberOfGoldKeysSpentOnGoldChestsKey = "NumberOfGoldKeysSpentOnGoldChests";

	public const string NumberOfGoldKeysSpentOnPlatinumChestsKey = "NumberOfGoldKeysSpentOnPlatinumChests";

	public const string NumberOfBuildingsBuiltOnGrassKey = "NumberOfBuildingsBuiltOnGrass";

	public const string NumberOfBuildingsBuiltOnBeachKey = "NumberOfBuildingsBuiltOnBeach";

	public const string NumberOfBuildingsBuiltOnWaterKey = "NumberOfBuildingsBuiltOnWater";

	public const string NumberOfBuildingsBuiltOnSandKey = "NumberOfBuildingsBuiltOnSand";

	public const string NumberOfBuildingsBuiltOnRockKey = "NumberOfBuildingsBuiltOnRock";

	public const string NumberOfBuildingsBuiltOnMudKey = "NumberOfBuildingsBuiltOnMud";

	public const string NumberOfBuildingsBuiltOnSnowKey = "NumberOfBuildingsBuiltOnSnow";

	public const string NumberOfCommercialBuildingsBuiltKey = "NumberOfCommercialBuildingsBuilt";

	public const string NumberOfResidentialBuildingsBuiltKey = "NumberOfResidentialBuildingsBuilt";

	public const string NumberOfCommunityBuildingsBuiltKey = "NumberOfCommunityBuildingsBuilt";

	public const string NumberOfDecorationBuildingsBuiltKey = "NumberOfDecorationBuildingsBuilt";

	public const string NumberOfRoadsBuiltKey = "NumberOfRoadsBuilt";

	private const string CashSpentKey = "CashSpent";

	private const string CashSpentOnCurrencyConversionKey = "CashSpentOnCurrencyConversion";

	public const string SilverKeysSpentKey = "NumberOfSilverKeysSpent";

	private const string SilverKeysSpentOnCurrencyConversionKey = "SilverKeysSpentOnCurrencyConversion";

	private const string SilverKeysEarnedKey = "NumberOfSilverKeysReceived";

	private const string SilverKeysEarnedFromUpgradesKey = "NumberOfSilverKeysReceivedFromUpgrades";

	private const string SilverKeysEarnedFromExpansionChestKey = "SilverKeysEarnedFromExpansionChest";

	private const string SilverKeysEarnedFromWalkerBalloonKey = "SilverKeysEarnedFromWalkerBalloon";

	private const string SilverKeysEarnedFromCurrencyConversionKey = "SilverKeysEarnedFromCurrencyConversion";

	public const string GoldKeysSpentKey = "NumberOfGoldKeysSpent";

	private const string GoldKeysSpentOnCurrencyConversionKey = "GoldKeysSpentOnCurrencyConversion";

	private const string GoldKeysEarnedKey = "NumberOfGoldKeysReceived";

	private const string GoldKeysEarnedFromUpgradesKey = "NumberOfGoldKeysReceivedFromUpgrades";

	private const string GoldKeysEarnedFromExpansionChestKey = "GoldKeysEarnedFromExpansionChest";

	private const string GoldKeysEarnedFromCurrencyConversionKey = "GoldKeysEarnedFromCurrencyConversion";

	private const string GoldKeysEarnedWheelOfFortuneKey = "GoldKeysEarned_WheelOfFortune";

	public const string GoldSpentKey = "GoldSpent";

	private const string GoldSpentSpeedupsKey = "GoldSpent_Speedups";

	private const string GoldSpentUpgradeSpeedupsKey = "GoldSpent_UpgradeSpeedups";

	private const string GoldSpentBuildSpeedupsKey = "GoldSpent_BuildSpeedups";

	private const string GoldSpentAirshipSpeedupsKey = "GoldSpent_AirshipSpeedups";

	private const string GoldSpentDemolishSpeedupsKey = "GoldSpent_DemolishSpeedups";

	private const string GoldSpentGoldBuildingsKey = "GoldSpent_GoldBuildings";

	private const string GoldSpentCashBuildingsKey = "GoldSpent_CashBuildings2";

	private const string GoldSpentExpansionsKey = "GoldSpent_Expansions";

	private const string GoldSpentCashExchangeKey = "GoldSpent_CashExchange";

	private const string GoldSpentCraneHireKey = "GoldSpent_CraneHire";

	private const string GoldSpentCranePurchaseKey = "GoldSpent_CranePurchase";

	private const string GoldSpentImmediateBuildKey = "GoldSpent_ImmediateBuild";

	private const string GoldSpentOnImmediateUpgradesKey = "NumberIFSpedUpUpgrades";

	private const string GoldSpentWheelOfFortuneKey = "GoldSpent_WheelOfFortune";

	private const string GoldEarnedFromCurrencyConversionKey = "GoldEarnedFromCurrencyConversion";

	private const string GoldEarnedWheelOfFortuneKey = "GoldEarned_WheelOfFortune";

	private const string TokensSpentWheelOfFortuneKey = "TokensSpent_WheelOfFortune";

	private const string TokensSpentKey = "TokensSpent";

	private const string TokensEarnedFromExpansionChestKey = "TokensEarnedFromExpansionChest";

	private const string CurrenciesEarnedFriendIslandVisitingKey = "CurrenciesEarnedFriendIslandVisiting";

	public const string DailyQuestsCompletedTodayKey = "DailyQuestsCompletedToday";

	private readonly GameState _gameState;

	private IsometricIsland _currentIsland;

	private IslandId _islandId = IslandId.None;

	private readonly Dictionary<IslandId, CIGIslandStats> _cachedIslandStats = new Dictionary<IslandId, CIGIslandStats>();

	private Builder _subscribedToBuilder;

	private RoadBuilder _subscribedToRoadBuilder;

	private IsometricGrid _subscribedToGrid;

	private StorageDictionary NumberOfDict => _storage.GetStorageDict("NumberOf");

	private StorageDictionary CashCountDict => _storage.GetStorageDict("CashCount");

	public int NumberOfExpansionsPurchased
	{
		get
		{
			return _storage.Get("NumberOfExpansionsPurchased", 0);
		}
		private set
		{
			int numberOfExpansionsPurchased = NumberOfExpansionsPurchased;
			_storage.Set("NumberOfExpansionsPurchased", value);
			OnValueChanged("NumberOfExpansionsPurchased", numberOfExpansionsPurchased, NumberOfExpansionsPurchased);
		}
	}

	public int NumberOfExpansionsPurchasedWithGold
	{
		get
		{
			return _storage.Get("NumberOfExpansionsPurchasedWithgold", 0);
		}
		private set
		{
			int numberOfExpansionsPurchasedWithGold = NumberOfExpansionsPurchasedWithGold;
			_storage.Set("NumberOfExpansionsPurchasedWithgold", value);
			OnValueChanged("NumberOfExpansionsPurchasedWithgold", numberOfExpansionsPurchasedWithGold, NumberOfExpansionsPurchasedWithGold);
		}
	}

	public int NumberOfExpansionsPurchasedWithCash
	{
		get
		{
			return _storage.Get("NumberOfExpansionsPurchasedWithCash", 0);
		}
		private set
		{
			int numberOfExpansionsPurchasedWithCash = NumberOfExpansionsPurchasedWithCash;
			_storage.Set("NumberOfExpansionsPurchasedWithCash", value);
			OnValueChanged("NumberOfExpansionsPurchasedWithCash", numberOfExpansionsPurchasedWithCash, NumberOfExpansionsPurchasedWithCash);
		}
	}

	public int NumberOfUnlockedElements
	{
		get
		{
			return _storage.Get("NumberOfUnlockedElements", 0);
		}
		private set
		{
			int numberOfUnlockedElements = NumberOfUnlockedElements;
			_storage.Set("NumberOfUnlockedElements", value);
			OnValueChanged("NumberOfUnlockedElements", numberOfUnlockedElements, NumberOfUnlockedElements);
		}
	}

	public int NumberOfUsedElements
	{
		get
		{
			return _storage.Get("NumberOfUsedElements", 0);
		}
		private set
		{
			int numberOfUsedElements = NumberOfUsedElements;
			_storage.Set("NumberOfUsedElements", value);
			OnValueChanged("NumberOfUsedElements", numberOfUsedElements, NumberOfUsedElements);
		}
	}

	private StorageDictionary UnlockedSurfaceTypeCounts => _storage.GetStorageDict("UnlockedSurfaceTypeCounts");

	public int NumberOfUpgrades
	{
		get
		{
			return _storage.Get("NumberOfUpgrades", 0);
		}
		private set
		{
			int numberOfUpgrades = NumberOfUpgrades;
			_storage.Set("NumberOfUpgrades", value);
			OnValueChanged("NumberOfUpgrades", numberOfUpgrades, NumberOfUpgrades);
		}
	}

	public int NumberOfBuildings
	{
		get
		{
			return _storage.Get("NumberOfBuildings", 0);
		}
		private set
		{
			int numberOfBuildings = NumberOfBuildings;
			_storage.Set("NumberOfBuildings", value);
			OnValueChanged("NumberOfBuildings", numberOfBuildings, NumberOfBuildings);
		}
	}

	public int NumberOfLevel10Upgrades
	{
		get
		{
			return _storage.Get("NumberOfLevel10Upgrades", 0);
		}
		private set
		{
			int numberOfLevel10Upgrades = NumberOfLevel10Upgrades;
			_storage.Set("NumberOfLevel10Upgrades", value);
			OnValueChanged("NumberOfLevel10Upgrades", numberOfLevel10Upgrades, NumberOfLevel10Upgrades);
		}
	}

	public int GlobalBuildingsBuilt
	{
		get
		{
			return _storage.Get("GlobalBuildingsBuilt", 0);
		}
		private set
		{
			int globalBuildingsBuilt = GlobalBuildingsBuilt;
			_storage.Set("GlobalBuildingsBuilt", value);
			OnValueChanged("GlobalBuildingsBuilt", globalBuildingsBuilt, GlobalBuildingsBuilt);
		}
	}

	public int NumberOfVideosWatched
	{
		get
		{
			return _storage.Get("GoldVideosWatched", 0);
		}
		private set
		{
			int numberOfVideosWatched = NumberOfVideosWatched;
			_storage.Set("GoldVideosWatched", value);
			OnValueChanged("GoldVideosWatched", numberOfVideosWatched, NumberOfVideosWatched);
		}
	}

	public int NumberOfVideosClicked
	{
		get
		{
			return _storage.Get("GoldVideosClicked", 0);
		}
		private set
		{
			int numberOfVideosClicked = NumberOfVideosClicked;
			_storage.Set("GoldVideosClicked", value);
			OnValueChanged("GoldVideosClicked", numberOfVideosClicked, NumberOfVideosClicked);
		}
	}

	public int NumberOfInterstitialsWatched
	{
		get
		{
			return _storage.Get("InterstitialsWatched", 0);
		}
		private set
		{
			int numberOfInterstitialsWatched = NumberOfInterstitialsWatched;
			_storage.Set("InterstitialsWatched", value);
			OnValueChanged("InterstitialsWatched", numberOfInterstitialsWatched, NumberOfInterstitialsWatched);
		}
	}

	public int NumberOfInterstitialsClicked
	{
		get
		{
			return _storage.Get("InterstitialsClicked", 0);
		}
		private set
		{
			int numberOfInterstitialsClicked = NumberOfInterstitialsClicked;
			_storage.Set("InterstitialsClicked", value);
			OnValueChanged("InterstitialsClicked", numberOfInterstitialsClicked, NumberOfInterstitialsClicked);
		}
	}

	public int NumberOfGoldPackagesBought
	{
		get
		{
			return _storage.Get("NumberOfGoldPackagesBought", 0);
		}
		private set
		{
			int numberOfGoldPackagesBought = NumberOfGoldPackagesBought;
			_storage.Set("NumberOfGoldPackagesBought", value);
			OnValueChanged("NumberOfGoldPackagesBought", numberOfGoldPackagesBought, NumberOfGoldPackagesBought);
		}
	}

	public decimal CashCollected
	{
		get
		{
			return _storage.Get("CashCollected", decimal.Zero);
		}
		private set
		{
			decimal cashCollected = CashCollected;
			_storage.Set("CashCollected", value);
			OnValueChanged("CashCollected", cashCollected, CashCollected);
		}
	}

	public int TimesCollected
	{
		get
		{
			return _storage.Get("TimesCollected", 0);
		}
		private set
		{
			int timesCollected = TimesCollected;
			_storage.Set("TimesCollected", value);
			OnValueChanged("TimesCollected", timesCollected, TimesCollected);
		}
	}

	public int SpeedupsExecuted
	{
		get
		{
			return _storage.Get("SpeedupsExecuted", 0);
		}
		private set
		{
			int speedupsExecuted = SpeedupsExecuted;
			_storage.Set("SpeedupsExecuted", value);
			OnValueChanged("SpeedupsExecuted", speedupsExecuted, SpeedupsExecuted);
		}
	}

	public int NumberOfIslandsUnlocked
	{
		get
		{
			return _storage.Get("NumberOfIslandsUnlocked", 0);
		}
		private set
		{
			int numberOfIslandsUnlocked = NumberOfIslandsUnlocked;
			_storage.Set("NumberOfIslandsUnlocked", value);
			OnValueChanged("NumberOfIslandsUnlocked", numberOfIslandsUnlocked, NumberOfIslandsUnlocked);
		}
	}

	public int GlobalBuildingsBuiltWithGold
	{
		get
		{
			return _storage.Get("GlobalBuildingsBuiltWithGold", 0);
		}
		private set
		{
			int globalBuildingsBuiltWithGold = GlobalBuildingsBuiltWithGold;
			_storage.Set("GlobalBuildingsBuiltWithGold", value);
			OnValueChanged("GlobalBuildingsBuiltWithGold", globalBuildingsBuiltWithGold, GlobalBuildingsBuiltWithGold);
		}
	}

	public int DaysPlayedStreak
	{
		get
		{
			return _storage.Get("DaysPlayedStreak", 0);
		}
		private set
		{
			int daysPlayedStreak = DaysPlayedStreak;
			_storage.Set("DaysPlayedStreak", value);
			OnValueChanged("DaysPlayedStreak", daysPlayedStreak, DaysPlayedStreak);
		}
	}

	public int NumberOfTimesPlayed
	{
		get
		{
			return _storage.Get("NumberOfTimesPlayed", 0);
		}
		private set
		{
			int numberOfTimesPlayed = NumberOfTimesPlayed;
			_storage.Set("NumberOfTimesPlayed", value);
			OnValueChanged("NumberOfTimesPlayed", numberOfTimesPlayed, NumberOfTimesPlayed);
		}
	}

	public int NumberOfDaysPlayed
	{
		get
		{
			return _storage.Get("NumberOfDaysPlayed", 0);
		}
		private set
		{
			int numberOfDaysPlayed = NumberOfDaysPlayed;
			_storage.Set("NumberOfDaysPlayed", value);
			OnValueChanged("NumberOfDaysPlayed", numberOfDaysPlayed, NumberOfDaysPlayed);
		}
	}

	public int NumberOfDailyBonusStreaksCompleted
	{
		get
		{
			return _storage.Get("NumberOfDailyBonusStreaksCompleted", 0);
		}
		private set
		{
			int numberOfDailyBonusStreaksCompleted = NumberOfDailyBonusStreaksCompleted;
			_storage.Set("NumberOfDailyBonusStreaksCompleted", value);
			OnValueChanged("NumberOfDailyBonusStreaksCompleted", numberOfDailyBonusStreaksCompleted, NumberOfDailyBonusStreaksCompleted);
		}
	}

	public int NumberOfIAPsPurchased
	{
		get
		{
			return _storage.Get("NumberOfIAPsPurchased", 0);
		}
		private set
		{
			int numberOfIAPsPurchased = NumberOfIAPsPurchased;
			_storage.Set("NumberOfIAPsPurchased", value);
			OnValueChanged("NumberOfIAPsPurchased", numberOfIAPsPurchased, NumberOfIAPsPurchased);
		}
	}

	public int NumberOfCurrencyMenuWatched
	{
		get
		{
			return _storage.Get("NumberOfCurrencyMenuWatched", 0);
		}
		private set
		{
			int numberOfCurrencyMenuWatched = NumberOfCurrencyMenuWatched;
			_storage.Set("NumberOfCurrencyMenuWatched", value);
			OnValueChanged("NumberOfCurrencyMenuWatched", numberOfCurrencyMenuWatched, NumberOfCurrencyMenuWatched);
		}
	}

	public int TotalNumberOfRoads
	{
		get
		{
			int num = 0;
			IList<IslandId> allIslandIds = IslandExtensions.AllIslandIds;
			int i = 0;
			for (int count = allIslandIds.Count; i < count; i++)
			{
				CIGIslandStats islandStats = GetIslandStats(allIslandIds[i]);
				if (islandStats != null)
				{
					num += islandStats.RoadCount;
				}
			}
			return num;
		}
	}

	public int TotalNumberOfMaxLevelBuildings
	{
		get
		{
			int num = 0;
			IList<IslandId> allIslandIds = IslandExtensions.AllIslandIds;
			int i = 0;
			for (int count = allIslandIds.Count; i < count; i++)
			{
				CIGIslandStats islandStats = GetIslandStats(allIslandIds[i]);
				if (islandStats != null)
				{
					num += islandStats.MaxLevelBuildingCount;
				}
			}
			return num;
		}
	}

	public int TotalNumberOfDecorations
	{
		get
		{
			int num = 0;
			IList<IslandId> allIslandIds = IslandExtensions.AllIslandIds;
			int i = 0;
			for (int count = allIslandIds.Count; i < count; i++)
			{
				CIGIslandStats islandStats = GetIslandStats(allIslandIds[i]);
				if (islandStats != null)
				{
					num += islandStats.DecorationsCount;
				}
			}
			return num;
		}
	}

	public int NumberOfKeyDealsWithoutLandmarksPurchased
	{
		get
		{
			return _storage.Get("NumberOfKeyDealsWithoutLandmarksPurchased", 0);
		}
		private set
		{
			int numberOfKeyDealsWithoutLandmarksPurchased = NumberOfKeyDealsWithoutLandmarksPurchased;
			_storage.Set("NumberOfKeyDealsWithoutLandmarksPurchased", value);
			OnValueChanged("NumberOfKeyDealsWithoutLandmarksPurchased", numberOfKeyDealsWithoutLandmarksPurchased, NumberOfKeyDealsWithoutLandmarksPurchased);
		}
	}

	public int NumberOfBuildingsBuiltOnGrass
	{
		get
		{
			return _storage.Get("NumberOfBuildingsBuiltOnGrass", 0);
		}
		private set
		{
			int numberOfBuildingsBuiltOnGrass = NumberOfBuildingsBuiltOnGrass;
			_storage.Set("NumberOfBuildingsBuiltOnGrass", value);
			OnValueChanged("NumberOfBuildingsBuiltOnGrass", numberOfBuildingsBuiltOnGrass, NumberOfBuildingsBuiltOnGrass);
		}
	}

	public int NumberOfBuildingsBuiltOnBeach
	{
		get
		{
			return _storage.Get("NumberOfBuildingsBuiltOnBeach", 0);
		}
		private set
		{
			int numberOfBuildingsBuiltOnBeach = NumberOfBuildingsBuiltOnBeach;
			_storage.Set("NumberOfBuildingsBuiltOnBeach", value);
			OnValueChanged("NumberOfBuildingsBuiltOnBeach", numberOfBuildingsBuiltOnBeach, NumberOfBuildingsBuiltOnBeach);
		}
	}

	public int NumberOfBuildingsBuiltOnWater
	{
		get
		{
			return _storage.Get("NumberOfBuildingsBuiltOnWater", 0);
		}
		private set
		{
			int numberOfBuildingsBuiltOnWater = NumberOfBuildingsBuiltOnWater;
			_storage.Set("NumberOfBuildingsBuiltOnWater", value);
			OnValueChanged("NumberOfBuildingsBuiltOnWater", numberOfBuildingsBuiltOnWater, NumberOfBuildingsBuiltOnWater);
		}
	}

	public int NumberOfBuildingsBuiltOnSand
	{
		get
		{
			return _storage.Get("NumberOfBuildingsBuiltOnSand", 0);
		}
		private set
		{
			int numberOfBuildingsBuiltOnSand = NumberOfBuildingsBuiltOnSand;
			_storage.Set("NumberOfBuildingsBuiltOnSand", value);
			OnValueChanged("NumberOfBuildingsBuiltOnSand", numberOfBuildingsBuiltOnSand, NumberOfBuildingsBuiltOnSand);
		}
	}

	public int NumberOfBuildingsBuiltOnRock
	{
		get
		{
			return _storage.Get("NumberOfBuildingsBuiltOnRock", 0);
		}
		private set
		{
			int numberOfBuildingsBuiltOnRock = NumberOfBuildingsBuiltOnRock;
			_storage.Set("NumberOfBuildingsBuiltOnRock", value);
			OnValueChanged("NumberOfBuildingsBuiltOnRock", numberOfBuildingsBuiltOnRock, NumberOfBuildingsBuiltOnRock);
		}
	}

	public int NumberOfBuildingsBuiltOnMud
	{
		get
		{
			return _storage.Get("NumberOfBuildingsBuiltOnMud", 0);
		}
		private set
		{
			int numberOfBuildingsBuiltOnMud = NumberOfBuildingsBuiltOnMud;
			_storage.Set("NumberOfBuildingsBuiltOnMud", value);
			OnValueChanged("NumberOfBuildingsBuiltOnMud", numberOfBuildingsBuiltOnMud, NumberOfBuildingsBuiltOnMud);
		}
	}

	public int NumberOfBuildingsBuiltOnSnow
	{
		get
		{
			return _storage.Get("NumberOfBuildingsBuiltOnSnow", 0);
		}
		private set
		{
			int numberOfBuildingsBuiltOnSnow = NumberOfBuildingsBuiltOnSnow;
			_storage.Set("NumberOfBuildingsBuiltOnSnow", value);
			OnValueChanged("NumberOfBuildingsBuiltOnSnow", numberOfBuildingsBuiltOnSnow, NumberOfBuildingsBuiltOnSnow);
		}
	}

	public int NumberOfCommercialBuildingsBuilt
	{
		get
		{
			return _storage.Get("NumberOfCommercialBuildingsBuilt", 0);
		}
		private set
		{
			int numberOfCommercialBuildingsBuilt = NumberOfCommercialBuildingsBuilt;
			_storage.Set("NumberOfCommercialBuildingsBuilt", value);
			OnValueChanged("NumberOfCommercialBuildingsBuilt", numberOfCommercialBuildingsBuilt, NumberOfCommercialBuildingsBuilt);
		}
	}

	public int NumberOfResidentialBuildingsBuilt
	{
		get
		{
			return _storage.Get("NumberOfResidentialBuildingsBuilt", 0);
		}
		private set
		{
			int numberOfResidentialBuildingsBuilt = NumberOfResidentialBuildingsBuilt;
			_storage.Set("NumberOfResidentialBuildingsBuilt", value);
			OnValueChanged("NumberOfResidentialBuildingsBuilt", numberOfResidentialBuildingsBuilt, NumberOfResidentialBuildingsBuilt);
		}
	}

	public int NumberOfCommunityBuildingsBuilt
	{
		get
		{
			return _storage.Get("NumberOfCommunityBuildingsBuilt", 0);
		}
		private set
		{
			int numberOfCommunityBuildingsBuilt = NumberOfCommunityBuildingsBuilt;
			_storage.Set("NumberOfCommunityBuildingsBuilt", value);
			OnValueChanged("NumberOfCommunityBuildingsBuilt", numberOfCommunityBuildingsBuilt, NumberOfCommunityBuildingsBuilt);
		}
	}

	public int NumberOfDecorationBuildingsBuilt
	{
		get
		{
			return _storage.Get("NumberOfDecorationBuildingsBuilt", 0);
		}
		private set
		{
			int numberOfDecorationBuildingsBuilt = NumberOfDecorationBuildingsBuilt;
			_storage.Set("NumberOfDecorationBuildingsBuilt", value);
			OnValueChanged("NumberOfDecorationBuildingsBuilt", numberOfDecorationBuildingsBuilt, NumberOfDecorationBuildingsBuilt);
		}
	}

	public int NumberOfRoadsBuilt
	{
		get
		{
			return _storage.Get("NumberOfRoadsBuilt", 0);
		}
		private set
		{
			int numberOfRoadsBuilt = NumberOfRoadsBuilt;
			_storage.Set("NumberOfRoadsBuilt", value);
			OnValueChanged("NumberOfRoadsBuilt", numberOfRoadsBuilt, NumberOfRoadsBuilt);
		}
	}

	public decimal CashSpent
	{
		get
		{
			return _storage.Get("CashSpent", decimal.Zero);
		}
		private set
		{
			decimal cashSpent = CashSpent;
			_storage.Set("CashSpent", value);
			OnValueChanged("CashSpent", cashSpent, CashSpent);
		}
	}

	public decimal CashSpentOnCurrencyConversion
	{
		get
		{
			return _storage.Get("CashSpentOnCurrencyConversion", decimal.Zero);
		}
		private set
		{
			decimal cashSpentOnCurrencyConversion = CashSpentOnCurrencyConversion;
			_storage.Set("CashSpentOnCurrencyConversion", value);
			OnValueChanged("CashSpentOnCurrencyConversion", cashSpentOnCurrencyConversion, CashSpentOnCurrencyConversion);
		}
	}

	public int SilverKeysSpent
	{
		get
		{
			return _storage.Get("NumberOfSilverKeysSpent", 0);
		}
		private set
		{
			int silverKeysSpent = SilverKeysSpent;
			_storage.Set("NumberOfSilverKeysSpent", value);
			OnValueChanged("NumberOfSilverKeysSpent", silverKeysSpent, SilverKeysSpent);
		}
	}

	public decimal SilverKeysSpentOnCurrencyConversion
	{
		get
		{
			return _storage.Get("SilverKeysSpentOnCurrencyConversion", decimal.Zero);
		}
		private set
		{
			decimal silverKeysSpentOnCurrencyConversion = SilverKeysSpentOnCurrencyConversion;
			_storage.Set("SilverKeysSpentOnCurrencyConversion", value);
			OnValueChanged("SilverKeysSpentOnCurrencyConversion", silverKeysSpentOnCurrencyConversion, SilverKeysSpentOnCurrencyConversion);
		}
	}

	public int SilverKeysEarned
	{
		get
		{
			return _storage.Get("NumberOfSilverKeysReceived", 0);
		}
		private set
		{
			int silverKeysEarned = SilverKeysEarned;
			_storage.Set("NumberOfSilverKeysReceived", value);
			OnValueChanged("NumberOfSilverKeysReceived", silverKeysEarned, SilverKeysEarned);
		}
	}

	public decimal SilverKeysEarnedFromCurrencyConversion
	{
		get
		{
			return _storage.Get("SilverKeysEarnedFromCurrencyConversion", decimal.Zero);
		}
		private set
		{
			decimal silverKeysEarnedFromCurrencyConversion = SilverKeysEarnedFromCurrencyConversion;
			_storage.Set("SilverKeysEarnedFromCurrencyConversion", value);
			OnValueChanged("SilverKeysEarnedFromCurrencyConversion", silverKeysEarnedFromCurrencyConversion, SilverKeysEarnedFromCurrencyConversion);
		}
	}

	public int SilverKeysEarnedFromUpgrades
	{
		get
		{
			return _storage.Get("NumberOfSilverKeysReceivedFromUpgrades", 0);
		}
		private set
		{
			int silverKeysEarnedFromUpgrades = SilverKeysEarnedFromUpgrades;
			_storage.Set("NumberOfSilverKeysReceivedFromUpgrades", value);
			OnValueChanged("NumberOfSilverKeysReceivedFromUpgrades", silverKeysEarnedFromUpgrades, SilverKeysEarnedFromUpgrades);
		}
	}

	public int SilverKeysEarnedFromExpansionChest
	{
		get
		{
			return _storage.Get("SilverKeysEarnedFromExpansionChest", 0);
		}
		private set
		{
			int silverKeysEarnedFromExpansionChest = SilverKeysEarnedFromExpansionChest;
			_storage.Set("SilverKeysEarnedFromExpansionChest", value);
			OnValueChanged("SilverKeysEarnedFromExpansionChest", silverKeysEarnedFromExpansionChest, SilverKeysEarnedFromExpansionChest);
		}
	}

	public int SilverKeysEarnedFromWalkerBalloon
	{
		get
		{
			return _storage.Get("SilverKeysEarnedFromWalkerBalloon", 0);
		}
		private set
		{
			int silverKeysEarnedFromWalkerBalloon = SilverKeysEarnedFromWalkerBalloon;
			_storage.Set("SilverKeysEarnedFromWalkerBalloon", value);
			OnValueChanged("SilverKeysEarnedFromWalkerBalloon", silverKeysEarnedFromWalkerBalloon, SilverKeysEarnedFromWalkerBalloon);
		}
	}

	public int GoldKeysSpent
	{
		get
		{
			return _storage.Get("NumberOfGoldKeysSpent", 0);
		}
		private set
		{
			int goldKeysSpent = GoldKeysSpent;
			_storage.Set("NumberOfGoldKeysSpent", value);
			OnValueChanged("NumberOfGoldKeysSpent", goldKeysSpent, GoldKeysSpent);
		}
	}

	public decimal GoldKeysSpentOnCurrencyConversion
	{
		get
		{
			return _storage.Get("GoldKeysSpentOnCurrencyConversion", decimal.Zero);
		}
		private set
		{
			decimal goldKeysSpentOnCurrencyConversion = GoldKeysSpentOnCurrencyConversion;
			_storage.Set("GoldKeysSpentOnCurrencyConversion", value);
			OnValueChanged("GoldKeysSpentOnCurrencyConversion", goldKeysSpentOnCurrencyConversion, GoldKeysSpentOnCurrencyConversion);
			_storage.Set("GoldKeysSpentOnCurrencyConversion", value);
		}
	}

	public int GoldKeysEarned
	{
		get
		{
			return _storage.Get("NumberOfGoldKeysReceived", 0);
		}
		private set
		{
			int goldKeysEarned = GoldKeysEarned;
			_storage.Set("NumberOfGoldKeysReceived", value);
			OnValueChanged("NumberOfGoldKeysReceived", goldKeysEarned, GoldKeysEarned);
		}
	}

	public decimal GoldKeysEarnedFromCurrencyConversion
	{
		get
		{
			return _storage.Get("GoldKeysEarnedFromCurrencyConversion", decimal.Zero);
		}
		private set
		{
			decimal goldKeysEarnedFromCurrencyConversion = GoldKeysEarnedFromCurrencyConversion;
			_storage.Set("GoldKeysEarnedFromCurrencyConversion", value);
			OnValueChanged("GoldKeysEarnedFromCurrencyConversion", goldKeysEarnedFromCurrencyConversion, GoldKeysEarnedFromCurrencyConversion);
		}
	}

	public int GoldKeysEarnedFromUpgrades
	{
		get
		{
			return _storage.Get("NumberOfGoldKeysReceivedFromUpgrades", 0);
		}
		private set
		{
			int goldKeysEarnedFromUpgrades = GoldKeysEarnedFromUpgrades;
			_storage.Set("NumberOfGoldKeysReceivedFromUpgrades", value);
			OnValueChanged("NumberOfGoldKeysReceivedFromUpgrades", goldKeysEarnedFromUpgrades, GoldKeysEarnedFromUpgrades);
		}
	}

	public int GoldKeysEarnedFromExpansionChest
	{
		get
		{
			return _storage.Get("GoldKeysEarnedFromExpansionChest", 0);
		}
		private set
		{
			int goldKeysEarnedFromExpansionChest = GoldKeysEarnedFromExpansionChest;
			_storage.Set("GoldKeysEarnedFromExpansionChest", value);
			OnValueChanged("GoldKeysEarnedFromExpansionChest", goldKeysEarnedFromExpansionChest, GoldKeysEarnedFromExpansionChest);
		}
	}

	public decimal GoldKeysEarnedWheelOfFortune
	{
		get
		{
			return _storage.Get("GoldKeysEarned_WheelOfFortune", decimal.Zero);
		}
		private set
		{
			decimal goldKeysEarnedWheelOfFortune = GoldKeysEarnedWheelOfFortune;
			_storage.Set("GoldKeysEarned_WheelOfFortune", value);
			OnValueChanged("GoldKeysEarned_WheelOfFortune", goldKeysEarnedWheelOfFortune, GoldKeysEarnedWheelOfFortune);
		}
	}

	public long GoldSpent
	{
		get
		{
			return _storage.Get("GoldSpent", 0L);
		}
		private set
		{
			long goldSpent = GoldSpent;
			_storage.Set("GoldSpent", value);
			OnValueChanged("GoldSpent", goldSpent, GoldSpent);
		}
	}

	public int GoldSpentOnImmediateUpgrades
	{
		get
		{
			return _storage.Get("NumberIFSpedUpUpgrades", 0);
		}
		private set
		{
			int goldSpentOnImmediateUpgrades = GoldSpentOnImmediateUpgrades;
			_storage.Set("NumberIFSpedUpUpgrades", value);
			OnValueChanged("NumberIFSpedUpUpgrades", goldSpentOnImmediateUpgrades, GoldSpentOnImmediateUpgrades);
		}
	}

	public int GoldSpentSpeedups
	{
		get
		{
			return _storage.Get("GoldSpent_Speedups", 0);
		}
		private set
		{
			int goldSpentSpeedups = GoldSpentSpeedups;
			_storage.Set("GoldSpent_Speedups", value);
			OnValueChanged("GoldSpent_Speedups", goldSpentSpeedups, GoldSpentSpeedups);
		}
	}

	public int GoldSpentUpgradeSpeedups
	{
		get
		{
			return _storage.Get("GoldSpent_UpgradeSpeedups", 0);
		}
		private set
		{
			int goldSpentUpgradeSpeedups = GoldSpentUpgradeSpeedups;
			_storage.Set("GoldSpent_UpgradeSpeedups", value);
			OnValueChanged("GoldSpent_UpgradeSpeedups", goldSpentUpgradeSpeedups, GoldSpentUpgradeSpeedups);
		}
	}

	public int GoldSpentBuildSpeedups
	{
		get
		{
			return _storage.Get("GoldSpent_BuildSpeedups", 0);
		}
		private set
		{
			int goldSpentBuildSpeedups = GoldSpentBuildSpeedups;
			_storage.Set("GoldSpent_BuildSpeedups", value);
			OnValueChanged("GoldSpent_BuildSpeedups", goldSpentBuildSpeedups, GoldSpentBuildSpeedups);
		}
	}

	public int GoldSpentAirshipSpeedups
	{
		get
		{
			return _storage.Get("GoldSpent_AirshipSpeedups", 0);
		}
		private set
		{
			int goldSpentAirshipSpeedups = GoldSpentAirshipSpeedups;
			_storage.Set("GoldSpent_AirshipSpeedups", value);
			OnValueChanged("GoldSpent_AirshipSpeedups", goldSpentAirshipSpeedups, GoldSpentAirshipSpeedups);
		}
	}

	public int GoldSpentDemolishSpeedups
	{
		get
		{
			return _storage.Get("GoldSpent_DemolishSpeedups", 0);
		}
		private set
		{
			int goldSpentDemolishSpeedups = GoldSpentDemolishSpeedups;
			_storage.Set("GoldSpent_DemolishSpeedups", value);
			OnValueChanged("GoldSpent_DemolishSpeedups", goldSpentDemolishSpeedups, GoldSpentDemolishSpeedups);
		}
	}

	public int GoldSpentGoldBuildings
	{
		get
		{
			return _storage.Get("GoldSpent_GoldBuildings", 0);
		}
		private set
		{
			int goldSpentGoldBuildings = GoldSpentGoldBuildings;
			_storage.Set("GoldSpent_GoldBuildings", value);
			OnValueChanged("GoldSpent_GoldBuildings", goldSpentGoldBuildings, GoldSpentGoldBuildings);
		}
	}

	public int GoldSpentCashBuildings
	{
		get
		{
			return _storage.Get("GoldSpent_CashBuildings2", 0);
		}
		private set
		{
			int goldSpentCashBuildings = GoldSpentCashBuildings;
			_storage.Set("GoldSpent_CashBuildings2", value);
			OnValueChanged("GoldSpent_CashBuildings2", goldSpentCashBuildings, GoldSpentCashBuildings);
		}
	}

	public int GoldSpentExpansions
	{
		get
		{
			return _storage.Get("GoldSpent_Expansions", 0);
		}
		private set
		{
			int goldSpentExpansions = GoldSpentExpansions;
			_storage.Set("GoldSpent_Expansions", value);
			OnValueChanged("GoldSpent_Expansions", goldSpentExpansions, GoldSpentExpansions);
		}
	}

	public int GoldSpentCashExchange
	{
		get
		{
			return _storage.Get("GoldSpent_CashExchange", 0);
		}
		private set
		{
			int goldSpentCashExchange = GoldSpentCashExchange;
			_storage.Set("GoldSpent_CashExchange", value);
			OnValueChanged("GoldSpent_CashExchange", goldSpentCashExchange, GoldSpentCashExchange);
		}
	}

	public int GoldSpentCraneHire
	{
		get
		{
			return _storage.Get("GoldSpent_CraneHire", 0);
		}
		private set
		{
			int goldSpentCraneHire = GoldSpentCraneHire;
			_storage.Set("GoldSpent_CraneHire", value);
			OnValueChanged("GoldSpent_CraneHire", goldSpentCraneHire, GoldSpentCraneHire);
		}
	}

	public int GoldSpentCranePurchase
	{
		get
		{
			return _storage.Get("GoldSpent_CranePurchase", 0);
		}
		private set
		{
			int goldSpentCranePurchase = GoldSpentCranePurchase;
			_storage.Set("GoldSpent_CranePurchase", value);
			OnValueChanged("GoldSpent_CranePurchase", goldSpentCranePurchase, GoldSpentCranePurchase);
		}
	}

	public int GoldSpentImmediateBuild
	{
		get
		{
			return _storage.Get("GoldSpent_ImmediateBuild", 0);
		}
		private set
		{
			int goldSpentImmediateBuild = GoldSpentImmediateBuild;
			_storage.Set("GoldSpent_ImmediateBuild", value);
			OnValueChanged("GoldSpent_ImmediateBuild", goldSpentImmediateBuild, GoldSpentImmediateBuild);
		}
	}

	public decimal GoldSpentWheelOfFortune
	{
		get
		{
			return _storage.Get("GoldSpent_WheelOfFortune", decimal.Zero);
		}
		private set
		{
			decimal goldSpentWheelOfFortune = GoldSpentWheelOfFortune;
			_storage.Set("GoldSpent_WheelOfFortune", value);
			OnValueChanged("GoldSpent_WheelOfFortune", goldSpentWheelOfFortune, GoldSpentWheelOfFortune);
		}
	}

	public decimal GoldEarnedFromCurrencyConversion
	{
		get
		{
			return _storage.Get("GoldEarnedFromCurrencyConversion", decimal.Zero);
		}
		private set
		{
			decimal goldEarnedFromCurrencyConversion = GoldEarnedFromCurrencyConversion;
			_storage.Set("GoldEarnedFromCurrencyConversion", value);
			OnValueChanged("GoldEarnedFromCurrencyConversion", goldEarnedFromCurrencyConversion, GoldEarnedFromCurrencyConversion);
		}
	}

	public decimal GoldEarnedWheelOfFortune
	{
		get
		{
			return _storage.Get("GoldEarned_WheelOfFortune", decimal.Zero);
		}
		private set
		{
			decimal goldEarnedWheelOfFortune = GoldEarnedWheelOfFortune;
			_storage.Set("GoldEarned_WheelOfFortune", value);
			OnValueChanged("GoldEarned_WheelOfFortune", goldEarnedWheelOfFortune, GoldEarnedWheelOfFortune);
		}
	}

	public decimal TokensSpent
	{
		get
		{
			return _storage.Get("TokensSpent", decimal.Zero);
		}
		private set
		{
			decimal tokensSpent = TokensSpent;
			_storage.Set("TokensSpent", value);
			OnValueChanged("TokensSpent", tokensSpent, TokensSpent);
		}
	}

	public decimal TokensSpentWheelOfFortune
	{
		get
		{
			return _storage.Get("TokensSpent_WheelOfFortune", decimal.Zero);
		}
		private set
		{
			decimal tokensSpentWheelOfFortune = TokensSpentWheelOfFortune;
			_storage.Set("TokensSpent_WheelOfFortune", value);
			OnValueChanged("TokensSpent_WheelOfFortune", tokensSpentWheelOfFortune, TokensSpentWheelOfFortune);
		}
	}

	public decimal TokensEarnedFromExpansionChest
	{
		get
		{
			return _storage.Get("TokensEarnedFromExpansionChest", decimal.Zero);
		}
		private set
		{
			decimal tokensEarnedFromExpansionChest = TokensEarnedFromExpansionChest;
			_storage.Set("TokensEarnedFromExpansionChest", value);
			OnValueChanged("TokensEarnedFromExpansionChest", tokensEarnedFromExpansionChest, TokensEarnedFromExpansionChest);
		}
	}

	public decimal CurrenciesEarnedFriendIslandVisiting
	{
		get
		{
			return _storage.Get("CurrenciesEarnedFriendIslandVisiting", decimal.Zero);
		}
		private set
		{
			decimal currenciesEarnedFriendIslandVisiting = CurrenciesEarnedFriendIslandVisiting;
			_storage.Set("CurrenciesEarnedFriendIslandVisiting", value);
			OnValueChanged("CurrenciesEarnedFriendIslandVisiting", currenciesEarnedFriendIslandVisiting, CurrenciesEarnedFriendIslandVisiting);
		}
	}

	public int NumberOfWoodenChestsOpened
	{
		get
		{
			return _storage.Get("NumberOfWoodenChestsOpened", 0);
		}
		private set
		{
			int numberOfWoodenChestsOpened = NumberOfWoodenChestsOpened;
			_storage.Set("NumberOfWoodenChestsOpened", value);
			OnValueChanged("NumberOfWoodenChestsOpened", numberOfWoodenChestsOpened, NumberOfWoodenChestsOpened);
		}
	}

	public int NumberOfSilverChestsOpened
	{
		get
		{
			return _storage.Get("NumberOfSilverChestsOpened", 0);
		}
		private set
		{
			int numberOfSilverChestsOpened = NumberOfSilverChestsOpened;
			_storage.Set("NumberOfSilverChestsOpened", value);
			OnValueChanged("NumberOfSilverChestsOpened", numberOfSilverChestsOpened, NumberOfSilverChestsOpened);
		}
	}

	public int NumberOfGoldChestsOpened
	{
		get
		{
			return _storage.Get("NumberOfGoldChestsOpened", 0);
		}
		private set
		{
			int numberOfGoldChestsOpened = NumberOfGoldChestsOpened;
			_storage.Set("NumberOfGoldChestsOpened", value);
			OnValueChanged("NumberOfGoldChestsOpened", numberOfGoldChestsOpened, NumberOfGoldChestsOpened);
		}
	}

	public int NumberOfPlatinumChestsOpened
	{
		get
		{
			return _storage.Get("NumberOfPlatinumChestsOpened", 0);
		}
		private set
		{
			int numberOfPlatinumChestsOpened = NumberOfPlatinumChestsOpened;
			_storage.Set("NumberOfPlatinumChestsOpened", value);
			OnValueChanged("NumberOfPlatinumChestsOpened", numberOfPlatinumChestsOpened, NumberOfPlatinumChestsOpened);
		}
	}

	public decimal NumberOfSilverKeysSpentOnSilverChests
	{
		get
		{
			return _storage.Get("NumberOfSilverKeysSpentOnSilverChests", decimal.Zero);
		}
		private set
		{
			decimal numberOfSilverKeysSpentOnSilverChests = NumberOfSilverKeysSpentOnSilverChests;
			_storage.Set("NumberOfSilverKeysSpentOnSilverChests", value);
			OnValueChanged("NumberOfSilverKeysSpentOnSilverChests", numberOfSilverKeysSpentOnSilverChests, NumberOfSilverKeysSpentOnSilverChests);
		}
	}

	public decimal NumberOfGoldKeysSpentOnGoldChests
	{
		get
		{
			return _storage.Get("NumberOfGoldKeysSpentOnGoldChests", decimal.Zero);
		}
		private set
		{
			decimal numberOfGoldKeysSpentOnGoldChests = NumberOfGoldKeysSpentOnGoldChests;
			_storage.Set("NumberOfGoldKeysSpentOnGoldChests", value);
			OnValueChanged("NumberOfGoldKeysSpentOnGoldChests", numberOfGoldKeysSpentOnGoldChests, NumberOfGoldKeysSpentOnGoldChests);
		}
	}

	public decimal NumberOfGoldKeysSpentOnPlatinumChests
	{
		get
		{
			return _storage.Get("NumberOfGoldKeysSpentOnPlatinumChests", decimal.Zero);
		}
		private set
		{
			decimal numberOfGoldKeysSpentOnPlatinumChests = NumberOfGoldKeysSpentOnPlatinumChests;
			_storage.Set("NumberOfGoldKeysSpentOnPlatinumChests", value);
			OnValueChanged("NumberOfGoldKeysSpentOnPlatinumChests", numberOfGoldKeysSpentOnPlatinumChests, NumberOfGoldKeysSpentOnPlatinumChests);
		}
	}

	public int DailyQuestsCompleted
	{
		get
		{
			return _storage.Get("DailyQuestsCompletedToday", 0);
		}
		set
		{
			int dailyQuestsCompleted = DailyQuestsCompleted;
			_storage.Set("DailyQuestsCompletedToday", value);
			OnValueChanged("DailyQuestsCompletedToday", dailyQuestsCompleted, DailyQuestsCompleted);
		}
	}

	private int TotalMinutesSinceCleanGame => Mathf.Max(0, Mathf.FloorToInt((float)(AntiCheatDateTime.UtcNow - FirstDateTimePlayed).TotalMinutes));

	private int TotalHoursSinceCleanGame => Mathf.Max(0, Mathf.FloorToInt((float)(AntiCheatDateTime.UtcNow - FirstDateTimePlayed).TotalHours));

	private int TotalDaysSinceCleanGame => Mathf.Max(0, Mathf.FloorToInt((float)(AntiCheatDateTime.UtcNow - FirstDateTimePlayed).TotalDays));

	private StorageDictionary IslandStatsData => _storage.GetStorageDict("IslandStatsData");

	private DateTime FirstDateTimePlayed
	{
		get
		{
			return _storage.GetDateTime("FirstDateTimePlayed", AntiCheatDateTime.UtcNow);
		}
		set
		{
			_storage.Set("FirstDateTimePlayed", value.ToBinary());
		}
	}

	private DateTime LastDatePlayed
	{
		get
		{
			return DateTime.FromBinary(_storage.Get("LastDatePlayed", FirstDateTimePlayed.Date.ToBinary()));
		}
		set
		{
			_storage.Set("LastDatePlayed", value.ToBinary());
		}
	}

	public event SpeedupExecutedEventHandler SpeedupExecutedEvent;

	private void FireSpeedupExecutedEvent()
	{
		this.SpeedupExecutedEvent?.Invoke();
	}

	public GameStats(StorageDictionary storage, GameState gameState)
		: base(storage)
	{
		_gameState = gameState;
		NumberOfTimesPlayed++;
		Analytics.LogEvent("session");
		if (!storage.Contains("FirstDateTimePlayed"))
		{
			FirstDateTimePlayed = AntiCheatDateTime.UtcNow;
		}
		DateTime date = AntiCheatDateTime.UtcNow.Date;
		int days = (date - LastDatePlayed).Days;
		LastDatePlayed = date;
		if (days > 0)
		{
			NumberOfDaysPlayed++;
			Analytics.LogEvent("day_played");
		}
		switch (days)
		{
		case 1:
			AddDaysPlayedStreak(1);
			break;
		default:
			ResetDaysPlayedStreak();
			break;
		case 0:
			break;
		}
		Analytics.SessionStats(NumberOfTimesPlayed, _gameState.Level, (long)_gameState.Balance.GetValue("Gold"), NumberOfBuildings, NumberOfExpansionsPurchased);
		_gameState.CurrenciesEarnedEvent += OnCurrenciesEarned;
		_gameState.CurrenciesSpentEvent += OnCurrenciesSpent;
		_gameState.LevelUpEvent += OnLevelUp;
		_gameState.MinutePlayedEvent += OnMinutePlayed;
	}

	public void SetIslandId(IslandId islandId)
	{
		_islandId = islandId;
	}

	public void Release()
	{
		if (_subscribedToBuilder != null)
		{
			_subscribedToBuilder.BuildingBuiltEvent -= OnBuildingBuilt;
			_subscribedToBuilder = null;
		}
		if (_subscribedToRoadBuilder != null)
		{
			_subscribedToRoadBuilder.RoadBuiltEvent -= OnRoadBuilt;
			_subscribedToRoadBuilder.RoadRemovedEvent -= OnRoadRemoved;
			_subscribedToRoadBuilder.RoadsAppliedEvent -= OnRoadsApplied;
			_subscribedToRoadBuilder = null;
		}
		if (_subscribedToGrid != null)
		{
			_subscribedToGrid.GridTileRemovedEvent -= OnTileRemoved;
			_subscribedToGrid = null;
		}
	}

	public void OnNewBlockUnlocked(IsometricGrid grid, ExpansionBlock block)
	{
		for (int i = 0; i < block.Size.u; i++)
		{
			for (int j = 0; j < block.Size.v; j++)
			{
				AddGridElementUnlocked(grid[block.Origin.u + i, block.Origin.v + j].Type);
			}
		}
	}

	public void SetCurrentIsland(IsometricIsland isometricIsland)
	{
		if (_subscribedToBuilder != null)
		{
			_subscribedToBuilder.BuildingBuiltEvent -= OnBuildingBuilt;
			_subscribedToBuilder = null;
		}
		if (_subscribedToRoadBuilder != null)
		{
			_subscribedToRoadBuilder.RoadBuiltEvent -= OnRoadBuilt;
			_subscribedToRoadBuilder.RoadRemovedEvent -= OnRoadRemoved;
			_subscribedToRoadBuilder.RoadsAppliedEvent -= OnRoadsApplied;
			_subscribedToRoadBuilder = null;
		}
		if (_subscribedToGrid != null)
		{
			_subscribedToGrid.GridTileRemovedEvent -= OnTileRemoved;
			_subscribedToGrid = null;
		}
		_currentIsland = isometricIsland;
		if (_currentIsland != null)
		{
			if (GetIslandStats(_currentIsland.IslandId) == null)
			{
				IslandStatsData.Set(_currentIsland.IslandId.ToString(), new StorageDictionary());
				GetIslandStats(_currentIsland.IslandId).PopuplateStats(_currentIsland);
			}
			_subscribedToBuilder = _currentIsland.Builder;
			_subscribedToBuilder.BuildingBuiltEvent += OnBuildingBuilt;
			_subscribedToRoadBuilder = _currentIsland.RoadBuilder;
			_subscribedToRoadBuilder.RoadBuiltEvent += OnRoadBuilt;
			_subscribedToRoadBuilder.RoadRemovedEvent += OnRoadRemoved;
			_subscribedToRoadBuilder.RoadsAppliedEvent += OnRoadsApplied;
			_subscribedToGrid = _currentIsland.IsometricGrid;
			_subscribedToGrid.GridTileRemovedEvent += OnTileRemoved;
		}
	}

	public int CashCount(string objectName)
	{
		return CashCountDict.Get(objectName, 0);
	}

	public int NumberOf(string objectName)
	{
		return NumberOfDict.Get(objectName, 0);
	}

	public int GetIslandScore()
	{
		return NumberOfIslandsUnlocked * 500 + GetUniqueBuildingTypeCount() * 150 + TotalNumberOfMaxLevelBuildings * 50 + TotalNumberOfDecorations * 25 + TotalNumberOfRoads * 5;
	}

	public int GetSurfaceTypeElementsUnlocked(SurfaceType surfaceType)
	{
		return UnlockedSurfaceTypeCounts.Get(surfaceType.ToString(), 0);
	}

	public void AddNumberOfExpansionsPurchased(int delta)
	{
		if (NumberOfExpansionsPurchased == 0 && delta > 0)
		{
			Analytics.FirstExpansionBought(NumberOfTimesPlayed);
		}
		NumberOfExpansionsPurchased += delta;
	}

	public void AddNumberOfExpansionsPurchasedWithGold(int delta)
	{
		int numberOfExpansionsPurchasedWithGold = NumberOfExpansionsPurchasedWithGold;
		NumberOfExpansionsPurchasedWithGold += delta;
		Analytics.ExpansionBoughtWithGold(NumberOfExpansionsPurchased, numberOfExpansionsPurchasedWithGold, NumberOfExpansionsPurchasedWithGold);
	}

	public void AddNumberOfExpansionsPurchasedWithCash(int delta)
	{
		NumberOfExpansionsPurchasedWithCash += delta;
		Analytics.LogEvent("purchase_expansion_with_cash");
	}

	public void AddNumberOfMaxLevelBuildings(int delta)
	{
		CIGIslandStats islandStats = GetIslandStats(_islandId);
		if (islandStats != null)
		{
			islandStats.MaxLevelBuildingCount += delta;
		}
		else
		{
			UnityEngine.Debug.LogError("islandStats is null can't add max level building to MaxLevelBuildingCount");
		}
	}

	public void AddNumberOfUpgrades(int delta)
	{
		NumberOfUpgrades += delta;
	}

	public void AddNumberOfLevel10Upgrades(int delta)
	{
		NumberOfLevel10Upgrades += delta;
	}

	public void AddGlobalBuildingsBuilt(int delta)
	{
		GlobalBuildingsBuilt += delta;
	}

	public void AddNumberOfVideosWatched(int delta, VideoSource source)
	{
		int numberOfVideosWatched = NumberOfVideosWatched;
		NumberOfVideosWatched += delta;
		Analytics.VideoWatched(numberOfVideosWatched, NumberOfVideosWatched, source);
	}

	public void AddNumberOfVideosClicked(int delta, VideoSource source)
	{
		int numberOfVideosClicked = NumberOfVideosClicked;
		NumberOfVideosClicked += delta;
		Analytics.VideoClicked(numberOfVideosClicked, NumberOfVideosClicked, source);
	}

	public void AddNumberOfInterstitialsWatched(int delta)
	{
		int numberOfInterstitialsWatched = NumberOfInterstitialsWatched;
		NumberOfInterstitialsWatched += delta;
		Analytics.LogEvent("interstitial_watched");
		Analytics.LogEventsWhenPassingThreshold(Analytics.ConversionEventInterstitialWatched, numberOfInterstitialsWatched, NumberOfInterstitialsWatched);
	}

	public void AddNumberOfInterstitialsClicked(int delta)
	{
		int numberOfInterstitialsClicked = NumberOfInterstitialsClicked;
		NumberOfInterstitialsClicked += delta;
		Analytics.LogEvent("interstitial_clicked");
		Analytics.LogEventsWhenPassingThreshold(Analytics.ConversionEventInterstitialClicked, numberOfInterstitialsClicked, NumberOfInterstitialsClicked);
	}

	public void AddProfitCollected(Currencies delta)
	{
		decimal value = delta.GetValue("Cash");
		if (value > decimal.Zero)
		{
			CashCollected += value;
			Analytics.LogEvent("cash_profit_collected");
		}
	}

	public void AddTimesCollected()
	{
		TimesCollected++;
	}

	public void AddIslandUnlocked()
	{
		NumberOfIslandsUnlocked++;
	}

	public void ResetDaysPlayedStreak()
	{
		DaysPlayedStreak = 0;
	}

	public void AddNumberOfDailyBonusStreaksCompleted(int delta)
	{
		int numberOfDailyBonusStreaksCompleted = NumberOfDailyBonusStreaksCompleted;
		NumberOfDailyBonusStreaksCompleted += delta;
		Analytics.LogEvent("daily_bonus_streak_completed");
		Analytics.LogEventsWhenPassingThreshold(Analytics.ConversionEventDailyBonusStreakCompleted, numberOfDailyBonusStreaksCompleted, NumberOfDailyBonusStreaksCompleted);
	}

	public void AddNumberOfIAPsPurchased(int delta)
	{
		int numberOfIAPsPurchased = NumberOfIAPsPurchased;
		NumberOfIAPsPurchased += delta;
		int numberOfIAPsPurchased2 = NumberOfIAPsPurchased;
		switch (NumberOfTimesPlayed)
		{
		case 1:
			Analytics.LogEventsWhenPassingThreshold(Analytics.ConversionEventSession1IAPPurchased, numberOfIAPsPurchased, numberOfIAPsPurchased2);
			break;
		case 2:
			Analytics.LogEventsWhenPassingThreshold(Analytics.ConversionEventSession2IAPPurchased, numberOfIAPsPurchased, numberOfIAPsPurchased2);
			break;
		case 3:
			Analytics.LogEventsWhenPassingThreshold(Analytics.ConversionEventSession3IAPPurchased, numberOfIAPsPurchased, numberOfIAPsPurchased2);
			break;
		}
	}

	public void IAPViewed(string iapIdentifier)
	{
		Analytics.IAPViewed(iapIdentifier);
	}

	public void CurrencyMenuWatched(int delta)
	{
		int numberOfCurrencyMenuWatched = NumberOfCurrencyMenuWatched;
		NumberOfCurrencyMenuWatched += delta;
		int numberOfCurrencyMenuWatched2 = NumberOfCurrencyMenuWatched;
		Analytics.CurrencyMenuWatched(TotalMinutesSinceCleanGame, TotalDaysSinceCleanGame, NumberOfTimesPlayed, numberOfCurrencyMenuWatched, numberOfCurrencyMenuWatched2);
	}

	public void AddKeyDealWithoutLandmarkPurchased()
	{
		NumberOfKeyDealsWithoutLandmarksPurchased++;
	}

	public void AddBuildingPurchased(BuildingProperties buildingProperties, Currencies spent)
	{
		if (spent.GetValue("Gold") > decimal.Zero && buildingProperties.BaseConstructionCost.IsMatchingName("Gold"))
		{
			AddGoldBuilding();
		}
		Analytics.BuildingPurchased(buildingProperties.BaseKey, buildingProperties.Type, buildingProperties.SurfaceType.ToString(), spent);
	}

	public void AddBuildingUpgradeStarted(BuildingProperties buildingProperties, int level, Currencies spent)
	{
		Analytics.BuildingUpgradeStarted(buildingProperties.BaseKey, level, spent);
	}

	public void AddCurrencyConversion(Currencies fromCurrencies, Currencies toCurrencies)
	{
		Currency currency = fromCurrencies.GetCurrency(0);
		Currency currency2 = toCurrencies.GetCurrency(0);
		AddCurrenciesSpentCurrencyConversion(currency);
		AddCurrenciesEarnedCurrencyConversion(currency2);
		Analytics.CurrencyConversionStarted(currency.Name, (long)currency.Value, currency2.Name, (long)currency2.Value);
	}

	public void AddWoodenTreasureChestOpened(bool watchedVideo)
	{
		Analytics.WoodenTreasureChestOpen(TreasureChestType.Wooden.ToString(), watchedVideo);
		NumberOfWoodenChestsOpened++;
	}

	public void AddTreasureChestPurchased(TreasureChestType type, Currencies cost)
	{
		string text = null;
		if (cost.ContainsPositive("GoldKey"))
		{
			text = "GoldKey";
		}
		else if (cost.ContainsPositive("SilverKey"))
		{
			text = "SilverKey";
		}
		decimal value = cost.GetValue(text);
		switch (type)
		{
		case TreasureChestType.Silver:
			NumberOfSilverChestsOpened++;
			NumberOfSilverKeysSpentOnSilverChests += value;
			break;
		case TreasureChestType.Gold:
			NumberOfGoldChestsOpened++;
			NumberOfGoldKeysSpentOnGoldChests += value;
			break;
		case TreasureChestType.Platinum:
			NumberOfPlatinumChestsOpened++;
			NumberOfGoldKeysSpentOnPlatinumChests += value;
			break;
		}
		if (text == null)
		{
			Analytics.TreasureChestPurchasedWithoutKeys(type.ToString());
		}
		else
		{
			Analytics.TreasureChestPurchasedWithKeys(type.ToString(), text, (long)value);
		}
	}

	public void QuestCompleted(string questName, Currencies reward)
	{
		Analytics.QuestCompleted(questName, reward, TotalHoursSinceCleanGame);
	}

	public void IslandPurchased(string islandName, Currency spent)
	{
		Analytics.IslandPurchased(islandName, spent, NumberOfIslandsUnlocked, TotalHoursSinceCleanGame);
	}

	private CIGIslandStats GetCurrentIslandStats()
	{
		return GetIslandStats(_islandId);
	}

	private int GetUniqueBuildingTypeCount()
	{
		int num = 0;
		foreach (string key in NumberOfDict.InternalDictionary.Keys)
		{
			if (NumberOf(key) > 0)
			{
				num++;
			}
		}
		return num;
	}

	private void AddBuilding(string objectName)
	{
		NumberOfDict.Set(objectName, 1 + NumberOfDict.Get(objectName, 0));
	}

	private void AddGoldBuilding()
	{
		GlobalBuildingsBuiltWithGold++;
	}

	private void AddCashBuilding(string objectName)
	{
		CashCountDict.Set(objectName, 1 + CashCountDict.Get(objectName, 0));
	}

	private void RemoveBuilding(string objectName)
	{
		NumberOfDict.Set(objectName, NumberOfDict.Get(objectName, 0) - 1);
	}

	private void RemoveCashBuilding(string objectName)
	{
		CashCountDict.Set(objectName, Mathf.Max(0, CashCountDict.Get(objectName, 0) - 1));
	}

	private CIGIslandStats GetIslandStats(IslandId islandId)
	{
		if (_cachedIslandStats.ContainsKey(islandId))
		{
			return _cachedIslandStats[islandId];
		}
		string key = islandId.ToString();
		StorageDictionary islandStatsData = IslandStatsData;
		if (islandStatsData.Contains(key))
		{
			_cachedIslandStats[islandId] = new CIGIslandStats(islandStatsData.GetStorageDict(key));
			return _cachedIslandStats[islandId];
		}
		return null;
	}

	private void AddSpeedupExecuted()
	{
		int speedupsExecuted = SpeedupsExecuted;
		SpeedupsExecuted++;
		Analytics.LogEvent("speedup");
		Analytics.LogEventsWhenPassingThreshold(Analytics.ConversionEventSpeedup, speedupsExecuted, SpeedupsExecuted);
		FireSpeedupExecutedEvent();
	}

	private void AddGoldSpent(decimal spent)
	{
		long goldSpent = GoldSpent;
		GoldSpent += (int)spent;
		long goldSpent2 = GoldSpent;
		switch (NumberOfTimesPlayed)
		{
		case 1:
			Analytics.LogEventsWhenPassingThreshold(Analytics.ConversionEventSession1SpendGold, goldSpent, goldSpent2);
			break;
		case 2:
			Analytics.LogEventsWhenPassingThreshold(Analytics.ConversionEventSession2SpendGold, goldSpent, goldSpent2);
			break;
		}
		Analytics.LogEventsWhenPassingThreshold(Analytics.ConversionEventSpendGold, goldSpent, goldSpent2);
	}

	private void AddCurrenciesSpentCurrencyConversion(Currency currency)
	{
		string name = currency.Name;
		if (!(name == "Cash"))
		{
			if (!(name == "SilverKey"))
			{
				if (name == "GoldKey")
				{
					GoldKeysSpentOnCurrencyConversion += currency.Value;
				}
			}
			else
			{
				SilverKeysSpentOnCurrencyConversion += currency.Value;
			}
		}
		else
		{
			CashSpentOnCurrencyConversion += currency.Value;
		}
	}

	private void AddCurrenciesEarnedCurrencyConversion(Currency currency)
	{
		string name = currency.Name;
		if (!(name == "SilverKey"))
		{
			if (!(name == "GoldKey"))
			{
				if (name == "Gold")
				{
					GoldEarnedFromCurrencyConversion += currency.Value;
				}
			}
			else
			{
				GoldKeysEarnedFromCurrencyConversion += currency.Value;
			}
		}
		else
		{
			SilverKeysEarnedFromCurrencyConversion += currency.Value;
		}
	}

	private void AddGridElementUnlocked(SurfaceType surfaceType)
	{
		if (surfaceType != 0)
		{
			NumberOfUnlockedElements++;
			string key = surfaceType.ToString();
			int num = UnlockedSurfaceTypeCounts.Get(key, 0);
			UnlockedSurfaceTypeCounts.Set(key, num + 1);
		}
	}

	private void AddCurrenciesSpentSpeedup(Currencies currencies)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			Currency currency = currencies.GetCurrency(i);
			string name = currency.Name;
			if (name == "Gold")
			{
				GoldSpentSpeedups += (int)currency.Value;
				Analytics.SpeedupWithGold(NumberOfTimesPlayed);
			}
		}
	}

	private void AddCurrenciesSpentUpgradeSpeedup(Currencies currencies)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			Currency currency = currencies.GetCurrency(i);
			string name = currency.Name;
			if (name == "Gold")
			{
				GoldSpentUpgradeSpeedups += (int)currency.Value;
			}
		}
	}

	private void AddCurrenciesSpentBuildSpeedup(Currencies currencies)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			Currency currency = currencies.GetCurrency(i);
			string name = currency.Name;
			if (name == "Gold")
			{
				GoldSpentBuildSpeedups += (int)currency.Value;
			}
		}
	}

	private void AddCurrenciesSpentAirshipSpeedup(Currencies currencies)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			Currency currency = currencies.GetCurrency(i);
			string name = currency.Name;
			if (name == "Gold")
			{
				GoldSpentAirshipSpeedups += (int)currency.Value;
			}
		}
	}

	private void AddCurrenciesSpentDemolishSpeedup(Currencies currencies)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			Currency currency = currencies.GetCurrency(i);
			string name = currency.Name;
			if (name == "Gold")
			{
				GoldSpentDemolishSpeedups += (int)currency.Value;
			}
		}
	}

	private void AddCurrenciesSpentGoldBuildings(Currencies currencies)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			Currency currency = currencies.GetCurrency(i);
			string name = currency.Name;
			if (name == "Gold")
			{
				GoldSpentGoldBuildings += (int)currency.Value;
				Analytics.GoldBuildingPurchased(NumberOfTimesPlayed, TotalHoursSinceCleanGame);
			}
		}
	}

	private void AddCurrenciesSpentCashBuildings(Currencies currencies)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			Currency currency = currencies.GetCurrency(i);
			string name = currency.Name;
			if (name == "Gold")
			{
				decimal value = GoldSpentCashBuildings;
				GoldSpentCashBuildings += (int)currency.Value;
				Analytics.SupplementBuildingPurchased(TotalHoursSinceCleanGame);
				Analytics.LogEventsWhenPassingThreshold(Analytics.ConversionEventSupplementalBuildingPurchased, (long)value, GoldSpentCashBuildings);
			}
		}
	}

	private void AddCurrenciesSpentExpansions(Currencies currencies)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			Currency currency = currencies.GetCurrency(i);
			string name = currency.Name;
			if (name == "Gold")
			{
				GoldSpentExpansions += (int)currency.Value;
			}
		}
	}

	private void AddCurrenciesSpentCashExchange(Currencies currencies)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			Currency currency = currencies.GetCurrency(i);
			string name = currency.Name;
			if (name == "Gold")
			{
				GoldSpentCashExchange += (int)currency.Value;
			}
		}
	}

	private void AddCurrenciesSpentCraneHire(Currencies currencies)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			Currency currency = currencies.GetCurrency(i);
			string name = currency.Name;
			if (name == "Gold")
			{
				GoldSpentCraneHire += (int)currency.Value;
			}
		}
	}

	private void AddCurrenciesSpentBuildingConstructionInstant(Currencies currencies)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			Currency currency = currencies.GetCurrency(i);
			string name = currency.Name;
			if (name == "Gold")
			{
				GoldSpentImmediateBuild += (int)currency.Value;
				Analytics.BuildingImmediateBuild();
			}
		}
	}

	private void AddCurrenciesSpentBuildingUpgradeInstant(Currencies currencies)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			Currency currency = currencies.GetCurrency(i);
			string name = currency.Name;
			if (name == "Gold")
			{
				GoldSpentOnImmediateUpgrades += (int)currency.Value;
			}
		}
	}

	private void AddCurrenciesSpentCurrencyConversion(Currencies currencies)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			Currency currency = currencies.GetCurrency(i);
			switch (currency.Name)
			{
			case "Cash":
				CashSpentOnCurrencyConversion += currency.Value;
				break;
			case "SilverKey":
				SilverKeysSpentOnCurrencyConversion += currency.Value;
				break;
			case "GoldKey":
				GoldKeysSpentOnCurrencyConversion += currency.Value;
				break;
			}
		}
	}

	private void AddCurrenciesEarnedCurrencyConversion(Currencies currencies)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			Currency currency = currencies.GetCurrency(i);
			switch (currency.Name)
			{
			case "SilverKey":
				SilverKeysEarnedFromCurrencyConversion += currency.Value;
				break;
			case "GoldKey":
				GoldKeysEarnedFromCurrencyConversion += currency.Value;
				break;
			case "Gold":
				GoldEarnedFromCurrencyConversion += currency.Value;
				break;
			}
		}
	}

	private void AddCurrenciesEarnedWalkerBalloon(Currencies currencies)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			Currency currency = currencies.GetCurrency(i);
			string name = currency.Name;
			if (name == "SilverKey")
			{
				SilverKeysEarnedFromWalkerBalloon += (int)currency.Value;
			}
		}
	}

	private void AddCurrenciesEarnedBuildingUpgrade(Currencies currencies)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			Currency currency = currencies.GetCurrency(i);
			string name = currency.Name;
			if (!(name == "SilverKey"))
			{
				if (name == "GoldKey")
				{
					GoldKeysEarnedFromUpgrades += (int)currency.Value;
				}
			}
			else
			{
				SilverKeysEarnedFromUpgrades += (int)currency.Value;
			}
		}
	}

	private void AddCurrenciesEarnedBuildingCollect(Currencies currencies)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			Currency currency = currencies.GetCurrency(i);
			string name = currency.Name;
			if (name == "Cash")
			{
				CashCollected += currency.Value;
				Analytics.LogEvent("cash_profit_collected");
			}
		}
	}

	private void AddCurrenciesEarnedExpansionChest(Currencies currencies)
	{
		if (currencies.KeyCount > 0)
		{
			int i = 0;
			for (int keyCount = currencies.KeyCount; i < keyCount; i++)
			{
				Currency currency = currencies.GetCurrency(i);
				switch (currency.Name)
				{
				case "SilverKey":
					SilverKeysEarnedFromExpansionChest += (int)currency.Value;
					break;
				case "GoldKey":
					GoldKeysEarnedFromExpansionChest += (int)currency.Value;
					break;
				case "Token":
					TokensEarnedFromExpansionChest += currency.Value;
					break;
				}
				Analytics.WheelOfFortuneEarned(currency.Name, (long)currency.Value);
			}
		}
		else
		{
			Analytics.WheelOfFortuneEarned("empty", 0L);
		}
	}

	private void AddCurrenciesSpentWheelOfFortune(Currencies currencies)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			Currency currency = currencies.GetCurrency(i);
			string name = currency.Name;
			if (!(name == "Gold"))
			{
				if (name == "Token")
				{
					TokensSpentWheelOfFortune += currency.Value;
				}
			}
			else
			{
				GoldSpentWheelOfFortune += currency.Value;
			}
			Analytics.WheelOfFortuneSpent(currency.Name, (long)currency.Value);
		}
	}

	private void AddCurrenciesEarnedWheelOfFortune(Currencies currencies)
	{
		if (currencies.KeyCount == 0)
		{
			Analytics.WheelOfFortuneEarned("empty", 0L);
			return;
		}
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			Currency currency = currencies.GetCurrency(i);
			string name = currency.Name;
			if (!(name == "Gold"))
			{
				if (name == "GoldKey")
				{
					GoldKeysEarnedWheelOfFortune += currency.Value;
				}
			}
			else
			{
				GoldEarnedWheelOfFortune += currency.Value;
			}
			Analytics.WheelOfFortuneEarned(currency.Name, (long)currency.Value);
		}
	}

	private void AddCurrenciesEarnedFriendIslandVisiting(Currencies currencies)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			CurrenciesEarnedFriendIslandVisiting += currencies.GetCurrency(i).Value;
		}
	}

	private void AddDaysPlayedStreak(int days)
	{
		DaysPlayedStreak += days;
		Analytics.DaysPlayedStreak(DaysPlayedStreak);
	}

	private void OnRoadBuilt(Road road)
	{
		int num = road.IsNormalRoad ? 1 : 2;
		NumberOfRoadsBuilt += num;
		GetCurrentIslandStats().RoadCount += num;
	}

	private void OnRoadRemoved(Road road)
	{
		int num = road.IsNormalRoad ? 1 : 2;
		NumberOfRoadsBuilt -= num;
		GetCurrentIslandStats().RoadCount -= num;
	}

	private void OnRoadsApplied(int added, int removed)
	{
		Analytics.RoadsChanged(added, removed, NumberOfRoadsBuilt);
	}

	private void OnBuildingBuilt(GridTile tile, bool newBuilding)
	{
		if (IsometricIsland.Current.Expansions.GetBlockForIndex(tile.Index).Unlocked)
		{
			NumberOfUsedElements += tile.Properties.Size.u * tile.Properties.Size.v;
		}
		CIGBuilding cIGBuilding = tile as CIGBuilding;
		if (!(cIGBuilding != null) || cIGBuilding.BuildingProperties.Activatable)
		{
			return;
		}
		string baseKey = cIGBuilding.BuildingProperties.BaseKey;
		AddBuilding(baseKey);
		if (cIGBuilding.WasBuiltWithCash)
		{
			AddCashBuilding(baseKey);
		}
		NumberOfBuildings++;
		CIGDecoration x = cIGBuilding as CIGDecoration;
		if (x != null)
		{
			GetCurrentIslandStats().DecorationsCount++;
		}
		if (newBuilding)
		{
			if (cIGBuilding is CIGCommercialBuilding)
			{
				NumberOfCommercialBuildingsBuilt++;
			}
			else if (cIGBuilding is CIGResidentialBuilding)
			{
				NumberOfResidentialBuildingsBuilt++;
			}
			else if (cIGBuilding is CIGCommunityBuilding)
			{
				NumberOfCommunityBuildingsBuilt++;
			}
			else if (x != null)
			{
				NumberOfDecorationBuildingsBuilt++;
			}
			switch (tile.Element.Type)
			{
			case SurfaceType.Grass:
				NumberOfBuildingsBuiltOnGrass++;
				break;
			case SurfaceType.Beach:
				NumberOfBuildingsBuiltOnBeach++;
				break;
			case SurfaceType.Water:
				NumberOfBuildingsBuiltOnWater++;
				break;
			case SurfaceType.Sand:
				NumberOfBuildingsBuiltOnSand++;
				break;
			case SurfaceType.Rock:
				NumberOfBuildingsBuiltOnRock++;
				break;
			case SurfaceType.Mud:
				NumberOfBuildingsBuiltOnMud++;
				break;
			case SurfaceType.Snow:
				NumberOfBuildingsBuiltOnSnow++;
				break;
			}
		}
	}

	private void OnTileRemoved(GridTile tile)
	{
		NumberOfUsedElements -= tile.Properties.Size.u * tile.Properties.Size.v;
		CIGBuilding cIGBuilding = tile as CIGBuilding;
		if (cIGBuilding != null)
		{
			string baseKey = cIGBuilding.BuildingProperties.BaseKey;
			RemoveBuilding(baseKey);
			if (cIGBuilding.WasBuiltWithCash)
			{
				RemoveCashBuilding(baseKey);
			}
			NumberOfBuildings--;
			if (cIGBuilding.CurrentLevel == cIGBuilding.BuildingProperties.MaximumLevel && cIGBuilding.BuildingProperties.MaximumLevel > 1)
			{
				GetCurrentIslandStats().MaxLevelBuildingCount--;
			}
			if (cIGBuilding as CIGDecoration != null)
			{
				GetCurrentIslandStats().DecorationsCount--;
			}
		}
	}

	private void OnCurrenciesEarned(Currencies currencies, CurrenciesEarnedReason earnedReason)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			Currency currency = currencies.GetCurrency(i);
			if (!Analytics.EarnItems.TryGetValue(earnedReason, out string value))
			{
				value = Analytics.EarnItems[CurrenciesEarnedReason.Unknown];
				UnityEngine.Debug.LogError("Unknown CurrenciesEarnedReason. Will send 'Unknown' to firebase!");
			}
			long currentTotal = (long)_gameState.Balance.GetValue(currency.Name);
			Analytics.EarnVirtualCurrency(currency.Name, value, (long)currency.Value, currentTotal);
		}
		int j = 0;
		for (int keyCount2 = currencies.KeyCount; j < keyCount2; j++)
		{
			Currency currency2 = currencies.GetCurrency(j);
			string name = currency2.Name;
			if (!(name == "SilverKey"))
			{
				if (name == "GoldKey")
				{
					GoldKeysEarned += (int)currency2.Value;
				}
			}
			else
			{
				SilverKeysEarned += (int)currency2.Value;
			}
		}
		switch (earnedReason)
		{
		case CurrenciesEarnedReason.LevelUp:
		case CurrenciesEarnedReason.BuildingConstruction:
		case CurrenciesEarnedReason.DailyReward:
		case CurrenciesEarnedReason.CheatMenu:
		case CurrenciesEarnedReason.Quest:
		case CurrenciesEarnedReason.StarterDeal:
		case CurrenciesEarnedReason.Reward:
		case CurrenciesEarnedReason.Fishing:
		case CurrenciesEarnedReason.FriendGift:
		case CurrenciesEarnedReason.TutorialGift:
		case CurrenciesEarnedReason.FlyingStartDeal:
			break;
		case CurrenciesEarnedReason.CurrencyConversion:
			AddCurrenciesEarnedCurrencyConversion(currencies);
			break;
		case CurrenciesEarnedReason.WalkerBalloon:
			AddCurrenciesEarnedWalkerBalloon(currencies);
			break;
		case CurrenciesEarnedReason.BuildingUpgrade:
			AddCurrenciesEarnedBuildingUpgrade(currencies);
			break;
		case CurrenciesEarnedReason.BuildingCollect:
			AddCurrenciesEarnedBuildingCollect(currencies);
			break;
		case CurrenciesEarnedReason.ExpansionChest:
			AddCurrenciesEarnedExpansionChest(currencies);
			break;
		case CurrenciesEarnedReason.WheelOfFortune:
			AddCurrenciesEarnedWheelOfFortune(currencies);
			break;
		case CurrenciesEarnedReason.IAP:
			if (currencies.Contains("Gold"))
			{
				NumberOfGoldPackagesBought++;
			}
			break;
		case CurrenciesEarnedReason.FriendIslandVisiting:
			AddCurrenciesEarnedFriendIslandVisiting(currencies);
			break;
		default:
			UnityEngine.Debug.LogWarning(string.Format("{0} '{1}' is not being tracked", "CurrenciesEarnedReason", earnedReason));
			break;
		}
	}

	private void OnCurrenciesSpent(Currencies currencies, CurrenciesSpentReason spentReason)
	{
		int i = 0;
		for (int keyCount = currencies.KeyCount; i < keyCount; i++)
		{
			Currency currency = currencies.GetCurrency(i);
			if (!Analytics.SpendItems.TryGetValue(spentReason, out string value))
			{
				value = Analytics.SpendItems[CurrenciesSpentReason.Unknown];
				UnityEngine.Debug.LogError("Unknown CurrenciesSpentReason. Will send 'Unknown' to firebase!");
			}
			Analytics.SpendVirtualCurrency(currency.Name, value, (long)currency.Value);
		}
		int j = 0;
		for (int keyCount2 = currencies.KeyCount; j < keyCount2; j++)
		{
			Currency currency2 = currencies.GetCurrency(j);
			switch (currency2.Name)
			{
			case "Cash":
				CashSpent += currency2.Value;
				break;
			case "Gold":
				AddGoldSpent(currency2.Value);
				break;
			case "SilverKey":
				SilverKeysSpent += (int)currency2.Value;
				break;
			case "GoldKey":
				GoldKeysSpent += (int)currency2.Value;
				break;
			case "Token":
				TokensSpent += currency2.Value;
				break;
			}
		}
		switch (spentReason)
		{
		case CurrenciesSpentReason.BuildingUpgrade:
		case CurrenciesSpentReason.IslandUnlock:
		case CurrenciesSpentReason.BuildingWarehouse:
		case CurrenciesSpentReason.KeyDeals:
		case CurrenciesSpentReason.OneTimeOffer:
		case CurrenciesSpentReason.TreasureChest:
		case CurrenciesSpentReason.SpecialQuest:
		case CurrenciesSpentReason.AirshipSend:
		case CurrenciesSpentReason.CraneOffer:
		case CurrenciesSpentReason.CheatMenu:
			break;
		case CurrenciesSpentReason.CurrencyConversion:
			AddCurrenciesSpentCurrencyConversion(currencies);
			break;
		case CurrenciesSpentReason.SpeedupUpgrade:
			AddCurrenciesSpentUpgradeSpeedup(currencies);
			AddCurrenciesSpentSpeedup(currencies);
			AddSpeedupExecuted();
			break;
		case CurrenciesSpentReason.SpeedupBuild:
			AddCurrenciesSpentBuildSpeedup(currencies);
			AddCurrenciesSpentSpeedup(currencies);
			AddSpeedupExecuted();
			break;
		case CurrenciesSpentReason.SpeedupAirship:
			AddCurrenciesSpentAirshipSpeedup(currencies);
			AddCurrenciesSpentSpeedup(currencies);
			AddSpeedupExecuted();
			break;
		case CurrenciesSpentReason.SpeedupDemolish:
			AddCurrenciesSpentDemolishSpeedup(currencies);
			AddCurrenciesSpentSpeedup(currencies);
			AddSpeedupExecuted();
			break;
		case CurrenciesSpentReason.GoldBuilding:
			AddCurrenciesSpentGoldBuildings(currencies);
			break;
		case CurrenciesSpentReason.GoldBuildingInstant:
			AddCurrenciesSpentGoldBuildings(currencies);
			AddCurrenciesSpentBuildingConstructionInstant(currencies);
			break;
		case CurrenciesSpentReason.CashBuilding:
			AddCurrenciesSpentCashBuildings(currencies);
			break;
		case CurrenciesSpentReason.CashBuildingInstant:
			AddCurrenciesSpentCashBuildings(currencies);
			AddCurrenciesSpentBuildingConstructionInstant(currencies);
			break;
		case CurrenciesSpentReason.Expansion:
			AddCurrenciesSpentExpansions(currencies);
			break;
		case CurrenciesSpentReason.CashExchange:
			AddCurrenciesSpentCashExchange(currencies);
			break;
		case CurrenciesSpentReason.CraneHire:
			AddCurrenciesSpentCraneHire(currencies);
			break;
		case CurrenciesSpentReason.BuildingUpgradeInstant:
			AddCurrenciesSpentBuildingUpgradeInstant(currencies);
			break;
		case CurrenciesSpentReason.WheelOfFortune:
			AddCurrenciesSpentWheelOfFortune(currencies);
			break;
		default:
			UnityEngine.Debug.LogWarning(string.Format("{0} '{1}' is not being tracked", "CurrenciesSpentReason", spentReason));
			break;
		}
	}

	private void OnMinutePlayed(long oldMinutesPlayed, long newMinutesPlayed)
	{
		long num = Analytics.SessionLengthsInMinutes.Find((long x) => x > oldMinutesPlayed);
		if (num != 0L && newMinutesPlayed >= num)
		{
			Analytics.SessionLength(num, TotalHoursSinceCleanGame);
		}
		switch (NumberOfTimesPlayed)
		{
		case 1:
			Analytics.LogEventsWhenPassingThreshold(Analytics.ConversionEventSession1SessionLength, oldMinutesPlayed, newMinutesPlayed);
			break;
		case 2:
			Analytics.LogEventsWhenPassingThreshold(Analytics.ConversionEventSession2SessionLength, oldMinutesPlayed, newMinutesPlayed);
			break;
		}
	}

	private void OnLevelUp(int oldLevel, int newLevel)
	{
		Analytics.LevelUp(newLevel);
		Analytics.LevelStats(newLevel, _gameState.GlobalPopulation, _gameState.GlobalHappiness, _gameState.GlobalJobs, _gameState.TotalMinutesPlayed);
		switch (NumberOfTimesPlayed)
		{
		case 1:
			Analytics.LogEventsWhenPassingThreshold(Analytics.ConversionEventSession1Level, oldLevel, newLevel);
			break;
		case 2:
			Analytics.LogEventsWhenPassingThreshold(Analytics.ConversionEventSession2Level, oldLevel, newLevel);
			break;
		}
	}
}
