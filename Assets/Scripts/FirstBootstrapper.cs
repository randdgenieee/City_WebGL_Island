using CIG;
using UnityEngine;

public class FirstBootstrapper : MonoBehaviour
{
	[SerializeField]
	private ExceptionReporter _exceptionReporter;

	private void Start()
	{
		StorageController.Migrate();
		Screen.sleepTimeout = -1;
		_exceptionReporter.Initialize();
		Loader.LoadScene(new WelcomeSceneRequest());
	}
}
