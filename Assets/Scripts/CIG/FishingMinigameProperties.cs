namespace CIG
{
	[BalancePropertyClass("fishingMinigame", true)]
	public class FishingMinigameProperties : BaseProperties
	{
		private const string DurationSecondsKey = "durationSeconds";

		private const string MarkerPercentagePerSecondKey = "markerPercentagePerSecond";

		private const string GreenAreaStartSizePercentageKey = "greenAreaStartSizePercentage";

		private const string YellowAreaStartSizePercentageKey = "yellowAreaStartSizePercentage";

		private const string AreaPositionPaddingPercentageKey = "areaPositionPaddingPercentage";

		private const string AreaScaleMultiplierKey = "areaScaleMultiplier";

		[BalanceProperty("durationSeconds")]
		public float DurationSeconds
		{
			get;
		}

		[BalanceProperty("greenAreaStartSizePercentage")]
		public float GreenAreaStartSizePercentage
		{
			get;
		}

		[BalanceProperty("yellowAreaStartSizePercentage")]
		public float YellowAreaStartSizePercentage
		{
			get;
		}

		[BalanceProperty("areaPositionPaddingPercentage")]
		public float AreaPositionPaddingPercentage
		{
			get;
		}

		[BalanceProperty("areaScaleMultiplier")]
		public float AreaScaleMultiplier
		{
			get;
		}

		[BalanceProperty("markerPercentagePerSecond")]
		public float MarkerPercentagePerSecond
		{
			get;
		}

		public FishingMinigameProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			DurationSeconds = GetProperty("durationSeconds", 15f);
			MarkerPercentagePerSecond = GetProperty("markerPercentagePerSecond", 140f);
			GreenAreaStartSizePercentage = GetProperty("greenAreaStartSizePercentage", 30f);
			YellowAreaStartSizePercentage = GetProperty("yellowAreaStartSizePercentage", 8f);
			AreaPositionPaddingPercentage = GetProperty("areaPositionPaddingPercentage", 10f);
			AreaScaleMultiplier = GetProperty("areaScaleMultiplier", 0.93f);
		}
	}
}
