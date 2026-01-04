using CIG;
using System;
using UnityEngine;

public sealed class UISpriteAssetCollection : DictionaryAssetCollection<UISpriteAssetCollection.UISprite, UISpriteType, Sprite, UISpriteAssetCollection>
{
	[Serializable]
	public class UISprite : SerializableDictionary
	{
	}

	public Sprite GetChestSprite(TreasureChestType chestType)
	{
		switch (chestType)
		{
		case TreasureChestType.Gold:
			return GetAsset(UISpriteType.GoldChest);
		case TreasureChestType.Platinum:
			return GetAsset(UISpriteType.PlatinumChest);
		case TreasureChestType.Silver:
			return GetAsset(UISpriteType.SilverChest);
		case TreasureChestType.Wooden:
			return GetAsset(UISpriteType.WoodenChest);
		default:
			UnityEngine.Debug.LogErrorFormat("No sprite available for chest type '{0}'.", chestType);
			return null;
		}
	}

	public Sprite GetWalkerBalloonSprite(WalkerBalloonType walkerBalloonType)
	{
		switch (walkerBalloonType)
		{
		case WalkerBalloonType.Upgrade:
		case WalkerBalloonType.Community:
		case WalkerBalloonType.Residential:
		case WalkerBalloonType.Unemployed:
		case WalkerBalloonType.Advisor:
			return GetAsset(UISpriteType.ExclamationBalloonIcon);
		case WalkerBalloonType.Cash:
			return GetAsset(UISpriteType.CashBalloonIcon);
		case WalkerBalloonType.XP:
			return GetAsset(UISpriteType.XpBalloonIcon);
		case WalkerBalloonType.SilverKey:
			return GetAsset(UISpriteType.SilverKeyBalloonIcon);
		case WalkerBalloonType.FishingRod:
			return GetAsset(UISpriteType.FishingRodBalloonIcon);
		default:
			UnityEngine.Debug.LogErrorFormat("No sprite available for walker balloon type '{0}'.", walkerBalloonType);
			return null;
		}
	}
}
