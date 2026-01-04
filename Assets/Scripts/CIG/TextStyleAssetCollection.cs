using System;

namespace CIG
{
	public sealed class TextStyleAssetCollection : DictionaryAssetCollection<TextStyleAssetCollection.TextStyleItem, TextStyleType, TextStyle, TextStyleAssetCollection>
	{
		[Serializable]
		public class TextStyleItem : SerializableDictionary
		{
			public TextStyleItem(TextStyleType key, TextStyle value)
			{
				_key = key;
				_value = value;
			}
		}
	}
}
