using CIG;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingWarehousePopup : Popup
{
	[SerializeField]
	private RecyclerGridLayoutGroup _recyclerGrid;

	private readonly Dictionary<GameObject, BuildingWarehouseItem> _gameObjectToItemMapping = new Dictionary<GameObject, BuildingWarehouseItem>();

	private BuildingWarehouseManager _buildingWarehouseManager;

	private IslandsManager _islandsManager;

	public override string AnalyticsScreenName => "building_warehouse";

	public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
	{
		base.Initialize(model, canvasScaler);
		_buildingWarehouseManager = model.Game.BuildingWarehouseManager;
		_islandsManager = model.Game.IslandsManager;
		_recyclerGrid.PushInstances();
		_recyclerGrid.Init(_buildingWarehouseManager.AllCount, InitializeBuildingWarehouseItem);
	}

	protected override void OnDestroy()
	{
		if (SingletonMonobehaviour<FPSLimiter>.IsAvailable)
		{
			SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
		}
		if (_buildingWarehouseManager != null)
		{
			_buildingWarehouseManager.SlotUnlockedEvent -= OnSlotUnlocked;
			_buildingWarehouseManager = null;
		}
		base.OnDestroy();
	}

	public override void Open(PopupRequest request)
	{
		base.Open(request);
		SingletonMonobehaviour<FPSLimiter>.Instance.PushUnlimitedFPSRequest(this);
		_buildingWarehouseManager.SlotUnlockedEvent += OnSlotUnlocked;
		_recyclerGrid.Refresh();
	}

	public BuildingWarehouseItem ScrollToAndGetEmptySlot()
	{
		int index = _buildingWarehouseManager.AllBuildingsCount + _buildingWarehouseManager.VacantUnlockedSlots;
		GameObject instance = _recyclerGrid.ScrollToAndGetInstance(index);
		return GetItem(instance);
	}

	protected override void Closed()
	{
		_buildingWarehouseManager.SlotUnlockedEvent -= OnSlotUnlocked;
		SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
		base.Closed();
	}

	private bool InitializeBuildingWarehouseItem(GameObject go, int index)
	{
		if (index < 0 || index >= _buildingWarehouseManager.AllCount)
		{
			return false;
		}
		BuildingWarehouseItem item = GetItem(go);
		if (index < _buildingWarehouseManager.NewBuildingCount)
		{
			InitializeItemFromWarehouse(item, index, newBuilding: true);
		}
		else if (index < _buildingWarehouseManager.AllBuildingsCount)
		{
			InitializeItemFromWarehouse(item, index - _buildingWarehouseManager.NewBuildingCount, newBuilding: false);
		}
		else if (index < _buildingWarehouseManager.AllBuildingsCount + _buildingWarehouseManager.VacantUnlockedSlots)
		{
			item.InitUnlockedSlot();
		}
		else
		{
			int num = index - (_buildingWarehouseManager.AllBuildingsCount + _buildingWarehouseManager.VacantUnlockedSlots);
			int unlockPrice = _buildingWarehouseManager.GetUnlockPrice(num);
			item.InitLockedSlot(unlockPrice, (num == 0) ? new Action(BuySlot) : null);
		}
		return true;
	}

	private void InitializeItemFromWarehouse(BuildingWarehouseItem item, int index, bool newBuilding)
	{
		if (_buildingWarehouseManager.TryGetBuilding(index, newBuilding, out BuildingProperties properties, out int level, out bool isCashBuilding))
		{
			item.InitPopulatedSlot(properties, level, delegate
			{
				Place(index, properties, level, newBuilding, isCashBuilding);
			});
		}
	}

	private BuildingWarehouseItem GetItem(GameObject instance)
	{
		if (!_gameObjectToItemMapping.TryGetValue(instance, out BuildingWarehouseItem value))
		{
			value = (_gameObjectToItemMapping[instance] = instance.GetComponent<BuildingWarehouseItem>());
		}
		return value;
	}

	private void Place(int index, BuildingProperties properties, int level, bool isNewBuilding, bool isCashBuilding)
	{
		if (IsometricIsland.Current.IsometricGrid.ElementTypeAvailable(properties.SurfaceType))
		{
			_popupManager.RequestPopup(new BuildConfirmWarehousePopupRequest(properties, isNewBuilding, isCashBuilding, delegate(CIGBuilding builtBuilding)
			{
				_buildingWarehouseManager.RemoveBuilding(index, isNewBuilding);
				builtBuilding.BuildFromWarehouse(isNewBuilding, level);
			}));
			_popupManager.CloseAllOpenPopups(instant: false);
		}
		else
		{
			_popupManager.RequestPopup(GenericPopupRequest.UnavailableSurfaceTypePopupRequest(_popupManager, _islandsManager, properties));
		}
	}

	private void BuySlot()
	{
		_buildingWarehouseManager.UnlockSlot();
	}

	private void OnSlotUnlocked()
	{
		_recyclerGrid.Refresh();
	}
}
