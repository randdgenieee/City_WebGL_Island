using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace CIG
{
    public static class Analytics
    {
        private const string EventLevelUp = "level_up";

        private const string EventLevelStats = "level_stats";

        private const string EventTutorialStarted = "tutorial_started";

        private const string EventTutorialStepReached = "tutorial_step_reached";

        private const string EventTutorialFinished = "tutorial_finished";

        private const string EventTutorialStopped = "tutorial_quit";

        private const string EventQuestAccepted = "quest_accepted";

        private const string EventQuestCompleted = "quest_completed";

        private const string EventQuestCompletedFirst24h = "quest_completed_first24h";

        private const string EventQuestCompletedFirst48h = "quest_completed_first48h";

        private const string EventQuestCompletedFirst72h = "quest_completed_first72h";

        private const string EventQuestRewardClaimed = "quest_reward_claimed";

        private const string EventIAPViewed = "iap_viewed";

        private const string EventIAPPaymentInitiated = "iap_payment_initated";

        private const string EventIAPPaymentCancelled = "iap_payment_cancelled";

        private const string EventIAPPaymentConfirmed = "iap_payment_confirmed";

        private const string EventIAPPaymentInvalid = "iap_payment_invalid";

        public const string EventSpeedup = "speedup";

        private const string EventSpeedupFirstSession = "session_1_speedup";

        private const string EventSpeedupWithGold = "speedup_gold";

        public const string EventSpeedupInTutorial = "speedup_in_tutorial";

        private const string EventVideoOpened = "video_opened";

        private const string EventVideoWatched = "video_watched";

        private const string EventVideoClicked = "video_clicked";

        private const string EventVideoCanceled = "video_canceled";

        public const string EventInterstitialOpened = "interstitial_Opened";

        public const string EventInterstitialWatched = "interstitial_watched";

        public const string EventInterstitialClicked = "interstitial_clicked";

        public const string EventSession = "session";

        private const string EventSessionStats = "session_stats";

        private const string EventSessionLength = "session_length";

        private const string EventSessionLengthFirst24h = "session_length_first24h";

        private const string EventSessionLengthFirst48h = "session_length_first48h";

        private const string EventSessionLengthFirst72h = "session_length_first72h";

        public const string EventDayPlayed = "day_played";

        public const string EventDailyBonusStreakCompleted = "daily_bonus_streak_completed";

        private const string EventSupplementalBuildingPurchased = "supplemental_building_purchased";

        private const string EventSupplementalBuildingPurchasedFirst24h = "supplemental_building_purchased_first24h";

        private const string EventSupplementalBuildingPurchasedFirst48h = "supplemental_building_purchased_first48h";

        private const string EventSupplementalBuildingPurchasedFirst72h = "supplemental_building_purchased_first72h";

        private const string EventGoldBuildingPurchased = "gold_building_purchased";

        private const string EventGoldBuildingInFirstSession = "session_1_gold_building_build";

        private const string EventGoldBuilingPurchasedFirst24h = "gold_building_purchased_first24h";

        private const string EventGoldBuilingPurchasedFirst48h = "gold_building_purchased_first48h";

        private const string EventGoldBuilingPurchasedFirst72h = "gold_building_purchased_first72h";

        public const string EventGiftcodeRedeemed = "giftcode_redeemed";

        private const string EventPurchaseExpansionWithGold = "purchase_expansion_with_gold";

        public const string EventPurchaseExpansionWithCash = "purchase_expansion_with_cash";

        private const string EventCurrencyConversionStarted = "currency_conversion_started";

        private const string EventPurchasedTreasureChestWithKeys = "purchase_treasure_chest_with_keys";

        private const string EventPurchasedTreasureChestWithoutKeys = "purchase_treasure_chest_without_keys";

        private const string EventWheelOfFortuneSpent = "wheel_of_fortune_spent";

        private const string EventWheelOfFortuneEarned = "wheel_of_fortune_earned";

        private const string EventBuildingPurchased = "purchase_building";

        private const string EventBuildingImmediateBuild = "building_immediate_build";

        private const string EventBuildingUpgradeStarted = "upgrade_building";

        private const string EventBuildingDemolished = "demolish_building";

        private const string EventBuildingMoved = "move_building";

        private const string EventBuildingToWarehouse = "building_to_warehouse";

        private const string EventBuildingFromWarehouse = "building_from_warehouse";

        public const string EventCashProfitCollected = "cash_profit_collected";

        private const string EventIslandPurchased = "island_purchased";

        private const string EventSecondIslandPurchased0Hours = "island_2_purchased_after_0_hours";

        private const string EventSecondIslandPurchased1Hour = "island_2_purchased_after_1_hour";

        private const string EventSecondIslandPurchased2Hours = "island_2_purchased_after_2_hours";

        private const string EventSecondIslandPurchased3Hours = "island_2_purchased_after_3_hours";

        private const string EventSecondIslandPurchased4PlusHours = "island_2_purchased_after_4_plus_hours";

        private const string EventThirdIslandPurchased0Hours = "island_3_purchased_after_0_hours";

        private const string EventThirdIslandPurchased1Hour = "island_3_purchased_after_1_hour";

        private const string EventThirdIslandPurchased2Hours = "island_3_purchased_after_2_hours";

        private const string EventThirdIslandPurchased3Hours = "island_3_purchased_after_3_hours";

        private const string EventThirdIslandPurchased4PlusHours = "island_3_purchased_after_4_plus_hours";

        private const string EventShopTabClicked = "shop_tab_clicked";

        private const string EventSSPTabClicked = "ssp_tab_clicked";

        private const string EventWalkerBalloonClicked = "walker_balloon_clicked";

        private const string EventAirshipSent = "airship_sent";

        private const string EventIslandOpened = "island_opened";

        private const string EventKeyDealPurchased = "key_deal_purchased";

        private const string EventOneTimeOfferBuildingPurchased = "one_time_offer_building_purchased";

        private const string EventOneTimeOfferTreasureChestPurchased = "one_time_offer_treasure_chest_purchased";

        public const string EventOneTimeOfferBuildingSeen = "one_time_offer_building_seen";

        public const string EventOneTimeOfferTreasureChestSeen = "one_time_offer_building_seen";

        public const string EventOneTimeOfferIAPIntentTreasureChest = "one_time_offer_iap_intent_treasurechest";

        private const string EventVisitedCurrencyShopWithin5Minutes = "visited_currency_shop_within_5m";

        private const string EventVisitedCurrencyShopWithin10Minutes = "visited_currency_shop_within_10m";

        private const string EventVisitedCurrencyShopWithin15Minutes = "visited_currency_shop_within_15m";

        private const string EventFirstExpansionBoughtWithGold = "first_expansion_bought_with_gold";

        private const string EventSecondExpansionBoughtWithGold = "second_expansion_bought_with_gold";

        private const string EventThirdExpansionBoughtWithGold = "third_expansion_bought_with_gold";

        private const string EventFirstExpansionBoughtInSession1 = "first_expansion_bought_in_session_1";

        private const string EventFirstExpansionBoughtInSession2 = "first_expansion_bought_in_session_2";

        private const string EventFirstExpansionBoughtInSession3 = "first_expansion_bought_in_session_3";

        private const string EventFirstExpansionBoughtInSession4 = "first_expansion_bought_in_session_4";

        private const string EventFirstExpansionBoughtInSession5Plus = "first_expansion_bought_in_session_5_plus";

        private const string EventFishingQuestStarted = "fishing_quest_started";

        private const string EventFishingQuestExpired = "fishing_quest_expired";

        private const string EventFishingQuestCollected = "fishing_quest_collected";

        private const string EventFishingMinigameStarted = "fishing_minigame_started";

        public const string EventExceptionReporterCrash = "exceptionreport";

        public const string EventCraneOfferStarted = "crane_offer_started";

        public const string EventCraneOfferIAPPurchased = "crane_offer_iap_purchased";

        private const string EventFriendGiftReceived = "friend_gift_received";

        private const string EventFriendGiftSent = "friend_gift_sent";

        public const string EventFriendRequestSentFriendCode = "friend_request_sent_friendcode";

        public const string EventFriendRequestSentSuggestion = "friend_request_sent_suggestion";

        public const string EventFriendRequestAccepted = "friend_request_accepted";

        public const string EventFriendRequestDeclined = "friend_request_declined";

        private const string EventExpansionWarehouseTutorialGroup = "expansion_warehouse_tutorial_group";

        private const string EventEligibleForExpansionWarehouseTutorial = "eligible_for_expansion_warehouse_tutorial";

        private const string EventStartBalanceGroup = "start_balance_group";

        private const string ParameterStartBalanceGroup = "abgroup";

        private const string EventSocialAuthentication = "social_authentication";

        private const string ParameterSocialAuthenticationEnabled = "enabled";

        private const string EventPopulation = "population_reached";

        private const string EventEmployees = "employees_reached";

        private const string EventRoadsChanged = "roads_changed";

        private const string EventDaysPlayedStreak = "days_played_streak";

        public const string EventNewsletterSubscription = "newsletter_subscription";

        private const string EventEngagement = "ss_engagement";

        public const string EventAppToForeground = "app_to_foreground";

        public const string EventAppToBackground = "app_to_background";

        private const string EventSettingsMusicChanged = "settings_music_changed";

        private const string EventSettingsSFXChanged = "settings_sfx_changed";

        private const string EventSettingsNotificationsChanged = "settings_notifications_changed";

        private const string EventSettingsLanguageChanged = "settings_language_changed";

        public static readonly KeyValuePair<long, string>[] ConversionEventSpeedup = new KeyValuePair<long, string>[4]
        {
            new KeyValuePair<long, string>(1L, "speedup_1"),
            new KeyValuePair<long, string>(5L, "speedup_5"),
            new KeyValuePair<long, string>(10L, "speedup_10"),
            new KeyValuePair<long, string>(20L, "speedup_20")
        };

        public static readonly KeyValuePair<long, string>[] ConversionEventExpansionPurchasedWithGold = new KeyValuePair<long, string>[4]
        {
            new KeyValuePair<long, string>(1L, "expansion_purchased_1"),
            new KeyValuePair<long, string>(5L, "expansion_purchased_5"),
            new KeyValuePair<long, string>(10L, "expansion_purchased_10"),
            new KeyValuePair<long, string>(20L, "expansion_purchased_20")
        };

        public static readonly KeyValuePair<long, string>[] ConversionEventSession1Level = new KeyValuePair<long, string>[3]
        {
            new KeyValuePair<long, string>(10L, "session_1_level_10"),
            new KeyValuePair<long, string>(15L, "session_1_level_15"),
            new KeyValuePair<long, string>(20L, "session_1_level_20")
        };

        public static readonly KeyValuePair<long, string>[] ConversionEventSession2Level = new KeyValuePair<long, string>[3]
        {
            new KeyValuePair<long, string>(10L, "session_2_level_10"),
            new KeyValuePair<long, string>(15L, "session_2_level_15"),
            new KeyValuePair<long, string>(20L, "session_2_level_20")
        };

        public static readonly KeyValuePair<long, string>[] ConversionEventSession1SpendGold = new KeyValuePair<long, string>[3]
        {
            new KeyValuePair<long, string>(10L, "session_1_spend_gold_10"),
            new KeyValuePair<long, string>(50L, "session_1_spend_gold_50"),
            new KeyValuePair<long, string>(100L, "session_1_spend_gold_100")
        };

        public static readonly KeyValuePair<long, string>[] ConversionEventSession2SpendGold = new KeyValuePair<long, string>[3]
        {
            new KeyValuePair<long, string>(10L, "session_2_spend_gold_10"),
            new KeyValuePair<long, string>(50L, "session_2_spend_gold_50"),
            new KeyValuePair<long, string>(100L, "session_2_spend_gold_100")
        };

        public static readonly KeyValuePair<long, string>[] ConversionEventSession1IAPPurchased = new KeyValuePair<long, string>[1]
        {
            new KeyValuePair<long, string>(1L, "session_1_iap_purchased_1")
        };

        public static readonly KeyValuePair<long, string>[] ConversionEventSession2IAPPurchased = new KeyValuePair<long, string>[1]
        {
            new KeyValuePair<long, string>(1L, "session_2_iap_purchased_1")
        };

        public static readonly KeyValuePair<long, string>[] ConversionEventSession3IAPPurchased = new KeyValuePair<long, string>[1]
        {
            new KeyValuePair<long, string>(1L, "session_3_iap_purchased_1")
        };

        public static readonly KeyValuePair<long, string>[] ConversionEventSession1SessionLength = new KeyValuePair<long, string>[3]
        {
            new KeyValuePair<long, string>(10L, "session_1_session_length_10"),
            new KeyValuePair<long, string>(25L, "session_1_session_length_25"),
            new KeyValuePair<long, string>(40L, "session_1_session_length_40")
        };

        public static readonly KeyValuePair<long, string>[] ConversionEventSession2SessionLength = new KeyValuePair<long, string>[3]
        {
            new KeyValuePair<long, string>(10L, "session_2_session_length_10"),
            new KeyValuePair<long, string>(25L, "session_2_session_length_25"),
            new KeyValuePair<long, string>(40L, "session_2_session_length_40")
        };

        public static readonly KeyValuePair<long, string>[] ConversionEventVideoWatched = new KeyValuePair<long, string>[2]
        {
            new KeyValuePair<long, string>(1L, "video_watched_1"),
            new KeyValuePair<long, string>(100L, "video_watched_100")
        };

        public static readonly KeyValuePair<long, string>[] ConversionEventVideoClicked = new KeyValuePair<long, string>[1]
        {
            new KeyValuePair<long, string>(1L, "video_clicked_1")
        };

        public static readonly KeyValuePair<long, string>[] ConversionEventInterstitialWatched = new KeyValuePair<long, string>[1]
        {
            new KeyValuePair<long, string>(1L, "interstitial_watched_1")
        };

        public static readonly KeyValuePair<long, string>[] ConversionEventInterstitialClicked = new KeyValuePair<long, string>[1]
        {
            new KeyValuePair<long, string>(1L, "interstitial_clicked_1")
        };

        public static readonly KeyValuePair<long, string>[] ConversionEventSpendGold = new KeyValuePair<long, string>[5]
        {
            new KeyValuePair<long, string>(25L, "spend_gold_25"),
            new KeyValuePair<long, string>(100L, "spend_gold_100"),
            new KeyValuePair<long, string>(500L, "spend_gold_500"),
            new KeyValuePair<long, string>(1000L, "spend_gold_1000"),
            new KeyValuePair<long, string>(5000L, "spend_gold_5000")
        };

        public static readonly KeyValuePair<long, string>[] ConversionEventDailyBonusStreakCompleted = new KeyValuePair<long, string>[1]
        {
            new KeyValuePair<long, string>(1L, "daily_bonus_streak_completed_1")
        };

        public static readonly KeyValuePair<long, string>[] ConversionEventSupplementalBuildingPurchased = new KeyValuePair<long, string>[1]
        {
            new KeyValuePair<long, string>(1L, "supplemental_building_purchased_1")
        };

        public static readonly KeyValuePair<long, string>[] ConversionEventSession1CurrencyMenuWatched = new KeyValuePair<long, string>[1]
        {
            new KeyValuePair<long, string>(5L, "session_1_currency_menu_watched_5")
        };

        public static readonly KeyValuePair<long, string>[] ConversionEventSession1Or2CurrencyMenuWatched = new KeyValuePair<long, string>[1]
        {
            new KeyValuePair<long, string>(5L, "session_1_or_2_currency_menu_watched_5")
        };

        public static readonly KeyValuePair<long, string>[] ConversionEventWeek1CurrencyMenuWatched = new KeyValuePair<long, string>[1]
        {
            new KeyValuePair<long, string>(10L, "week_1_currency_menu_watched_10")
        };

        private const string ParameterSessionCount = "session_count";

        private const string ParameterGoldBalance = "gold_balance";

        private const string ParameterBuildingCount = "building_count";

        private const string ParameterExpansionsBought = "expansions_bought";

        private const string ParameterLevel = "level";

        private const string ParameterPopulation = "population";

        private const string ParameterHappiness = "happiness";

        private const string ParameterJobs = "jobs";

        private const string ParameterMinutesPlayed = "minutes_played";

        private const string ParameterTutorialType = "type";

        private const string ParameterTutorialStep = "step";

        private const string ParameterQuestName = "quest";

        private const string ParameterIAPIdentifier = "iap_identifier";

        private const string ParameterIAPPaymentError = "iap_payment_error";

        private const string ParameterBuildingName = "building_name";

        private const string ParameterBuildingType = "building_type";

        private const string ParameterSurfaceType = "surface_type";

        private const string ParameterWarehouseSource = "warehouse_source";

        private const string ParameterChestType = "chest_type";

        private const string ParameterWoodenChestOpenedWithVideo = "opened_with_video";

        private const string ParameterFromCurrency = "from_currency";

        private const string ParameterFromAmount = "from_amount";

        private const string ParameterToCurrency = "to_currency";

        private const string ParameterToAmount = "to_amount";

        private const string ParameterOfferName = "offer_name";

        private const string ParameterChestCost = "chest_cost";

        private const string ParameterIslandName = "island_name";

        private const string ParameterTabName = "tab_name";

        private const string ParameterBalloonType = "balloon_type";

        private const string ParameterFromIsland = "from_island";

        private const string ParameterToIsland = "to_island";

        private const string ParameterSecondsSinceQuestStart = "seconds_since_quest_start";

        private const string ParameterCurrencyType = "currency_type";

        private const string ParameterCurrencyAmount = "currency_amount";

        private const string ParameterABGroup = "abgroup";

        private const string ParameterEligible = "eligible";

        private const string ParameterVideoSource = "video_source";

        private const string ParameterTotal = "total";

        private const string ParameterRoadsAdded = "added";

        private const string ParameterRoadsRemoved = "removed";

        public static readonly IReadOnlyDictionary<CurrenciesSpentReason, string> SpendItems = new Dictionary<CurrenciesSpentReason, string>
        {
            {
                CurrenciesSpentReason.Unknown,
                "unknown"
            },
            {
                CurrenciesSpentReason.SpeedupUpgrade,
                "speedup_upgrade"
            },
            {
                CurrenciesSpentReason.SpeedupBuild,
                "speedup_build"
            },
            {
                CurrenciesSpentReason.SpeedupAirship,
                "speedup_airship"
            },
            {
                CurrenciesSpentReason.SpeedupDemolish,
                "speedup_demolish"
            },
            {
                CurrenciesSpentReason.GoldBuilding,
                "gold_building"
            },
            {
                CurrenciesSpentReason.GoldBuildingInstant,
                "instant_construction"
            },
            {
                CurrenciesSpentReason.CashBuilding,
                "cash_building"
            },
            {
                CurrenciesSpentReason.CashBuildingInstant,
                "instant_construction"
            },
            {
                CurrenciesSpentReason.Expansion,
                "expansion"
            },
            {
                CurrenciesSpentReason.CashExchange,
                "cash_exchange"
            },
            {
                CurrenciesSpentReason.CraneHire,
                "crane_hired"
            },
            {
                CurrenciesSpentReason.BuildingUpgrade,
                "building_upgrade"
            },
            {
                CurrenciesSpentReason.BuildingUpgradeInstant,
                "immediate_upgrade"
            },
            {
                CurrenciesSpentReason.IslandUnlock,
                "unlock_island"
            },
            {
                CurrenciesSpentReason.CurrencyConversion,
                "currency_conversion"
            },
            {
                CurrenciesSpentReason.BuildingWarehouse,
                "building_warehouse_slot"
            },
            {
                CurrenciesSpentReason.KeyDeals,
                "key_deal"
            },
            {
                CurrenciesSpentReason.OneTimeOffer,
                "one_time_offer"
            },
            {
                CurrenciesSpentReason.TreasureChest,
                "treasure_chest"
            },
            {
                CurrenciesSpentReason.WheelOfFortune,
                "wheel_of_fortune"
            },
            {
                CurrenciesSpentReason.SpecialQuest,
                "special_quest"
            },
            {
                CurrenciesSpentReason.AirshipSend,
                "airship_send"
            },
            {
                CurrenciesSpentReason.CraneOffer,
                "crane_offer"
            },
            {
                CurrenciesSpentReason.CheatMenu,
                "cheat_menu"
            }
        };

        public static readonly IReadOnlyDictionary<CurrenciesEarnedReason, string> EarnItems = new Dictionary<CurrenciesEarnedReason, string>
        {
            {
                CurrenciesEarnedReason.Unknown,
                "unknown"
            },
            {
                CurrenciesEarnedReason.CurrencyConversion,
                "currency_conversion"
            },
            {
                CurrenciesEarnedReason.LevelUp,
                "level_up"
            },
            {
                CurrenciesEarnedReason.WalkerBalloon,
                "balloon"
            },
            {
                CurrenciesEarnedReason.BuildingConstruction,
                "building_construction"
            },
            {
                CurrenciesEarnedReason.BuildingUpgrade,
                "upgrade"
            },
            {
                CurrenciesEarnedReason.BuildingCollect,
                "building_collect"
            },
            {
                CurrenciesEarnedReason.ExpansionChest,
                "expansion_chest"
            },
            {
                CurrenciesEarnedReason.DailyReward,
                "daily_reward"
            },
            {
                CurrenciesEarnedReason.Quest,
                "quest_reward"
            },
            {
                CurrenciesEarnedReason.StarterDeal,
                "starter_deal"
            },
            {
                CurrenciesEarnedReason.WheelOfFortune,
                "wheel_of_fortune"
            },
            {
                CurrenciesEarnedReason.FriendIslandVisiting,
                "friend_island_visiting"
            },
            {
                CurrenciesEarnedReason.IAP,
                "iap"
            },
            {
                CurrenciesEarnedReason.Reward,
                "reward"
            },
            {
                CurrenciesEarnedReason.Fishing,
                "fishing"
            },
            {
                CurrenciesEarnedReason.FriendGift,
                "friend_gift"
            },
            {
                CurrenciesEarnedReason.CheatMenu,
                "cheat_menu"
            },
            {
                CurrenciesEarnedReason.TutorialGift,
                "tutorial_gift"
            },
            {
                CurrenciesEarnedReason.FlyingStartDeal,
                "flying_start_deal"
            }
        };

        private static readonly IReadOnlyDictionary<WarehouseSource, string> WarehouseSourceItems = new Dictionary<WarehouseSource, string>
        {
            {
                WarehouseSource.Unknown,
                "unknown"
            },
            {
                WarehouseSource.OnePointFive,
                "one_point_five"
            },
            {
                WarehouseSource.Chest,
                "chest"
            },
            {
                WarehouseSource.Keydeal,
                "keydeal"
            },
            {
                WarehouseSource.LandmarkShop,
                "landmark_shop"
            },
            {
                WarehouseSource.OneTimeOffer,
                "one_time_offer"
            },
            {
                WarehouseSource.Gift,
                "gift"
            },
            {
                WarehouseSource.CheatMenu,
                "cheat_menu"
            }
        };

        private static readonly IReadOnlyDictionary<VideoSource, string> VideoSourceItems = new Dictionary<VideoSource, string>
        {
            {
                VideoSource.Unknown,
                "unknown"
            },
            {
                VideoSource.CheatMenu,
                "cheat_menu"
            },
            {
                VideoSource.Interstitial,
                "interstitial"
            },
            {
                VideoSource.CurrencyConversionSpeedup,
                "currency_conversion_speedup"
            },
            {
                VideoSource.FreeKeys,
                "free_keys"
            },
            {
                VideoSource.OpenWoodenChest,
                "open_wooden_chest"
            },
            {
                VideoSource.DoubleWoodenChestReward,
                "double_wooden_chest_reward"
            },
            {
                VideoSource.DoubleDailyReward,
                "double_daily_reward"
            }
        };

        private const string UserPropertySaveGuid = "save_guid";

        private const string UserPropertyABGroup = "abgroup";

        private const string UserPropertyABGroupCollection = "abgroup_collection";

        private const string NotificationsProperty = "notifications";

        private const string UserPropertyCheater = "cheater";

        private const string UserPropertyCheaterLocalInvalidIap = "cheater_local_inval_iap";

        private const string UserPropertyDeveloper = "developer";

        private static readonly List<long> SessionStatEventSessionCounts = new List<long>
        {
            2L,
            3L,
            4L,
            5L,
            6L,
            10L,
            15L,
            25L
        };

        public static readonly List<long> SessionLengthsInMinutes = new List<long>
        {
            5L,
            10L,
            20L,
            25L,
            30L
        };

        public static bool TrySetSaveGuid(string saveGuid)
        {
            return false;
        }

        public static void SetABGroup(string abGroup)
        {
        }

        public static void SetABGroupCollection(string abGroup)
        {
        }

        public static void SetNotificationsEnabled(bool enabled)
        {
        }

        public static void SetCheaterFlag(int cheaterFlag)
        {
        }

        public static void SetCheaterDeveloperUser(string developerUser)
        {
        }

        public static void SpendVirtualCurrency(string currencyName, string itemName, long amount)
        {
            if (amount != 0L)
            {
                {
                    ValidateFirebaseAnalyticsState("SpendVirtualCurrency");
                }
            }
        }

        public static void EarnVirtualCurrency(string currencyName, string itemName, long amount, long currentTotal)
        {
            if (amount != 0L)
            {
                {
                    ValidateFirebaseAnalyticsState("EarnVirtualCurrency");
                }
            }
        }

        public static void LevelUp(int level)
        {
            {
                ValidateFirebaseAnalyticsState("LevelUp");
            }
        }

        public static void LevelStats(int level, int population, int happiness, int jobs, long minutesPlayed)
        {
            {
                ValidateFirebaseAnalyticsState("LevelStats");
            }
        }

        public static void SetScreenView(string screenName)
        {
            {
                ValidateFirebaseAnalyticsState("ScreenView");
            }
        }

        public static void TutorialStarted(TutorialType type)
        {
            LogEvent("tutorial_started", "type", type.ToString());
        }

        public static void TutorialStepReached(TutorialType type, int tutorialStep)
        {
            LogEvent("tutorial_step_reached", "type", type.ToString(), "step", tutorialStep);
        }

        public static void TutorialFinished(TutorialType type)
        {
            LogEvent("tutorial_finished", "type", type.ToString());
        }

        public static void TutorialStopped(TutorialType type)
        {
            LogEvent("tutorial_quit", "type", type.ToString());
        }

        public static void QuestAccepted(string questName)
        {
            {
                ValidateFirebaseAnalyticsState("QuestAccepted");
            }
        }

        public static void QuestCompleted(string questName, Currencies reward, double totalHoursSinceCleanGame)
        {
            LogEvent("quest_completed", "quest", questName, reward);
            if (totalHoursSinceCleanGame <= 24.0)
            {
                LogEvent("quest_completed_first24h", "quest", questName, reward);
            }
            if (totalHoursSinceCleanGame <= 48.0)
            {
                LogEvent("quest_completed_first48h", "quest", questName, reward);
            }
            if (totalHoursSinceCleanGame <= 72.0)
            {
                LogEvent("quest_completed_first72h", "quest", questName, reward);
            }
        }

        public static void QuestRewardClaimed(string questName)
        {
            {
                ValidateFirebaseAnalyticsState("QuestRewardClaimed");
            }
        }

        public static void IAPViewed(string iapIdentifier)
        {
            {
                ValidateFirebaseAnalyticsState("IAPViewed");
            }
        }

        public static void CurrencyMenuWatched(int totalMinutesSinceCleanGame, int totalDaysSinceCleanGame, int numberOfTimesPlayed, int oldValue, int newValue)
        {
            if (totalMinutesSinceCleanGame <= 5)
            {
                LogEvent("visited_currency_shop_within_5m");
            }
            if (totalMinutesSinceCleanGame <= 10)
            {
                LogEvent("visited_currency_shop_within_10m");
            }
            if (totalMinutesSinceCleanGame <= 15)
            {
                LogEvent("visited_currency_shop_within_15m");
            }
            switch (numberOfTimesPlayed)
            {
                case 1:
                    LogEventsWhenPassingThreshold(ConversionEventSession1CurrencyMenuWatched, oldValue, newValue);
                    LogEventsWhenPassingThreshold(ConversionEventSession1Or2CurrencyMenuWatched, oldValue, newValue);
                    break;
                case 2:
                    LogEventsWhenPassingThreshold(ConversionEventSession1Or2CurrencyMenuWatched, oldValue, newValue);
                    break;
            }
            if ((long)totalDaysSinceCleanGame <= 7L)
            {
                LogEventsWhenPassingThreshold(ConversionEventWeek1CurrencyMenuWatched, oldValue, newValue);
            }
        }

        public static void IAPPaymentInitiated(string iapIdentifier)
        {
            {
                ValidateFirebaseAnalyticsState("IAPPaymentInitiated");
            }
        }

        public static void IAPPaymentCancelled(string iapIdentifier, string error)
        {
            {
                ValidateFirebaseAnalyticsState("IAPPaymentCancelled");
            }
        }

        public static void IAPPaymentConfirmed(string iapIdentifier)
        {
            {
                ValidateFirebaseAnalyticsState("IAPPaymentConfirmed");
            }
        }

        public static void IAPPaymentInvalid(string iapIdentifier, bool cheaterLocalInvalidIap)
        {
            {
                ValidateFirebaseAnalyticsState("IAPPaymentInvalid");
            }
        }

        public static void SetFriendCode(string key)
        {
            {
                ValidateFirebaseAnalyticsState("SetFriendCode");
            }
        }

        public static void CurrencyConversionStarted(string fromCurrency, long fromAmount, string toCurrency, long toAmount)
        {
            {
                ValidateFirebaseAnalyticsState("CurrencyConversionStarted");
            }
        }

        public static void WoodenTreasureChestOpen(string chestTypeName, bool withVideo)
        {
            {
                ValidateFirebaseAnalyticsState("WoodenTreasureChestOpen");
            }
        }

        public static void TreasureChestPurchasedWithKeys(string chestTypeName, string costCurrencyName, long costAmount)
        {
            {
                ValidateFirebaseAnalyticsState("TreasureChestPurchasedWithKeys");
            }
        }

        public static void TreasureChestPurchasedWithoutKeys(string chestTypeName)
        {
            {
                ValidateFirebaseAnalyticsState("TreasureChestPurchasedWithoutKeys");
            }
        }

        public static void WheelOfFortuneSpent(string currency, long amount)
        {
            {
                ValidateFirebaseAnalyticsState("WheelOfFortuneSpent");
            }
        }

        public static void WheelOfFortuneEarned(string currency, long amount)
        {
            {
                ValidateFirebaseAnalyticsState("WheelOfFortuneEarned");
            }
        }

        public static void GoldBuildingPurchased(int numberOfTimesPlayed, int totalHoursSinceCleanGame)
        {
            LogEvent("gold_building_purchased");
            if (numberOfTimesPlayed == 1)
            {
                LogEvent("session_1_gold_building_build");
            }
            if (totalHoursSinceCleanGame <= 24)
            {
                LogEvent("gold_building_purchased_first24h");
            }
            if (totalHoursSinceCleanGame <= 48)
            {
                LogEvent("gold_building_purchased_first48h");
            }
            if (totalHoursSinceCleanGame <= 72)
            {
                LogEvent("gold_building_purchased_first72h");
            }
        }

        public static void SupplementBuildingPurchased(int totalHoursSinceCleanGame)
        {
            LogEvent("supplemental_building_purchased");
            if (totalHoursSinceCleanGame <= 24)
            {
                LogEvent("supplemental_building_purchased_first24h");
            }
            if (totalHoursSinceCleanGame <= 48)
            {
                LogEvent("supplemental_building_purchased_first48h");
            }
            if (totalHoursSinceCleanGame <= 72)
            {
                LogEvent("supplemental_building_purchased_first72h");
            }
        }

        public static void BuildingPurchased(string buildingName, string buildingType, string surfaceType, Currencies cost)
        {
            if (FirebaseManager.IsAvailable)
            {
                Currency currency = cost.GetCurrency(0);
                if (currency.IsValid)
                {
                }
            }
            else
            {
                ValidateFirebaseAnalyticsState("LogEvent('purchase_building')");
            }
        }

        public static void BuildingUpgradeStarted(string buildingName, long level, Currencies cost)
        {
            if (FirebaseManager.IsAvailable)
            {
                Currency currency = cost.GetCurrency(0);
                if (currency.IsValid)
                {
                }
            }
            else
            {
                ValidateFirebaseAnalyticsState("LogEvent('upgrade_building')");
            }
        }

        public static void BuildingDemolished(string buildingName)
        {
            LogEvent("demolish_building", "building_name", buildingName);
        }

        public static void BuildingMoved(string buildingName)
        {
            LogEvent("move_building", "building_name", buildingName);
        }

        public static void BuildingMovedToWarehouse(string buildingName, int level, WarehouseSource source)
        {
            if (FirebaseManager.IsAvailable)
            {
            }
            else
            {
                ValidateFirebaseAnalyticsState(string.Format("LogEvent('{0}')", "building_to_warehouse"));
            }
        }

        public static void BuildingMovedFromWarehouse(string buildingName, int level, WarehouseSource source)
        {
            if (FirebaseManager.IsAvailable)
            {
            }
            else
            {
                ValidateFirebaseAnalyticsState(string.Format("LogEvent('{0}')", "building_from_warehouse"));
            }
        }

        public static void BuildingImmediateBuild()
        {
            LogEvent("building_immediate_build");
        }

        public static void ShopTabClicked(string tabName)
        {
            LogEvent("shop_tab_clicked", "tab_name", tabName);
        }

        public static void SSPTabClicked(string tabName)
        {
            LogEvent("ssp_tab_clicked", "tab_name", tabName);
        }

        public static void WalkerRewardBalloonClicked(string balloonType, Currency reward)
        {
            LogEvent("walker_balloon_clicked", "balloon_type", balloonType, reward);
        }

        public static void WalkerExclamationBalloonClicked(string balloonType)
        {
            LogEvent("walker_balloon_clicked", "balloon_type", balloonType);
        }

        public static void IslandPurchased(string islandName, Currency cost, int islandsUnlocked, double hoursPlayed)
        {
            LogEvent("island_purchased", "island_name", islandName, cost);
            switch (islandsUnlocked)
            {
                case 2:
                    if (!0.0.Equals(hoursPlayed))
                    {
                        if (!1.0.Equals(hoursPlayed))
                        {
                            if (!2.0.Equals(hoursPlayed))
                            {
                                if (3.0.Equals(hoursPlayed))
                                {
                                    LogEvent("island_2_purchased_after_3_hours");
                                }
                                else
                                {
                                    LogEvent("island_2_purchased_after_4_plus_hours");
                                }
                            }
                            else
                            {
                                LogEvent("island_2_purchased_after_2_hours");
                            }
                        }
                        else
                        {
                            LogEvent("island_2_purchased_after_1_hour");
                        }
                    }
                    else
                    {
                        LogEvent("island_2_purchased_after_0_hours");
                    }
                    break;
                case 3:
                    if (!0.0.Equals(hoursPlayed))
                    {
                        if (!1.0.Equals(hoursPlayed))
                        {
                            if (!2.0.Equals(hoursPlayed))
                            {
                                if (3.0.Equals(hoursPlayed))
                                {
                                    LogEvent("island_3_purchased_after_3_hours");
                                }
                                else
                                {
                                    LogEvent("island_3_purchased_after_4_plus_hours");
                                }
                            }
                            else
                            {
                                LogEvent("island_3_purchased_after_2_hours");
                            }
                        }
                        else
                        {
                            LogEvent("island_3_purchased_after_1_hour");
                        }
                    }
                    else
                    {
                        LogEvent("island_3_purchased_after_0_hours");
                    }
                    break;
            }
        }

        public static void AirshipSent(string origin, string destination)
        {
            LogEvent("airship_sent", "from_island", origin, "to_island", destination);
        }

        public static void IslandOpened(string island)
        {
            LogEvent("island_opened", "island_name", island);
        }

        public static void KeyDealPurchased(string buildingName, Currencies cost)
        {
            LogEvent("key_deal_purchased", "building_name", buildingName, cost);
        }

        public static void FishingQuestStarted()
        {
            LogEvent("fishing_quest_started");
        }

        public static void FishingQuestExpired()
        {
            LogEvent("fishing_quest_expired");
        }

        public static void FishingMinigameStarted()
        {
            LogEvent("fishing_minigame_started");
        }

        public static void FishingRewardCollected(Currencies reward, long secondsSinceQuestStart)
        {
            LogEvent("fishing_quest_collected", "seconds_since_quest_start", secondsSinceQuestStart, reward);
        }

        public static void OneTimeOfferBuildingPurchased(string buildingName, Currency cost)
        {
            LogEvent("one_time_offer_building_purchased", "building_name", buildingName, cost);
        }

        public static void OneTimeOfferTreasureChestPurchased(string chestType, string cost)
        {
            LogEvent("one_time_offer_treasure_chest_purchased", "chest_type", chestType, "chest_cost", cost);
        }

        public static void LogOneTimeOfferView(string oneTimeOfferEvent, string offerObjectName, int playerLevel)
        {
            if (FirebaseManager.IsAvailable)
            {
            }
            else
            {
                ValidateFirebaseAnalyticsState("OneTimeOfferView");
            }
        }

        public static void SessionStats(long sessionCount, long level, long goldBalance, long buildingCount, long expansionsbought)
        {
            if (SessionStatEventSessionCounts.Contains(sessionCount))
            {
                if (FirebaseManager.IsAvailable)
                {
                }
                else
                {
                    ValidateFirebaseAnalyticsState("SessionStats");
                }
            }
        }

        public static void SessionLength(long sessionLengthInMinutes, int totalHoursSinceCleanGame)
        {
            {
                ValidateFirebaseAnalyticsState("SessionLength");
            }
        }

        public static void SpeedupWithGold(int numberOfTimesPlayed)
        {
            {
                ValidateFirebaseAnalyticsState("SpeedupWithGold");
            }
        }

        public static void VideoOpened(VideoSource source)
        {
            {
                ValidateFirebaseAnalyticsState("VideoOpened");
            }
        }

        public static void VideoWatched(int oldValue, int numberOfVideosWatched, VideoSource source)
        {
            {
                ValidateFirebaseAnalyticsState("VideoWatched");
            }
        }

        public static void VideoClicked(int oldValue, int numberOfVideosClicked, VideoSource source)
        {
            {
                ValidateFirebaseAnalyticsState("VideoClicked");
            }
        }

        public static void VideoCanceled(VideoSource source)
        {
            {
                ValidateFirebaseAnalyticsState("VideoCanceled");
            }
        }

        public static void ExpansionBoughtWithGold(int expansion, int oldValue, int numberOfExpansionsPurchasedWithGold)
        {
            LogEventsWhenPassingThreshold(ConversionEventExpansionPurchasedWithGold, oldValue, numberOfExpansionsPurchasedWithGold);
            LogEvent("purchase_expansion_with_gold");
            switch (expansion)
            {
                case 1:
                    LogEvent("first_expansion_bought_with_gold");
                    break;
                case 2:
                    LogEvent("second_expansion_bought_with_gold");
                    break;
                case 3:
                    LogEvent("third_expansion_bought_with_gold");
                    break;
            }
        }

        public static void FirstExpansionBought(int session)
        {
            switch (session)
            {
                case 1:
                    LogEvent("first_expansion_bought_in_session_1");
                    break;
                case 2:
                    LogEvent("first_expansion_bought_in_session_2");
                    break;
                case 3:
                    LogEvent("first_expansion_bought_in_session_3");
                    break;
                case 4:
                    LogEvent("first_expansion_bought_in_session_4");
                    break;
                default:
                    LogEvent("first_expansion_bought_in_session_5_plus");
                    break;
            }
        }

        public static void FriendGiftReceived(Currencies currencies)
        {
            LogEvent("friend_gift_received", currencies);
        }

        public static void FriendGiftSent(Currencies currencies)
        {
            LogEvent("friend_gift_sent", currencies);
        }

        public static void StartBalanceGroupSet(string abGroup)
        {
            LogEvent("start_balance_group", "abgroup", abGroup);
        }

        public static void ExpansionWarehouseTutorialGroupSet(string abGroup)
        {
            LogEvent("expansion_warehouse_tutorial_group", "abgroup", abGroup);
        }

        public static void EligibleForExpansionWarehouseTutorial(bool eligible)
        {
            LogEvent("eligible_for_expansion_warehouse_tutorial", "eligible", eligible.ToString());
        }

        public static void LogFriendcodeChanged(long oldFriendcode, long newFriendcode)
        {
            {
                ValidateFirebaseAnalyticsState("LogEvent('friendcode_changed_temp')");
            }
        }

        public static void SocialAuthenticationChanged(bool enabled)
        {
            LogEvent("social_authentication", "enabled", enabled.ToString());
        }

        public static void PopulationReached(long count)
        {
            LogEvent("population_reached", count);
        }

        public static void EmployeesReached(long count)
        {
            LogEvent("employees_reached", count);
        }

        public static void RoadsChanged(long added, long removed, long total)
        {
            {
                ValidateFirebaseAnalyticsState("LogEvent('roads_changed')");
            }
        }

        public static void DaysPlayedStreak(long streak)
        {
            LogEvent("days_played_streak", streak);
        }

        public static void Engagement(long milliseconds)
        {
            LogEvent("ss_engagement", milliseconds);
        }

        public static void SettingsMusicChanged(bool on)
        {
            LogEvent("settings_music_changed", on.ToString());
        }

        public static void SettingsSFXChanged(bool on)
        {
            LogEvent("settings_sfx_changed", on.ToString());
        }

        public static void SettingsLanguageChanged(string identifier)
        {
            LogEvent("settings_language_changed", identifier);
        }

        public static void LogEvent(string eventName)
        {
            {
                ValidateFirebaseAnalyticsState($"LogEvent('{eventName}')");
            }
        }

        public static void LogEventsWhenPassingThreshold(KeyValuePair<long, string>[] conversionDictionary, long oldValue, long newValue)
        {
            if (oldValue == newValue)
            {
                return;
            }
            int i = 0;
            for (int num = conversionDictionary.Length; i < num; i++)
            {
                KeyValuePair<long, string> keyValuePair = conversionDictionary[i];
                if (keyValuePair.Key > oldValue && keyValuePair.Key <= newValue)
                {
                    LogEvent(keyValuePair.Value);
                }
            }
        }

        private static void ValidateFirebaseAnalyticsState(string eventName)
        {
        }

        private static void LogEvent(string eventName, string parameterName, string parameterValue)
        {
            {
                ValidateFirebaseAnalyticsState($"LogEvent('{eventName}')");
            }
        }

        private static void LogEvent(string eventName, string parameterName, string parameterValue, string parameterName2, long parameterValue2)
        {
            {
                ValidateFirebaseAnalyticsState($"LogEvent('{eventName}')");
            }
        }

        private static void LogEvent(string eventName, string parameterName, string parameterValue, string parameterName2, string parameterValue2)
        {
            {
                ValidateFirebaseAnalyticsState($"LogEvent('{eventName}')");
            }
        }

        private static void LogEvent(string eventName, string parameterName, string parameterValue, Currencies currencies)
        {
            LogEvent(eventName, parameterName, parameterValue, currencies.GetCurrency(0));
        }

        private static void LogEvent(string eventName, string parameterName, string parameterValue, Currency currency)
        {
            {
                ValidateFirebaseAnalyticsState($"LogEvent('{eventName}')");
            }
        }

        private static void LogEvent(string eventName, string parameterName, long parameterValue, Currencies currencies)
        {
            LogEvent(eventName, parameterName, parameterValue, currencies.GetCurrency(0));
        }

        private static void LogEvent(string eventName, string parameterName, long parameterValue, Currency currency)
        {
            {
                ValidateFirebaseAnalyticsState("LogEvent('" + eventName + "')");
            }
        }

        private static void LogEvent(string eventName, Currencies currencies)
        {
            if (FirebaseManager.IsAvailable)
            {
                Currency currency = currencies.GetCurrency(0);
            }
            else
            {
                ValidateFirebaseAnalyticsState("LogEvent('" + eventName + "')");
            }
        }

        private static void LogEvent(string eventName, long parameterValue)
        {
            {
                ValidateFirebaseAnalyticsState("LogEvent('" + eventName + "')");
            }
        }

        private static void LogEvent(string eventName, string parameterValue)
        {
            {
                ValidateFirebaseAnalyticsState("LogEvent('" + eventName + "')");
            }
        }

        private static string GetWarehouseSourceString(WarehouseSource source)
        {
            if (WarehouseSourceItems.TryGetValue(source, out string value))
            {
                return value;
            }
            UnityEngine.Debug.LogErrorFormat("Can't find WarehouseSource string for WarehouseSource '{0}'", source);
            return WarehouseSourceItems[WarehouseSource.Unknown];
        }

        private static string GetVideoSourceString(VideoSource source)
        {
            if (VideoSourceItems.TryGetValue(source, out string value))
            {
                return value;
            }
            UnityEngine.Debug.LogErrorFormat("Can't find VideoSource string for VideoSource '{0}'", source);
            return VideoSourceItems[VideoSource.Unknown];
        }
    }
}
