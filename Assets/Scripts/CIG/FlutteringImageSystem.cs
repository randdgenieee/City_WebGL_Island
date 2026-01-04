using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class FlutteringImageSystem : MonoBehaviour
	{
		private enum Direction
		{
			Left,
			Right,
			Up,
			Down
		}

		[SerializeField]
		private Direction _direction = Direction.Up;

		[SerializeField]
		private float _speed = 100f;

		[SerializeField]
		private float _spawnDelayMin = 0.1f;

		[SerializeField]
		private float _spawnDelayMax = 0.6f;

		[SerializeField]
		private FlutteringImage _flutteringImagePrefab;

		private readonly Queue<FlutteringImage> _flutteringImagesPool = new Queue<FlutteringImage>();

		private readonly List<FlutteringImage> _activeFlutteringImages = new List<FlutteringImage>();

		private Timing _timing;

		private bool _isPlaying;

		private Vector3 _bottomLeft;

		private Vector3 _topLeft;

		private Vector3 _topRight;

		private Vector3 _bottomRight;

		private IEnumerator _spawnRoutine;

		private IEnumerator _moveRoutine;

		public void Initialize(Timing timing)
		{
			_timing = timing;
			Vector3[] array = new Vector3[4];
			((RectTransform)base.transform).GetLocalCorners(array);
			_bottomLeft = array[0];
			_topLeft = array[1];
			_topRight = array[2];
			_bottomRight = array[3];
			FillPool(Mathf.CeilToInt(GetTravelTime() / _spawnDelayMin));
		}

		public void Play()
		{
			if (!_isPlaying)
			{
				_isPlaying = true;
				Prewarm(GetTravelTime());
				StartCoroutine(_spawnRoutine = SpawnRoutine());
				StartCoroutine(_moveRoutine = MoveRoutine());
			}
		}

		public void Stop()
		{
			_isPlaying = false;
			if (_spawnRoutine != null)
			{
				StopCoroutine(_spawnRoutine);
				_spawnRoutine = null;
			}
			if (_moveRoutine != null)
			{
				StopCoroutine(_moveRoutine);
				_moveRoutine = null;
			}
			for (int num = _activeFlutteringImages.Count - 1; num >= 0; num--)
			{
				ReturnToPool(_activeFlutteringImages[num]);
			}
		}

		private IEnumerator SpawnRoutine()
		{
			while (true)
			{
				yield return new WaitForAnimationTimeSeconds(_timing, UnityEngine.Random.Range(_spawnDelayMin, _spawnDelayMax));
				SetFlutteringImageStartingPosition(GetFromPool());
			}
		}

		private IEnumerator MoveRoutine()
		{
			while (true)
			{
				float deltaTime = _timing.GetDeltaTime(DeltaTimeType.Unscaled);
				for (int num = _activeFlutteringImages.Count - 1; num >= 0; num--)
				{
					FlutteringImage flutteringImage = _activeFlutteringImages[num];
					MoveFlutteringImage(flutteringImage, deltaTime);
				}
				yield return null;
			}
		}

		private void MoveFlutteringImage(FlutteringImage flutteringImage, Vector3 movement, Func<Vector3, bool> shouldReturnToPool)
		{
			Vector3 vector = flutteringImage.transform.localPosition + movement;
			if (shouldReturnToPool(vector))
			{
				ReturnToPool(flutteringImage);
			}
			else
			{
				flutteringImage.transform.localPosition = vector;
			}
		}

		private void SetFlutteringImageStartingPosition(FlutteringImage flutteringImage)
		{
			Vector3 extents = flutteringImage.Extents;
			switch (_direction)
			{
			case Direction.Right:
				flutteringImage.transform.localPosition = new Vector3(_bottomLeft.x - extents.x, _bottomLeft.y + (_topLeft.y - _topRight.y) * UnityEngine.Random.value);
				break;
			case Direction.Left:
				flutteringImage.transform.localPosition = new Vector3(_bottomRight.x + extents.x, _topLeft.y - (_topLeft.y + _bottomRight.y) * UnityEngine.Random.value);
				break;
			case Direction.Down:
				flutteringImage.transform.localPosition = new Vector3(_topLeft.x + (_topRight.x - _topLeft.x) * UnityEngine.Random.value, _topLeft.y + extents.y);
				break;
			case Direction.Up:
				flutteringImage.transform.localPosition = new Vector3(_bottomLeft.x + (_bottomRight.x - _bottomLeft.x) * UnityEngine.Random.value, _bottomLeft.y - extents.y);
				break;
			}
		}

		private void MoveFlutteringImage(FlutteringImage flutteringImage, float deltaTime)
		{
			Vector3 extents = flutteringImage.Extents;
			float d = deltaTime * _speed;
			switch (_direction)
			{
			case Direction.Left:
				MoveFlutteringImage(flutteringImage, Vector3.left * d, (Vector3 position) => position.x + extents.x <= _bottomLeft.x);
				break;
			case Direction.Right:
				MoveFlutteringImage(flutteringImage, Vector3.right * d, (Vector3 position) => position.x - extents.x >= _bottomRight.x);
				break;
			case Direction.Up:
				MoveFlutteringImage(flutteringImage, Vector3.up * d, (Vector3 position) => position.y - extents.y >= _topLeft.y);
				break;
			case Direction.Down:
				MoveFlutteringImage(flutteringImage, Vector2.down * d, (Vector3 position) => position.y + extents.y <= _bottomLeft.y);
				break;
			}
		}

		private float GetTravelTime()
		{
			float num = 0f;
			switch (_direction)
			{
			case Direction.Left:
			case Direction.Right:
				num = _bottomRight.x - _bottomLeft.x + _flutteringImagePrefab.Extents.x * 2f;
				break;
			case Direction.Up:
			case Direction.Down:
				num = _topLeft.y - _bottomLeft.y + _flutteringImagePrefab.Extents.y * 2f;
				break;
			}
			return num / _speed;
		}

		private void FillPool(int poolSize)
		{
			for (int num = poolSize; num > 0; num--)
			{
				FlutteringImage flutteringImage = InstantiateNewFlutteringImage();
				flutteringImage.gameObject.SetActive(value: false);
				_flutteringImagesPool.Enqueue(flutteringImage);
			}
		}

		private void Prewarm(float seconds)
		{
			while (seconds > 0f)
			{
				seconds -= UnityEngine.Random.Range(_spawnDelayMin, _spawnDelayMax);
				FlutteringImage fromPool = GetFromPool();
				SetFlutteringImageStartingPosition(fromPool);
				if (seconds > 0f)
				{
					MoveFlutteringImage(fromPool, seconds);
				}
			}
		}

		private void ReturnToPool(FlutteringImage flutteringImage)
		{
			flutteringImage.gameObject.SetActive(value: false);
			_activeFlutteringImages.Remove(flutteringImage);
			_flutteringImagesPool.Enqueue(flutteringImage);
		}

		private FlutteringImage GetFromPool()
		{
			FlutteringImage flutteringImage;
			if (_flutteringImagesPool.Count > 0)
			{
				flutteringImage = _flutteringImagesPool.Dequeue();
				flutteringImage.gameObject.SetActive(value: true);
			}
			else
			{
				flutteringImage = InstantiateNewFlutteringImage();
				UnityEngine.Debug.LogWarning("There were not enough " + _flutteringImagePrefab.name + " instances available in the pool.");
			}
			_activeFlutteringImages.Add(flutteringImage);
			return flutteringImage;
		}

		private FlutteringImage InstantiateNewFlutteringImage()
		{
			FlutteringImage flutteringImage = UnityEngine.Object.Instantiate(_flutteringImagePrefab, base.transform);
			flutteringImage.Initialize();
			return flutteringImage;
		}
	}
}
