using CIG;

public class CIGCommunityBuilding : CIGBuilding
{
	public override Currency UpgradeCost => base.UpgradeCost.Multiply(_multipliers.GetMultiplier(MultiplierType.CommunityCashCostUpgrade), RoundingMethod.Nearest);

	public override Currency UpgradeInstantCost => base.UpgradeInstantCost.Multiply(_multipliers.GetMultiplier(MultiplierType.CommunityInstantGoldCostUpgrade), RoundingMethod.Nearest);

	public override bool InfoRequiresFrequentRefresh
	{
		get
		{
			if (base.IsUpgrading || CanActivate || base.State == BuildingState.Preview || base.State != BuildingState.Normal)
			{
				return base.InfoRequiresFrequentRefresh;
			}
			return false;
		}
	}
}
