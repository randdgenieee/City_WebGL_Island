using System.Collections;
using UnityEngine;

namespace CIG
{
	public class AutoSaver : MonoBehaviour
	{
		[SerializeField]
		private float _saveInterval = 60f;

		private IEnumerator _autoSaveRoutine;

		public void Initialize()
		{
			StartAutoSaveRoutine();
		}

		public void ApplicationPause(bool paused)
		{
			if (paused)
			{
				PrepareLeavingGame();
			}
			else
			{
				StartAutoSaveRoutine();
			}
		}

		public void ApplicationQuit()
		{
			PrepareLeavingGame();
		}

		private void StartAutoSaveRoutine()
		{
			if (_autoSaveRoutine != null)
			{
				StopCoroutine(_autoSaveRoutine);
			}
			StartCoroutine(_autoSaveRoutine = AutoSaveRoutine());
		}

		private void PrepareLeavingGame()
		{
			if (_autoSaveRoutine != null)
			{
				StopCoroutine(_autoSaveRoutine);
				_autoSaveRoutine = null;
			}
			StorageController.Save();
		}

		private IEnumerator AutoSaveRoutine()
		{
			while (true)
			{
				yield return new WaitForSecondsRealtime(_saveInterval);
				StorageController.Save();
			}
		}
	}
}
