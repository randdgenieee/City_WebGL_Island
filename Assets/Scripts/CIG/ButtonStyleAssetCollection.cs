using System;

namespace CIG
{
	public sealed class ButtonStyleAssetCollection : DictionaryAssetCollection<ButtonStyleAssetCollection.ButtonStyleItem, ButtonStyleType, ButtonStyle, ButtonStyleAssetCollection>
	{
		[Serializable]
		public class ButtonStyleItem : SerializableDictionary
		{
			public ButtonStyleItem(ButtonStyleType key, ButtonStyle value)
			{
				_key = key;
				_value = value;
			}
		}
	}
}
