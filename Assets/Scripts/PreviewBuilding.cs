using CIG;
using System;
using System.Collections.Generic;
using Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class PreviewBuilding : GridTile, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler, IPinchHandler, IPointerEnterHandler
{
	[Serializable]
	private class GroundSprites
	{
		[SerializeField]
		private SurfaceType _surfaceType;

		[SerializeField]
		private Sprite _floorSprite;

		[SerializeField]
		private Sprite _holeSprite;

		public SurfaceType SurfaceType => _surfaceType;

		public Sprite FloorSprite => _floorSprite;

		public Sprite HoleSprite => _holeSprite;
	}

	private static readonly Vector2 DefaultHoleSize = new Vector2(2f, 2f);

	private static readonly Vector3 DefaultArrowNWLocalPosition = new Vector3(-50f, 75f, 0f);

	private static readonly Vector3 DefaultArrowNELocalPosition = new Vector3(50f, 75f, 0f);

	private static readonly Vector3 DefaultArrowSWLocalPosition = new Vector3(-50f, 25f, 0f);

	private static readonly Vector3 DefaultArrowSELocalPosition = new Vector3(50f, 25f, 0f);

	[SerializeField]
	private PolygonCollider2D _buildingCollider;

	[SerializeField]
	private PolygonCollider2D _floorCollider;

	[SerializeField]
	private Transform _buildingHover;

	[SerializeField]
	private Transform[] _buildingTileTransforms;

	[Header("Tweeners")]
	[SerializeField]
	private Tweener _buildIntroTweener;

	[SerializeField]
	private Tweener _moveIntroTweener;

	[SerializeField]
	private Tweener _outroTweener;

	[SerializeField]
	private Tweener _idleTweener;

	[SerializeField]
	private Tweener _flipIntroTweener;

	[SerializeField]
	private Tweener _flipOutroTweener;

	[Header("Arrows")]
	[SerializeField]
	private Transform _arrowNW;

	[SerializeField]
	private Transform _arrowNE;

	[SerializeField]
	private Transform _arrowSW;

	[SerializeField]
	private Transform _arrowSE;

	[Header("Particles")]
	[SerializeField]
	private ParticleSystem _outroParticles;

	[SerializeField]
	private Renderer _outroParticlesRenderer;

	[SerializeField]
	private ParticleSystem _buildIntroParticles;

	[SerializeField]
	private ParticleSystem _moveIntroParticles;

	[Header("Sprites")]
	[SerializeField]
	private SpriteRenderer _floor;

	[SerializeField]
	private SpriteRenderer _hole;

	[SerializeField]
	private List<GroundSprites> _groundSprites;

	[SerializeField]
	private GroundSprites _fallbackSprites;

	[SerializeField]
	private BoostUnderlay _boostUnderlay;

	private Builder _builder;

	private Vector3 _buildingHoverOffset;

	private Vector3 _tileScale;

	private Action<CIGBuilding> _outroTweenerFinishedCallback;

	private bool _transferDrag = true;

	public CIGBuilding Building
	{
		get;
		private set;
	}

	protected override void OnDestroy()
	{
		_buildIntroTweener.FinishedPlaying -= OnIntroFinishedPlaying;
		_moveIntroTweener.FinishedPlaying -= OnIntroFinishedPlaying;
		_outroTweener.FinishedPlaying -= OnOutroFinishedPlaying;
		_flipIntroTweener.FinishedPlaying -= OnFlipIntroFinishedPlaying;
		_outroTweenerFinishedCallback = null;
		if (SingletonMonobehaviour<FPSLimiter>.IsAvailable)
		{
			SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
		}
		base.OnDestroy();
	}

	void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
	{
		SingletonMonobehaviour<FPSLimiter>.Instance.PushUnlimitedFPSRequest(this);
	}

	void IDragHandler.OnDrag(PointerEventData eventData)
	{
		Vector3 vector = eventData.pointerCurrentRaycast.worldPosition - new Vector3(0f, IsometricGrid.ElementSize.y * (float)Building.Properties.Size.v / 2f);
		GridPoint gridPointForWorldPosition = _isometricGrid.GetGridPointForWorldPosition(vector);
		GridIndex gridIndex = new GridIndex(gridPointForWorldPosition);
		if (_isometricGrid.IsWithinBounds(gridIndex))
		{
			UpdatePosition(gridIndex, vector - _isometricGrid.GetWorldPositionForGridIndex(gridIndex), moving: true);
		}
	}

	void IEndDragHandler.OnEndDrag(PointerEventData eventData)
	{
		SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
		UpdatePosition(base.Index, base.Element.Origin, moving: false);
	}

	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
	{
		if (eventData.pointerPress != null && _transferDrag)
		{
			eventData.pointerPress = eventData.pointerEnter;
			eventData.pointerPressRaycast = eventData.pointerCurrentRaycast;
			eventData.pointerDrag = eventData.pointerEnter;
			_transferDrag = false;
		}
	}

	void IPinchHandler.OnPinch(PinchEventData pinchEvent)
	{
	}

	public void Initialize(IsometricGrid isometricGrid, IslandsManagerView islandsManagerView, WorldMap worldMap, BuildingWarehouseManager buildingWarehouseManager, CraneManager craneManager, GameStats gameStats, GameState gameState, PopupManager popupManager, Multipliers multipliers, Timing timing, RoutineRunner routineRunner, OverlayManager overlayManager, CIGBuilding building, bool isNewBuilding, GridIndex startIndex, Builder builder, CIGIslandState islandState)
	{
		_builder = builder;
		Initialize(new StorageDictionary(), isometricGrid, islandsManagerView, worldMap, buildingWarehouseManager, craneManager, gameStats, gameState, popupManager, multipliers, timing, routineRunner, building.Properties, overlayManager, islandState);
		Building = building;
		CopyGridTile(Building);
		_canHide = false;
		_buildingHoverOffset = _buildingHover.localPosition;
		base.name = "(PREVIEW) " + Building.name;
		InitFloorAndArrows();
		SetBoostUnderlay();
		_buildingCollider.CopyPathsFrom(Building.Collider);
		_floorCollider.CopyPathsFrom(Building.FloorCollider);
		if (isNewBuilding)
		{
			base.Status = GridTileStatus.Preview;
			Building.Status = GridTileStatus.Preview;
			_buildIntroTweener.FinishedPlaying += OnIntroFinishedPlaying;
			_buildIntroTweener.Play();
			_buildIntroParticles.Play();
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.NewBuilding);
		}
		else
		{
			base.Status = GridTileStatus.Moving;
			Building.Status = GridTileStatus.Moving;
			OnHiddenChanged(base.Hidden);
			_moveIntroTweener.FinishedPlaying += OnIntroFinishedPlaying;
			_moveIntroTweener.Play();
			_moveIntroParticles.Play();
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.LiftBuilding);
		}
		UpdatePosition(startIndex, isometricGrid.GetWorldPositionForGridIndex(startIndex), moving: false);
		Building.gameObject.SetActive(value: false);
		_outroTweener.FinishedPlaying += OnOutroFinishedPlaying;
		_flipIntroTweener.FinishedPlaying += OnFlipIntroFinishedPlaying;
	}

	public void UpdateMirrored(bool mirrored)
	{
		if (base.Mirrored != mirrored)
		{
			if (_flipIntroTweener.IsPlaying)
			{
				_flipIntroTweener.Stop();
			}
			if (_flipOutroTweener.IsPlaying)
			{
				_flipOutroTweener.Stop();
			}
			_flipIntroTweener.Play();
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.FlipBuilding);
		}
	}

	public void CancelBuild()
	{
		SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.DropBuilding);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public GridTile CancelMove()
	{
		Building.transform.parent = base.transform.parent;
		Building.SetColor(Color.white);
		Building.gameObject.SetActive(value: true);
		return Building;
	}

	public CIGBuilding FinishBuild(Action<CIGBuilding> animationFinishedCallback)
	{
		_idleTweener.StopAndReset();
		_outroTweenerFinishedCallback = animationFinishedCallback;
		_outroTweener.Play();
		return Building;
	}

	private void InitFloorAndArrows()
	{
		_tileScale = new Vector3((float)Building.Properties.Size.u / DefaultHoleSize.x, (float)Building.Properties.Size.v / DefaultHoleSize.y, 1f);
		int i = 0;
		for (int num = _buildingTileTransforms.Length; i < num; i++)
		{
			_buildingTileTransforms[i].localScale = _tileScale;
		}
		GroundSprites groundSprites = _groundSprites.Find((GroundSprites x) => x.SurfaceType == base.Properties.SurfaceType);
		if (groundSprites == null)
		{
			UnityEngine.Debug.LogWarningFormat("Using a fallback for the required surfacetype ({0})", base.Properties.SurfaceType);
			groundSprites = _fallbackSprites;
		}
		_floor.sprite = groundSprites.FloorSprite;
		_hole.sprite = groundSprites.HoleSprite;
		_arrowNW.localPosition = Vector3.Scale(DefaultArrowNWLocalPosition, _tileScale);
		_arrowNE.localPosition = Vector3.Scale(DefaultArrowNELocalPosition, _tileScale);
		_arrowSW.localPosition = Vector3.Scale(DefaultArrowSWLocalPosition, _tileScale);
		_arrowSE.localPosition = Vector3.Scale(DefaultArrowSELocalPosition, _tileScale);
	}

	private void UpdatePosition(GridIndex newIndex, Vector3 offset, bool moving)
	{
		if (base.Index.u != newIndex.u || base.Index.v != newIndex.v)
		{
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.MoveBuilding);
		}
		base.Index = newIndex;
		UpdateColor(_builder.CanBuild());
		if (base.Mirrored)
		{
			offset.x *= -1f;
		}
		offset = (moving ? (offset + 2f * _buildingHoverOffset) : _buildingHoverOffset);
		_buildingHover.localPosition = offset;
	}

	private void UpdateColor(bool canBeBuilt)
	{
		Color color = canBeBuilt ? Color.white : Color.red;
		SetColor(color);
		_floor.color = color;
		_hole.color = color;
	}

	private void SetBoostUnderlay()
	{
		CIGLandmarkBuilding cIGLandmarkBuilding = Building as CIGLandmarkBuilding;
		if (cIGLandmarkBuilding != null)
		{
			_boostUnderlay.gameObject.SetActive(value: true);
			_boostUnderlay.SetData(cIGLandmarkBuilding.Properties.Size.u + cIGLandmarkBuilding.BoostTiles * 2, cIGLandmarkBuilding.Properties.Size.v);
		}
		else
		{
			_boostUnderlay.gameObject.SetActive(value: false);
		}
	}

	private void OnIntroFinishedPlaying(Tweener tweener)
	{
		tweener.FinishedPlaying -= OnIntroFinishedPlaying;
		_idleTweener.Play();
	}

	private void OnFlipIntroFinishedPlaying(Tweener tweener)
	{
		base.Mirrored = !base.Mirrored;
		UpdateTransform();
		Vector3 localScale = new Vector3(_tileScale.x * Mathf.Sign(base.transform.localScale.x), _tileScale.y, _tileScale.z);
		int i = 0;
		for (int num = _buildingTileTransforms.Length; i < num; i++)
		{
			_buildingTileTransforms[i].localScale = localScale;
		}
		_flipOutroTweener.Play();
	}

	private void OnOutroFinishedPlaying(Tweener tweener)
	{
		tweener.FinishedPlaying -= OnOutroFinishedPlaying;
		_outroParticles.Play();
		_outroParticles.transform.parent = null;
		_outroParticlesRenderer.sortingOrder = Building.SpriteRenderer.sortingOrder;
		UnityEngine.Object.Destroy(_outroParticles.gameObject, _outroParticles.main.duration);
		_canHide = Building.CanHide;
		Building.CopyGridTile(this);
		Building.gameObject.SetActive(value: true);
		if (_outroTweenerFinishedCallback != null)
		{
			_outroTweenerFinishedCallback(Building);
			_outroTweenerFinishedCallback = null;
		}
		SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.DropBuilding);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
