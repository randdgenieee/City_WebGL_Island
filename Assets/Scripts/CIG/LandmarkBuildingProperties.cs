using CIG.Translation;
using System.Collections.Generic;

namespace CIG
{
	[BalancePropertyClass("landmark", false)]
	[BalanceSortedArrayProperties("boostPercentage", true)]
	[BalanceSortedArrayProperties("boostTiles", true)]
	public class LandmarkBuildingProperties : BuildingProperties
	{
		private const string BoostPercentagePerLevelKey = "boostPercentage";

		private const string BoostTilesPerLevelKey = "boostTiles";

		[BalanceProperty("boostPercentage")]
		public List<int> BoostPercentagePerLevel
		{
			get;
		}

		[BalanceProperty("boostTiles")]
		public List<int> BoostTilesPerLevel
		{
			get;
		}

		public override ILocalizedString CategoryName => Localization.EmptyLocalizedString;

		public LandmarkBuildingProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			BoostPercentagePerLevel = GetProperty("boostPercentage", new List<int>());
			BoostTilesPerLevel = GetProperty("boostTiles", new List<int>());
		}

		public override List<BuildingProperty> GetShownProperties(bool preview)
		{
			List<BuildingProperty> shownProperties = base.GetShownProperties(preview);
			shownProperties.Add(BuildingProperty.BoostPercentage);
			shownProperties.Add(BuildingProperty.BoostReach);
			return shownProperties;
		}
	}
}
