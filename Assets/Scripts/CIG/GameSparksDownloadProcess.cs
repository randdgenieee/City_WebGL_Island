using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace CIG
{
	public class GameSparksDownloadProcess
	{
		private readonly RoutineRunner _routineRunner;

		private Action<byte[]> _onSuccess;

		private Action _onError;

		private IEnumerator _downloadRoutine;

		public GameSparksDownloadProcess(RoutineRunner routineRunner, string url, Action<byte[]> onSuccess, Action onError)
		{
			_routineRunner = routineRunner;
			_onSuccess = onSuccess;
			_onError = onError;
			_routineRunner.StartCoroutine(_downloadRoutine = DownloadRoutine(url));
		}

		public void Release()
		{
			if (_routineRunner != null && _downloadRoutine != null)
			{
				_routineRunner.StopCoroutine(_downloadRoutine);
				_downloadRoutine = null;
			}
			_onSuccess = null;
			_onError = null;
		}

		public void Cancel()
		{
			if (_routineRunner != null && _downloadRoutine != null)
			{
				_routineRunner.StopCoroutine(_downloadRoutine);
				_downloadRoutine = null;
				EventTools.Fire(_onError);
				Release();
			}
		}

		private IEnumerator DownloadRoutine(string url)
		{
			UnityWebRequest webRequest = UnityWebRequest.Get(url);
			yield return webRequest.SendWebRequest();
			if (string.IsNullOrEmpty(webRequest.error))
			{
				EventTools.Fire(_onSuccess, webRequest.downloadHandler.data);
			}
			else
			{
				UnityEngine.Debug.LogError("Failed to download data: " + webRequest.error);
				EventTools.Fire(_onError);
			}
			_downloadRoutine = null;
			Release();
		}
	}
}
