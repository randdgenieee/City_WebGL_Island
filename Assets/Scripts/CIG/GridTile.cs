using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class GridTile : MonoBehaviour
	{
		public enum GridTileStatus
		{
			None,
			Preview,
			Moving,
			Created,
			Destroyed
		}

		[SerializeField]
		protected bool _canHide = true;

		[SerializeField]
		private int _orderOffset;

		[SerializeField]
		private bool _canShowOnReadOnlyGrid = true;

		[SerializeField]
		protected GridTileSpriteRenderer _gridTileSpriteRenderer;

		protected IsometricGrid _isometricGrid;

		protected IslandsManagerView _islandsManagerView;

		protected WorldMap _worldMap;

		protected BuildingWarehouseManager _buildingWarehouseManager;

		protected CraneManager _craneManager;

		protected GameStats _gameStats;

		protected GameState _gameState;

		protected PopupManager _popupManager;

		protected Timing _timing;

		protected Multipliers _multipliers;

		protected RoutineRunner _routineRunner;

		protected OverlayManager _overlayManager;

		protected CIGIslandState _islandState;

		private GridTileStatus _status;

		private bool _hidden;

		private GridIndex _index;

		private IEnumerator _pulseRoutine;

		private const string MirroredKey = "Mirrored";

		private const string UniqueIdentifierKey = "UniqueIdentifier";

		private StorageDictionary _storage;

		private bool _mirrored;

		public GridTileProperties Properties
		{
			get;
			private set;
		}

		public GridTileStatus Status
		{
			get
			{
				return _status;
			}
			set
			{
				if (value != _status)
				{
					GridTileStatus status = _status;
					_status = value;
					OnStatusChanged(status);
				}
			}
		}

		public string UniqueIdentifier
		{
			get;
			private set;
		}

		public bool CanHide => _canHide;

		public bool Mirrored
		{
			get
			{
				return _mirrored;
			}
			set
			{
				if (_mirrored != value)
				{
					_mirrored = value;
					OnMirroredChanged(_mirrored);
				}
			}
		}

		public GridIndex Index
		{
			get
			{
				return _index;
			}
			set
			{
				_index = value;
				UpdateTransform();
			}
		}

		public bool Hidden
		{
			get
			{
				return _hidden;
			}
			protected set
			{
				if (CanHide && value != _hidden)
				{
					_hidden = value;
					OnHiddenChanged(_hidden);
				}
			}
		}

		public GridPoint Middle
		{
			get
			{
				float u = (float)(Index.u + 1) - (float)Properties.Size.u * 0.5f;
				float v = (float)(Index.v + 1) - (float)Properties.Size.v * 0.5f;
				return new GridPoint(u, v);
			}
		}

		public GridElement Element => _isometricGrid[Index];

		public SpriteRenderer SpriteRenderer => _gridTileSpriteRenderer.SpriteRenderer;

		public List<ChildSpriteRenderer> ChildSpriteRenderers => _gridTileSpriteRenderer.ChildSpriteRenderers;

		public GridTileSpriteRenderer GridTileSpriteRenderer => _gridTileSpriteRenderer;

		public bool CanShowOnReadOnlyGrid => _canShowOnReadOnlyGrid;

		public virtual void Initialize(StorageDictionary storage, IsometricGrid isometricGrid, IslandsManagerView islandsManagerView, WorldMap worldMap, BuildingWarehouseManager buildingWarehouseManager, CraneManager craneManager, GameStats gameStats, GameState gameState, PopupManager popupManager, Multipliers multipliers, Timing timing, RoutineRunner routineRunner, GridTileProperties properties, OverlayManager overlayManager, CIGIslandState islandState, GridIndex? index = default(GridIndex?))
		{
			Properties = properties;
			_isometricGrid = isometricGrid;
			_islandsManagerView = islandsManagerView;
			_worldMap = worldMap;
			_buildingWarehouseManager = buildingWarehouseManager;
			_craneManager = craneManager;
			_gameStats = gameStats;
			_gameState = gameState;
			_popupManager = popupManager;
			_multipliers = multipliers;
			_timing = timing;
			_routineRunner = routineRunner;
			_overlayManager = overlayManager;
			_islandState = islandState;
			Deserialize(storage);
			if (index.HasValue)
			{
				_index = index.Value;
			}
			Builder.TilesHiddenChangedEvent += OnTilesHiddenChanged;
			Hidden = Builder.TilesHidden;
		}

		private void OnDisable()
		{
			TryStopPulseRoutine();
		}

		protected virtual void OnDestroy()
		{
			Builder.TilesHiddenChangedEvent -= OnTilesHiddenChanged;
		}

		public static int GetSortingOrder(GridIndex index, GridSize size)
		{
			return 10 * (index.u + index.v - size.u);
		}

		public static int GetSortingOrder(GridPoint point, GridSize size)
		{
			return GetSortingOrder(new GridIndex((int)point.U, (int)point.V), size);
		}

		public void DestroyTile()
		{
			Status = GridTileStatus.Destroyed;
			if (_isometricGrid.Tiles.Contains(this))
			{
				_isometricGrid.RemoveAtIndex(_index, Properties.Size, serialize: true);
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}

		public void CopyGridTile(GridTile otherTile)
		{
			Index = otherTile.Index;
			Mirrored = otherTile.Mirrored;
			Properties = otherTile.Properties;
			CopyGridTilePrefab(otherTile);
			OnHiddenChanged(Hidden);
		}

		public void CopyGridTilePrefab(GridTile otherTile)
		{
			_canHide = otherTile.CanHide;
			_orderOffset = otherTile._orderOffset;
			_gridTileSpriteRenderer.Copy(otherTile._gridTileSpriteRenderer);
		}

		public void UpdateTransform()
		{
			Vector3 localPosition = _isometricGrid[Index].Origin;
			localPosition.z = 0f;
			UpdateSortingOrder();
			Vector3 one = Vector3.one;
			if (Mirrored)
			{
				one.x = 0f - one.x;
			}
			base.transform.localPosition = localPosition;
			base.transform.localScale = one;
		}

		public virtual void UpdateSortingOrder()
		{
			int sortingOrder = GetSortingOrder(Index, Properties.Size) + _orderOffset + 1;
			SetSortingOrder(sortingOrder);
		}

		public void SetSortingOrder(int sortingOrder)
		{
			_gridTileSpriteRenderer.SetSortingOrder(sortingOrder);
		}

		public void SetColor(Color color)
		{
			_gridTileSpriteRenderer.SetColor(color);
		}

		public void SetMaterial(Material material)
		{
			_gridTileSpriteRenderer.SetMaterial(material);
		}

		public void PulseColor(Color color, float duration)
		{
			TryStopPulseRoutine();
			StartCoroutine(_pulseRoutine = PulseRoutine(color, duration));
			SetMaterial(SingletonMonobehaviour<MaterialAssetCollection>.Instance.GetAsset(MaterialType.SpriteAddColor));
		}

		public void TryStopPulseRoutine()
		{
			if (_pulseRoutine != null)
			{
				StopPulseRoutine();
			}
		}

		protected virtual void OnStatusChanged(GridTileStatus oldStatus)
		{
		}

		protected virtual void OnHiddenChanged(bool hidden)
		{
			_gridTileSpriteRenderer.SetHidden(hidden);
		}

		protected virtual void OnMirroredChanged(bool mirrored)
		{
		}

		private void StopPulseRoutine()
		{
			StopCoroutine(_pulseRoutine);
			_pulseRoutine = null;
			if (SingletonMonobehaviour<MaterialAssetCollection>.IsAvailable)
			{
				SetMaterial(SingletonMonobehaviour<MaterialAssetCollection>.Instance.GetAsset(MaterialType.SpriteTransparent));
			}
			SetColor(Color.white);
		}

		private IEnumerator PulseRoutine(Color toColor, float duration)
		{
			float time = 0f;
			float halfDuration = duration / 2f;
			while (time < duration)
			{
				time += _timing.GetDeltaTime(DeltaTimeType.Animation) * 0.5f;
				SetColor(Color.Lerp(Color.clear, toColor, Mathf.PingPong(time, halfDuration)));
				yield return null;
			}
			TryStopPulseRoutine();
		}

		private void OnTilesHiddenChanged(bool hidden)
		{
			Hidden = hidden;
		}

		public virtual StorageDictionary Serialize()
		{
			_storage.Set("Mirrored", Mirrored);
			_storage.Set("UniqueIdentifier", UniqueIdentifier);
			return _storage;
		}

		protected virtual void Deserialize(StorageDictionary storage)
		{
			_storage = storage;
			UniqueIdentifier = storage.Get("UniqueIdentifier", Guid.NewGuid().ToString());
			Mirrored = _storage.Get("Mirrored", defaultValue: false);
		}
	}
}
