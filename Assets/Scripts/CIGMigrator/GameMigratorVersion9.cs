using System.Collections.Generic;

namespace CIGMigrator
{
	public class GameMigratorVersion9 : IMigrator
	{
		private const string IsometricGridStorageKey = "IsometricGrid";

		private const string PrefabNameKey = "PrefabName";

		private const string WaterBuoyOldName = "waterbuoy";

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
				Dictionary<string, object> dictionary2;
				object value2;
				Dictionary<string, object> dictionary;
				if (storageRoot.TryGetValue(IslandKeys[i], out object value) && (dictionary = (value as Dictionary<string, object>)) != null && dictionary.TryGetValue("IsometricGrid", out value2) && (dictionary2 = (value2 as Dictionary<string, object>)) != null)
				{
					List<string> list = new List<string>();
					foreach (KeyValuePair<string, object> item in dictionary2)
					{
						string text;
						Dictionary<string, object> dictionary3;
						if ((dictionary3 = (item.Value as Dictionary<string, object>)) != null && dictionary3.TryGetValue("PrefabName", out object value3) && (text = (value3 as string)) != null && text.Equals("waterbuoy"))
						{
							list.Add(item.Key);
						}
					}
					int j = 0;
					for (int count = list.Count; j < count; j++)
					{
						dictionary2.Remove(list[j]);
					}
				}
			}
		}
	}
}
