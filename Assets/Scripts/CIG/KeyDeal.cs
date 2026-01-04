using UnityEngine;

namespace CIG
{
	public class KeyDeal
	{
		public delegate void KeyDealPurchasedHandler(KeyDeal keyDeal);

		public enum KeyDealType
		{
			Invalid,
			CashLocked,
			CashUnlocked,
			GoldLocked,
			GoldUnlocked
		}

		private readonly KeyDealsManager _keyDealsManager;

		private readonly GameStats _gameStats;

		private readonly GameState _gameState;

		private readonly BuildingWarehouseManager _buildingWarehouseManager;

		private readonly Multipliers _multipliers;

		private readonly decimal _priceMultiplier;

		private const string BuildingNameKey = "BuildingName";

		private const string PriceMultiplierKey = "PriceMultiplier";

		private const string PurchasedKey = "Purchased";

		public BuildingProperties BuildingProperties
		{
			get;
		}

		public KeyDealType Type
		{
			get;
			private set;
		}

		public bool Purchased
		{
			get;
			private set;
		}

		public Currency Price
		{
			get
			{
				switch (Type)
				{
				case KeyDealType.CashLocked:
				case KeyDealType.GoldLocked:
				case KeyDealType.GoldUnlocked:
					return GoldKeyPrice;
				case KeyDealType.CashUnlocked:
					if (!BuildingProperties.GetConstructionCost(_gameState, _gameStats, _buildingWarehouseManager).IsMatchingName("Cash"))
					{
						return GoldKeyPrice;
					}
					return SilverKeyPrice;
				default:
					UnityEngine.Debug.LogError("The KeyDeal is invalid, which means it doesn't have a price.");
					return Currency.Invalid;
				}
			}
		}

		private Currency SilverKeyPrice => Currency.SilverKeyCurrency(CIGUtilities.Round(BuildingProperties.SilverKeysConstructionCost * _priceMultiplier * _multipliers.GetMultiplier(MultiplierType.KeyDealSilverKeyCost), RoundingMethod.Nearest, 0));

		private Currency GoldKeyPrice => Currency.GoldKeyCurrency(CIGUtilities.Round(BuildingProperties.GoldKeysConstructionCost * _priceMultiplier * _multipliers.GetMultiplier(MultiplierType.KeyDealGoldKeyCost), RoundingMethod.Nearest, 0));

		public Currency BuildingConstructionCost => BuildingProperties.GetConstructionCost(_gameState, _gameStats, _buildingWarehouseManager);

		public event KeyDealPurchasedHandler KeyDealPurchasedEvent;

		private void FireKeyDealPurchasedEvent()
		{
			this.KeyDealPurchasedEvent?.Invoke(this);
		}

		private KeyDeal(BuildingProperties buildingProperties, KeyDealType type, KeyDealsManager keyDealsManager, GameStats gameStats, GameState gameState, Multipliers multipliers, BuildingWarehouseManager buildingWarehouseManager)
		{
			Type = type;
			_keyDealsManager = keyDealsManager;
			_gameStats = gameStats;
			_gameState = gameState;
			_multipliers = multipliers;
			_buildingWarehouseManager = buildingWarehouseManager;
			BuildingProperties = buildingProperties;
			_priceMultiplier = (decimal)Random.Range(0.75f, 1.25f);
		}

		public static bool TryCreate(KeyDealsManager keyDealsManager, GameStats gameStats, GameState gameState, Multipliers multipliers, BuildingWarehouseManager buildingWarehouseManager, BuildingProperties buildingProperties, out KeyDeal keyDeal)
		{
			KeyDealType keyDealType = DetermineKeyDealType(keyDealsManager, gameStats, gameState, buildingProperties);
			if (keyDealType != 0)
			{
				keyDeal = new KeyDeal(buildingProperties, keyDealType, keyDealsManager, gameStats, gameState, multipliers, buildingWarehouseManager);
				return true;
			}
			keyDeal = null;
			return false;
		}

		public bool UpdateType()
		{
			KeyDealType type = Type;
			Type = DetermineKeyDealType(_keyDealsManager, _gameStats, _gameState, BuildingProperties);
			return type != Type;
		}

		public void SetPurchased()
		{
			Purchased = true;
			if (!(BuildingProperties is LandmarkBuildingProperties))
			{
				_gameStats.AddKeyDealWithoutLandmarkPurchased();
			}
			FireKeyDealPurchasedEvent();
		}

		private static KeyDealType DetermineKeyDealType(KeyDealsManager keyDealsManager, GameStats gameStats, GameState gameState, BuildingProperties buildingProperties)
		{
			if (buildingProperties != null && !(buildingProperties is LandmarkBuildingProperties) && (buildingProperties.SurfaceType == SurfaceType.AnyTypeOfLand || buildingProperties.SurfaceType == SurfaceType.Grass || buildingProperties.SurfaceType == SurfaceType.Water || gameStats.GetSurfaceTypeElementsUnlocked(buildingProperties.SurfaceType) > 0))
			{
				if (buildingProperties.IsGoldBuilding)
				{
					if (buildingProperties.GoldKeysConstructionCost > decimal.Zero)
					{
						if (buildingProperties.IsUnlocked(gameState.Level))
						{
							return KeyDealType.GoldUnlocked;
						}
						if (buildingProperties.GetNextUnlockLevel(gameState.Level) <= gameState.Level + keyDealsManager.MaxLevelsHigherGoldBuilding)
						{
							return KeyDealType.GoldLocked;
						}
					}
				}
				else if (buildingProperties.SilverKeysConstructionCost > decimal.Zero && buildingProperties.GoldKeysConstructionCost > decimal.Zero)
				{
					if (buildingProperties.IsUnlocked(gameState.Level))
					{
						return KeyDealType.CashUnlocked;
					}
					if (buildingProperties.GetNextUnlockLevel(gameState.Level) <= gameState.Level + keyDealsManager.MaxLevelsHigherCashBuilding)
					{
						return KeyDealType.CashLocked;
					}
				}
			}
			return KeyDealType.Invalid;
		}

		public KeyDeal(StorageDictionary dictionary, KeyDealsManager keyDealsManager, GameStats gameStats, GameState gameState, Multipliers multipliers, Properties properties, BuildingWarehouseManager buildingWarehouseManager)
		{
			_keyDealsManager = keyDealsManager;
			_gameStats = gameStats;
			_gameState = gameState;
			_multipliers = multipliers;
			_buildingWarehouseManager = buildingWarehouseManager;
			string baseKey = dictionary.Get("BuildingName", string.Empty);
			_priceMultiplier = dictionary.Get("PriceMultiplier", decimal.Zero);
			Purchased = dictionary.Get("Purchased", defaultValue: false);
			BuildingProperties = properties.GetProperties<BuildingProperties>(baseKey);
			Type = DetermineKeyDealType(_keyDealsManager, _gameStats, _gameState, BuildingProperties);
		}

		public StorageDictionary Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			if (Type != 0)
			{
				storageDictionary.Set("BuildingName", BuildingProperties.BaseKey);
				storageDictionary.Set("PriceMultiplier", _priceMultiplier);
				storageDictionary.Set("Purchased", Purchased);
			}
			return storageDictionary;
		}
	}
}
