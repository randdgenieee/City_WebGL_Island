using CIG.Translation;
using SparkLinq;
using System;
using System.Collections.Generic;

namespace CIG
{
	[BalanceHiddenProperty("gridPositionX", typeof(int), RequiredKey = false)]
	[BalanceHiddenProperty("gridPositionY", typeof(int), RequiredKey = false)]
	[BalanceHiddenProperty("constructionInstantGold", typeof(decimal), RequiredKey = false)]
	[BalanceHiddenProperty("spriteIdentifier", typeof(string))]
	[BalanceEquallySizedProperties(new string[]
	{
		"upgradeCash",
		"upgradeInstantGold",
		"rewardSilverKeys",
		"rewardGoldKeys"
	})]
	[BalancePairedProperties(new string[]
	{
		"upgradeCash",
		"upgradeInstantGold",
		"rewardSilverKeys",
		"rewardGoldKeys"
	})]
	[BalanceSortedArrayProperties("upgradeCash", true)]
	[BalanceSortedArrayProperties("upgradeInstantGold", true)]
	[BalanceSortedArrayProperties("rewardSilverKeys", true)]
	[BalanceSortedArrayProperties("rewardGoldKeys", true)]
	[BalanceSortedArrayProperties("upgradeDurationSeconds", true)]
	[BalanceSortedArrayProperties("happiness", true)]
	[BalanceSortedArrayProperties("unlocks", true)]
	public abstract class BuildingProperties : GridTileProperties
	{
		private const string ConstructionCostKey = "constructionCost";

		private const string ConstructionInstantGoldKey = "constructionInstantGold";

		private const string ConstructionDurationSecondsKey = "constructionDurationSeconds";

		private const string DemolishDurationSecondsKey = "demolishDurationSeconds";

		private const string ConstructionRewardKey = "constructionXP";

		private const string ConstructionCranesKey = "constructionCranes";

		private const string GoldCostAtMaxKey = "goldCostAtMax";

		private const string CheckForRoadKey = "checkForRoad";

		private const string MovableKey = "movable";

		private const string DestructibleKey = "destructible";

		private const string StringReferenceKey = "stringReference";

		private const string UpgradeCashCostsKey = "upgradeCash";

		private const string UpgradeGoldCostsKey = "upgradeInstantGold";

		private const string UpgradeCashRewardsKey = "rewardSilverKeys";

		private const string UpgradeGoldRewardsKey = "rewardGoldKeys";

		private const string UpgradeDurationsSecondsKey = "upgradeDurationSeconds";

		private const string UpgradeCranesKey = "upgradeCranes";

		private const string GridPositionXKey = "gridPositionX";

		private const string GridPositionYKey = "gridPositionY";

		private const string HappinessPerLevelKey = "happiness";

		private const string SilverKeysConstructionCostKey = "constructionCostKeysSilver";

		private const string GoldKeysConstructionCostKey = "constructionCostKeysGold";

		private const string UnlockLevelsKey = "unlocks";

		private const decimal ConstructionCostGoldPenalty = 2m;

		private readonly decimal _constructionInstantGoldCost;

		[BalanceProperty("constructionCost", RequiredKey = false)]
		public Currency BaseConstructionCost
		{
			get;
		}

		[BalanceProperty("constructionDurationSeconds")]
		public int ConstructionDurationSeconds
		{
			get;
		}

		[BalanceProperty("constructionXP", ParseType = typeof(decimal), RequiredKey = false)]
		public Currency ConstructionReward
		{
			get;
		}

		[BalanceProperty("constructionCranes", RequiredKey = false)]
		public int ConstructionCranes
		{
			get;
		}

		[BalanceProperty("goldCostAtMax", RequiredKey = false)]
		public int GoldCostAtMax
		{
			get;
		}

		[BalanceProperty("demolishDurationSeconds")]
		public int DemolishDurationSeconds
		{
			get;
		}

		[BalanceProperty("checkForRoad", RequiredKey = false)]
		public bool CheckForRoad
		{
			get;
		}

		[BalanceProperty("movable")]
		public bool Movable
		{
			get;
		}

