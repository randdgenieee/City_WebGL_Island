using CIG;
using System;
using UnityEngine;

public sealed class SurfaceSpriteAssetCollection : DictionaryAssetCollection<SurfaceSpriteAssetCollection.SurfaceSprite, SurfaceType, SurfaceSpriteAssetCollection.SurfaceSprites, SurfaceSpriteAssetCollection>
{
	[Serializable]
	public class SurfaceSprite : SerializableDictionary
	{
	}

	[Serializable]
	public class SurfaceSprites
	{
		[SerializeField]
		private Sprite _icon;

		[SerializeField]
		private Sprite _background;

		[SerializeField]
		private Sprite _frame;

		[SerializeField]
		private Sprite _floor;

		public Sprite Icon => _icon;

		public Sprite Background => _background;

		public Sprite Frame => _frame;

		public Sprite Floor => _floor;
	}
}
