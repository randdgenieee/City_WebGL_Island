using System;
using System.Collections.Generic;

namespace CIGMigrator
{
	public class GameMigratorVersion10 : IMigrator
	{
		private const string IsometricGridStorageKey = "IsometricGrid";

		private const string GridTileKey = "GridTile";

		private const string LandmarkStorageKeyIdentifier = "CurrentBoostTiles";

		private const string PrefabNameKey = "PrefabName";

		private const string UniqueIdentifierKey = "UniqueIdentifier";

		private const string BuildingBoostedPercentageKey = "BoostedPercentage";

		private const string LandmarkCurrentBoostPercentageKey = "CurrentBoostPercentage";

		private const string LandmarkBoostedBuildingsIdentifiersKey = "BoostedBuildingsIDs";

		private const string LevelKey = "level";

		private const string MaxPeopleKey = "maxPeople";

		private const string MaxEmployeesKey = "maxEmployees";

		private const string ProfitKey = "profit";

		private const string HappinessKey = "happiness";

		private const string CIGIslandStateKey = "IslandState";

		private const string CIGIslandStateHappinessKey = "Happiness";

		private const string CIGIslandStateHousingKey = "Housing";

		private const string CIGIslandStateJobsKey = "Jobs";

		private const string GameStateKey = "GameState";

		private const string GameStateGlobalHappinessKey = "GlobalHappiness";

		private const string GameStateGlobalHousingKey = "GlobalHousing";

		private const string GameStateGlobalJobsKey = "GlobalJobs";

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

		private static readonly Dictionary<string, int[]> LandmarksBoostPercentages = new Dictionary<string, int[]>
		{
			["famousNetherlands"] = new int[11]
			{
				15,
				20,
				25,
				30,
				35,
				40,
				50,
				55,
				60,
				65,
				70
			},
			["famousGermany"] = new int[11]
			{
				20,
				25,
				30,
				35,
				40,
				45,
				55,
				60,
				65,
				70,
				75
			},
			["famousBrazil"] = new int[11]
			{
				25,
				30,
				35,
				40,
				45,
				50,
				60,
				65,
				70,
				75,
				80
			},
			["famousIndonesia"] = new int[11]
			{
				30,
				35,
				40,
				45,
				50,
				55,
				65,
				70,
				75,
				80,
				85
			},
			["famousJapan"] = new int[11]
			{
				35,
				40,
				45,
				50,
				55,
				60,
				70,
				75,
				80,
				85,
				90
			},
			["famousItaly"] = new int[11]
			{
				40,
				45,
				50,
				55,
				60,
				65,
				75,
				80,
				85,
				90,
				95
			},
			["famousBritain"] = new int[11]
			{
				15,
				20,
				25,
				30,
				35,
				40,
				50,
				55,
				60,
				65,
				70
			},
			["famousUSA"] = new int[11]
			{
				20,
				25,
				30,
				35,
				40,
				45,
				55,
				60,
				65,
				70,
				75
			},
			["famousChina"] = new int[11]
			{
				25,
				30,
				35,
				40,
				45,
				50,
				60,
				65,
				70,
				75,
				80
			},
			["famousSouthKorea"] = new int[11]
			{
				30,
				35,
				40,
				45,
				50,
				55,
				65,
				70,
				75,
				80,
				85
			},
			["famousRussia"] = new int[11]
			{
				35,
				40,
				45,
				50,
				55,
				60,
				70,
				75,
				80,
				85,
				90
			},
			["famousPhillipines"] = new int[11]
			{
				40,
				45,
				50,
				55,
				60,
				65,
				75,
				80,
				85,
				90,
				95
			},
			["famousSpain"] = new int[11]
			{
				15,
				20,
				25,
				30,
				35,
				40,
				50,
				55,
				60,
				65,
				70
			},
			["famousFrance"] = new int[11]
			{
				20,
				25,
				30,
				35,
				40,
				45,
				55,
				60,
				65,
				70,
				75
			},
			["famousIndia"] = new int[11]
			{
				25,
				30,
				35,
				40,
				45,
				50,
				60,
				65,
				70,
				75,
				80
			},
			["famousTurkey"] = new int[11]
			{
				30,
				35,
				40,
				45,
				50,
				55,
				65,
				70,
				75,
				80,
				85
			},
			["famousMexico"] = new int[11]
			{
				35,
				40,
				45,
				50,
				55,
				60,
				70,
				75,
				80,
				85,
				90
			},
			["famousThailand"] = new int[11]
			{
				40,
				45,
				50,
				55,
				60,
				65,
				75,
				80,
				85,
				90,
				95
			},
			["famousEgypt"] = new int[11]
			{
				15,
				20,
				25,
				30,
				35,
				40,
				50,
				55,
				60,
				65,
				70
			},
			["famousSweden"] = new int[11]
			{
				20,
				25,
				30,
				35,
				40,
				45,
				55,
				60,
				65,
				70,
				75
			}
		};

