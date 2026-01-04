using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class Properties
	{
		public const string PropertiesFileName = "cig5.objects";

		public const string TypeKey = "type";

		private const string AllDailyQuestsQuestBaseKey = "allDailyQuestsQuest";

		public const string AdMobInterstitialAdsPropertiesBaseKey = "admobInterstitials";

		public const string AdSequencePropertiesBaseKey = "adSequence";

		public const string CurrencyConversionsPropertiesBaseKey = "currencyConversions";

		public const string BuildingWarehousePropertiesBaseKey = "buildingWarehouse";

		public const string DailyRewardsPropertiesBaseKey = "dailyRewards";

		public const string FeatureFlagPropertiesBaseKey = "featureFlags";

		public const string FishingEventPropertiesBaseKey = "fishingEvent";

		public const string FishingMinigamePropertiesBaseKey = "fishingMinigame";

		public const string FishingQuestPropertiesBaseKey = "fishingQuest";

		public const string FlyingStartDealPropertiesBaseKey = "flyingStartDeal";

		public const string FriendsManagerPropertiesBaseKey = "friends";

		public const string ExpansionPropertiesBaseKey = "expansionCost";

		public const string InterstitialAdsPropertiesBaseKey = "interstitialAds";

		public const string IslandVisitingCurrencyBaseKey = "islandVisitingCurrency";

		public const string KeyDealsPropertiesBaseKey = "keyDeals";

		public const string NewsletterPropertiesBaseKey = "newsletter";

		public const string OneTimeOfferBuildingPropertiesBaseKey = "oneTimeOfferBuilding";

		public const string OneTimeOfferTreasureChestPropertiesBaseKey = "oneTimeOfferTreasureChest";

		public const string SilverKeyWalkerBalloonPropertiesBaseKey = "silverKeyBalloon";

		public const string TreasureChestManagerPropertiesBaseKey = "treasureChests";

		public const string VideoAds1PropertiesBaseKey = "videoAds1";

		public const string VideoAds2PropertiesBaseKey = "videoAds2";

		public const string VideoAds3PropertiesBaseKey = "videoAds3";

		public const string WheelOfFortunePropertiesBaseKey = "wheelOfFortune";

		public const string CurrencyConversionPropertiesTypeKey = "currencyConversion";

		public const string DailyRewardStreakPropertiesTypeKey = "dailyRewardStreak";

		public const string DailyQuestPropertiesTypeKey = "dailyQuest";

		public const string ExpansionSignPropertiesBaseKey = "BuySign";

		public const string ExpansionChestPropertiesBaseKey = "ExpansionChest";

		public const string IslandConnectionPropertiesTypeKey = "islandConnection";

		public const string IslandPropertiesTypeKey = "island";

		public const string LevelPropertiesTypeKey = "level";

		public const string OngoingQuestPropertiesTypeKey = "ongoingQuest";

		public const string QuestGroupPropertiesTypeKey = "questGroup";

		public const string StartBalancePropertiesTypeKey = "startBalance";

		public const string TreasureChestPropertiesTypeKey = "treasureChest";

		public const string WalkerBalloonPropertiesTypeKey = "walkerBalloon";

		public const string WheelOfFortuneStreakPropertiesTypeKey = "wheelOfFortuneStreak";

		public const string SaleManagerPropertiesBaseKey = "saleManager";

		public const string CraneOfferManagerPropertiesBaseKey = "craneOfferManager";

		public const string AirshipPlatformPropertiesTypeKey = "airshipPlatform";

		public const string CommercialBuildingPropertiesTypeKey = "commercial";

		public const string CommunityBuildingPropertiesTypeKey = "community";

		public const string DecorationBuildingPropertiesTypeKey = "decoration";

		public const string ExpansionChestPropertiesTypeKey = "expansionChest";

		public const string ExpansionSignPropertiesTypeKey = "expansionSign";

		public const string LandmarkBuildingPropertiesTypeKey = "landmark";

		public const string ResidentialBuildingPropertiesTypeKey = "residential";

		public const string RoadPropertiesTypeKey = "road";

		public const string SceneryPropertiesTypeKey = "scenery";

		public const string CraneOfferIAPPropertiesKey = "craneOfferIAP";

		public const string CraneOfferCurrencyPropertiesKey = "craneOfferCurrency";

		private readonly CombinedPropertiesDictionary _propsDict;

		private readonly Dictionary<string, BaseProperties> _propertiesCache;

		private readonly Dictionary<string, List<string>> _keysByType;

		public List<BuildingProperties> AllBuildingProperties
		{
			get;
		}

		public List<GridTileProperties> AllAirshipPlatforms
		{
			get;
		}

		public List<BuildingProperties> AllCommercialBuildings
		{
			get;
		}

		public List<BuildingProperties> AllCommunityBuildings
		{
			get;
		}

		public List<BuildingProperties> AllDecorationBuildings
		{
			get;
		}

		public List<BuildingProperties> AllLandmarkBuildings
		{
			get;
		}

		public List<BuildingProperties> AllResidentialBuildings
		{
			get;
		}

		public List<GridTileProperties> AllScenery
		{
			get;
		}

		public List<IslandProperties> AllIslandProperties
		{
			get;
		}

		public List<WalkerBalloonProperties> AllWalkerBalloonProperties
		{
			get;
		}

		public AdMobInterstitialAdsProperties AdMobInterstitialAdsProperties
		{
			get;
		}

		public AdSequenceProperties AdSequenceProperties
		{
			get;
			private set;
		}

		public BuildingWarehouseProperties BuildingWarehouseProperties
		{
			get;
		}

		public CurrencyConversionsProperties CurrencyConversionsProperties
		{
			get;
		}

		public DailyRewardsProperties DailyRewardsProperties
		{
			get;
		}

		public ExpansionProperties ExpansionProperties
		{
			get;
		}

		public FeatureFlagProperties FeatureFlagProperties
		{
			get;
		}

		public FishingEventProperties FishingEventProperties
		{
			get;
		}

		public FlyingStartDealProperties FlyingStartDealProperties
		{
			get;
		}

		public FriendsManagerProperties FriendsManagerProperties
		{
			get;
		}

		public GameStateProperties GameStateProperties
		{
			get;
		}

		public InterstitialAdsProperties InterstitialAdsProperties
		{
			get;
		}

		public IslandConnectionGraphProperties IslandConnectionGraphProperties
		{
			get;
		}

		public IslandVisitingCurrencyProperties IslandVisitingCurrencyProperties
		{
			get;
		}

		public KeyDealsProperties KeyDealsProperties
		{
			get;
		}

		public NewsletterProperties NewsletterProperties
		{
			get;
		}

		public OneTimeOfferProperties OneTimeOfferProperties
		{
			get;
		}

		public QuestsManagerProperties QuestsManagerProperties
		{
			get;
		}

		public TreasureChestManagerProperties TreasureChestManagerProperties
		{
			get;
		}

		public VideoAds1Properties VideoAds1Properties
		{
			get;
		}

		public VideoAds2Properties VideoAds2Properties
		{
			get;
		}

		public VideoAds3Properties VideoAds3Properties
		{
			get;
		}

		public WheelOfFortuneProperties WheelOfFortuneProperties
		{
			get;
		}

		public SaleManagerProperties SaleManagerProperties
		{
			get;
		}

		public CraneOfferManagerProperties CraneManagerOfferProperties
		{
			get;
		}

		public Properties(Dictionary<string, string> overrides)
		{
			_propsDict = new CombinedPropertiesDictionary("cig5.objects", new PropertiesDictionary(overrides));
			_propertiesCache = new Dictionary<string, BaseProperties>();
			_keysByType = new Dictionary<string, List<string>>();
			AdMobInterstitialAdsProperties = new AdMobInterstitialAdsProperties(_propsDict, "admobInterstitials");
			AdSequenceProperties = new AdSequenceProperties(_propsDict, "adSequence");
			BuildingWarehouseProperties = new BuildingWarehouseProperties(_propsDict, "buildingWarehouse");
			CurrencyConversionsProperties = new CurrencyConversionsProperties(_propsDict, "currencyConversions", GetProperties<CurrencyConversionProperties>(GetBaseKeysWithType("currencyConversion")));
			DailyRewardsProperties = new DailyRewardsProperties(_propsDict, "dailyRewards", GetProperties<DailyRewardStreakProperties>(GetBaseKeysWithType("dailyRewardStreak")));
			ExpansionProperties = new ExpansionProperties(_propsDict, "expansionCost");
			FeatureFlagProperties = new FeatureFlagProperties(_propsDict, "featureFlags");
			FishingEventProperties = new FishingEventProperties(_propsDict, "fishingEvent", new FishingMinigameProperties(_propsDict, "fishingMinigame"), new SpecialQuestProperties(_propsDict, "fishingQuest"));
			FlyingStartDealProperties = new FlyingStartDealProperties(_propsDict, "flyingStartDeal");
			FriendsManagerProperties = new FriendsManagerProperties(_propsDict, "friends");
			GameStateProperties = new GameStateProperties(GetProperties<LevelProperties>(GetBaseKeysWithType("level")), GetProperties<StartBalanceProperties>(GetBaseKeysWithType("startBalance")));
			InterstitialAdsProperties = new InterstitialAdsProperties(_propsDict, "interstitialAds");
			IslandConnectionGraphProperties = new IslandConnectionGraphProperties(GetProperties<IslandConnectionProperties>(GetBaseKeysWithType("islandConnection")));
			IslandVisitingCurrencyProperties = new IslandVisitingCurrencyProperties(_propsDict, "islandVisitingCurrency");
			KeyDealsProperties = new KeyDealsProperties(_propsDict, "keyDeals");
			NewsletterProperties = new NewsletterProperties(_propsDict, "newsletter");
			OneTimeOfferProperties = new OneTimeOfferProperties(new OneTimeOfferBuildingProperties(_propsDict, "oneTimeOfferBuilding"), new OneTimeOfferTreasureChestProperties(_propsDict, "oneTimeOfferTreasureChest"));
			OngoingQuestsManagerProperties ongoingQuestsManagerProperties = new OngoingQuestsManagerProperties(GetProperties<OngoingQuestProperties>(GetBaseKeysWithType("ongoingQuest")));
			DailyQuestsManagerProperties dailyQuestsManagerProperties = new DailyQuestsManagerProperties(GetProperties<DailyQuestProperties>("allDailyQuestsQuest"), GetProperties<DailyQuestProperties>(GetBaseKeysWithType("dailyQuest")), GetProperties<QuestGroupProperties>(GetBaseKeysWithType("questGroup")));
			QuestsManagerProperties = new QuestsManagerProperties(ongoingQuestsManagerProperties, dailyQuestsManagerProperties);
			TreasureChestManagerProperties = new TreasureChestManagerProperties(_propsDict, "treasureChests", GetProperties<TreasureChestProperties>(GetBaseKeysWithType("treasureChest")));
			VideoAds1Properties = new VideoAds1Properties(_propsDict, "videoAds1");
			VideoAds2Properties = new VideoAds2Properties(_propsDict, "videoAds2");
			VideoAds3Properties = new VideoAds3Properties(_propsDict, "videoAds3");
			WheelOfFortuneProperties = new WheelOfFortuneProperties(_propsDict, "wheelOfFortune", GetProperties<WheelOfFortuneStreakProperties>(GetBaseKeysWithType("wheelOfFortuneStreak")));
			SaleManagerProperties = new SaleManagerProperties(_propsDict, "saleManager");
			CraneManagerOfferProperties = new CraneOfferManagerProperties(_propsDict, "craneOfferManager", GetProperties<CraneOfferIAPProperties>(GetBaseKeysWithType("craneOfferIAP")), GetProperties<CraneOfferCurrencyProperties>(GetBaseKeysWithType("craneOfferCurrency")));
			AllIslandProperties = GetProperties<IslandProperties>(GetBaseKeysWithType("island"));
			AllWalkerBalloonProperties = GetProperties<WalkerBalloonProperties>(GetBaseKeysWithType("walkerBalloon"));
			AllAirshipPlatforms = GetProperties<GridTileProperties>(GetBaseKeysWithType("airshipPlatform"));
			AllCommercialBuildings = GetProperties<BuildingProperties>(GetBaseKeysWithType("commercial"));
			AllCommunityBuildings = GetProperties<BuildingProperties>(GetBaseKeysWithType("community"));
			AllDecorationBuildings = GetProperties<BuildingProperties>(GetBaseKeysWithType("decoration"));
			AllLandmarkBuildings = GetProperties<BuildingProperties>(GetBaseKeysWithType("landmark"));
			AllResidentialBuildings = GetProperties<BuildingProperties>(GetBaseKeysWithType("residential"));
			AllScenery = GetProperties<GridTileProperties>(GetBaseKeysWithType("scenery"));
			AllBuildingProperties = new List<BuildingProperties>();
			AllBuildingProperties.AddRange(AllCommercialBuildings);
			AllBuildingProperties.AddRange(AllCommunityBuildings);
			AllBuildingProperties.AddRange(AllDecorationBuildings);
			AllBuildingProperties.AddRange(AllLandmarkBuildings);
			AllBuildingProperties.AddRange(AllResidentialBuildings);
		}

		public T GetProperties<T>(string baseKey) where T : BaseProperties
		{
			if (_propertiesCache.TryGetValue(baseKey, out BaseProperties value) && value is T)
			{
				return (T)value;
			}
			if (!HasProperties(baseKey))
			{
				UnityEngine.Debug.LogErrorFormat("Properties object with base key '{0}' does not exist.", baseKey);
				return null;
			}
			if (!_propsDict.TryGetValue(baseKey, "type", out string value2))
			{
				UnityEngine.Debug.LogErrorFormat("Properties object with base key '{0}' is missing 'type' field.", baseKey);
				return null;
			}
			switch (value2)
			{
			case "currencyConversion":
				value = new CurrencyConversionProperties(_propsDict, baseKey);
				break;
			case "dailyRewardStreak":
				value = new DailyRewardStreakProperties(_propsDict, baseKey);
				break;
			case "dailyQuest":
				value = new DailyQuestProperties(_propsDict, baseKey);
				break;
			case "level":
				value = new LevelProperties(_propsDict, baseKey);
				break;
			case "islandConnection":
				value = new IslandConnectionProperties(_propsDict, baseKey);
				break;
			case "island":
				value = new IslandProperties(_propsDict, baseKey);
				break;
			case "ongoingQuest":
				value = new OngoingQuestProperties(_propsDict, baseKey);
				break;
			case "questGroup":
				value = new QuestGroupProperties(_propsDict, baseKey);
				break;
			case "treasureChest":
				value = new TreasureChestProperties(_propsDict, baseKey);
				break;
			case "walkerBalloon":
				value = ((!(baseKey == "silverKeyBalloon")) ? new WalkerBalloonProperties(_propsDict, baseKey) : new SilverKeyBalloonProperties(_propsDict, baseKey));
				break;
			case "wheelOfFortuneStreak":
				value = new WheelOfFortuneStreakProperties(_propsDict, baseKey);
				break;
			case "airshipPlatform":
				value = new GridTileProperties(_propsDict, baseKey);
				break;
			case "commercial":
				value = new CommercialBuildingProperties(_propsDict, baseKey);
				break;
			case "community":
				value = new CommunityBuildingProperties(_propsDict, baseKey);
				break;
			case "decoration":
				value = new DecorationBuildingProperties(_propsDict, baseKey);
				break;
			case "expansionChest":
				value = new ExpansionChestProperties(_propsDict, baseKey);
				break;
			case "expansionSign":
				value = new GridTileProperties(_propsDict, baseKey);
				break;
			case "landmark":
				value = new LandmarkBuildingProperties(_propsDict, baseKey);
				break;
			case "residential":
				value = new ResidentialBuildingProperties(_propsDict, baseKey);
				break;
			case "road":
				value = new GridTileProperties(_propsDict, baseKey);
				break;
			case "scenery":
				value = new GridTileProperties(_propsDict, baseKey);
				break;
			case "craneOfferIAP":
				value = new CraneOfferIAPProperties(_propsDict, baseKey);
				break;
			case "craneOfferCurrency":
				value = new CraneOfferCurrencyProperties(_propsDict, baseKey);
				break;
			case "startBalance":
				value = new StartBalanceProperties(_propsDict, baseKey);
				break;
			default:
				UnityEngine.Debug.LogError("Unknown type: " + baseKey + ".type = " + value2);
				return null;
			}
			_propertiesCache[baseKey] = value;
			return (T)value;
		}

		public bool HasProperties(string baseKey)
		{
			return _propsDict.HasBaseKey(baseKey);
		}

		public bool HasProperties(string baseKey, string type)
		{
			if (!HasProperties(baseKey))
			{
				return false;
			}
			if (!_propsDict.TryGetValue(baseKey, "type", out string value) || value != type)
			{
				return false;
			}
			return true;
		}

		private List<string> GetBaseKeysWithType(string type)
		{
			if (!_keysByType.ContainsKey(type))
			{
				_keysByType[type] = _propsDict.GetBaseKeysByKeyValue("type", type);
			}
			return _keysByType[type];
		}

		private List<T> GetProperties<T>(List<string> baseKeys) where T : BaseProperties
		{
			List<T> list = new List<T>();
			int count = baseKeys.Count;
			for (int i = 0; i < count; i++)
			{
				list.Add(GetProperties<T>(baseKeys[i]));
			}
			return list;
		}

		private List<T> GetProperties<T>(List<string> baseKeys, Predicate<T> pred) where T : BaseProperties
		{
			List<T> list = new List<T>();
			int count = baseKeys.Count;
			for (int i = 0; i < count; i++)
			{
				T properties = GetProperties<T>(baseKeys[i]);
				if (pred(properties))
				{
					list.Add(properties);
				}
			}
			return list;
		}
	}
}
