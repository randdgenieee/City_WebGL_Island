using CIG;
using UnityEngine;

namespace CIGEditor
{
	public class ChestTestSceneBootstrapper : MonoBehaviour
	{
		[SerializeField]
		private AudioManager _audioManager;

		private void Start()
		{
			_audioManager.Initialize(new Settings(new StorageDictionary()));
		}
	}
}
