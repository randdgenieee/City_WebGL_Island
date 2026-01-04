using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class HUDShopButton : HUDRegionElement
	{
		[SerializeField]
		private NewBuildingsBadge _badge;

		[SerializeField]
		private RectTransform _maskTransform;

		[SerializeField]
		private GameObject _chestBadge;

		private GameState _gameState;

		private PopupManager _popupManager;

		private BuildingsManager _buildingsManager;

		private TreasureChestManager _treasureChestManager;

		private Properties _properties;

		public RectTransform MaskTransform => _maskTransform;

		public void Initialize(GameState gameState, PopupManager popupManager, BuildingsManager buildingsManager, TreasureChestManager treasureChestManager, Properties properties)
		{
			_gameState = gameState;
			_popupManager = popupManager;
			_buildingsManager = buildingsManager;
			_treasureChestManager = treasureChestManager;
			_properties = properties;
			_badge.Initialize(_gameState);
			_gameState.VisuallyLevelledUpEvent += OnVisuallyLevelledUp;
			_buildingsManager.ViewedBuildingHistoryChangedEvent += OnViewedBuildingHistoryChanged;
			OnViewedBuildingHistoryChanged(_buildingsManager.ViewedBuildingHistory);
			_treasureChestManager.ChestOpenableChangedEvent += OnChestOpenableChanged;
			OnChestOpenableChanged();
		}

		private void OnDestroy()
		{
			if (_gameState != null)
			{
				_gameState.VisuallyLevelledUpEvent -= OnVisuallyLevelledUp;
				_gameState = null;
			}
			if (_buildingsManager != null)
			{
				_buildingsManager.ViewedBuildingHistoryChangedEvent -= OnViewedBuildingHistoryChanged;
				_buildingsManager = null;
			}
			if (_treasureChestManager != null)
			{
				_treasureChestManager.ChestOpenableChangedEvent -= OnChestOpenableChanged;
				_treasureChestManager = null;
			}
			_popupManager = null;
		}

		public void OnShopClicked()
		{
			_popupManager.RequestPopup(new ShopPopupRequest());
		}

		private void UpdateNewBuildingsBadge(List<string> viewedBuildingHistory)
		{
			List<BuildingProperties> list = new List<BuildingProperties>();
			list.AddRange(_properties.AllResidentialBuildings);
			list.AddRange(_properties.AllCommercialBuildings);
			list.AddRange(_properties.AllCommunityBuildings);
			_badge.UpdateNewBuildingsBadge(list, viewedBuildingHistory);
		}

		private void OnVisuallyLevelledUp(int level)
		{
			UpdateNewBuildingsBadge(_buildingsManager.ViewedBuildingHistory);
		}

		private void OnViewedBuildingHistoryChanged(List<string> buildingHistory)
		{
			UpdateNewBuildingsBadge(buildingHistory);
		}

		private void OnChestOpenableChanged()
		{
			_chestBadge.SetActive(_treasureChestManager.HasOpenableChest);
		}
	}
}
