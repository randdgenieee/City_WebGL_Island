using System;

namespace CIG
{
	public sealed class TMPTextStyleAssetCollection : DictionaryAssetCollection<TMPTextStyleAssetCollection.TMPTextStyleItem, TMPTextStyleType, TMPTextStyle, TMPTextStyleAssetCollection>
	{
		[Serializable]
		public class TMPTextStyleItem : SerializableDictionary
		{
			public TMPTextStyleItem(TMPTextStyleType key, TMPTextStyle value)
			{
				_key = key;
				_value = value;
			}
		}
	}
}
