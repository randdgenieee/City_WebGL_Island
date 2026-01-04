using System.Collections;
using UnityEngine;

namespace CIG
{
    public class WelcomeSceneRequest : SceneRequest
    {
        private const int TotalProcesses = 6;

        private int _completedProcesses;

        private bool _antiCheatDateTimeTimestampUpdated;

        public override string SceneName => "Welcome";

        public override string LoaderSceneName => "SplashSparkling";

        public override float LoadingWeight => 0.4f;

        public Model Model
        {
            get;
            private set;
        }

        private int CompletedProcesses
        {
            get
            {
                return _completedProcesses;
            }
            set
            {
                _completedProcesses = value;
                UpdateProgress(_completedProcesses);
            }
        }

        public override IEnumerator LoadDuringSceneSwitch()
        {
            CompletedProcesses = 0;
            yield return FirebaseManager.InitFirebaseApp();
            CompletedProcesses++;
            Model = new Model();
            yield return Model.GameServer.WebService.SyncWithServer();
            UserIdLogger.Log();
            CompletedProcesses++;
            CompletedProcesses++;
            CompletedProcesses++;
            yield return Model.GameServer.GameSparksServer.CloudStorage.SyncWithServer();
            Model.GameServer.GameSparksServer.CloudStorage.EnablePullOnAuthenticationChange();
            Model.StartGame();
            Analytics.SetNotificationsEnabled(Model.Device.Settings.NotificationsEnabled);
            CompletedProcesses++;
            if (AntiCheatDateTime.LastConfirmedTimestampSource != TimestampSource.Server)
            {
                float endTime = Time.unscaledTime + 2f;
                AntiCheatDateTime.LastConfirmedTimestampUpdateEvent += OnAntiCheatDateTimeTimestampUpdated;
                while (!_antiCheatDateTimeTimestampUpdated && endTime > Time.unscaledTime)
                {
                    yield return null;
                }
                AntiCheatDateTime.LastConfirmedTimestampUpdateEvent -= OnAntiCheatDateTimeTimestampUpdated;
            }
            CompletedProcesses = 6;
            base.HasCompleted = true;
        }

        private void UpdateProgress(int completedProcesses)
        {
            base.Progress = (float)completedProcesses / 6f;
        }

        private void OnAntiCheatDateTimeTimestampUpdated(TimestampSource source)
        {
            _antiCheatDateTimeTimestampUpdated = (source == TimestampSource.Server);
        }
    }
}
