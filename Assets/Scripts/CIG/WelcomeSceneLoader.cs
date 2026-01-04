using UnityEngine;
using UnityEngine.EventSystems;

namespace CIG
{
	public class WelcomeSceneLoader : SceneLoader
	{
		[SerializeField]
		private EventSystem _eventSystem;

		protected override void OnStartLoading()
		{
			_eventSystem.enabled = false;
		}
	}
}
