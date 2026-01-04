using System;

namespace CIG
{
	public class BuildConfirmBuyPopupRequest : BuildConfirmPopupRequest
	{
		public override bool IsCashBuilding
		{
			get;
		}

		public BuildConfirmBuyPopupRequest(BuildingProperties buildingProperties, Currency cost, bool isCashBuilding, BuildFinishType finishType, Action<CIGBuilding> onBuildConfirmed = null)
			: base(buildingProperties, cost, isNewBuilding: true, moveCameraToTarget: true, finishType, onBuildConfirmed)
		{
			IsCashBuilding = isCashBuilding;
		}
	}
}
