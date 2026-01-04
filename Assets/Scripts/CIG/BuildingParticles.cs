using System.Collections;
using UnityEngine;

namespace CIG
{
	public class BuildingParticles : MonoBehaviour
	{
		public delegate void PlayedEventHandler();

		[SerializeField]
		private ParticleSystem _backgroundParticles;

		[SerializeField]
		private ParticleSystem _foregroundParticles;

		private bool _isLooping;

		private float _minWaitTime;

		private float _maxWaitTime;

		private IEnumerator _playRoutine;

		public event PlayedEventHandler PlayedEvent;

		private void FirePlayedEvent()
		{
			this.PlayedEvent?.Invoke();
		}

		public void Initialize(GridTile gridTile)
		{
			base.transform.localScale = new Vector3(gridTile.Properties.Size.u, gridTile.Properties.Size.v, 1f);
			SetSortingOrder(gridTile.SpriteRenderer.sortingOrder);
		}

		private void OnEnable()
		{
			if (_isLooping)
			{
				StartPlayRoutine();
			}
		}

		private void OnDisable()
		{
			StopPlayRoutine();
		}

		public void Play()
		{
			if (_backgroundParticles != null)
			{
				_backgroundParticles.Play();
			}
			if (_foregroundParticles != null)
			{
				_foregroundParticles.Play();
			}
			FirePlayedEvent();
		}

		public void Play(float minWaitTime, float maxWaitTime)
		{
			_isLooping = true;
			_minWaitTime = minWaitTime;
			_maxWaitTime = maxWaitTime;
			StartPlayRoutine();
		}

		public void Stop()
		{
			_isLooping = false;
			StopPlayRoutine();
		}

		public void SetSortingOrder(int sortingOrder)
		{
			if (_backgroundParticles != null)
			{
				_backgroundParticles.GetComponent<Renderer>().sortingOrder = sortingOrder - 1;
			}
			if (_foregroundParticles != null)
			{
				_foregroundParticles.GetComponent<Renderer>().sortingOrder = sortingOrder + 1;
			}
		}

		private void StartPlayRoutine()
		{
			if (base.gameObject.activeInHierarchy && _playRoutine == null)
			{
				StartCoroutine(_playRoutine = PlayRoutine(_minWaitTime, _maxWaitTime));
			}
		}

		private void StopPlayRoutine()
		{
			if (_playRoutine != null)
			{
				StopCoroutine(_playRoutine);
				_playRoutine = null;
			}
		}

		private IEnumerator PlayRoutine(float minWaitTime, float maxWaitTime)
		{
			while (true)
			{
				yield return new WaitForSeconds(UnityEngine.Random.Range(minWaitTime, maxWaitTime));
				Play();
			}
		}
	}
}
