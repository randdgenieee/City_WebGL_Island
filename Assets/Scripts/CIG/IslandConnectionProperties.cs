namespace CIG
{
	[BalancePropertyClass("islandConnection", false)]
	public class IslandConnectionProperties : BaseProperties
	{
		private const string DurationKey = "travelDurationSeconds";

		private const string FromKey = "from";

		private const string ToKey = "to";

		[BalanceProperty("travelDurationSeconds")]
		public int Duration
		{
			get;
		}

		[BalanceProperty("from", ParseType = typeof(int))]
		public IslandId From
		{
			get;
		}

		[BalanceProperty("to", ParseType = typeof(int))]
		public IslandId To
		{
			get;
		}

		public IslandConnectionProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			Duration = GetProperty("travelDurationSeconds", 0);
			From = (IslandId)GetProperty("from", -1);
			To = (IslandId)GetProperty("to", -1);
		}
	}
}
