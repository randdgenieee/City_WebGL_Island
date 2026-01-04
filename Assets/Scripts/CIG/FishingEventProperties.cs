namespace CIG
{
	[BalancePropertyClass("fishingEvent", true)]
	public class FishingEventProperties : BaseProperties
	{
		private const string StartLocationsActiveKey = "startLocationsActive";

		private const string LocationsIntervalSecondsKey = "locationsIntervalSeconds";

		public FishingMinigameProperties MinigameProperties
		{
			get;
		}

		public SpecialQuestProperties QuestProperties
		{
			get;
		}

		[BalanceProperty("startLocationsActive")]
		public int StartLocationsActive
		{
			get;
		}

		[BalanceProperty("locationsIntervalSeconds")]
		public float LocationsIntervalSeconds
		{
			get;
		}

		public FishingEventProperties(PropertiesDictionary propsDict, string baseKey, FishingMinigameProperties minigameProperties, SpecialQuestProperties questProperties)
			: base(propsDict, baseKey)
		{
			MinigameProperties = minigameProperties;
			QuestProperties = questProperties;
			StartLocationsActive = GetProperty("startLocationsActive", 4);
			LocationsIntervalSeconds = GetProperty("locationsIntervalSeconds", 300f);
		}
	}
}
