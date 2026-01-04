using CIG;
using CIG.Translation;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public sealed class BuildingWarehouseManager : IProductConsumer
{
    public delegate void WarehouseBuildingAdded(BuildingProperties buildingProperties);

    public delegate void WarehouseBuildingRemoved(int oldCount, int newCount);

    public delegate void SlotUnlockedEventHandler();

    public class StoredBuilding : IStorable
    {
        private const string NameKey = "name";

        private const string LevelKey = "level";

        private const string CashBuildingKey = "cashBuilding";

        private const string SourceKey = "source";

        public string Name
        {
            get;
        }

        public int Level
        {
            get;
        }

        public bool IsCashBuilding
        {
            get;
        }

        public WarehouseSource Source
        {
            get;
        }

        public StoredBuilding(BuildingProperties buildingProperties, int level, bool isCashBuilding, WarehouseSource source)
        {
            Name = buildingProperties.BaseKey;
            Level = level;
            IsCashBuilding = isCashBuilding;
            Source = source;
        }

        public StoredBuilding(StorageDictionary storage)
        {
            Name = storage.Get("name", string.Empty);
            Level = storage.Get("level", 1);
            IsCashBuilding = storage.Get("cashBuilding", defaultValue: false);
            Source = (WarehouseSource)storage.Get("source", 0);
        }

        StorageDictionary IStorable.Serialize()
        {
            StorageDictionary storageDictionary = new StorageDictionary();
            storageDictionary.Set("name", Name);
            storageDictionary.Set("level", Level);
            storageDictionary.Set("cashBuilding", IsCashBuilding);
            storageDictionary.Set("source", (int)Source);
            return storageDictionary;
        }
    }

    [CompilerGenerated]
    private sealed class DisplayClass41_0
    {
        public BuildingWarehouseManager _003C_003E4__this;

        public Currency cost;

        internal void g__BuyAction0()
        {
            _003C_003E4__this._gameState.SpendCurrencies(cost, CurrenciesSpentReason.BuildingWarehouse, b__1);
        }

        internal void b__1(bool success, Currencies spent)
        {
            if (success)
            {
                _003C_003E4__this._unlockedSlots++;
                _003C_003E4__this.FireSlotUnlockedEvent();
            }
        }
    }

    private const int InitiallyUnlockedCount = 1;

    private readonly StorageDictionary _storage;

    private readonly GameState _gameState;

    private readonly Properties _properties;

    private readonly BuildingWarehouseProperties _buildingWarehouseProperties;

    private readonly PopupManager _popupManager;

    private readonly List<StoredBuilding> _storedBuildings;

    private readonly List<StoredBuilding> _newBuildings;

    private int _unlockedSlots;

    private const string UnlockedSlotsKey = "UnlockedSlots";

    private const string StoredBuildingsKey = "StoredBuildings";

    private const string NewBuildingsKey = "NewBuildings";

    public int NewBuildingCount => _newBuildings.Count;

    public int StoredBuildingCount => _storedBuildings.Count;

    public int AllBuildingsCount => NewBuildingCount + StoredBuildingCount;

    public int VacantUnlockedSlots => _unlockedSlots - StoredBuildingCount;

    public int LockedSlots => _buildingWarehouseProperties.Prices.Count + 1 - _unlockedSlots;

    public int AllCount => NewBuildingCount + _unlockedSlots + LockedSlots;

    public event WarehouseBuildingAdded WarehouseBuildingAddedEvent;

    public event WarehouseBuildingRemoved WarehouseBuildingRemovedEvent;

    public event SlotUnlockedEventHandler SlotUnlockedEvent;

    private void FireWarehouseBuildingAdded(BuildingProperties buildingProperties)
    {
        this.WarehouseBuildingAddedEvent?.Invoke(buildingProperties);
    }

    private void FireWarehouseBuildingRemoved(int oldCount, int newCount)
    {
        this.WarehouseBuildingRemovedEvent?.Invoke(oldCount, newCount);
    }

    private void FireSlotUnlockedEvent()
    {
        this.SlotUnlockedEvent?.Invoke();
    }

    public BuildingWarehouseManager(StorageDictionary storage, GameState gameState, Properties properties, PopupManager popupManager)
    {
        _storage = storage;
        _gameState = gameState;
        _properties = properties;
        _popupManager = popupManager;
        _buildingWarehouseProperties = _properties.BuildingWarehouseProperties;
        _unlockedSlots = _storage.Get("UnlockedSlots", 1);
        _storedBuildings = _storage.GetModels("StoredBuildings", (StorageDictionary sd) => new StoredBuilding(sd));
        _newBuildings = _storage.GetModels("NewBuildings", (StorageDictionary sd) => new StoredBuilding(sd));
    }

    public void SaveBuilding(BuildingProperties buildingProperties, int level, bool wasBuildWithCash, bool newBuilding, WarehouseSource source)
    {
        StoredBuilding storedBuilding = new StoredBuilding(buildingProperties, level, wasBuildWithCash, source);
        if (newBuilding)
        {
            _newBuildings.Add(storedBuilding);
        }
        else
        {
            _storedBuildings.Add(storedBuilding);
        }
        Serialize();
        FireWarehouseBuildingAdded(buildingProperties);
        Analytics.BuildingMovedToWarehouse(storedBuilding.Name, storedBuilding.Level, source);
    }

    public bool TryGetBuilding(int index, bool newBuilding, out BuildingProperties buildingProperties, out int level, out bool isCashBuilding)
    {
        StoredBuilding storedBuilding = newBuilding ? _newBuildings[index] : _storedBuildings[index];
        buildingProperties = _properties.GetProperties<BuildingProperties>(storedBuilding.Name);
        level = storedBuilding.Level;
        isCashBuilding = storedBuilding.IsCashBuilding;
        return buildingProperties != null;
    }

    public void RemoveBuilding(int index, bool newBuilding)
    {
        StoredBuilding storedBuilding;
        if (newBuilding)
        {
            storedBuilding = _newBuildings[index];
            _newBuildings.RemoveAt(index);
        }
        else
        {
            storedBuilding = _storedBuildings[index];
            _storedBuildings.RemoveAt(index);
        }
        int allBuildingsCount = AllBuildingsCount;
        FireWarehouseBuildingRemoved(allBuildingsCount + 1, allBuildingsCount);
        Analytics.BuildingMovedFromWarehouse(storedBuilding.Name, storedBuilding.Level, storedBuilding.Source);
    }

    public int GetUnlockPrice(int index)
    {
        return _buildingWarehouseProperties.Prices[index + _unlockedSlots - 1];
    }

    public void UnlockSlot()
    {
        DisplayClass41_0 displayClass41_ = new DisplayClass41_0();
        displayClass41_._003C_003E4__this = this;
        int unlockPrice = GetUnlockPrice(0);
        displayClass41_.cost = Currency.CashCurrency(unlockPrice);
        GenericPopupRequest request = new GenericPopupRequest("building_warehouse_buy_slot_confirm").SetTexts(Localization.Key("slot_empty"), Localization.Key("purchase_item_confirm")).SetIcon(SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.WarehouseSlot)).SetGreenButton(Localization.Integer(unlockPrice), null, displayClass41_.g__BuyAction0, SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(displayClass41_.cost))
            .SetRedButton(Localization.Key("cancel"));
        _popupManager.RequestPopup(request);
    }

    public int GetCashBuildingCount(string buildingName)
    {
        int num = 0;
        int i = 0;
        for (int count = _storedBuildings.Count; i < count; i++)
        {
            StoredBuilding storedBuilding = _storedBuildings[i];
            if (storedBuilding.Name == buildingName && storedBuilding.IsCashBuilding)
            {
                num++;
            }
        }
        int j = 0;
        for (int count2 = _newBuildings.Count; j < count2; j++)
        {
            StoredBuilding storedBuilding2 = _newBuildings[j];
            if (storedBuilding2.Name == buildingName && storedBuilding2.IsCashBuilding)
            {
                num++;
            }
        }
        return num;
    }

    public int GetBuildingCount(string buildingName)
    {
        int num = 0;
        int i = 0;
        for (int count = _storedBuildings.Count; i < count; i++)
        {
            if (_storedBuildings[i].Name == buildingName)
            {
                num++;
            }
        }
        int j = 0;
        for (int count2 = _newBuildings.Count; j < count2; j++)
        {
            if (_newBuildings[j].Name == buildingName)
            {
                num++;
            }
        }
        return num;
    }

    bool IProductConsumer.ConsumeProduct(TOCIStoreProduct product)
    {
        StoreProductCategory category = product.Category;
        if (category == StoreProductCategory.Landmark)
        {
            string landmarkName = product.LandmarkName;
            if (landmarkName == null)
            {
                UnityEngine.Debug.LogError("Can't consume product '" + product.Title + "' because it has no property 'name'.");
                return false;
            }
            BuildingProperties buildingProperties = _properties.AllLandmarkBuildings.Find((BuildingProperties l) => landmarkName == l.BaseKey);
            if (buildingProperties == null)
            {
                UnityEngine.Debug.LogError("Can't consume product '" + product.Title + "' because game product name '" + landmarkName + "' can't be matched to a landmark.");
                return false;
            }
            SaveBuilding(buildingProperties, 0, wasBuildWithCash: false, newBuilding: true, WarehouseSource.LandmarkShop);
            return true;
        }
        UnityEngine.Debug.LogErrorFormat("Missing product consumer for product category '{0}'", product.Category);
        return false;
    }

    public void Serialize()
    {
        _storage.Set("UnlockedSlots", _unlockedSlots);
        _storage.Set("StoredBuildings", _storedBuildings);
        _storage.Set("NewBuildings", _newBuildings);
    }
}
