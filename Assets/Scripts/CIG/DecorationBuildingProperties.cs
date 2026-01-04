using CIG.Translation;
using System.Collections.Generic;

namespace CIG
{
	[BalancePropertyClass("decoration", false)]
	public class DecorationBuildingProperties : BuildingProperties
	{
		public override ILocalizedString CategoryName => Localization.Key("shop_decorations");

		public DecorationBuildingProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
		}

		public override bool CanBeBuilt(int level, GameStats gameStats, BuildingWarehouseManager buildingWarehouseManager)
		{
			return true;
		}

		public override List<BuildingProperty> GetShownProperties(bool preview)
		{
			List<BuildingProperty> shownProperties = base.GetShownProperties(preview);
			shownProperties.Add(BuildingProperty.Happiness);
			return shownProperties;
		}
	}
}
