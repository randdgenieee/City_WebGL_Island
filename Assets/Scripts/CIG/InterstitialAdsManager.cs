using System;

namespace CIG
{
	public sealed class InterstitialAdsManager
	{
		public delegate void AvailabilityChangedEventHandler();

		private readonly StorageDictionary _storage;

		private readonly AdWaterfall _adWaterfall;

		private readonly GameState _gameState;

		private readonly PopupManager _popupManager;

		private readonly IslandsManager _islandsManager;

		private readonly TutorialIntermediary _tutorialManager;

		private readonly IAPStore<TOCIStoreProduct> _iapStore;

		private readonly InterstitialAdsProperties _properties;

		private int _interstitialsShownCount;

		private int _levelUpNextInterstitialLevel;

		private int _inGameNextInterstitialSecondsPlayed;

		private DateTime _cooldownAfterIAPPurchased;

		private const string NextLevelUpInterstitialLevelStorageKey = "NextLevelUpInterstitialLevel";

		public bool IsReady
		{
			get
			{
				if (_adWaterfall != null && _tutorialManager.HasActiveTutorial && _interstitialsShownCount < _properties.MaxPerSession && _cooldownAfterIAPPurchased < AntiCheatDateTime.UtcNow)
				{
					return _adWaterfall.IsReady;
				}
				return false;
			}
		}

		private bool LevelUpInterstitialIsReady
		{
			get
			{
				if (_gameState.TotalMinutesPlayed >= _properties.LevelMinutesPlayed)
				{
					return _levelUpNextInterstitialLevel <= _gameState.Level;
				}
				return false;
			}
		}

		private bool InGameInterstitialReady
		{
			get
			{
				if (!_popupManager.IsShowingPopup && _gameState.TotalMinutesPlayed >= _properties.IngameMinutesPlayed)
				{
					return _inGameNextInterstitialSecondsPlayed <= _gameState.SecondsPlayedInThisSession;
				}
				return false;
			}
		}

		public event AvailabilityChangedEventHandler AvailabilityChangedEvent;

		private void FireAvailabilityChangedEvent()
		{
			if (this.AvailabilityChangedEvent != null)
			{
				this.AvailabilityChangedEvent();
			}
		}

		public InterstitialAdsManager(StorageDictionary storage, GameState gameState, PopupManager popupManager, IslandsManager islandsManager, AdProviderPool adProviderPool, TutorialIntermediary tutorialManager, CriticalProcesses criticalProcesses, IAPStore<TOCIStoreProduct> iapStore, InterstitialAdsProperties properties)
		{
			_storage = storage;
			_gameState = gameState;
			_popupManager = popupManager;
			_islandsManager = islandsManager;
			_tutorialManager = tutorialManager;
			_iapStore = iapStore;
			_properties = properties;
			AdProviderType[] adProviderSequence = new AdProviderType[1]
			{
				AdProviderType.AdMobInterstitial
			};
			_adWaterfall = new AdWaterfall(adProviderPool, criticalProcesses, adProviderSequence);
			_levelUpNextInterstitialLevel = _storage.Get("NextLevelUpInterstitialLevel", 1);
			_inGameNextInterstitialSecondsPlayed = _properties.IngameIntervalSeconds;
			_popupManager.PopupClosedEvent += OnPopupClosed;
			_islandsManager.IslandChangedEvent += OnIslandChanged;
			adProviderPool.AdAvailabilityChangedEvent += OnAdAvailabilityChangedEvent;
			_iapStore.PurchaseSuccessEvent += OnPurchaseSuccess;
		}

		public void Release()
		{
			_popupManager.PopupClosedEvent -= OnPopupClosed;
			_islandsManager.IslandChangedEvent -= OnIslandChanged;
			_iapStore.PurchaseSuccessEvent -= OnPurchaseSuccess;
		}

		public bool ShowAd()
		{
			if (!IsReady)
			{
				return false;
			}
			bool num = _adWaterfall.ShowAd(OnInterstitialWatched, VideoSource.Interstitial);
			if (num)
			{
				Analytics.LogEvent("interstitial_Opened");
			}
			return num;
		}

		private void TryShowInGameInterstitial()
		{
			if (InGameInterstitialReady && ShowAd())
			{
				_inGameNextInterstitialSecondsPlayed = _gameState.SecondsPlayedInThisSession + _properties.IngameIntervalSeconds;
			}
		}

		private void OnPopupClosed(PopupRequest request, bool instant)
		{
			if (request is LevelUpPopupRequest && LevelUpInterstitialIsReady)
			{
				if (ShowAd())
				{
					_levelUpNextInterstitialLevel = _gameState.Level + _properties.LevelAmount;
				}
			}
			else
			{
				TryShowInGameInterstitial();
			}
		}

		private void OnIslandChanged(IslandId islandId, bool isVisiting)
		{
			TryShowInGameInterstitial();
		}

		private void OnInterstitialWatched(bool success, bool clicked)
		{
			_interstitialsShownCount++;
		}

		private void OnAdAvailabilityChangedEvent(AdProviderType providerType)
		{
			if (_adWaterfall.ContainsAdProvider(providerType))
			{
				FireAvailabilityChangedEvent();
			}
		}

		private void OnPurchaseSuccess(Purchase<TOCIStoreProduct> purchase)
		{
			_cooldownAfterIAPPurchased = AntiCheatDateTime.UtcNow.AddSeconds(_properties.CooldownAfterIapSeconds);
		}

		public void Serialize()
		{
			_storage.Set("NextLevelUpInterstitialLevel", _levelUpNextInterstitialLevel);
		}
	}
}
