using SparkLinq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class OneTimeOfferBuilding : OneTimeOfferBase
	{
		private readonly StorageDictionary _storage;

		private readonly BuildingWarehouseManager _buildingWarehouseManager;

		private readonly GameState _gameState;

		private readonly GameStats _gameStats;

		private readonly OneTimeOfferBuildingProperties _properties;

		private readonly List<BuildingProperties> _allBuildingProperties;

		private readonly Dictionary<string, double> _shownOffers;

		private double _lastShown;

		private float? _currentDiscount;

		public const string StorageKey = "OneTimeOfferBuilding";

		private const string CurrentDiscountKey = "CurrentDiscount";

		private const string ShownOffersKey = "ShownOffers";

		private const string LastShownKey = "LastShown";

		public override float CurrentDiscountPercentage
		{
			get
			{
				if (!_currentDiscount.HasValue)
				{
					_currentDiscount = _properties.InitialDiscount;
				}
				return _currentDiscount.Value;
			}
			protected set
			{
				_currentDiscount = Mathf.Clamp(value, _properties.MinimumDiscount, _properties.MaximumDiscount);
			}
		}

		public BuildingProperties BuildingProperties
		{
			get;
			private set;
		}

		public override decimal NormalPrice
		{
			get
			{
				if (BuildingProperties == null)
				{
					return decimal.Zero;
				}
				return BuildingProperties.GetConstructionCost(_gameState, _gameStats, _buildingWarehouseManager).Value;
			}
		}

		public OneTimeOfferBuilding(StorageDictionary storage, GameState gameState, BuildingWarehouseManager warehouseManager, OneTimeOfferBuildingProperties properties, List<BuildingProperties> allBuildingProperties, GameStats gameStats)
			: base(properties)
		{
			_gameState = gameState;
			_buildingWarehouseManager = warehouseManager;
			_storage = storage;
			_properties = properties;
			_allBuildingProperties = allBuildingProperties;
			_gameStats = gameStats;
			_shownOffers = storage.GetDictionary<double>("ShownOffers");
			_lastShown = storage.Get("LastShown", 0.0);
			_currentDiscount = storage.Get<float?>("CurrentDiscount", null);
		}

		protected override void OnOfferShown()
		{
			_shownOffers.Add(BuildingProperties.BaseKey, Timing.UtcNow);
			_lastShown = Timing.UtcNow;
		}

		public void Purchase(Action onSucces, Action onPurchaseCanceled)
		{
			Currency price = Currency.GoldCurrency(base.DiscountedPrice);
			_gameState.SpendCurrencies(price, 0, allowShopPopup: false, CurrenciesSpentReason.OneTimeOffer, delegate(bool success, Currencies spent)
			{
				if (success)
				{
					_buildingWarehouseManager.SaveBuilding(BuildingProperties, 0, wasBuildWithCash: false, newBuilding: true, WarehouseSource.OneTimeOffer);
					CurrentDiscountPercentage -= _properties.DiscountDecrement;
					Analytics.OneTimeOfferBuildingPurchased(BuildingProperties.BaseKey, price);
					OnOfferShown();
					BuildingProperties = null;
					onSucces?.Invoke();
				}
				else
				{
					onPurchaseCanceled?.Invoke();
				}
			});
		}

		public override void IgnoreOffer()
		{
			CurrentDiscountPercentage += _properties.DiscountIncrement;
			OnOfferShown();
			BuildingProperties = null;
		}

		public override bool CanDealBeOffered(int level)
		{
			if (base.OfferEnabled && level >= _properties.MinimumLevelRequired && Timing.UtcNow - _lastShown >= (double)_properties.CooldownSeconds)
			{
				if (BuildingProperties == null)
				{
					SelectNewOfferBuilding();
				}
				return BuildingProperties != null;
			}
			return false;
		}

		private void SelectNewOfferBuilding()
		{
			RemovedExpiredOffers();
			int currentLevel = _gameState.Level;
			List<BuildingProperties> list = new List<BuildingProperties>(_allBuildingProperties);
			decimal currentGold = _gameState.Balance.GetValue("Gold");
			List<BuildingProperties> list2 = from b in list
				where b.GetConstructionCost(_gameState, _gameStats, _buildingWarehouseManager).IsMatchingName("Gold") && !_shownOffers.ContainsKey(b.BaseKey) && GetDiscountedPrice(b.GetConstructionCost(_gameState, _gameStats, _buildingWarehouseManager).Value) > currentGold
				select b;
			List<BuildingProperties> list3 = from b in list2
				where b.IsUnlocked(currentLevel)
				select b;
			if (list3.Count > 0)
			{
				BuildingProperties = list3.PickRandom();
				return;
			}
			list3 = from b in list2
				where !b.IsUnlocked(currentLevel)
				select b;
			if (list3.Count > 0)
			{
				BuildingProperties = list3.PickRandom();
			}
		}

		private void RemovedExpiredOffers()
		{
			double expirationTime = Timing.UtcNow - (double)_properties.ExpirationSeconds;
			_shownOffers.RemoveAll((string key, double value) => value < expirationTime);
		}

		public StorageDictionary Serialize()
		{
			_storage.Set("ShownOffers", _shownOffers);
			_storage.Set("LastShown", _lastShown);
			_storage.SetOrRemove("CurrentDiscount", _currentDiscount);
			return _storage;
		}
	}
}
