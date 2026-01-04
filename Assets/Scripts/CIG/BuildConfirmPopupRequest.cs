using System;

namespace CIG
{
	public abstract class BuildConfirmPopupRequest : PopupRequest
	{
		public CIGBuilding BuildingPrefab
		{
			get;
		}

		public BuildingProperties BuildingProperties
		{
			get;
		}

		public abstract bool IsCashBuilding
		{
			get;
		}

		public bool IsNewBuilding
		{
			get;
		}

		public bool IsMoving
		{
			get;
		}

		public BuildFinishType FinishType
		{
			get;
		}

		public Action<CIGBuilding> OnBuildConfirmed
		{
			get;
		}

		public Currency Cost
		{
			get;
		}

		public bool MoveCameraToTarget
		{
			get;
		}

		protected BuildConfirmPopupRequest(BuildingProperties buildingProperties, Currency cost, bool isNewBuilding, bool moveCameraToTarget, BuildFinishType finishType, Action<CIGBuilding> onBuildConfirmed = null)
			: this(SingletonMonobehaviour<BuildingsAssetCollection>.Instance.GetAsset(buildingProperties.BaseKey) as CIGBuilding, cost, isNewBuilding, isMoving: false, moveCameraToTarget, finishType, onBuildConfirmed)
		{
			BuildingProperties = buildingProperties;
		}

		protected BuildConfirmPopupRequest(CIGBuilding buildingPrefab, Currency cost, bool isNewBuilding, bool isMoving, bool moveCameraToTarget, BuildFinishType finishType, Action<CIGBuilding> onBuildConfirmed = null)
			: base(typeof(BuildConfirmPopup), enqueue: true, dismissable: false, showModalBackground: false, firstInQueue: true, HUDRegionType.CurrencyBars | HUDRegionType.Quests | HUDRegionType.ShopButton | HUDRegionType.RoadsButton | HUDRegionType.MapButton | HUDRegionType.MinigamesButton | HUDRegionType.LeaderboardButton | HUDRegionType.SocialButton | HUDRegionType.SettingsButton | HUDRegionType.UpgradesButton | HUDRegionType.KeyDealsButton | HUDRegionType.WarehouseButton | HUDRegionType.FlyingStartDealButton)
		{
			BuildingPrefab = buildingPrefab;
			BuildingProperties = BuildingPrefab.BuildingProperties;
			IsNewBuilding = isNewBuilding;
			IsMoving = isMoving;
			OnBuildConfirmed = onBuildConfirmed;
			FinishType = finishType;
			Cost = cost;
			MoveCameraToTarget = moveCameraToTarget;
		}
	}
}
