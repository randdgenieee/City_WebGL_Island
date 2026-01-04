using UnityEngine;

namespace CIG
{
	public abstract class AnimatedSpriteBase : MonoBehaviour
	{
		[SerializeField]
		protected float _framesPerSecond = 12f;

		[SerializeField]
		protected Sprite[] _sprites;

		[SerializeField]
		protected bool _playOnStart = true;

		[SerializeField]
		protected AnimationMode _animationMode;

		[SerializeField]
		[ShowWhen("_animationMode", ComparisonType.Equals, new object[]
		{
			AnimationMode.PingPong,
			AnimationMode.Loop
		})]
		protected float _waitAtEndSeconds;

		[SerializeField]
		[ShowWhen("_animationMode", ComparisonType.Equals, new object[]
		{
			AnimationMode.SyncedLoop
		})]
		protected float _syncInterval;

		[SerializeField]
		protected DeltaTimeType _deltaTimeType;

		private int _spriteIndex;

		private bool _isPlaying;

		private float _currentTime;

		private float _lastFrameTime;

		private int _pingpongSign;

		private float _waitUntilTime;

		public float FPS
		{
			get
			{
				return _framesPerSecond;
			}
			set
			{
				_framesPerSecond = value;
			}
		}

		public Sprite[] Sprites => _sprites;

		public bool PlayOnStart => _playOnStart;

		public AnimationMode AnimationMode
		{
			get
			{
				return _animationMode;
			}
			set
			{
				if (_animationMode != value)
				{
					Reset();
					_animationMode = value;
				}
			}
		}

		public float WaitAtEndSeconds
		{
			get
			{
				return _waitAtEndSeconds;
			}
			set
			{
				if (!Mathf.Approximately(_waitAtEndSeconds, value))
				{
					Reset();
					_waitAtEndSeconds = value;
				}
			}
		}

		public bool IsPlaying => _isPlaying;

		private void Start()
		{
			if (_sprites != null && _playOnStart && _animationMode != AnimationMode.SyncedLoop)
			{
				Play();
			}
		}

		private void OnEnable()
		{
			if (_sprites != null && _playOnStart && _animationMode == AnimationMode.SyncedLoop)
			{
				float num = Time.unscaledTime % _syncInterval;
				int num2 = _sprites.Length;
				if (_framesPerSecond / (float)num2 > num)
				{
					_currentTime = num;
					_spriteIndex = Mathf.RoundToInt(_framesPerSecond * num);
					Animate(0f);
					_isPlaying = true;
				}
				this.Invoke(SyncedPlay, _syncInterval - num, realtime: true);
			}
		}

		private void OnDisable()
		{
			if (_animationMode == AnimationMode.SyncedLoop)
			{
				_isPlaying = false;
				Animate(0f);
				Reset();
				this.CancelInvoke(SyncedPlay);
			}
		}

		private void Update()
		{
			if (_isPlaying)
			{
				Animate(SingletonMonobehaviour<TimingView>.IsAvailable ? SingletonMonobehaviour<TimingView>.Instance.GetDeltaTime(_deltaTimeType) : Time.deltaTime);
			}
		}

		public void ReplaceSprites(Sprite[] sprites)
		{
			float num = (float)_spriteIndex / (float)_sprites.Length;
			_sprites = sprites;
			_spriteIndex = Mathf.Clamp(Mathf.FloorToInt((float)_sprites.Length * num), 0, _sprites.Length - 1);
			UpdateSprite(_sprites[_spriteIndex]);
		}

		public void Play()
		{
			_isPlaying = true;
			Animate(0f);
		}

		public void Pause()
		{
			_isPlaying = false;
		}

		public void Stop()
		{
			_isPlaying = false;
			_playOnStart = false;
			Reset();
		}

		public void Reset()
		{
			_currentTime = 0f;
			_lastFrameTime = 0f;
			_spriteIndex = 0;
			_pingpongSign = 0;
			_waitUntilTime = 0f;
		}

		protected abstract void UpdateSprite(Sprite sprite);

		private void SyncedPlay()
		{
			Reset();
			Play();
			this.Invoke(SyncedPlay, _syncInterval - Time.unscaledTime % _syncInterval, realtime: true);
		}

		private void Animate(float deltaTime)
		{
			int num = (_sprites != null) ? _sprites.Length : 0;
			if (num > 0)
			{
				float num2 = 1f / _framesPerSecond;
				_currentTime += deltaTime;
				if (_currentTime >= _waitUntilTime && _currentTime - _lastFrameTime > num2)
				{
					_lastFrameTime = _currentTime;
					switch (_animationMode)
					{
					case AnimationMode.Loop:
						_spriteIndex = (_spriteIndex + 1) % num;
						if (_spriteIndex == num - 1)
						{
							_waitUntilTime = _currentTime + _waitAtEndSeconds;
						}
						break;
					case AnimationMode.PingPong:
						_spriteIndex += _pingpongSign;
						if (_spriteIndex >= num - 1)
						{
							_pingpongSign = -1;
							_spriteIndex = num - 1;
							_waitUntilTime = _currentTime + _waitAtEndSeconds;
						}
						else if (_spriteIndex <= 0)
						{
							_pingpongSign = 1;
							_spriteIndex = 0;
							_waitUntilTime = _currentTime + _waitAtEndSeconds;
						}
						break;
					case AnimationMode.Random:
						_spriteIndex = UnityEngine.Random.Range(0, num);
						break;
					case AnimationMode.Single:
					case AnimationMode.SyncedLoop:
						_spriteIndex++;
						if (_spriteIndex >= num - 1)
						{
							Pause();
						}
						break;
					}
				}
				_spriteIndex = Mathf.Clamp(_spriteIndex, 0, num - 1);
				UpdateSprite(_sprites[_spriteIndex]);
			}
			else
			{
				UnityEngine.Debug.LogWarningFormat("[AnimatedSprite] Missing Sprites for '{0}'", base.name);
			}
		}
	}
}
