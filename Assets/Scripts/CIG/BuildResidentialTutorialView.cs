using CIG.Translation;

namespace CIG
{
	public class BuildResidentialTutorialView : BuildingTutorialView<BuildResidentialTutorial, CIGResidentialBuilding>
	{
		protected override ILocalizedString Text => Localization.Key("tutorial.first_residential_building");

		protected override ShopMenuTabs ShopMenuTab => ShopMenuTabs.Residential;
	}
}
