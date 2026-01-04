using System.Collections;

namespace CIG
{
	public class FishingMinigame
	{
		public delegate void StartedEventHandler();

		public delegate void FinishedEventHandler(int score);

		public delegate void MarkerMovedEventHandler(float position);

		public delegate void CanCatchChangedEventHandler(bool canCatch);

		public delegate void FishCaughtEventHandler(int amount);

		private const double CatchPauseDuration = 0.2;

		private readonly RoutineRunner _routineRunner;

		private readonly Timing _timing;

		private readonly FishingMinigameProperties _properties;

		private bool _isFinished;

		private int _lastCatchResult;

		private bool _canCatch;

		private bool _catchPauseActive;

		private double _endTime;

		private int _markerDirection;

		private IEnumerator _gameplayRoutine;

		public bool IsPlaying => TimeLeft > 0.0;

		public int Score
		{
			get;
			private set;
		}

		public double TimeLeft => _endTime - Timing.UtcNow;

		public float MarkerPosition
		{
			get;
			private set;
		}

		public FishingMinigameArea Area
		{
			get;
		}

		public bool CanCatch
		{
			get
			{
				return _canCatch;
			}
			private set
			{
				if (_canCatch != value)
				{
					_canCatch = value;
					FireCanCatchChangedEvent(_canCatch);
				}
			}
		}

		public event StartedEventHandler StartedEvent;

		public event FinishedEventHandler FinishedEvent;

		public event MarkerMovedEventHandler MarkerMovedEvent;

		public event CanCatchChangedEventHandler CanCatchChangedEvent;

		public event FishCaughtEventHandler FishCaughtEvent;

		private void FireStartedEvent()
		{
			this.StartedEvent?.Invoke();
		}

		private void FireFinishedEvent(int score)
		{
			this.FinishedEvent?.Invoke(score);
		}

		private void FireMarkerMovedEvent(float position)
		{
			this.MarkerMovedEvent?.Invoke(position);
		}

		private void FireCanCatchChangedEvent(bool canCatch)
		{
			this.CanCatchChangedEvent?.Invoke(canCatch);
		}

		private void FireFishCaughtEvent(int amount)
		{
			this.FishCaughtEvent?.Invoke(amount);
		}

		public FishingMinigame(RoutineRunner routineRunner, Timing timing, FishingMinigameProperties properties)
		{
			_routineRunner = routineRunner;
			_timing = timing;
			_properties = properties;
			Area = new FishingMinigameArea(_properties.GreenAreaStartSizePercentage, _properties.YellowAreaStartSizePercentage, _properties.AreaPositionPaddingPercentage, _properties.AreaScaleMultiplier);
		}

		public void StartPlaying()
		{
			Analytics.FishingMinigameStarted();
			if (_gameplayRoutine != null)
			{
				_routineRunner.StopCoroutine(_gameplayRoutine);
			}
			_routineRunner.StartCoroutine(_gameplayRoutine = GameplayRoutine());
			FireStartedEvent();
		}

		public void StopPlaying()
		{
			if (_gameplayRoutine != null)
			{
				_routineRunner.StopCoroutine(_gameplayRoutine);
				_gameplayRoutine = null;
			}
			CanCatch = false;
			_isFinished = true;
			FireFinishedEvent(Score);
		}

		public void CatchFish()
		{
			_lastCatchResult = 0;
			if (IsPlaying && !_isFinished)
			{
				_lastCatchResult = Area.CatchFish(MarkerPosition);
				Score += _lastCatchResult;
			}
			_catchPauseActive = true;
			CanCatch = false;
			FireFishCaughtEvent(_lastCatchResult);
		}

		private void MoveMarker(float deltaTime)
		{
			MarkerPosition += _properties.MarkerPercentagePerSecond * deltaTime * (float)_markerDirection;
			if (MarkerPosition >= 100f)
			{
				_markerDirection = -1;
				MarkerPosition = 0f - MarkerPosition + 200f;
				CanCatch = true;
			}
			else if (MarkerPosition <= 0f)
			{
				_markerDirection = 1;
				MarkerPosition = 0f - MarkerPosition;
				CanCatch = true;
			}
			FireMarkerMovedEvent(MarkerPosition);
		}

		private IEnumerator GameplayRoutine()
		{
			CanCatch = true;
			_endTime = Timing.UtcNow + (double)_properties.DurationSeconds;
			_markerDirection = 1;
			while (IsPlaying)
			{
				if (_catchPauseActive)
				{
					yield return new WaitForUnscaledTimeSeconds(0.2);
					_catchPauseActive = false;
					Area.MoveArea(_lastCatchResult);
				}
				else
				{
					MoveMarker(_timing.GetDeltaTime(DeltaTimeType.Unscaled));
				}
				yield return null;
			}
			CanCatch = false;
			_gameplayRoutine = null;
			StopPlaying();
		}
	}
}
