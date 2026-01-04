namespace CIG
{
	public abstract class VideoAdsPropertiesBase : BaseProperties
	{
		private const string UnlockSecondsKey = "unlockSeconds";

		private const string TimeoutSecondsKey = "timeoutSeconds";

		private const string TimespanDurationHoursKey = "timespanDurationHours";

		private const string MaxVideosPerTimespanKey = "maxVideosPerTimespan";

		[BalanceProperty("unlockSeconds")]
		public int UnlockSeconds
		{
			get;
			private set;
		}

		[BalanceProperty("timeoutSeconds")]
		public double TimeoutSeconds
		{
			get;
			private set;
		}

		[BalanceProperty("timespanDurationHours")]
		public int TimespanDurationHours
		{
			get;
			private set;
		}

		[BalanceProperty("maxVideosPerTimespan")]
		public int MaxVideosPerTimespan
		{
			get;
			private set;
		}

		public VideoAdsPropertiesBase(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			UnlockSeconds = GetProperty("unlockSeconds", 60);
			TimeoutSeconds = GetProperty("timeoutSeconds", 180.0);
			TimespanDurationHours = GetProperty("timespanDurationHours", 1);
			MaxVideosPerTimespan = GetProperty("maxVideosPerTimespan", 3);
		}
	}
}
