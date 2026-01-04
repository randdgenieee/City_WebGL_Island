using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

namespace CIG
{
    public static class AntiCheatDateTime
    {
        public delegate void LastConfirmedTimestampUpdateHandler(TimestampSource source);

        private class AntiCheatDateTimeMonoBehaviour : MonoBehaviour
        {
            public delegate void FocusReceivedHandler(bool focus);

            public event FocusReceivedHandler FocusChangedEvent;

            private void FireFocusChangedEvent(bool focus)
            {
                if (this.FocusChangedEvent != null)
                {
                    this.FocusChangedEvent(focus);
                }
            }

            private void OnApplicationFocus(bool focus)
            {
                FireFocusChangedEvent(focus);
            }
        }

        public const int MaxRequestsPerSession = 100;

        private static readonly AntiCheatDateTimeMonoBehaviour _monoBehaviour;

        private static DateTime _lastConfirmedDateTime;

        private static DateTime _lastConfirmedSystemDateTime;

        private static long _lastConfirmedDeviceUptime;

        private static float _lastConfirmedPlayTime;

        private static IEnumerator _serverRoutine;

        private static int _requestsThisSession;

        private static readonly IDeviceTime _deviceTime;

        private const string StorageKey = "AntiCheatDateTime";

        private const string LastConfirmedDateTimeKey = "LastConfirmedDateTime";

        private const string LastConfirmedSystemDateTimeKey = "LastConfirmedSystemDateTime";

        private const string LastConfirmedDeviceUptimeKey = "LastConfirmedDeviceUptime";

        private const string LastConfirmedPlayTimeKey = "LastConfirmedPlayTime";

        private static StorageDictionary _storage;

        public static DateTime UtcNow => _lastConfirmedDateTime.AddSeconds(Time.unscaledTime - _lastConfirmedPlayTime);

        public static DateTime Now => UtcNow.ToLocalTime();

        public static DateTime Today => Now.Date;

        public static DateTime TimeTillMidnight
        {
            get
            {
                TimeSpan value = UtcNow - Now;
                return Today.AddDays(1.0).Add(value);
            }
        }

        public static TimestampSource LastConfirmedTimestampSource
        {
            get;
            private set;
        }

        private static StorageDictionary Storage => _storage ?? (_storage = StorageController.ForeverRoot.GetStorageDict("AntiCheatDateTime"));

        public static event LastConfirmedTimestampUpdateHandler LastConfirmedTimestampUpdateEvent;

        private static void FireLastConfirmedTimestampUpdateEvent(TimestampSource source)
        {
            if (AntiCheatDateTime.LastConfirmedTimestampUpdateEvent != null)
            {
                AntiCheatDateTime.LastConfirmedTimestampUpdateEvent(source);
            }
        }

        static AntiCheatDateTime()
        {
            _deviceTime = new FallbackDeviceTime();
            Deserialize();
            _monoBehaviour = new GameObject("AntiCheatDateTime").AddComponent<AntiCheatDateTimeMonoBehaviour>();
            UnityEngine.Object.DontDestroyOnLoad(_monoBehaviour);
            _monoBehaviour.FocusChangedEvent += OnFocusChanged;
            RefreshTimestamp();
        }

        public static DateTime ConvertToSystemDateTime(DateTime dateTime, DateTimeKind dateTimeKind)
        {
            TimeSpan t;
            if ((uint)dateTimeKind <= 1u || dateTimeKind != DateTimeKind.Local)
            {
                t = dateTime - UtcNow;
                return DateTime.UtcNow + t;
            }
            t = dateTime - Now;
            return DateTime.Now + t;
        }

        private static void OnFocusChanged(bool focus)
        {
            if (focus)
            {
                RefreshTimestamp();
            }
            else
            {
                StopRefreshTimestamp();
            }
        }

        private static void RefreshTimestamp()
        {
            UpdateDeviceTime();
            if (_serverRoutine == null && _requestsThisSession < 100)
            {
                _monoBehaviour.StartCoroutine(_serverRoutine = UpdateServerTime());
                _requestsThisSession++;
            }
        }

        private static void StopRefreshTimestamp()
        {
            if (_serverRoutine != null)
            {
                _monoBehaviour.StopCoroutine(_serverRoutine);
                _serverRoutine = null;
            }
        }

        private static void UpdateDeviceTime()
        {
            if (_deviceTime.HasRestartedSinceLastCheck(_lastConfirmedDeviceUptime))
            {
                if (_lastConfirmedDateTime == DateTime.MinValue)
                {
                    UpdateLastConfirmedTimestamp(DateTime.UtcNow, _deviceTime.GetTimeSinceBoot(), TimestampSource.Guess);
                }
                else
                {
                    UpdateLastConfirmedTimestamp(_lastConfirmedDateTime + (DateTime.UtcNow - _lastConfirmedSystemDateTime), _deviceTime.GetTimeSinceBoot(), TimestampSource.Guess);
                }
            }
            else
            {
                long timeSinceBoot = _deviceTime.GetTimeSinceBoot();
                UpdateLastConfirmedTimestamp(_lastConfirmedDateTime.AddSeconds(timeSinceBoot - _lastConfirmedDeviceUptime), timeSinceBoot, TimestampSource.Device);
            }
        }

        private static IEnumerator UpdateServerTime()
        {
            UnityWebRequest webRequest = UnityWebRequest.Head("https://gamesapi.sparklingsociety.net/time.html");
            yield return webRequest.SendWebRequest();
            if (!webRequest.isNetworkError)
            {
                string responseHeader = webRequest.GetResponseHeader("Date");
                if (DateTime.TryParse(responseHeader, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime result))
                {
                    UpdateLastConfirmedTimestamp(result, _deviceTime.GetTimeSinceBoot(), TimestampSource.Server);
                }
                else
                {
                    UnityEngine.Debug.LogWarningFormat("Could not parse '{0}' to a 'DateTime'.", responseHeader);
                }
            }
            _serverRoutine = null;
        }

        private static void UpdateLastConfirmedTimestamp(DateTime dateTime, long deviceTime, TimestampSource source)
        {
            _lastConfirmedDateTime = dateTime;
            _lastConfirmedSystemDateTime = DateTime.UtcNow;
            _lastConfirmedPlayTime = Time.unscaledTime;
            _lastConfirmedDeviceUptime = deviceTime;
            Serialize();
            LastConfirmedTimestampSource = source;
            FireLastConfirmedTimestampUpdateEvent(source);
        }

        private static void Serialize()
        {
            Storage.Set("LastConfirmedDateTime", _lastConfirmedDateTime);
            Storage.Set("LastConfirmedSystemDateTime", _lastConfirmedSystemDateTime);
            Storage.Set("LastConfirmedDeviceUptime", _lastConfirmedDeviceUptime);
            Storage.Set("LastConfirmedPlayTime", _lastConfirmedPlayTime);
        }

        private static void Deserialize()
        {
            _lastConfirmedDateTime = Storage.GetDateTime("LastConfirmedDateTime", DateTime.MinValue);
            _lastConfirmedSystemDateTime = Storage.GetDateTime("LastConfirmedSystemDateTime", DateTime.MinValue);
            _lastConfirmedDeviceUptime = Storage.Get("LastConfirmedDeviceUptime", 0L);
            _lastConfirmedPlayTime = Storage.Get("LastConfirmedPlayTime", 0f);
        }
    }
}
