using System;
using System.Collections.Generic;

namespace CIGMigrator
{
	public class GameMigratorVersion11 : IMigrator
	{
		private const string IsometricGridStorageKey = "IsometricGrid";

		private const string GridTileIndexKey = "Index";

		private const string PrefabNameKey = "PrefabName";

		private const string BuySignName = "BuySign";

		private const string ExpansionsStorageKey = "Expansions";

		private const string BuySignIndexKey = "BuySignIndex";

		private const string UKey = "U";

		private const string VKey = "V";

		private static readonly string[] IslandKeys = new string[9]
		{
			"Island01",
			"Island02",
			"Island03",
			"Island04",
			"Island05",
			"Island06",
			"Island07",
			"Island08",
			"Island09"
		};

		public void Migrate(Dictionary<string, object> storageRoot)
		{
			int i = 0;
			for (int num = IslandKeys.Length; i < num; i++)
			{
				Dictionary<string, object> expansionsStorage;
				object value3;
				Dictionary<string, object> dictionary;
				Dictionary<string, object> dictionary2;
				object value2;
				if (storageRoot.TryGetValue(IslandKeys[i], out object value) && (dictionary = (value as Dictionary<string, object>)) != null && dictionary.TryGetValue("IsometricGrid", out value2) && (dictionary2 = (value2 as Dictionary<string, object>)) != null && dictionary.TryGetValue("Expansions", out value3) && (expansionsStorage = (value3 as Dictionary<string, object>)) != null)
				{
					HashSet<Tuple<int, int>> linkedBuySignIndices = GetLinkedBuySignIndices(expansionsStorage);
					List<string> buySignKeysToRemove = GetBuySignKeysToRemove(dictionary2, linkedBuySignIndices);
					int j = 0;
					for (int count = buySignKeysToRemove.Count; j < count; j++)
					{
						dictionary2.Remove(buySignKeysToRemove[j]);
					}
				}
			}
		}

		private HashSet<Tuple<int, int>> GetLinkedBuySignIndices(Dictionary<string, object> expansionsStorage)
		{
			HashSet<Tuple<int, int>> hashSet = new HashSet<Tuple<int, int>>();
			foreach (KeyValuePair<string, object> item in expansionsStorage)
			{
				Dictionary<string, object> storage;
				Dictionary<string, object> dictionary;
				if ((dictionary = (item.Value as Dictionary<string, object>)) != null && dictionary.TryGetValue("BuySignIndex", out object value) && (storage = (value as Dictionary<string, object>)) != null && TryParseGridIndex(storage, out Tuple<int, int> gridIndex))
				{
					hashSet.Add(gridIndex);
				}
			}
			return hashSet;
		}

		private List<string> GetBuySignKeysToRemove(Dictionary<string, object> isometricGridStorage, HashSet<Tuple<int, int>> linkedBuySignIndices)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, object> item in isometricGridStorage)
			{
				Tuple<int, int> gridIndex;
				Dictionary<string, object> storage;
				object value2;
				Dictionary<string, object> dictionary;
				string text;
				if ((dictionary = (item.Value as Dictionary<string, object>)) != null && dictionary.TryGetValue("PrefabName", out object value) && (text = (value as string)) != null && text.Equals("BuySign") && dictionary.TryGetValue("Index", out value2) && (storage = (value2 as Dictionary<string, object>)) != null && TryParseGridIndex(storage, out gridIndex) && !linkedBuySignIndices.Contains(gridIndex))
				{
					list.Add(item.Key);
				}
			}
			return list;
		}

		private static bool TryParseGridIndex(Dictionary<string, object> storage, out Tuple<int, int> gridIndex)
		{
			object obj;
			if (storage.TryGetValue("U", out object value) && (obj = value) is int)
			{
				int item = (int)obj;
				if (storage.TryGetValue("V", out object value2) && (obj = value2) is int)
				{
					int item2 = (int)obj;
					gridIndex = new Tuple<int, int>(item, item2);
					return true;
				}
			}
			gridIndex = new Tuple<int, int>(-1, -1);
			return false;
		}
	}
}
