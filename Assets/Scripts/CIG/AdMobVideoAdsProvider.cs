using SparkLinq;
using System;
using System.Collections;
using UnityEngine;

namespace CIG
{
    public sealed class AdMobVideoAdsProvider : IAdProvider
    {
        private const string PlayStoreTestAdId = "ca-app-pub-3940256099942544/5224354917";

        private const string PlayStoreRewardedAdId = "ca-app-pub-2218023806663419/6759409301";

        private const string PlayStoreInterstitialAdId = "ca-app-pub-2218023806663419/4525347105";

        private const string iOSTestAdId = "ca-app-pub-3940256099942544/1712485313";

        private const string iOSRewardedAdId = "ca-app-pub-2218023806663419/1340982546";

        private const string iOSInterstitialAdId = "ca-app-pub-2218023806663419/8772424316";

        private const float MaxBackOffSeconds = 120f;

        private const int CachedAdCount = 3;

        private static AdProviderState _providerState;

        private readonly RoutineRunner _routineRunner;

        private AdSequenceType _currentAdSequenceType;

        private AdState _adState;

        private Action<bool, bool> _callback;

        private IEnumerator _loadRoutine;

        private bool? _loadStatus;

        private float _backOffSeconds;

        private IEnumerator _retryRoutine;

        private bool _completedAd;

        private bool _clickedAd;

        private bool TagForChildDirectedTreatmentEnabled => true;

        private string RewardedAdId => "ca-app-pub-2218023806663419/6759409301";

        private string InterstitialAdId => "ca-app-pub-2218023806663419/4525347105";

        public bool IsReady
        {
            get
            {
                return false;
            }
        }

        AdProviderType IAdProvider.AdProviderType => AdProviderType.AdMobVideo;

        AdType IAdProvider.AdType => AdType.Video;

        AdProviderState IAdProvider.AdProviderState => _providerState;

        public event AdAvailabilityChangedEventHandler AvailabilityChangedEvent;

        private void FireAvailabilityChangedEvent()
        {
            this.AvailabilityChangedEvent?.Invoke(this);
        }

        public AdMobVideoAdsProvider(RoutineRunner routineRunner)
        {
            _routineRunner = routineRunner;
        }

        IEnumerator IAdProvider.Initialize()
        {
            if (_providerState == AdProviderState.None)
            {
                _providerState = AdProviderState.Initializing;
                try
                {
                    AdMobManager.Initialize();
                }
                catch (Exception ex)
                {
                    _providerState = AdProviderState.None;
                    UnityEngine.Debug.LogError("[AdMobVideoAdsProvider] Failed to Initialize AdMob. Error: " + ex.Message + "\r\nStacktrace:\r\n" + ex.StackTrace);
                    yield break;
                }
                _providerState = AdProviderState.Initialized;
            }
        }

        void IAdProvider.Release()
        {
        }

        IEnumerator IAdProvider.StartCaching(AdSequenceType adSequenceType)
        {
            if (_providerState != AdProviderState.Initialized)
            {
                UnityEngine.Debug.LogWarning("AdMobVideoAdsProvider is not yet initialized!");
                yield break;
            }
            if (_currentAdSequenceType != adSequenceType)
            {
                if (_loadRoutine != null)
                {
                    _routineRunner.StopCoroutine(_loadRoutine);
                    _loadRoutine = null;
                }
                if (_retryRoutine != null)
                {
                    _routineRunner.StopCoroutine(_retryRoutine);
                    _retryRoutine = null;
                }
                int i = 0;
                ResetAdState();
                _currentAdSequenceType = adSequenceType;
            }
        }

        bool IAdProvider.ShowAd(Action<bool, bool> callback)
        {
            if (IsReady)
            {
                _callback = callback;
                _adState = AdState.Showing;
                ScreenView.PushScreenView("video_admob");
                return true;
            }
            return false;
        }

        private void TrySchedulingDelayedCachingRoutine()
        {
            if (_retryRoutine == null)
            {
                float num = 5f;
                _backOffSeconds = Mathf.Min(_backOffSeconds + 5f, 120f);
                num += _backOffSeconds;
                _routineRunner.StartCoroutine(_retryRoutine = RetryCachingRoutine(num));
                UnityEngine.Debug.LogWarning(string.Format("[{0}] Ad Failed To Load. Retrying in {1} seconds", "AdMobVideoAdsProvider", num));
            }
        }


        private void ResetAdState()
        {
            _adState = AdState.None;
            _completedAd = false;
            _clickedAd = false;
        }

        private void ExecuteVideoWatchOnMainThread()
        {
            bool completedAd = _completedAd;
            bool clickedAd = _clickedAd;
            ResetAdState();
            FireAvailabilityChangedEvent();
            ScreenView.PopScreenView("video_admob");
            EventTools.Fire(_callback, completedAd, clickedAd);
            _callback = null;
        }

        private void TryDestroyAd(object sender)
        {
        }

        private void OnRewardedAdLoaded(object sender, EventArgs e)
        {
            _loadStatus = true;
            _backOffSeconds = 0f;
        }


        private void OnRewardedAdClosed(object sender, EventArgs e)
        {
            TryDestroyAd(sender);
            _routineRunner.Invoke(ExecuteVideoWatchOnMainThread, 0f);
        }


        private void OnAdLeavingApplication(object sender, EventArgs e)
        {
            _clickedAd = true;
        }

        private IEnumerator RetryCachingRoutine(float timeOut)
        {
            yield return new WaitForSecondsRealtime(timeOut);
            _retryRoutine = null;
        }
    }
}
