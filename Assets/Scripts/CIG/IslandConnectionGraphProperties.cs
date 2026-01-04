using System.Collections.Generic;

namespace CIG
{
	public class IslandConnectionGraphProperties
	{
		public List<IslandConnectionProperties> IslandConnectionsProperties
		{
			get;
			private set;
		}

		public IslandConnectionGraphProperties(List<IslandConnectionProperties> islandConnectionProperties)
		{
			IslandConnectionsProperties = islandConnectionProperties;
		}
	}
}
