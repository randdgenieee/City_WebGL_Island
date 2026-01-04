using UnityEngine;

namespace CIG
{
	public class WelcomeBootstrapper : MonoBehaviour
	{
		[SerializeField]
		private SessionRestarter _sessionRestarter;

		[SerializeField]
		private AutoSaver _autoSaver;

		[SerializeField]
		private WelcomeSceneCloudSaveResolver _cloudSaveResolver;

		[SerializeField]
		private WelcomeView _welcomeView;

		[SerializeField]
		private AudioManager _audioManager;

		[SerializeField]
		private SceneLoader _sceneLoader;

		private Model _model;

		private void Start()
		{
			WelcomeSceneRequest welcomeSceneRequest = Loader.LastSceneRequest as WelcomeSceneRequest;
			if (welcomeSceneRequest == null)
			{
				UnityEngine.Debug.LogError("Scene was not loaded using Scene Loader!");
				return;
			}
			_model = welcomeSceneRequest.Model;
			_audioManager.Initialize(_model.Device.Settings);
			_welcomeView.Initialize(_model, _sceneLoader);
			_sessionRestarter.Initialize(_model, _sceneLoader);
			_autoSaver.Initialize();
			_cloudSaveResolver.Initialize(_model);
		}

		private void OnApplicationPause(bool paused)
		{
			_autoSaver.ApplicationPause(paused);
			_sessionRestarter.ApplicationPause(paused);
		}

		private void OnApplicationQuit()
		{
			_autoSaver.ApplicationQuit();
			_model.Release();
			_model = null;
		}
	}
}
