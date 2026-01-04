namespace CIG
{
	public class OneTimeOfferManager
	{
		private readonly StorageDictionary _storage;

		private readonly OneTimeOfferTreasureChest _treasureChestOffer;

		private readonly OneTimeOfferBuilding _buildingOffer;

		public OneTimeOfferManager(StorageDictionary storage, GameState gameState, GameStats gameStats, BuildingWarehouseManager buildingsWarehouseManager, TreasureChestManager treasureManager, IAPStore<TOCIStoreProduct> storeManager, Properties properties)
		{
			_storage = storage;
			_buildingOffer = new OneTimeOfferBuilding(_storage.GetStorageDict("OneTimeOfferBuilding"), gameState, buildingsWarehouseManager, properties.OneTimeOfferProperties.OneTimeOfferBuildingProperties, properties.AllBuildingProperties, gameStats);
			_treasureChestOffer = new OneTimeOfferTreasureChest(_storage.GetStorageDict("OneTimeOfferTreasureChest"), treasureManager, storeManager, properties.OneTimeOfferProperties.OneTimeOfferTreasureChestProperties);
		}

		public void Release()
		{
			_treasureChestOffer.Release();
		}

		public OneTimeOfferBase GetAvailableOffer(int level)
		{
			if (_treasureChestOffer.CanDealBeOffered(level))
			{
				return _treasureChestOffer;
			}
			if (_buildingOffer.CanDealBeOffered(level))
			{
				return _buildingOffer;
			}
			return null;
		}

		public void Serialize()
		{
			_storage.Set("OneTimeOfferTreasureChest", _treasureChestOffer.Serialize());
			_storage.Set("OneTimeOfferBuilding", _buildingOffer.Serialize());
		}
	}
}
