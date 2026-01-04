using CIG.Translation;
using Tweening;
using UnityEngine;

namespace CIG
{
	public class HUDWarehouseButton : HUDCurrencyTweenHelper
	{
		[SerializeField]
		private Tweener _iconTweener;

		[SerializeField]
		private GameObject _countBadge;

		[SerializeField]
		private LocalizedText _countLabel;

		[SerializeField]
		private RectTransform _maskTransform;

		[SerializeField]
		private GameObject _firstTimeBadgeRoot;

		private PopupManager _popupManager;

		private BuildingWarehouseManager _buildingWarehouseManager;

		private decimal _currentValue;

		private decimal _endValue;

		public RectTransform MaskTransform => _maskTransform;

		public void Initialize(PopupManager popupManager, BuildingWarehouseManager buildingWarehouseManager)
		{
			_popupManager = popupManager;
			_buildingWarehouseManager = buildingWarehouseManager;
			Initialize();
			_buildingWarehouseManager.WarehouseBuildingRemovedEvent += OnWarehouseBuildingRemoved;
			GetWarehouseBuildingCount();
			SetFirstTimeBadgeShown(shown: false);
			SetActiveTweener(_iconTweener);
		}

		protected override void OnDestroy()
		{
			if (_buildingWarehouseManager != null)
			{
				_buildingWarehouseManager.WarehouseBuildingRemovedEvent -= OnWarehouseBuildingRemoved;
				_buildingWarehouseManager = null;
			}
			_popupManager = null;
			base.OnDestroy();
		}

		public void OnClicked()
		{
			_popupManager.RequestPopup(new BuildingWarehousePopupRequest());
		}

		public override void FlyingCurrencyFinishedPlaying(Currency earnedCurrency, bool animateHudElement = true)
		{
			_endValue += earnedCurrency.Value;
			if (animateHudElement)
			{
				TweenTo(_currentValue, _endValue);
			}
			else
			{
				UpdateValue(_endValue);
			}
			base.FlyingCurrencyFinishedPlaying(earnedCurrency, animateHudElement);
		}

		public void SetFirstTimeBadgeShown(bool shown)
		{
			_firstTimeBadgeRoot.SetActive(shown);
			UpdateCountBadge();
		}

		protected override void UpdateValue(decimal value)
		{
			_currentValue = value;
			UpdateCountBadge();
		}

		private void GetWarehouseBuildingCount()
		{
			_endValue = _buildingWarehouseManager.AllBuildingsCount;
			UpdateValue(_endValue);
		}

		private void OnWarehouseBuildingRemoved(int oldCount, int newCount)
		{
			decimal num = newCount - oldCount;
			_endValue += num;
			TweenTo(_currentValue, _endValue);
		}

		private void UpdateCountBadge()
		{
			_countLabel.LocalizedString = Localization.Integer(_currentValue);
			_countBadge.SetActive(_currentValue > 0.5m && !_firstTimeBadgeRoot.activeSelf);
		}
	}
}
