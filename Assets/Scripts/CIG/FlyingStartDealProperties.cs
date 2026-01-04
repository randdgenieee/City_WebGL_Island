namespace CIG
{
	[BalancePropertyClass("flyingStartDeal", true)]
	public class FlyingStartDealProperties : BaseProperties
	{
		private const string DurationSecondsKey = "durationSeconds";

		private const string StartDelaySecondsKey = "startDelaySeconds";

		private const string EnabledKey = "enabled";

		[BalanceProperty("durationSeconds")]
		public int DurationSeconds
		{
			get;
		}

		[BalanceProperty("startDelaySeconds")]
		public int StartDelaySeconds
		{
			get;
		}

		[BalanceProperty("enabled")]
		public bool Enabled
		{
			get;
		}

		public FlyingStartDealProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			DurationSeconds = GetProperty("durationSeconds", 3600);
			StartDelaySeconds = GetProperty("startDelaySeconds", 10);
			Enabled = GetProperty("enabled", defaultValue: false);
		}
	}
}
