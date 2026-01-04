namespace CIG
{
	public class OneTimeOfferProperties
	{
		public OneTimeOfferBuildingProperties OneTimeOfferBuildingProperties
		{
			get;
			private set;
		}

		public OneTimeOfferTreasureChestProperties OneTimeOfferTreasureChestProperties
		{
			get;
			private set;
		}

		public OneTimeOfferProperties(OneTimeOfferBuildingProperties oneTimeOfferBuildingProperties, OneTimeOfferTreasureChestProperties oneTimeOfferTreasureChestProperties)
		{
			OneTimeOfferBuildingProperties = oneTimeOfferBuildingProperties;
			OneTimeOfferTreasureChestProperties = oneTimeOfferTreasureChestProperties;
		}
	}
}
