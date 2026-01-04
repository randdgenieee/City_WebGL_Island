using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CIG
{
	public class Loader : MonoBehaviour
	{
		private static Scene[] _oldScenes;

		public static SceneRequest LastSceneRequest
		{
			get;
			private set;
		}

		public static bool IsLoading
		{
			get;
			private set;
		}

		protected virtual void Start()
		{
			StartCoroutine(DoLoadingRoutine(LastSceneRequest));
		}

		public static void LoadScene(SceneRequest data)
		{
			if (!IsLoading)
			{
				IsLoading = true;
				int sceneCount = SceneManager.sceneCount;
				_oldScenes = new Scene[sceneCount];
				for (int i = 0; i < sceneCount; i++)
				{
					_oldScenes[i] = SceneManager.GetSceneAt(i);
				}
				LastSceneRequest = data;
				SceneManager.LoadSceneAsync(LastSceneRequest.LoaderSceneName, LoadSceneMode.Additive);
			}
			else
			{
				string text = (data == null) ? "(data.SceneName is null)" : data.SceneName;
				string text2 = (LastSceneRequest == null) ? "(_data.SceneName is null)" : LastSceneRequest.SceneName;
				UnityEngine.Debug.LogErrorFormat("Requested scene '{0}' while '{1}' is being loaded.", text, text2);
			}
		}

		private IEnumerator DoLoadingRoutine(SceneRequest data)
		{
			OnStartLoading();
			SetProgress(0f);
			yield return StartCoroutine(DoIntroAnimation());
			for (int i = _oldScenes.Length - 1; i >= 0; i--)
			{
				if (_oldScenes[i].IsValid())
				{
					yield return SceneManager.UnloadSceneAsync(_oldScenes[i].name);
				}
				else
				{
					UnityEngine.Debug.LogErrorFormat("Tried to unload scene '{0}', but it is invalid!", _oldScenes[i].name);
				}
			}
			yield return Resources.UnloadUnusedAssets();
			StartCoroutine(data.LoadDuringSceneSwitch());
			while (!data.HasCompleted)
			{
				SetProgress(data.Progress * data.LoadingWeight);
				yield return null;
			}
			AsyncOperation loadOp = SceneManager.LoadSceneAsync(data.SceneName, LoadSceneMode.Additive);
			loadOp.allowSceneActivation = false;
			while (loadOp.progress < 0.9f)
			{
				SetProgress(data.LoadingWeight + loadOp.progress * (1f - data.LoadingWeight));
				yield return null;
			}
			loadOp.allowSceneActivation = true;
			while (!loadOp.isDone)
			{
				SetProgress(data.LoadingWeight + loadOp.progress * (1f - data.LoadingWeight));
				yield return null;
			}
			SetProgress(1f);
			SceneManager.SetActiveScene(SceneManager.GetSceneByName(data.SceneName));
			yield return StartCoroutine(DoOutroAnimation());
			SceneManager.UnloadSceneAsync(data.LoaderSceneName);
			Resources.UnloadUnusedAssets();
			GC.Collect();
			IsLoading = false;
		}

		protected virtual void OnStartLoading()
		{
		}

		protected virtual void SetProgress(float normalisedProgress)
		{
		}

		protected virtual IEnumerator DoIntroAnimation()
		{
			yield break;
		}

		protected virtual IEnumerator DoOutroAnimation()
		{
			yield break;
		}
	}
}
