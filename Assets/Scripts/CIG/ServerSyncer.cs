using System.Collections;
using UnityEngine;

namespace CIG
{
	public class ServerSyncer : MonoBehaviour
	{
		private const float SuccessWaitTime = 900f;

		private const float ErrorWaitTime = 60f;

		private GameServer _gameServer;

		private Game _game;

		public void Initialize(GameServer gameServer, Game game)
		{
			_gameServer = gameServer;
			_game = game;
			_gameServer.ServerSyncEvent += OnServerSynced;
			OnServerSynced(_gameServer.LastSyncSuccess);
		}

		private void OnDestroy()
		{
			if (_gameServer != null)
			{
				_gameServer.ServerSyncEvent -= OnServerSynced;
				_gameServer = null;
			}
		}

		private void OnServerSynced(bool success)
		{
			StartCoroutine(SyncWithServerRoutine(success ? 900f : 60f));
		}

		private IEnumerator SyncWithServerRoutine(float waitTime)
		{
			yield return new WaitForSeconds(waitTime);
			yield return _gameServer.SyncWithServer(_game);
		}
	}
}
