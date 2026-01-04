using SparkLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class KeyDealsManager
	{
		public delegate void OnKeyDealsChangedHandler(KeyDeal[] keyDeals);

		public delegate void KeyDealBoughtEventHandler(KeyDeal keyDeal);

		public const int RefreshGoldPrice = 1;

		private readonly StorageDictionary _storage;

		private readonly GameState _gameState;

		private readonly GameStats _gameStats;

		private readonly BuildingWarehouseManager _buildingWarehouseManager;

		private readonly RoutineRunner _routineRunner;

		private readonly Multipliers _multipliers;

		private readonly KeyDealsProperties _properties;

		private readonly List<BuildingProperties> _allBuildingProperties;

		private DateTime _nextUpdateTime;

		private readonly List<KeyDeal> _keyDeals;

		private IEnumerator _waitRoutine;

		private const string NextUpdateTimeKey = "NextUpdateTime";

		private const string KeyDealsListKey = "KeyDealsList";

		public TimeSpan DealTimeRemaining => _nextUpdateTime - AntiCheatDateTime.UtcNow;

		public KeyDeal[] KeyDeals => _keyDeals.ToArray();

		public int Amount => _properties.Amount;

		public int MaxLevelsHigherCashBuilding => _properties.MaxLevelsHigherCashBuilding;

		public int MaxLevelsHigherGoldBuilding => _properties.MaxLevelsHigherGoldBuilding;

		public event OnKeyDealsChangedHandler OnKeyDealsChangedEvent;

		public event KeyDealBoughtEventHandler KeyDealBoughtEvent;

		private void FireOnKeyDealsChangedEvent()
		{
			this.OnKeyDealsChangedEvent?.Invoke(KeyDeals);
		}

		private void FireKeyDealBoughtEvent(KeyDeal keyDeal)
		{
			this.KeyDealBoughtEvent?.Invoke(keyDeal);
		}

		public KeyDealsManager(StorageDictionary storage, GameState gameState, GameStats gameStats, Multipliers multipliers, BuildingWarehouseManager buildingWarehouseManager, RoutineRunner routineRunner, Properties properties)
		{
			_storage = storage;
			_gameState = gameState;
			_gameStats = gameStats;
			_multipliers = multipliers;
			_buildingWarehouseManager = buildingWarehouseManager;
			_routineRunner = routineRunner;
			_properties = properties.KeyDealsProperties;
			_allBuildingProperties = properties.AllBuildingProperties;
			_nextUpdateTime = _storage.GetDateTime("NextUpdateTime", AntiCheatDateTime.UtcNow);
			_keyDeals = _storage.GetModels("KeyDealsList", (StorageDictionary sd) => new KeyDeal(sd, this, _gameStats, _gameState, _multipliers, properties, buildingWarehouseManager));
			_gameState.VisuallyLevelledUpEvent += OnVisuallyLevelledUp;
			if (_nextUpdateTime < AntiCheatDateTime.UtcNow || _keyDeals.Count < _properties.Amount)
			{
				RefreshKeyDeals();
				return;
			}
			int i = 0;
			for (int count = _keyDeals.Count; i < count; i++)
			{
				_keyDeals[i].KeyDealPurchasedEvent += OnKeyDealPurchased;
			}
			StartWaitRoutine();
		}

		public void BuyKeyDealsRefresh()
		{
			_gameState.SpendCurrencies(Currency.GoldCurrency(decimal.One), CurrenciesSpentReason.KeyDeals, delegate(bool success, Currencies spent)
			{
				if (success)
				{
					RefreshKeyDeals();
				}
			});
		}

		public void BuyKeyDeal(KeyDeal keyDeal)
		{
			_gameState.SpendCurrencies(keyDeal.Price, CurrenciesSpentReason.KeyDeals, delegate(bool success, Currencies spent)
			{
				if (success)
				{
					_buildingWarehouseManager.SaveBuilding(keyDeal.BuildingProperties, 0, wasBuildWithCash: false, newBuilding: true, WarehouseSource.Keydeal);
					keyDeal.SetPurchased();
					Analytics.KeyDealPurchased(keyDeal.BuildingProperties.BaseKey, spent);
				}
			});
		}

		private void RefreshKeyDeals()
		{
			_nextUpdateTime = AntiCheatDateTime.UtcNow.AddSeconds(_properties.RefreshTimeSeconds);
			PickDealBuildings();
			StartWaitRoutine();
		}

		private void StartWaitRoutine()
		{
			if (_waitRoutine != null)
			{
				_routineRunner.StopCoroutine(_waitRoutine);
			}
			_routineRunner.StartCoroutine(_waitRoutine = WaitForEndOfDeal());
		}

		private IEnumerator WaitForEndOfDeal()
		{
			yield return new WaitUntilUTCDateTime(_nextUpdateTime);
			_waitRoutine = null;
			RefreshKeyDeals();
		}

		private void PickDealBuildings()
		{
			for (int num = _keyDeals.Count - 1; num >= 0; num--)
			{
				_keyDeals[num].KeyDealPurchasedEvent -= OnKeyDealPurchased;
			}
			_keyDeals.Clear();
			List<BuildingProperties> list = new List<BuildingProperties>(_allBuildingProperties);
			list.Shuffle();
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				if (_keyDeals.Count >= _properties.Amount)
				{
					break;
				}
				if (KeyDeal.TryCreate(this, _gameStats, _gameState, _multipliers, _buildingWarehouseManager, list[i], out KeyDeal keyDeal))
				{
					_keyDeals.Add(keyDeal);
					keyDeal.KeyDealPurchasedEvent += OnKeyDealPurchased;
				}
			}
			if (_keyDeals.Count != _properties.Amount)
			{
				UnityEngine.Debug.LogErrorFormat("Only {0} key deals could be created", _keyDeals.Count);
			}
			FireOnKeyDealsChangedEvent();
		}

		private void OnVisuallyLevelledUp(int level)
		{
			bool flag = false;
			int i = 0;
			for (int count = _keyDeals.Count; i < count; i++)
			{
				flag |= _keyDeals[i].UpdateType();
			}
			if (flag)
			{
				FireOnKeyDealsChangedEvent();
			}
		}

		private void OnKeyDealPurchased(KeyDeal keyDeal)
		{
			FireKeyDealBoughtEvent(keyDeal);
			FireOnKeyDealsChangedEvent();
		}

		public void Serialize()
		{
			_storage.Set("NextUpdateTime", _nextUpdateTime);
			_storage.Set("KeyDealsList", new List<KeyDeal>(_keyDeals), (KeyDeal kd) => kd.Serialize());
		}
	}
}
