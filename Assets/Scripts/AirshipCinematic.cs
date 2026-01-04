using CIG;
using System.Collections;
using Tweening;
using UnityEngine;

public class AirshipCinematic : MonoBehaviour
{
	[SerializeField]
	private Camera _camera;

	[SerializeField]
	private Tweener _airShipTweener;

	[SerializeField]
	private Tweener _cameraTweener;

	[SerializeField]
	private Tweener _barsTweener;

	[SerializeField]
	private Tweener _propellorTweener;

	[SerializeField]
	private AnimatedSprite _propellorAnimatedSprite;

	[SerializeField]
	private AnimatedSprite _lightsAnimatedSprite;

	private IslandsManagerView _islandsManagerView;

	private WorldMap _worldMap;

	private AirshipState _airshipState;

	private IsometricIsland _island;

	private IEnumerator _animationRoutine;

	private bool _isPlaying;

	private void Awake()
	{
		_camera.enabled = false;
	}

	public void Initialize(IslandsManagerView islandsManagerView, WorldMap worldMap)
	{
		_islandsManagerView = islandsManagerView;
		_worldMap = worldMap;
		_airshipState = _worldMap.Airship;
		_worldMap.VisibilityChangedEvent += OnWorldMapVisibilityChanged;
		_airshipState.StateChangedEvent += OnStateChanged;
		_airShipTweener.FinishedPlaying += OnAirshipTweenerFinishedPlaying;
		_propellorTweener.FinishedPlaying += OnPropellorTweenerFinishedPlaying;
		ToggleVisibility();
		ToggleAnimations(active: false);
	}

	private void OnDestroy()
	{
		if (_worldMap != null)
		{
			_worldMap.VisibilityChangedEvent -= OnWorldMapVisibilityChanged;
			_worldMap = null;
		}
		if (_airshipState != null)
		{
			_airshipState.StateChangedEvent -= OnStateChanged;
			_airshipState = null;
		}
		if (_airShipTweener != null)
		{
			_airShipTweener.FinishedPlaying -= OnAirshipTweenerFinishedPlaying;
		}
		if (_propellorTweener != null)
		{
			_propellorTweener.FinishedPlaying -= OnPropellorTweenerFinishedPlaying;
		}
		_islandsManagerView = null;
		_island = null;
	}

	public void StartCinematic(IsometricIsland island)
	{
		if (!_isPlaying)
		{
			_island = island;
			base.gameObject.SetActive(value: true);
			_island.SetCinematicPlaying(playing: true);
			_island.IslandInput.PushDisableIslandInteractionRequest(this);
			_camera.enabled = true;
			ToggleAnimations(active: true);
			_isPlaying = true;
			StartCoroutine(_animationRoutine = AnimationRoutine());
		}
	}

	private void StartAnimation()
	{
		if (!_isPlaying)
		{
			base.gameObject.SetActive(value: true);
			_airShipTweener.StopAndReset();
			_airShipTweener.Play();
			ToggleAnimations(active: true);
			_isPlaying = true;
		}
	}

	private void StopAnimation()
	{
		if (_isPlaying)
		{
			if (_animationRoutine != null)
			{
				StopCoroutine(_animationRoutine);
				_animationRoutine = null;
			}
			_airShipTweener.StopAndReset(resetToEnd: true);
			_cameraTweener.StopAndReset(resetToEnd: true);
			_barsTweener.StopAndReset(resetToEnd: true);
			_camera.enabled = false;
			if (_island != null)
			{
				_island.SetCinematicPlaying(playing: false);
				_island.IslandInput.PopDisableIslandInteractionRequest(this);
				_island.CameraOperator.PopDisableInputRequest(this);
				_island.CameraOperator.AlignWithCamera(_camera);
				_island = null;
			}
			ToggleVisibility();
			ToggleAnimations(active: false);
			_isPlaying = false;
		}
	}

	private void ToggleVisibility()
	{
		base.gameObject.SetActive(_airshipState.CurrentIslandId == _islandsManagerView.IslandsManager.CurrentIsland);
	}

	private void ToggleAnimations(bool active)
	{
		_lightsAnimatedSprite.gameObject.SetActive(active || (_airshipState.CurrentState == AirshipState.State.Travelling && _airshipState.ToIslandId == _islandsManagerView.IslandsManager.CurrentIsland));
		if (active)
		{
			_lightsAnimatedSprite.Play();
			_propellorAnimatedSprite.Play();
			_propellorTweener.StopAndReset();
		}
		else
		{
			_lightsAnimatedSprite.Stop();
		}
	}

	private void OnWorldMapVisibilityChanged(bool visible)
	{
		if (visible)
		{
			StopAnimation();
		}
	}

	private void OnStateChanged(AirshipState.State previousstate, AirshipState.State newstate)
	{
		ToggleVisibility();
		ToggleAnimations(_isPlaying);
		if (newstate == AirshipState.State.Landed)
		{
			StartAnimation();
		}
	}

	private void OnAirshipTweenerFinishedPlaying(Tweener tweener)
	{
		StopAnimation();
		if (_propellorAnimatedSprite.enabled && !_propellorTweener.IsPlaying)
		{
			_propellorTweener.Reset();
			_propellorTweener.Play();
		}
	}

	private void OnPropellorTweenerFinishedPlaying(Tweener tweener)
	{
		if (!tweener.IsPlaybackReversed)
		{
			_propellorAnimatedSprite.Stop();
		}
	}

	private IEnumerator AnimationRoutine()
	{
		_airShipTweener.StopAndReset();
		_barsTweener.StopAndReset();
		_cameraTweener.StopAndReset();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitUntil(() => !SingletonMonobehaviour<CinematicEffect>.Instance.IsShowing);
		_airShipTweener.Play();
		_barsTweener.Play();
		_cameraTweener.Play();
		_animationRoutine = null;
	}
}
