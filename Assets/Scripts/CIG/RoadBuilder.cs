using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CIG
{
	public class RoadBuilder : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		public delegate void RoadBuiltEventHandler(Road road);

		public delegate void RoadRemovedEventHandler(Road road);

		public delegate void RoadsAppliedEventHandler(int added, int removed);

		private static readonly Color32 PreviewColor = new Color32(128, 196, byte.MaxValue, byte.MaxValue);

		private static readonly Color32 RemoveColor = new Color32(byte.MaxValue, 128, 128, byte.MaxValue);

		private IsometricGrid _isometricGrid;

		private IslandInput _islandInput;

		private IslandsManagerView _islandsManagerView;

		private WorldMap _worldMap;

		private BuildingWarehouseManager _buildingWarehouseManager;

		private CraneManager _craneManager;

		private GameStats _gameStats;

		private GameState _gameState;

		private PopupManager _popupManager;

		private Multipliers _multipliers;

		private Timing _timing;

		private RoutineRunner _routineRunner;

		private Properties _properties;

		private CIGIslandState _islandState;

		private OverlayManager _overlayManager;

		private readonly Dictionary<GridIndex, Road> _roadsToRemove = new Dictionary<GridIndex, Road>();

		private readonly Dictionary<GridIndex, Road> _roadsToAdd = new Dictionary<GridIndex, Road>();

		private List<Road> _roads;

		private RoadType _currentRoadType;

		public bool IsBuildingRoad
		{
			get;
			private set;
		}

		public bool HasChanges
		{
			get
			{
				if (_roadsToRemove.Count <= 0)
				{
					return _roadsToAdd.Count > 0;
				}
				return true;
			}
		}

		public event RoadBuiltEventHandler RoadBuiltEvent;

		public event RoadRemovedEventHandler RoadRemovedEvent;

		public event RoadsAppliedEventHandler RoadsAppliedEvent;

		private void FireRoadBuiltEvent(Road road)
		{
			this.RoadBuiltEvent?.Invoke(road);
		}

		private void FireRoadRemovedEvent(Road road)
		{
			this.RoadRemovedEvent?.Invoke(road);
		}

		private void FireRoadsAppliedEvent(int added, int removed)
		{
			this.RoadsAppliedEvent?.Invoke(added, removed);
		}

		public void Initialize(IsometricGrid isometricGrid, IslandInput islandInput, IslandsManagerView islandsManagerView, WorldMap worldMap, BuildingWarehouseManager buildingWarehouseManager, CraneManager craneManager, GameStats gameStats, GameState gameState, PopupManager popupManager, Multipliers multipliers, Timing timing, RoutineRunner routineRunner, Properties properties, OverlayManager overlayManager, CIGIslandState islandState)
		{
			_isometricGrid = isometricGrid;
			_islandInput = islandInput;
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
			_roads = _isometricGrid.FindAll<Road>();
			UpdateAllRoadSprites();
			_isometricGrid.GridTileAddedEvent += OnTileAdded;
			_isometricGrid.GridTileRemovedEvent += OnTileRemoved;
		}

		private void OnDestroy()
		{
			if (_isometricGrid != null)
			{
				_isometricGrid.GridTileAddedEvent -= OnTileAdded;
				_isometricGrid.GridTileRemovedEvent -= OnTileRemoved;
				_isometricGrid = null;
			}
			if (_overlayManager != null)
			{
				_overlayManager.EnableInteractionRequest(this);
				_overlayManager = null;
			}
			if (_islandInput != null)
			{
				_islandInput.PopDisableIslandInteractionRequest(this);
			}
		}

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			if (IsBuildingRoad)
			{
				PlaceRoad(new GridIndex(_isometricGrid.GetGridPointForWorldPosition(eventData.pointerCurrentRaycast.worldPosition)));
			}
		}

		public void BeginRoad(RoadType roadType)
		{
			if (!IsBuildingRoad)
			{
				_overlayManager.DisableInteractionRequest(this);
				_islandInput.PushDisableIslandInteractionRequest(this);
			}
			IsBuildingRoad = true;
			_currentRoadType = roadType;
		}

		public void ApplyRoads()
		{
			int count = _roadsToAdd.Count;
			int count2 = _roadsToRemove.Count;
			foreach (KeyValuePair<GridIndex, Road> item in _roadsToRemove)
			{
				item.Value.DestroyTile();
			}
			_roadsToRemove.Clear();
			foreach (KeyValuePair<GridIndex, Road> item2 in _roadsToAdd)
			{
				BuildAt(item2.Value, item2.Key);
			}
			_roadsToAdd.Clear();
			UpdateAllRoadSprites();
			_isometricGrid.Serialize();
			StopBuilding();
			FireRoadsAppliedEvent(count, count2);
		}

		public void CancelRoads()
		{
			foreach (KeyValuePair<GridIndex, Road> item in _roadsToRemove)
			{
				SetRoadColor(item.Value, MaterialType.SpriteTransparent, Color.white);
			}
			_roadsToRemove.Clear();
			foreach (KeyValuePair<GridIndex, Road> item2 in _roadsToAdd)
			{
				item2.Value.DestroyTile();
			}
			_roadsToAdd.Clear();
			UpdateAllRoadSprites();
			StopBuilding();
		}

		public int GetRoadCount(Predicate<Road> predicate)
		{
			int num = 0;
			int i = 0;
			for (int count = _roads.Count; i < count; i++)
			{
				if (predicate(_roads[i]))
				{
					num++;
				}
			}
			return num;
		}

		public bool RoadExists(Predicate<Road> predicate)
		{
			int i = 0;
			for (int count = _roads.Count; i < count; i++)
			{
				if (predicate(_roads[i]))
				{
					return true;
				}
			}
			return false;
		}

		private void PlaceRoad(GridIndex index)
		{
			if (!_isometricGrid.IsWithinBounds(index))
			{
				return;
			}
			GridTile tile = _isometricGrid[index.u, index.v].Tile;
			Road road = tile as Road;
			bool flag = tile != null && road != null;
			Road value;
			bool flag2 = _roadsToAdd.TryGetValue(index, out value);
			Road value2;
			bool flag3 = _roadsToRemove.TryGetValue(index, out value2);
			if (_currentRoadType == RoadType.None)
			{
				if (flag2)
				{
					RemoveBuildPreview(index, value);
					RemoveDestroyPreview(index, value2);
					SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.Demolish, playOneAtATime: true);
				}
				else if (flag3)
				{
					RemoveDestroyPreview(index, value2);
					SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.PlaceRoad);
				}
				else if (flag)
				{
					ShowDestroyPreview(index, road);
					SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.Demolish, playOneAtATime: true);
				}
			}
			else if (flag2)
			{
				RoadType roadType = _currentRoadType ^ value.RoadType;
				if (roadType == RoadType.None || (flag && road.RoadType == roadType))
				{
					RemoveBuildPreview(index, value);
					RemoveDestroyPreview(index, value2);
					SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.Demolish, playOneAtATime: true);
				}
				else if (SingletonMonobehaviour<RoadPrefabsAssetCollection>.Instance.ContainsAsset(roadType))
				{
					RemoveBuildPreview(index, value);
					ShowBuildPreview(index, roadType, road);
					SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.PlaceRoad);
				}
			}
			else if (flag3)
			{
				if (value2.RoadType == _currentRoadType)
				{
					RemoveDestroyPreview(index, value2);
					SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.PlaceRoad);
				}
				else
				{
					ShowBuildPreview(index, _currentRoadType, road);
					SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.PlaceRoad);
				}
			}
			else if (!flag)
			{
				ShowBuildPreview(index, _currentRoadType, null);
				SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.PlaceRoad);
			}
			else
			{
				RoadType roadType2 = _currentRoadType ^ road.RoadType;
				ShowDestroyPreview(index, road);
				ShowBuildPreview(index, roadType2, road);
				SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.PlaceRoad);
			}
		}

		private void StopBuilding()
		{
			_currentRoadType = RoadType.None;
			IsBuildingRoad = false;
			_overlayManager.EnableInteractionRequest(this);
			_islandInput.PopDisableIslandInteractionRequest(this);
		}

		private void ShowBuildPreview(GridIndex index, RoadType roadType, Road existingRoad)
		{
			if (!_properties.HasProperties(roadType.ToString()))
			{
				return;
			}
			GridTileProperties properties = _properties.GetProperties<GridTileProperties>(roadType.ToString());
			if (CanBuildTile(properties, index, preview: true))
			{
				Road road = (Road)SingletonMonobehaviour<BuildingsAssetCollection>.Instance.GetAsset(properties.BaseKey);
				Road road2 = UnityEngine.Object.Instantiate(road, _isometricGrid.transform);
				road2.Initialize(new StorageDictionary(), _isometricGrid, _islandsManagerView, _worldMap, _buildingWarehouseManager, _craneManager, _gameStats, _gameState, _popupManager, _multipliers, _timing, _routineRunner, properties, _overlayManager, _islandState);
				road2.name = road.name;
				road2.Index = index;
				SetRoadColor(road2, MaterialType.SpriteGreyscaleTint, PreviewColor);
				_roadsToAdd.Add(index, road2);
				OnRoadsChanged(road2);
				if (existingRoad != null)
				{
					SetRoadColor(existingRoad, MaterialType.SpriteGreyscaleTint, Color.clear);
				}
			}
		}

		private void RemoveBuildPreview(GridIndex index, Road previewRoad)
		{
			previewRoad.DestroyTile();
			_roadsToAdd.Remove(index);
			OnRoadsChanged(previewRoad);
		}

		private void ShowDestroyPreview(GridIndex index, Road existingRoad)
		{
			if (existingRoad != null)
			{
				_roadsToRemove.Add(index, existingRoad);
				SetRoadColor(existingRoad, MaterialType.SpriteGreyscaleTint, RemoveColor);
			}
		}

		private void RemoveDestroyPreview(GridIndex index, Road existingRoad)
		{
			if (existingRoad != null)
			{
				_roadsToRemove.Remove(index);
				SetRoadColor(existingRoad, MaterialType.SpriteTransparent, Color.white);
			}
		}

		private void SetRoadColor(Road road, MaterialType material, Color color)
		{
			road.SetMaterial(SingletonMonobehaviour<MaterialAssetCollection>.Instance.GetAsset(material));
			road.SetColor(color);
		}

		private void BuildAt(Road clone, GridIndex index)
		{
			if (CanBuildTile(clone.Properties, index, preview: false))
			{
				_isometricGrid.AddTileAt(clone, index, serialize: false);
				SetRoadColor(clone, MaterialType.SpriteTransparent, Color.white);
			}
			else
			{
				clone.DestroyTile();
			}
		}

		private bool CanBuildTile(GridTileProperties properties, GridIndex index, bool preview)
		{
			if (!_isometricGrid.IsWithinBounds(index, properties.Size))
			{
				return false;
			}
			GridSize size = properties.Size;
			for (int num = index.v; num > index.v - size.v; num--)
			{
				for (int num2 = index.u; num2 > index.u - size.u; num2--)
				{
					GridElement gridElement = _isometricGrid[num2, num];
					if (!CanBuildTileOnElement(properties, gridElement) || (preview && gridElement.Tile != null && !(gridElement.Tile is Road)) || (!preview && gridElement.Tile != null))
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool IsElementTypeCompatible(SurfaceType type, SurfaceType required)
		{
			if (type != required)
			{
				if (required == SurfaceType.AnyTypeOfLand)
				{
					return type.IsLand();
				}
				return false;
			}
			return true;
		}

		private bool CanBuildTileOnElement(GridTileProperties properties, GridElement element)
		{
			if (element.Unlocked)
			{
				return IsElementTypeCompatible(element.Type, properties.SurfaceType);
			}
			return false;
		}

		private void UpdateAllRoadSprites()
		{
			int i = 0;
			for (int count = _roads.Count; i < count; i++)
			{
				Road road = _roads[i];
				road.UpdateNeighbours(FindNeighboursForRoad(road));
			}
		}

		private Dictionary<Direction, GridTile> FindNeighboursForRoad(Road road)
		{
			return new Dictionary<Direction, GridTile>
			{
				[Direction.NE] = FindNeighbourInDirection(road.Index, Direction.NE),
				[Direction.SE] = FindNeighbourInDirection(road.Index, Direction.SE),
				[Direction.SW] = FindNeighbourInDirection(road.Index, Direction.SW),
				[Direction.NW] = FindNeighbourInDirection(road.Index, Direction.NW)
			};
		}

		private GridTile FindNeighbourInDirection(GridIndex index, Direction direction)
		{
			GridIndex neighbour = index.GetNeighbour(direction);
			GridTile result = null;
			if (_isometricGrid.IsWithinBounds(neighbour))
			{
				result = ((!_roadsToAdd.TryGetValue(neighbour, out Road value)) ? _isometricGrid[neighbour].Tile : value);
			}
			return result;
		}

		private void NotifyBuildingsOfRoadChange(Road road, Dictionary<Direction, GridTile> neighbours)
		{
			if (road != null)
			{
				foreach (KeyValuePair<Direction, GridTile> neighbour in neighbours)
				{
					Building building = neighbour.Value as Building;
					if (building != null)
					{
						building.OnAdjacentRoadsChanged(road);
					}
				}
			}
		}

		private void NotifyRoadsOfRoadChange(Road road, Dictionary<Direction, GridTile> neighbours)
		{
			road.UpdateNeighbours(neighbours);
			foreach (KeyValuePair<Direction, GridTile> neighbour in neighbours)
			{
				Road road2 = neighbour.Value as Road;
				if (road2 != null)
				{
					road2.UpdateNeighbours(FindNeighboursForRoad(road2));
				}
			}
		}

		private void OnRoadsChanged(Road road)
		{
			Dictionary<Direction, GridTile> neighbours = FindNeighboursForRoad(road);
			NotifyRoadsOfRoadChange(road, neighbours);
			NotifyBuildingsOfRoadChange(road, neighbours);
		}

		private void OnTileAdded(GridTile gridTile)
		{
			Road road = gridTile as Road;
			if (road != null)
			{
				_roads.Add(road);
				OnRoadsChanged(road);
				FireRoadBuiltEvent(road);
			}
		}

		private void OnTileRemoved(GridTile gridTile)
		{
			Road road = gridTile as Road;
			if (road != null)
			{
				_roads.Remove(road);
				OnRoadsChanged(road);
				FireRoadRemovedEvent(road);
			}
		}
	}
}