		[BalanceProperty("destructible")]
		public bool Destructible
		{
			get;
		}

		[BalanceProperty("stringReference")]
		public string StringReference
		{
			get;
		}

		[BalanceProperty("upgradeCash", ParseType = typeof(List<decimal>), RequiredKey = false)]
		public List<Currency> UpgradeCashCosts
		{
			get;
		}

		[BalanceProperty("upgradeInstantGold", ParseType = typeof(List<decimal>), RequiredKey = false)]
		public List<Currency> UpgradeGoldCosts
		{
			get;
		}

		[BalanceProperty("rewardSilverKeys", ParseType = typeof(List<decimal>), RequiredKey = false)]
		public List<Currency> UpgradeCashRewards
		{
			get;
		}

		[BalanceProperty("rewardGoldKeys", ParseType = typeof(List<decimal>), RequiredKey = false)]
		public List<Currency> UpgradeGoldRewards
		{
			get;
		}

		[BalanceProperty("upgradeDurationSeconds", RequiredKey = false)]
		public List<int> UpgradeDurationsSeconds
		{
			get;
		}

		public int MaximumLevel
		{
			get;
		}

		[BalanceProperty("upgradeCranes", RequiredKey = false)]
		public int UpgradeCranes
		{
			get;
		}

		public bool Activatable
		{
			get;
		}

		public GridIndex GridPosition
		{
			get;
		}

		[BalanceProperty("happiness", RequiredKey = false)]
		public List<int> HappinessPerLevel
		{
			get;
		}

		[BalanceProperty("unlocks", RequiredKey = false)]
		public List<int> UnlockLevels
		{
			get;
		}

		[BalanceProperty("constructionCostKeysSilver", RequiredKey = false)]
		public decimal SilverKeysConstructionCost
		{
			get;
		}

		[BalanceProperty("constructionCostKeysGold", RequiredKey = false)]
		public decimal GoldKeysConstructionCost
		{
			get;
		}

		public abstract ILocalizedString CategoryName
		{
			get;
		}

		public ILocalizedString LocalizedName => Localization.Key(StringReference);

		public bool IsGoldBuilding => BaseConstructionCost.IsMatchingName("Gold");

		protected BuildingProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			ConstructionReward = Currency.XPCurrency(GetProperty("constructionXP", decimal.Zero, optional: true));
			BaseConstructionCost = GetProperty("constructionCost", Currency.Invalid, optional: true);
			_constructionInstantGoldCost = GetProperty("constructionInstantGold", decimal.Zero, optional: true);
			ConstructionDurationSeconds = GetProperty("constructionDurationSeconds", 0);
			DemolishDurationSeconds = GetProperty("demolishDurationSeconds", 0);
			ConstructionCranes = GetProperty("constructionCranes", 1, optional: true);
			GoldCostAtMax = GetProperty("goldCostAtMax", 0, optional: true);
			CheckForRoad = (GetProperty("checkForRoad", defaultValue: true, optional: true) && base.SurfaceType.IsLand());
			Movable = GetProperty("movable", defaultValue: true);
			Destructible = GetProperty("destructible", defaultValue: true);
			StringReference = GetProperty("stringReference", "unknown");
			UpgradeCashCosts = ListExtensions.ConvertAll(GetProperty("upgradeCash", new List<decimal>(), optional: true), Currency.CashCurrency);
			UpgradeGoldCosts = ListExtensions.ConvertAll(GetProperty("upgradeInstantGold", new List<decimal>(), optional: true), Currency.GoldCurrency);
			UpgradeCashRewards = ListExtensions.ConvertAll(GetProperty("rewardSilverKeys", new List<decimal>(), optional: true), Currency.SilverKeyCurrency);
			UpgradeGoldRewards = ListExtensions.ConvertAll(GetProperty("rewardGoldKeys", new List<decimal>(), optional: true), Currency.GoldKeyCurrency);
			UpgradeDurationsSeconds = GetProperty("upgradeDurationSeconds", new List<int>(), optional: true);
			MaximumLevel = UpgradeCashCosts.Count;
			UpgradeCranes = GetProperty("upgradeCranes", 1, optional: true);
			Activatable = (HasProperty("gridPositionX") && HasProperty("gridPositionY"));
			GridPosition = new GridIndex(GetProperty("gridPositionX", -1, optional: true), GetProperty("gridPositionY", -1, optional: true));
			HappinessPerLevel = GetProperty("happiness", new List<int>(), optional: true);
			UnlockLevels = GetProperty("unlocks", new List<int>(), optional: true);
			SilverKeysConstructionCost = GetProperty("constructionCostKeysSilver", decimal.Zero, optional: true);
			GoldKeysConstructionCost = GetProperty("constructionCostKeysGold", decimal.Zero, optional: true);
		}

