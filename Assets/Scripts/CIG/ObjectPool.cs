using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class ObjectPool : MonoBehaviour
	{
		[SerializeField]
		private GameObject _prefab;

		[SerializeField]
		private int _initialPoolCount;

		private List<GameObject> _availablePool = new List<GameObject>();

		private List<GameObject> _claimedPool = new List<GameObject>();

		public int AvailableInstances => _availablePool.Count;

		public int ClaimedInstances => _claimedPool.Count;

		public int TotalInstances => AvailableInstances + ClaimedInstances;

		private void Awake()
		{
			for (int i = 0; i < _initialPoolCount; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(_prefab, base.transform, worldPositionStays: false);
				gameObject.SetActive(value: false);
				_availablePool.Add(gameObject);
			}
		}

		public T Pop<T>(Transform parent) where T : MonoBehaviour
		{
			return Pop(base.transform).GetComponent<T>();
		}

		public GameObject Pop(Transform parent)
		{
			GameObject gameObject;
			if (_availablePool.Count == 0)
			{
				UnityEngine.Debug.LogWarning("Failed to pop an instance from the Pool, no more instances available. - Instantiating a new one !");
				gameObject = UnityEngine.Object.Instantiate(_prefab, base.transform, worldPositionStays: false);
			}
			else
			{
				gameObject = _availablePool[0];
				_availablePool.RemoveAt(0);
			}
			_claimedPool.Add(gameObject);
			gameObject.SetActive(value: true);
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			return gameObject;
		}

		public void Push(List<GameObject> instances)
		{
			int count = instances.Count;
			for (int i = 0; i < count; i++)
			{
				Push(instances[i]);
			}
		}

		public void Push(GameObject instance)
		{
			if (!_claimedPool.Remove(instance))
			{
				if (!_availablePool.Contains(instance))
				{
					UnityEngine.Debug.LogWarningFormat("Failed to push '{0}' back to the pool, it doesn't belong to this ObjectPool!", instance.name);
				}
				else
				{
					UnityEngine.Debug.LogWarningFormat("Failed to push '{0}' back to the pool, it was never popped!", instance.name);
				}
			}
			else
			{
				instance.transform.SetParent(base.transform, worldPositionStays: false);
				instance.SetActive(value: false);
				_availablePool.Add(instance);
			}
		}
	}
}
