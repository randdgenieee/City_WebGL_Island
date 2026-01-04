using CIG.Translation;
using UnityEngine;

namespace CIG
{
	public class QuestFactory
	{
		private const string BuyExpansionsQuest = "buyExpansions";

		private const string CollectQuest = "collect";

		private const string EmployeesQuest = "employees";

		private const string PopuplationQuest = "population";

		private const string PlayDaysStreakQuest = "playDaysStreak";

		private const string PlayFiveMinuteSessionsQuest = "playFiveMinuteSessions";

		private const string PlayMinutesQuest = "playMinutes";

		private const string PlaySessionsQuest = "playSessions";

		private const string SpeedupQuest = "speedup";

		private const string SpendGoldQuest = "spendGold";

		private const string SpendSilverKeysQuest = "spendSilverKeys";

		private const string SpendGoldKeysQuest = "spendGoldKeys";

		private const string BuildingConstructQuest = "buildingConstruct";

		private const string BuildingConstructGoldQuest = "buildingConstructGold";

		private const string BuildingUpgradeQuest = "buildingUpgrade";

		private const string LevelTenUpgradesQuest = "levelTenUpgrades";

		private const string BuildGrassBuildingsQuest = "buildGrassBuildings";

		private const string BuildBeachBuildingsQuest = "buildBeachBuildings";

		private const string BuildWaterBuildingsQuest = "buildWaterBuildings";

		private const string BuildSandBuildingsQuest = "buildSandBuildings";

		private const string BuildRockBuildingsQuest = "buildRockBuildings";

		private const string BuildMudBuildingsQuest = "buildMudBuildings";

		private const string BuildSnowBuildingsQuest = "buildSnowBuildings";

		private const string BuildCommercialBuildingsQuest = "buildCommercialBuildings";

		private const string BuildResidentialBuildingsQuest = "buildResidentialBuildings";

		private const string BuildCommunityBuildingsQuest = "buildCommunityBuildings";

		private const string BuildDecorationBuildingsQuest = "buildDecorationBuildings";

		private const string BuildRoadsQuest = "buildRoads";

		public const string FishingEventQuest = "fishingEvent";

		private const string AllDailyQuestsQuest = "allDailyQuests";

		private readonly GameState _gameState;

		private readonly GameStats _gameStats;

		public QuestFactory(GameState gameState, GameStats gameStats)
		{
			_gameState = gameState;
			_gameStats = gameStats;
		}

		public QuestTarget GetQuestTarget(StorageDictionary storage, QuestProperties properties)
		{
			OngoingQuestProperties properties2;
			if ((properties2 = (properties as OngoingQuestProperties)) != null)
			{
				return new OngoingQuestTarget(storage, properties2);
			}
			DailyQuestProperties properties3;
			if ((properties3 = (properties as DailyQuestProperties)) != null)
			{
				return new DailyQuestTarget(storage, properties3);
			}
			UnityEngine.Debug.LogErrorFormat("Cannot create QuestTarget for unknown QuestProperties type: {0}", properties.GetType().Name);
			return null;
		}

