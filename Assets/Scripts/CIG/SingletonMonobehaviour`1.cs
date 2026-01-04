using UnityEngine;

namespace CIG
{
	public class SingletonMonobehaviour<T> : MonoBehaviour where T : SingletonMonobehaviour<T>
	{
		private static T _instance;

		public static T Instance
		{
			get
			{
				if ((Object)_instance == (Object)null)
				{
					T val = UnityEngine.Object.FindObjectOfType<T>();
					if ((Object)val == (Object)null)
					{
						UnityEngine.Debug.LogWarning("Make sure there is one instance of '" + typeof(T).Name + "' in the current scene.");
					}
					else
					{
						if (Application.isEditor)
						{
							UnityEngine.Debug.LogError("_instance is null. Did you implement 'Awake' without override?");
							return val;
						}
						UnityEngine.Debug.LogError("Please do not call '" + typeof(T).Name + "' in 'Awake'");
					}
				}
				return _instance;
			}
		}

		public static bool IsAvailable => (Object)_instance != (Object)null;

		protected virtual void Awake()
		{
			if ((Object)_instance == (Object)null)
			{
				_instance = (T)this;
			}
			else if (_instance != this)
			{
				UnityEngine.Debug.LogError("Another instance of 'T' already exists! This object will be destroyed");
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		protected virtual void OnDestroy()
		{
			_instance = null;
		}
	}
}
