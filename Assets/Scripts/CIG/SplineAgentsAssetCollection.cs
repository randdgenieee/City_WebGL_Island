using System;

namespace CIG
{
	public sealed class SplineAgentsAssetCollection : DictionaryAssetCollection<SplineAgentsAssetCollection.SplineAgentPrefab, SplineAgentType, SplineAgent, SplineAgentsAssetCollection>
	{
		[Serializable]
		public class SplineAgentPrefab : SerializableDictionary
		{
			public SplineAgentPrefab(SplineAgentType key, SplineAgent value)
			{
				_key = key;
				_value = value;
			}
		}
	}
}
