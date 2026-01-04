namespace CIG
{
	public class IslandConnection
	{
		public int TravelDuration
		{
			get;
			private set;
		}

		public IslandId From
		{
			get;
			private set;
		}

		public IslandId To
		{
			get;
			private set;
		}

		public IslandConnection(IslandConnectionProperties properties)
		{
			To = properties.To;
			From = properties.From;
			TravelDuration = properties.Duration;
		}
	}
}
