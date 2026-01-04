using System;
using UnityEngine;

namespace CIG
{
	public abstract class SceneCloudSaveResolver : MonoBehaviour
	{
		private Model _model;

		protected CloudStorage _cloudStorage;

		public void Initialize(Model model)
		{
			_model = model;
			_cloudStorage = _model.GameServer.GameSparksServer.CloudStorage;
			_cloudStorage.ConflictResolutionCompleteEvent += OnConflictResolutionComplete;
			OnConflictResolutionComplete(_cloudStorage.LastResolutionResult);
		}

		private void OnDestroy()
		{
			if (_cloudStorage != null)
			{
				_cloudStorage.ConflictResolutionCompleteEvent -= OnConflictResolutionComplete;
				_cloudStorage = null;
			}
		}

		protected void OnConflictResultPickLocal()
		{
			_cloudStorage.ConflictResolved();
			StorageController.Save();
		}

		protected void OnConflictResultPickCloud(Action callback)
		{
			_cloudStorage.LoadLastCloudSaveGame(delegate
			{
				OnPickCloudSuccess(callback);
			}, delegate
			{
				OnPickCloudError(callback);
			});
		}

		protected virtual void OnConflictAskPlayer()
		{
		}

		protected virtual void OnConflictResultInvalidGameVersion()
		{
		}

		protected virtual void OnPickCloudSuccess(Action callback)
		{
			_model.StartGame();
			_cloudStorage.ConflictResolved();
			StorageController.Save();
			EventTools.Fire(callback);
		}

		protected virtual void OnPickCloudError(Action callback)
		{
			EventTools.Fire(callback);
		}

		private void OnConflictResolutionComplete(ConflictResolver.ConflictSolution result)
		{
			UnityEngine.Debug.Log("[SceneCloudSaveResolver] OnConflictResolutionComplete: " + result);
			switch (result)
			{
			default:
				_cloudStorage.ConflictResolved();
				StorageController.Save();
				break;
			case ConflictResolver.ConflictSolution.AskPlayer:
				OnConflictAskPlayer();
				break;
			case ConflictResolver.ConflictSolution.PickLocal:
				OnConflictResultPickLocal();
				break;
			case ConflictResolver.ConflictSolution.PickCloud:
				OnConflictResultPickCloud(null);
				break;
			case ConflictResolver.ConflictSolution.InvalidGameVersion:
				OnConflictResultInvalidGameVersion();
				break;
			case ConflictResolver.ConflictSolution.SwitchedUser:
				OnConflictAskPlayer();
				break;
			}
		}
	}
}
