using CIG;
using CIG.Translation;
using System.Collections.Generic;
using UnityEngine;

public class RewardItemData
{
	private const float CurrencyMaxImageWidth = 112f;

	private const float BuildingMaxImageWidth = 300f;

	public BuildingProperties BuildingProperties
	{
		get;
	}

	public Sprite Sprite
	{
		get;
	}

	public Sprite SmallSprite
	{
		get;
	}

	public ILocalizedString Text
	{
		get;
	}

	public ParticleType? Particles
	{
		get;
	}

	public Clip? ParticleClip
	{
		get;
	}

	public Currency Price
	{
		get;
	}

	public float MaxImageWidth
	{
		get;
	}

	private RewardItemData(BuildingProperties buildingProperties, GameState gameState, GameStats gameStats, BuildingWarehouseManager buildingWarehouseManager)
	{
		BuildingProperties = buildingProperties;
		Sprite = SingletonMonobehaviour<BuildingsAssetCollection>.Instance.GetAsset(BuildingProperties.BaseKey).SpriteRenderer.sprite;
		Price = BuildingProperties.GetConstructionCost(gameState, gameStats, buildingWarehouseManager);
		MaxImageWidth = 300f;
		SmallSprite = null;
		Text = null;
		Particles = null;
		ParticleClip = null;
	}

	private RewardItemData(Sprite sprite, Sprite smallSprite, ILocalizedString text, ParticleType? particles = default(ParticleType?), Clip? particleClip = default(Clip?), float maxImageWidth = 112f)
	{
		Sprite = sprite;
		SmallSprite = smallSprite;
		Text = text;
		Particles = particles;
		ParticleClip = particleClip;
		MaxImageWidth = maxImageWidth;
		BuildingProperties = null;
		Price = Currency.Invalid;
	}

	public static List<RewardItemData> FromReward(Reward reward, GameState gameState, GameStats gameStats, BuildingWarehouseManager buildingWarehouseManager)
	{
		List<BuildingProperties> buildings = reward.Buildings;
		Currencies currencies = reward.Currencies;
		List<RewardItemData> list = new List<RewardItemData>();
		int i = 0;
		for (int count = buildings.Count; i < count; i++)
		{
			list.Add(new RewardItemData(buildings[i], gameState, gameStats, buildingWarehouseManager));
		}
		int j = 0;
		for (int keyCount = currencies.KeyCount; j < keyCount; j++)
		{
			Currency currency = currencies.GetCurrency(j);
			string name = currency.Name;
			int num = (int)currency.Value;
			if (num == 0)
			{
				UnityEngine.Debug.LogWarningFormat("Received a Currency {0} with value 0.", name);
				continue;
			}
			switch (name)
			{
			case "Crane":
				for (int l = 0; l < num; l++)
				{
					list.Add(new RewardItemData(SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.Crane), SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.Crane), Localization.Key("crane")));
				}
				break;
			case "Gold":
				list.Add(new RewardItemData(SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(currency, CurrencySpriteSize.Special), SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(currency, CurrencySpriteSize.Medium), Localization.Integer(num), ParticleType.TreasureChestGoldFountain, Clip.CurrencyFountain, 168f));
				break;
			case "Cash":
				list.Add(new RewardItemData(SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(currency, CurrencySpriteSize.Special), SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(currency, CurrencySpriteSize.Medium), Localization.Integer(num), ParticleType.TreasureChestCashFountain, Clip.CurrencyFountain, 152f));
				break;
			case "GoldKey":
				list.Add(new RewardItemData(SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(currency, CurrencySpriteSize.Special), SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(currency, CurrencySpriteSize.Large), Localization.Integer(num), ParticleType.TreasureChestGoldKeyFountain, Clip.CurrencyFountain));
				break;
			case "SilverKey":
				list.Add(new RewardItemData(SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(currency, CurrencySpriteSize.Special), SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(currency, CurrencySpriteSize.Large), Localization.Integer(num), ParticleType.TreasureChestSilverKeyFountain, Clip.CurrencyFountain));
				break;
			case "Token":
				list.Add(new RewardItemData(SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(currency, CurrencySpriteSize.Special), SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(currency, CurrencySpriteSize.Large), Localization.Integer(num), ParticleType.TreasureChestTokenFountain, Clip.CurrencyFountain));
				break;
			case "LevelUp":
			{
				int k = 0;
				for (int num2 = num; k < num2; k++)
				{
					list.Add(new RewardItemData(SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.LevelUp), SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.LevelUp), Localization.Key("iap.title.levels$1")));
				}
				break;
			}
			case "XP":
				list.Add(new RewardItemData(SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(currency, CurrencySpriteSize.Special), SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(currency, CurrencySpriteSize.Medium), Localization.Integer(num)));
				break;
			case "FishingRod":
			{
				Sprite asset = SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.FishingRodBalloonIcon);
				list.Add(new RewardItemData(asset, asset, Localization.Integer(num)));
				break;
			}
			default:
				UnityEngine.Debug.LogErrorFormat("Received a Currency {0} with value {1}, but it is not supported by RewardItemData, and thus is not shown in any UI using this data structure.", name, num);
				break;
			}
		}
		return list;
	}
}