		public void Migrate(Dictionary<string, object> storageRoot)
		{
			Dictionary<string, object> gameStateStorage;
			if (!storageRoot.TryGetValue("GameState", out object value) || (gameStateStorage = (value as Dictionary<string, object>)) == null)
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int i = 0;
			for (int num4 = IslandKeys.Length; i < num4; i++)
			{
				Dictionary<string, object> cigIslandStateStorage;
				object value4;
				Dictionary<string, object> dictionary;
				Dictionary<string, object> dictionary2;
				object value3;
				if (storageRoot.TryGetValue(IslandKeys[i], out object value2) && (dictionary = (value2 as Dictionary<string, object>)) != null && dictionary.TryGetValue("IsometricGrid", out value3) && (dictionary2 = (value3 as Dictionary<string, object>)) != null && dictionary.TryGetValue("IslandState", out value4) && (cigIslandStateStorage = (value4 as Dictionary<string, object>)) != null)
				{
					Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
					foreach (KeyValuePair<string, object> item in dictionary2)
					{
						Dictionary<string, object> dictionary5;
						Dictionary<string, object> dictionary4;
						if ((dictionary4 = (item.Value as Dictionary<string, object>)) != null && dictionary4.TryGetValue("GridTile", out object value5) && (dictionary5 = (value5 as Dictionary<string, object>)) != null && dictionary5.ContainsKey("CurrentBoostTiles"))
						{
							SetCorrectLandmarkBoostPercentages(dictionary4, dictionary5);
							GetLandmarkBoostedBuildings(dictionary5, dictionary3);
						}
					}
					SetBuildingsBoostedPercentageAndRecalculateMaxStats(dictionary2, dictionary3, out int happinessDifference, out int maxPeopleDifference, out int maxEmployeesDifference);
					UpdateCIGIslandState(cigIslandStateStorage, happinessDifference, maxPeopleDifference, maxEmployeesDifference);
					num += happinessDifference;
					num2 += maxPeopleDifference;
					num3 += maxEmployeesDifference;
				}
			}
			UpdateGameState(gameStateStorage, num, num2, num3);
		}

		private void GetLandmarkBoostedBuildings(Dictionary<string, object> gridTileDataDictionary, Dictionary<string, int> boostedBuildingsIdentifiersAndLandmarkBoostPercentage)
		{
			object obj;
			if (gridTileDataDictionary.TryGetValue("CurrentBoostPercentage", out object value) && (obj = value) is int)
			{
				int num = (int)obj;
				List<object> list;
				if (gridTileDataDictionary.TryGetValue("BoostedBuildingsIDs", out object value2) && (list = (value2 as List<object>)) != null)
				{
					foreach (object item in list)
					{
						string text;
						if ((text = (item as string)) != null)
						{
							if (boostedBuildingsIdentifiersAndLandmarkBoostPercentage.ContainsKey(text))
							{
								string key = text;
								boostedBuildingsIdentifiersAndLandmarkBoostPercentage[key] += num;
							}
							else
							{
								boostedBuildingsIdentifiersAndLandmarkBoostPercentage.Add(text, num);
							}
						}
					}
				}
			}
		}

		private void SetCorrectLandmarkBoostPercentages(Dictionary<string, object> tileDataDictionary, Dictionary<string, object> gridTileDataDictionary)
		{
			object obj;
			object value2;
			string key;
			if (tileDataDictionary.TryGetValue("PrefabName", out object value) && (key = (value as string)) != null && gridTileDataDictionary.TryGetValue("level", out value2) && (obj = value2) is int)
			{
				int num = (int)obj;
				int[] value3;
				if (LandmarksBoostPercentages.TryGetValue(key, out value3) && num >= 0 && num < value3.Length)
				{
					gridTileDataDictionary["CurrentBoostPercentage"] = LandmarksBoostPercentages[key][num];
				}
			}
		}

		private int GetNewBoostedValue(int currentStat, int newBoostPercentage, int oldBoostPercentage)
		{
			return (int)Math.Ceiling((float)currentStat / (1f + (float)oldBoostPercentage / 100f) * (1f + (float)newBoostPercentage / 100f));
		}

