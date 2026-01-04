using CIG;
using System;
using UnityEngine;

public sealed class CurrencyPackSpriteAssetCollection : DictionaryAssetCollection<CurrencyPackSpriteAssetCollection.CurrencyPackSprite, string, Sprite, CurrencyPackSpriteAssetCollection>
{
	[Serializable]
	public class CurrencyPackSprite : SerializableDictionary
	{
	}
}
