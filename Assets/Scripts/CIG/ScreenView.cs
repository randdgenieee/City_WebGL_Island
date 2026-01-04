using SparkLinq;
using System.Collections.Generic;

namespace CIG
{
	public static class ScreenView
	{
		private const string ScreenNone = "none";

		public const string ScreenWelcomeScene = "welcome_scene";

		public const string ScreenTutorialDialog = "tutorial_dialog";

		public const string ScreenWorldMap = "worldmap";

		public const string ScreenWorldmapVisiting = "worldmap_visiting";

		public const string ScreenIslandFormat = "island_{0}";

		public const string ScreenIslandVisitingFormat = "island_visiting_{0}";

		public const string ScreenVideoAdColony = "video_adcolony";

		public const string ScreenVideoVungle = "video_vungle";

		public const string ScreenVideoAdMob = "video_admob";

		public const string ScreenInterstitialAdMob = "interstitial_admob";

		public const string ScreenPopupSettings = "settings";

		public const string ScreenPopupBuyExpansion = "buy_expansion";

		public const string ScreenPopupDailyReward = "daily_reward";

		public const string ScreenPopupDailyRewardCollect = "daily_reward_collect";

		public const string ScreenPopupLevelUp = "level_up";

		public const string ScreenPopupCrossPromo = "cross_promo";

		public const string ScreenPopupReceiveReward = "receive_currencies";

		public const string ScreenPopupKeyDeals = "key_deals";

		public const string ScreenPopupFishingMinigame = "fishing_minigame";

		public const string ScreenPopupWheelOfFortune = "wheel_of_fortune";

		public const string ScreenPopupMoreCashGold = "more_cash_gold";

		public const string ScreenPopupCraneOffer = "crane_offer";

		public const string ScreenPopupFlyingStartDeal = "flying_start_deal";

		public const string ScreenPopupUnavailableSurface = "unavailable_surface";

		public const string ScreenPopupWatchKeyVideoConfirm = "watch_key_video_confirm";

		public const string ScreenPopupExpansionChestLocked = "expansion_chest_locked";

		public const string ScreenPopupCityAdvisor = "city_advisor";

		public const string ScreenPopupCraneHire = "crane_hire";

		public const string ScreenPopupLanguageConfirm = "language_confirm";

		public const string ScreenPopupTutorialQuitConfirm = "tutorial_quit_confirm";

		public const string ScreenPopupPurchaseLoading = "purchase_loading";

		public const string ScreenPopupPurchaseFailed = "purchase_failed";

		public const string ScreenPopupPurchaseConsumed = "purchase_consumed";

		public const string ScreenPopupQuitGameConfirm = "quit_game_confirm";

		public const string ScreenPopupUpdateAvailable = "update_available";

		public const string ScreenPopupBuildingWarehouse = "building_warehouse";

		public const string ScreenPopupBuildingWarehouseBuySlotConfirm = "building_warehouse_buy_slot_confirm";

		public const string ScreenPopupRoadBuilding = "road_building";

		public const string ScreenPopupRoadBuildingConfirm = "road_building_confirm";

		public const string ScreenPopupCommunityWalkerBalloon = "community_walker_balloon";

		public const string ScreenPopupResidentialWalkerBalloon = "residential_walker_balloon";

		public const string ScreenPopupUnemployedWalkerBalloon = "unemployed_walker_balloon";

		public const string ScreenPopupUpgradeWalkerBalloon = "upgrade_walker_balloon";

		public const string ScreenPopupBuilding = "building";

		public const string ScreenPopupBuildConfirm = "build_confirm";

		public const string ScreenPopupBuildingDemolishConfirm = "building_demolish_confirm";

		public const string ScreenPopupBuildingCantUpgrade = "building_cant_upgrade";

		public const string ScreenPopupGSLogin = "gs_login";

		public const string ScreenPopupGSLoginSuccess = "gs_login_success";

		public const string ScreenPopupGSRegister = "gs_register";

		public const string ScreenPopupGSRegisterSuccess = "gs_register_success";

		public const string ScreenPopupGSError = "gs_error";

		public const string ScreenPopupLogoutConfirm = "logout_confirm";

		public const string ScreenPopupLinkConfirm = "link_confirm";

		public const string ScreenPopupLinkDevice = "link_device";

		public const string ScreenPopupLinkDeviceSuccess = "link_device_success";

		public const string ScreenPopupLinkGetCode = "link_get_code";

