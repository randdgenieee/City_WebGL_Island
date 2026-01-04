using System.Collections;
using UnityEngine;

namespace CIG
{
	public abstract class SceneLoader : MonoBehaviour
	{
		private Coroutine _waitRoutine;

		private SceneRequest _dataToLoad;

		public void LoadScene(SceneRequest data)
		{
			if (_waitRoutine == null)
			{
				_dataToLoad = data;
				_waitRoutine = StartCoroutine(WaitForLoader());
			}
			else
			{
				string text = (data == null) ? "(data.SceneName is null)" : data.SceneName;
				string text2 = (_dataToLoad == null) ? "(_data.SceneName is null)" : _dataToLoad.SceneName;
				UnityEngine.Debug.LogErrorFormat("Requested scene \"{0}\" while already waiting to load \"{1}\".", text, text2);
			}
		}

		protected virtual void OnStartLoading()
		{
		}

		private IEnumerator WaitForLoader()
		{
			while (Loader.IsLoading)
			{
				yield return null;
			}
			OnStartLoading();
			Loader.LoadScene(_dataToLoad);
		}
	}
}
