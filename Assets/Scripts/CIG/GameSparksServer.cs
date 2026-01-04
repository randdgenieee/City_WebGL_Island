using GameSparks.Api.Messages;
using System;
using System.Collections;
using UnityEngine;

namespace CIG
{
    public class GameSparksServer
    {
        public delegate void ShutdownEventHandler();

        private const float SyncTimeoutSeconds = 10f;

        private bool _hasSynced;

        public RoutineRunner _gameSparksRoutineRunner;

        public CIGGameSparksInstance GameSparksInstance
        {
            get;
        }

        public CIGGameSparksPlatform GameSparksPlatform
        {
            get;
        }

        public GameSparksAuthenticator Authenticator
        {
            get;
        }

        public GameSparksUser GameSparksUser
        {
            get;
        }

        public GameSparksAuthenticationController AuthenticationController
        {
            get;
        }

        public CloudStorage CloudStorage
        {
            get;
        }


        public GameCenterService GameCenterService
        {
            get;
        }

        public GameSparksLeaderboards Leaderboards
        {
            get;
        }

        public GameSparksFriends Friends
        {
            get;
        }

        public GameSparksIslandVisiting IslandVisiting
        {
            get;
        }

        public GameSparksLikes Likes
        {
            get;
        }

        public GameSparksPushNotifications GameSparksPushNotifications
        {
            get;
        }

        public event ShutdownEventHandler ShutdownEvent;

        private void FireShutdownEvent()
        {
            this.ShutdownEvent?.Invoke();
        }

        public GameSparksServer(Settings settings)
        {
            _gameSparksRoutineRunner = new RoutineRunner("GameSparksRoutineRunner");
            GameCenterService = new GameCenterService();
            GameSparksPlatform = new CIGGameSparksPlatform();
            GameSparksInstance = new CIGGameSparksInstance();
            GameSparksInstance.Initialise(GameSparksPlatform);
            Authenticator = new GameSparksAuthenticator(GameSparksInstance, settings);
            GameSparksUser = new GameSparksUser(GameSparksInstance, Authenticator);
            CloudStorage = new CloudStorage(GameSparksInstance, GameSparksPlatform, Authenticator, GameSparksUser, _gameSparksRoutineRunner);
            Leaderboards = new GameSparksLeaderboards(GameSparksInstance, Authenticator);
            Friends = new GameSparksFriends(GameSparksInstance);
            IslandVisiting = new GameSparksIslandVisiting(GameSparksInstance, _gameSparksRoutineRunner);
            Likes = new GameSparksLikes(GameSparksInstance);
            GameSparksPushNotifications = new GameSparksPushNotifications(GameSparksInstance, Authenticator, settings);
            CIGGameSparksInstance gameSparksInstance = GameSparksInstance;
            gameSparksInstance.GameSparksAvailable = (Action<bool>)Delegate.Combine(gameSparksInstance.GameSparksAvailable, new Action<bool>(OnGameSparksAvailable));
            SessionTerminatedMessage.Listener = (Action<SessionTerminatedMessage>)Delegate.Combine(SessionTerminatedMessage.Listener, new Action<SessionTerminatedMessage>(OnSessionTerminatedMessageReceived));
            _gameSparksRoutineRunner.StartCoroutine(UpdateRoutine());
        }

        public void Release()
        {
            CIGGameSparksInstance gameSparksInstance = GameSparksInstance;
            gameSparksInstance.GameSparksAvailable = (Action<bool>)Delegate.Remove(gameSparksInstance.GameSparksAvailable, new Action<bool>(OnGameSparksAvailable));
            SessionTerminatedMessage.Listener = (Action<SessionTerminatedMessage>)Delegate.Remove(SessionTerminatedMessage.Listener, new Action<SessionTerminatedMessage>(OnSessionTerminatedMessageReceived));
            GameSparksInstance.Release(delegate
            {
                _gameSparksRoutineRunner.Release();
            });
            CloudStorage.Release();
            IslandVisiting.Release();
            Friends.Release();
            FireShutdownEvent();
        }

        public IEnumerator SyncWithServer()
        {
            bool authenticationResponseReceived = false;
            float timeoutTime = Time.time + 10f;
            while (!authenticationResponseReceived && timeoutTime > Time.time)
            {
                yield return null;
            }
            _hasSynced = true;
        }

        private void OnGameSparksAvailable(bool available)
        {
            if (_hasSynced && available)
            {
                AuthenticationController.AutoAuthenticate(null, null);
            }
        }

        private void OnSessionTerminatedMessageReceived(SessionTerminatedMessage message)
        {
            AuthenticationController.AutoAuthenticate(null, null);
        }

        private IEnumerator UpdateRoutine()
        {
            while (true)
            {
                GameSparksPlatform.UpdateFromMainThread();
                yield return null;
            }
        }

        public void Serialize()
        {
            CloudStorage.Serialize();
        }
    }
}
