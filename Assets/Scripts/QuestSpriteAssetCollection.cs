using CIG;
using System;
using UnityEngine;

internal class QuestSpriteAssetCollection : DictionaryAssetCollection<QuestSpriteAssetCollection.QuestSprites, QuestSpriteType, QuestSpriteAssetCollection.QuestSprite, QuestSpriteAssetCollection>
{
	[Serializable]
	public class QuestSprites : SerializableDictionary
	{
	}

	[Serializable]
	public class QuestSprite
	{
		[SerializeField]
		private CompositeSpriteImage.CompositeSpriteData _largeSpriteData;

		[SerializeField]
		private CompositeSpriteImage.CompositeSpriteData _smallSpriteData;

		public CompositeSpriteImage.CompositeSpriteData LargeSpriteData => _largeSpriteData;

		public CompositeSpriteImage.CompositeSpriteData SmallSpriteData => _smallSpriteData;
	}
}
