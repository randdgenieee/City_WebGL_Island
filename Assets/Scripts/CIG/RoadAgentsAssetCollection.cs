using System;
using System.Collections.Generic;

namespace CIG
{
	public sealed class RoadAgentsAssetCollection : DictionaryAssetCollection<RoadAgentsAssetCollection.RoadAgentPrefabs, RoadType, List<RoadAgent>, RoadAgentsAssetCollection>
	{
		[Serializable]
		public class RoadAgentPrefabs : SerializableDictionary
		{
			public RoadAgentPrefabs(RoadType key, List<RoadAgent> value)
			{
				_key = key;
				_value = value;
			}
		}
	}
}
