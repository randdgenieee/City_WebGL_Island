using CIG.Translation;
using System.Collections.Generic;

namespace CIG
{
	[BalancePropertyClass("community", false)]
	public class CommunityBuildingProperties : BuildingProperties
	{
		public override ILocalizedString CategoryName => Localization.Key("shop_community");

		public CommunityBuildingProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
		}

		public override List<BuildingProperty> GetShownProperties(bool preview)
		{
			List<BuildingProperty> shownProperties = base.GetShownProperties(preview);
			shownProperties.Add(BuildingProperty.Happiness);
			return shownProperties;
		}
	}
}
