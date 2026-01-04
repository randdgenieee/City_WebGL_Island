using System;

namespace CIG
{
	public sealed class RoadPrefabsAssetCollection : DictionaryAssetCollection<RoadPrefabsAssetCollection.RoadPrefab, RoadType, Road, RoadPrefabsAssetCollection>
	{
		[Serializable]
		public class RoadPrefab : SerializableDictionary
		{
			public RoadPrefab(RoadType key, Road value)
			{
				_key = key;
				_value = value;
			}
		}
	}
}
