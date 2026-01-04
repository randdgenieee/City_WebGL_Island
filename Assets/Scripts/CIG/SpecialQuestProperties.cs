namespace CIG
{
	[BalancePropertyClass("fishingQuest", true)]
	public class SpecialQuestProperties : DailyQuestProperties
	{
		private const string DurationSecondsKey = "durationSeconds";

		private const string StartCostKey = "startCost";

		[BalanceProperty("durationSeconds")]
		public float DurationSeconds
		{
			get;
		}

		[BalanceProperty("startCost")]
		public Currencies StartCost
		{
			get;
		}

		public SpecialQuestProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			DurationSeconds = GetProperty("durationSeconds", 0f);
			StartCost = GetProperty("startCost", new Currencies());
		}
	}
}