		public const string ScreenPopupLinkUseCode = "link_use_code";

		public const string ScreenPopupTreasureChest = "treasure_chest";

		public const string ScreenPopupTreasureChestContents = "treasure_chest_contents";

		public const string ScreenPopupBuyChestConfirm = "buy_chest_confirm";

		public const string ScreenPopupCloudStorageConflict = "cloud_storage_conflict";

		public const string ScreenPopupCloudStorageConflictConfirm = "cloudstorage_conflict_confirm";

		public const string ScreenPopupCloudStorageConflictGameVersion = "cloudstorage_conflict_game_version";

		public const string ScreenPopupCloudStorageConflictError = "cloudstorage_conflict_error";

		public const string ScreenPopupRatingRequest = "rating_request";

		public const string ScreenPopupRatingLike = "rating_like";

		public const string ScreenPopupRatingDislike = "rating_dislike";

		public const string ScreenPopupShopFormat = "shop_{0}";

		public const string ScreenPopupSSPMenuFormat = "ssp_{0}";

		public const string ScreenPopupFriendDeclineConfirm = "friend_decline_confirm";

		public const string ScreenPopupFriendRequestSentSuccess = "friend_request_sent_success";

		public const string ScreenPopupFriendInviteCodeError = "friend_invite_code_error";

		public const string ScreenPopupGiftError = "gift_error";

		public const string ScreenPopupNewsletterSuccess = "newsletter_success";

		public const string ScreenPopupNewsletterError = "newsletter_error";

		public const string ScreenPopupOneTimeOfferFormat = "one_time_offer_{0}";

		public const string ScreenPopupOneTimeOfferTreasureChest = "treasure_chest";

		public const string ScreenPopupOneTimeOfferBuilding = "building";

		public const string ScreenPopupOneTimeOfferConfirmClose = "one_time_offer_confirm_close";

		public const string ScreenPopupSpecialQuestFormat = "special_quest_{0}";

		public const string ScreenPopupSpecialQuestFishing = "fishing";

		public const string ScreenPopupSpeedupFormat = "speedup_{0}";

		public const string ScreenPopupSpeedupBuildingUpgrade = "building_upgrade";

		public const string ScreenPopupSpeedupBuildingConstruction = "building_construction";

		public const string ScreenPopupSpeedupBuildingDemolish = "building_demolish";

		public const string ScreenPopupSpeedupAirship = "airship";

		public const string ScreenPopupLeaderboardsFormat = "leaderboards_{0}_{1}";

		public const string ScreenPopupLeaderboardsLocal = "local";

		public const string ScreenPopupLeaderboardsGlobal = "global";

		public const string ScreenPopupLeaderboardsAroundMe = "around_me";

		public const string ScreenPopupLeaderboardsTop = "top";

		public const string ScreenPopupQuestFormat = "quests_{0}";

		public const string ScreenPopupQuestDaily = "daily";

		public const string ScreenPopupQuestOngoing = "ongoing";

		private static readonly List<string> _screenViews = new List<string>();

		private static string _bottomScreenView;

		private static string _currentScreenView;

		public static void SetBottomScreenView(string screenView)
		{
			_bottomScreenView = screenView;
			UpdateCurrentScreenView();
		}

		public static void RemoveBottomScreenView(string screenView)
		{
			if (_bottomScreenView == screenView)
			{
				_bottomScreenView = null;
			}
		}

		public static void PushScreenView(string screenName)
		{
			_screenViews.Add(screenName);
			UpdateCurrentScreenView();
		}

		public static void UpdateScreenView(string oldScreenName, string newScreenName)
		{
			_screenViews.Remove(oldScreenName);
			PushScreenView(newScreenName);
		}

		public static void PopScreenView(string screenName)
		{
			if (_screenViews.Remove(screenName))
			{
				UpdateCurrentScreenView();
			}
		}

		private static void UpdateCurrentScreenView()
		{
			if (_screenViews.Count > 0)
			{
				SetScreenView(_screenViews.Last());
			}
			else if (!string.IsNullOrEmpty(_bottomScreenView))
			{
				SetScreenView(_bottomScreenView);
			}
			else
			{
				SetScreenView("none");
			}
		}

		private static void SetScreenView(string screenName)
		{
			if (_currentScreenView != screenName)
			{
				_currentScreenView = screenName;
				Analytics.SetScreenView(screenName);
			}
		}
	}
}
