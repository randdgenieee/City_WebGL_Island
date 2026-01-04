using CIG;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIGMigrator
{
	public class GameMigratorVersion2 : IMigrator
	{
		private const string GameStatsStorageKey = "GameStats";

		private const string ExpansionsStorageKey = "Expansions";

		private const string UnlockedSurfaceTypeCountsKey = "UnlockedSurfaceTypeCounts";

		private const string UnlockedKey = "Unlocked";

		private const string NumberOfUnlockedElementsKey = "NumberOfUnlockedElements";

		private const string NumberOfUnlockedLandElementsKey = "NumberOfUnlockedLandElements";

		private const int BlockSizeU = 8;

		private const int BlockSizeV = 8;

		private static readonly GridSize BlockSize = new GridSize(8, 8);

		public void Migrate(Dictionary<string, object> storageRoot)
		{
			GameMigratorVersion2Parameters gameMigratorVersion2Parameters = Resources.Load<GameMigratorVersion2Parameters>("Migrators/GameMigratorVersion2Parameters");
			if (gameMigratorVersion2Parameters == null)
			{
				throw new Exception("No parameters ScriptableObject was found for GameMigratorVersion2, cannot proceed.");
			}
			if (!storageRoot.TryGetValue("GameStats", out object value) || !(value is Dictionary<string, object>))
			{
				return;
			}
			Dictionary<string, object> dictionary = (Dictionary<string, object>)value;
			if (dictionary.ContainsKey("UnlockedSurfaceTypeCounts"))
			{
				return;
			}
			int num = 0;
			Dictionary<SurfaceType, int> dictionary2 = new Dictionary<SurfaceType, int>();
			int i = 0;
			for (int num2 = gameMigratorVersion2Parameters.IslandSetups.Length; i < num2; i++)
			{
				IslandSetup islandSetup = gameMigratorVersion2Parameters.IslandSetups[i];
				object value3;
				if (!storageRoot.TryGetValue(islandSetup.IslandId.ToString(), out object value2) || !(value2 is Dictionary<string, object>) || !((Dictionary<string, object>)value2).TryGetValue("Expansions", out value3) || !(value3 is Dictionary<string, object>))
				{
					continue;
				}
				Dictionary<string, object> dictionary3 = (Dictionary<string, object>)value3;
				int j = 0;
				for (int count = islandSetup.Expansions.Count; j < count; j++)
				{
					IslandSetup.Expansion expansion = islandSetup.Expansions[j];
					object value5;
					if (!dictionary3.TryGetValue($"({expansion.Index.u},{expansion.Index.v})", out object value4) || !(value4 is Dictionary<string, object>) || !((Dictionary<string, object>)value4).TryGetValue("Unlocked", out value5) || !(value5 is bool) || !(bool)value5)
					{
						continue;
					}
					GridIndex index = expansion.Index;
					index.u *= 8;
					index.v *= 8;
					for (int k = index.u; k < index.u + BlockSize.u; k++)
					{
						for (int l = index.v; l < index.v + BlockSize.v; l++)
						{
							SurfaceType surfaceType = islandSetup[k, l];
							if (surfaceType != 0)
							{
								if (dictionary2.ContainsKey(surfaceType))
								{
									Dictionary<SurfaceType, int> dictionary4 = dictionary2;
									SurfaceType key = surfaceType;
									dictionary4[key]++;
								}
								else
								{
									dictionary2.Add(surfaceType, 1);
								}
								num++;
							}
						}
					}
				}
			}
			dictionary.Add("UnlockedSurfaceTypeCounts", ConvertToStorage(dictionary2));
			int unlockedElementsValue = GetUnlockedElementsValue(dictionary, "NumberOfUnlockedElements");
			if (num > unlockedElementsValue)
			{
				dictionary["NumberOfUnlockedElements"] = num;
			}
			dictionary.Remove("NumberOfUnlockedLandElements");
		}

		private int GetUnlockedElementsValue(Dictionary<string, object> storage, string key)
		{
			if (storage.TryGetValue(key, out object value) && value is int)
			{
				return (int)value;
			}
			return 0;
		}

		private Dictionary<string, object> ConvertToStorage(Dictionary<SurfaceType, int> dict)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (KeyValuePair<SurfaceType, int> item in dict)
			{
				dictionary.Add(item.Key.ToString(), item.Value);
			}
			return dictionary;
		}
	}
}
