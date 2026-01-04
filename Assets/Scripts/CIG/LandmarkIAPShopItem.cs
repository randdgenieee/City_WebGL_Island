using System;
using UnityEngine;

namespace CIG
{
	public class LandmarkIAPShopItem : IAPShopItem
	{
		[SerializeField]
		private LocalizedText _name;

		[SerializeField]
		private BuildingImage _image;

		[SerializeField]
		private GameObject _checkmark;

		private GameStats _gameStats;

		private BuildingWarehouseManager _warehouseManager;

		private PopupManager _popupManager;

		private BuildingProperties _buildingProperties;

		private void OnDestroy()
		{
			if (_warehouseManager != null)
			{
				_warehouseManager.WarehouseBuildingAddedEvent -= WarehouseManagerOnWarehouseBuildingAddedEvent;
				_warehouseManager = null;
			}
		}

		public void Initialize(GameStats gameStats, BuildingWarehouseManager warehouseManager, PopupManager popupManager, BuildingProperties buildingProperties, TOCIStoreProduct product, Action<TOCIStoreProduct> onClick)
		{
			_gameStats = gameStats;
			_warehouseManager = warehouseManager;
			_popupManager = popupManager;
			_buildingProperties = buildingProperties;
			_name.LocalizedString = buildingProperties.LocalizedName;
			_image.Initialize(buildingProperties);
			Initialize(product, onClick, playRayTweener: true);
		}

		public override void SetVisible(bool visible)
		{
			base.SetVisible(visible);
			if (visible)
			{
				_warehouseManager.WarehouseBuildingAddedEvent -= WarehouseManagerOnWarehouseBuildingAddedEvent;
				_warehouseManager.WarehouseBuildingAddedEvent += WarehouseManagerOnWarehouseBuildingAddedEvent;
				SetCheckmarkVisible();
			}
			else
			{
				_warehouseManager.WarehouseBuildingAddedEvent -= WarehouseManagerOnWarehouseBuildingAddedEvent;
			}
		}

		public void OnQuestionMarkClicked()
		{
			_popupManager.RequestPopup(new BuildingPopupRequest(_buildingProperties, BuildingPopupContent.LandmarkPreview));
		}

		private void SetCheckmarkVisible()
		{
			_checkmark.SetActive(_gameStats.NumberOf(_buildingProperties.BaseKey) > 0 || _warehouseManager.GetBuildingCount(_buildingProperties.BaseKey) > 0);
		}

		private void WarehouseManagerOnWarehouseBuildingAddedEvent(BuildingProperties buildingProperties)
		{
			if (buildingProperties == _buildingProperties)
			{
				SetCheckmarkVisible();
			}
		}
	}
}
