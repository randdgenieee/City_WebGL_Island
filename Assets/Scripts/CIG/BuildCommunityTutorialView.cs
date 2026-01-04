using CIG.Translation;

namespace CIG
{
	public class BuildCommunityTutorialView : BuildingTutorialView<BuildCommunityTutorial, CIGCommunityBuilding>
	{
		protected override ILocalizedString Text => Localization.Key("tutorial.community_building_attract_citizens_town");

		protected override ShopMenuTabs ShopMenuTab => ShopMenuTabs.Community;
	}
}
