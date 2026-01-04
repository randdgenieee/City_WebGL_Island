using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
    public abstract class BaseVideoAdsManager<TProperties> where TProperties : VideoAdsPropertiesBase
    {
        public delegate void AvailabilityChangedEventHandler();

        private const float ProviderAvailabilityValidationDelayInSeconds = 30f;

        private readonly StorageDictionary _storage;

        protected readonly AdProviderPool _adProviderPool;

        protected readonly TProperties _properties;

        private readonly AdWaterfall _adWaterfall;

        private readonly RoutineRunner _routineRunner;

        private DateTime _unlockTime;

        private DateTime _unlockStartTime;

        private List<DateTime> _watchedVideoTimestamps;

        private IEnumerator _availabilityRoutine;

        private const string UnlockStartTimeStorageKey = "UnlockStartTime";

        private const string WatchedVideoTimestampsStorageKey = "WatchedVideoTimestamps";

        private int WatchedVideoTimespanSeconds => _properties.TimespanDurationHours * 3600;

        public bool IsReady
        {
            get
            {
                return true;
                //if (_adWaterfall == null || !Mathf.Approximately(RemainingSecondsUntilNextVideo, 0f))
                //{
                //	return false;
                //}
                //return _adWaterfall.IsReady;
            }
        }

        private double RemainingUnlockSeconds => _unlockTime.Subtract(AntiCheatDateTime.UtcNow).TotalSeconds;

        private double RemainingVideoTimeoutSeconds
        {
            get
            {
                int count = _watchedVideoTimestamps.Count;
                if (count <= 0)
                {
                    return 0.0;
                }
                return _properties.TimeoutSeconds - AntiCheatDateTime.UtcNow.Subtract(_watchedVideoTimestamps[count - 1]).TotalSeconds;
            }
        }

        private double RemainingVideoTimespanTimeoutSeconds
        {
            get
            {
                if (_properties.MaxVideosPerTimespan == 0)
                {
                    return double.MaxValue;
                }
                int count = _watchedVideoTimestamps.Count;
                if (count < _properties.MaxVideosPerTimespan || count == 0)
                {
                    return 0.0;
                }
                return (double)WatchedVideoTimespanSeconds - AntiCheatDateTime.UtcNow.Subtract(_watchedVideoTimestamps[0]).TotalSeconds;
            }
        }

        private float RemainingSecondsUntilNextVideo => Mathf.Max(0f, (float)RemainingUnlockSeconds, (float)RemainingVideoTimeoutSeconds, (float)RemainingVideoTimespanTimeoutSeconds);

        public event AvailabilityChangedEventHandler AvailabilityChangedEvent;

        private void FireAvailabilityChangedEvent()
        {
            this.AvailabilityChangedEvent?.Invoke();
        }

        protected BaseVideoAdsManager(StorageDictionary storage, AdProviderPool adProviderPool, RoutineRunner routineRunner, AdWaterfall adWaterfall, TProperties properties)
        {
            _storage = storage;
            _adProviderPool = adProviderPool;
            _properties = properties;
            _routineRunner = routineRunner;
            _adWaterfall = adWaterfall;
            Deserialize();
            _unlockTime = _unlockStartTime.AddSeconds(_properties.UnlockSeconds);
            _adProviderPool.AdAvailabilityChangedEvent += OnAdAvailabilityChangedEvent;
            if (!IsReady)
            {
                float delay = Mathf.Max(RemainingSecondsUntilNextVideo, 30f);
                StartAvailabilityRoutine(delay);
            }
        }

        public bool ShowAd(Action<bool, bool> callback, VideoSource source)
        {
            callback(true, true);
            return true;

            if (!IsReady)
            {
                return false;
            }
            bool num = _adWaterfall.ShowAd(delegate (bool success, bool clicked)
            {
                OnVideoWatched(success, clicked, callback);
            }, source);
            if (num)
            {
                Analytics.VideoOpened(source);
            }
            return num;
        }

        private void StopAvailabilityRoutine()
        {
            if (_availabilityRoutine != null)
            {
                _routineRunner.StopCoroutine(_availabilityRoutine);
                _availabilityRoutine = null;
            }
        }

        private void StartAvailabilityRoutine(float delay)
        {
            StopAvailabilityRoutine();
            _routineRunner.StartCoroutine(_availabilityRoutine = AvailabilityRoutine(delay));
        }

        private void OnVideoWatched(bool success, bool clicked, Action<bool, bool> callback)
        {
            PruneWatchedVideoTimestamps();
            _watchedVideoTimestamps.Add(AntiCheatDateTime.UtcNow);
            StartAvailabilityRoutine(RemainingSecondsUntilNextVideo);
            callback?.Invoke(success, clicked);
        }

        private void OnAdAvailabilityChangedEvent(AdProviderType providerType)
        {
            if (_adWaterfall.ContainsAdProvider(providerType))
            {
                FireAvailabilityChangedEvent();
            }
        }

        private void PruneWatchedVideoTimestamps()
        {
            DateTime now = AntiCheatDateTime.UtcNow;
            _watchedVideoTimestamps.RemoveAll((DateTime x) => now.Subtract(x).TotalHours >= (double)_properties.TimespanDurationHours);
        }

        private IEnumerator AvailabilityRoutine(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            PruneWatchedVideoTimestamps();
            _availabilityRoutine = null;
            if (IsReady)
            {
                FireAvailabilityChangedEvent();
                yield break;
            }
            float delay2 = Math.Max(RemainingSecondsUntilNextVideo, 30f);
            _routineRunner.StartCoroutine(_availabilityRoutine = AvailabilityRoutine(delay2));
        }

        public void Serialize()
        {
            _storage.Set("UnlockStartTime", _unlockStartTime);
            _storage.Set("WatchedVideoTimestamps", _watchedVideoTimestamps);
        }

        private void Deserialize()
        {
            _unlockStartTime = _storage.GetDateTime("UnlockStartTime", AntiCheatDateTime.UtcNow);
            _watchedVideoTimestamps = _storage.GetDateTimeList("WatchedVideoTimestamps");
        }
    }
}
