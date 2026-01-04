using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace CIG
{
	public class GameSparksUploadProcess
	{
		public const string UploadTypeKey = "Type";

		private readonly RoutineRunner _routineRunner;

		private readonly string _uploadData;

		private readonly string _fileName;

		private Action _onSuccess;

		private Action _onError;

		private IEnumerator _uploadRoutine;

		public GameSparksUploadProcess(CIGGameSparksInstance gameSparksInstance, RoutineRunner routineRunner, Dictionary<string, object> metaData, string uploadData, string fileName, Action onSuccess, Action onError)
		{
			_routineRunner = routineRunner;
			_uploadData = uploadData;
			_fileName = fileName;
			_onSuccess = onSuccess;
			_onError = onError;
			new GetUploadUrlRequest(gameSparksInstance).SetUploadData(new GSRequestData(metaData)).Send(delegate(GetUploadUrlResponse successResponse)
			{
				_routineRunner.StartCoroutine(_uploadRoutine = UploadRoutine(successResponse.Url));
			}, delegate
			{
				EventTools.Fire(onError);
			});
		}

		public void Release()
		{
			if (_routineRunner != null && _uploadRoutine != null)
			{
				_routineRunner.StopCoroutine(_uploadRoutine);
				_uploadRoutine = null;
			}
			_onSuccess = null;
			_onError = null;
		}

		public void Cancel()
		{
			if (_routineRunner != null && _uploadRoutine != null)
			{
				_routineRunner.StopCoroutine(_uploadRoutine);
				_uploadRoutine = null;
				EventTools.Fire(_onError);
				Release();
			}
		}

		private IEnumerator UploadRoutine(string url)
		{
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddBinaryData("file", Encoding.UTF8.GetBytes(_uploadData), _fileName, "string");
			UnityWebRequest webRequest = UnityWebRequest.Post(url, wWWForm);
			yield return webRequest.SendWebRequest();
			if (webRequest.isNetworkError)
			{
				_onError?.Invoke();
			}
			else
			{
				_onSuccess?.Invoke();
			}
			_uploadRoutine = null;
			Release();
		}
	}
}
