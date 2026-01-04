using System.Collections;
using System.Collections.Generic;

namespace CIG
{
    public class GameServer
    {
        public delegate void ServerSyncHandler(bool success);

        private const string WebServiceStorageKey = "WebService";

        private readonly RemoteConfig _remoteConfig;

        public RoutineRunner ServerRoutineRunner
        {
            get;
        }

        public WebService WebService
        {
            get;
        }

        public GameSparksServer GameSparksServer
        {
            get;
        }

        public CrossPromo CrossPromo
        {
            get;
        }

        public IAPStore<TOCIStoreProduct> IAPStore
        {
            get;
        }

        public Dictionary<string, string> OverriddenGameProperties
        {
            get
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                CopyDictionary(WebService.OverriddenGameProperties, dictionary);
                CopyDictionary(FirebaseManager.RemoteConfig.OverriddenGameProperties, dictionary);
                return dictionary;
            }
        }

        public bool LastSyncSuccess
        {
            get
            {
                if (WebService.LastSyncSuccess)
                {
                    if (FirebaseManager.IsAvailable)
                    {
                        return _remoteConfig.LastSyncSuccess;
                    }
                    return true;
                }
                return false;
            }
        }

        public event ServerSyncHandler ServerSyncEvent;

        private void FireServerSyncEvent(bool success)
        {
            this.ServerSyncEvent?.Invoke(success);
        }

        public GameServer(StorageDictionary storage, Device device)
        {
            ServerRoutineRunner = new RoutineRunner("ServerRoutineRunner");
            _remoteConfig = FirebaseManager.RemoteConfig;
            GameSparksServer = new GameSparksServer(device.Settings);
            WebService = new WebService(storage.GetStorageDict("WebService"), device, GameSparksServer);
            CrossPromo = new CrossPromo(ServerRoutineRunner);
            IAPStore = new IAPStore<TOCIStoreProduct>(device.User, WebService, ServerRoutineRunner, (StorageDictionary properties) => new TOCIStoreProduct(properties));
        }

        public void Release()
        {
            ServerRoutineRunner.Release();
            GameSparksServer.Release();
        }

        public IEnumerator SyncWithServer(Game game)
        {
            yield return WebService.SyncWithServerExtended(game);
            FireServerSyncEvent(LastSyncSuccess);
        }

        private static void CopyDictionary(Dictionary<string, string> source, Dictionary<string, string> destination)
        {
            foreach (KeyValuePair<string, string> item in source)
            {
                destination[item.Key] = item.Value;
            }
        }

        public void Serialize()
        {
            GameSparksServer.Serialize();
            WebService.Serialize();
        }
    }
}
