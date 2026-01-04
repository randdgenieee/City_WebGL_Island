using System;

namespace CIG
{
	public class BuildConfirmMovePopupRequest : BuildConfirmPopupRequest
	{
		public override bool IsCashBuilding => base.BuildingPrefab.WasBuiltWithCash;

		public BuildConfirmMovePopupRequest(CIGBuilding building, bool moveCameraToTarget, Action<CIGBuilding> onBuildConfirmed = null)
			: base(building, Currency.Invalid, isNewBuilding: false, isMoving: true, moveCameraToTarget, BuildFinishType.Normal, onBuildConfirmed)
		{
		}
	}
}
