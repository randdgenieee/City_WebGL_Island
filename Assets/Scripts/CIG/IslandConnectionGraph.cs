using System.Collections.Generic;

namespace CIG
{
	public class IslandConnectionGraph
	{
		private readonly List<IslandConnection> _allConnections = new List<IslandConnection>();

		private readonly Dictionary<IslandId, List<IslandConnection>> _islandConnections = new Dictionary<IslandId, List<IslandConnection>>();

		public IslandConnectionGraph(List<IslandConnectionProperties> connectionProperties)
		{
			int i = 0;
			for (int count = connectionProperties.Count; i < count; i++)
			{
				_allConnections.Add(new IslandConnection(connectionProperties[i]));
			}
			int j = 0;
			for (int count2 = _allConnections.Count; j < count2; j++)
			{
				IslandConnection islandConnection = _allConnections[j];
				if (_islandConnections.TryGetValue(islandConnection.From, out List<IslandConnection> value))
				{
					value.Add(islandConnection);
				}
				else
				{
					_islandConnections[islandConnection.From] = new List<IslandConnection>
					{
						islandConnection
					};
				}
				if (_islandConnections.TryGetValue(islandConnection.To, out value))
				{
					value.Add(islandConnection);
				}
				else
				{
					_islandConnections[islandConnection.To] = new List<IslandConnection>
					{
						islandConnection
					};
				}
			}
		}

		public List<IslandConnection> GetConnections(IslandId islandId)
		{
			if (_islandConnections.TryGetValue(islandId, out List<IslandConnection> value))
			{
				return value;
			}
			return new List<IslandConnection>();
		}

		public bool AreIslandsConnected(IslandId island1, IslandId island2)
		{
			return GetConnections(island1).Find((IslandConnection connection) => connection.To == island2) != null;
		}

		public int GetTravelTimeBetweenIslands(IslandId island1, IslandId island2)
		{
			return GetConnections(island1).Find((IslandConnection connection) => connection.To == island2)?.TravelDuration ?? (-1);
		}
	}
}
