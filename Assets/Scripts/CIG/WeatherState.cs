using System;
using System.Collections;
using Tweening;
using UnityEngine;

namespace CIG
{
	public class WeatherState : MonoBehaviour
	{
		[SerializeField]
		private Tweener[] _tweeners;

		[SerializeField]
		private float _minDuration;

		[SerializeField]
		private float _maxDuration;

		[SerializeField]
		private float _weight;

		private Timing _timing;

		private RoutineRunner _routineRunner;

		private double _longestTweenDuration;

		private IEnumerator _currentRoutine;

		private double _startTime;

		private double _duration;

		public float Weight => _weight;

		public void Initialize(Timing timing, RoutineRunner routineRunner)
		{
			_timing = timing;
			_routineRunner = routineRunner;
			_longestTweenDuration = 0.0;
			int i = 0;
			for (int num = _tweeners.Length; i < num; i++)
			{
				_longestTweenDuration = Math.Max(_longestTweenDuration, _tweeners[i].AnimationTime);
			}
		}

		private void OnDestroy()
		{
			if (_routineRunner != null && _currentRoutine != null)
			{
				_routineRunner.StopCoroutine(_currentRoutine);
				_currentRoutine = null;
			}
		}

		public void EnterStateNow()
		{
			int i = 0;
			for (int num = _tweeners.Length; i < num; i++)
			{
				_tweeners[i].StopAndReset(resetToEnd: true);
			}
		}

		public IEnumerator StateRoutine(double startTime, double duration)
		{
			_startTime = startTime;
			_duration = duration;
			if (_timing.GameTime < _startTime + _longestTweenDuration)
			{
				yield return EnteringState();
			}
			double exitTime = _startTime + _duration;
			if (_timing.GameTime < exitTime - _longestTweenDuration)
			{
				yield return EnteredState();
			}
			if (_timing.GameTime < exitTime)
			{
				yield return ExitingState();
			}
			ExitedState();
		}

		public double GetDuration()
		{
			return UnityEngine.Random.Range(_minDuration, _maxDuration);
		}

		private IEnumerator EnteringState()
		{
			float offset = (float)(_timing.GameTime - _startTime);
			int i = 0;
			for (int num = _tweeners.Length; i < num; i++)
			{
				_tweeners[i].PlayWithOffset(offset);
			}
			double num2 = _startTime + _longestTweenDuration;
			yield return new WaitForGameTimeSeconds(_timing, num2 - _timing.GameTime);
		}

		private IEnumerator EnteredState()
		{
			EnterStateNow();
			double num = _startTime + _duration - _longestTweenDuration;
			yield return new WaitForGameTimeSeconds(_timing, num - _timing.GameTime);
		}

		private IEnumerator ExitingState()
		{
			double num = _startTime + _duration;
			double num2 = num - _longestTweenDuration;
			float offset = (float)(_timing.GameTime - num2);
			int i = 0;
			for (int num3 = _tweeners.Length; i < num3; i++)
			{
				_tweeners[i].PlayReverseWithOffset(offset);
			}
			yield return new WaitForGameTimeSeconds(_timing, num - _timing.GameTime);
		}

		private void ExitedState()
		{
			int i = 0;
			for (int num = _tweeners.Length; i < num; i++)
			{
				_tweeners[i].StopAndReset();
			}
		}
	}
}
