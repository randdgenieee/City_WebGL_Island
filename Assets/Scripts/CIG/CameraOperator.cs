using System;
using UnityEngine;

namespace CIG
{
	public abstract class CameraOperator : MonoBehaviour
	{
		[Flags]
		protected enum AnimationMode
		{
			None = 0x0,
			Velocity = 0x1,
			Destination = 0x2,
			Zoom = 0x4,
			DestinationAndZoom = 0x6
		}

		private const float VelocityReductionFactor = 0.95f;

		protected const float VelocityStartMagnitudeThreshold = 30f;

		private const float VelocityEndMagnitudeThreshold = 5f;

		private const float DefaultAnimationDuration = 0.6f;

		private static readonly AnimationCurve DestinationAnimationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		private const float CameraYOffset = -250f;

		protected Timing _timing;

		protected AnimationMode _animationMode;

		private Vector3 _animationVelocity;

		private Vector2 _animationSource;

		private Vector2 _animationDestination;

		private float _animationZoomSource;

		private float _animationZoomDestination;

		private TimeSpan _animationStartTime;

		private float _animationDuration;

		protected abstract float Zoom
		{
			get;
			set;
		}

		protected abstract Vector2 CameraPosition
		{
			get;
			set;
		}

		protected void InitializeCameraOperator(Timing timing)
		{
			_timing = timing;
			_animationMode = AnimationMode.None;
		}

		private void OnDestroy()
		{
			if (SingletonMonobehaviour<FPSLimiter>.IsAvailable)
			{
				SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
			}
		}

		public void ScrollTo(GameObject go, float animationDuration = 0.6f)
		{
			Collider2D component = go.GetComponent<Collider2D>();
			Vector2 position = (component != null) ? new Vector3(component.bounds.center.x, component.bounds.center.y + -250f) : new Vector3(go.transform.position.x, go.transform.position.y + -250f);
			ScrollTo(position, animationDuration);
		}

		public void ScrollTo(Vector2 position, float animationDuration = 0.6f)
		{
			_animationSource = CameraPosition;
			_animationDestination = position;
			_animationStartTime = _timing.AnimationTimeSpan;
			_animationDuration = animationDuration;
			_animationMode = AnimationMode.Destination;
			SingletonMonobehaviour<FPSLimiter>.Instance.PushUnlimitedFPSRequest(this);
		}

		public void ZoomTo(float toZoom, float? fromZoom = default(float?), float animationDuration = 0.6f)
		{
			_animationZoomSource = (fromZoom ?? Zoom);
			_animationZoomDestination = toZoom;
			_animationStartTime = _timing.AnimationTimeSpan;
			_animationDuration = animationDuration;
			_animationMode = AnimationMode.Zoom;
			SingletonMonobehaviour<FPSLimiter>.Instance.PushUnlimitedFPSRequest(this);
		}

		public void ScrollAndZoom(GameObject go, float toZoom, float? fromZoom = default(float?), Vector2? offset = default(Vector2?), float animationDuration = 0.6f)
		{
			Collider2D component = go.GetComponent<Collider2D>();
			Vector2 vector = (component != null) ? component.bounds.center : go.transform.position;
			if (offset.HasValue)
			{
				vector += offset.Value;
			}
			ScrollAndZoom(vector, toZoom, fromZoom, animationDuration);
		}

		public void ScrollAndZoom(Vector2 position, float toZoom, float? fromZoom = default(float?), float animationDuration = 0.6f)
		{
			_animationSource = CameraPosition;
			_animationDestination = position;
			_animationZoomSource = (fromZoom ?? Zoom);
			_animationZoomDestination = toZoom;
			_animationStartTime = _timing.AnimationTimeSpan;
			_animationDuration = animationDuration;
			_animationMode = AnimationMode.DestinationAndZoom;
			SingletonMonobehaviour<FPSLimiter>.Instance.PushUnlimitedFPSRequest(this);
		}

		protected bool HandleAnimations()
		{
			if (_animationMode == AnimationMode.None)
			{
				return false;
			}
			if (_animationMode == AnimationMode.Velocity)
			{
				_animationVelocity *= 0.95f;
				OnVelocityAnimation(_animationVelocity, _timing.GetDeltaTime(DeltaTimeType.Animation));
				if (_animationVelocity.magnitude < 5f)
				{
					_animationMode = AnimationMode.None;
					SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
				}
			}
			else
			{
				float num = DestinationAnimationCurve.Evaluate((float)(_timing.AnimationTimeSpan - _animationStartTime).TotalSeconds / _animationDuration);
				bool flag = num < 1f;
				if (Contains(_animationMode, AnimationMode.Destination))
				{
					OnDestinationAnimation(num, flag, _animationSource, _animationDestination);
				}
				if (Contains(_animationMode, AnimationMode.Zoom))
				{
					OnZoomAnimation(num, flag, _animationZoomSource, _animationZoomDestination);
				}
				if (!flag)
				{
					_animationMode = AnimationMode.None;
					SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
				}
			}
			return true;
		}

		protected void SetVelocityAnimation(Vector3 velocity)
		{
			_animationVelocity = velocity;
			_animationMode = AnimationMode.Velocity;
		}

		protected abstract void OnVelocityAnimation(Vector3 velocity, float deltaTime);

		protected abstract void OnDestinationAnimation(float progress, bool inProgress, Vector2 fromPosition, Vector2 toPosition);

		protected abstract void OnZoomAnimation(float progress, bool inProgress, float fromZoom, float toZoom);

		private bool Contains(AnimationMode animationModes, AnimationMode animationMode)
		{
			return (animationModes & animationMode) != AnimationMode.None;
		}
	}
}