		public QuestProgressor GetQuestProgressor(StorageDictionary storage, string questType, QuestProgressOriginType progressOriginType, out QuestSpriteType questSpriteType, out QuestDescription localizationKey)
		{
			switch (questType)
			{
			case "buildGrassBuildings":
			{
				questSpriteType = QuestSpriteType.FacilitiesGrass;
				localizationKey = new QuestDescription("quest_build_buildings_on", true, Localization.Key("surfacetype_grass"));
				string key = "NumberOfBuildingsBuiltOnGrass";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.NumberOfBuildingsBuiltOnGrass), _gameStats.NumberOfBuildingsBuiltOnGrass);
			}
			case "buildBeachBuildings":
			{
				questSpriteType = QuestSpriteType.FacilitiesBeach;
				localizationKey = new QuestDescription("quest_build_buildings_on", true, Localization.Key("surfacetype_beach"));
				string key = "NumberOfBuildingsBuiltOnBeach";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.NumberOfBuildingsBuiltOnBeach), _gameStats.NumberOfBuildingsBuiltOnBeach);
			}
			case "buildWaterBuildings":
			{
				questSpriteType = QuestSpriteType.FacilitiesWater;
				localizationKey = new QuestDescription("quest_build_buildings_on", true, Localization.Key("surfacetype_water"));
				string key = "NumberOfBuildingsBuiltOnWater";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.NumberOfBuildingsBuiltOnWater), _gameStats.NumberOfBuildingsBuiltOnWater);
			}
			case "buildSandBuildings":
			{
				questSpriteType = QuestSpriteType.FacilitiesSand;
				localizationKey = new QuestDescription("quest_build_buildings_on", true, Localization.Key("surfacetype_sand"));
				string key = "NumberOfBuildingsBuiltOnSand";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.NumberOfBuildingsBuiltOnSand), _gameStats.NumberOfBuildingsBuiltOnSand);
			}
			case "buildRockBuildings":
			{
				questSpriteType = QuestSpriteType.FacilitiesRock;
				localizationKey = new QuestDescription("quest_build_buildings_on", true, Localization.Key("surfacetype_rock"));
				string key = "NumberOfBuildingsBuiltOnRock";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.NumberOfBuildingsBuiltOnRock), _gameStats.NumberOfBuildingsBuiltOnRock);
			}
			case "buildMudBuildings":
			{
				questSpriteType = QuestSpriteType.FacilitiesMud;
				localizationKey = new QuestDescription("quest_build_buildings_on", true, Localization.Key("surfacetype_mud"));
				string key = "NumberOfBuildingsBuiltOnMud";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.NumberOfBuildingsBuiltOnMud), _gameStats.NumberOfBuildingsBuiltOnMud);
			}
			case "buildSnowBuildings":
			{
				questSpriteType = QuestSpriteType.FacilitiesSnow;
				localizationKey = new QuestDescription("quest_build_buildings_on", true, Localization.Key("surfacetype_snow"));
				string key = "NumberOfBuildingsBuiltOnSnow";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.NumberOfBuildingsBuiltOnSnow), _gameStats.NumberOfBuildingsBuiltOnSnow);
			}
			case "buildCommercialBuildings":
			{
				questSpriteType = QuestSpriteType.FacilitiesCommercial;
				localizationKey = new QuestDescription("quest_build_commercial", true);
				string key = "NumberOfCommercialBuildingsBuilt";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.NumberOfCommercialBuildingsBuilt), _gameStats.NumberOfCommercialBuildingsBuilt);
			}
			case "buildResidentialBuildings":
			{
				questSpriteType = QuestSpriteType.FacilitiesResidential;
				localizationKey = new QuestDescription("quest_build_residential", true);
				string key = "NumberOfResidentialBuildingsBuilt";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.NumberOfResidentialBuildingsBuilt), _gameStats.NumberOfResidentialBuildingsBuilt);
			}
			case "buildCommunityBuildings":
			{
				questSpriteType = QuestSpriteType.FacilitiesCommunity;
				localizationKey = new QuestDescription("quest_build_community", true);
				string key = "NumberOfCommunityBuildingsBuilt";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.NumberOfCommunityBuildingsBuilt), _gameStats.NumberOfCommunityBuildingsBuilt);
			}
			case "buildDecorationBuildings":
			{
				questSpriteType = QuestSpriteType.FacilitiesDecoration;
				localizationKey = new QuestDescription("reward_build_decorations", true);
				string key = "NumberOfDecorationBuildingsBuilt";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.NumberOfDecorationBuildingsBuilt), _gameStats.NumberOfDecorationBuildingsBuilt);
			}
			case "buildRoads":
			{
				questSpriteType = QuestSpriteType.Roads;
				localizationKey = new QuestDescription("quest_build_roads", true);
				string key = "NumberOfRoadsBuilt";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.NumberOfRoadsBuilt), _gameStats.NumberOfRoadsBuilt);
			}
			case "buildingConstruct":
			{
				questSpriteType = QuestSpriteType.Facilities;
				localizationKey = new QuestDescription("reward_facilities_built", true);
				string key = "GlobalBuildingsBuilt";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.GlobalBuildingsBuilt), _gameStats.GlobalBuildingsBuilt);
			}
			case "buildingConstructGold":
			{
				questSpriteType = QuestSpriteType.FacilitiesGold;
				localizationKey = new QuestDescription("reward_facilities_gold", false);
				string key = "GlobalBuildingsBuiltWithGold";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.GlobalBuildingsBuiltWithGold), _gameStats.GlobalBuildingsBuiltWithGold);
			}
			case "buildingUpgrade":
			{
				questSpriteType = QuestSpriteType.Upgrades;
				localizationKey = new QuestDescription("reward_upgrades", true);
				string key = "NumberOfUpgrades";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.NumberOfUpgrades), _gameStats.NumberOfUpgrades);
			}
			case "buyExpansions":
			{
				questSpriteType = QuestSpriteType.Expansion;
				localizationKey = new QuestDescription("reward_buy_expansions", true);
				string key = "NumberOfExpansionsPurchased";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.NumberOfExpansionsPurchased), _gameStats.NumberOfExpansionsPurchased);
			}
			case "collect":
			{
				questSpriteType = QuestSpriteType.Cash;
				localizationKey = new QuestDescription("reward_profit_collected", true);
				string key = "TimesCollected";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.TimesCollected), _gameStats.TimesCollected);
			}
			case "levelTenUpgrades":
			{
				questSpriteType = QuestSpriteType.UpgradesMax;
				localizationKey = new QuestDescription("reward_upgrades_level10", true);
				string key = "NumberOfLevel10Upgrades";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.NumberOfLevel10Upgrades), _gameStats.NumberOfLevel10Upgrades);
			}
			case "playDaysStreak":
			{
				questSpriteType = QuestSpriteType.PlayTime;
				localizationKey = new QuestDescription("reward_play_days_streak", false);
				string key = "DaysPlayedStreak";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.DaysPlayedStreak), _gameStats.DaysPlayedStreak);
			}
			case "playSessions":
			{
				questSpriteType = QuestSpriteType.PlayTime;
				localizationKey = new QuestDescription("reward_play_times", true);
				string key = "NumberOfTimesPlayed";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.NumberOfTimesPlayed), _gameStats.NumberOfTimesPlayed);
			}
			case "speedup":
			{
				questSpriteType = QuestSpriteType.Speedup;
				localizationKey = new QuestDescription("reward_speedup$n", true);
				string key = "SpeedupsExecuted";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.SpeedupsExecuted), _gameStats.SpeedupsExecuted);
			}
			case "spendGold":
			{
				questSpriteType = QuestSpriteType.Gold;
				localizationKey = new QuestDescription("reward_spend_gold", false);
				string key = "GoldSpent";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.GoldSpent), _gameStats.GoldSpent);
			}
			case "spendSilverKeys":
			{
				questSpriteType = QuestSpriteType.SilverKeys;
				localizationKey = new QuestDescription("quest_spend_silverkeys", false);
				string key = "NumberOfSilverKeysSpent";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.SilverKeysSpent), _gameStats.SilverKeysSpent);
			}
			case "spendGoldKeys":
			{
				questSpriteType = QuestSpriteType.GoldKeys;
				localizationKey = new QuestDescription("quest_spend_goldkeys", false);
				string key = "NumberOfGoldKeysSpent";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.GoldKeysSpent), _gameStats.GoldKeysSpent);
			}
			case "allDailyQuests":
			{
				questSpriteType = QuestSpriteType.AllDailyQuests;
				localizationKey = new QuestDescription("complete_all_daily_quests", false);
				string key = "DailyQuestsCompletedToday";
				return new AbstractStateQuestProgressor<GameStats>(storage, _gameStats, key, GetStartValue(progressOriginType, _gameStats.DailyQuestsCompleted), _gameStats.DailyQuestsCompleted);
			}
			case "employees":
			{
				questSpriteType = QuestSpriteType.Employees;
				localizationKey = new QuestDescription("reward_employees", true);
				string key = "GlobalEmployees";
				return new AbstractStateQuestProgressor<GameState>(storage, _gameState, key, GetStartValue(progressOriginType, _gameState.GlobalEmployees), _gameState.GlobalEmployees);
			}
			case "playFiveMinuteSessions":
			{
				questSpriteType = QuestSpriteType.PlayTime;
				localizationKey = new QuestDescription("reward_play_times_5_minutes", true);
				string key = "TotalFiveMinuteSessions";
				return new AbstractStateQuestProgressor<GameState>(storage, _gameState, key, GetStartValue(progressOriginType, _gameState.TotalFiveMinuteSessions), _gameState.TotalFiveMinuteSessions);
			}
			case "playMinutes":
			{
				questSpriteType = QuestSpriteType.PlayTime;
				localizationKey = new QuestDescription("reward_minutes_played", true);
				string key = "TotalMinutesPlayed";
				return new AbstractStateQuestProgressor<GameState>(storage, _gameState, key, GetStartValue(progressOriginType, _gameState.TotalMinutesPlayed), _gameState.TotalMinutesPlayed);
			}
			case "population":
			{
				questSpriteType = QuestSpriteType.Population;
				localizationKey = new QuestDescription("reward_citizens", true);
				string key = "GlobalPopulation";
				return new AbstractStateQuestProgressor<GameState>(storage, _gameState, key, GetStartValue(progressOriginType, _gameState.GlobalPopulation), _gameState.GlobalPopulation);
			}
			case "fishingEvent":
				questSpriteType = QuestSpriteType.FishingEvent;
				localizationKey = new QuestDescription("fishing_quest_description", true);
				return new BalanceQuestProgressor(storage, _gameState, "Fish", 0L);
			default:
				UnityEngine.Debug.LogErrorFormat("Unkown quest type: {0}", questType);
				questSpriteType = QuestSpriteType.Employees;
				localizationKey = new QuestDescription(string.Empty, false);
				return null;
			}
		}

		private static TValue GetStartValue<TValue>(QuestProgressOriginType progressOriginType, TValue currentProgress)
		{
			switch (progressOriginType)
			{
			case QuestProgressOriginType.Zero:
				return currentProgress;
			case QuestProgressOriginType.Current:
				return default(TValue);
			default:
				UnityEngine.Debug.LogErrorFormat("Unknown {0}: {1}", typeof(QuestProgressOriginType).Name, progressOriginType);
				return default(TValue);
			}
		}
	}
}
