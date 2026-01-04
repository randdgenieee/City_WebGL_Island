using System;
using UnityEngine;

namespace CIG
{
	public sealed class IslandsAssetCollection : DictionaryAssetCollection<IslandsAssetCollection.Islands, IslandId, IslandsAssetCollection.IslandPrefabs, IslandsAssetCollection>
	{
		[Serializable]
		public class Islands : SerializableDictionary
		{
		}

		[Serializable]
		public class IslandPrefabs
		{
			[SerializeField]
			private IslandBootstrapper _normalPrefab;

			[SerializeField]
			private ReadOnlyIslandBootstrapper _readOnlyPrefab;

			public IslandBootstrapper NormalPrefab => _normalPrefab;

			public ReadOnlyIslandBootstrapper ReadOnlyPrefab => _readOnlyPrefab;
		}
	}
}
