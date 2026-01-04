using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace CIG
{
	[BalancePropertyClass("treasureChest", false)]
	[BalanceHiddenProperty("contains", typeof(List<string>))]
	[BalanceHiddenProperty("containsChance", typeof(List<string>))]
	[BalanceHiddenProperty("guaranteedCurrencies", typeof(Currencies))]
	[BalanceHiddenProperty("guaranteedBuildings", typeof(List<string>))]
	[BalanceListCountProperties("containsChance", true)]
	[BalanceListCountProperties("guaranteedBuildings", true)]
	public class TreasureChestProperties : BaseProperties
	{
		private const string CostKey = "cost";

		private const string ValueKey = "value";

		private const string IAPGameProductNameKey = "iap";

		private const string MaxBuildingsKey = "maxBuildings";

		private const string ContainsKey = "contains";

		private const string ContainsChanceKey = "containsChance";

		private const string GuaranteedCurrenciesKey = "guaranteedCurrencies";

		private const string GuaranteedBuildingsKey = "guaranteedBuildings";

		private const string VisibleItemsKey = "visibleItems";

		public TreasureChestType TreasureChestType
		{
			get;
		}

		[BalanceProperty("cost", RequiredKey = false)]
		public Currencies Cost
		{
			get;
		}

		[BalanceProperty("value")]
		public Currencies Value
		{
			get;
		}

		[BalanceProperty("visibleItems", RequiredKey = false)]
		public List<string> VisibleItems
		{
			get;
		}

		[BalanceProperty("iap", RequiredKey = false)]
		public string IAPGameProductName
		{
			get;
		}

		[BalanceProperty("maxBuildings")]
		public int MaxBuildings
		{
			get;
		}

		public float ChanceLevelUp
		{
			get;
		}

		public float ChanceCrane
		{
			get;
		}

		public float ChanceLandmark
		{
			get;
		}

		public bool ContainsBuildingsGold
		{
			get;
		}

		public bool ContainsBuildingsCash
		{
			get;
		}

		public bool ContainsCash
		{
			get;
		}

		public bool ContainsGold
		{
			get;
		}

		public bool ContainsSilverKeys
		{
			get;
		}

		public bool ContainsGoldKeys
		{
			get;
		}

		public bool ContainsXP
		{
			get;
		}

		public bool ContainsTokens
		{
			get;
		}

		public Currencies GuaranteedCurrencies
		{
			get;
		}

		public int GuaranteedGoldBuildings
		{
			get;
		}

		public int GuaranteedCashBuildings
		{
			get;
		}

		public int GuaranteedLandmarks
		{
			get;
		}

		public int ContainsCurrenciesCount
		{
			get;
		}

		public TreasureChestProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			if (EnumExtensions.TryParseEnum(baseKey, out TreasureChestType parsedEnum))
			{
				TreasureChestType = parsedEnum;
			}
			else
			{
				UnityEngine.Debug.LogWarningFormat("Cannot parse '{2}.{0}' to {1}", baseKey, "TreasureChestType", base.BaseKey);
			}
			Cost = GetProperty("cost", new Currencies());
			Value = GetProperty("value", new Currencies());
			IAPGameProductName = GetProperty("iap", "unknown", optional: true);
			MaxBuildings = GetProperty("maxBuildings", 0);
			List<string> property = GetProperty("contains", new List<string>());
			ContainsCurrenciesCount = property.Count;
			ContainsCash = property.Contains("Cash");
			ContainsGold = property.Contains("Gold");
			ContainsSilverKeys = property.Contains("SilverKey");
			ContainsGoldKeys = property.Contains("GoldKey");
			ContainsXP = property.Contains("XP");
			ContainsTokens = property.Contains("Token");
			ContainsBuildingsGold = property.Contains("BuildingGold");
			ContainsBuildingsCash = property.Contains("BuildingCash");
			Dictionary<string, float> dict = BaseProperties.ParseKeyValue(GetProperty("containsChance", new List<string>()), delegate(string input, out float output)
			{
				return float.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out output);
			});
			ChanceLevelUp = ContainsChance(dict, "LevelUp");
			ChanceCrane = ContainsChance(dict, "Crane");
			ChanceLandmark = ContainsChance(dict, "Landmark");
			GuaranteedCurrencies = GetProperty("guaranteedCurrencies", new Currencies());
			Dictionary<string, int> dict2 = BaseProperties.ParseKeyValue(GetProperty("guaranteedBuildings", new List<string>()), delegate(string input, out int output)
			{
				return int.TryParse(input, out output);
			});
			GuaranteedGoldBuildings = ContainsGuaranteed(dict2, "BuildingGold");
			GuaranteedCashBuildings = ContainsGuaranteed(dict2, "BuildingCash");
			GuaranteedLandmarks = ContainsGuaranteed(dict2, "Landmark");
			VisibleItems = GetProperty("visibleItems", new List<string>(), optional: true);
		}

		private static float ContainsChance(Dictionary<string, float> dict, string name)
		{
			if (!dict.TryGetValue(name, out float value))
			{
				return 0f;
			}
			return value;
		}

		private static int ContainsGuaranteed(Dictionary<string, int> dict, string name)
		{
			if (!dict.TryGetValue(name, out int value))
			{
				return 0;
			}
			return value;
		}
	}
}
