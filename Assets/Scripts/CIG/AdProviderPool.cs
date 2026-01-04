using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
    public sealed class AdProviderPool
    {
        public delegate void AdProviderAvailabilityChangedEventHandler(AdProviderType providerType);

        public const float TimeBetweenAdLoadAttemptsInSeconds = 5f;

        private readonly Dictionary<AdProviderType, IAdProvider> _adProviders = new Dictionary<AdProviderType, IAdProvider>();

        private readonly StorageDictionary _storage;

        private readonly RoutineRunner _routineRunner;

        private readonly GameStats _gameStats;

        private readonly CriticalProcesses _criticalProcesses;

        private readonly AdSequenceProperties _adSequenceProperties;

        private int _videosWatchedToday;

        private DateTime _todayExpireTime;

        private AdSequenceType _currentAdSequenceType;

        private IEnumerator _initializeRoutine;

        private IEnumerator _cacheRoutine;

        private bool _watchingAd;

        private const string VideosWatchedTodayKey = "VideosWatchedToday";

        private const string TodayExpireTimeKey = "TodayExpireTime";

        public event AdProviderAvailabilityChangedEventHandler AdAvailabilityChangedEvent;

        private void FireAdAvailabilityChangedEvent(AdProviderType providerType)
        {
            this.AdAvailabilityChangedEvent?.Invoke(providerType);
        }

        public AdProviderPool(StorageDictionary storage, RoutineRunner routineRunner, User user, CriticalProcesses criticalProcesses, GameStats gameStats, AdSequenceProperties adSequenceProperties, AdMobInterstitialAdsProperties admobInterstitialAdsProperties)
        {
            _storage = storage;
            _routineRunner = routineRunner;
            _criticalProcesses = criticalProcesses;
            _gameStats = gameStats;
            _adSequenceProperties = adSequenceProperties;
            _videosWatchedToday = _storage.Get("VideosWatchedToday", 0);
            _todayExpireTime = _storage.GetDateTime("TodayExpireTime", AntiCheatDateTime.Today.AddDays(1.0));
            _currentAdSequenceType = _adSequenceProperties.GetAdSequenceType(_videosWatchedToday);
            _adProviders[AdProviderType.AdMobVideo] = new AdMobVideoAdsProvider(routineRunner);
            foreach (KeyValuePair<AdProviderType, IAdProvider> adProvider in _adProviders)
            {
                adProvider.Value.AvailabilityChangedEvent += OnAvailabilityChanged;
            }
            _routineRunner.StartCoroutine(_initializeRoutine = InitializeProvidersRoutine());
        }

        public void Release()
        {
            foreach (KeyValuePair<AdProviderType, IAdProvider> adProvider in _adProviders)
            {
                adProvider.Value.Release();
            }
        }

        public bool AdProviderExists(AdProviderType providerType)
        {
            return _adProviders.ContainsKey(providerType);
        }

        public bool IsReady(AdProviderType providerType)
        {
            if (_watchingAd || _initializeRoutine != null)
            {
                return false;
            }
            return GetAdProvider(providerType)?.IsReady ?? false;
        }

        public bool ShowAd(AdProviderType providerType, Action<bool, bool> callback, VideoSource source)
        {
            IAdProvider adProvider = GetAdProvider(providerType);
            if (!_watchingAd && adProvider != null && adProvider.IsReady)
            {
                if (adProvider.ShowAd(delegate (bool success, bool clicked)
                {
                    OnAdWatched(adProvider, success, clicked, callback, source);
                }))
                {
                    _watchingAd = true;
                    _criticalProcesses.RegisterCriticalProcess(this);
                    return true;
                }
                UnityEngine.Debug.LogError($"Failed to ShowAd for '{providerType}'");
            }
            return false;
        }

        private IAdProvider GetAdProvider(AdProviderType providerType)
        {
            if (_adProviders.TryGetValue(providerType, out IAdProvider value))
            {
                return value;
            }
            UnityEngine.Debug.LogWarning($"Missing AdProvider '{providerType}'");
            return null;
        }

        private void OnAvailabilityChanged(IAdProvider adProvider)
        {
            FireAdAvailabilityChangedEvent(adProvider.AdProviderType);
        }

        private void OnAdWatched(IAdProvider adProvider, bool success, bool clicked, Action<bool, bool> callback, VideoSource source)
        {
            if (success)
            {
                switch (adProvider.AdType)
                {
                    case AdType.Video:
                        _gameStats.AddNumberOfVideosWatched(1, source);
                        if (clicked)
                        {
                            _gameStats.AddNumberOfVideosClicked(1, source);
                        }
                        IncrementVideosWatchedToday();
                        break;
                    case AdType.Interstitial:
                        _gameStats.AddNumberOfInterstitialsWatched(1);
                        if (clicked)
                        {
                            _gameStats.AddNumberOfInterstitialsClicked(1);
                        }
                        break;
                    default:
                        UnityEngine.Debug.LogError($"Failed to count stats for AdType '{adProvider.AdType}'");
                        break;
                }
            }
            else if (adProvider.AdType == AdType.Video)
            {
                Analytics.VideoCanceled(source);
                if (clicked)
                {
                    _gameStats.AddNumberOfVideosClicked(1, source);
                }
            }
            _watchingAd = false;
            _criticalProcesses.UnregisterCriticalProcess(this);
            callback?.Invoke(success, clicked);
            StartCaching();
        }

        private void IncrementVideosWatchedToday()
        {
            if (_todayExpireTime < AntiCheatDateTime.Now)
            {
                _videosWatchedToday = 0;
                _todayExpireTime = AntiCheatDateTime.Today.AddDays(1.0);
            }
            _videosWatchedToday++;
            _currentAdSequenceType = _adSequenceProperties.GetAdSequenceType(_videosWatchedToday);
        }

        private void StartCaching()
        {
            if (_cacheRoutine != null)
            {
                _routineRunner.StopCoroutine(_cacheRoutine);
            }
            _routineRunner.StartCoroutine(_cacheRoutine = CacheProvidersRoutine());
        }

        private IEnumerator InitializeProvidersRoutine()
        {
            foreach (KeyValuePair<AdProviderType, IAdProvider> adProvider in _adProviders)
            {
                yield return adProvider.Value.Initialize();
            }
            _initializeRoutine = null;
            StartCaching();
        }

        private IEnumerator CacheProvidersRoutine()
        {
            foreach (KeyValuePair<AdProviderType, IAdProvider> adProvider in _adProviders)
            {
                if (adProvider.Value.AdProviderState == AdProviderState.Initialized)
                {
                    yield return adProvider.Value.StartCaching(_currentAdSequenceType);
                }
            }
            _cacheRoutine = null;
        }

        public void Serialize()
        {
            _storage.Set("VideosWatchedToday", _videosWatchedToday);
            _storage.Set("TodayExpireTime", _todayExpireTime);
        }
    }
}
