using CIG;

public class CIGDecoration : CIGBuilding
{
	public override bool InfoRequiresFrequentRefresh
	{
		get
		{
			if (base.State == BuildingState.Preview || base.State != BuildingState.Normal || base.IsUpgrading)
			{
				return base.InfoRequiresFrequentRefresh;
			}
			return false;
		}
	}

	protected override bool CanShowUpgradeSign => false;
}
