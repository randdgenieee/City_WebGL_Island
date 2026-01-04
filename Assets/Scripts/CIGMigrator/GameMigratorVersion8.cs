using System.Collections.Generic;

namespace CIGMigrator
{
	public class GameMigratorVersion8 : IMigrator
	{
		private const string GridTileKey = "GridTile";

		private const string LandmarkStorageKeyIdentifier = "CurrentBoostTiles";

		private const string LevelKey = "level";

		private const string IsometricGridStorageKey = "IsometricGrid";

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
					foreach (KeyValuePair<string, object> item in dictionary2)
					{
						object obj;
						object value4;
						Dictionary<string, object> dictionary4;
						Dictionary<string, object> dictionary3;
						if ((dictionary3 = (item.Value as Dictionary<string, object>)) != null && dictionary3.TryGetValue("GridTile", out object value3) && (dictionary4 = (value3 as Dictionary<string, object>)) != null && dictionary4.ContainsKey("CurrentBoostTiles") && dictionary4.TryGetValue("level", out value4) && (obj = value4) is int)
						{
							int num2 = (int)obj;
							dictionary4["level"] = num2 * 2;
						}
					}
				}
			}
		}
	}
}
