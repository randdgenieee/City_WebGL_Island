using CIG;
using System;
using UnityEngine;

public sealed class CurrencySpriteAssetCollection : DictionaryAssetCollection<CurrencySpriteAssetCollection.CurrencySpriteDictionary, CurrencyType, CurrencySpriteAssetCollection.CurrencySprites, CurrencySpriteAssetCollection>
{
	[Serializable]
	public class CurrencySpriteDictionary : SerializableDictionary
	{
	}

	[Serializable]
	public class CurrencySprites
	{
		[SerializeField]
		private Sprite _smallestSprite;

		[SerializeField]
		private Sprite _mediumSprite;

		[SerializeField]
		private Sprite _largeSprite;

		[SerializeField]
		private Sprite _specialSprite;

		public Sprite SmallestSprite => _smallestSprite;

		public Sprite MediumSprite => _mediumSprite;

		public Sprite LargeSprite => _largeSprite;

		public Sprite SpecialSprite => _specialSprite;
	}

	public Sprite GetCurrencySprite(Currency currency, CurrencySpriteSize size = CurrencySpriteSize.Smallest)
	{
		return GetCurrencySprite(currency.ToCurrencyType(), size);
	}

	public Sprite GetCurrencySprite(CurrencyType currencyType, CurrencySpriteSize size = CurrencySpriteSize.Smallest)
	{
		if (currencyType != CurrencyType.Unknown)
		{
			CurrencySprites asset = GetAsset(currencyType);
			switch (size)
			{
			case CurrencySpriteSize.Smallest:
				return asset.SmallestSprite;
			case CurrencySpriteSize.Medium:
				return asset.MediumSprite;
			case CurrencySpriteSize.Large:
				return asset.LargeSprite;
			case CurrencySpriteSize.Special:
				return asset.SpecialSprite;
			default:
				UnityEngine.Debug.LogError("No currency sprite available for size: " + size);
				return null;
			}
		}
		UnityEngine.Debug.LogError("No currency sprite available for currencyType: " + currencyType);
		return null;
	}
}
