using SparkLinq;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class Reward : IStorable
	{
		private static readonly HashSet<string> CashGiftKeys = new HashSet<string>
		{
			"cashgift",
			"giftcash"
		};

		private static readonly HashSet<string> GoldGiftKeys = new HashSet<string>
		{
			"goldgift",
			"giftgold"
		};

		private static readonly HashSet<string> SilverKeyGiftKeys = new HashSet<string>
		{
			"silverkeygift",
			"silverkeysgift",
			"giftsilverkey",
			"giftsilverkeys"
		};

		private static readonly HashSet<string> GoldKeyGiftKeys = new HashSet<string>
		{
			"goldkeygift",
			"goldkeysgift",
			"giftgoldkey",
			"giftgoldkeys"
		};

		private static readonly HashSet<string> XPGiftKey = new HashSet<string>
		{
			"xpgift",
			"giftxp"
		};

		private static readonly HashSet<string> BuildingGiftKey = new HashSet<string>
		{
			"buildinggift",
			"buildingsgift",
			"giftbuilding",
			"giftbuildings"
		};

		private const string CurrenciesKey = "Currencies";

		private const string BuildingsKey = "Buildings";

		public Currencies Currencies
		{
			get;
			set;
		}

		public List<BuildingProperties> Buildings
		{
			get;
		}

		public bool IsEmpty
		{
			get
			{
				if (Currencies.IsEmpty())
				{
					return Buildings.Count == 0;
				}
				return false;
			}
		}

		public Reward()
			: this(new Currencies())
		{
		}

		public Reward(Currency currency)
			: this(new Currencies(currency), new List<BuildingProperties>())
		{
		}

		public Reward(Currencies currencies)
			: this(currencies, new List<BuildingProperties>())
		{
		}

		public Reward(Reward reward)
			: this(new Currencies(reward.Currencies), new List<BuildingProperties>(reward.Buildings))
		{
		}

		public Reward(Currencies currencies, List<BuildingProperties> buildings)
		{
			Currencies = currencies;
			Buildings = buildings;
		}

		public Reward(StorageDictionary storage, Properties properties)
		{
			Currencies = storage.GetModel("Currencies", (StorageDictionary sd) => new Currencies(sd), null);
			List<string> list = storage.GetList<string>("Buildings");
			Buildings = new List<BuildingProperties>();
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				string text = list[i];
				BuildingProperties properties2 = properties.GetProperties<BuildingProperties>(text);
				if (properties2 == null)
				{
					UnityEngine.Debug.LogError("Couldn't find building with name: " + text + ".");
				}
				else
				{
					Buildings.Add(properties2);
				}
			}
		}

		public static Reward operator +(Reward reward1, Reward reward2)
		{
			if (reward1 == null)
			{
				return reward2;
			}
			if (reward2 == null)
			{
				return reward1;
			}
			Currencies currencies = reward1.Currencies + reward2.Currencies;
			List<BuildingProperties> buildings = reward1.Buildings.ToList().Concat(reward2.Buildings);
			return new Reward(currencies, buildings);
		}

		public static Reward operator -(Reward reward1, Reward reward2)
		{
			Currencies currencies = reward1.Currencies - reward2.Currencies;
			List<BuildingProperties> list = reward1.Buildings.ToList();
			int i = 0;
			for (int count = reward2.Buildings.Count; i < count; i++)
			{
				BuildingProperties item = reward2.Buildings[i];
				list.Remove(item);
			}
			return new Reward(currencies, list);
		}

		public static Reward ParseFromServer(string value, Properties properties)
		{
			string[] array = value.Split(',');
			Currencies currencies = new Currencies();
			List<BuildingProperties> list = new List<BuildingProperties>();
			int i = 0;
			for (int num = array.Length; i < num; i++)
			{
				string text = array[i];
				if (text.StartsWith("XP"))
				{
					currencies += ParseCurrency(text.Substring(2), "XP");
				}
				else
				{
					if (text.StartsWith("X"))
					{
						continue;
					}
					if (text.StartsWith("SK"))
					{
						currencies += ParseCurrency(text.Substring(2), "SilverKey");
					}
					else if (text.StartsWith("GK"))
					{
						currencies += ParseCurrency(text.Substring(2), "GoldKey");
					}
					else if (text.StartsWith("CR"))
					{
						currencies += ParseCurrency(text.Substring(1), "Crane");
					}
					else if (text.StartsWith("T"))
					{
						currencies += ParseCurrency(text.Substring(1), "Token");
					}
					else if (text.StartsWith("G"))
					{
						currencies += ParseCurrency(text.Substring(1), "Gold");
					}
					else if (text.StartsWith("C"))
					{
						currencies += ParseCurrency(text.Substring(1), "Cash");
					}
					else if (text.StartsWith("B"))
					{
						if (TryParseBuilding(text.Substring(1), properties, out BuildingProperties buildingProperties))
						{
							list.Add(buildingProperties);
						}
					}
					else
					{
						UnityEngine.Debug.LogError("Unrecognized currency type: '" + text + "'. This value was passed on from the server.");
					}
				}
			}
			return new Reward(currencies, list);
		}

		public static Reward ParseFromFirebaseMessage(IDictionary<string, string> value, Properties properties)
		{
			Currencies currencies = new Currencies();
			List<BuildingProperties> list = new List<BuildingProperties>();
			foreach (KeyValuePair<string, string> item2 in value)
			{
				string item = item2.Key.ToLower();
				BuildingProperties buildingProperties;
				if (CashGiftKeys.Contains(item))
				{
					currencies += ParseCurrency(item2.Value, "Cash");
				}
				else if (GoldGiftKeys.Contains(item))
				{
					currencies += ParseCurrency(item2.Value, "Gold");
				}
				else if (SilverKeyGiftKeys.Contains(item))
				{
					currencies += ParseCurrency(item2.Value, "SilverKey");
				}
				else if (GoldKeyGiftKeys.Contains(item))
				{
					currencies += ParseCurrency(item2.Value, "GoldKey");
				}
				else if (XPGiftKey.Contains(item))
				{
					currencies += ParseCurrency(item2.Value, "XP");
				}
				else if (BuildingGiftKey.Contains(item) && TryParseBuilding(item2.Value, properties, out buildingProperties))
				{
					list.Add(buildingProperties);
				}
			}
			return new Reward(currencies, list);
		}

		public void Give(GameState gameState, CraneManager craneManager, BuildingWarehouseManager buildingWarehouseManager, WarehouseSource source)
		{
			int i = 0;
			for (int count = Buildings.Count; i < count; i++)
			{
				buildingWarehouseManager.SaveBuilding(Buildings[i], 0, wasBuildWithCash: false, newBuilding: true, source);
			}
			int num = (int)Currencies.GetValue("Crane");
			if (num > 0)
			{
				craneManager.AddCranes(num);
			}
			Currencies.SetValue("Crane", decimal.Zero);
			int num2 = (int)Currencies.GetValue("LevelUp");
			if (num2 > 0)
			{
				Currencies += Currency.XPCurrency(gameState.GetXpForLevelsUp(num2));
			}
			Currencies.SetValue("LevelUp", decimal.Zero);
			gameState.GiveCurrencies(Currencies.WithoutEmpty(), CurrenciesEarnedReason.Reward);
		}

		private static bool TryParseBuilding(string buildingName, Properties properties, out BuildingProperties buildingProperties)
		{
			buildingProperties = properties.GetProperties<BuildingProperties>(buildingName);
			if (buildingProperties == null)
			{
				UnityEngine.Debug.LogError("Parse error: Couldn't find building with name: " + buildingName + ". This value was passed on from the server.");
			}
			return buildingProperties != null;
		}

		private static Currencies ParseCurrency(string value, string currencyType)
		{
			if (!long.TryParse(value, out long result))
			{
				UnityEngine.Debug.LogError("Parse error: `" + value + "` is not a long. This value was passed on from the server for type `" + currencyType + "`.");
				return new Currencies();
			}
			return new Currencies(currencyType, result);
		}

		StorageDictionary IStorable.Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("Currencies", Currencies);
			storageDictionary.Set("Buildings", from buildingProperties in Buildings
				select buildingProperties.BaseKey);
			return storageDictionary;
		}
	}
}
