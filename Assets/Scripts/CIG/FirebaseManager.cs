using SUISS.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CIG
{
    public static class FirebaseManager
    {
        public enum StorageWarning
        {
            None,
            DiskFull,
            CreateDirectoryUnauthorizedException
        }

        private static readonly HashSet<string> MessageTypeKeys;

        private static bool _initSuccess;

        public static bool IsAvailable
        {
            get
            {
                return false;
            }
        }

        public static bool IsInitializing
        {
            get;
            private set;
        }

        public static bool IsAnalyticsEnabled
        {
            get;
            private set;
        }

        public static StorageWarning FirebaseAppStorageWarning
        {
            get;
            private set;
        }

        public static bool IsAvailableOnPlatform => true;

        public static MessagingGifts MessagingGifts
        {
            get;
            private set;
        }

        public static string MessageToken
        {
            get;
            private set;
        }

        public static MessagingSale MessagingSale
        {
            get;
        }

        public static RemoteConfig RemoteConfig
        {
            get;
        }

        static FirebaseManager()
        {
            MessageTypeKeys = new HashSet<string>
            {
                "messagetype",
                "typemessage",
                "type"
            };
            _initSuccess = false;
            MessagingGifts = new MessagingGifts();
            MessagingSale = new MessagingSale();
            RemoteConfig = new RemoteConfig();
        }

        public static IEnumerator InitFirebaseApp()
        {
            if (_initSuccess || IsInitializing)
            {
                yield break;
            }
            IsInitializing = true;
            FirebaseAppStorageWarning = StorageWarning.None;
            _initSuccess = false;
        }


        private static StorageWarning GetStorageWarning(Exception ex)
        {
            if (Storage.ExceptionMeansFullDisk(ex) || Storage.ExceptionMeansFullDisk(ex.InnerException))
            {
                return StorageWarning.DiskFull;
            }
            if (ex is UnauthorizedAccessException)
            {
                return StorageWarning.CreateDirectoryUnauthorizedException;
            }
            return StorageWarning.None;
        }

        private static void CreateFirebaseInstance()
        {
            IsAnalyticsEnabled = (0 == 0);
            try
            {
                _initSuccess = true;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogErrorFormat("[FirebaseManager] Failed to Init: {0}", ex);
                FirebaseAppStorageWarning = GetStorageWarning(ex);
            }
        }

    }
}
