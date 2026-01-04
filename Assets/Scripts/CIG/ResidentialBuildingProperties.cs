using CIG.Translation;
using System.Collections.Generic;

namespace CIG
{
	[BalancePropertyClass("residential", false)]
	[BalanceSortedArrayProperties("people", true)]
	public class ResidentialBuildingProperties : BuildingProperties
	{
		private const string PeoplePerLevelKey = "people";

		[BalanceProperty("people")]
		public List<int> PeoplePerLevel
		{
			get;
		}

		public override ILocalizedString CategoryName => Localization.Key("residential");

		public ResidentialBuildingProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			PeoplePerLevel = GetProperty("people", new List<int>());
		}

		public override List<BuildingProperty> GetShownProperties(bool preview)
		{
			List<BuildingProperty> shownProperties = base.GetShownProperties(preview);
			shownProperties.Add(BuildingProperty.People);
			return shownProperties;
		}
	}
}
