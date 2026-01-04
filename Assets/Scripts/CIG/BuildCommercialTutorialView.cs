using CIG.Translation;

namespace CIG
{
	public class BuildCommercialTutorialView : BuildingTutorialView<BuildCommercialTutorial, CIGCommercialBuilding>
	{
		protected override ILocalizedString Text => Localization.Key("tutorial_build_commercial");

		protected override ShopMenuTabs ShopMenuTab => ShopMenuTabs.Commercial;
	}
}