		private void SetBuildingsBoostedPercentageAndRecalculateMaxStats(Dictionary<string, object> isometricGridStorage, Dictionary<string, int> buildingsIdentifiersAndLandmarkBoostPercentage, out int happinessDifference, out int maxPeopleDifference, out int maxEmployeesDifference)
		{
			happinessDifference = 0;
			maxPeopleDifference = 0;
			maxEmployeesDifference = 0;
			foreach (KeyValuePair<string, object> item in isometricGridStorage)
			{
				object value2;
				Dictionary<string, object> dictionary2;
				Dictionary<string, object> dictionary;
				object obj;
				if ((dictionary = (item.Value as Dictionary<string, object>)) != null && dictionary.TryGetValue("GridTile", out object value) && (dictionary2 = (value as Dictionary<string, object>)) != null && dictionary2.TryGetValue("BoostedPercentage", out value2) && (obj = value2) is int)
				{
					int num = (int)obj;
					int value4;
					string key;
					if (dictionary2.TryGetValue("UniqueIdentifier", out object value3) && (key = (value3 as string)) != null && buildingsIdentifiersAndLandmarkBoostPercentage.TryGetValue(key, out value4) && num != value4)
					{
						if (dictionary2.TryGetValue("maxPeople", out object value5) && (obj = value5) is int)
						{
							int num2 = (int)obj;
							int newBoostedValue = GetNewBoostedValue(num2, value4, num);
							maxPeopleDifference += newBoostedValue - num2;
							dictionary2["maxPeople"] = newBoostedValue;
						}
						if (dictionary2.ContainsKey("maxEmployees"))
						{
							UpdateCommercialBuilding(dictionary2, value4, num, out int employeesDifference);
							maxEmployeesDifference += employeesDifference;
						}
						if (dictionary2.TryGetValue("happiness", out object value6) && (obj = value6) is int)
						{
							int num3 = (int)obj;
							int newBoostedValue2 = GetNewBoostedValue(num3, value4, num);
							happinessDifference += newBoostedValue2 - num3;
							dictionary2["happiness"] = newBoostedValue2;
						}
						dictionary2["BoostedPercentage"] = value4;
					}
				}
			}
		}

		private void UpdateCommercialBuilding(Dictionary<string, object> gridTileDataDictionary, int newBoostPercentage, int oldBoostPercentage, out int employeesDifference)
		{
			object obj;
			if (gridTileDataDictionary.TryGetValue("maxEmployees", out object value) && (obj = value) is int)
			{
				int num = (int)obj;
				int newBoostedValue = GetNewBoostedValue(num, newBoostPercentage, oldBoostPercentage);
				employeesDifference = newBoostedValue - num;
				gridTileDataDictionary["maxEmployees"] = newBoostedValue;
			}
			else
			{
				employeesDifference = 0;
			}
			Dictionary<string, object> dictionary;
			if (gridTileDataDictionary.TryGetValue("profit", out object value2) && (dictionary = (value2 as Dictionary<string, object>)) != null)
			{
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				foreach (KeyValuePair<string, object> item in dictionary)
				{
					if ((obj = item.Value) is decimal)
					{
						decimal d = (decimal)obj;
						decimal num2 = Math.Ceiling(d / (decimal.One + (decimal)oldBoostPercentage / 100m) * (decimal.One + (decimal)newBoostPercentage / 100m));
						dictionary2.Add(item.Key, num2);
					}
				}
				gridTileDataDictionary["profit"] = dictionary2;
			}
		}

		private void UpdateCIGIslandState(Dictionary<string, object> cigIslandStateStorage, int happinessDifference, int maxHousingDifference, int maxJobsDifference)
		{
			int num2;
			object obj;
			if (cigIslandStateStorage.TryGetValue("Happiness", out object value) && (obj = value) is int)
			{
				int num = (int)obj;
				num2 = num;
			}
			else
			{
				num2 = 0;
			}
			int num3 = num2;
			cigIslandStateStorage["Happiness"] = num3 + happinessDifference;
			int num5;
			if (cigIslandStateStorage.TryGetValue("Housing", out object value2) && (obj = value2) is int)
			{
				int num4 = (int)obj;
				num5 = num4;
			}
			else
			{
				num5 = 0;
			}
			int num6 = num5;
			cigIslandStateStorage["Housing"] = num6 + maxHousingDifference;
			int num8;
			if (cigIslandStateStorage.TryGetValue("Jobs", out object value3) && (obj = value3) is int)
			{
				int num7 = (int)obj;
				num8 = num7;
			}
			else
			{
				num8 = 0;
			}
			int num9 = num8;
			cigIslandStateStorage["Jobs"] = num9 + maxJobsDifference;
		}

		private void UpdateGameState(Dictionary<string, object> gameStateStorage, int globalHappinessDifference, int globalHousingDifference, int globalJobsDifference)
		{
			int num2;
			object obj;
			if (gameStateStorage.TryGetValue("GlobalHappiness", out object value) && (obj = value) is int)
			{
				int num = (int)obj;
				num2 = num;
			}
			else
			{
				num2 = 0;
			}
			int num3 = num2;
			gameStateStorage["GlobalHappiness"] = num3 + globalHappinessDifference;
			int num5;
			if (gameStateStorage.TryGetValue("GlobalHousing", out object value2) && (obj = value2) is int)
			{
				int num4 = (int)obj;
				num5 = num4;
			}
			else
			{
				num5 = 0;
			}
			int num6 = num5;
			gameStateStorage["GlobalHousing"] = num6 + globalHousingDifference;
			int num8;
			if (gameStateStorage.TryGetValue("GlobalJobs", out object value3) && (obj = value3) is int)
			{
				int num7 = (int)obj;
				num8 = num7;
			}
			else
			{
				num8 = 0;
			}
			int num9 = num8;
			gameStateStorage["GlobalJobs"] = num9 + globalJobsDifference;
		}
	}
}
