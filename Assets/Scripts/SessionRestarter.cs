using CIG;
using System;
using UnityEngine;

public class SessionRestarter : MonoBehaviour
{
	public delegate void ReturnedFromBackgroundEventHandler(double secondsInBackground);

	private Model _model;

	private SceneLoader _sceneLoader;

	private DateTime _pauseTime;

	private bool _isPaused;

	public float SessionIdleSeconds
	{
		get
		{
			if (!Debug.isDebugBuild)
			{
				return 14400f;
			}
			return 60f;
		}
	}

	public event ReturnedFromBackgroundEventHandler ReturnedFromBackgroundEvent;

	private void FireReturnedFromBackgroundEvent(double secondsInBackground)
	{
		this.ReturnedFromBackgroundEvent?.Invoke(secondsInBackground);
	}

	public void Initialize(Model model, SceneLoader sceneLoader)
	{
		_model = model;
		_sceneLoader = sceneLoader;
	}

	public bool RestartSession()
	{
		if (_model.Game == null || !_model.Game.CriticalProcesses.HasCriticalProcess)
		{
			_model.Release();
			_sceneLoader.LoadScene(new WelcomeSceneRequest());
			return true;
		}
		return false;
	}

	public bool ApplicationPause(bool isPaused)
	{
		if (isPaused)
		{
			_pauseTime = AntiCheatDateTime.UtcNow;
			_isPaused = true;
		}
		else if (_isPaused)
		{
			_isPaused = false;
			double totalSeconds = (AntiCheatDateTime.UtcNow - _pauseTime).TotalSeconds;
			if ((_model.Game == null || !_model.Game.CriticalProcesses.HasCriticalProcess) && totalSeconds > (double)SessionIdleSeconds)
			{
				return RestartSession();
			}
			FireReturnedFromBackgroundEvent(totalSeconds);
		}
		return false;
	}
}
