using CIG;
using CIG.Translation;
using Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AirshipView : MovingImageAgent
{
	public delegate void AirshipAnimationFinishedHandler();

	private const double TakeOffTweenerDuration = 2.5;

	private const double TakeOffBoostDuration = 2.0;

	private const float SpeedupDuration = 2f;

	[SerializeField]
	private Graphic[] _graphicRaycasters;

	[SerializeField]
	private Tweener _hoverTweener;

	[SerializeField]
	private Tweener _takeOffTweener;

	[SerializeField]
	private Tweener _landTweener;

	[SerializeField]
	private AnimatedSpriteBase[] _animatedSprites;

	private PopupManager _popupManager;

	private IslandsManager _islandsManager;

	private WorldMapView _worldMapView;

	private OverlayManager _overlayManager;

	private AirshipState _state;

	private ProgressOverlay _progressOverlay;

	private float _startAngle;

	private float _targetAngle;

	private double _speedupStartTime;

	private Vector3? _speedupStartPosition;

	private Vector3 _boostedStartPosition;

	private Vector3 _fromIslandPosition;

	private Vector3 _toIslandPosition;

	public bool IsFlying
	{
		get
		{
			if (_state.CurrentState != 0)
			{
				return IsUpspeeding;
			}
			return true;
		}
	}

	protected override bool CanUpdate
	{
		get
		{
			if (!base.CanUpdate || _state == null || _state.CurrentState != 0)
			{
				return IsUpspeeding;
			}
			return true;
		}
	}

	private bool IsUpspeeding => _speedupStartPosition.HasValue;

	public event AirshipAnimationFinishedHandler AirshipAnimationFinishedEvent;

	public void FireAirshipAnimationFinishedEvent()
	{
		this.AirshipAnimationFinishedEvent?.Invoke();
	}

	public void Initialize(AirshipState state, Timing timing, PopupManager popupManager, IslandsManager islandsManager, WorldMapView worldMapView, OverlayManager overlayManager)
	{
		_state = state;
		_popupManager = popupManager;
		_islandsManager = islandsManager;
		_worldMapView = worldMapView;
		_overlayManager = overlayManager;
		_state.StateChangedEvent += OnStateChanged;
		EnterState(_state.CurrentState);
		InitializeMovingAgent(timing);
		_islandsManager.VisitingStartedEvent += OnVisitingStarted;
		_islandsManager.VisitingStoppedEvent += OnVisitingStopped;
		_takeOffTweener.FinishedPlaying += OnTakeOffTweenerFinishedPlaying;
	}

	private void OnEnable()
	{
		if (_state != null)
		{
			_state.StateChangedEvent += OnStateChanged;
			EnterState(_state.CurrentState);
		}
	}

	private void OnDisable()
	{
		if (_state != null)
		{
			_state.StateChangedEvent -= OnStateChanged;
		}
	}

	private void OnDestroy()
	{
		if (_islandsManager != null)
		{
			_islandsManager.VisitingStartedEvent -= OnVisitingStarted;
			_islandsManager.VisitingStoppedEvent -= OnVisitingStopped;
			_islandsManager = null;
		}
		if (_takeOffTweener != null)
		{
			_takeOffTweener.FinishedPlaying -= OnTakeOffTweenerFinishedPlaying;
		}
	}

	public void OnClicked()
	{
		if (_state.CurrentState == AirshipState.State.Travelling)
		{
			Sprite asset = SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.BalloonIcon);
			Sprite asset2 = SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.WaitIcon);
			Sprite asset3 = SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.GenericPopupDefaultIconBackground);
			SpeedupPopupRequest request = new SpeedupPopupRequest(_state.TravelUpspeedableProcess, Localization.Key("airship"), Localization.Key("traveling"), Localization.Key("wait"), null, asset2, asset, null, asset3, "airship");
			_popupManager.RequestPopup(request);
		}
	}

	protected override void SetInitialPosition()
	{
		_position = _worldMapView.GetWorldMapIslandPosition((_state.CurrentState == AirshipState.State.Travelling) ? _state.FromIslandId : _state.CurrentIslandId);
		if (_state.CurrentState == AirshipState.State.Travelling)
		{
			CalculateAngles();
		}
	}

	protected override void UpdatePositionAndAngle(double deltaTime)
	{
		if (IsUpspeeding)
		{
			float num = (float)(_timing.GameTime - _speedupStartTime) / 2f;
			_position = Vector3.Lerp(_speedupStartPosition.Value, _toIslandPosition, num);
			_angle = _targetAngle;
			if (num >= 1f)
			{
				_speedupStartPosition = null;
				PerformLandingTweeners(_state.CurrentState);
				FireAirshipAnimationFinishedEvent();
			}
		}
		else
		{
			if (_state.CurrentState != 0)
			{
				return;
			}
			double num2 = _state.TravelDuration - _state.TravelTimeLeft;
			if (num2 < 2.5)
			{
				_position = _fromIslandPosition;
				float num3 = Mathf.LerpAngle(_startAngle, _targetAngle, (float)(num2 / 2.5));
				if (num3 < 0f)
				{
					num3 += 360f;
				}
				_angle = num3;
			}
			else if (num2 < 4.5)
			{
				float t = (float)((num2 - 2.5) / 2.0);
				_position = Vector3.Lerp(_fromIslandPosition, _boostedStartPosition, t);
				_angle = _targetAngle;
			}
			else
			{
				float t2 = (float)((num2 - 2.5) / (_state.TravelDuration - 2.5));
				_position = Vector3.Lerp(_boostedStartPosition, _toIslandPosition, t2);
				_angle = _targetAngle;
			}
		}
	}

	private void OnSpeedupped()
	{
		_speedupStartTime = _timing.GameTime;
		_speedupStartPosition = _position;
	}

	private void CalculateAngles()
	{
		_startAngle = _angle;
		if (_state.ToIslandId == IslandId.None)
		{
			_targetAngle = 0f;
			return;
		}
		Vector3 worldMapIslandPosition = _worldMapView.GetWorldMapIslandPosition(_state.FromIslandId);
		Vector3 worldMapIslandPosition2 = _worldMapView.GetWorldMapIslandPosition(_state.ToIslandId);
		Vector3 vector = worldMapIslandPosition - worldMapIslandPosition2;
		float num = Vector3.Angle(Vector3.up, vector);
		if (vector.x < 0f)
		{
			num = 360f - num;
		}
		_targetAngle = num;
	}

	private void ShowTravelProgressOverlay(UpspeedableProcess process)
	{
		if (_progressOverlay == null)
		{
			_progressOverlay = _overlayManager.CreateOverlay<ProgressOverlay>(base.gameObject, OverlayType.Progress);
			_progressOverlay.Initialize(process, 36f);
		}
	}

	private void RemoveTravelProgressOverlay()
	{
		if (_progressOverlay != null)
		{
			_progressOverlay.Remove();
			_progressOverlay = null;
		}
	}

	private void EnterState(AirshipState.State newState)
	{
		SetState(null, newState);
	}

	private void OnStateChanged(AirshipState.State previousState, AirshipState.State newState)
	{
		SetState(previousState, newState);
	}

	private void SetState(AirshipState.State? previousState, AirshipState.State newState)
	{
		int i = 0;
		for (int num = _graphicRaycasters.Length; i < num; i++)
		{
			_graphicRaycasters[i].raycastTarget = (newState == AirshipState.State.Travelling);
		}
		switch (newState)
		{
		case AirshipState.State.Travelling:
			_speedupStartPosition = null;
			_state.UpspeededEvent += OnSpeedupped;
			_fromIslandPosition = _worldMapView.GetWorldMapIslandPosition(_state.FromIslandId);
			_toIslandPosition = _worldMapView.GetWorldMapIslandPosition(_state.ToIslandId);
			_boostedStartPosition = Vector3.Lerp(_fromIslandPosition, _toIslandPosition, 0.3f);
			CalculateAngles();
			ShowTravelProgressOverlay(_state.TravelUpspeedableProcess);
			SetAnimatedSpritesPlaying(playing: true);
			_takeOffTweener.StopAndReset();
			_landTweener.StopAndReset();
			_hoverTweener.StopAndReset();
			if (previousState.HasValue && previousState.Value == AirshipState.State.Landed)
			{
				_takeOffTweener.Play();
			}
			break;
		case AirshipState.State.Landed:
			base.transform.localPosition = _worldMapView.GetWorldMapIslandPosition(_state.CurrentIslandId);
			RemoveTravelProgressOverlay();
			if (!IsUpspeeding)
			{
				PerformLandingTweeners(previousState);
				FireAirshipAnimationFinishedEvent();
			}
			break;
		case AirshipState.State.Hovering:
			base.transform.localPosition = _worldMapView.GetWorldMapIslandPosition(_state.CurrentIslandId);
			RemoveTravelProgressOverlay();
			SetAnimatedSpritesPlaying(playing: true);
			_takeOffTweener.StopAndReset();
			_landTweener.StopAndReset();
			_hoverTweener.PlayIfStopped();
			break;
		}
	}

	private void PerformLandingTweeners(AirshipState.State? previousState)
	{
		if (_state.CurrentState == AirshipState.State.Landed)
		{
			SetAnimatedSpritesPlaying(playing: false);
			_hoverTweener.StopAndReset();
			_takeOffTweener.StopAndReset();
			if (!previousState.HasValue)
			{
				_landTweener.StopAndReset(resetToEnd: true);
				return;
			}
			_landTweener.StopAndReset();
			_landTweener.Play();
		}
	}

	private void OnVisitingStarted()
	{
		base.gameObject.SetActive(value: false);
	}

	private void OnVisitingStopped()
	{
		base.gameObject.SetActive(value: true);
	}

	private void OnTakeOffTweenerFinishedPlaying(Tweener tweener)
	{
		if (_state.CurrentState == AirshipState.State.Travelling)
		{
			_hoverTweener.Play();
		}
	}

	private void SetAnimatedSpritesPlaying(bool playing)
	{
		int i = 0;
		for (int num = _animatedSprites.Length; i < num; i++)
		{
			AnimatedSpriteBase animatedSpriteBase = _animatedSprites[i];
			animatedSpriteBase.Reset();
			if (playing && !animatedSpriteBase.IsPlaying)
			{
				animatedSpriteBase.Play();
			}
			else if (!playing && animatedSpriteBase.IsPlaying)
			{
				animatedSpriteBase.Stop();
			}
		}
	}
}
