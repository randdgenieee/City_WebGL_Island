using UnityEngine;

namespace Tweening
{
	public abstract class TweenerTrackBase : MonoBehaviour
	{
		[SerializeField]
		private float _delay;

		[SerializeField]
		private float _duration;

		[SerializeField]
		private AnimationCurve _curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		private bool _initialized;

		public float Delay => _delay;

		public float Duration => _duration;

		public float EndTime => _delay + _duration;

		public AnimationCurve Curve => _curve;

		public abstract void UpdateComponentValue(float evaluatedTime);

		public abstract void ResetComponentValue();

		public void ResetTrack()
		{
			_initialized = false;
			ResetComponentValue();
		}

		public void InitializeTrack()
		{
			if (!_initialized)
			{
				ResetComponentValue();
				_initialized = true;
			}
		}
	}
}
