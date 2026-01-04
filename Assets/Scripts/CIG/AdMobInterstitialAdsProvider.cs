using System;
using System.Collections;
using UnityEngine;

namespace CIG
{
    public sealed class AdMobInterstitialAdsProvider
    {
        private const string PlaystoreTestAdId = "ca-app-pub-3940256099942544/1033173712";

        private const string iOSTestAdId = "ca-app-pub-3940256099942544/4411468910";

        private const float MaxBackOffSeconds = 120f;

        private static AdProviderState _providerState;

        private static AdState _adState;

        private readonly RoutineRunner _routineRunner;

        private readonly User _user;

        private readonly AdMobInterstitialAdsProperties _properties;

        private IEnumerator _loadRoutine;

        private bool _finishedLoading;

        private Action<bool, bool> _callback;

        private float _backOffSeconds;

        private IEnumerator _retryRoutine;

        private bool IsPayingUser => _user.IsPayingUser;

        private string AdId
        {
            get
            {
                if (IsPayingUser)
                {
                    return _properties.PlaystorePayerAdId;
                }
                return _properties.PlaystoreNonPayerAdId;
            }
        }

        private string TestAdId => "ca-app-pub-3940256099942544/1033173712";

        private bool TagForChildDirectedTreatmentEnabled => true;

        public bool IsReady
        {
            get
            {
                return true;
            }
        }


        public event AdAvailabilityChangedEventHandler AvailabilityChangedEvent;

        private void FireAvailabilityChangedEvent()
        {
        }

        public AdMobInterstitialAdsProvider(RoutineRunner routineRunner, User user, AdMobInterstitialAdsProperties properties)
        {
            _routineRunner = routineRunner;
            _user = user;
            _properties = properties;
        }




        private void DestroyAd()
        {
        }

        private void TrySchedulingDelayedCachingRoutine(bool applyBackOff)
        {
            if (_retryRoutine == null)
            {
                float num = 5f;
                if (applyBackOff)
                {
                    _backOffSeconds = Mathf.Min(_backOffSeconds + 5f, 120f);
                    num += _backOffSeconds;
                }
                _routineRunner.StartCoroutine(_retryRoutine = RetryCachingRoutine(num));
                UnityEngine.Debug.LogWarningFormat("[AdMobInterstitialAdsProvider] Ad Failed To Load. Retrying in {0} seconds", num);
            }
        }

        private void ExecuteVideoWatchOnMainThread()
        {
            EventTools.Fire(_callback, value0: true, value1: false);
            _callback = null;
            ScreenView.PopScreenView("interstitial_admob");
            FireAvailabilityChangedEvent();
        }

        private void OnAdLoaded(object sender, EventArgs e)
        {
            _finishedLoading = true;
            _backOffSeconds = 0f;
            _adState = AdState.Available;
            _routineRunner.Invoke(FireAvailabilityChangedEvent, 0f);
        }


        private void OnAdClosed(object sender, EventArgs args)
        {
            DestroyAd();
            _routineRunner.Invoke(ExecuteVideoWatchOnMainThread, 0f);
        }

        private IEnumerator RetryCachingRoutine(float timeOut)
        {
            yield return new WaitForSecondsRealtime(timeOut);
            _retryRoutine = null;
        }
    }
}
