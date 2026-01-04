using SparkLinq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class IsometricGrid : MonoBehaviour
	{
		public delegate void GridTileAddedEventHandler(GridTile tile);

		public delegate void GridTileRemovedEventHandler(GridTile tile);

		public static readonly Vector2 ElementSize = new Vector2(100f, 50f);

		[SerializeField]
		private ReadOnlyWrapper _readOnlyWrapper;

		private IslandsManagerView _islandsManagerView;

		private Properties _properties;

		private WorldMap _worldMap;

		private BuildingWarehouseManager _buildingWarehouseManager;

		private CraneManager _craneManager;

		private GameStats _gameStats;

		private GameState _gameState;

		private PopupManager _popupManager;

		private Multipliers _multipliers;

		private Timing _timing;

		private RoutineRunner _routineRunner;

		private OverlayManager _overlayManager;

		private CIGIslandState _islandState;

		private GridElement[,] _elements;

		private readonly Dictionary<GridIndex, GridTile> _tiles = new Dictionary<GridIndex, GridTile>();

		private readonly Dictionary<string, GridTile> _gridTiles = new Dictionary<string, GridTile>();

		private readonly HashSet<SurfaceType> _availableElementTypes = new HashSet<SurfaceType>();

		private const string PrefabNameKey = "PrefabName";

		private const string GridTileKey = "GridTile";

		private const string GridTileIndexKey = "Index";

		private const string MirroredKey = "Mirrored";

		private StorageDictionary _storage;

		public HashSet<SurfaceType> AvailableElementTypes => _availableElementTypes;

		public GridSize Size
		{
			get;
			private set;
		}

		public GridElement this[int u, int v] => _elements[u, v];

		public GridElement this[GridIndex index] => _elements[index.u, index.v];

		public GridOverlay GridOverlay
		{
			get;
			private set;
		}

		public ICollection<GridTile> Tiles => _tiles.Values;

		public Bounds IslandWorldBounds
		{
			get;
			private set;
		}

		public bool ReadOnlyGrid
		{
			get;
			private set;
		}

		public event GridTileAddedEventHandler GridTileAddedEvent;

		public event GridTileRemovedEventHandler GridTileRemovedEvent;

		private void FireGridTileAddedEvent(GridTile tile)
		{
			this.GridTileAddedEvent?.Invoke(tile);
		}

		private void FireGridTileRemovedEvent(GridTile tile)
		{
			this.GridTileRemovedEvent?.Invoke(tile);
		}

		public void Initialize(StorageDictionary storage, IslandSetup islandSetup, IslandsManagerView islandsManagerView, WorldMap worldMap, BuildingWarehouseManager buildingWarehouseManager, CraneManager craneManager, GameStats gameStats, GameState gameState, PopupManager popupManager, Multipliers multipliers, Timing timing, RoutineRunner routineRunner, Properties properties, OverlayManager overlayManager, GridOverlay gridOverlay, CIGIslandState islandState, bool readOnly)
		{
			GridOverlay = gridOverlay;
			ReadOnlyGrid = readOnly;
			Size = islandSetup.Size;
			IslandWorldBounds = islandSetup.IslandBounds;
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
			_properties = properties;
			_overlayManager = overlayManager;
			_islandState = islandState;
			GridOverlay.Initialize(islandSetup.Size, islandSetup.Offset, ReadOnlyGrid);
			_elements = new GridElement[Size.u, Size.v];
			for (int i = 0; i < Size.u; i++)
			{
				for (int j = 0; j < Size.v; j++)
				{
					GridIndex gridIndex = new GridIndex(i, j);
					SurfaceType surfaceType = islandSetup[i, j];
					GridElement gridElement = new GridElement(this, gridIndex, surfaceType);
					_elements[gridIndex.u, gridIndex.v] = gridElement;
					_availableElementTypes.Add(surfaceType);
				}
			}
			Deserialize(storage);
		}

		public GridIndex ConvertIslandCoordinateToGridIndex(Vector2 islandCoordinate, bool clamp = false)
		{
			return ConvertIslandCoordinateToGridIndex(base.transform.position, Size, islandCoordinate, clamp);
		}

		public static GridIndex ConvertIslandCoordinateToGridIndex(Vector3 gridPosition, GridSize gridSize, Vector2 islandCoordinate, bool clamp = false)
		{
			float num = gridPosition.y - islandCoordinate.y + 0.5f * (islandCoordinate.x - gridPosition.x);
			GridIndex gridIndex = default(GridIndex);
			gridIndex.u = Mathf.FloorToInt(num / ElementSize.y);
			if (clamp)
			{
				gridIndex.u = Mathf.Clamp(gridIndex.u, 0, gridSize.u - 1);
			}
			else if (gridIndex.u < 0 || gridIndex.u >= gridSize.u)
			{
				gridIndex.u = 0;
			}
			num = gridPosition.y - islandCoordinate.y - 0.5f * (islandCoordinate.x - gridPosition.x);
			gridIndex.v = Mathf.FloorToInt(num / ElementSize.y);
			if (clamp)
			{
				gridIndex.v = Mathf.Clamp(gridIndex.v, 0, gridSize.v - 1);
			}
			else if (gridIndex.v < 0 || gridIndex.v >= gridSize.v)
			{
				return GridIndex.invalid;
			}
			return gridIndex;
		}

		public static Vector2 GetPositionForGridPoint(GridPoint point)
		{
			return new Vector2((point.U - point.V) * ElementSize.x * 0.5f, (point.U + point.V) * ElementSize.y * -0.5f);
		}

		public Vector3 GetWorldPositionForGridPoint(GridPoint point)
		{
			return base.transform.position + (Vector3)GetPositionForGridPoint(point);
		}

		public static Vector2 GetPositionForGridIndex(GridIndex index)
		{
			return new Vector2((float)(index.u - index.v) * ElementSize.x * 0.5f, (float)(index.u + index.v) * ElementSize.y * -0.5f);
		}

		public Vector3 GetWorldPositionForGridIndex(GridIndex index)
		{
			return base.transform.position + (Vector3)GetPositionForGridIndex(index);
		}

		public static GridPoint GetGridPointForPosition(Vector2 position)
		{
			return new GridPoint(position.x / ElementSize.x + position.y / (0f - ElementSize.y), position.y / (0f - ElementSize.y) - position.x / ElementSize.x);
		}

		public GridPoint GetGridPointForWorldPosition(Vector3 position)
		{
			position -= base.transform.position;
			return GetGridPointForPosition(position);
		}

		public void AddTileAt(GridTile tile, GridIndex index, bool serialize)
		{
			if (!IsWithinBounds(index, tile.Properties.Size))
			{
				UnityEngine.Debug.LogWarningFormat("Index '{1}' for '{0}' is out of bounds", tile.name, index);
				return;
			}
			for (int num = index.v; num > index.v - tile.Properties.Size.v; num--)
			{
				for (int num2 = index.u; num2 > index.u - tile.Properties.Size.u; num2--)
				{
					_elements[num2, num].Tile = tile;
				}
			}
			_tiles[index] = tile;
			tile.transform.parent = base.transform;
			tile.Index = index;
			tile.Status = GridTile.GridTileStatus.Created;
			_gridTiles[tile.UniqueIdentifier] = tile;
			if (serialize)
			{
				Serialize();
			}
			FireGridTileAddedEvent(tile);
		}

		public void RemoveAtIndex(GridIndex index, GridSize size, bool serialize)
		{
			if (!_tiles.ContainsKey(index))
			{
				UnityEngine.Debug.LogWarning($"Tile does not exist at {index}.");
				return;
			}
			for (int num = index.v; num > index.v - size.v; num--)
			{
				for (int num2 = index.u; num2 > index.u - size.u; num2--)
				{
					_elements[num2, num].Tile = null;
				}
			}
			GridTile gridTile = _tiles[index];
			if (serialize)
			{
				_storage.Remove(index.ToString());
			}
			_gridTiles.Remove(gridTile.UniqueIdentifier);
			_tiles.Remove(index);
			FireGridTileRemovedEvent(gridTile);
		}

		public GridTile GetTileAt(GridIndex index)
		{
			int u = index.u;
			int v = index.v;
			if (u < 0 || u >= _elements.GetLength(0) || v < 0 || v >= _elements.GetLength(1))
			{
				return null;
			}
			return _elements[u, v].Tile;
		}

		public GridTile GetGridTileByUniqueIdentifier(string uniqueIdentifier)
		{
			_gridTiles.TryGetValue(uniqueIdentifier, out GridTile value);
			return value;
		}

		public GridTile FindRandomGridTile(Predicate<GridTile> predicate = null)
		{
			List<GridTile> list = new List<GridTile>();
			int i = 0;
			for (int length = _elements.GetLength(0); i < length; i++)
			{
				int j = 0;
				for (int length2 = _elements.GetLength(1); j < length2; j++)
				{
					GridElement gridElement = _elements[i, j];
					if (gridElement.Tile != null && (predicate == null || predicate(gridElement.Tile)))
					{
						list.Add(gridElement.Tile);
					}
				}
			}
			return list.PickRandom();
		}

		public List<GridTile> GetNeighbourTiles(GridTile tile)
		{
			GridIndex index = tile.Index;
			GridSize size = tile.Properties.Size;
			List<GridTile> list = new List<GridTile>();
			for (int num = index.v; num > index.v - size.v; num--)
			{
				int[] array = new int[2]
				{
					index.u + 1,
					index.u - size.u
				};
				foreach (int num2 in array)
				{
					GridIndex index2 = new GridIndex(num2, num);
					if (IsWithinBounds(index2))
					{
						GridElement gridElement = _elements[num2, num];
						if (gridElement.Tile != null)
						{
							list.Add(gridElement.Tile);
						}
					}
				}
			}
			for (int num3 = index.u; num3 > index.u - size.u; num3--)
			{
				int[] array = new int[2]
				{
					index.v + 1,
					index.v - size.v
				};
				foreach (int num4 in array)
				{
					GridIndex index3 = new GridIndex(num3, num4);
					if (IsWithinBounds(index3))
					{
						GridElement gridElement2 = _elements[num3, num4];
						if (gridElement2.Tile != null)
						{
							list.Add(gridElement2.Tile);
						}
					}
				}
			}
			return list;
		}

		public List<T> FindAll<T>() where T : GridTile
		{
			List<T> list = new List<T>();
			foreach (GridTile tile in Tiles)
			{
				T val = tile as T;
				if ((UnityEngine.Object)val != (UnityEngine.Object)null)
				{
					list.Add(val);
				}
			}
			return list;
		}

		public HashSet<T> FindInRange<T>(GridTile tile, int range) where T : GridTile
		{
			HashSet<T> hashSet = new HashSet<T>();
			int num = tile.Index.u + range;
			int num2 = tile.Index.v + range;
			int num3 = tile.Index.u - tile.Properties.Size.u - range + 1;
			int num4 = tile.Index.v - tile.Properties.Size.v - range + 1;
			for (int i = num3; i <= num; i++)
			{
				for (int j = num4; j <= num2; j++)
				{
					T val;
					if (IsWithinBounds(i, j) && (val = (_elements[i, j].Tile as T)) != null && val != tile)
					{
						hashSet.Add(val);
					}
				}
			}
			return hashSet;
		}

		public bool IsWithinBounds(GridIndex index, GridSize size)
		{
			for (int num = index.v; num > index.v - size.v; num--)
			{
				for (int num2 = index.u; num2 > index.u - size.u; num2--)
				{
					if (!IsWithinBounds(num2, num))
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool IsWithinBounds(GridIndex index)
		{
			if (!index.isInvalid && index.u < Size.u)
			{
				return index.v < Size.v;
			}
			return false;
		}

		public bool IsWithinBounds(int u, int v)
		{
			if (u >= 0 && v >= 0 && u < Size.u)
			{
				return v < Size.v;
			}
			return false;
		}

		public bool ElementTypeAvailable(SurfaceType surfaceType)
		{
			if (surfaceType != SurfaceType.AnyTypeOfLand)
			{
				return _availableElementTypes.Contains(surfaceType);
			}
			return true;
		}

		private bool IsAllowedReadOnly(GridTile tile)
		{
			return !(tile is Road);
		}

		public void Serialize()
		{
			if (!ReadOnlyGrid)
			{
				_storage.InternalDictionary.Clear();
				foreach (GridTile value in _tiles.Values)
				{
					string key = value.Index.ToString();
					StorageDictionary storageDictionary = new StorageDictionary();
					storageDictionary.Set("Index", value.Index);
					storageDictionary.Set("Mirrored", value.Mirrored);
					storageDictionary.Set("PrefabName", value.name);
					storageDictionary.Set("GridTile", value.Serialize());
					_storage.Set(key, storageDictionary);
				}
			}
		}

		private void Deserialize(StorageDictionary storage)
		{
			_storage = storage;
			foreach (KeyValuePair<string, object> item in _storage.InternalDictionary)
			{
				StorageDictionary storageDict = _storage.GetStorageDict(item.Key);
				string text = storageDict.Get("PrefabName", string.Empty);
				GridIndex model = storageDict.GetModel("Index", (StorageDictionary sd) => new GridIndex(sd), GridIndex.invalid);
				GridTile asset = SingletonMonobehaviour<BuildingsAssetCollection>.Instance.GetAsset(text);
				if (asset == null)
				{
					UnityEngine.Debug.LogError("No prefab found for " + text);
				}
				else if (asset.CanShowOnReadOnlyGrid || !ReadOnlyGrid)
				{
					GridTileProperties properties = _properties.GetProperties<GridTileProperties>(text);
					GridTile gridTile;
					if (ReadOnlyGrid && IsAllowedReadOnly(asset))
					{
						ReadOnlyWrapper readOnlyWrapper = UnityEngine.Object.Instantiate(_readOnlyWrapper, base.transform);
						readOnlyWrapper.Initialize(storageDict.GetStorageDict("GridTile"), asset, this, _islandsManagerView, _worldMap, _buildingWarehouseManager, _craneManager, _gameStats, _gameState, _popupManager, _multipliers, _timing, _routineRunner, _overlayManager, properties, _islandState);
						gridTile = readOnlyWrapper;
					}
					else
					{
						gridTile = UnityEngine.Object.Instantiate(asset, base.transform);
						gridTile.Initialize(storageDict.GetStorageDict("GridTile"), this, _islandsManagerView, _worldMap, _buildingWarehouseManager, _craneManager, _gameStats, _gameState, _popupManager, _multipliers, _timing, _routineRunner, properties, _overlayManager, _islandState, model);
					}
					gridTile.name = text;
					AddTileAt(gridTile, model, serialize: false);
				}
			}
		}
	}
}
