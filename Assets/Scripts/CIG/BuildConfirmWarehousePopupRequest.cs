using System;

namespace CIG
{
	public class BuildConfirmWarehousePopupRequest : BuildConfirmPopupRequest
	{
		public override bool IsCashBuilding
		{
			get;
		}

		public BuildConfirmWarehousePopupRequest(BuildingProperties buildingProperties, bool isNewBuilding, bool isCashBuilding, Action<CIGBuilding> onBuildConfirmed = null)
			: base(buildingProperties, Currency.Invalid, isNewBuilding, moveCameraToTarget: true, BuildFinishType.Warehouse, onBuildConfirmed)
		{
			IsCashBuilding = isCashBuilding;
		}
	}
}
