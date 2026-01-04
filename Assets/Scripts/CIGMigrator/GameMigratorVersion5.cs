using SparkLinq;
using System.Collections.Generic;

namespace CIGMigrator
{
	public class GameMigratorVersion5 : IMigrator
	{
		private const string TreasureChestStorageKey = "TreasureChestManager";

		private const string TreasureChestUnconsumedRewardsKey = "UnconsumedRewards";

		private const string LevelUpsKey = "LevelUps";

		private const string CranesKey = "Cranes";

		private const string CashBuildingsKey = "CashBuildings";

		private const string GoldBuildingsKey = "GoldBuildings";

		private const string LandmarkBuildingKey = "LandmarkBuilding";

		private const string SilverKeysKey = "SilverKeys";

		private const string GoldKeysKey = "GoldKeys";

		private const string CashKey = "Cash";

		private const string GoldKey = "Gold";

		private const string XPKey = "XP";

		private const string RewardKey = "Reward";

		private const string CurrenciesKey = "Currencies";

		private const string BuildingsKey = "Buildings";

		private const string CashCurrency = "Cash";

		private const string GoldCurrency = "Gold";

		private const string XPCurrency = "XP";

		private const string LevelUpCurrency = "LevelUp";

		private const string CraneCurrency = "Crane";

		private const string SilverKeyCurrency = "SilverKey";

		private const string GoldKeyCurrency = "GoldKey";

		public void Migrate(Dictionary<string, object> storageRoot)
		{
			object value2;
			if (!storageRoot.TryGetValue("TreasureChestManager", out object value) || !(value is Dictionary<string, object>) || !((Dictionary<string, object>)value).TryGetValue("UnconsumedRewards", out value2) || !(value2 is List<object>))
			{
				return;
			}
			List<object> list = (List<object>)value2;
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				Dictionary<string, object> dictionary = list[i] as Dictionary<string, object>;
				object value11;
				object value10;
				object value9;
				object value8;
				object value7;
				object value6;
				object value5;
				object value4;
				if (dictionary != null && dictionary.TryGetValue("LevelUps", out object value3) && value3 is int && dictionary.TryGetValue("Cranes", out value4) && value4 is int && dictionary.TryGetValue("CashBuildings", out value5) && value5 is List<object> && dictionary.TryGetValue("GoldBuildings", out value6) && value6 is List<object> && dictionary.TryGetValue("SilverKeys", out value7) && value7 is int && dictionary.TryGetValue("GoldKeys", out value8) && value8 is int && dictionary.TryGetValue("Cash", out value9) && value9 is int && dictionary.TryGetValue("Gold", out value10) && value10 is int && dictionary.TryGetValue("XP", out value11) && value11 is int)
				{
					dictionary.Remove("LevelUps");
					dictionary.Remove("Cranes");
					dictionary.Remove("CashBuildings");
					dictionary.Remove("GoldBuildings");
					dictionary.Remove("SilverKeys");
					dictionary.Remove("GoldKeys");
					dictionary.Remove("Cash");
					dictionary.Remove("Gold");
					dictionary.Remove("XP");
					Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
					dictionary2.Add("Crane", (decimal)(int)value4);
					dictionary2.Add("SilverKey", (decimal)(int)value7);
					dictionary2.Add("GoldKey", (decimal)(int)value8);
					dictionary2.Add("Cash", (decimal)(int)value9);
					dictionary2.Add("Gold", (decimal)(int)value10);
					dictionary2.Add("XP", (decimal)(int)value11);
					dictionary2.Add("LevelUp", (decimal)(int)value3);
					List<object> list2 = new List<object>();
					list2.Concat((List<object>)value5);
					list2.Concat((List<object>)value6);
					if (dictionary.TryGetValue("LandmarkBuilding", out object value12) && value12 is string)
					{
						list2.Add((string)value12);
					}
					Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
					dictionary3.Add("Currencies", dictionary2);
					dictionary3.Add("Buildings", list2);
					dictionary.Add("Reward", dictionary3);
				}
			}
		}
	}
}
