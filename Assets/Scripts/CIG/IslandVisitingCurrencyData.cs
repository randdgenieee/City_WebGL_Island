using SparkLinq;
using System;
using System.Collections.Generic;

namespace CIG
{
	public class IslandVisitingCurrencyData : IStorable
	{
		private class IslandCurrencyData : IStorable
		{
			private const string AmountKey = "Amount";

			private const string BuildingsShowingCurrencyKey = "BuildingsShowingCurrency";

			public int Amount
			{
				get;
				private set;
			}

			public HashSet<string> BuildingsShowingCurrency
			{
				get;
			}

			public IslandCurrencyData(int amount)
			{
				Amount = amount;
				BuildingsShowingCurrency = new HashSet<string>();
			}

			public IslandCurrencyData(StorageDictionary storage)
			{
				Amount = storage.Get("Amount", 0);
				BuildingsShowingCurrency = storage.GetHashSet<string>("BuildingsShowingCurrency");
			}

			public void ShowCurrency(string buildingGUID)
			{
				BuildingsShowingCurrency.Add(buildingGUID);
			}

			public void CollectCurrency(string buildingGUID)
			{
				Amount--;
				BuildingsShowingCurrency.Remove(buildingGUID);
			}

			StorageDictionary IStorable.Serialize()
			{
				StorageDictionary storageDictionary = new StorageDictionary();
				storageDictionary.Set("Amount", Amount);
				storageDictionary.Set("BuildingsShowingCurrency", BuildingsShowingCurrency);
				return storageDictionary;
			}
		}

		private readonly Dictionary<IslandId, IslandCurrencyData> _currenciesPerIsland;

		private readonly DateTime _expiration;

		private const string CurrenciesPerIslandKey = "CurrenciesPerIsland";

		private const string ExpirationKey = "Expiration";

		public bool IsValid => AntiCheatDateTime.UtcNow < _expiration;

		public IslandVisitingCurrencyData(List<int> possibleAmountPerIsland, DateTime expiration)
		{
			_expiration = expiration;
			_currenciesPerIsland = new Dictionary<IslandId, IslandCurrencyData>();
			IList<IslandId> allIslandIds = IslandExtensions.AllIslandIds;
			int i = 0;
			for (int count = allIslandIds.Count; i < count; i++)
			{
				_currenciesPerIsland.Add(allIslandIds[i], new IslandCurrencyData(possibleAmountPerIsland.PickRandom()));
			}
		}

		public IslandVisitingCurrencyData(StorageDictionary storage)
		{
			_currenciesPerIsland = ConvertToIslandDictionary(storage.GetDictionaryModels("CurrenciesPerIsland", (StorageDictionary sd) => new IslandCurrencyData(sd)));
			_expiration = storage.GetDateTime("Expiration", AntiCheatDateTime.UtcNow);
		}

		public void ShowCurrency(IslandId islandId, string buildingGUID)
		{
			if (_currenciesPerIsland.TryGetValue(islandId, out IslandCurrencyData value))
			{
				value.ShowCurrency(buildingGUID);
			}
		}

		public void CollectCurrency(IslandId islandId, string buildingGUID)
		{
			if (_currenciesPerIsland.TryGetValue(islandId, out IslandCurrencyData value))
			{
				value.CollectCurrency(buildingGUID);
			}
		}

		public int GetCurrencyCount(IslandId islandId)
		{
			if (_currenciesPerIsland.TryGetValue(islandId, out IslandCurrencyData value))
			{
				return value.Amount;
			}
			return 0;
		}

		public HashSet<string> GetBuildingsShowingCurrency(IslandId islandId)
		{
			if (_currenciesPerIsland.TryGetValue(islandId, out IslandCurrencyData value))
			{
				return value.BuildingsShowingCurrency;
			}
			return null;
		}

		StorageDictionary IStorable.Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("CurrenciesPerIsland", ConvertToStringDictionary(_currenciesPerIsland));
			storageDictionary.Set("Expiration", _expiration);
			return storageDictionary;
		}

		private Dictionary<string, IslandCurrencyData> ConvertToStringDictionary(Dictionary<IslandId, IslandCurrencyData> dict)
		{
			Dictionary<string, IslandCurrencyData> dictionary = new Dictionary<string, IslandCurrencyData>();
			foreach (KeyValuePair<IslandId, IslandCurrencyData> item in dict)
			{
				dictionary.Add(((int)item.Key).ToString(), item.Value);
			}
			return dictionary;
		}

		private Dictionary<IslandId, IslandCurrencyData> ConvertToIslandDictionary(Dictionary<string, IslandCurrencyData> dict)
		{
			Dictionary<IslandId, IslandCurrencyData> dictionary = new Dictionary<IslandId, IslandCurrencyData>();
			foreach (KeyValuePair<string, IslandCurrencyData> item in dict)
			{
				dictionary.Add((IslandId)int.Parse(item.Key), item.Value);
			}
			return dictionary;
		}
	}
}
