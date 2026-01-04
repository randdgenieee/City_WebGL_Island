using System;

namespace CIG
{
	public sealed class ShopFrameStyleAssetCollection : DictionaryAssetCollection<ShopFrameStyleAssetCollection.ShopFrameStyleItem, ShopFrameStyleType, ShopFrameStyle, ShopFrameStyleAssetCollection>
	{
		[Serializable]
		public class ShopFrameStyleItem : SerializableDictionary
		{
			public ShopFrameStyleItem(ShopFrameStyleType key, ShopFrameStyle value)
			{
				_key = key;
				_value = value;
			}
		}
	}
}
