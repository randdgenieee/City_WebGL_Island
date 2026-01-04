using CIG;
using System.Collections.Generic;
using UnityEngine;

namespace Tweening
{
	public sealed class Tweener : MonoBehaviour
	{
		public delegate void FinishedPlayingEventHandler(Tweener tweener);

		[SerializeField]
		private bool _playOnStart;

		[SerializeField]
		private bool _loop;

		[SerializeField]
		private DeltaTimeType _deltaTimeType;

		[SerializeField]
		private List<TweenerTrackBase> _tracks;

		private float _time;

		public bool IsPlaying
		{
			get;
			private set;
		}

		public bool IsPaused
		{
			get;
			private set;
		}

		public bool IsPlaybackReversed
		{
			get;
			private set;
		}

		public float AnimationTime
		{
			get;
			private set;
		}

		public bool IsAtStart
		{
			get;
			private set;
		}

		public bool IsAtEnd
		{
			get;
			private set;
		}

		public event FinishedPlayingEventHandler FinishedPlaying;

		private void Awake()
		{
			_tracks.Sort((TweenerTrackBase a, TweenerTrackBase b) => a.Delay.CompareTo(b.Delay));
			AnimationTime = CalcHighestEndTime();
		}

		private void Start()
		{
			if (_playOnStart)
			{
				Play();
			}
		}

		private void Update()
		{
			if (!IsPlaying || IsPaused)
			{
				return;
			}
			if (_time >= AnimationTime && _loop)
			{
				_time = 0f;
				UpdateTrackComponentValues();
			}
			if (SingletonMonobehaviour<TimingView>.IsAvailable)
			{
				_time += SingletonMonobehaviour<TimingView>.Instance.GetDeltaTime(_deltaTimeType);
			}
			else
			{
				_time += Time.deltaTime;
			}
			UpdateTrackComponentValues();
			if (_time >= AnimationTime && !_loop)
			{
				UpdateTrackComponentValues(IsPlaybackReversed ? 0f : AnimationTime);
				IsPlaying = false;
				IsAtStart = IsPlaybackReversed;
				IsAtEnd = !IsPlaybackReversed;
				if (this.FinishedPlaying != null)
				{
					this.FinishedPlaying(this);
				}
			}
		}

		public void Play()
		{
			Play(playReversed: false, 0f);
		}

		public void PlayReverse()
		{
			Play(playReversed: true, 0f);
		}

		public void PlayWithOffset(float offset)
		{
			offset = ((!_loop) ? Mathf.Clamp(offset, 0f, AnimationTime) : (offset % AnimationTime));
			Play(playReversed: false, offset);
		}

		public void PlayReverseWithOffset(float offset)
		{
			offset = ((!_loop) ? Mathf.Clamp(offset, 0f, AnimationTime) : (offset % AnimationTime));
			Play(playReversed: true, offset);
		}

		public void TogglePause()
		{
			IsPaused = !IsPaused;
		}

		public void Stop()
		{
			if (!IsPlaying)
			{
				UnityEngine.Debug.LogWarning("Tweener isn't playing.");
			}
			else
			{
				IsPlaying = false;
			}
		}

		public void Reset(bool resetToEnd = false)
		{
			if (IsPlaying)
			{
				UnityEngine.Debug.LogWarning("Can't reset while playing.");
				return;
			}
			for (int num = _tracks.Count - 1; num >= 0; num--)
			{
				_tracks[num].ResetTrack();
			}
			if (resetToEnd)
			{
				UpdateTrackComponentValues(CalcHighestEndTime());
			}
			IsAtStart = !resetToEnd;
			IsAtEnd = resetToEnd;
		}

		public void StopAndReset(bool resetToEnd = false)
		{
			if (IsPlaying)
			{
				Stop();
			}
			Reset(resetToEnd);
		}

		public void PlayIfStopped()
		{
			if (!IsPlaying)
			{
				Play();
			}
		}

		public void StopIfPlaying()
		{
			if (IsPlaying)
			{
				Stop();
			}
		}

		private void Play(bool playReversed, float offset)
		{
			if (IsPlaying)
			{
				UnityEngine.Debug.LogWarning("Tweener is already playing.");
				return;
			}
			IsPlaybackReversed = playReversed;
			IsPlaying = true;
			IsPaused = false;
			IsAtStart = false;
			IsAtEnd = false;
			_time = offset;
			AnimationTime = CalcHighestEndTime();
			for (int num = _tracks.Count - 1; num >= 0; num--)
			{
				_tracks[num].ResetTrack();
			}
			UpdateTrackComponentValues();
		}

		private void UpdateTrackComponentValues()
		{
			UpdateTrackComponentValues(IsPlaybackReversed ? (AnimationTime - _time) : _time);
		}

		private void UpdateTrackComponentValues(float trackTime)
		{
			int count = _tracks.Count;
			for (int i = 0; i < count; i++)
			{
				UpdateComponentValue(_tracks[i], trackTime);
			}
		}

		private static void UpdateComponentValue(TweenerTrackBase track, float time)
		{
			if (time >= track.Delay)
			{
				track.InitializeTrack();
				float time2;
				switch (track.Curve.postWrapMode)
				{
				case WrapMode.Loop:
					time2 = (time - track.Delay) / track.Duration;
					break;
				case WrapMode.PingPong:
					time2 = Mathf.PingPong((time - track.Delay) / track.Duration, 1f);
					break;
				default:
					time2 = Mathf.Clamp01((time - track.Delay) / track.Duration);
					break;
				}
				track.UpdateComponentValue(track.Curve.Evaluate(time2));
			}
		}

		private float CalcHighestEndTime()
		{
			float num = 0f;
			int count = _tracks.Count;
			for (int i = 0; i < count; i++)
			{
				num = Mathf.Max(num, _tracks[i].EndTime);
			}
			return num;
		}
	}
}