		public bool IsUnlocked(int level)
		{
			if (UnlockLevels.Count != 0)
			{
				return UnlockLevels[0] <= level;
			}
			return true;
		}

		public int GetNextUnlockLevel(int level)
		{
			if (!IsUnlocked(level))
			{
				return UnlockLevels[0];
			}
			int i = 0;
			for (int count = UnlockLevels.Count; i < count; i++)
			{
				int num = UnlockLevels[i];
				if (num > level)
				{
					return num;
				}
			}
			return int.MaxValue;
		}

		public virtual bool CanBeBuilt(int level, GameStats gameStats, BuildingWarehouseManager buildingWarehouseManager)
		{
			if (BaseConstructionCost.IsMatchingName("Gold"))
			{
				return true;
			}
			int num = gameStats.NumberOf(base.BaseKey) + buildingWarehouseManager.GetBuildingCount(base.BaseKey);
			int totalUnlockedCount = GetTotalUnlockedCount(level);
			return num < totalUnlockedCount;
		}

		public Currency GetConstructionCost(GameState gameState, GameStats gameStats, BuildingWarehouseManager buildingWarehouseManager)
		{
			Currency currency = BaseConstructionCost;
			if (UnlockLevels.Count == 0 || !currency.IsValid)
			{
				return currency;
			}
			int num = gameStats.CashCount(base.BaseKey) + buildingWarehouseManager.GetCashBuildingCount(base.BaseKey);
			int val = gameStats.NumberOf(base.BaseKey) + buildingWarehouseManager.GetBuildingCount(base.BaseKey) - num;
			if (!currency.IsMatchingName("Gold"))
			{
				int num2 = UnlockLevels.Count((int l) => l <= gameState.Level);
				if (num >= num2)
				{
					currency = new Currency("Gold", GoldCostAtMax);
				}
			}
			if (currency.IsMatchingName("Gold"))
			{
				decimal amount = 2m * (decimal)Math.Max(0, val);
				currency += amount;
			}
			return currency;
		}

		public Currency GetInstantConstructionCost(GameState gameState, GameStats gameStats, BuildingWarehouseManager buildingWarehouseManager, Multipliers multipliers)
		{
			Currency constructionCost = GetConstructionCost(gameState, gameStats, buildingWarehouseManager);
			decimal d = constructionCost.IsMatchingName("Gold") ? constructionCost.Value : decimal.Zero;
			decimal speedupCostGoldForSeconds = GoldCostUtility.GetSpeedupCostGoldForSeconds(multipliers, ConstructionDurationSeconds);
			decimal value = Math.Max(d + speedupCostGoldForSeconds, _constructionInstantGoldCost);
			return new Currency("Gold", value);
		}

		public virtual List<BuildingProperty> GetShownProperties(bool preview)
		{
			List<BuildingProperty> list = new List<BuildingProperty>();
			if (preview && ConstructionReward.IsMatchingName("XP"))
			{
				list.Add(BuildingProperty.ConstructionXp);
			}
			return list;
		}

		private int GetTotalUnlockedCount(int level)
		{
			if (UnlockLevels.Count == 0)
			{
				return 1;
			}
			int num = 0;
			int i = 0;
			for (int count = UnlockLevels.Count; i < count && UnlockLevels[i] <= level; i++)
			{
				num++;
			}
			return num;
		}
	}
}
