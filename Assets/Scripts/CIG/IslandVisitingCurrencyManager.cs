using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class IslandVisitingCurrencyManager
	{
		private readonly StorageDictionary _storage;

		private readonly GameState _gameState;

		private readonly FriendsManager _friendsManager;

		private readonly IslandVisitingCurrencyProperties _properties;

		private readonly Dictionary<string, IslandVisitingCurrencyData> _currencyDatas;

		private int _currenciesCollected;

		private readonly Dictionary<string, int> _currenciesCollectedPerFriend;

		private readonly DateTime _currenciesRemainingExpireTime;

		private const string CurrencyDatasKey = "CurrencyDatas";

		private const string CurrenciesCollectedKey = "CurrenciesCollected";

		private const string CurrenciesCollectedPerFriendKey = "CurrenciesCollectedPerFriend";

		private const string CurrenciesCollectedExpireTimeKey = "CurrenciesCollectedExpireTime";

		public Currency Currency
		{
			get;
		}

		public IslandVisitingCurrencyManager(StorageDictionary storage, GameState gameState, FriendsManager friendsManager, IslandVisitingCurrencyProperties properties)
		{
			_storage = storage;
			_gameState = gameState;
			_friendsManager = friendsManager;
			_properties = properties;
			Currency = new Currency(_properties.Currency, decimal.One);
			_currencyDatas = _storage.GetDictionaryModels("CurrencyDatas", (StorageDictionary sd) => new IslandVisitingCurrencyData(sd));
			_currenciesCollected = _storage.Get("CurrenciesCollected", 0);
			_currenciesCollectedPerFriend = _storage.GetDictionary<int>("CurrenciesCollectedPerFriend");
			_currenciesRemainingExpireTime = _storage.GetDateTime("CurrenciesCollectedExpireTime", DateTime.MinValue);
			if (AntiCheatDateTime.UtcNow >= _currenciesRemainingExpireTime)
			{
				_currenciesCollectedPerFriend.Clear();
				_currenciesCollected = 0;
				_currenciesRemainingExpireTime = AntiCheatDateTime.Today.AddDays(1.0);
			}
		}

		public int GetCurrencyAmountRemaining(string userId, IslandId islandId)
		{
			int num = 0;
			if (_currencyDatas.TryGetValue(userId, out IslandVisitingCurrencyData value))
			{
				num = value.GetCurrencyCount(islandId);
			}
			int currenciesRemainingForFriend = GetCurrenciesRemainingForFriend(userId);
			int num2 = _properties.DailyAmountTotal - _currenciesCollected;
			return Mathf.Min(num, currenciesRemainingForFriend, num2);
		}

		public void ShowCurrency(string userId, IslandId islandId, string buildingGUID)
		{
			if (_currencyDatas.TryGetValue(userId, out IslandVisitingCurrencyData value))
			{
				value.ShowCurrency(islandId, buildingGUID);
			}
		}

		public void CollectCurrency(string userId, IslandId islandId, string buildingGUID, object earnSource)
		{
			if (_currencyDatas.TryGetValue(userId, out IslandVisitingCurrencyData value))
			{
				value.CollectCurrency(islandId, buildingGUID);
				_currenciesCollectedPerFriend.TryGetValue(userId, out int value2);
				_currenciesCollectedPerFriend[userId] = value2 + 1;
				_currenciesCollected++;
				_gameState.EarnCurrencies(Currency, CurrenciesEarnedReason.FriendIslandVisiting, new FlyingCurrenciesData(earnSource));
			}
		}

		public IslandVisitingCurrencyData GetCurrencyData(string userId)
		{
			if (_friendsManager.HasFriend(userId))
			{
				if (!_currencyDatas.TryGetValue(userId, out IslandVisitingCurrencyData value) || !value.IsValid)
				{
					value = new IslandVisitingCurrencyData(_properties.AmountRange, AntiCheatDateTime.UtcNow.AddSeconds(_properties.CollectDurationSeconds));
					_currencyDatas[userId] = value;
				}
				return value;
			}
			return null;
		}

		private int GetCurrenciesRemainingForFriend(string userId)
		{
			int num = _properties.DailyAmountPerFriend;
			if (_currenciesCollectedPerFriend.TryGetValue(userId, out int value))
			{
				num -= value;
			}
			return num;
		}

		public void Serialize()
		{
			_storage.Set("CurrencyDatas", _currencyDatas);
			_storage.Set("CurrenciesCollected", _currenciesCollected);
			_storage.Set("CurrenciesCollectedPerFriend", _currenciesCollectedPerFriend);
			_storage.Set("CurrenciesCollectedExpireTime", _currenciesRemainingExpireTime);
		}
	}
}
