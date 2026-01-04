namespace CIG
{
	public class Game
	{
		private const string ArcadeManagerStorageKey = "MinigamesManager";

		private const string OneTimeOfferStorageKey = "OneTimeOfferManager";

		private const string ServerGiftsStorageKey = "ServerGifts";

		private const string TreasureChestManagerStorageKey = "TreasureChestManager";

		private const string ExpansionCostManagerStorageKey = "ExpansionCostManager";

		private const string IslandVisitingManagerStorageKey = "OtherIslandsData";

		private const string WorldMapStorageKey = "WorldMap";

		private const string BuildingWarehouseManagerStorageKey = "BuildingWarehouseManager";

		private const string FriendsManagerStorageKey = "FriendsManager";

		private const string LikeRegistrarStorageKey = "LikeRegistrar";

		private const string CraneManagerStorageKey = "CraneManager";

		private const string BuildingsManagerStorageKey = "BuildingsManager";

		private const string IslandsManagerStorageKey = "IslandsManager";

		private const string IslandVisitingTokenManagerStorageKey = "IslandVisitingTokenManager";

		private const string KeyDealsManagerStorageKey = "KeyDealsManager";

		private const string TutorialManagerStorageKey = "TutorialManager";

		private const string DailyRewardsManagerStorageKey = "DailyRewardsManager";

		private const string QuestsManagerStorageKey = "QuestsManager";

		private const string GameStatsStorageKey = "GameStats";

		private const string GameStateStorageKey = "GameState";

		private const string SaleManagerStorageKey = "SaleManager";

		private const string RatingRequesterStorageKey = "RatingRequester";

		private const string NewsletterManagerStorageKey = "NewsletterManager";

		private const string TimingStorageKey = "Timing";

		private const string AdProviderPoolStorageKey = "AdProviderPool";

		private const string VideoAds1ManagerStorageKey = "VideoAds1Manager";

		private const string VideoAds2ManagerStorageKey = "VideoAds2Manager";

		private const string InterstitialAdsManagerStorageKey = "InterstitialAdsManager";

		private const string CrossPromoStorageKey = "CrossPromo";

		private const string LocalNotificationManagerStorageKey = "LocalNotificationManager";

		private const string FishingEventStorageKey = "FishingEvent";

		private const string CraneOfferManagerStorageKey = "CraneOfferManager";

		private const string FlyingStartDealStorageKey = "FlyingStartDeal";

		public Properties Properties
		{
			get;
		}

		public RoutineRunner GameRoutineRunner
		{
			get;
		}

		public Timing Timing
		{
			get;
		}

		public GameStats GameStats
		{
			get;
		}

		public GameState GameState
		{
			get;
		}

		public CityAdvisor CityAdvisor
		{
			get;
		}

		public TutorialManager TutorialManager
		{
			get;
		}

		public WorldMap WorldMap
		{
			get;
		}

		public IslandVisitingManager IslandVisitingManager
		{
			get;
		}

		public IslandsManager IslandsManager
		{
			get;
		}

		public ExpansionCostManager ExpansionCostManager
		{
			get;
		}

		public IslandVisitingCurrencyManager IslandVisitingCurrencyManager
		{
			get;
		}

		public PopupManager PopupManager
		{
			get;
		}

		public CurrencyConversionManager CurrencyConversionManager
		{
			get;
		}

		public BuildingWarehouseManager BuildingWarehouseManager
		{
			get;
		}

		public BuildingsManager BuildingsManager
		{
			get;
		}

		public ServerGifts ServerGifts
		{
			get;
		}

		public SaleManager SaleManager
		{
			get;
		}

		public FriendsManager FriendsManager
		{
			get;
		}

		public LeaderboardsManager LeaderboardsManager
		{
			get;
		}

		public LikeRegistrar LikeRegistrar
		{
			get;
		}

		public DailyRewardsManager DailyRewardsManager
		{
			get;
		}

		public KeyDealsManager KeyDealsManager
		{
			get;
		}

		public OneTimeOfferManager OneTimeOfferManager
		{
			get;
		}

		public TreasureChestManager TreasureChestManager
		{
			get;
		}

		public LocalNotificationManager LocalNotificationManager
		{
			get;
		}

		public ComeBackNotificationsManager ComeBackNotificationsManager
		{
			get;
		}

		public CraneManager CraneManager
		{
			get;
		}

		public QuestsManager QuestsManager
		{
			get;
		}

		public ArcadeManager ArcadeManager
		{
			get;
		}

		public AdProviderPool AdProviderPool
		{
			get;
		}

		public VideoAds1Manager VideoAds1Manager
		{
			get;
		}

		public VideoAds2Manager VideoAds2Manager
		{
			get;
		}

		public VideoAds3Manager VideoAds3Manager
		{
			get;
		}

		public InterstitialAdsManager InterstitialAdsManager
		{
			get;
		}

		public RatingRequester RatingRequester
		{
			get;
		}

		public NewsletterManager NewsletterManager
		{
			get;
		}

		public CriticalProcesses CriticalProcesses
		{
			get;
		}

		public SessionManager SessionManager
		{
			get;
		}

		public CrossPromoPopupScheduler CrossPromoPopupScheduler
		{
			get;
		}

		public FishingEvent FishingEvent
		{
			get;
		}

		public CraneOfferManager CraneOfferManager
		{
			get;
		}

		public FlyingStartDealManager FlyingStartDealManager
		{
			get;
		}

		public EngagementTracker EngagementTracker
		{
			get;
		}

		public Game(StorageDictionary storage, Device device, GameServer gameServer, CIGGameVersion firstVersion)
		{
			TutorialIntermediary tutorialIntermediary = new TutorialIntermediary();
			GameRoutineRunner = new RoutineRunner("GameRoutineRunner");
			Properties = new Properties(gameServer.OverriddenGameProperties);
			Timing = new Timing(storage.GetStorageDict("Timing"));
			CriticalProcesses = new CriticalProcesses();
			SessionManager = new SessionManager();
			PopupManager = new PopupManager();
			GameState = new GameState(storage.GetStorageDict("GameState"), gameServer.WebService.Multipliers, PopupManager, GameRoutineRunner, Properties.GameStateProperties);
			GameStats = new GameStats(storage.GetStorageDict("GameStats"), GameState);
			LocalNotificationManager = new LocalNotificationManager(storage.GetStorageDict("LocalNotificationManager"), device.Settings, GameState, gameServer.WebService, SessionManager);
			AdProviderPool = new AdProviderPool(storage.GetStorageDict("AdProviderPool"), GameRoutineRunner, device.User, CriticalProcesses, GameStats, Properties.AdSequenceProperties, Properties.AdMobInterstitialAdsProperties);
			VideoAds1Manager = new VideoAds1Manager(storage.GetStorageDict("VideoAds1Manager"), AdProviderPool, CriticalProcesses, PopupManager, GameRoutineRunner, Properties.VideoAds1Properties);
			VideoAds2Manager = new VideoAds2Manager(storage.GetStorageDict("VideoAds2Manager"), AdProviderPool, CriticalProcesses, GameRoutineRunner, Properties.VideoAds2Properties);
			VideoAds3Manager = new VideoAds3Manager(storage.GetStorageDict("VideoAds2Manager"), AdProviderPool, CriticalProcesses, GameRoutineRunner, Properties.VideoAds3Properties);
			SaleManager = new SaleManager(storage.GetStorageDict("SaleManager"), gameServer.WebService, GameRoutineRunner, Properties.SaleManagerProperties);
			ExpansionCostManager = new ExpansionCostManager(storage.GetStorageDict("ExpansionCostManager"), Properties.ExpansionProperties);
			FriendsManager = new FriendsManager(storage.GetStorageDict("FriendsManager"), GameState, gameServer.GameSparksServer.Friends, gameServer.GameSparksServer.Authenticator, Properties.FriendsManagerProperties);
			LikeRegistrar = new LikeRegistrar(storage.GetStorageDict("LikeRegistrar"), gameServer.GameSparksServer.Likes, FriendsManager);
			IslandVisitingManager = new IslandVisitingManager(storage.GetStorageDict("OtherIslandsData"), gameServer.GameSparksServer.Authenticator, gameServer.GameSparksServer.IslandVisiting, GameState, LikeRegistrar, GameRoutineRunner);
			CraneManager = new CraneManager(storage.GetStorageDict("CraneManager"), Timing, PopupManager);
			BuildingWarehouseManager = new BuildingWarehouseManager(storage.GetStorageDict("BuildingWarehouseManager"), GameState, Properties, PopupManager);
			BuildingsManager = new BuildingsManager(storage.GetStorageDict("BuildingsManager"), GameState, GameStats);
			CurrencyConversionManager = new CurrencyConversionManager(GameState, Properties.CurrencyConversionsProperties);
			TreasureChestManager = new TreasureChestManager(storage.GetStorageDict("TreasureChestManager"), GameState, LocalNotificationManager, CraneManager, BuildingWarehouseManager, GameRoutineRunner, PopupManager, GameStats, Properties, gameServer.WebService.Multipliers, VideoAds3Manager, tutorialIntermediary, firstVersion);
			CityAdvisor = new CityAdvisor(GameStats, GameState, PopupManager, Properties);
			IslandsManager = new IslandsManager(storage.GetStorageDict("IslandsManager"), GameStats, IslandVisitingManager);
			WorldMap = new WorldMap(storage.GetStorageDict("WorldMap"), Timing, GameRoutineRunner, gameServer.WebService.Multipliers, GameState, IslandsManager, Properties.IslandConnectionGraphProperties);
			CrossPromoPopupScheduler = new CrossPromoPopupScheduler(storage.GetStorageDict("CrossPromo"), GameRoutineRunner, PopupManager, WorldMap, GameState, gameServer.CrossPromo);
			IslandVisitingCurrencyManager = new IslandVisitingCurrencyManager(storage.GetStorageDict("IslandVisitingTokenManager"), GameState, FriendsManager, Properties.IslandVisitingCurrencyProperties);
			ArcadeManager = new ArcadeManager(storage.GetStorageDict("MinigamesManager"), gameServer.WebService, GameState, Properties.WheelOfFortuneProperties);
			QuestFactory questFactory = new QuestFactory(GameState, GameStats);
			FishingEvent = new FishingEvent(storage.GetStorageDict("FishingEvent"), Properties.FishingEventProperties, questFactory, GameState, LocalNotificationManager, PopupManager, GameRoutineRunner, Timing);
			InterstitialAdsManager = new InterstitialAdsManager(storage.GetStorageDict("InterstitialAdsManager"), GameState, PopupManager, IslandsManager, AdProviderPool, tutorialIntermediary, CriticalProcesses, gameServer.IAPStore, Properties.InterstitialAdsProperties);
			KeyDealsManager = new KeyDealsManager(storage.GetStorageDict("KeyDealsManager"), GameState, GameStats, gameServer.WebService.Multipliers, BuildingWarehouseManager, GameRoutineRunner, Properties);
			ComeBackNotificationsManager = new ComeBackNotificationsManager(LocalNotificationManager);
			OneTimeOfferManager = new OneTimeOfferManager(storage.GetStorageDict("OneTimeOfferManager"), GameState, GameStats, BuildingWarehouseManager, TreasureChestManager, gameServer.IAPStore, Properties);
			ServerGifts = new ServerGifts(storage.GetStorageDict("ServerGifts"), gameServer.WebService, PopupManager, Properties);
			LeaderboardsManager = new LeaderboardsManager(GameStats, GameState, gameServer.GameSparksServer.Authenticator, gameServer.GameSparksServer.Leaderboards, LikeRegistrar, GameRoutineRunner);
			QuestsManager = new QuestsManager(storage.GetStorageDict("QuestsManager"), GameState, GameStats, GameRoutineRunner, questFactory, Properties.QuestsManagerProperties);
			CraneOfferManager = new CraneOfferManager(storage.GetStorageDict("CraneOfferManager"), CraneManager, PopupManager, GameRoutineRunner, gameServer.IAPStore, GameState, Properties.CraneManagerOfferProperties);
			FlyingStartDealManager = new FlyingStartDealManager(storage.GetStorageDict("FlyingStartDeal"), tutorialIntermediary, GameState, GameRoutineRunner, gameServer.IAPStore, Properties.FlyingStartDealProperties);
			NewsletterManager = new NewsletterManager(storage.GetStorageDict("NewsletterManager"), gameServer.WebService, PopupManager, ServerGifts, GameRoutineRunner, Properties.NewsletterProperties);
			RatingRequester = new RatingRequester(storage.GetStorageDict("RatingRequester"), gameServer.WebService, PopupManager, GameState);
			EngagementTracker = new EngagementTracker(GameRoutineRunner);
			TutorialManager = new TutorialManager(storage.GetStorageDict("TutorialManager"), GameState, GameStats, WorldMap, PopupManager, IslandsManager, CraneManager, BuildingWarehouseManager, TreasureChestManager, CurrencyConversionManager, FishingEvent);
			tutorialIntermediary.SetTutorialManager(TutorialManager);
			DailyRewardsManager = new DailyRewardsManager(storage.GetStorageDict("DailyRewardsManager"), GameState, LocalNotificationManager, PopupManager, TutorialManager, GameStats, Properties.DailyRewardsProperties, VideoAds3Manager);
		}

		public void Release()
		{
			GameRoutineRunner.Release();
			GameStats.Release();
			ServerGifts.Release();
			AdProviderPool.Release();
			SaleManager.Release();
			RatingRequester.Release();
			InterstitialAdsManager.Release();
			FriendsManager.Release();
			OneTimeOfferManager.Release();
		}

		public void Serialize()
		{
			LocalNotificationManager.Serialize();
			AdProviderPool.Serialize();
			VideoAds1Manager.Serialize();
			VideoAds2Manager.Serialize();
			InterstitialAdsManager.Serialize();
			GameState.Serialize();
			SaleManager.Serialize();
			ExpansionCostManager.Serialize();
			FriendsManager.Serialize();
			LikeRegistrar.Serialize();
			IslandVisitingManager.Serialize();
			CraneManager.Serialize();
			BuildingWarehouseManager.Serialize();
			BuildingsManager.Serialize();
			TreasureChestManager.Serialize();
			IslandsManager.Serialize();
			WorldMap.Serialize();
			IslandVisitingCurrencyManager.Serialize();
			FishingEvent.Serialize();
			ArcadeManager.Serialize();
			KeyDealsManager.Serialize();
			DailyRewardsManager.Serialize();
			OneTimeOfferManager.Serialize();
			ServerGifts.Serialize();
			LeaderboardsManager.Serialize();
			QuestsManager.Serialize();
			CraneOfferManager.Serialize();
			CrossPromoPopupScheduler.Serialize();
			FlyingStartDealManager.Serialize();
			NewsletterManager.Serialize();
			RatingRequester.Serialize();
		}
	}
}
